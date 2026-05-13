
using Microsoft.AspNetCore.Identity;

namespace HomeTaste.Infrastructure.Identity.Entity
{
    public class IdentityApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? ProfileImagePublicId { get; set; }
    }
}
