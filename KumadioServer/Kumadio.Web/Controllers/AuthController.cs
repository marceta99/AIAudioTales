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

namespace Kumadio.Web.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly AppSettings _appSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDtoMapper<DTORegister, User> _registerMapper;
        private readonly IDtoMapper<DTORegisterCreator, User> _registerCreatorMapper;
        private readonly IDtoMapper<User, DTOReturnUser> _returnUserMapper;

        public AuthController(
            IAuthService authService,
            IOptions<AppSettings> appSettings,
            IHttpContextAccessor httpContextAccessor,
            IDtoMapper<DTORegister, User> registerMapper,
            IDtoMapper<DTORegisterCreator, User> registerCreatorMapper,
            IDtoMapper<User, DTOReturnUser> returnUserMapper)
        {
            _authService = authService;
            _appSettings = appSettings.Value;
            _httpContextAccessor = httpContextAccessor;
            _registerMapper = registerMapper;
            _registerCreatorMapper = registerCreatorMapper;
            _returnUserMapper = returnUserMapper;
        }

        #region GET

        [HttpGet("refresh-web")]
        public async Task<IActionResult> RefreshWeb()
        {
            var refreshTokenHash = _httpContextAccessor.HttpContext?.Request.Cookies["X-Refresh-Token"];
            if (string.IsNullOrEmpty(refreshTokenHash)) return DomainErrors.Auth.RefreshTokenMissing.ToBadRequest();

            var refreshTokenResult = await _authService.GetRefreshToken(refreshTokenHash);
            if (refreshTokenResult.IsFailure) return refreshTokenResult.Error.ToBadRequest();

            var refreshToken = refreshTokenResult.Value;
            if (refreshToken.Expires < DateTime.Now)
            {
                // If expired, revoke and force re-login
                var revokeTokenResult = await RevokeTokenHelper();
                if (revokeTokenResult.IsFailure) return revokeTokenResult.Error.ToBadRequest();

                return DomainErrors.Auth.RefreshTokenExpired.ToBadRequest();
            }

            var userResult = await _authService.GetUserWithRefreshToken(refreshToken);
            if (userResult.IsFailure) return userResult.Error.ToBadRequest();

            var user = userResult.Value;

            var accessToken = GenerateJwt(user);
            if (String.IsNullOrEmpty(accessToken))
                return BadRequest("Problem with refresh");

            var newRefreshToken = GenerateRefreshToken();

            SetJwtCookie(accessToken);
            var setRefreshResult = await SetRefreshTokenCookie(newRefreshToken, user);
            if (setRefreshResult.IsFailure)
                return setRefreshResult.Error.ToBadRequest();

            return Ok("Refresh successful");
        }

        [HttpGet("refresh-mobile")]
        public async Task<IActionResult> RefreshMobile()
        {
            var refreshTokenHash = _httpContextAccessor.HttpContext?.Request.Cookies["X-Refresh-Token"];
            if (string.IsNullOrEmpty(refreshTokenHash)) return DomainErrors.Auth.RefreshTokenMissing.ToBadRequest();

            var refreshTokenResult = await _authService.GetRefreshToken(refreshTokenHash);
            if (refreshTokenResult.IsFailure) return refreshTokenResult.Error.ToBadRequest();

            var refreshToken = refreshTokenResult.Value;
            if (refreshToken.Expires < DateTime.Now)
            {
                // If expired, revoke and force re-login
                var revokeTokenResult = await RevokeTokenHelper();
                if (revokeTokenResult.IsFailure) return revokeTokenResult.Error.ToBadRequest();

                return DomainErrors.Auth.RefreshTokenExpired.ToBadRequest();
            }

            var userResult = await _authService.GetUserWithRefreshToken(refreshToken);
            if (userResult.IsFailure) return userResult.Error.ToBadRequest();

            var user = userResult.Value;

            var newAccessToken = GenerateJwt(user);
            if (String.IsNullOrEmpty(newAccessToken))
                return BadRequest("Problem with refresh");

            var newRefreshToken = GenerateRefreshToken();

            return Ok(new
            {
                accessToken = newAccessToken,
                refreshToken = newRefreshToken
            });
        }

        [HttpGet("current-user")]
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

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] DTORegister model)
        {
            var user = _registerMapper.Map(model);

            var result = await _authService.Register(user!, model.Password);
            if (result.IsFailure) return result.Error!.ToBadRequest();

            return MessageResponse.Ok("Successful registration");
        }

        [HttpPost("register-creator")]
        public async Task<IActionResult> RegisterCreator([FromBody] DTORegisterCreator model)
        {
            var user = _registerCreatorMapper.Map(model);

            var registerCreatorResult = await _authService.RegisterCreator(user!, model.Password);
            if (registerCreatorResult.IsFailure) return registerCreatorResult.Error.ToBadRequest();

            return MessageResponse.Ok("Successful registration");
        }

        [HttpPost("login-mobile")]
        public async Task<IActionResult> LoginMobile([FromBody] DTOLogin model)
        {
            var loginResult = await _authService.Login(model.Email, model.Password);
            if (loginResult.IsFailure) return loginResult.Error.ToBadRequest();

            var user = loginResult.Value;

            var accessToken = GenerateJwt(user);
            if (String.IsNullOrEmpty(accessToken))
                return BadRequest("Problem with login");

            var refreshToken = GenerateRefreshToken();

            return Ok(new { accessToken, refreshToken });
        }

        [HttpPost("login-web")]
        public async Task<IActionResult> LoginWeb([FromBody] DTOLogin model)
        {
            var loginResult = await _authService.Login(model.Email, model.Password);
            if (loginResult.IsFailure)
                return loginResult.Error.ToBadRequest();

            var user = loginResult.Value;

            var accessToken = GenerateJwt(user);
            if (String.IsNullOrEmpty(accessToken))
                return BadRequest("Problem with login"); 

            var refreshToken = GenerateRefreshToken();

            SetJwtCookie(accessToken);
            var setRefreshResult = await SetRefreshTokenCookie(refreshToken, user);
            if (setRefreshResult.IsFailure)
                return setRefreshResult.Error.ToBadRequest();

            return MessageResponse.Ok("Successful login");
        }

        [HttpPost("google-login")]
        public async Task<ActionResult<DTOReturnUser>> GoogleLogin([FromBody] string credentials)
        {
            /*var settings = new GoogleJsonWebSignature.ValidationSettings
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

            return Ok(_returnUserMapper.Map(user));*/
            return Ok();
        }

        [HttpPost("logout-web")]
        public async Task<IActionResult> LogoutWeb()
        {
            // read refresh cookie
            var refreshTokenHash = _httpContextAccessor.HttpContext?.Request.Cookies["X-Refresh-Token"];

            if (!string.IsNullOrEmpty(refreshTokenHash))
            {
                //remove the token
                await _authService.DeleteRefreshToken(refreshTokenHash);
            }

            // set cookies to expire
            Response.Cookies.Append("X-ACcess-Token", "", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddDays(-1)
            });
            Response.Cookies.Append("X-Refresh-Token", "", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddDays(-1)
            });

            return MessageResponse.Ok("Logged out");
        }

        [HttpPost("logout-mobile")]
        public async Task<IActionResult> LogoutMobile([FromBody] DTOLogout dto)
        {
            // we remove the refresh token from the user's store so it can't be used again
            var deleteRefreshResult = await _authService.DeleteRefreshToken(dto.RefreshToken);
            if (deleteRefreshResult.IsFailure) return deleteRefreshResult.Error.ToBadRequest();

            return MessageResponse.Ok("Logged out");
        }


        #endregion

        #region DELETE

        [HttpDelete("revoke-token")]
        public async Task<IActionResult> RevokeToken()
        {
            var revokeResult = await RevokeTokenHelper();

            if (revokeResult.IsFailure) return revokeResult.Error.ToBadRequest();

            return Ok();
        }

        #endregion

        #region Private Helper Methods
        private async Task<Result> RevokeTokenHelper()
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
        private string? GenerateJwt(User user)
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

            return encryptedToken;
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
