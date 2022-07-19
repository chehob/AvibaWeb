using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.ViewModels.KRSClientViewModels
{
    public class TicketNotificationViewModel
    {
        public string Uuid { get; set; }
        public string OperationDateTime { get; set; }
        public string TicketNumber { get; set; }
        public string PassengerName { get; set; }
        public string Phone { get; set; }        
    }
}
