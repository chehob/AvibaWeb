using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class VBookingManagementLuggage
    {
        [Key]
        public Guid Id { get; set; }

        public decimal LuggageAmount { get; set; }
        public DateTime DateTime { get; set; }
        public string DeskID { get; set; }
        public decimal LuggageRate { get; set; }
        public string Airline { get; set; }
        public string Session { get; set; }
    }
}
