using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class CorporatorReceipt
    {
        public enum CRPaymentStatus
        {
            [Display(Name = "Не оплачен")]
            Unpaid,
            [Display(Name = "Частичная оплата")]
            Partial,
            [Display(Name = "Оплачен")]
            Paid
        }

        public enum CRType
        {
            WebSite = 0,
            CorpClient
        }

        [Key]
        public int CorporatorReceiptId { get; set; }

        public int? ReceiptNumber { get; set; }

        public string CorporatorId { get; set; }
        [ForeignKey("CorporatorId")]
        public virtual Counterparty Corporator { get; set; }

        public int? PayeeAccountId { get; set; }
        [ForeignKey("PayeeAccountId")]
        public virtual FinancialAccount PayeeAccount { get; set; }

        public decimal? Amount { get; set; }

        public decimal? FeeRate { get; set; }

        public CRPaymentStatus StatusId { get; set; }

        public DateTime? IssuedDateTime { get; set; }

        public decimal? PaidAmount { get; set; }

        public DateTime? PaidDateTime { get; set; }

        public CRType TypeId { get; set; }

        public virtual ICollection<CorporatorReceiptItem> Items { get; set; }

        public virtual ICollection<CorporatorReceiptOperation> Operations { get; set; }
    }
}
