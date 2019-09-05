using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.ViewModels.BookingManagement
{
    public class FilterViewModel
    {
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
        public List<SelectResultItem> children { get; set; }
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
