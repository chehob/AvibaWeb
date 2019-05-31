using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AvibaWeb.DomainModels;

namespace AvibaWeb.ViewModels.ManagementViewModels
{
    public class TicketCancelViewModel
    {
        [Display(Name = "Статус")] public TicketCancelOperation.TCOType? Status { get; set; }

        [Display(Name = "Сумма билета")] public string Payment { get; set; }
        [Display(Name = "Номер билета")] public string BSONumber { get; set; }
        [Display(Name = "Пассажир")] public string PassengerName { get; set; }
        [Display(Name = "Маршрут")] public string Route { get; set; }

        [Display(Name = "Дата создания")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy H:mm}")]
        public DateTime? DealDateTime { get; set; }

        [Display(Name = "Дата операции")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy H:mm}")]
        public DateTime TicketOperationDateTime { get; set; }

        [Display(Name = "Комментарий")] public string Description { get; set; }

        [Display(Name = "Менеджер")] public string ManagerName { get; set; }

        [Display(Name = "Статус билета")] public VTicketCancelList.TOType TicketStatus { get; set; }

        public int TicketId { get; set; }
    }

    public class TicketListRequest
    {
        public string fromDate { get; set; }
        public string toDate { get; set; }
    }
}
