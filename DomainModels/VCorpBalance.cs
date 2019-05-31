using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class VCorpBalance
    {
        [Key]
        public Guid Id { get; set; }
        public string CorpName { get; set; }
        public decimal Balance { get; set; }
        public DateTime LastPaymentDate { get; set; }
        public DateTime LastReceiptDate { get; set; }
    }
}
