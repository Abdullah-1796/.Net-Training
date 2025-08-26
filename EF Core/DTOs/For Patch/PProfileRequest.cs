namespace EF_Core.DTOs.For_Patch
{
    public class PProfileRequest
    {
        public required string Cnic { get; set; }

        public string? Name { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }
    }
}
