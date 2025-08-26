using System.ComponentModel.DataAnnotations;

namespace EF_Core.DTOs
{
    public class UpdatePasswordRequest
    {
        [Required]
        public required string Cnic { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password do not match")]
        public required string ConfirmPassword { get; set; }
    }
}
