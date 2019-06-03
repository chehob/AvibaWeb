﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AvibaWeb.DomainModels;
using AvibaWeb.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AvibaWeb.ViewModels.ExpenditureViewModels
{
    public class ExpenditureListViewModel
    {
        public int ExpenditureId { get; set; }

        public int ExpenditureOperationId { get; set; }

        [Display(Name = "Наименование")]
        public string Name { get; set; }

        [Display(Name = "Сумма")]
        public decimal Amount { get; set; }

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

}