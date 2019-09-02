using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class VAirlines
    {
        [Key]
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string FullName { get; set; }
    }
}
