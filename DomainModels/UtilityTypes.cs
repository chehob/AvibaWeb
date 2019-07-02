using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public enum PaymentTypes
    {
        [Display(Name = "Наличные")]
        Cash,
        [Display(Name = "Безналичный расчет")]
        Cashless
    }
}
