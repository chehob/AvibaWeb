using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class OfficeDebit
    {
        [Key]
        public int OfficeDebitId { get; set; }

        [DisplayName("Сумма")]
        public decimal Amount { get; set; }

        [DisplayName("Комментарий")]
        public string Description { get; set; }

        public virtual ICollection<OfficeDebitOperation> Operations { get; set; }
    }
}
