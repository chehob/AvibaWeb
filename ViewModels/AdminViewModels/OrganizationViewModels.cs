using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AvibaWeb.DomainModels;
using AvibaWeb.ViewModels.CorpReceiptViewModels;

namespace AvibaWeb.ViewModels.AdminViewModels
{
    public class FinancialAccountViewModel
    {
        public int OrganizationId { get; set; }

        public FinancialAccount Account { get; set; }
    }

    public class DocumentListViewModel
    {
        public int CorporatorDocumentId { get; set; }

        public string Organization { get; set; }

        [Display(Name = "Договор")] public string Document { get; set; }

        [Display(Name = "Дата создания")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy H:mm}")]
        public DateTime Date { get; set; }
    }

    public class CorporatorDocumentsViewModel
    {
        public List<string> Counterparties { get; set; }
        public List<string> Organizations { get; set; }
    }

    public class CreateDocumentViewModel
    {
        public List<string> Counterparties { get; set; }
        public List<string> Organizations { get; set; }
        public DocumentEditData Document { get; set; }
    }

    public class DocumentEditData
    {
        public int DocumentId { get; set; }
        public string CorporatorName { get; set; }
        public string OrganizationName { get; set; }
        public string Doc { get; set; }
        public string Date { get; set; }
    }

    public class CreateDocumentPostViewModel
    {
        public int? DocumentId { get; set; }
        public string OrgName { get; set; }
        public string CorpName { get; set; }
        public string Document { get; set; }
        public string IssuedDateTime { get; set; }
    }

    public class EditDocumentTaxesViewModel
    {
        public int OrganizationId { get; set; }
        public string CorporatorId { get; set; }
        public List<CorpFeeListViewModel> FeeRates { get; set; }
    }
}
