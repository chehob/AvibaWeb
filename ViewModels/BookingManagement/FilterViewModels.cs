using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.ViewModels.BookingManagement
{
    public class FilterViewModel
    {
        public List<string> Cities { get; set; }
        public List<KeyValuePair<string,string>> Airlines { get; set; }
    }

    public class DeskFilterItem
    {
        public string id { get; set; }
        public string text { get; set; }
        public bool @checked { get; set; }
        public List<DeskFilterItem> children { get; set; }
    }
}
