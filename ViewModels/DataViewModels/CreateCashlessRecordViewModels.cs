using AvibaWeb.DomainModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.ViewModels.DataViewModels
{
    public class CounterpartyInfo
    {
        public string ITN { get; set; }
        public string Name { get; set; }
    }

    public class CreateCashlessRecordViewModel
    {
        public List<string> Counterparties { get; set; }
        public List<string> Organizations { get; set; }

        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string OrderNumber { get; set; }
    }

    public class CreateCashlessRecordCreateViewModel
    {
        public string PayerName { get; set; }
        public string PayerBankName { get; set; }
        public string PayeeName { get; set; }
        public string PayeeBankName { get; set; }
        public string Amount { get; set; }
        public string Description { get; set; }
        public string OrderNumber { get; set; }
    }

    public class OrganizationFinancialAccountsViewModel
    {
        [Display(Name = "Счет")]
        public List<string> Accounts { get; set; }
    }

    public class CreateCounterpartyViewModel
    {
        public IEnumerable<SelectListItem> CounterpartyTypes { get; set; }

        public Counterparty Counterparty { get; set; }
    }
}
