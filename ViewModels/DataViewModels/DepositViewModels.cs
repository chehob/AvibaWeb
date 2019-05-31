using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.ViewModels.DataViewModels
{
    public class DepositItem
    {
        public enum DepositItemType
        {
            Provider,
            Subagent
        }

        public string ITN { get; set; }
        public string Name { get; set; }
        public decimal Deposit { get; set; }
        public DepositItemType Type { get; set; }
    }

    public class DepositViewModel
    {
        public List<DepositItem> Deposits { get; set; }
    }
}
