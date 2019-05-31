using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class CorporatorReceiptOperation
    {
        public enum CROType
        {
            [Display(Name = "Оформлен")]
            New,
            [Display(Name = "Отмена")]
            Cancelled
        }

        public int CorporatorReceiptOperationId { get; set; }

        public int CorporatorReceiptId { get; set; }
        [ForeignKey("CorporatorReceiptId")]
        public virtual CorporatorReceipt Receipt { get; set; }

        public CROType OperationTypeId { get; set; }

        public DateTime OperationDateTime { get; set; }
    }
}
