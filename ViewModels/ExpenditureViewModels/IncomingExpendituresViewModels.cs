using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.ViewModels.ExpenditureViewModels
{
    public class IncomingExpenditureItem
    {
        public string Amount { get; set; }
        public string CounterpartyName { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public int OperationId { get; set; }
        public bool IsProcessed { get; set; }
    }

    public class IncomingExpendituresViewModel
    {
        //public CRMPType Type { get; set; }
        public List<IncomingExpenditureItem> Items { get; set; }
    }
}
