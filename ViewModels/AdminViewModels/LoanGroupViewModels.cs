using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AvibaWeb.DomainModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AvibaWeb.ViewModels.AdminViewModels
{
    public class LoanGroupViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Наименование")]
        public string Name { get; set; }

        public List<string> Counterparties { get; set; }

        public List<string> GroupCounterparties { get; set; }

        [Display(Name = "Активно")]
        public bool IsActive { get; set; }
    }

    public class CounterpartyOperation
    {
        public string OperationDate { get; set; }
        public string PayeeName { get; set; }
        public string Amount { get; set; }
        public bool IsProcessed { get; set; }
    }

    public class ShowCounterpartyOperationsViewModel
    {
        public int LoanGroupId { get; set; }
        public string CounterpartyId { get; set; }

        public List<CounterpartyOperation> CounterpartyOperations { get; set; }
    }
}
