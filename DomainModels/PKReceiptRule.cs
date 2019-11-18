using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class PKReceiptRule
    {
        [Key]
        public int PKReceiptRuleId { get; set; }

        public decimal Rate { get; set; }
    }
}
