using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AvibaWeb.DomainModels
{
    public class ExpenditureType
    {
        [Key]
        public int ExpenditureTypeId { get; set; }

        [Display(Name = "Наименование")]
        public string Description { get; set; }

        [Display(Name = "Активен")]
        [DefaultValue(true)]
        public bool IsActive { get; set; } = true;
    }
}