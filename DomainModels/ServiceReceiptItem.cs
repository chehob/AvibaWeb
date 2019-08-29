using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class ServiceReceiptItem
    {
        [Key]
        public int ServiceReceiptItemId { get; set; }

        public int ServiceReceiptId { get; set; }
        [ForeignKey("ServiceReceiptId")]
        public virtual ServiceReceipt Receipt { get; set; }

        public int? SegmentId { get; set; }

        public decimal Amount { get; set; }

        public bool IsCanceled { get; set; }
    }
}
