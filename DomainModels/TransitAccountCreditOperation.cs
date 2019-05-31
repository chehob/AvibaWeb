using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class TransitAccountCreditOperation
    {
        public enum TACOType
        {
            [Display(Name = "Оформлен")]
            New,
            [Display(Name = "Отмена")]
            Cancelled
        }

        public int TransitAccountCreditOperationId { get; set; }

        public int TransitAccountCreditId { get; set; }
        [ForeignKey("TransitAccountCreditId")]
        public virtual TransitAccountCredit TransitAccountCredit { get; set; }

        public TACOType OperationTypeId { get; set; }

        public DateTime OperationDateTime { get; set; }
    }
}
