using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class LoanGroup
    {
        [Key]
        public int LoanGroupId { get; set; }

        [Display(Name = "Наименование")]
        public string Description { get; set; }

        [Column(TypeName = "Money")]
        public decimal Balance { get; set; }

        [Display(Name = "Активен")]
        [DefaultValue(true)]
        public bool IsActive { get; set; } = true;

        public List<Counterparty> Counterparties { get; set; }
    }
}
