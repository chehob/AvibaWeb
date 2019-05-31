using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class OfficeDebitOperation
    {
        public enum ODOType
        {
            [Display(Name = "Оформлена")]
            New,
            [Display(Name = "Отмена")]
            Cancelled
        }

        public int OfficeDebitOperationId { get; set; }

        public int OfficeDebitId { get; set; }
        [ForeignKey("OfficeDebitId")]
        public virtual OfficeDebit OfficeDebit { get; set; }

        public ODOType OperationTypeId { get; set; }

        public DateTime OperationDateTime { get; set; }
    }
}
