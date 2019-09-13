using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.ViewModels.ReportViewModels
{
    public class FinalSummaryViewItem
    {
        public FinalSummaryViewItem()
        {
            nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";
        }
        private NumberFormatInfo nfi;

        public string Name { get; set; }
        public int DeskGroupId { get; set; }

        public decimal IncomeAmount { get; set; }
        public string IncomeAmountStr => IncomeAmount.ToString("#,0.00", nfi);
        public decimal ExpenditureAmount { get; set; }
        public string ExpenditureAmountStr => ExpenditureAmount.ToString("#,0.00", nfi);

        public string TotalAmountStr => (IncomeAmount - ExpenditureAmount).ToString("#,0.00", nfi);

        public decimal SalesAmount { get; set; }
        public string SalesAmountStr => SalesAmount.ToString("#,0.00", nfi);
    }

    public class FinalSummaryViewModel
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public ICollection<FinalSummaryViewItem> Items { get; set; }
    }
}
