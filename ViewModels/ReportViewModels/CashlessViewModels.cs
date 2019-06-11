using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.ViewModels.ReportViewModels
{
    public class CounterpartyAccountBalance
    {
        public string CounterpartyId { get; set; }
        public string CounterpartyName { get; set; }
        public string Account { get; set; }
        public decimal Balance { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal BalanceEnd => Balance + Debit - Credit;
    }

    public class CashlessViewModel
    {
        public string Name { get; set; }
        public List<CounterpartyAccountBalance> AccountBalances { get; set; }
    }

    public class CashlessCorpItem
    {
        public CashlessCorpItem()
        {
            nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";
        }
        private NumberFormatInfo nfi;

        public string CorpName { get; set; }
        public decimal Balance { get; set; }
        public string BalanceStr => Balance.ToString("#,0.00", nfi);
        public int LastPaymentDays { get; set; }
        public int LastReceiptDays { get; set; }
    }

    public class CashlessCorpViewModel
    {
        public CashlessCorpViewModel()
        {
            nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";
        }
        private NumberFormatInfo nfi;

        public List<CashlessCorpItem> Items { get; set; }
        public string NegBalance => Items.Where(i => i.Balance < 0).Sum(i => i.Balance).ToString("#,0.00", nfi);
        public string PosBalance => Items.Where(i => i.Balance >= 0).Sum(i => i.Balance).ToString("#,0.00", nfi);
        public string TotalStr => Items.Sum(i => i.Balance).ToString("#,0.00", nfi);
    }

    public class OrganizationAccountBalance
    {
        public OrganizationAccountBalance()
        {
            nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";
        }
        private NumberFormatInfo nfi;

        public int Id { get; set; }
        public string LatestUpload { get; set; }
        public string Account { get; set; }
        public string BankName { get; set; }
        public string Balance { get; set; }
    }

    public class OrganizationCashlessViewModel
    {
        public OrganizationCashlessViewModel()
        {
            nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";
        }
        private NumberFormatInfo nfi;

        public List<OrganizationCashlessInfo> Organizations;
        public string Balance => Organizations.Sum(o => decimal.Parse(o.Balance.Replace(".", ",").Replace(" ", string.Empty))).ToString("#,0.00", nfi);
        public string LoanGroupsBalance { get; set; }
        public string Total => (decimal.Parse(Balance.Replace(".", ",").Replace(" ", string.Empty)) +
            decimal.Parse(LoanGroupsBalance.Replace(".", ",").Replace(" ", string.Empty))).ToString("#,0.00", nfi);
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
        public List<OrganizationAccountBalance> AccountBalances { get; set; }
        public string Balance => AccountBalances.Sum(ab => decimal.Parse(ab.Balance.Replace(".", ",").Replace(" ", string.Empty))).ToString("#,0.00", nfi);
    }

    public class AccountOperationsViewModel
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int AccountId { get; set; }
        public bool IsAllOperations { get; set; }
        public string OrgName { get; set; }
        public string BankName { get; set; }
        public List<AccountOperationData> Operations { get; set; }
    }

    public class AccountOperationData
    {
        public string OperationDateTime { get; set; }
        public string OrderNumber { get; set; }
        public string CounterpartyName { get; set; }
        public decimal Amount { get; set; }
        public string AmountStr { get; set; }
        public string Description { get; set; }
        public string PayeeName { get; set; }
    }
}
