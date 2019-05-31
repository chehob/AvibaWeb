using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class KRSCancelRequestOperation
    {
        public enum KCROType
        {
            [Display(Name = "Новый")]
            New,
            [Display(Name = "Принят")]
            Accepted,
            [Display(Name = "Отказ")]
            Rejected
        }

        public int KRSCancelRequestOperationId { get; set; }

        public int KRSCancelRequestId { get; set; }
        [ForeignKey("KRSCancelRequestId")]
        public virtual KRSCancelRequest Request { get; set; }

        public KCROType OperationTypeId { get; set; }

        public DateTime OperationDateTime { get; set; }

        [MaxLength(128)]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual AppUser Manager { get; set; }
    }
}
