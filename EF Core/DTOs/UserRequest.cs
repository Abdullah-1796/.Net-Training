using EF_Core.Enumerations;

namespace EF_Core.DTOs
{
    public class UserRequest
    {
        public required string Cnic { get; set; }

        public required string Name { get; set; }

        public required string Phone { get; set; }

        public required string Email { get; set; }

        public required string Password { get; set; }

        public required UserRole role { get; set; }
    }
}
