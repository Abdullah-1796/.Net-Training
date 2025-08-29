namespace EF_Core.DTOs
{
    public class BookingRequest
    {
        public required int RoomNo { get; set; }
        public required string Cnic { get; set; }
        public required DateOnly CheckinDate { get; set; }
        public required int Duration { get; set; }
    }
}
