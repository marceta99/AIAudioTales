using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Kumadio.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Kumadio.Core.Interfaces;
using Kumadio.Domain.Entities;
using Kumadio.Domain.Enums;
using Google.Apis.Auth;

namespace Kumadio.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(IAuthRepository authRepository,
                           IHttpContextAccessor httpContextAccessor)
        {
            _authRepository = authRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<int> RegisterAsync(User user, string password)
        {
            // Hash password
            using (HMACSHA512 hmac = new HMACSHA512())
            {
                user.PasswordSalt = hmac.Key;
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }

            // Store in DB
            var result = await _authRepository.AddNewUser(user);
            
            // This returns 1 if success, 0 if email was in use, etc.
            return result;
        }

        public async Task<int> RegisterCreatorAsync(User user, string password)
        {
            using (HMACSHA512 hmac = new HMACSHA512())
            {
                user.PasswordSalt = hmac.Key;
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }

            var result = await _authRepository.AddNewUser(user);
            return result;
        }

        public async Task<DTOReturnUser?> LoginAsync(DTOLogin model)
        {
            // Basic checks
            if (string.IsNullOrWhiteSpace(model.Email) ||
                string.IsNullOrWhiteSpace(model.Password))
            {
                return null;
            }

            // 1) Check if email is used
            var emailUsed = await _authRepository.IsEmailUsed(model.Email);
            if (!emailUsed) return null;

            // 2) Retrieve user
            var user = await _authRepository.GetUserWithEmail(model.Email);
            if (user == null) return null;

            // 3) Check password
            var passwordIsCorrect = CheckPassword(model.Password, user);
            if (!passwordIsCorrect) return null;

            // 4) Generate JWT & Refresh
            await GenerateJwt(user);

            // 5) Convert user to DTO and return
            return new DTOReturnUser
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role
            };
        }

        public async Task RefreshTokenAsync()
        {
            var refreshToken = _httpContextAccessor.HttpContext?.Request.Cookies["X-Refresh-Token"];
            if (string.IsNullOrEmpty(refreshToken))
                throw new Exception("Missing refresh token cookie");

            var tokenInDb = await _authRepository.GetRefreshToken(refreshToken);
            if (tokenInDb == null)
                throw new Exception("Refresh token not found in DB");

            if (tokenInDb.Expires < DateTime.Now)
            {
                // If expired, revoke and force re-login
                await RevokeTokenAsync();
                throw new Exception("Refresh token has expired");
            }

            var user = await _authRepository.GetUserWithRefreshToken(tokenInDb);
            if (user == null)
                throw new Exception("User not found for refresh token");

            await GenerateJwt(user);
        }

        public async Task RevokeTokenAsync()
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
                await _authRepository.DeleteRefreshTokenForUser(email);
            }

            // Optionally, clear cookies
            var response = _httpContextAccessor.HttpContext?.Response;
            response?.Cookies.Delete("X-Access-Token");
            response?.Cookies.Delete("X-Refresh-Token");
        }

        public async Task<DTOReturnUser?> LoginWithGoogleAsync(string googleCredentials)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new List<string> { _appSettings.GoogleClientId }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(googleCredentials, settings);
            if (payload == null)
                return null;

            var user = await _authRepository.GetUserWithEmail(payload.Email);
            if (user == null)
                return null;

            await GenerateJwt(user);

            return new DTOReturnUser
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role
            };
        }

        public async Task<object?> GetCurrentUserAsync()
        {
            // This is basically the same logic from the old controller method:
            var jwtTokenCookie = _httpContextAccessor.HttpContext?.Request.Cookies["X-Access-Token"];
            if (string.IsNullOrEmpty(jwtTokenCookie))
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(jwtTokenCookie);

            var emailClaim = token.Claims.FirstOrDefault(c => c.Type == "email");
            if (emailClaim == null)
                return null;

            var email = emailClaim.Value;
            var user = await _authRepository.GetUserWithEmail(email);
            if (user == null) return null;

            return new { email = user.Email, role = user.Role.ToString() };
        }

        public async Task<User?> GetUserWithEmail(string email)
        {
            return await _authRepository.GetUserWithEmail(email);
        }

        // -------------------------------------------------------------------
        // PRIVATE HELPER METHODS
        // -------------------------------------------------------------------

        private bool CheckPassword(string password, User user)
        {
            using (HMACSHA512 hmac = new HMACSHA512(user.PasswordSalt))
            {
                var computed = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computed.SequenceEqual(user.PasswordHash);
            }
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

            await _authRepository.SaveRefreshTokenInDb(refreshToken, user);
        }
    }

}