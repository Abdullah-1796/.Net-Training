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
        public Guid CustomerId { get; set; }

        [Column("roomid")]
        public Guid RoomId { get; set; }

        [Column("checkin")]
        public DateOnly CheckIn { get; set; }

        [Column("checkout")]
        public DateOnly CheckOut { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public Customer customer { get; set; }

        [ForeignKey(nameof(RoomId))]
        public Room room { get; set; }
    }
}
