using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class VKRSTickets
    {
        public enum TOType
        {
            [Display(Name = "Продажа")]
            Sale = 1,
            [Display(Name = "Возврат")]
            Refund,
            [Display(Name = "Принят в обмен")]
            ExchangeOld,
            [Display(Name = "Выдан в обмен")]
            ExchangeNew,
            [Display(Name = "Отмена")]
            Cancel,
            [Display(Name = "Вынужденный возврат")]
            ForcedRefund,
            [Display(Name = "Вынужденный обмен")]
            ForcedExchange
        }

        [Key]
        public Guid Id { get; set; }
        public int OperationId { get; set; }
        public string TicketNumber { get; set; }
        public string PassengerName { get; set; }
        public string SegCount { get; set; }        
        public decimal Payment { get; set; }
        public decimal KRSAmount { get; set; }
        public TOType OperationTypeId { get; set; }
        public DateTime OperationDateTime { get; set; }
    }
}
