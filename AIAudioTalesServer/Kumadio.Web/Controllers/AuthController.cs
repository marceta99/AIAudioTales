using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Google.Apis.Auth;
using Kumadio.Core.Interfaces;
using Kumadio.Web.DTOS.Auth;
using Kumadio.Domain.Entities;
using Kumadio.Web.DTOS;
using Kumadio.Web.Settings;
using Kumadio.Web.Mappers.Base;
using Kumadio.Core.Common;
using Kumadio.Web.Common;

namespace AIAudioTalesServer.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly AppSettings _appSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper<DTORegister, User> _registerMapper;
        private readonly IMapper<DTORegisterCreator, User> _registerCreatorMapper;
        private readonly IMapper<User, DTOReturnUser> _returnUserMapper;

        public AuthController(
            IAuthService authService,
            IOptions<AppSettings> appSettings,
            IHttpContextAccessor httpContextAccessor,
            IMapper<DTORegister, User> registerMapper, 
            IMapper<DTORegisterCreator, User> registerCreatorMapper,
            IMapper<User, DTOReturnUser> returnUserMapper)
        {
            _authService = authService;
            _appSettings = appSettings.Value;
            _httpContextAccessor = httpContextAccessor;
            _registerMapper = registerMapper;
            _registerCreatorMapper = registerCreatorMapper;
            _returnUserMapper = returnUserMapper;
        }

        #region GET

        [HttpGet("RefreshToken")]
        public async Task<ActionResult> RefreshToken()
        {
            var refreshTokenHash = _httpContextAccessor.HttpContext?.Request.Cookies["X-Refresh-Token"];
            if (string.IsNullOrEmpty(refreshTokenHash)) return DomainErrors.Auth.RefreshTokenMissing.ToBadRequest();

            var refreshTokenResult = await _authService.GetRefreshToken(refreshTokenHash);
            if (refreshTokenResult.IsFailure) return refreshTokenResult.Error.ToBadRequest();

            var refreshToken = refreshTokenResult.Value;
            if (refreshToken.Expires < DateTime.Now)
            {
                // If expired, revoke and force re-login
                var revokeTokenResult = await RevokeTokenAsync();
                if (revokeTokenResult.IsFailure) return revokeTokenResult.Error.ToBadRequest();

                return DomainErrors.Auth.RefreshTokenExpired.ToBadRequest();
            }

            var userResult = await _authService.GetUserWithRefreshToken(refreshToken);
            if (userResult.IsFailure) return userResult.Error.ToBadRequest();

            var generateJwtResult = await GenerateJwt(userResult.Value);
            if (generateJwtResult.IsFailure) return generateJwtResult.Error.ToBadRequest();

            return Ok();
        }

        [HttpGet("GetCurrentUser")]
        public async Task<ActionResult<DTOReturnUser>> GetCurrentUser()
        {
            var jwtToken = _httpContextAccessor.HttpContext?.Request.Cookies["X-Access-Token"];
            if (string.IsNullOrEmpty(jwtToken)) return DomainErrors.Auth.JwtTokenMissing.ToBadRequest();

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(jwtToken);

            var emailClaim = token.Claims.FirstOrDefault(c => c.Type == "email");
            if (emailClaim == null) return DomainErrors.Auth.EmailClaimMissing.ToBadRequest();

            var email = emailClaim.Value;
            var result = await _authService.GetUserWithEmail(email);
            if (result.IsFailure) return result.Error.ToBadRequest();

            return Ok(_returnUserMapper.Map(result.Value));
        }

        #endregion

        #region POST

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] DTORegister model)
        {
            var user = _registerMapper.Map(model);

            var result = await _authService.Register(user!, model.Password);
            if (result.IsFailure) return result.Error!.ToBadRequest();

            return Ok();
        }

        [HttpPost("RegisterCreator")]
        public async Task<IActionResult> RegisterCreator([FromBody] DTORegisterCreator model)
        {
            var user = _registerCreatorMapper.Map(model);

            var registerCreatorResult = await _authService.RegisterCreator(user!, model.Password);
            if (registerCreatorResult.IsFailure) return registerCreatorResult.Error.ToBadRequest();

            return Ok();
        }

        [HttpPost("Login")]
        public async Task<ActionResult<DTOReturnUser>> Login([FromBody] DTOLogin model)
        {
            var loginResult = await _authService.Login(model.Email, model.Password);
            if (loginResult.IsFailure) return loginResult.Error.ToBadRequest();

            var user = loginResult.Value;
            var generateJwtResult = await GenerateJwt(user);
            if (generateJwtResult.IsFailure) return generateJwtResult.Error.ToBadRequest();

            return Ok(_returnUserMapper.Map(user));
        }

        [HttpPost("LoginWithGoogle")]
        public async Task<ActionResult<DTOReturnUser>> LoginWithGoogle([FromBody] string credentials)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new List<string> { _appSettings.GoogleClientId }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(credentials, settings);
            if (payload == null) return DomainErrors.Auth.GoogleCredentialsNotValid.ToBadRequest();

            var resultGetUser = await _authService.GetUserWithEmail(payload.Email);
            if (resultGetUser.IsFailure) return resultGetUser.Error.ToBadRequest();

            var user = resultGetUser.Value;
            var generateJwtResult = await GenerateJwt(user);
            if (generateJwtResult.IsFailure) return generateJwtResult.Error.ToBadRequest();

            return Ok(_returnUserMapper.Map(user));
        }

        #endregion

        #region DELETE

        [HttpDelete("RevokeToken")]
        public async Task<IActionResult> RevokeToken()
        {
            var revokeResult = await RevokeTokenAsync();

            if (revokeResult.IsFailure) return revokeResult.Error.ToBadRequest();

            return Ok();
        }

        #endregion

        #region Private Helper Methods
        private async Task<Result> RevokeTokenAsync()
        {
            var jwtToken = _httpContextAccessor.HttpContext?.Request.Cookies["X-Access-Token"];
            if (string.IsNullOrEmpty(jwtToken)) return DomainErrors.Auth.JwtTokenMissing;

            // Decode the JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(jwtToken);

            var emailClaim = token.Claims.FirstOrDefault(c => c.Type == "email");
            if (emailClaim != null)
            {
                var email = emailClaim.Value;
                var deleteTokenResult = await _authService.DeleteRefreshTokenForUser(email);

                if (deleteTokenResult.IsFailure) return deleteTokenResult;
            }

            // Optionally, clear cookies
            var response = _httpContextAccessor.HttpContext?.Response;
            response?.Cookies.Delete("X-Access-Token");
            response?.Cookies.Delete("X-Refresh-Token");

            return Result.Success();
        }
        private async Task<Result> GenerateJwt(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("email", user.Email),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var encryptedToken = tokenHandler.WriteToken(token);

            SetJwtCookie(encryptedToken);

            // Generate & set refresh token
            var refreshToken = GenerateRefreshToken();
            return await SetRefreshTokenCookie(refreshToken, user);
        }
        private void SetJwtCookie(string tokenValue)
        {
            var response = _httpContextAccessor.HttpContext?.Response;
            response?.Cookies.Append("X-Access-Token", tokenValue, new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(5),
                HttpOnly = true,
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None,
                Path = "/"
            });
        }
        private RefreshToken GenerateRefreshToken()
        {
            return new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Created = DateTime.Now,
                Expires = DateTime.Now.AddDays(7)
            };
        }
        private async Task<Result> SetRefreshTokenCookie(RefreshToken refreshToken, User user)
        {
            var response = _httpContextAccessor.HttpContext?.Response;
            response?.Cookies.Append("X-Refresh-Token", refreshToken.Token, new CookieOptions
            {
                Expires = refreshToken.Expires,
                HttpOnly = true,
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None,
                Path = "/"
            });

            return await _authService.SaveRefreshToken(refreshToken, user);
        }
        
        #endregion
    }

}
