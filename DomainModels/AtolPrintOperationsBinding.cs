using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class AtolPrintOperationsBinding
    {
        [Key]
        public int AtolPrintOperationsBindingId { get; set; }

        public int PrintOperationId { get; set; }
        public int TypeId { get; set; }
        public int _1CStatus { get; set; }
    }
}
