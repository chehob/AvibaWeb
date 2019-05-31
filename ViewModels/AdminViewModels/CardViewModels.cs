using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using AvibaWeb.DomainModels;

namespace AvibaWeb.ViewModels.AdminViewModels
{
    public class CreateCardViewModel
    {
        [Required]
        [Display(Name = "Номер карты")]
        public string Number { get; set; }

        public SelectList Users { get; set; }
        public string UserId { get; set; }
    }

    public class EditCardViewModel
    {
        public EditCardViewModel()
        {
        }

        public EditCardViewModel(Card card, SelectList users)
        {
            this.Id = card.CardId;
            this.Number = card.Number;
            this.UserId = card.UserId;
            this.Users = users;
        }

        [Required]
        [Display(Name = "Номер карты")]
        public string Number { get; set; }

        public SelectList Users { get; set; }
        public string UserId { get; set; }

        public int Id { get; set; }
    }
}