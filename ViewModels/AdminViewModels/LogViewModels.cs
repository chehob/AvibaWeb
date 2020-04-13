using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.ViewModels.AdminViewModels
{
    public class LogViewModel
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public List<LogData> Records { get; set; }
    }

    public class LogData
    {
        public enum LogRecordCategory
        {
            Transit = 0
        }

        public string OperationDateTime { get; set; }
        public string Description { get; set; }
        public LogRecordCategory Category { get; set; }
        public string CategoryStr { get; set; }
        public string OldBalance { get; set; }
        public string Delta { get; set; }
        public string NewBalance { get; set; }
        public string ModifiedBy { get; set; }
    }
}
