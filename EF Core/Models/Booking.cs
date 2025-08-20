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

        [Column("checkin")]
        public required DateOnly CheckIn { get; set; }

        [Column("checkout")]
        public DateOnly CheckOut { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public Customer customer { get; set; }

        [ForeignKey(nameof(RoomId))]
        public Room room { get; set; }
    }
}
