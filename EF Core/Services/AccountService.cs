using EF_Core.DTOs;
using EF_Core.DTOs.For_Patch;
using EF_Core.Enumerations;
using EF_Core.Models;
using EF_Core.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace EF_Core.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AccountService(UserManager<ApplicationUser> userManager, IConfiguration configuration, IEmailService emailService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<IdentityResult> CreateUser(UserRequest userRequest, UserRole role, string currentUser)
        {
            if (userRequest == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User cannot be null." });
            }
            var user = await _userManager.FindByNameAsync(userRequest.Cnic);
            if (user != null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User already exists." });
            }
            var cUSer = await _userManager.FindByNameAsync(currentUser);
            var currentRole = await _userManager.GetRolesAsync(cUSer!);

            bool flag = false;
            if (currentRole.Contains(UserRole.Admin.ToString()) && !userRequest.role.Equals(UserRole.Admin))
            {
                flag = true;
            }
            else if (currentRole.Contains(UserRole.Manager.ToString()) && userRequest.role.Equals(UserRole.Receptionist))
            {
                flag = true;
            }

            if ((flag))
            {
                var applicationUser = new ApplicationUser
                {
                    UserName = userRequest.Cnic,
                    Name = userRequest.Name,
                    Email = userRequest.Email,
                    PhoneNumber = userRequest.Phone
                };

                var result = await _userManager.CreateAsync(applicationUser, userRequest.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(applicationUser, role.ToString());
                    return result;
                }
            }
            return IdentityResult.Failed(new IdentityError { Description = $"You are not allowed to create user of {userRequest.role.ToString()} role type." });
        }
        public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordRequest resetPassword)
        {
            var user = await _userManager.FindByEmailAsync(resetPassword.Email);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Invalid Request" });
            }

            var role = await _userManager.GetRolesAsync(user);
            if(role.Contains(UserRole.Admin.ToString()))
            {
                return IdentityResult.Failed(new IdentityError { Description = "Only Admins can access this feature!" });
            }
            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(resetPassword.Token));

            var result = await _userManager.ResetPasswordAsync(user, decodedToken, resetPassword.Password);

            //if (!result.Succeeded)
            //{
            //    await _userManager.UpdateSecurityStampAsync(user);
            //}
            return result;
        }

        public async Task<IdentityResult> SendPasswordResetLinkAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
            {
                return IdentityResult.Failed(new IdentityError { Description = "User does not exists or confirmed!" });
            }

            var role = await _userManager.GetRolesAsync(user);
            if (role.Contains(UserRole.Admin.ToString()))
            {
                return IdentityResult.Failed(new IdentityError { Description = "Only Admins can access this feature!" });
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var baseUrl = _configuration["AppSettings:BaseUrl"];
            var resetLink = $"{baseUrl}/Account/ResetPassword?email={user.Email}&token={encodedToken}";

            await _emailService.SendPasswordResetEmailAsync(user.Email!, user.Name!, resetLink);
            return IdentityResult.Success;
        }

        public async Task<ProfileResponse?> GetMyProfileAsync(string currentUser)
        {
            var user = await _userManager.FindByNameAsync(currentUser);
            if (user == null)
            {
                return null;
            }
            var role = await _userManager.GetRolesAsync(user);
            return new ProfileResponse
            {
                Cnic = user.UserName,
                Name = user.Name,
                Email = user.Email,
                Phone = user.PhoneNumber,
                Roles = role
            };
        }

        public async Task<ProfileResponse?> GetProfileAsync(string cnic, string currentUser)
        {
            var user = await _userManager.FindByNameAsync(cnic);
            if (user == null)
            {
                return null;
            }

            var role = await _userManager.GetRolesAsync(user);
            var currentRole = await _userManager.GetRolesAsync(await _userManager.FindByNameAsync(currentUser));

            bool flag = false;

            if (cnic != currentUser)
            {
                switch (currentRole[0])
                {
                    case "Admin":
                        flag = true;
                        break;
                    case "Manager":
                        if (role.Contains(UserRole.Receptionist.ToString()))
                        {
                            flag = true;
                        }
                        break;
                    default:
                        flag = false;
                        break;
                }
            }

            if (flag)
            {
                return new ProfileResponse
                {
                    Cnic = user.UserName,
                    Name = user.Name,
                    Email = user.Email,
                    Phone = user.PhoneNumber,
                    Roles = role
                };
            }
            return null;
        }

        public async Task<ProfileResponse?> UpdateProfile(PProfileRequest profileRequest)
        {
            var user = await _userManager.FindByNameAsync(profileRequest.Cnic);
            if (user == null)
            {
                return null;
            }
            user.Name = profileRequest.Name ?? user.Name;
            user.PhoneNumber = profileRequest.Phone ?? user.PhoneNumber;
            user.Email = profileRequest.Email ?? user.Email;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return new ProfileResponse
                {
                    Cnic = user.UserName,
                    Name = user.Name,
                    Email = user.Email,
                    Phone = user.PhoneNumber,
                    Roles = await _userManager.GetRolesAsync(user)
                };
            }
            return null;
        }

        public async Task<IdentityResult> UpdateUserPasswordAsync(UpdatePasswordRequest updatePasswordRequest)
        {
            var user = await _userManager.FindByNameAsync(updatePasswordRequest.Cnic);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            }
            var role = await _userManager.GetRolesAsync(user);
            if (role.Contains(UserRole.Admin.ToString()))
            {
                return IdentityResult.Failed(new IdentityError { Description = "Admin cannot update his own password, he should forget password" });
            }
            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, updatePasswordRequest.Password);
            var result = await _userManager.UpdateAsync(user);
            return result;
        }
    }
}
