using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.ViewModels.CorpClientViewModels
{
    public class ReviseReportViewModel
    {
        public List<KeyValuePair<string, string>> Counterparties { get; set; }
        public List<KeyValuePair<int, string>> Organizations { get; set; }
    }

    public class ReviseReportPDFViewModel
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string OrgName { get; set; }
        public string PayerName { get; set; }
        public string OldDebit { get; set; }
        public string OldCredit { get; set; }
        public List<ReviseReportPDFItem> Items { get; set; }
        public string Debit { get; set; }
        public string Credit { get; set; }
        public decimal Balance { get; set; }
        public string NewDebit { get; set; }
        public string NewCredit { get; set; }
        public string SignatureFileName { get; set; }
        public string StampFileName { get; set; }
        public string SignatureImage { get; set; }
        public string StampImage { get; set; }
    }

    public class ReviseReportPDFItem
    {
        public ReviseReportPDFItem()
        {
            nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";
            nfi.NumberDecimalSeparator = ",";
        }
        private NumberFormatInfo nfi;

        public int Rank { get; set; }
        public DateTime Date { get; set; }
        public string DateStr { get; set; }
        public string Label { get; set; }
        public decimal? Debit { get; set; }
        public string DebitStr => Debit != null ? Debit.Value.ToString("0.00", nfi) : "";
        public decimal? Credit { get; set; }
        public string CreditStr => Credit != null ? Credit.Value.ToString("0.00", nfi) : "";
    }
}
