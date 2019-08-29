using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class ServiceReceiptOperation
    {
        public enum SROType
        {
            [Display(Name = "Продажа")]
            Sale,
            [Display(Name = "Аннуляция")]
            Cancel
        }

        [Key]
        public int ServiceReceiptOperationId { get; set; }

        public int ServiceReceiptId { get; set; }
        [ForeignKey("ServiceReceiptId")]
        public virtual ServiceReceipt Receipt { get; set; }

        public string DeskIssuedId { get; set; }
        [ForeignKey("DeskIssuedId")]
        public virtual Desk DeskIssued { get; set; }

        public DateTime DateTime { get; set; }

        public SROType Type { get; set; }
    }
}
