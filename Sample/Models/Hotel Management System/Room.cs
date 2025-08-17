namespace Sample.Models.Hotel_Management_System
{
    public class Room
    {
        public Guid Rid { get; set; }
        public int RoomNo { get; set; }
        public required int Capacity { get; set; }
        public string? Status { get; set; } = "Available"; // Default status is "Available"
    }
}
