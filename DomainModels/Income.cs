using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class Income
    {
        [Key]
        public int IncomeId { get; set; }

        public string Name { get; set; }

        [Column(TypeName = "Money")]
        public decimal Amount { get; set; }

        public int DeskGroupId { get; set; }
        [ForeignKey("DeskGroupId")]
        public virtual DeskGroup DeskGroup { get; set; }

        public virtual ICollection<IncomeOperation> Operations { get; set; }
    }
}
