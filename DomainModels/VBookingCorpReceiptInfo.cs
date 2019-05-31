using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class VBookingCorpReceiptInfo
    {
        [Key]
        public Guid Id { get; set; }
        public int ReceiptId { get; set; }
        public int ReceiptNumber { get; set; }
        public decimal Amount { get; set; }
    }
}
