using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace AvibaWeb.ViewModels.ReportViewModels
{
    public class OnlineInfoModel
    {
        public class BalanceInfoElement
        {
            public decimal Balance { get; set; }

            public string Name { get; set; }
            public string BalanceStr => Balance.ToString("### ### ##0.00 руб", CultureInfo.InvariantCulture);
        }

        public string AvibaTotalInfo => AvibaBalanceInfo.Sum(x => x.Balance).ToString("### ### ##0.00 руб", CultureInfo.InvariantCulture);
        public List<BalanceInfoElement> AvibaBalanceInfo { get; set; }

        public string AviaTourTotalInfo => AviaTourBalanceInfo.Sum(x => x.Balance).ToString("### ### ##0.00 руб", CultureInfo.InvariantCulture);
        public List<BalanceInfoElement> AviaTourBalanceInfo { get; set; }

        public string CollectorsTotalInfo => CollectorsBalanceInfo.Sum(x => x.Balance).ToString("### ### ##0.00 руб", CultureInfo.InvariantCulture);
        public List<BalanceInfoElement> CollectorsBalanceInfo { get; set; }

        public string TotalBalanceStr =>
            (AvibaBalanceInfo.Sum(x => x.Balance) + AviaTourBalanceInfo.Sum(x => x.Balance) +
             CollectorsBalanceInfo.Sum(x => x.Balance) + OfficeBalance + TransitBalance)
            .ToString("### ### ##0.00 руб", CultureInfo.InvariantCulture);

        public string DeskBalanceStr =>
            (AvibaBalanceInfo.Sum(x => x.Balance) + AviaTourBalanceInfo.Sum(x => x.Balance))
            .ToString("### ### ##0.00 руб", CultureInfo.InvariantCulture);

        public string OfficeBalanceStr => OfficeBalance.ToString("### ### ##0.00 руб", CultureInfo.InvariantCulture);
        public decimal OfficeBalance { get; set; }

        public decimal TransitBalance { get; set; }
        public string OfficeCollectorsTotalInfo =>
            (CollectorsBalanceInfo.Sum(x => x.Balance) + OfficeBalance + TransitBalance).ToString("### ### ##0.00 руб",
                CultureInfo.InvariantCulture);

        public OfficeBillInfoViewModel OfficeBillInfo { get; set; }
    }

    public class OfficeBillEditViewModel
    {
        public string _5kBillSum { get; set; }
        public string _2kBillSum { get; set; }
        public decimal OfficeBalance { get; set; }
    }

    public class OfficeBillInfoViewModel
    {
        public string _5kBillSum { get; set; }
        public string _2kBillSum { get; set; }
        public string RemainderSum { get; set; }
    }
}