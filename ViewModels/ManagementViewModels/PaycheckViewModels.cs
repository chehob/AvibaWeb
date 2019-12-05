using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.ViewModels.ManagementViewModels
{
    public class PaycheckOperationsCheckInInfo
    {
        public string CheckInDateTime { get; set; }
        public string DeskId { get; set; }
    }

    public class PaycheckOperationsViewItem
    {
        public string Name { get; set; }
        public string Amount { get; set; }
        public List<PaycheckOperationsCheckInInfo> CheckIns { get; set; }
    }

    public class PaycheckOperationsViewModel
    {
        public List<PaycheckOperationsViewItem> Items { get; set; }
    }
}
