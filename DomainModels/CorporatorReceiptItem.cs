using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class CorporatorReceiptItem
    {
        public enum CRIType
        {
            [Display(Name = "Билет")]
            Ticket = 0,
            [Display(Name = "Багаж")]
            Luggage = 1
        }

        [Key]
        public int CorporatorReceiptItemId { get; set; }

        public int CorporatorReceiptId { get; set; }
        [ForeignKey("CorporatorReceiptId")]
        public virtual CorporatorReceipt Receipt { get; set; }

        public decimal Amount { get; set; }

        public decimal FeeRate { get; set; }

        public bool PerSegment { get; set; } = true;

        public bool IsPercent { get; set; } = false;

        public string PassengerName { get; set; }

        public string Route { get; set; }

        public int? TicketOperationId { get; set; }

        public CRIType TypeId { get; set; }
    }
}
