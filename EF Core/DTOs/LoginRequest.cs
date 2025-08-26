using System.ComponentModel.DataAnnotations;

namespace EF_Core.DTOs
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Cnic is required")]
        [RegularExpression(@"^\d{13}", ErrorMessage = "Cnic must be in the format 0000000000000")]
        public required string Cnic { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
        }
}
