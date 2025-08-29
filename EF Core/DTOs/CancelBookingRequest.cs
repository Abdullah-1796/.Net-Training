namespace EF_Core.DTOs
{
    public class CancelBookingRequest
    {
        public required int RoomNo { get; set; }
        public required string Cnic { get; set; }
    }
}
