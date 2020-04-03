using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AvibaWeb.DomainModels;

namespace AvibaWeb.ViewModels.TransitViewModels
{
    public class CreateDebitViewModel
    {
        public string TransitBalance { get; set; }

        public bool IsEditBalance { get; set; }

        public TransitAccountDebit Debit { get; set; }
    }
}
