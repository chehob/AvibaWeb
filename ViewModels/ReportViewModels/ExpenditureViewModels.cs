using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using AvibaWeb.DomainModels;
using AvibaWeb.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using static AvibaWeb.DomainModels.ExpenditureOperation;

namespace AvibaWeb.ViewModels.ReportViewModels
{
    public enum ExpenditureSummaryGrouping
    {
        ByObject = 1,
        ByDeskGroup = 2
    }

    public class ExpenditureSummaryViewItem
    {
        public ExpenditureSummaryViewItem()
        {
            nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";
        }
        private NumberFormatInfo nfi;

        public int DeskGroupId { get; set; }
        public int ObjectId { get; set; }

        [Display(Name = "Наименование")]
        public string Name { get; set; }

        [Display(Name = "Сумма")]
        public string Amount => (AmountCash + AmountCashless).ToString("#,0.00", nfi);

        public decimal AmountCash { get; set; }
        public decimal AmountCashless { get; set; }

        public string AmountCashStr => AmountCash.ToString("#,0.00", nfi);
        public string AmountCashlessStr => AmountCashless.ToString("#,0.00", nfi);
    }

    public class ExpenditureSummaryViewItemGroup
    {
        public ExpenditureSummaryViewItemGroup()
        {
            nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";
        }
        private NumberFormatInfo nfi;
        
        public string Name { get; set; }
        public decimal Amount => AmountCash + AmountCashless;
        public decimal AmountCash { get; set; }
        public decimal AmountCashless { get; set; }
        public string AmountStr => Amount.ToString("#,0.00", nfi);
        public string AmountCashStr => AmountCash.ToString("#,0.00", nfi);
        public string AmountCashlessStr => AmountCashless.ToString("#,0.00", nfi);
        public List<ExpenditureSummaryViewItem> Items { get; set; }
    }

    public class ExpenditureSummaryViewModel
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Amount { get; set; }
        public string AmountCash { get; set; }
        public string AmountCashless { get; set; }
        public List<ExpenditureSummaryViewItemGroup> ItemGroups { get; set; }
    }

    public class ExpenditureSummaryOperationsItem
    {
        public string OperationDateTime { get; set; }
        public string Amount { get; set; }
        public string PaymentType { get; set; }
    }

    public class ExpenditureSummaryOperationsViewModel
    {
        public List<ExpenditureSummaryOperationsItem> Items { get; set; }
    }
}