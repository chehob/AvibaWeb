using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class LoanExpenditureOperation
    {
        public enum LEOType
        {
            [Display(Name = "Оформлена")]
            New,
            [Display(Name = "Отмена")]
            Cancelled
        }

        public int LoanExpenditureOperationId { get; set; }

        public int LoanExpenditureId { get; set; }
        [ForeignKey("LoanExpenditureId")]
        public virtual LoanExpenditure LoanExpenditure { get; set; }

        public LEOType OperationTypeId { get; set; }

        public DateTime OperationDateTime { get; set; }
    }
}
