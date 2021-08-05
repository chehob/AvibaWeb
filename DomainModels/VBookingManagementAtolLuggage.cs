using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class VBookingManagementAtolLuggage
    {
        [Key]
        public Guid Id { get; set; }

        public int AtolServerId { get; set; }

        public string AtolServerName { get; set; }

        public decimal? LuggageAmount { get; set; }

        public decimal FeeAmount { get; set; }

        public DateTime OperationDateTime { get; set; }
    }
}
