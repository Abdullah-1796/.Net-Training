using EF_Core.DTOs;
using EF_Core.DTOs.For_Patch;
using EF_Core.Enumerations;
using EF_Core.Models;
using Microsoft.AspNetCore.Identity;

namespace EF_Core.Services.Interfaces
{
    public interface IAuthService
    {
        public Task<SignInResult> LoginUserAsync(LoginRequest loginRequest);
        public Task LogoutUserAsync();
        
    }
}
