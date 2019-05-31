using System.ComponentModel.DataAnnotations;

namespace AvibaWeb.DomainModels
{
    public class Card
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CardId { get; set; }

        [Display(Name = "Номер")]
        public string Number { get; set; }

        [Display(Name = "Владелец")]
        public string UserId { get; set; }
        public virtual AppUser User { get; set; }
    }
}