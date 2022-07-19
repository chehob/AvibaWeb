using AvibaWeb.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.ViewModels.KRSClientViewModels
{
    public class OperationsViewModel
    {
        public List<OperationsViewItem> Items;
    }

    public class OperationsViewItem
    {
        public int OperationId { get; set; }
        public string TicketNumber { get; set; }
        public string PassengerName { get; set; }
        public string SegCount { get; set; }
        public string Payment { get; set; }
        public string KRSAmount { get; set; }
        public string Total { get; set; }
        public VKRSTickets.TOType OperationTypeId { get; set; }
    }
}
