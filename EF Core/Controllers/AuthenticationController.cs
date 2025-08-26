using EF_Core.DTOs;
using EF_Core.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EF_Core.Services.Interfaces;
using EF_Core.Enumerations;
using EF_Core.DTOs.For_Patch;
using Microsoft.AspNetCore.Authorization;

namespace EF_Core.Controllers
{
    [Route("api/")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthenticationController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return Ok($"User {User.Identity.Name} is already logged in");
            }

            if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Cnic) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return BadRequest("Email and password are required.");
            }
            var result = await _authService.LoginUserAsync(loginRequest);
            if (result.Succeeded)
            {
                return Ok("Login successful.");
            }
            else
            {
                return Unauthorized("Invalid login attempt.");
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                await _authService.LogoutUserAsync();
                return Ok("Logout successful.");
            }

            return Ok($"User not logged in");
        }

        
    }
}
