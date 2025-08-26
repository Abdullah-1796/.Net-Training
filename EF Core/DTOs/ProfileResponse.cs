namespace EF_Core.DTOs
{
    public class ProfileResponse
    {
        public required string Cnic { get; set; }

        public required string Name { get; set; }

        public required string Phone { get; set; }

        public required string Email { get; set; }

        public required IList<string> Roles { get; set; }
    }
}
