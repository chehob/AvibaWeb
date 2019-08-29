using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class VServiceReceiptIncomeInfo
    {
        [Key]
        public Guid Id { get; set; }

        public decimal Amount { get; set; }
        public DateTime DateTime { get; set; }
        public string DeskIssuedId { get; set; }
    }
}
