using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class LoanExpenditure
    {
        [Key]
        public int LoanExpenditureId { get; set; }

        public string Description { get; set; }

        [Column(TypeName = "Money")]
        public decimal Amount { get; set; }
    }
}
