using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using AvibaWeb.DomainModels;
using AvibaWeb.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AvibaWeb.ViewModels.CollectionViewModels
{
    public class CollectionListItem
    {
        public int CollectionId { get; set; }
        public int CollectionOperationId { get; set; }
        public string Amount { get; set; }
        public string ProviderName { get; set; }
        public string DeskId { get; set; }
        public string DeskName { get; set; }
        public string CollectorName { get; set; }
        public CollectionOperationType.COType Status { get; set; }
        public string IssuedDateTime { get; set; }
        public string AcceptedDateTime { get; set; }
        public PaymentTypes PaymentType { get; set; }
        public string Comment { get; set; }
    }

    public class CollectionDatatableViewModel
    {
        public bool IsAdmin { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public List<CollectionListItem> Items { get; set; }
    }

    public class CollectionListViewModel
    {
        public int CollectionId { get; set; }

        public int CollectionOperationId { get; set; }

        [Display(Name = "Сумма")] public decimal Amount { get; set; }

        [Display(Name = "Поставщик")] public string ProviderName { get; set; }

        [Display(Name = "Пульт")] public string DeskId { get; set; }

        [Display(Name = "Касса")] public string DeskName { get; set; }

        [Display(Name = "Получатель")] public string CollectorName { get; set; }

        [Display(Name = "Статус")] public CollectionOperationType.COType Status { get; set; }

        [Display(Name = "Дата создания")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy H:mm}")]
        public DateTime IssuedDateTime { get; set; }

        [Display(Name = "Дата получения")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy H:mm}")]
        public DateTime? AcceptedDateTime { get; set; }

        [Display(Name = "Тип оплаты")] public PaymentTypes PaymentType { get; set; }

        [Display(Name = "Комментарий")] public string Comment { get; set; }
    }

    public class CreateCollectionModel
    {
        [Required] [Display(Name = "Сумма")] public decimal Amount { get; set; }

        [Required] public string SelectedCollector { get; set; }

        [Display(Name = "Получатель")] public IEnumerable<SelectListItem> Collectors { get; set; }

        [Display(Name = "Тип оплаты")] public PaymentTypes PaymentType { get; set; }

        public string ProviderId { get; set; }

        public string Comment { get; set; }

        public string RedirectAction { get; set; }
    }

    public class BalanceModel
    {
        public BalanceModel()
        {
            Balance = 0;
            IncomingNotAccepted = 0;
            IssuedNotAccepted = 0;
        }

        public string BalanceStr => Balance.ToString("### ### ##0.00");
        public decimal Balance { get; set; }

        public string IncomingNotAcceptedStr => IncomingNotAccepted.ToString("### ### ##0.00");
        public decimal IncomingNotAccepted { get; set; }

        public string IssuedNotAcceptedStr => IssuedNotAccepted.ToString("### ### ##0.00");
        public decimal IssuedNotAccepted { get; set; }

        public string TotalStr => (Balance + IncomingNotAccepted - IssuedNotAccepted).ToString("### ### ##0.00");
    }

    public class CollectionInfoModel
    {
        public int IncomingCollections { get; set; }
    }

    public class OfficeBalanceViewModel
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public OfficeBalanceRecord CurrentData { get; set; }
        public List<OfficeBalanceRecord> Records { get; set; }
    }

    public class OfficeBalanceRecord
    {
        public OfficeBalanceRecord()
        {
            nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";
        }
        private NumberFormatInfo nfi;

        public string SaveDateTime { get; set; }
        public decimal Total { get; set; }
        public string TotalStr => Total.ToString("#,0.00", nfi);
        public decimal _5kBill { get; set; }
        public string _5kBillStr => _5kBill.ToString("#,0.00", nfi);
        public decimal _2kBill { get; set; }
        public string _2kBillStr => _2kBill.ToString("#,0.00", nfi);

        public string BalanceStr => (Total - _5kBill - _2kBill).ToString("#,0.00", nfi);        
    }

    public class OfficeHistoryRequest
    {
        public string fromDate { get; set; }
        public string toDate { get; set; }
    }
}