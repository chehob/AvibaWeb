using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AvibaWeb.DomainModels;

namespace AvibaWeb.ViewModels.ManagementAccountingViewModels
{
    public class CashBlockViewModel
    {
        public CashBlockViewModel()
        {
            nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";
        }
        private NumberFormatInfo nfi;

        public decimal OfficeBalance { get; set; }
        public string OfficeBalanceStr => OfficeBalance.ToString("#,0.00", nfi);

        public decimal DeskBalance { get; set; }
        public string DeskBalanceStr => DeskBalance.ToString("#,0.00", nfi);

        public decimal CollectorsBalance { get; set; }
        public decimal TransitBalance { get; set; }

        public string OfficeCollectorsTotalStr =>
            (OfficeBalance + CollectorsBalance + TransitBalance).ToString("#,0.00", nfi);

        public string TotalBalanceStr =>
            (OfficeBalance + CollectorsBalance + TransitBalance + DeskBalance).ToString("#,0.00", nfi);
    }

    public class OrganizationCashlessInfo
    {
        public OrganizationCashlessInfo()
        {
            nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";
        }
        private NumberFormatInfo nfi;

        public string Name { get; set; }
        public decimal Balance { get; set; }
        public string BalanceStr => Balance.ToString("#,0.00", nfi);
        public string CustomBalanceStr { get; set; }
    }

    public class CashlessBlockViewModel
    {
        public CashlessBlockViewModel()
        {
            nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";
        }
        private NumberFormatInfo nfi;

        public List<OrganizationCashlessInfo> Organizations;
        public string Total => Organizations.Sum(o => o.Balance).ToString("#,0.00", nfi);
    }

    public class ProvidersBlockViewModel
    {
        public ProvidersBlockViewModel()
        {
            nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";
        }
        private NumberFormatInfo nfi;

        public List<OrganizationCashlessInfo> Organizations;
        public string Total => Organizations.Where(o => o.Name != "ПАО \"Авиакомпания \"Сибирь\"").Sum(o => o.Balance).ToString("#,0.00", nfi);
    }

    public class CorporatorBlockViewModel
    {
        public string NegBalance { get; set; }
        public string PosBalance { get; set; }
        public string TotalStr { get; set; }
    }

    public class TransitBlockViewModel
    {
        public TransitBlockViewModel()
        {
            nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";
        }
        private NumberFormatInfo nfi;

        public List<LoanGroup> LoanGroups { get; set; }
        public string Total => LoanGroups.Sum(g => g.Balance).ToString("#,0.00", nfi);
    }

    public class SummaryBlockViewModel
    {
        public SummaryBlockViewModel()
        {
            nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";
        }
        private NumberFormatInfo nfi;

        public decimal OrganizationBalance { get; set; }
        public string OrganizationBalanceStr => OrganizationBalance.ToString("#,0.00", nfi);
        public decimal CorpNegativeBalance { get; set; }
        public string CorpNegativeBalanceStr => CorpNegativeBalance.ToString("#,0.00", nfi);
        public decimal TransitTotal { get; set; }
        public string TransitTotalStr => TransitTotal.ToString("#,0.00", nfi);
        public decimal ProvidersTotal { get; set; }
        public string ProvidersTotalStr => ProvidersTotal.ToString("#,0.00", nfi);
        public decimal SubagentsTotal { get; set; }
        public string SubagentsTotalStr => SubagentsTotal.ToString("#,0.00", nfi);
        public decimal DepositTotal { get; set; }
        public string DepositTotalStr => DepositTotal.ToString("#,0.00", nfi);

        public string CashlessTotal =>
            (OrganizationBalance + CorpNegativeBalance + TransitTotal + DepositTotal)
            .ToString("#,0.00", nfi);

        public decimal CashTotal { get; set; }
        public string CashTotalStr => CashTotal.ToString("#,0.00", nfi);

        public string FinalTotal =>
            (OrganizationBalance + CorpNegativeBalance + TransitTotal + ProvidersTotal + SubagentsTotal + DepositTotal +
             CashTotal).ToString("#,0.00", nfi);
    }
}
