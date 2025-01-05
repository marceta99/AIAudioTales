using System.Security.Cryptography;
using System.Text;
using Kumadio.Infrastructure.Interfaces;
using Kumadio.Core.Interfaces;
using Kumadio.Domain.Entities;

namespace Kumadio.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;

        public AuthService(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
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

        public async Task<User?> ValidateLogin(string email, string password)
        {
            var emailUsed = await _authRepository.IsEmailUsed(email);
            if (!emailUsed) return null;

            var user = await _authRepository.GetUserWithEmail(email);
            if (user == null) return null;

            var passwordIsCorrect = CheckPassword(password, user);
            if (!passwordIsCorrect) return null;

            return user;
        }

        public async Task<RefreshToken> GetRefreshToken(string refreshToken)
        {
            var tokenInDb = await _authRepository.GetRefreshToken(refreshToken);
            if (tokenInDb == null) throw new Exception("Refresh token not found in DB");

            return tokenInDb;
        }

        public async Task<User> GetUserWithRefreshToken(RefreshToken refreshToken)
        {
            var user = await _authRepository.GetUserWithRefreshToken(refreshToken);
            if (user == null)
                throw new Exception("User not found for refresh token");

            return user;
        }

        public async Task DeleteRefreshTokenForUser(string email)
        {
            await _authRepository.DeleteRefreshTokenForUser(email);
        }

        public async Task<User?> GetUserWithEmail(string email)
        {
            return await _authRepository.GetUserWithEmail(email);
        }

        public async Task SaveRefreshToken(RefreshToken refreshToken, User user)
        {
            await _authRepository.SaveRefreshToken(refreshToken, user);
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

    }

}