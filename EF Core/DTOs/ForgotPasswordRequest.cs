using System.ComponentModel.DataAnnotations;

namespace EF_Core.DTOs
{
    public class ForgotPasswordRequest
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Email address is not valid.")]
        public required string Email { get; set; }
    }
}
