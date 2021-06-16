using AvibaWeb.ViewModels.AdminViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AvibaWeb.DomainModels;
using AvibaWeb.ViewModels.CorpReceiptViewModels;

namespace AvibaWeb.ViewModels.CorpClientViewModels
{
    public class CorporatorDocumentsViewModel
    {
        public string ITN { get; set; }
        public List<DocumentListViewModel> Documents { get; set; }
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

    public class CreateDocumentViewModel
    {
        public string ITN { get; set; }
        public List<string> Counterparties { get; set; }
        public List<string> Organizations { get; set; }
        public DocumentEditData Document { get; set; }

        public List<CorporatorDocumentFeeItemData> FeeItems { get; set; }
    }

    public class CreateDocumentPostViewModel
    {
        public int? DocumentId { get; set; }
        public string OrgName { get; set; }
        public string ITN { get; set; }
        public string Document { get; set; }
        public string IssuedDateTime { get; set; }

        public List<CorporatorDocumentFeeItemData> FeeItems { get; set; }
    }

    public class EditDocumentTaxesViewModel
    {
        public int OrganizationId { get; set; }
        public string CorporatorId { get; set; }
        public List<CorpFeeListViewModel> FeeRates { get; set; }
    }

    public class CorporatorAccountsViewModel
    {
        public string ITN { get; set; }
        public List<CorporatorAccount> Accounts { get; set; }
    }

    public class CreateAccountViewModel
    {
        public string ITN { get; set; }
        public CorporatorAccount Account { get; set; }
    }

    public class ReviseReportRequest
    {
        public string fromDate { get; set; }
        public string toDate { get; set; }       
        public string payerId { get; set; }
        public string payerName { get; set; }
        public string payeeId { get; set; }
        public string payeeName { get; set; }
        public string payeeBankName { get; set; }
        public bool isSigned { get; set; }
    }

    public class CorporatorsReportViewModel
    {
        public List<KeyValuePair<string,ReceiptTotals>> Data { get; set; }
    }

    public class DateRangeRequest
    {
        public string fromDate { get; set; }
        public string toDate { get; set; }
    }

    public class CorporatorDocumentPDFViewModel
    {
        public string DocNum { get; set; }
        public string DocDate { get; set; }
        public string CorporatorName { get; set; }
        public string ManagementPosition { get; set; }
        public string ManagementName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string ITN { get; set; }
        public string KPP { get; set; }
        public string OGRN { get; set; }
        public string CorporatorAccountDescription { get; set; }
        public string CorporatorAccountAddress { get; set; }
        public string CorrespondentAccount { get; set; }
        public string BIK { get; set; }
        public string Phone { get; set; }
        public string ManagementPositionGenitive { get; set; }
        public string ManagementNameGenitive { get; set; }

        public List<CorporatorDocumentFeeItemData> FeeItems { get; set; }
    }

    public class CorporatorDocumentFeeItemData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FeeStr { get; set; }
    }
}
