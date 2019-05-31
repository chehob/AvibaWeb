using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class CounterpartyType
    {
        [Key]
        public int CounterpartyTypeId { get; set; }

        [Display(Name = "Тип контрагента")]
        public string Description { get; set; }

        [Display(Name = "Активен")]
        [DefaultValue(true)]
        public bool IsActive { get; set; } = true;
    }
}
