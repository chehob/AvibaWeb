using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AvibaWeb.ViewModels.AdminViewModels
{
    public class SubagentDesksViewModel
    {
        public string Id { get; set; }

        [Display(Name = "Наименование")]
        public string Name { get; set; }

        public List<string> DeskIds { get; set; }
        [Display(Name = "Пульты")]
        public MultiSelectList Desks { get; set; }

        [Display(Name = "Активно")]
        public bool IsActive { get; set; }
    }
}
