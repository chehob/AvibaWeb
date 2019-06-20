using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AvibaWeb.DomainModels;
using static AvibaWeb.DomainModels.CorporatorReceiptMultiPayment;

namespace AvibaWeb.ViewModels.CorpReceiptViewModels
{
    public class MultiPaymentItem
    {
        public string Amount { get; set; }
        public string CounterpartyName { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public int PaymentId { get; set; }
        public bool IsProcessed { get; set; }
    }

    public class MultiPaymentViewModel
    {
        public CRMPType Type { get; set; }
        public List<MultiPaymentItem> Items { get; set; }
    }

    public class MultiPaymentReceipt
    {
        public decimal Amount { get; set; }
        public string AmountStr { get; set; }
        public int ReceiptNumber { get; set; }
        public int ReceiptId { get; set; }
        public string CorpName { get; set; }
    }

    public class MultiPaymentProcessViewModel
    {
        public CRMPType Type { get; set; }
        public CorporatorReceiptMultiPayment Payment { get; set; }
        public List<MultiPaymentReceipt> Receipts { get; set; }

        public List<KeyValuePair<string, string>> Counterparties { get; set; }
    }

    public class MultiPaymentProcessPostViewModel
    {
        public int PaymentId { get; set; }
        public List<MultiPaymentReceipt> Receipts { get; set; }
    }
}
