using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class VBookingManagementOperation
    {
        [Key]
        public Guid Id { get; set; }
        public string TicketID { get; set; }
        public int Parent { get; set; }

        public string Airline { get; set; }
        public string Flight { get; set; }
        public DateTime FlightDate { get; set; }

        public int OperationID { get; set; }
        public string OperationType { get; set; }
        
        public decimal Fare { get; set; }
        public decimal TaxAmount { get; set; }
        public string PaymentType { get; set; }
        public decimal Penalty { get; set; }
        public string KRSTax { get; set; }

        public string BookDeskID { get; set; }
        public DateTime? BookDateTime { get; set; }

        public string Phone { get; set; }
        public string Email { get; set; }

        public string FullName { get; set; }

        public string DeskID { get; set; }
        public DateTime ExecutionDateTime { get; set; }
        public string AirlineCode { get; set; }
        public string Origin { get; set; }
        public string OriginEn { get; set; }
        public string Destination { get; set; }
        public string DestinationEn { get; set; }
        public string Session { get; set; }
    }
}
