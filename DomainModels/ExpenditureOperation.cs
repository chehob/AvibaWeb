using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvibaWeb.DomainModels
{
    public class ExpenditureOperation
    {
        public enum EOType
        {
            [Display(Name = "Оформлена")]
            New,
            [Display(Name = "Отмена")]
            Cancelled
        }

        public int ExpenditureOperationId { get; set; }

        public int ExpenditureId { get; set; }
        [ForeignKey("ExpenditureId")]
        public virtual Expenditure Expenditure { get; set; }

        public EOType OperationTypeId { get; set; }

        public DateTime OperationDateTime { get; set; }
    }
}