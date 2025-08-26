using Microsoft.AspNetCore.Identity;

namespace EF_Core.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? Name { get; set; } // Custom property

    }
}
