using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AvibaWeb.DomainModels;

namespace AvibaWeb.ViewModels.CorpReceiptViewModels
{
    public class MultiPaymentViewModel
    {
        public string Amount { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public int PaymentId { get; set; }
    }

    public class MultiPaymentReceipt
    {
        public string Amount { get; set; }
        public int ReceiptNumber { get; set; }
    }

    public class MultiPaymentProcessViewModel
    {
        public CorporatorReceiptMultiPayment Payment { get; set; }
        public List<MultiPaymentReceipt> Receipts { get; set; }
    }
}
