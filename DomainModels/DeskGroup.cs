using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AvibaWeb.DomainModels
{
    public class DeskGroup
    {
        [Key]
        public int DeskGroupId { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Desk> Desks { get; set; }

        [Display(Name = "Активно")]
        [DefaultValue(true)]
        public bool IsActive { get; set; } = true;
    }
}