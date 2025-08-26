using EF_Core.DTOs;
using EF_Core.DTOs.For_Patch;
using EF_Core.Enumerations;
using EF_Core.Models;
using EF_Core.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace EF_Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        

        public async Task<SignInResult> LoginUserAsync(LoginRequest loginRequest)
        {
            var user = await _userManager.FindByNameAsync(loginRequest.Cnic);
            if (user == null)
            {
                return SignInResult.Failed;
            }
            var result = await _signInManager.PasswordSignInAsync(loginRequest.Cnic, loginRequest.Password, loginRequest.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return result;
            }
            else
            {
                return SignInResult.Failed;
            }
        }

        public async Task LogoutUserAsync()
        {
            await _signInManager.SignOutAsync();
        }

        
    }
}
