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
using AutoMapper;
using Kumadio.Domain.Entities;
using Kumadio.Web.DTOS;
using Kumadio.Web.Settings;

namespace AIAudioTalesServer.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthController(IAuthService authService, IMapper mapper, IOptions<AppSettings> appSettings, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _authService = authService;
            _appSettings = appSettings.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] DTORegister model)
        {
            var user = _mapper.Map<User>(model);
            var result = await _authService.RegisterAsync(user, model.Password);

            if (result == 0) return BadRequest("Problem with registering user");

            return Ok("User successfully registered");
        }

        [HttpPost("RegisterCreator")]
        public async Task<IActionResult> RegisterCreator([FromBody] DTORegisterCreator model)
        {
            var user = _mapper.Map<User>(model);
            var result = await _authService.RegisterCreatorAsync(user, model.Password);

            if (result == 0) return BadRequest("User with that email already exists");
            
            return Ok();
        }

        [HttpPost("Login")]
        public async Task<ActionResult<DTOReturnUser>> Login([FromBody] DTOLogin model)
        {
            var user = await _authService.ValidateLogin(model.Email, model.Password);

            if (user == null) return BadRequest("User name or password was invalid");

            await GenerateJwt(user);

            return Ok(_mapper.Map<DTOReturnUser>(user));
        }

        [HttpGet("RefreshToken")]
        public async Task<ActionResult> RefreshToken()
        {
            try
            {
                var refreshToken = _httpContextAccessor.HttpContext?.Request.Cookies["X-Refresh-Token"];
                if (string.IsNullOrEmpty(refreshToken)) return BadRequest();

                var tokenInDb = await _authService.GetRefreshToken(refreshToken);

                if (tokenInDb.Expires < DateTime.Now)
                {
                    // If expired, revoke and force re-login
                    await RevokeTokenAsync();
                    return BadRequest("Refresh token has expired.");
                }

                var user = await _authService.GetUserWithRefreshToken(tokenInDb);

                await GenerateJwt(user);

                return Ok();
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpDelete("RevokeToken")]
        public async Task<IActionResult> RevokeToken()
        {
            await RevokeTokenAsync();
            return Ok();
        }

        [HttpPost("LoginWithGoogle")]
        public async Task<ActionResult<DTOReturnUser>> LoginWithGoogle([FromBody] string credentials)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new List<string> { _appSettings.GoogleClientId }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(credentials, settings);
            if (payload == null) return BadRequest();

            var user = await _authService.GetUserWithEmail(payload.Email);
            if (user == null) return BadRequest();

            await GenerateJwt(user);

            var dtoUser = _mapper.Map<DTOReturnUser>(user);
            return Ok(dtoUser);
        }

        [HttpGet("GetCurrentUser")]
        public async Task<ActionResult<DTOReturnUser>> GetCurrentUser()
        {
            var jwtTokenCookie = _httpContextAccessor.HttpContext?.Request.Cookies["X-Access-Token"];
            if (string.IsNullOrEmpty(jwtTokenCookie)) return BadRequest();

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(jwtTokenCookie);

            var emailClaim = token.Claims.FirstOrDefault(c => c.Type == "email");
            if (emailClaim == null) return BadRequest();

            var email = emailClaim.Value;

            var user = await _authService.GetUserWithEmail(email);
            if (user == null) return BadRequest();

            var dtoUser = _mapper.Map<DTOReturnUser>(user);
            return Ok(dtoUser);
        }


        #region Private Helper Methods
        private async Task RevokeTokenAsync()
        {
            var jwtTokenCookie = _httpContextAccessor.HttpContext?.Request.Cookies["X-Access-Token"];
            if (string.IsNullOrEmpty(jwtTokenCookie))
                return; // no token to revoke

            // Decode the JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(jwtTokenCookie);

            var emailClaim = token.Claims.FirstOrDefault(c => c.Type == "email");
            if (emailClaim != null)
            {
                var email = emailClaim.Value;
                await _authService.DeleteRefreshTokenForUser(email);
            }

            // Optionally, clear cookies
            var response = _httpContextAccessor.HttpContext?.Response;
            response?.Cookies.Delete("X-Access-Token");
            response?.Cookies.Delete("X-Refresh-Token");
        }
        private async Task GenerateJwt(User user)
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
            await SetRefreshTokenCookie(refreshToken, user);
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
        private async Task SetRefreshTokenCookie(RefreshToken refreshToken, User user)
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

            await _authService.SaveRefreshToken(refreshToken, user);
        }
        
        #endregion
    }

}
