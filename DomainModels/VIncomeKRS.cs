using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class VIncomeKRS
    {
        [Key]
        public Guid Id { get; set; }
        public int OperationTypeID { get; set; }
        public decimal Amount { get; set; }
        public int KRSID { get; set; }
        public int? TicketID { get; set; }
        public DateTime OperationDate { get; set; }
        public bool IsCanceled { get; set; }
        public int KRSNumber { get; set; }
        public int KRSType { get; set; }
        public string DeskID { get; set; }
    }
}
