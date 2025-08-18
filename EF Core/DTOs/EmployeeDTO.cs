using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EF_Core.DTOs
{
    public class EmployeeDTO
    {
        public Guid EmployeeId { get; set; } = Guid.NewGuid();

        public required string Cnic { get; set; }

        public string? Name { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public string? Role { get; set; }
    }
}
