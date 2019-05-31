using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AvibaWeb.DomainModels;
using X.PagedList;

namespace AvibaWeb.ViewModels.TransitViewModels
{
    public class TransitAccountViewModel
    {
        public string Balance { get; set; }

        public List<LoanGroup> LoanGroups { get; set; }
    }

    public class TransitAccountIssuedCredit
    {
        public int CreditId { get; set; }

        public int LoanGroupId { get; set; }

        public decimal Amount { get; set; }
        [Display(Name = "Сумма")]
        public string AmountStr { get; set; }

        public decimal AddAmount { get; set; }
        [Display(Name = "Добавочная сумма")]
        public string AddAmountStr { get; set; }

        [Display(Name = "Дата создания")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy H:mm}")]
        public DateTime IssuedDateTime { get; set; }

        [Display(Name = "Статус")]
        public TransitAccountCreditOperation.TACOType Status { get; set; }

        [Display(Name = "Комментарий")]
        public string Comment { get; set; }

        [Display(Name = "Группа")]
        public string LoanGroup { get; set; }
    }

    public class TransitAccountIssuedCreditsViewModel
    {
        public TransitAccountIssuedCreditsViewModel()
        {
            nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";
        }
        private NumberFormatInfo nfi;

        public List<TransitAccountIssuedCredit> Credits { get; set; }

        public string FinancialOperationsTotal { get; set; }
        public string CreditsTotal { get; set; }
        public string AddTotal { get; set; }
    }

    public class OfficeDebitViewModel
    {
        public decimal Balance { get; set; }
        public TransitAccountDebit Debit { get; set; }
    }
}
