using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace EF_Core.Models
{
    [Table("employees")]
    public class Employee
    {
        [Key]
        [Column("employeeid")]
        public Guid EmployeeId { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(13, MinimumLength = 13, ErrorMessage = "CNIC must be exactly 13 characters long!")]
        [Column("cnic", TypeName = "character varying(13)")]
        public required string Cnic { get; set; }

        [Required]
        [Column("name")]
        public required string Name { get; set; }

        [Required]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "Phone number must be exactly 11 characters long!")]
        [Column("phone")]
        public required string Phone { get; set; }

        [Required]
        [Column("email")]
        public required string Email { get; set; }

        [Required]
        [Column("role")]
        public required string Role { get; set; }
    }
}
