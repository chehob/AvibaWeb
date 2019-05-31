using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvibaWeb.DomainModels
{
    public class Cashier
    {
        public Cashier()
        {

        }

        public Cashier(string Id)
        {
            UserId = Id;
            Balance = 0;
            IsActive = true;
        }

        [Required]
        [Key, ForeignKey("User")]
        public string UserId { get; set; }

        [Column(TypeName = "Money")]
        public decimal Balance { get; set; }

        public bool IsActive { get; set; }

        public AppUser User { get; set; }
    }
}