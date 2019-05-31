using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class VKRSCancelRequest
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
        public int KRSCancelRequestId { get; set; }
        public string Name { get; set; }
        public string ManagerName { get; set; }
        public string Desk { get; set; }
        public KRSCancelRequestOperation.KCROType OperationTypeId { get; set; }
        public string BSONumber { get; set; }
        public string KRSNumber { get; set; }
        public DateTime DealDateTime { get; set; }
        public string PassengerName { get; set; }
        public string Route { get; set; }
        public decimal Payment { get; set; }
        public decimal KRSAmount { get; set; }
        public string Description { get; set; }
        public TOType OperationType { get; set; }
    }
}
