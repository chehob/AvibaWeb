using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class BMDeskGroup
    {
        [Key]
        public int BMDeskGroupId { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Desk> Desks { get; set; }

        [Display(Name = "Активно")]
        [DefaultValue(true)]
        public bool IsActive { get; set; } = true;
    }
}