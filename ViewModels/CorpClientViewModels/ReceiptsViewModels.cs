using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static AvibaWeb.DomainModels.CorporatorReceipt;

namespace AvibaWeb.ViewModels.CorpClientViewModels
{
    public class ReceiptsViewModel
    {
        public List<KeyValuePair<string, string>> Counterparties { get; set; }
        public List<KeyValuePair<string, string>> Organizations { get; set; }
    }

    public class ReceiptListItem
    {
        public int ReceiptId { get; set; }
        public string ReceiptNumber { get; set; }
        public string CreatedDate { get; set; }
        public string IssuedDateTime { get; set; }
        public string PaidDateTime { get; set; }
        public string PayeeBankName { get; set; }
        public string PayeeOrgName { get; set; }
        public string PayerOrgName { get; set; }
        public string TotalStr { get; set; }
        public string PartialStr { get; set; }
        public CRPaymentStatus Status { get; set; }
        public int TicketsToPDFCount { get; set; }
    }

    public class ReceiptListViewModel
    {
        public string AviaSegTotal { get; set; }
        public string ZdSegTotal { get; set; }
        public string LuggageSegTotal { get; set; }
        public string AviaCostTotal { get; set; }
        public string ZdCostTotal { get; set; }
        public string LuggageCostTotal { get; set; }
        public string FeeTotal { get; set; }
        public string AviaFeeTotal { get; set; }
        public string ZdFeeTotal { get; set; }
        public string LuggageFeeTotal { get; set; }

        public List<ReceiptListItem> Items { get; set; }
    }

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
