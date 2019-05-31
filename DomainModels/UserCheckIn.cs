using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class UserCheckIn
    {
        public int UserCheckInId { get; set; }

        [MaxLength(128)]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual AppUser User { get; set; }

        public string DeskId { get; set; }
        [ForeignKey("DeskId")]
        public virtual Desk Desk { get; set; }

        public DateTime CheckInDateTime { get; set; }
    }
}
