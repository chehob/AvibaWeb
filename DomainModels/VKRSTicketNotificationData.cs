using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class VKRSTicketNotificationData
    {
        [Key]
        public Guid Id { get; set; }
        public int OperationId { get; set; }
        public DateTime OperationDateTime { get; set; }
        public string TicketNumber { get; set; }
        public string PassengerName { get; set; }
        public string Phone { get; set; }        
    }
}
