﻿using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Google.Apis.Auth;
using AIAudioTalesServer.Settings;
using AIAudioTalesServer.Domain.Entities;
using AIAudioTalesServer.Domain.Enums;
using AIAudioTalesServer.Web.DTOS;
using AIAudioTalesServer.Web.DTOS.Auth;
using AIAudioTalesServer.Infrastructure.Interfaces;
using AIAudioTalesServer.Core.Interfaces;

namespace AIAudioTalesServer.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            // We no longer need IAuthRepository here
            _authService = authService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] Register model)
        {
            var result = await _authService.RegisterAsync(model);

            if (result == 0)
                return BadRequest("Problem with registering user");
            return Ok();
        }

        [HttpPost("RegisterCreator")]
        public async Task<IActionResult> RegisterCreator([FromBody] RegisterCreator model)
        {
            var result = await _authService.RegisterCreatorAsync(model);

            if (result == 0)
                return BadRequest("User with that email already exists");
            return Ok();
        }

        [HttpPost("Login")]
        public async Task<ActionResult<DTOReturnUser>> Login([FromBody] Login model)
        {
            var dtoUser = await _authService.LoginAsync(model);
            if (dtoUser == null)
                return BadRequest("User name or password was invalid");
            return Ok(dtoUser);
        }

        [HttpGet("RefreshToken")]
        public async Task<ActionResult> RefreshToken()
        {
            try
            {
                await _authService.RefreshTokenAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        // If you want to keep it as a public action:
        [HttpDelete("RevokeToken")]
        public async Task<IActionResult> RevokeToken()
        {
            await _authService.RevokeTokenAsync();
            return Ok();
        }

        [HttpPost("LoginWithGoogle")]
        public async Task<ActionResult<DTOReturnUser>> LoginWithGoogle([FromBody] string credentials)
        {
            var dtoUser = await _authService.LoginWithGoogleAsync(credentials);
            if (dtoUser == null)
                return BadRequest();
            return Ok(dtoUser);
        }

        [HttpGet("GetCurrentUser")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var currentUser = await _authService.GetCurrentUserAsync();
            if (currentUser == null)
                return BadRequest();
            return Ok(currentUser);
        }
    }
}
