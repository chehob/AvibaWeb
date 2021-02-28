using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class ServiceReceipt
    {
        public enum SRType
        {
            [Display(Name = "Багаж")]
            Luggage,
            [Display(Name = "Билет аэро")]
            AeroTicket,
            [Display(Name = "Билет сайта")]
            WebSiteTicket,
            [Display(Name = "Доп.услуги")]
            Custom,
            [Display(Name = "Билет ж/д")]
            RailwayTicket
        }

        public enum SRCustomOperationType
        {
            [Display(Name = "Продажа")]
            Sale = 1,
            [Display(Name = "Возврат")]
            Refund = 2,
            [Display(Name = "Обмен")]
            Exchange = 3,
            [Display(Name = "Выбор места")]
            Seat = 11,
            [Display(Name = "Продление регистрации")]
            Registration,
            [Display(Name = "Справка")]
            Reference,
            [Display(Name = "Копия билета")]
            TicketCopy,
            [Display(Name = "Доп.услуги")]
            CustomService,
            [Display(Name = "Другие")]
            Other,
            [Display(Name = "Услуги Победа")]
            PobedaService,
            [Display(Name = "Штраф за обмен")]
            ExchangePenalty = 19,
            [Display(Name = "Штраф за возврат")]
            RefundPenalty = 21,
            [Display(Name = "Расписка")]
            Receipt = 23
        }

        [Key]
        public int ServiceReceiptId { get; set; }

        public int? ServiceOperationId { get; set; }

        public SRType Type { get; set; }

        public string Serie { get; set; }

        public int Number { get; set; }

        public bool IsCanceled { get; set; }

        // Service document number for binding on insert after receipt
        public string DelayedInsertBSONumber { get; set; }

        public SRCustomOperationType? CustomOperationType { get; set; }

        // external service number
        public string EMDNumber { get; set; }

        public bool IsFiltered { get; set; } = false;

        public virtual ICollection<ServiceReceiptItem> Items { get; set; }
    }
}
