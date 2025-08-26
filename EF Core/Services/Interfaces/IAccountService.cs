using EF_Core.DTOs;
using EF_Core.DTOs.For_Patch;
using EF_Core.Enumerations;
using Microsoft.AspNetCore.Identity;

namespace EF_Core.Services.Interfaces
{
    public interface IAccountService
    {
        public Task<IdentityResult> CreateUser(UserRequest userRequest, UserRole role, string currentUser);
        public Task<ProfileResponse?> GetMyProfileAsync(string currentUser);
        public Task<ProfileResponse?> GetProfileAsync(string cnic, string currentUser);
        public Task<ProfileResponse?> UpdateProfile(PProfileRequest profileRequest);
        public Task<IdentityResult> UpdateUserPasswordAsync(UpdatePasswordRequest updatePasswordRequest);
        public Task<IdentityResult> SendPasswordResetLinkAsync(string email);
        public Task<IdentityResult> ResetPasswordAsync(ResetPasswordRequest resetPassword);
    }
}
