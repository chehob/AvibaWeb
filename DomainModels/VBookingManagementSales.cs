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
        public decimal PenaltySum { get; set; }

        public int? TypeID { get; set; }

        public string DeskID { get; set; }
        public DateTime ExecutionDateTime { get; set; }
        public string AirlineCode { get; set; }
        public string Origin { get; set; }
        public string OriginEn { get; set; }
        public string Destination { get; set; }
        public string DestinationEn { get; set; }

        public int SegCount { get; set; }

        public string Session { get; set; }
    }
}
