using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.ViewModels.CorpClientViewModels
{
    public class TicketPDFViewModel
    {
        public List<TicketPDFData> Tickets { get; set; }   
    }

    public class TicketPDFData
    {
        public int TicketId { get; set; }
        public string BlankImage { get; set; }
        public string TicketNumber { get; set; }
        public string PassengerName { get; set; }
        public string DocType { get; set; }
        public string Doc { get; set; }
        public List<SegmentPDFData> Seg { get; set; }
        public string Stamp { get; set; }
        public string IssuedBy { get; set; }
        public string pnr { get; set; }
        public string DateOfIssue { get; set; }
        public string FareCalc { get; set; }
        public string Luggage { get; set; }
        public string Total { get; set; }
        public string Payment { get; set; }
        public string Qr { get; set; }
        public string Status { get; set; }
        public string Endorsements { get; set; }
        public decimal Fare { get; set; }
        public string Class { get; set; }
        public bool IsInfant { get; set; }
        public bool IsExchange { get; set; }
        public decimal AddFee { get; set; }
        public decimal ZZFee { get; set; }
        public string ExTicketNumber { get; set; }
    }

    public class SegmentPDFData
    {
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string Flight { get; set; }
        public string Departure { get; set; }
        public string Arrival { get; set; }
    }
}
