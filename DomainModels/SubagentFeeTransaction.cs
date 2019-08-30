using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class SubagentFeeTransaction
    {
        [Key]
        public int SubagentFeeTransactionId { get; set; }

        public string SubagentId { get; set; }
        [ForeignKey("SubagentId")]
        public virtual SubagentData SubagentData { get; set; }

        [Column(TypeName = "Money")]
        public decimal Amount { get; set; }

        public DateTime TransactionDateTime { get; set; }
    }
}
