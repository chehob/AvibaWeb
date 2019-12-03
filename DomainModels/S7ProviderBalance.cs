using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class S7ProviderBalance
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "Money")]
        public decimal Balance { get; set; }
    }
}
