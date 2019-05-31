using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class VBookingCorporator
    {
        [Key]
        public Guid Id { get; set; }
        public int CorporatorId { get; set; }
        public string ITN { get; set; }
    }
}
