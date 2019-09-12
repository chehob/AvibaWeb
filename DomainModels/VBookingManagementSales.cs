using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class VBookingManagementSales
    {
        [Key]
        public Guid Id { get; set; }
        public int OperationTypeID { get; set; }

        public decimal CashAmount { get; set; }
        public decimal ChildCashAmount { get; set; }
        public decimal PKAmount { get; set; }
        public decimal ChildPKAmount { get; set; }
        public decimal BNAmount { get; set; }
        public decimal ChildBNAmount { get; set; }

        public int PenaltyCount { get; set; }
        public decimal CashPenalty { get; set; }
        public decimal PKPenalty { get; set; }
        public decimal BNPenalty { get; set; }
        public decimal ChildCashPenalty { get; set; }
        public decimal ChildPKPenalty { get; set; }
        public decimal ChildBNPenalty { get; set; }

        public int TypeId { get; set; }

        public string DeskId { get; set; }
        public DateTime ExecutionDateTime { get; set; }
        public string AirlineCode { get; set; }
        public string Origin { get; set; }
        public string OriginEn { get; set; }
        public string Destination { get; set; }
        public string DestinationEn { get; set; }

        public string Session { get; set; }

        public int TicketID { get; set; }
    }
}
