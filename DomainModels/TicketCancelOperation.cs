using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class TicketCancelOperation
    {
        public enum TCOType
        {
            [Display(Name = "Аннулирован")]
            Accepted,
            [Display(Name = "Отмена")]
            Rejected
        }

        public int TicketCancelOperationId { get; set; }

        public int TicketId { get; set; }

        [MaxLength(128)]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual AppUser Manager { get; set; }

        public string Description { get; set; }

        public TCOType OperationTypeId { get; set; }

        public DateTime OperationDateTime { get; set; }
    }
}
