using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.ViewModels.AdminViewModels
{
    public class PKReceiptRuleViewModel
    {
        public List<decimal> Rules { get; set; }
    }

    public class AddPKReceiptRuleViewModel
    {
        public decimal Rate { get; set; }
    }
}
