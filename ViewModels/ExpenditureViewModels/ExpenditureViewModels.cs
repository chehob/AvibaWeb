using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AvibaWeb.DomainModels;
using AvibaWeb.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using static AvibaWeb.DomainModels.ExpenditureOperation;

namespace AvibaWeb.ViewModels.ExpenditureViewModels
{
    public class ExpenditureViewItem
    {
        public int ExpenditureId { get; set; }

        public int ExpenditureOperationId { get; set; }

        [Display(Name = "Наименование")]
        public string Name { get; set; }

        [Display(Name = "Сумма")]
        public string Amount { get; set; }

        [Display(Name = "Подразделение")]
        public string DeskGroup { get; set; }

        [Display(Name = "Тип операции")]
        public string Type { get; set; }

        [Display(Name = "Статья расходов")]
        public string Object { get; set; }

        [Display(Name = "Дата создания")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy H:mm}")]
        public DateTime IssuedDateTime { get; set; }

        [Display(Name = "Статус")]
        public ExpenditureOperation.EOType Status { get; set; }
    }

    public class CreateExpenditureModel
    {
        [Display(Name = "Наименование")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Сумма")]
        public decimal Amount { get; set; }

        [Required]
        public int SelectedDeskGroupId { get; set; }

        [Required]
        public int SelectedTypeId { get; set; }

        [Required]
        public int SelectedObjectId { get; set; }

        [Display(Name = "Подразделение")]
        public IEnumerable<SelectListItem> DeskGroups { get; set; }

        [Display(Name = "Тип операции")]
        public IEnumerable<SelectListItem> Types { get; set; }

        [Display(Name = "Статья расходов")]
        public IEnumerable<SelectListItem> Objects { get; set; }
    }

    public class ExpendituresViewModel
    {
        public List<ExpenditureViewItem> Items { get; set; }
    }

    public class ExpenditureViewItemGroup
    {
        public EOType Status { get; set; }
        public int? IncomingExpenditureId { get; set; }
        public string Description { get; set; }
        public List<ExpenditureViewItem> Items { get; set; }
    }

    public class CashlessExpendituresViewModel
    {
        public List<ExpenditureViewItemGroup> ItemGroups { get; set; }
    }

    public enum ExpenditureSummaryGrouping
    {
        ByObject = 1,
        ByDeskGroup = 2
    }

    public class ExpenditureSummaryViewItem
    {
        [Display(Name = "Наименование")]
        public string Name { get; set; }

        [Display(Name = "Сумма")]
        public string Amount { get; set; }
    }

    public class ExpenditureSummaryViewItemGroup
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public string AmountStr { get; set; }
        public List<ExpenditureSummaryViewItem> Items { get; set; }
    }

    public class CashlessExpenditureSummaryViewModel
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Amount { get; set; }
        public List<ExpenditureSummaryViewItemGroup> ItemGroups { get; set; }
    }
}