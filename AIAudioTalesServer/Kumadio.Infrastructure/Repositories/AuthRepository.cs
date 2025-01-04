using Kumadio.Domain.Entities;
using Kumadio.Domain.Enums;
using Kumadio.Infrastructure.Data;
using Kumadio.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Kumadio.Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _dbContext;

        public AuthRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Minimal data-access only
        public async Task<int> AddNewUser(User user)
        {
            // The business logic for "empty name" or "email used?" is now in the service
            // Here, we simply attempt to insert into DB and handle conflicts.

            // We can still do a quick check if email is used, or throw an exception if needed
            var isUsed = await IsEmailUsed(user.Email);
            if (isUsed)
                return 0;

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return 1;
        }

        public async Task<bool> IsEmailUsed(string email)
        {
            var userExists = await _dbContext.Users.AnyAsync(u => u.Email == email);
            return userExists;
        }

        public async Task<User?> GetUserWithEmail(string email)
        {
            return await _dbContext.Users
                                   .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task SaveRefreshTokenInDb(RefreshToken refreshToken, User user)
        {
            var existingToken = await _dbContext.RefreshTokens
                                                .FirstOrDefaultAsync(rt => rt.UserId == user.Id);
            if (existingToken == null)
            {
                refreshToken.UserId = user.Id;
                _dbContext.RefreshTokens.Add(refreshToken);
            }
            else
            {
                existingToken.Token = refreshToken.Token;
                existingToken.Expires = refreshToken.Expires;
                existingToken.Created = refreshToken.Created;
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task<RefreshToken?> GetRefreshToken(string refreshToken)
        {
            return await _dbContext.RefreshTokens
                                   .FirstOrDefaultAsync(rt => rt.Token == refreshToken);
        }

        public async Task<User?> GetUserWithRefreshToken(RefreshToken refreshToken)
        {
            return await _dbContext.Users
                                   .FirstOrDefaultAsync(u => u.Id == refreshToken.UserId);
        }

        public async Task DeleteRefreshTokenForUser(string email)
        {
            var user = await GetUserWithEmail(email);
            if (user == null) return;

            var refreshToken = await _dbContext.RefreshTokens
                                               .FirstOrDefaultAsync(rt => rt.UserId == user.Id);
            if (refreshToken != null)
            {
                _dbContext.RefreshTokens.Remove(refreshToken);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task UpdateUserRole(Role role, int userId)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null) return;

            user.Role = role;
            await _dbContext.SaveChangesAsync();
        }
    }
}
