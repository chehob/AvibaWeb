using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class VTicketTaxPDFInfo
    {
        [Key]
        public Guid Id { get; set; }
        public int TicketID { get; set; }

        public decimal TaxAmount { get; set; }
        public string TaxType { get; set; }
    }
}
