using AvibaWeb.DomainModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AvibaWeb.ViewModels.RevenueViewModels
{
    public class OfficeDebitListViewModel
    {
        public int DebitId { get; set; }

        [Display(Name = "Наименование")]
        public string Description { get; set; }

        [Display(Name = "Сумма")]
        public decimal Amount { get; set; }

        [Display(Name = "Дата создания")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy H:mm}")]
        public DateTime IssuedDateTime { get; set; }

        [Display(Name = "Статус")]
        public OfficeDebitOperation.ODOType Status { get; set; }
    }

    public class AddProviderAgentFeeViewModel
    {
        public string ProviderId { get; set; }
    }

    public class AddProviderAgentFeePostModel
    {
        public string ProviderId { get; set; }
        public string FeeAmount { get; set; }
        public string Comment { get; set; }
    }

    public class ProviderAgentFeeViewModel
    {
        public List<ProviderData> Providers { get; set; }
    }

    public class ProviderData
    {
        public string ProviderId { get; set; }
        public string Name { get; set; }
        public string FeeAmount { get; set; }
    }

    public class ProviderAgentFeeTransactionsViewModel
    {
        public List<ProviderAgentFeeTransactionData> Records { get; set; }
    }

    public class ProviderAgentFeeTransactionData
    {
        public string TransactionDateTime { get; set; }
        public string Amount { get; set; }
        public string Comment { get; set; }
    }
}
