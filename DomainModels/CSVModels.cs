using System;
using System.Collections.Generic;
using CsvHelper.Configuration;

namespace AvibaWeb.DomainModels
{
    public class CashlessImportData
    {
        public IEnumerable<CashlessRecord> CashlessRecords { get; set; }
        public CashlessDestinationRecord CashlessDestination { get; set; }
    }

    public class CashlessRecord
    {
        public int Type { get; set; }
        public DateTime Date { get; set; }
        public string Number { get; set; }
        public int OperationType { get; set; }
        public string Amount { get; set; }
        public string Currency { get; set; }
        public string PaymentDescription { get; set; }
        public string PayeeBankBIC { get; set; }
        public string PayeeAccount { get; set; }
        public string PayeeName { get; set; }
        public string PayeeBankName { get; set; }
        public string PayeeITN { get; set; }
        public string PayeeKPP { get; set; }
        public string PayeeCorrAccount { get; set; }
    }

    public sealed class CashlessRecordMap : ClassMap<CashlessRecord>
    {
        public CashlessRecordMap()
        {
            Map(m => m.Type).Name("тип");
            Map(m => m.Date).Name("дата");
            Map(m => m.Number).Name("номер");
            Map(m => m.OperationType).Name("вид операции");
            Map(m => m.Amount).Name("сумма");
            Map(m => m.Currency).Name("валюта");
            Map(m => m.PaymentDescription).Name("основание платежа");
            Map(m => m.PayeeBankBIC).Name("бик банка получателя");
            Map(m => m.PayeeAccount).Name("счет получателя");
            Map(m => m.PayeeName).Name("наименование получателя");
            Map(m => m.PayeeBankName).Name("наименование банка получателя");
            Map(m => m.PayeeITN).Name("инн получателя");
        }
    }

    public class CashlessDestinationRecord
    {
        public CashlessDestinationRecord()
        {
            OpeningBalance = "0,00";
            ClosingBalance = "0,00";
            DebitTurnover = "0,00";
            CreditTurnover = "0,00";
        }

        public string AccountNumber { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string OpeningBalance { get; set; }
        public string ClosingBalance { get; set; }
        public string DebitTurnover { get; set; }
        public string CreditTurnover { get; set; }
    }

    public sealed class CashlessDestinationRecordMap : ClassMap<CashlessDestinationRecord>
    {
        public CashlessDestinationRecordMap()
        {
            Map(m => m.AccountNumber).Name("номер счета");
            Map(m => m.FromDate).Name("начальная дата");
            Map(m => m.ToDate).Name("конечная дата");
            Map(m => m.OpeningBalance).Name("входящий остаток");
            Map(m => m.ClosingBalance).Name("исходящий остаток");
            Map(m => m.DebitTurnover).Name("обороты по дебету");
            Map(m => m.CreditTurnover).Name("обороты по кредиту");
        }
    }
}
