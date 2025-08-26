using Microsoft.AspNetCore.Identity;

namespace EF_Core.Models
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public string? Description { get; set; } // Custom property
    }
}
