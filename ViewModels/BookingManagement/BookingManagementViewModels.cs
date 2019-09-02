using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.ViewModels.BookingManagement
{
    public class SalesViewItem
    {
        public SalesViewItem(bool isEmptyRow = false)
        {
            IsEmptyRow = isEmptyRow;
            nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";
        }
        private NumberFormatInfo nfi;

        public bool IsEmptyRow { get; set; } = false;
        public string Description { get; set; }
        public decimal SegCount { get; set; }
        public string SegCountStr => IsEmptyRow ? "" : SegCount.ToString();
        public decimal AmountCash { get; set; }
        public string AmountCashStr => IsEmptyRow ? "" : AmountCash.ToString("#,0.00", nfi);
        public decimal AmountPK { get; set; }
        public string AmountPKStr => IsEmptyRow ? "" : AmountPK.ToString("#,0.00", nfi);
        public decimal AmountBN { get; set; }
        public string AmountBNStr => IsEmptyRow ? "" : AmountBN.ToString("#,0.00", nfi);
        public string Total => IsEmptyRow ? "" : (AmountCash + AmountPK + AmountBN).ToString("#,0.00", nfi);
        public string SegCountCustomStyle { get; set; }
        public string AmountCashCustomStyle { get; set; }
        public string AmountPKCustomStyle { get; set; }
        public string AmountBNCustomStyle { get; set; }
        public string AmountTotalCustomStyle { get; set; }
    }

    public class SalesViewModel
    {
        public List<SalesViewItem> Items { get; set; }
    }
}
