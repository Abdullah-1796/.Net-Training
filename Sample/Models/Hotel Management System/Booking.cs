namespace Sample.Models.Hotel_Management_System
{
    public class Booking
    {
        public Guid bid {  get; set; }
        public int cid { get; set; }
        public int rid { get; set; }
        public string? CheckIn { get; set; }
        public string? CheckOut { get; set; } = null;
    }
}
