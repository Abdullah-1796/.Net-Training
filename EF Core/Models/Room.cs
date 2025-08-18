using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EF_Core.Models
{
    [Table("rooms")]
    public class Room
    {
        [Key]
        [Column("roomid")]
        public Guid RoomId { get; set; } = Guid.NewGuid();

        [Required]
        [Column("roomno")]
        public int RoomNo { get; set; }

        [Required]
        [Column("capacity")]
        public required int Capacity { get; set; }

        [Column("status")]
        public string? Status { get; set; } = "Available"; // Default status is "Available"

        [JsonIgnore]
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
