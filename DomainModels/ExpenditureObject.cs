using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AvibaWeb.DomainModels
{
    public class ExpenditureObject
    {
        [Key]
        public int ExpenditureObjectId { get; set; }

        [Display(Name = "Наименование")]
        public string Description { get; set; }

        [Display(Name = "Активен")]
        [DefaultValue(true)]
        public bool IsActive { get; set; } = true;
    }
}