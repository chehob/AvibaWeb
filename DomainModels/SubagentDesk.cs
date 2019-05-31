using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class SubagentDesk
    {
        [Key]
        public int SubagentDeskId { get; set; }

        public string SubagentId { get; set; }
        [ForeignKey("SubagentId")]
        public virtual SubagentData Subagent { get; set; }

        public string DeskId { get; set; }
        [ForeignKey("DeskId")]
        public virtual Desk Desk { get; set; }
    }
}
