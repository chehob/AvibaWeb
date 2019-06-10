using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class ProviderBalance
    {
        [Key]
        public string ProviderId { get; set; }
        [ForeignKey("ProviderId")]
        public virtual Counterparty Provider { get; set; }

        [Column(TypeName = "Money")]
        public decimal Balance { get; set; }

        [Column(TypeName = "Money")]
        public decimal Deposit { get; set; }

        public virtual ICollection<ProviderBalanceTransaction> Transactions { get; set; }
    }
}
