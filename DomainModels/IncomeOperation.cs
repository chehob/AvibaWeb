using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class IncomeOperation
    {
        public enum IOType
        {
            [Display(Name = "Оформлена")]
            New,
            [Display(Name = "Отмена")]
            Cancelled
        }

        public int IncomeOperationId { get; set; }

        public int IncomeId { get; set; }
        [ForeignKey("IncomeId")]
        public virtual Income Income { get; set; }

        public IOType OperationTypeId { get; set; }

        public DateTime OperationDateTime { get; set; }
    }
}
