using System.ComponentModel.DataAnnotations;

namespace EF_Core.DTOs.For_Patch
{
    public class EmployeePDTO
    {
        [Required]
        public required string Cnic { get; set; }

        public  string? Name { get; set; }

        public  string? Phone { get; set; }

        public  string? Email { get; set; }

        public  string? Role { get; set; }
    }
}
