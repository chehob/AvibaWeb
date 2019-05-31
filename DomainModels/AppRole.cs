using Microsoft.AspNetCore.Identity;

namespace AvibaWeb.DomainModels
{
    public class AppRole : IdentityRole
    {
        public AppRole() : base() { }

        public AppRole(string name) : base(name) { }

        //public virtual ICollection<AppUser> Users { get; set; }
    }
}