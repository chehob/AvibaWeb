using Microsoft.AspNetCore.Http;

namespace AvibaWeb.ViewModels.AccountViewModels
{
    public class ProfileViewModel
    {
        public string PhoneNumber { get; set; }
        public IFormFile Photo { get; set; }
    }
}
