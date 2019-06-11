using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class OfficeBalanceHistory
    {
        [Key]
        public int OfficeBalanceHistoryId { get; set; }

        public decimal Balance { get; set; }

        public DateTime SaveDateTime { get; set; }
    }
}
