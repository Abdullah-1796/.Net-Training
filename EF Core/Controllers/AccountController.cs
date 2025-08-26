using EF_Core.DTOs;
using EF_Core.DTOs.For_Patch;
using EF_Core.Services;
using EF_Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EF_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPost("register-user")]
        public async Task<IActionResult> Register([FromBody] UserRequest userRequest)
        {
            if (userRequest == null || string.IsNullOrEmpty(userRequest.Cnic) || string.IsNullOrEmpty(userRequest.Password) || string.IsNullOrEmpty(userRequest.Name))
            {
                return BadRequest("Cnic, password, role, and name are required.");
            }
            var result = await _accountService.CreateUser(userRequest, userRequest.role, User.Identity!.Name!);
            if (result.Succeeded)
            {
                return Ok("Registration successful.");
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpPost("forgotpassword")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (email == null)
            {
                return BadRequest("Email is required.");
            }

            var result = await _accountService.SendPasswordResetLinkAsync(email);
            if (result.Succeeded)
            {
                return Ok("Password reset link has been sent to your email.");
            }
            else
            {
                return BadRequest("Failed to send password reset link. Please ensure the email is correct and try again.");
            }
        }

        [HttpPost("resetpassword")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest resetPassword)
        {
            if (resetPassword == null || string.IsNullOrEmpty(resetPassword.Email) || string.IsNullOrEmpty(resetPassword.Password) || string.IsNullOrEmpty(resetPassword.Token))
            {
                return BadRequest("Email, Password, and Token are required.");
            }
            var result = await _accountService.ResetPasswordAsync(resetPassword);
            if (result.Succeeded)
            {
                return Ok("Password has been reset successfully.");
            }
            else
            {
                return BadRequest("Failed to reset password. The token may be invalid or expired.");
            }
        }

        [Authorize]
        [HttpGet("my-profile")]
        public async Task<IActionResult> GetMyProfile()
        {
            string currentUser = User.Identity!.Name!;
            var profile = await _accountService.GetMyProfileAsync(currentUser);
            if (profile == null)
            {
                return NotFound("Profile not found.");
            }
            return Ok(profile);
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile([FromQuery] string cnic)
        {
            if (string.IsNullOrEmpty(cnic))
            {
                return BadRequest("Cnic is required.");
            }
            string currentRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)!.ToString();
            string currentUser = User.Identity!.Name!;
            Console.WriteLine("Current User: " + currentUser);
            var profile = await _accountService.GetProfileAsync(cnic, currentUser);

            if (profile == null)
            {
                return NotFound("Profile not found or user not authorized.");
            }

            return Ok(profile);
        }

        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] PProfileRequest profileRequest)
        {
            if (profileRequest == null || string.IsNullOrEmpty(profileRequest.Cnic))
            {
                return BadRequest("Cnic is required.");
            }
            var updatedProfile = await _accountService.UpdateProfile(profileRequest);
            if (updatedProfile == null)
            {
                return NotFound("Profile not found.");
            }
            return Ok(updatedProfile);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("update-user-password")]
        public async Task<IActionResult> UpdateUserPassword([FromBody] UpdatePasswordRequest updatePasswordRequest)
        {
            if (updatePasswordRequest == null || string.IsNullOrEmpty(updatePasswordRequest.Cnic) || string.IsNullOrEmpty(updatePasswordRequest.Password))
            {
                return BadRequest("Cnic, current password, and new password are required.");
            }
            var result = await _accountService.UpdateUserPasswordAsync(updatePasswordRequest);
            if (result.Succeeded)
            {
                return Ok("Password updated successfully.");
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }
    }
}
