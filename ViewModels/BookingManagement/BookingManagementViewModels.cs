using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.ViewModels.BookingManagement
{
    public class SalesViewItem
    {
        public SalesViewItem()
        {
            nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";
        }
        private NumberFormatInfo nfi;
        
        public decimal SegCount { get; set; }
        public string SegCountStr => SegCount.ToString();
        public decimal AmountCash { get; set; }
        public string AmountCashStr => AmountCash.ToString("#,0.00", nfi);
        public decimal AmountPK { get; set; }
        public string AmountPKStr => AmountPK.ToString("#,0.00", nfi);
        public decimal AmountBN { get; set; }
        public string AmountBNStr => AmountBN.ToString("#,0.00", nfi);
        public string Total => (AmountCash + AmountPK + AmountBN).ToString("#,0.00", nfi);
    }

    public class SalesViewModel
    {
        public SalesViewItem AirSale { get; set; }
        public SalesViewItem AirPenalty { get; set; }
        public SalesViewItem AirExchange { get; set; }
        public SalesViewItem AirRefund { get; set; }
        public SalesViewItem AirForcedExchange { get; set; }
        public SalesViewItem AirForcedRefund { get; set; }
        public SalesViewItem AirTotal { get; set; }

        public SalesViewItem RailSale { get; set; }
        public SalesViewItem RailRefund { get; set; }

        public SalesViewItem FinalTotal { get; set; }
    }
}
