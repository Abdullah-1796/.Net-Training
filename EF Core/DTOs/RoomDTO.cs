using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EF_Core.DTOs
{
    public class RoomDTO
    {
        public required int RoomNo { get; set; }

        public required int Capacity { get; set; }

        public string? Status { get; set; } = "Available"; // Default status is "Available"
    }
}