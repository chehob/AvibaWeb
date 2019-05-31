using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AvibaWeb.Models;

namespace AvibaWeb.DomainModels
{
    public class Expenditure
    {
        [Key]
        public int ExpenditureId { get; set; }

        public string Name { get; set; }

        [Column(TypeName = "Money")]
        public decimal Amount { get; set; }

        public int TypeId { get; set; }
        [ForeignKey("TypeId")]
        public virtual ExpenditureType Type { get; set; }

        public int ObjectId { get; set; }
        [ForeignKey("ObjectId")]
        public virtual ExpenditureObject Object { get; set; }

        public int DeskGroupId { get; set; }
        [ForeignKey("DeskGroupId")]
        public virtual DeskGroup DeskGroup { get; set; }

        public virtual ICollection<ExpenditureOperation> Operations { get; set; }
    }
}