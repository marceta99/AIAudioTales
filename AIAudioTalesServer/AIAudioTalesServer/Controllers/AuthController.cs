using AIAudioTalesServer.Models.Auth;
using AIAudioTalesServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using AIAudioTalesServer.Models.Enums;
using AIAudioTalesServer.Data.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Google.Apis.Auth;
using AIAudioTalesServer.Data.Repositories;

namespace AIAudioTalesServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        private readonly AppSettings _applicationSettings;
        public AuthController(IAuthRepository authRepository, IOptions<AppSettings> applicationSettings)
        {
            _authRepository = authRepository;
            _applicationSettings = applicationSettings.Value;
        }
        
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] Register model)
        {
            var user = new User() { Email = model.Email, Role = Role.LISTENER };

            if (model.ConfirmPassword == model.Password)
            {
                using (HMACSHA512? hmac = new HMACSHA512())
                {
                    user.PasswordSalt = hmac.Key;
                    user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(model.Password));
                }
            }
            else
            {
                return BadRequest("Passwords don't match");
            }
            var result = await _authRepository.AddNewUser(user);

            if (result == 0) return BadRequest("User with that user name already exists");

            return Ok(user);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] Login model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Bad Request");
            }
            var userNameExists = await _authRepository.IsEmailUsed(model.Email);

            if (userNameExists == false)
            {
                return BadRequest("User name or password was invalid");
            }
            var user = await _authRepository.GetUserWithEmail(model.Email);
            var match = _authRepository.CheckPassword(model.Password, user);

            if (!match)
            {
                return BadRequest("User name or password was invalid");
            }
            JwtGenerator(user);
            return Ok(new { email = user.Email, role = user.Role.ToString() });
        }

        //this metod will generate new jwt token, when current jwt token expires
        [HttpGet("RefreshToken")]
        public async Task<ActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["X-Refresh-Token"];

            var token = await _authRepository.GetRefreshToken(refreshToken);

            if (token == null || token.Expires < DateTime.Now)
            {
                return Unauthorized("Token has expired");
            }
            var userWithThatRefreshToken = await _authRepository.GetUserWithRefreshToken(token);
            JwtGenerator(userWithThatRefreshToken);

            return Ok();
        }

        //this metod will be called as the last method to delete current refresh token on that user
        //if refresh token expires or refresh token is invalid, and user will have to login again 
        //to get new refresh token
        [HttpDelete("RevokeToken")]
        public async Task<IActionResult> RevokeToken()
        {
            // Get the JWT token cookie
            var jwtTokenCookie = Request.Cookies["X-Access-Token"];

            if (!string.IsNullOrEmpty(jwtTokenCookie))
            {
                // Decode the JWT token
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.ReadJwtToken(jwtTokenCookie);

                // Access custom claim "userName"
                var emailClaim = token.Claims.FirstOrDefault(c => c.Type == "email");

                if (emailClaim != null)
                {
                    var email = emailClaim.Value;

                    var user = await _authRepository.GetUserWithEmail(email);
                    if (user == null) return BadRequest();

                    await _authRepository.DeleteRefreshTokenForUser(user.Email);
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost("LoginWithGoogle")]
        public async Task<IActionResult> LoginWithGoogle([FromBody] string credentials)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new List<string> { this._applicationSettings.GoogleClientId }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(credentials, settings);

            var user = await _authRepository.GetUserWithEmail(payload.Email);
            
            if (user != null)
            {
                JwtGenerator(user);
                return Ok(new {email = user.Email, role = user.Role.ToString() });
            }
            else
            {
                return BadRequest();
            }
        }

        private dynamic JwtGenerator(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(this._applicationSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim("email", user.Email),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var encripterToken = tokenHandler.WriteToken(token);

            SetJwt(encripterToken);

            var refreshToken = GenerateRefreshToken();

            SetRefreshToken(refreshToken, user);

            return new { token = encripterToken, email = user.Email };
        }
        private void SetJwt(string encriptedToken)
        {
            HttpContext.Response.Cookies.Append("X-Access-Token", encriptedToken,
                new CookieOptions
                {
                    Expires = DateTime.Now.AddMinutes(5),
                    HttpOnly = true,
                    Secure = true,
                    IsEssential = true,
                    SameSite = SameSiteMode.None
                });
        }

        private RefreshToken GenerateRefreshToken()
        {
            var refreshToken = new RefreshToken()
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Created = DateTime.Now,
                Expires = DateTime.Now.AddMinutes(2)
            };

            return refreshToken;
        }

        private void SetRefreshToken(RefreshToken refreshToken, User user)
        {
            HttpContext.Response.Cookies.Append("X-Refresh-Token", refreshToken.Token,
                new CookieOptions
                {
                    Expires = refreshToken.Expires,
                    HttpOnly = true,
                    Secure = true,
                    IsEssential = true,
                    SameSite = SameSiteMode.None
                });

            _authRepository.SaveRefreshTokenInDb(refreshToken, user);           
        }
    }
}
