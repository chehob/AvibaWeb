using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class VTicketCancelList
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
        public string ManagerName { get; set; }
        public TicketCancelOperation.TCOType? OperationTypeId { get; set; }
        public string BSONumber { get; set; }
        public DateTime? DealDateTime { get; set; }
        public DateTime TicketOperationDateTime { get; set; }
        public string PassengerName { get; set; }
        public string Route { get; set; }
        public decimal Payment { get; set; }
        public string Description { get; set; }
        public TOType OperationType { get; set; }
        public int TicketId { get; set; }
    }
}
