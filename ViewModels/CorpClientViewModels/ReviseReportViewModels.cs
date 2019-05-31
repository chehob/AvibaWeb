using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.ViewModels.CorpClientViewModels
{
    public class ReviseReportViewModel
    {
        public List<string> Counterparties { get; set; }
        public List<string> Organizations { get; set; }
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
    }

    public class ReviseReportPDFItem
    {
        public string Date { get; set; }
        public string ReceiptNumber { get; set; }
        public string Amount { get; set; }
    }
}
