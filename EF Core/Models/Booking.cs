using EF_Core.Enumerations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EF_Core.Models
{
    [Table("bookings")]
    public class Booking
    {
        [Key]
        [Column("bookingid")]
        public Guid BookingId { get; set; } = Guid.NewGuid();

        [Column("customerid")]
        public required Guid CustomerId { get; set; }

        [Column("roomid")]
        public required Guid RoomId { get; set; }

        [Column("expectedcheckin")]
        public required DateOnly ExpectedCheckIn { get; set; }

        [Column("checkout")]
        public DateOnly CheckOut { get; set; }

        [Column("status")]
        public Status Status { get; set; } = Status.Booked;

        [Column ("duration")]
        [Range(1, 30, ErrorMessage = "Duration must be between 1 and 30 days.")]
        public required int Duration { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public Customer customer { get; set; }

        [ForeignKey(nameof(RoomId))]
        public Room room { get; set; }
    }
}
