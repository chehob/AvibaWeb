using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AvibaWeb.DomainModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AvibaWeb.ViewModels.AdminViewModels
{
    public class CounterpartyViewModel
    {
        public Counterparty Item { get; set; }
    }

    public class CounterpartyEditModel : CounterpartyViewModel
    {
        public CounterpartyEditModel()
        {
        }

        public CounterpartyEditModel(Counterparty counterparty)
        {
            this.Item = counterparty;
        }

        [Display(Name = "Тип")]
        public IEnumerable<SelectListItem> Types { get; set; }

        [Display(Name = "Подразделение")]
        public SelectList DeskGroups { get; set; }

        [Display(Name = "Статья расходов")]
        public SelectList ExpenditureObjects { get; set; }
    }
}
