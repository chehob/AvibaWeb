using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class CorporatorFeeRate
    {
        [Key]
        public int CorporatorFeeRateId { get; set; }

        public string ITN { get; set; }
        [ForeignKey("ITN")]
        public virtual Counterparty Corporator { get; set; }

        public int OrganizationId { get; set; }
        [ForeignKey("OrganizationId")]
        public virtual Organization Organization { get; set; }

        public int TicketTypeId { get; set; }

        public int OperationTypeId { get; set; }

        public decimal Rate { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string AirlineId { get; set; }

        public bool PerSegment { get; set; }

        public bool IsPercent { get; set; }
    }
}
