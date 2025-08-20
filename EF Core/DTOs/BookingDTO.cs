namespace EF_Core.DTOs
{
    public class BookingDTO
    {
        public required Guid BookingId { get; set; }
        public required int RoomNo { get; set; }
        public required string CustomerName { get; set; }
        public required string Phone { get; set; }
        public required string Email { get; set; }
        public required int Capacity { get; set; }
        public required string Status { get; set; }
    }
}
