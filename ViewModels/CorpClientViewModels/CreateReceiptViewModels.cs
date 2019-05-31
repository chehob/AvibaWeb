using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AvibaWeb.DomainModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AvibaWeb.ViewModels.CorpClientViewModels
{
    public class IndexViewModel
    {
        public bool IsUserLoggedInKRS { get; set; }
    }
}
