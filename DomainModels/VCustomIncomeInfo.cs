using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class VCustomIncomeInfo
    {
        [Key]
        public Guid Id { get; set; }

        public decimal Amount { get; set; }
        public DateTime OperationDateTime { get; set; }
        public int GroupId { get; set; }
    }
}
