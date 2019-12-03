using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class VBookingManagementPaycheck
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CheckInDateTime { get; set; }
        public string DeskId { get; set; }
    }
}
