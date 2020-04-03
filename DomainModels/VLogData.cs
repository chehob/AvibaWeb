using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class VLogData
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime OperationDateTime { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public decimal OldBalance { get; set; }
        public decimal Delta { get; set; }
        public decimal NewBalance { get; set; }
    }
}
