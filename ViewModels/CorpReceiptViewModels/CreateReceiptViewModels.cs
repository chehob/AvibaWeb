using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AvibaWeb.DomainModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using static AvibaWeb.DomainModels.CorporatorReceipt;
using static AvibaWeb.DomainModels.CorporatorReceiptItem;

namespace AvibaWeb.ViewModels.CorpReceiptViewModels
{
    public class CreateReceiptViewModel
    {
        public List<KeyValuePair<string,string>> Counterparties { get; set; }
        public List<KeyValuePair<string,string>> Organizations { get; set; }
        public ReceiptEditData Receipt { get; set; }
        public int SubGroupId { get; set; }
    }

    public class ReceiptEditData
    {
        public int ReceiptId { get; set; }
        public string CorporatorName { get; set; }
        public string OrganizationName { get; set; }
        public string BankName { get; set; }
        public decimal FeeRate { get; set; }
        public List<TicketListViewModel> Items { get; set; }
        public string CreatedDateTime { get; set; }
        public string IssuedDateTime { get; set; }
        public string PaidDateTime { get; set; }
        public string ReceiptNumber { get; set; }
        public CRPaymentStatus StatusId { get; set; }
    }

    public class ReceiptItem
    {
        public string TicketOperationId { get; set; }
        public decimal Amount { get; set; }
        public decimal FeeRate { get; set; }
        public string PassengerName { get; set; }
        public string Route { get; set; }
        public CRIType TypeId { get; set; }
        public bool PerSegment { get; set; }
        public bool IsPercent { get; set; }
        public int SegCount { get; set; }
    }

    public class CreateReceiptPostViewModel
    {
        public int? ReceiptId { get; set; }
        public string PayerId { get; set; }
        public string PayerName { get; set; }
        public string PayeeId { get; set; }
        public string PayeeName { get; set; }
        public string PayeeBankName { get; set; }
        public string FeeRate { get; set; }
        public List<ReceiptItem> Items { get; set; }
        public CRPaymentStatus StatusId { get; set; }
        public string IssuedDateTime { get; set; }
        public string PaidDateTime { get; set; }
        public decimal ReceiptTotal { get; set; }  
        public int SubGroupId { get; set; }
    }

    public class TicketListViewModel
    {
        public int TicketOperationId { get; set; }

        [Display(Name = "Номер билета")] public string TicketNumber { get; set; }

        [Display(Name = "Маршрут")] public string Route { get; set; }

        [Display(Name = "Пассажир")] public string PassengerName { get; set; }

        [Display(Name = "Дата создания")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy H:mm}")]
        public DateTime ExecutionDateTime { get; set; }

        [Display(Name = "Сумма")] public string Payment { get; set; }

        public string FeeRate { get; set; }

        public int SegCount { get; set; }

        [Display(Name = "Тип операции")] public VKRSCancelRequest.TOType OperationType { get; set; }

        public int OperationTypeId { get; set; }
        public int TicketTypeId { get; set; }
        public bool PerSegment { get; set; }
        public bool IsPercent { get; set; }

        public CRIType TypeId { get; set; }
    }

    public class TicketListRequest
    {
        public string payerId { get; set; }
        public string payeeId { get; set; }
        public string isOnlyPaid { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public List<ReceiptItem> ExceptItems { get; set; }
        public bool isMobile { get; set; }
    }

    public class CorpFeeListViewModel
    {
        public enum CFTicketType
        {
            [Display(Name = "Авиа")]
            Avia = 1,
            [Display(Name = "Ж/д")]
            Rail = 3,
            [Display(Name = "EMD")]
            EMD = 4,
        }

        public enum CFOpType
        {
            [Display(Name = "Продажа")]
            Sale = 1,
            [Display(Name = "Возврат")]
            Refund = 2,
            [Display(Name = "Обмен")]
            Exchange = 3,
        }

        public CFTicketType TicketType { get; set; }
        public CFOpType OperationType { get; set; }
        public int TicketTypeId { get; set; }
        public int OperationTypeId { get; set; }
        public decimal Rate { get; set; }
        public bool PerSegment { get; set; }
        public bool IsPercent { get; set; }
    }
}
