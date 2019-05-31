using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class VTicketPDFInfo
    {
        [Key]
        public Guid Id { get; set; }
        public int TicketID { get; set; }

        public string AirlineCode { get; set; }
        public long BSONumber { get; set; }

        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Patronymic { get; set; }
        public string DocumentType { get; set; }
        public string Passport { get; set; }

        public string AirlineName { get; set; }
        public string PNRID { get; set; }
        public DateTime DealDateTime { get; set; }

        public string FareCalc { get; set; }
        public decimal Fare { get; set; }
        public decimal RealFare { get; set; }
        public decimal VatAmount { get; set; }
        public string BasicFare { get; set; }
        public string AirlineShortName { get; set; }
        public int OperationTypeID { get; set; }
        public string Endorsements { get; set; }
        public string ConfirmationCode { get; set; }
        public string Stamp { get; set; }
        public int? Number { get; set; }
        public decimal AgeDiff { get; set; }
        public string ParentAirline { get; set; }
        public string ParentTicket { get; set; }
        public int? TypeID { get; set; }
        public int TicketOperationID { get; set; }

        public int IsExchange { get; set; }
        public decimal? ZZFee { get; set; }
        public decimal? AddFee { get; set; }
        public string ExBSONumber { get; set; }
    }
}
