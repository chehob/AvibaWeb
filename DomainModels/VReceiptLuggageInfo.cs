using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class VReceiptLuggageInfo
    {
        [Key]
        public Guid Id { get; set; }
        public int TicketOperationId { get; set; }
        public string LuggageNumber { get; set; }
        public string TicketNumber { get; set; }
        public DateTime ExecutionDateTime { get; set; }
        public VKRSCancelRequest.TOType OperationTypeID { get; set; }
        public string PassengerName { get; set; }
        public string TicketLabel { get; set; }
    }
}
