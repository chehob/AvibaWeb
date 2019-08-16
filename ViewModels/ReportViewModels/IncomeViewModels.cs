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
    public class IncomeSummaryViewItemGroup
    {
        public IncomeSummaryViewItemGroup()
        {
            nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";
        }
        private NumberFormatInfo nfi;

        public string Name { get; set; }
        public string Amount => (AmountKRS + AmountCorp).ToString("#,0.00", nfi);
        public decimal AmountKRS { get; set; }
        public string AmountKRSStr => AmountKRS.ToString("#,0.00", nfi);
        public decimal AmountCorp { get; set; }
        public string AmountCorpStr => AmountCorp.ToString("#,0.00", nfi);
    }

    public class IncomeSummaryViewModel
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string AmountKRS { get; set; }
        public List<IncomeSummaryViewItemGroup> ItemGroups { get; set; }
    }
}