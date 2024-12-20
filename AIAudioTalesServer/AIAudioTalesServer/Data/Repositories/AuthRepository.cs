﻿using AIAudioTalesServer.Data.Interfaces;
using AIAudioTalesServer.Models;
using AIAudioTalesServer.Models.DTOS;
using AIAudioTalesServer.Models.Enums;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using static System.Reflection.Metadata.BlobBuilder;

namespace AIAudioTalesServer.Data.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;


        public AuthRepository(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<int> AddNewUser(User user)
        {
            if (string.IsNullOrWhiteSpace(user.FirstName) || string.IsNullOrWhiteSpace(user.LastName))
            {
                return 0;
            }

            var isUsed = await IsEmailUsed(user.Email); 

            if (isUsed)
            {
                return 0; //user with that user name already exists
            }
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return 1;
        }
        public async Task<bool> IsEmailUsed(string email)
        {
            var userExists = await _dbContext.Users.Where(u => u.Email == email).FirstOrDefaultAsync();

            if (userExists == null) return false; //user with that email does not exists

            return true; //user with that email already exists 
        }
        public bool CheckPassword(string password, User user)
        {
            bool result;

            using (HMACSHA512? hmac = new HMACSHA512(user.PasswordSalt))
            {
                var compute = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                result = compute.SequenceEqual(user.PasswordHash);
            }
            return result;

        }
        public async Task<User> GetUserWithEmail(string email)
        {
            var user = await _dbContext.Users.Where(u => u.Email == email).FirstOrDefaultAsync();

            return user;
        }
        public DTOReturnUser GetDTOUser(User user)
        {
            return _mapper.Map<DTOReturnUser>(user);
        }
        public async Task SaveRefreshTokenInDb(RefreshToken refreshToken, User user)
        {
            var token = await _dbContext.RefreshTokens.Where(rt => rt.UserId == user.Id).FirstOrDefaultAsync();
            if (token == null) //if token was never created before for that user
            {
                refreshToken.UserId = user.Id;
                refreshToken.User = user;
                _dbContext.RefreshTokens.Add(refreshToken);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                token.Token = refreshToken.Token;
                token.Expires = refreshToken.Expires;
                token.Created = refreshToken.Created;
                await _dbContext.SaveChangesAsync();
            }

        }
        public async Task<RefreshToken> GetRefreshToken(string refreshToken)
        {
            var token = await _dbContext.RefreshTokens.Where(rt => rt.Token == refreshToken).FirstOrDefaultAsync();

            return token; 
        }
        public async Task<User> GetUserWithRefreshToken(RefreshToken refreshToken)
        {
            var token = await _dbContext.Users.Where(u => u.Id == refreshToken.UserId).FirstOrDefaultAsync();

            return token;
        }
        public async Task DeleteRefreshTokenForUser(string email)
        {
            var user = await _dbContext.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
            if (user == null) return;

            var refreshToken = await _dbContext.RefreshTokens.Where(rt => rt.UserId == user.Id).FirstOrDefaultAsync();
            _dbContext.RefreshTokens.Remove(refreshToken);
            await _dbContext.SaveChangesAsync();
        }
        public async Task UpdateUserRole(Role role, int userId)
        {
            var user = await _dbContext.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();

            if(user != null)
            {
                user.Role = role;
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
