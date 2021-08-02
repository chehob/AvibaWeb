using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class AtolPrintSettings
    {
        [Key]
        public int AtolPrintSettingsId { get; set; }

        public string AtolServerName { get; set; }

        public int PrintPercentage { get; set; }

        public int PrintLuggagePercentage { get; set; }

        public string AtolServerAddress { get; set; }

        public string CashierName { get; set; }

        public bool IsPermanent { get; set; }

        public string DeskBinding { get; set; }
    }
}
