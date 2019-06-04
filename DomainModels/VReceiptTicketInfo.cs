using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class VReceiptTicketInfo
    {
        [Key]
        public Guid Id { get; set; }
        public int TicketOperationId { get; set; }
        public string TicketLabel { get; set; }
        public string TicketRoute { get; set; }
        public string BSOLabel { get; set; }
        public string PassengerName { get; set; }
        public int SegCount { get; set; }
        public int? TicketType { get; set; }
    }
}
