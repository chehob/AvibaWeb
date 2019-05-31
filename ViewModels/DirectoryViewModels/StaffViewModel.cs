using System.Collections.Generic;
using AvibaWeb.DomainModels;
using AvibaWeb.Models;

namespace AvibaWeb.ViewModels.DirectoryViewModels
{
    public class StaffViewModel
    {
        public IEnumerable<AppUser> Members { get; set; }
    }
}