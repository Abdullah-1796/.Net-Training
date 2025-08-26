namespace EF_Core.DTOs
{
    public class RoomDTO
    {
        /// <summary>
        /// public Guid RoomId { get; set; } = Guid.NewGuid();
        /// </summary>

        public required int RoomNo { get; set; }

        public required int Capacity { get; set; }

        public required string Status { get; set; } = "Available"; // Default status is "Available"
    }
}