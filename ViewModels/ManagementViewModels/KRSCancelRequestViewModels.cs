using AvibaWeb.DomainModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.ViewModels.ManagementViewModels
{
    public class KRSCancelRequestViewModel
    {
        public int RequestId { get; set; }
        [Display(Name = "Статус")]  public KRSCancelRequestOperation.KCROType Status { get; set; }

        [Display(Name = "Сумма билета")] public string Payment { get; set; }
        [Display(Name = "Сумма КРС")] public string KRSAmount { get; set; }

        [Display(Name = "Номер билета")] public string BSONumber { get; set; }
        [Display(Name = "Номер КРС")] public string KRSNumber { get; set; }

        [Display(Name = "Пассажир")] public string PassengerName { get; set; }

        [Display(Name = "Маршрут")] public string Route { get; set; }

        [Display(Name = "Дата создания")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy H:mm}")]
        public DateTime DealDateTime { get; set; }

        [Display(Name = "Комментарий")] public string Description { get; set; }

        [Display(Name = "Кассир")] public string CashierName { get; set; }

        [Display(Name = "Пульт")] public string Desk { get; set; }

        [Display(Name = "Менеджер")] public string ManagerName { get; set; }

        [Display(Name = "Статус билета")] public VKRSCancelRequest.TOType TicketStatus { get; set; }
    }
}
