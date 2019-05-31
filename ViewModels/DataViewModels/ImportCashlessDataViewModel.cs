using System.Collections.Generic;
using AvibaWeb.DomainModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AvibaWeb.ViewModels.DataViewModels
{
    public class CashlessImportCounterpartyGroup
    {
        public List<CashlessRecord> Records { get; set; }

        public string ITN { get; set; }
        public string Name { get; set; }
        public bool IsUserITN { get; set; }
        public bool IsTransferAccount { get; set; }
        public bool IsKnownCounterparty { get; set; }        
        public Counterparty MissingCounterparty { get; set; }
    }

    public class CashlessImportViewData
    {
        // 
        public List<CashlessImportCounterpartyGroup> CounterpartyGroups { get; set; }

        public CashlessDestinationRecord Destination { get; set; }
    }

    public class CashlessImportViewModel
    {
        // Import data grouped by financial account
        public IEnumerable<CashlessImportViewData> FinancialAccountImportData { get; set; }

        public IEnumerable<SelectListItem> CounterpartyTypes { get; set; }
    }
}
