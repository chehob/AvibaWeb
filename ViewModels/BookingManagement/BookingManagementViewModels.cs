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

    public class KRSViewItem
    {
        public KRSViewItem()
        {
            nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";
        }
        private NumberFormatInfo nfi;

        public decimal SegCount { get; set; }
        public string SegCountStr => SegCount.ToString();
        public decimal KRSCount { get; set; }
        public string KRSCountStr => KRSCount.ToString();
        public decimal Amount { get; set; }
        public string AmountStr => Amount.ToString("#,0.00", nfi);
        public decimal AmountComm => Amount * new decimal(0.974);
        public string AmountCommStr => AmountComm.ToString("#,0.00", nfi);
    }

    public class KRSViewModel
    {
        public KRSViewItem CashSale { get; set; }
        public KRSViewItem CashExchange { get; set; }
        public KRSViewItem CashRefund { get; set; }
        public KRSViewItem CashForcedRefund { get; set; }
        public KRSViewItem CashService { get; set; }
        public KRSViewItem CashCancel { get; set; }
        public KRSViewItem CashTotal { get; set; }

        public KRSViewItem PKSale { get; set; }
        public KRSViewItem PKExchange { get; set; }
        public KRSViewItem PKRefund { get; set; }
        public KRSViewItem PKCancel { get; set; }
        public KRSViewItem PKTotal { get; set; }

        public KRSViewItem BNSale { get; set; }
        public KRSViewItem BNExchange { get; set; }
        public KRSViewItem BNRefund { get; set; }
        public KRSViewItem BNForcedRefund { get; set; }
        public KRSViewItem BNTotal { get; set; }

        public string FinalTotalStr { get; set; }
        public string FinalTotalCommStr { get; set; }
    }

    public class LuggageViewItem
    {
        public LuggageViewItem()
        {
            nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";
        }
        private NumberFormatInfo nfi;

        public string Airline { get; set; }
        public decimal DocCount { get; set; }
        public string DocCountStr => DocCount.ToString("#,0", nfi);
        public decimal Weight { get; set; }
        public string WeightStr => Weight.ToString("#,0", nfi);
        public decimal Amount { get; set; }
        public string AmountStr => Amount.ToString("#,0.00", nfi);
    }

    public class LuggageViewModel
    {
        public List<LuggageViewItem> Items { get; set; }
    }
}
