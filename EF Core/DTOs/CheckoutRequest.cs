namespace EF_Core.DTOs
{
    public class CheckoutRequest
    {
        public required int RoomNo { get; set; }
        public required string Cnic { get; set; }
        public required DateOnly CheckoutDate { get; set; }
    }
}
