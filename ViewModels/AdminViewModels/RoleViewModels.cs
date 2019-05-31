using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AvibaWeb.DomainModels;
using AvibaWeb.Models;

namespace AvibaWeb.ViewModels.AdminViewModels
{
    public class RoleViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<string> Members { get; set; }
    }

    public class RoleEditModel
    {
        public AppRole Role { get; set; }
        public IEnumerable<AppUser> Members { get; set; }
        public IEnumerable<AppUser> NonMembers { get; set; }
    }

    public class RoleModificationModel
    {
        [Required]
        public string RoleName { get; set; }
        public string[] IdsToAdd { get; set; }
        public string[] IdsToDelete { get; set; }
    }
}