using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Kumadio.Core.Interfaces;
using Kumadio.Web.DTOS.Auth;
using Kumadio.Domain.Entities;
using Kumadio.Web.DTOS;
using Kumadio.Web.Settings;
using Kumadio.Web.Mappers.Base;
using Kumadio.Core.Common;
using Kumadio.Web.Common;
using Google.Apis.Auth;
using Kumadio.Domain.Enums;

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
        private readonly IDtoMapper<OnboardingQuestion, DTOOnboardingQuestion> _onboardingQuestionMapper;
        private readonly IDtoMapper<DTORegisterCreator, User> _registerCreatorMapper;
        private readonly IDtoMapper<User, DTOReturnUser> _returnUserMapper;

        public AuthController(
            IAuthService authService,
            IOptions<AppSettings> appSettings,
            IHttpContextAccessor httpContextAccessor,
            IDtoMapper<DTORegister, User> registerMapper,
            IDtoMapper<OnboardingQuestion, DTOOnboardingQuestion> onboardingQuestionMapper,
            IDtoMapper<DTORegisterCreator, User> registerCreatorMapper,
            IDtoMapper<User, DTOReturnUser> returnUserMapper)
        {
            _authService = authService;
            _appSettings = appSettings.Value;
            _httpContextAccessor = httpContextAccessor;
            _registerMapper = registerMapper;
            _onboardingQuestionMapper = onboardingQuestionMapper;
            _registerCreatorMapper = registerCreatorMapper;
            _returnUserMapper = returnUserMapper;
        }

        #region GET

        [HttpGet("current-user")]
        // [Authorize]
        public async Task<ActionResult<DTOReturnUser>> GetCurrentUser()
        {
            var user = (User)HttpContext.Items["CurrentUser"]!;

            if (user == null)
                return DomainErrors.Auth.JwtTokenMissing.ToBadRequest();

            var result = await _authService.GetUserWithEmail(user.Email);
            if (result.IsFailure) return result.Error.ToBadRequest();

            return Ok(_returnUserMapper.Map(result.Value));
        }

        [HttpGet("onboarding-questions")]
        public ActionResult<IEnumerable<DTOOnboardingQuestion>> GetOnboardingQuestions()
        {
            var user = (User)HttpContext.Items["CurrentUser"]!;

            if (user == null)
                return DomainErrors.Auth.JwtTokenMissing.ToBadRequest();

            if (user.IsOnboarded)
                return DomainErrors.Auth.UserAlreadyOnboarded.ToBadRequest();

            var onboardingResult = _authService.GetOnboardingQuestions();

            if (onboardingResult.IsFailure) return onboardingResult.Error.ToBadRequest();

            return Ok(_onboardingQuestionMapper.Map(onboardingResult.Value));
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

        [HttpPost("google-register")]
        public async Task<IActionResult> GoogleRegister([FromBody] string idToken)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new List<string> { _appSettings.GoogleClientId }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            if (payload == null) return DomainErrors.Auth.GoogleCredentialsNotValid.ToBadRequest();

            var user = new User
            {
                Email = payload.Email,
                FirstName = payload.GivenName,
                LastName = payload.FamilyName,
                Role = Role.LISTENER_NO_SUBSCRIPTION,
                AuthProvider = AuthProvider.Google,
                ExternalId = payload.Subject
            };

            var result = await _authService.GoogleRegister(user);
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
            var loginResult = await LoginHelper(model);
            if (loginResult.IsFailure) return loginResult.Error.ToBadRequest();

            var (accessToken, refreshToken, user) = loginResult.Value;

            return Ok(new 
            {   
                accessToken,
                refreshToken,
                user = _returnUserMapper.Map(user)
            });
        }

        [HttpPost("login-web")]
        public async Task<ActionResult<DTOReturnUser>> LoginWeb([FromBody] DTOLogin model)
        {
            var loginResult = await LoginHelper(model);
            if (loginResult.IsFailure) return loginResult.Error.ToBadRequest();

            var (accessToken, refreshToken, user) = loginResult.Value;

            SetJwtCookie(accessToken);
            SetRefreshTokenCookie(refreshToken);

            return Ok(new
            {
                user = _returnUserMapper.Map(user)
            });
        }

        [HttpPost("google-login-web")]
        public async Task<ActionResult<DTOReturnUser>> GoogleLoginWeb([FromBody] string idToken)
        {
            var loginResult = await GoogleLoginHelper(idToken);
            if (loginResult.IsFailure) return loginResult.Error.ToBadRequest();

            var (accessToken, refreshToken, user) = loginResult.Value;

            SetJwtCookie(accessToken);
            SetRefreshTokenCookie(refreshToken);

            return Ok(new
            {
                user = _returnUserMapper.Map(user)
            });
        }

        [HttpPost("google-login-mobile")]
        public async Task<ActionResult<DTOReturnUser>> GoogleLoginMobile([FromBody] string idToken)
        {
            var loginResult = await GoogleLoginHelper(idToken);
            if (loginResult.IsFailure) return loginResult.Error.ToBadRequest();

            var (accessToken, refreshToken, user) = loginResult.Value;

            return Ok(new
            {
                accessToken,
                refreshToken,
                user = _returnUserMapper.Map(user)
            });
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
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(-1)
            });
            Response.Cookies.Append("X-Refresh-Token", "", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
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

        [HttpPost("refresh-web")]
        public async Task<IActionResult> RefreshWeb()
        {
            var refreshTokenHash = _httpContextAccessor.HttpContext?.Request.Cookies["X-Refresh-Token"];

            var refreshResult = await RefreshTokenHelper(refreshTokenHash);
            if (refreshResult.IsFailure)
                return refreshResult.Error.ToBadRequest();

            var (newAccessToken, newRefreshToken) = refreshResult.Value;

            SetJwtCookie(newAccessToken);
            SetRefreshTokenCookie(newRefreshToken);

            return MessageResponse.Ok("Refresh successful");
        }

        [HttpPost("refresh-mobile")]
        public async Task<IActionResult> RefreshMobile(DTORefreshToken refreshTokenDto)
        {
            var refreshResult = await RefreshTokenHelper(refreshTokenDto.RefreshToken);
            if (refreshResult.IsFailure) return refreshResult.Error.ToBadRequest();
            var (newAccessToken, newRefreshToken) = refreshResult.Value;

            return Ok(new
            {
                accessToken = newAccessToken,
                refreshToken = newRefreshToken
            });
        }

        [HttpPost("complete-onboarding")]
        public async Task<IActionResult> CompleteOnboarding([FromBody] DTOOnboardingData onboardingDataDto)
        {
            var user = (User)HttpContext.Items["CurrentUser"]!;

            if (user == null)
                return DomainErrors.Auth.JwtTokenMissing.ToBadRequest();

            if (user.IsOnboarded)
                return DomainErrors.Auth.UserAlreadyOnboarded.ToBadRequest();

            var onboardingData = new OnboardingData
            {
                UserId = user.Id,
                ChildAge = onboardingDataDto.ChildAge
            };

            var onboardingResult = await _authService.CompleteOnboarding(onboardingData, onboardingDataDto.SelectedOptions);

            if (onboardingResult.IsFailure) return onboardingResult.Error.ToBadRequest();

            return Ok();
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
        private async Task<Result<(string accessToken, RefreshToken refreshToken, User user)>> LoginHelper(DTOLogin model)
        {
            var loginResult = await _authService.Login(model.Email, model.Password);
            if (loginResult.IsFailure)
                return loginResult.Error;

            var user = loginResult.Value;

            var accessToken = GenerateJwt(user);
            if (String.IsNullOrEmpty(accessToken))
                return DomainErrors.Auth.JwtTokenIssue;

            var refreshToken = GenerateRefreshToken();

            var saveRefreshResult = await _authService.SaveRefreshToken(refreshToken, user);
            if (saveRefreshResult.IsFailure)
                return saveRefreshResult.Error;

            return (accessToken, refreshToken, user);
        }

        private async Task<Result<(string accessToken, RefreshToken refreshToken, User user)>> GoogleLoginHelper(string idToken)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new List<string> { _appSettings.GoogleClientId }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            if (payload == null) return DomainErrors.Auth.GoogleCredentialsNotValid;

            var resultGetUser = await _authService.GetUserWithEmail(payload.Email);
            if (resultGetUser.IsFailure) return resultGetUser.Error;

            var user = resultGetUser.Value;

            var accessToken = GenerateJwt(user);
            if (String.IsNullOrEmpty(accessToken))
                return DomainErrors.Auth.JwtTokenIssue;

            var refreshToken = GenerateRefreshToken();

            var saveRefreshResult = await _authService.SaveRefreshToken(refreshToken, user);
            if (saveRefreshResult.IsFailure) return saveRefreshResult.Error;

            return (accessToken, refreshToken, user);
        }

        private async Task<Result<(string newAccessToken, RefreshToken newRefreshToken)>> RefreshTokenHelper(string? oldRefreshToken)
        {
            if (string.IsNullOrEmpty(oldRefreshToken))
                return DomainErrors.Auth.RefreshTokenMissing;

            // 1) Retrieve old token from DB
            var refreshTokenResult = await _authService.GetRefreshToken(oldRefreshToken);
            if (refreshTokenResult.IsFailure)
                return refreshTokenResult.Error;

            var refreshToken = refreshTokenResult.Value;

            // 2) Check absolute expiry
            if (DateTime.Now > refreshToken.AbsoluteExpires)
            {
                // Revoke & force re-login
                var revokeTokenResult = await RevokeTokenHelper();
                if (revokeTokenResult.IsFailure)
                    return revokeTokenResult.Error;

                return DomainErrors.Auth.RefreshTokenExpired;
            }

            // 3) Check normal sliding expiry
            if (DateTime.Now > refreshToken.Expires)
            {
                // Revoke & force re-login
                var revokeTokenResult = await RevokeTokenHelper();
                if (revokeTokenResult.IsFailure)
                    return revokeTokenResult.Error;

                return DomainErrors.Auth.RefreshTokenExpired;
            }

            // 4) Retrieve the user
            var userResult = await _authService.GetUserWithRefreshToken(refreshToken);
            if (userResult.IsFailure)
                return userResult.Error;

            var user = userResult.Value;

            // 5) Generate new access token & new refresh token
            var newAccessToken = GenerateJwt(user);
            if (string.IsNullOrEmpty(newAccessToken))
                return DomainErrors.Auth.JwtTokenIssue;

            var newRefreshToken = GenerateRefreshToken(refreshToken);

            var saveRefreshResult = await _authService.SaveRefreshToken(newRefreshToken, user);
            if (saveRefreshResult.IsFailure) return saveRefreshResult.Error;

            return (newAccessToken, newRefreshToken);

        }
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
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var encryptedToken = tokenHandler.WriteToken(token);

            return encryptedToken;
        }
        private RefreshToken GenerateRefreshToken(RefreshToken? oldToken = null)
        {
            var now = DateTime.Now;
            if (oldToken == null)
            {
                // brand-new login scenario
                return new RefreshToken
                {
                    Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                    Created = now,
                    Expires = now.AddDays(7),   // sliding 7-day
                    AbsoluteExpires = now.AddDays(90) // absolute 90-day
                };
            }
            else
            {
                // We rotate to a new token each refresh, but keep the same AbsoluteExpires
                return new RefreshToken
                {
                    Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                    Created = now,
                    Expires = now.AddDays(15),
                    AbsoluteExpires = oldToken.AbsoluteExpires,
                    UserId = oldToken.UserId
                };
            }
        }
        private void SetJwtCookie(string tokenValue)
        {
            var response = _httpContextAccessor.HttpContext?.Response;
            response?.Cookies.Append("X-Access-Token", tokenValue, new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(15),
                HttpOnly = true,
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None,
                Path = "/"
            });
        }
        private void SetRefreshTokenCookie(RefreshToken refreshToken)
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
        }
        
        #endregion
    }

}
