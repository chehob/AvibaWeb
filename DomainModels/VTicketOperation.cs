using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class VTicketOperation
    {
        [Key]
        public Guid Id { get; set; }
        public int TicketOperationId { get; set; }
        public string BSONumber { get; set; }
        public DateTime ExecutionDateTime { get; set; }
        public string Route { get; set; }
        public decimal Payment { get; set; }
        public VKRSCancelRequest.TOType OperationTypeID { get; set; }
        public string PassengerName { get; set; }
        public int SegCount { get; set; }
        public string DeskId { get; set; }
        public int? TicketTypeId { get; set; }
        public CorporatorReceiptItem.CRIType ReceiptItemTypeId { get; set; }
    }
}
