using System.ComponentModel.DataAnnotations;
using AvibaWeb.DomainModels;
using AvibaWeb.Models;
using Microsoft.AspNetCore.Http;

namespace AvibaWeb.ViewModels.AdminViewModels
{
    public class UserViewModel
    {
        [Required]
        [Display(Name = "Имя пользователя")]
        public string UserName { get; set; }

        [StringLength(100, ErrorMessage = "Минимальная длина пароля составляет {2} символов.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public virtual string Password { get; set; }

        [Display(Name = "ФИО")]
        public string Name { get; set; }

        [Display(Name = "Должность")]
        public string Position { get; set; }

        [Display(Name = "Тел. номер")]
        public string PhoneNumber { get; set; }

        [Display(Name = "ИНН")]
        public string ITN { get; set; }

        //[Display(Name = "Организация")]
        //public string Organization { get; set; }

        [Display(Name = "Фото")]
        public IFormFile Photo { get; set; }
    }

    public class CreateViewModel : UserViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "Минимальная длина пароля составляет {2} символов.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public override string Password { get; set; }
    }

    public class EditViewModel : UserViewModel
    {
        public EditViewModel()
        {
        }

        public EditViewModel(AppUser user)
        {
            this.Id = user.Id;
            this.UserName = user.UserName;
            this.Name = user.Name;
            this.Position = user.Position;
            this.PhoneNumber = user.PhoneNumber;
            this.IsActive = user.IsActive;
            this.ITN = user.UserITN;
        }

        public string Id { get; set; }

        [Display(Name = "Активен")]
        public bool IsActive { get; set; }
    }
}