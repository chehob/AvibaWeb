using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static AvibaWeb.DomainModels.CorporatorReceipt;

namespace AvibaWeb.ViewModels.CorpReceiptViewModels
{
    public class CorpReceiptsViewModel
    {
        public int SubGroupId { get; set; }
        public List<CorpReceiptsItem> Items { get; set; }
    }

    public class CorpReceiptsItem
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

    public class ReceiptPDFItem
    {
        public decimal Amount { get; set; }
        public string AmountStr { get; set; }
        public string TicketLabel { get; set; }
        public int SegCount { get; set; }
        public string AmountLabelStr { get; set; }
    }

    public class ReceiptTaxItem
    {
        public string FeeStr { get; set; }
        public decimal Amount { get; set; }
        public string AmountStr { get; set; }
        public int SegCount { get; set; }
        public string AmountLabelStr { get; set; }
        public string TicketLabel { get; set; }
    }

    public class ReceiptPDFViewModel
    {
        public int ReceiptId { get; set; }

        public decimal TotalAmount { get; set; }
        public string TotalAmountStr { get; set; }
        public decimal ItemTotal { get; set; }
        public string ItemTotalStr { get; set; }
        public string ReceiptNumber { get; set; }
        public string PayerName { get; set; }
        public string PayerNameWithITN { get; set; }
        public string PayerAddress { get; set; }
        public string PayerCorrAccount { get; set; }
        public string PayerFinancialAccount { get; set; }
        public string PayerBankName { get; set; }
        public string PayerBIK { get; set; }
        public string PayerITN { get; set; }
        public string PayerKPP { get; set; }
        public string PayerHeadTitle { get; set; }
        public string PayerHeadName { get; set; }
        public List<ReceiptPDFItem> Items { get; set; }
        public List<ReceiptPDFItem> LuggageItems { get; set; }
        public List<ReceiptTaxItem> Taxes { get; set; }
        public string SignatureImage { get; set; }
        public string StampImage { get; set; }
        public string SignatureFileName { get; set; }
        public string StampFileName { get; set; }
        public string OrgHeadTitle { get; set; }
        public string OrgHeadName { get; set; }
        public string OrgITN { get; set; }
        public string OrgKPP { get; set; }
        public string OrgName { get; set; }
        public string OrgCorrAccount { get; set; }
        public string OrgFinancialAccount { get; set; }
        public string OrgBankName { get; set; }
        public string OrgBIK { get; set; }
        public string OrgAddress { get; set; }
        public decimal FeeRate { get; set; }
        public string FeeRateStr { get; set; }
        public decimal FeeTotal { get; set; }
        public string FeeTotalStr { get; set; }
        public int SegCountTotal { get; set; }
        public string IssuedDateTime { get; set; }
        public string PaymentTemplateLabelStr { get; set; }
        public string PaymentTemplateStr { get; set; }
    }
}
