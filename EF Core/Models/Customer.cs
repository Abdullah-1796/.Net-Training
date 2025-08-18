using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EF_Core.Models
{
    [Table("customers")]
    public class Customer
    {
        [Key]
        [Column("customerid")]
        public Guid CustomerId { get; set; } = Guid.NewGuid();

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

        [JsonIgnore]
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
