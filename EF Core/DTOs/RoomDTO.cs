using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EF_Core.DTOs
{
    public class RoomDTO
    {
        public Guid RoomId { get; set; } = Guid.NewGuid();

        public int RoomNo { get; set; }

        public int Capacity { get; set; }

        public string? Status { get; set; } = "Available"; // Default status is "Available"
    }
}