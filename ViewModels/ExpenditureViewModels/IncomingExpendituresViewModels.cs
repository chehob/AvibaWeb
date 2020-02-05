using AvibaWeb.DomainModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.ViewModels.ExpenditureViewModels
{
    public class IncomingExpenditureItem
    {
        public string Amount { get; set; }
        public string CounterpartyName { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public int OperationId { get; set; }
        public bool IsProcessed { get; set; }
    }

    public class IncomingExpendituresViewModel
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        //public CRMPType Type { get; set; }
        public List<IncomingExpenditureItem> Items { get; set; }
    }

    public class ProcessIncomingExpenditureViewModel
    {
        public IncomingExpenditure Expenditure { get; set; }

        [Display(Name = "Статья расходов")]
        public SelectList ExpenditureObjects { get; set; }

        public List<KeyValuePair<int,string>> DeskGroups { get; set; }
    }

    public class IncomingExpenditurePostItem
    {
        public int GroupId { get; set; }
        public decimal Amount { get; set; }
    }

    public class ProcessIncomingExpenditurePostModel
    {
        public int ExpenditureId { get; set; }
        public int ExpenditureObjectId { get; set; }
        public List<IncomingExpenditurePostItem> Items { get; set; }
    }
}
