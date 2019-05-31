using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class SubagentData
    {
        [Key]
        public string SubagentId { get; set; }
        [ForeignKey("SubagentId")]
        public virtual Counterparty Subagent { get; set; }

        [Column(TypeName = "Money")]
        public decimal Balance { get; set; }

        [Column(TypeName = "Money")]
        public decimal Deposit { get; set; }

        public virtual ICollection<SubagentDesk> SubagentDesks { get; set; }
    }
}
