using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EF_Core.DTOs
{
    public class EmployeeDTO
    {
        [Required]
        public required string Cnic { get; set; }

        [Required]
        public required string Name { get; set; }

        [Required]
        public required string Phone { get; set; }

        [Required]
        public required string Email { get; set; }

        [Required]
        public required string Role { get; set; }
    }
}
