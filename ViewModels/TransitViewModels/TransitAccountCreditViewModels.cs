using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AvibaWeb.DomainModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AvibaWeb.ViewModels.TransitViewModels
{
    public class TransitAccountCreditViewModel
    {
        [Required]
        public int SelectedGroupId { get; set; }

        [Display(Name = "Группа")]
        public IEnumerable<SelectListItem> LoanGroups { get; set; }

        public TransitAccountCredit Credit { get; set; }
    }

    public class CreateCreditViewModel
    {
        public string TransitBalance { get; set; }

        public bool IsEditBalance { get; set; }

        public TransitAccountCredit Credit { get; set; }
    }
}
