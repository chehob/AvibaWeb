using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class ProviderAgentFeeTransaction
    {
        [Key]
        public int ProviderAgentFeeTransactionId { get; set; }

        public string ProviderId { get; set; }
        [ForeignKey("ProviderId")]
        public virtual ProviderBalance ProviderBalance { get; set; }

        [Column(TypeName = "Money")]
        public decimal OldAgentFee { get; set; }

        [Column(TypeName = "Money")]
        public decimal Amount { get; set; }

        public string Comment { get; set; }

        public DateTime TransactionDateTime { get; set; }
    }
}
