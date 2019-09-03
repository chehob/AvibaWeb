using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.ViewModels.BookingManagement
{
    public class FilterViewModel
    {
        public List<KeyValuePair<string,string>> Airlines { get; set; }
    }

    public class DeskFilterItem
    {
        public string id { get; set; }
        public string text { get; set; }
        public bool @checked { get; set; }
        public List<DeskFilterItem> children { get; set; }
    }

    public class SelectResultItem
    {
        public string id { get; set; }
        public string text { get; set; }
    }

    public class SelectResult
    {
        public SelectResult()
        {
            results = new List<SelectResultItem>();
        }

        public List<SelectResultItem> results { get; set; }
    }
}
