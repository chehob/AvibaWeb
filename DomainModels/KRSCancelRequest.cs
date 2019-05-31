using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class KRSCancelRequest
    {
        public int KRSCancelRequestId { get; set; }

        public int KRSId { get; set; }

        [MaxLength(128)]
        public string CashierId { get; set; }
        [ForeignKey("CashierId")]
        public virtual AppUser Cashier { get; set; }

        public string DeskId { get; set; }
        [ForeignKey("DeskId")]
        public virtual Desk Desk { get; set; }

        public string Description { get; set; }
    }
}
