using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class VBookingManagementAtolTickets
    {
        [Key]
        public int AtolServerId { get; set; }

        public string AtolServerName { get; set; }

        public decimal? Amount { get; set; }

        public string PaymentType { get; set; }

        public int DocCount { get; set; }

        public int SegCount { get; set; } = 0;

        public DateTime OperationDateTime { get; set; }
    }
}
