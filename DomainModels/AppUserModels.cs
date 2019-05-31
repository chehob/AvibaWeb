using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using AvibaWeb.Models;
using Microsoft.AspNetCore.Identity;

namespace AvibaWeb.DomainModels
{
    public class AppUser : IdentityUser
    {
        public string Name { get; set; }
        public string Position { get; set; }
        public byte[] Photo { get; set; }
        //public string Organization { get; set; }

        [Column(TypeName = "Money")]
        public decimal Balance { get; set; }

        public virtual List<Card> Cards { get; set; }

        public AppUser()
        {
            Balance = 0;
        }

        [DefaultValue(true)]
        public bool IsActive { get; set; } = true;

        public int? BookingMappingId { get; set; }

        public int? PushAllUserId { get; set; }

        public string UserITN { get; set; }

        public virtual ICollection<AcceptedCollector> AcceptedCollectors { get; set; }

        public virtual ICollection<IdentityUserRole<string>> Roles { get; } = new List<IdentityUserRole<string>>();
    }    
}