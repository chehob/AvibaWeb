using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvibaWeb.DomainModels
{
    public class TransitAccount
    {
        [Key]
        public int TransitAccountId { get; set; }

        [Column(TypeName = "Money")]
        public decimal Balance { get; set; }

        [Display(Name = "Активен")]
        [DefaultValue(true)]
        public bool IsActive { get; set; } = true;
    }
}
