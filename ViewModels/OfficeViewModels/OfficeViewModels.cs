using AvibaWeb.DomainModels;
using System;
using System.ComponentModel.DataAnnotations;

namespace AvibaWeb.ViewModels.OfficeViewModels
{
    public class OfficeDebitListViewModel
    {
        public int DebitId { get; set; }

        [Display(Name = "Наименование")]
        public string Description { get; set; }

        [Display(Name = "Сумма")]
        public decimal Amount { get; set; }

        [Display(Name = "Дата создания")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy H:mm}")]
        public DateTime IssuedDateTime { get; set; }

        [Display(Name = "Статус")]
        public OfficeDebitOperation.ODOType Status { get; set; }
    }
}
