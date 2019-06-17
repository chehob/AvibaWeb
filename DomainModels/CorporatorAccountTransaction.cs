using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class CorporatorAccountTransaction
    {
        public enum CATType
        {
            Payment,
            Receipt
        }

        [Key]
        public int ProviderBalanceTransactionId { get; set; }

        public int AccountId { get; set; }
        [ForeignKey("AccountId")]
        public virtual CorporatorAccount CorporatorAccount { get; set; }

        [Column(TypeName = "Money")]
        public decimal OldBalance { get; set; }

        [Column(TypeName = "Money")]
        public decimal Amount { get; set; }

        public int TransactionItemId { get; set; }

        public DateTime TransactionDateTime { get; set; }

        public CATType TypeId { get; set; }
    }
}
