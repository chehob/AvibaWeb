using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class VSessionTypes
    {
        [Key]
        public Guid Id { get; set; }
        public int SessionId { get; set; }
        public string Name { get; set; }
    }
}
