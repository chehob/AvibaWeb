using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AvibaWeb.DomainModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AvibaWeb.ViewModels.LoanExpenditureViewModels
{
    public class CreateLoanExpenditureModel
    {
        [Display(Name = "Наименование")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Сумма")]
        public decimal Amount { get; set; }
    }

    public class LoanExpendituresViewModel
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public List<LoanExpenditureViewItem> Items { get; set; }
    }

    public class LoanExpenditureViewItem
    {
        public int LoanExpenditureId { get; set; }

        public int LoanExpenditureOperationId { get; set; }

        [Display(Name = "Наименование")]
        public string Description { get; set; }

        [Display(Name = "Сумма")]
        public string Amount { get; set; }

        [Display(Name = "Дата создания")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy H:mm}")]
        public DateTime IssuedDateTime { get; set; }

        [Display(Name = "Статус")]
        public LoanExpenditureOperation.LEOType Status { get; set; }
    }
}
