using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.ViewModels.AdminViewModels
{
    public class AtolSettingsViewModel
    {
        public List<AtolSettingsItemData> SettingsItems { get; set; }
    }

    public class AtolSettingsItemData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TicketPercent { get; set; }
        public int LuggagePercent { get; set; }
    }
}
