using Microsoft.AspNetCore.Identity;

namespace WhiteLagoon.Domain.Entities.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
