using Microsoft.AspNetCore.Identity;

namespace Deps_CleanArchitecture.Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public decimal Credito { get; set; } = 0;

        public string ClienteId { get; set; }

        public Cliente Cliente { get; set; }
    }

}