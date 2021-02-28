using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AvibaWeb.DomainModels;
using AvibaWeb.Infrastructure;
using AvibaWeb.Models;
using AvibaWeb.ViewModels.AdminViewModels;
using AvibaWeb.ViewModels.DataViewModels;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using MoreLinq;
using System.Globalization;
using AvibaWeb.ViewModels.CorpReceiptViewModels;

namespace AvibaWeb.Controllers
{
    public class DataController : Controller
    {
        private readonly AppIdentityDbContext _db;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IViewRenderService _viewRenderService;
        private readonly UserManager<AppUser> _userManager;

        public DataController(AppIdentityDbContext db, IHttpClientFactory httpClientFactory, IConfiguration configuration,
            IViewRenderService viewRenderService, UserManager<AppUser> usrMgr)
        {
            _db = db;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _viewRenderService = viewRenderService;
            _userManager = usrMgr;
        }

        public IActionResult Index()
        {
            return PartialView();
        }

        public IActionResult Cashless()
        {
            return PartialView();
        }

        [HttpGet]
        public IActionResult ImportCashlessData(CashlessImportViewModel model)
        {
            return PartialView(model);
        }

        [HttpPost]
        public async Task<IActionResult> ImportCashlessData(List<IFormFile> files)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var importData = new List<CashlessImportData>();
            foreach (var formFile in files)
            {
                var cashlessRecords = new List<CashlessRecord>();
                var cashlessDestination = new CashlessDestinationRecord();

                if (formFile.Length <= 0 || !formFile.FileName.Contains(".txt")) continue;

                string firstCsv;
                using (var memoryStream = new MemoryStream())
                {
                    await formFile.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;
                    using (var sr = new StreamReader(memoryStream, Encoding.GetEncoding("Windows-1251")))
                    {
                        firstCsv = sr.ReadToEnd();
                    }
                }

                // From date
                var index = firstCsv.IndexOf("ДатаНачала", StringComparison.Ordinal);
                var newLineIndex = firstCsv.IndexOf("\n", index, StringComparison.Ordinal);
                var sl = firstCsv.Substring(index, newLineIndex - index).Split("=");
                cashlessDestination.FromDate = DateTime.Parse(sl[1].Trim('\r'));

                // To date
                index = newLineIndex + 1;
                newLineIndex = firstCsv.IndexOf("\n", index, StringComparison.Ordinal);
                sl = firstCsv.Substring(index, newLineIndex - index).Split("=");
                cashlessDestination.ToDate = DateTime.Parse(sl[1].Trim('\r'));

                // Account number
                index = newLineIndex + 1;
                newLineIndex = firstCsv.IndexOf("\n", index, StringComparison.Ordinal);
                sl = firstCsv.Substring(index, newLineIndex - index).Split("=");
                cashlessDestination.AccountNumber = sl[1].Trim('\r');
                if (cashlessDestination.AccountNumber == "40702810710160006058")
                {
                    cashlessDestination.AccountNumber = "40702810510160006058";
                }

                index = firstCsv.IndexOf("СекцияРасчСчет", index, StringComparison.Ordinal);
                var rows = 0;
                while (index != -1)
                {
                    newLineIndex = firstCsv.IndexOf("\n", index, StringComparison.Ordinal);

                    // Opening balance
                    index = firstCsv.IndexOf("НачальныйОстаток", index, StringComparison.Ordinal);
                    newLineIndex = firstCsv.IndexOf("\n", index, StringComparison.Ordinal);
                    if (rows == 0)
                    {
                        sl = firstCsv.Substring(index, newLineIndex - index).Split("=");
                        cashlessDestination.OpeningBalance = sl[1].Trim('\r');
                    }

                    // Debit turnover
                    index = newLineIndex + 1;
                    newLineIndex = firstCsv.IndexOf("\n", index, StringComparison.Ordinal);
                    sl = firstCsv.Substring(index, newLineIndex - index).Split("=");
                    cashlessDestination.DebitTurnover =
                        (decimal.Parse(cashlessDestination.DebitTurnover, CultureInfo.InvariantCulture) +
                        decimal.Parse(sl[1].Trim('\r'), CultureInfo.InvariantCulture)).ToString();

                    // Credit turnover
                    index = newLineIndex + 1;
                    newLineIndex = firstCsv.IndexOf("\n", index, StringComparison.Ordinal);
                    sl = firstCsv.Substring(index, newLineIndex - index).Split("=");
                    cashlessDestination.CreditTurnover =
                        (decimal.Parse(cashlessDestination.CreditTurnover, CultureInfo.InvariantCulture) +
                        decimal.Parse(sl[1].Trim('\r'), CultureInfo.InvariantCulture)).ToString();

                    // Closing balance
                    index = newLineIndex + 1;
                    newLineIndex = firstCsv.IndexOf("\n", index, StringComparison.Ordinal);
                    sl = firstCsv.Substring(index, newLineIndex - index).Split("=");
                    cashlessDestination.ClosingBalance = sl[1].Trim('\r');

                    rows++;
                    index = firstCsv.IndexOf("СекцияРасчСчет", index, StringComparison.Ordinal);
                }

                cashlessDestination.OpeningBalance = decimal.Parse(cashlessDestination.OpeningBalance, CultureInfo.InvariantCulture).ToString("#,0.00", nfi);
                cashlessDestination.DebitTurnover = decimal.Parse(cashlessDestination.DebitTurnover, CultureInfo.InvariantCulture).ToString("#,0.00", nfi);
                cashlessDestination.CreditTurnover = decimal.Parse(cashlessDestination.CreditTurnover, CultureInfo.InvariantCulture).ToString("#,0.00", nfi);
                cashlessDestination.ClosingBalance = decimal.Parse(cashlessDestination.ClosingBalance, CultureInfo.InvariantCulture).ToString("#,0.00", nfi);

                var isDocTypeFound = false;
                var isDocFirstType = false;
                index = firstCsv.IndexOf("СекцияДокумент", StringComparison.Ordinal);
                while (index != -1)
                {
                    var recordStartIndex = index;
                    var record = new CashlessRecord();

                    newLineIndex = firstCsv.IndexOf("\n", index, StringComparison.Ordinal);

                    // Order Number
                    index = newLineIndex + 1;
                    newLineIndex = firstCsv.IndexOf("\n", index, StringComparison.Ordinal);
                    sl = firstCsv.Substring(index, newLineIndex - index).Split("=");
                    record.Number = sl[1].Trim('\r');

                    // Operation Date
                    index = newLineIndex + 1;
                    newLineIndex = firstCsv.IndexOf("\n", index, StringComparison.Ordinal);
                    sl = firstCsv.Substring(index, newLineIndex - index).Split("=");
                    record.Date = DateTime.Parse(sl[1].Trim('\r'));

                    // Payment Amount
                    index = newLineIndex + 1;
                    newLineIndex = firstCsv.IndexOf("\n", index, StringComparison.Ordinal);
                    sl = firstCsv.Substring(index, newLineIndex - index).Split("=");
                    record.Amount = decimal.Parse(sl[1].Trim('\r'), CultureInfo.InvariantCulture).ToString("#,0.00", nfi);

                    // Payee Account
                    index = firstCsv.IndexOf("ПлательщикСчет", index, StringComparison.Ordinal);
                    newLineIndex = firstCsv.IndexOf("\n", index, StringComparison.Ordinal);
                    sl = firstCsv.Substring(index, newLineIndex - index).Split("=");
                    var payAccount = sl[1].Trim('\r');

                    // Payee ITN
                    if (payAccount == cashlessDestination.AccountNumber)
                    {
                        record.Amount = record.Amount.Insert(0, "-");
                        index = firstCsv.IndexOf("ПолучательИНН", recordStartIndex, StringComparison.Ordinal);
                    }
                    else
                    {
                        index = firstCsv.IndexOf("ПлательщикИНН", recordStartIndex, StringComparison.Ordinal);
                    }
                    newLineIndex = firstCsv.IndexOf("\n", index, StringComparison.Ordinal);
                    sl = firstCsv.Substring(index, newLineIndex - index).Split("=");
                    record.PayeeITN = sl[1].Trim('\r');

                    // Payee Name
                    //index = newLineIndex + 1;
                    //newLineIndex = firstCsv.IndexOf("\n", index, StringComparison.Ordinal);
                    //sl = firstCsv.Substring(index, newLineIndex - index).Split("=");
                    //if (!isDocTypeFound)
                    //{
                    //    if (sl[0].Contains("ПлательщикКПП") || sl[0].Contains("ПолучательКПП"))
                    //    {
                    //        isDocFirstType = false;
                    //    }
                    //    else
                    //    {
                    //        isDocFirstType = true;
                    //    }
                    //    isDocTypeFound = true;
                    //}

                    //if (isDocFirstType)
                    //{
                    //    record.PayeeName = sl[1].Trim('\r');
                    //}
                    //else
                    //{
                    if (payAccount == cashlessDestination.AccountNumber)
                    {
                        index = firstCsv.IndexOf("Получатель1=", recordStartIndex, StringComparison.Ordinal);
                        if (index == -1)
                        {
                            index = firstCsv.IndexOf("Получатель=", recordStartIndex, StringComparison.Ordinal);
                        }
                    }
                    else
                    {
                        index = firstCsv.IndexOf("Плательщик1=", recordStartIndex, StringComparison.Ordinal);
                        if (index == -1)
                        {
                            index = firstCsv.IndexOf("Плательщик=", recordStartIndex, StringComparison.Ordinal);
                        }
                    }

                    newLineIndex = firstCsv.IndexOf("\n", index, StringComparison.Ordinal);
                    sl = firstCsv.Substring(index, newLineIndex - index).Split("=");
                    record.PayeeName = sl[1].Trim('\r');
                    //}

                    // Payee Account
                    index = firstCsv.IndexOf(
                        payAccount == cashlessDestination.AccountNumber ? "ПолучательСчет" : "ПлательщикСчет",
                        recordStartIndex, StringComparison.Ordinal);
                    newLineIndex = firstCsv.IndexOf("\n", index, StringComparison.Ordinal);
                    sl = firstCsv.Substring(index, newLineIndex - index).Split("=");
                    record.PayeeAccount = sl[1].Trim('\r');
                    if (record.PayeeAccount == "40702810710160006058")
                    {
                        record.PayeeAccount = "40702810510160006058";
                    }

                    // Payee Bank Name
                    index = firstCsv.IndexOf(
                        payAccount == cashlessDestination.AccountNumber ? "ПолучательБанк1=" : "ПлательщикБанк1=",
                        recordStartIndex, StringComparison.Ordinal);
                    newLineIndex = firstCsv.IndexOf("\n", index, StringComparison.Ordinal);
                    sl = firstCsv.Substring(index, newLineIndex - index).Split("=");
                    record.PayeeBankName = sl[1].Trim('\r');

                    index = firstCsv.IndexOf(
                        payAccount == cashlessDestination.AccountNumber ? "ПолучательБанк2=" : "ПлательщикБанк2=",
                        recordStartIndex, StringComparison.Ordinal);
                    if (index != -1)
                    {
                        newLineIndex = firstCsv.IndexOf("\n", index, StringComparison.Ordinal);
                        sl = firstCsv.Substring(index, newLineIndex - index).Split("=");
                        record.PayeeBankName += " " + sl[1].Trim('\r');
                    }

                    // Payee Bank BIC
                    index = firstCsv.IndexOf(
                        payAccount == cashlessDestination.AccountNumber ? "ПолучательБИК=" : "ПлательщикБИК=",
                        recordStartIndex, StringComparison.Ordinal);
                    newLineIndex = firstCsv.IndexOf("\n", index, StringComparison.Ordinal);
                    sl = firstCsv.Substring(index, newLineIndex - index).Split("=");
                    record.PayeeBankBIC = sl[1].Trim('\r');
                    if (record.PayeeAccount == "40702810510160006058" && record.PayeeBankBIC == "042007855")
                    {
                        record.PayeeBankBIC = "044525411";
                    }

                    // Payee KPP
                    index = firstCsv.IndexOf(
                        payAccount == cashlessDestination.AccountNumber ? "ПолучательКПП=" : "ПлательщикКПП=",
                        recordStartIndex, StringComparison.Ordinal);
                    if (index != -1)
                    {
                        newLineIndex = firstCsv.IndexOf("\n", index, StringComparison.Ordinal);
                        sl = firstCsv.Substring(index, newLineIndex - index).Split("=");
                        record.PayeeKPP = sl[1].Trim('\r');
                    }

                    // Payee Corr Account
                    index = firstCsv.IndexOf(
                        payAccount == cashlessDestination.AccountNumber ? "ПолучательКорсчет=" : "ПлательщикКорсчет=",
                        recordStartIndex, StringComparison.Ordinal);
                    newLineIndex = firstCsv.IndexOf("\n", index, StringComparison.Ordinal);
                    sl = firstCsv.Substring(index, newLineIndex - index).Split("=");
                    record.PayeeCorrAccount = sl[1].Trim('\r');

                    // Payment description
                    index = firstCsv.IndexOf("НазначениеПлатежа", recordStartIndex, StringComparison.Ordinal);
                    newLineIndex = firstCsv.IndexOf("\n", index, StringComparison.Ordinal);
                    sl = firstCsv.Substring(index, newLineIndex - index).Split("=", 2);
                    record.PaymentDescription = sl[1].Trim('\r');

                    cashlessRecords.Add(record);
                    index = firstCsv.IndexOf("СекцияДокумент", index, StringComparison.Ordinal);
                }

                var data = new CashlessImportData
                {
                    CashlessDestination = cashlessDestination,
                    CashlessRecords = cashlessRecords
                };

                importData.Add(data);
            }

            var importViewDataList = (from dataRecord in importData
                                      group dataRecord by dataRecord.CashlessDestination.AccountNumber
                into g
                                      let orderedGroup = g.OrderBy(r => r.CashlessDestination.FromDate)
                                      select new CashlessImportViewData
                                      {
                                          Destination = new CashlessDestinationRecord
                                          {
                                              AccountNumber = g.FirstOrDefault()?.CashlessDestination.AccountNumber,
                                              FromDate = orderedGroup.FirstOrDefault().CashlessDestination.FromDate,
                                              ToDate = orderedGroup.LastOrDefault().CashlessDestination.ToDate,
                                              OpeningBalance = orderedGroup.FirstOrDefault()?.CashlessDestination.OpeningBalance,
                                              ClosingBalance = orderedGroup.LastOrDefault()?.CashlessDestination.ClosingBalance,
                                              DebitTurnover = g.Sum(r =>
                                                  decimal.Parse(r.CashlessDestination.DebitTurnover
                                                      .Replace(" ", string.Empty), CultureInfo.InvariantCulture)).ToString("#,0.00", nfi),
                                              CreditTurnover = g.Sum(r =>
                                                  decimal.Parse(r.CashlessDestination.CreditTurnover
                                                      .Replace(" ", string.Empty), CultureInfo.InvariantCulture)).ToString("#,0.00", nfi)
                                          },
                                          CounterpartyGroups =
                                              (from r in g.SelectMany(r => r.CashlessRecords)
                                               group r by r.PayeeITN == "0000000000" || r.PayeeITN == "000000000000" ? r.PayeeName : r.PayeeITN
                                                  into cg
                                               select new CashlessImportCounterpartyGroup
                                               {
                                                   Name = cg.FirstOrDefault().PayeeName,
                                                   ITN = cg.FirstOrDefault().PayeeITN,
                                                   Records = cg.Select(cr => cr).ToList()
                                               }).ToList()
                                      }).ToList();

            await CheckMissingCounterparties(importViewDataList);

            var model = new CashlessImportViewModel
            {
                FinancialAccountImportData = importViewDataList,
                CounterpartyTypes = (from t in _db.CounterpartyTypes.Where(t => t.IsActive).OrderBy(t => t.Description)
                                     select new SelectListItem
                                     {
                                         Value = t.CounterpartyTypeId.ToString(),
                                         Text = t.Description
                                     }).ToList()
            };

            return PartialView("ImportCashlessData", model);
        }

        //[HttpPost]
        //public async Task<IActionResult> ImportCashlessData(List<IFormFile> files)
        //{
        //    var importData = new List<CashlessImportData>();
        //    foreach (var formFile in files)
        //    {
        //        var cashlessRecords = new List<CashlessRecord>();
        //        CashlessDestinationRecord cashlessDestination;

        //        if (formFile.Length <= 0 || !formFile.FileName.Contains(".csv")) continue;

        //        string firstCsv, secondCsv;
        //        using (var memoryStream = new MemoryStream())
        //        {
        //            await formFile.CopyToAsync(memoryStream);
        //            memoryStream.Position = 0;
        //            using (var sr = new StreamReader(memoryStream, Encoding.GetEncoding("Windows-1251")))
        //            {
        //                var firstHeaders = sr.ReadLine();
        //                var secondHeaders = sr.ReadLine();
        //                var secondData = sr.ReadLine();
        //                var firstData = sr.ReadToEnd();

        //                firstCsv = string.Join(System.Environment.NewLine, firstHeaders, firstData);
        //                secondCsv = string.Join(System.Environment.NewLine, secondHeaders, secondData);
        //            }
        //        }

        //        var config = new Configuration
        //        {
        //            Delimiter = ";",
        //            BadDataFound = context => { Debug.WriteLine($"Bad data found on row '{context.RawRow}'"); },
        //            MissingFieldFound = (headerNames, index, context) =>
        //            {
        //                Debug.WriteLine(
        //                    $"Field with names ['{string.Join("', '", headerNames)}'] at index '{index}' was not found.");
        //            },
        //            PrepareHeaderForMatch = header => header.ToLower()
        //        };

        //        using (var csvStream = new MemoryStream(Encoding.UTF8.GetBytes(firstCsv)))
        //        using (var textReader = new StreamReader(csvStream, Encoding.UTF8))
        //        using (var csv = new CsvReader(textReader, config))
        //        {
        //            csv.Configuration.RegisterClassMap<CashlessRecordMap>();
        //            cashlessRecords.AddRange(csv.GetRecords<CashlessRecord>().ToList());
        //        }

        //        using (var csvStream = new MemoryStream(Encoding.UTF8.GetBytes(secondCsv)))
        //        using (var textReader = new StreamReader(csvStream, Encoding.UTF8))
        //        using (var csv = new CsvReader(textReader, config))
        //        {
        //            csv.Configuration.RegisterClassMap<CashlessDestinationRecordMap>();
        //            cashlessDestination = csv.GetRecords<CashlessDestinationRecord>().FirstOrDefault();
        //        }

        //        var data = new CashlessImportData
        //        {
        //            CashlessDestination = cashlessDestination,
        //            CashlessRecords = cashlessRecords
        //        };

        //        importData.Add(data);
        //    }

        //    var importViewDataList = (from dataRecord in importData
        //                              group dataRecord by dataRecord.CashlessDestination.AccountNumber
        //        into g
        //                              let orderedGroup = g.OrderBy(r => r.CashlessDestination.FromDate)
        //                              select new CashlessImportViewData
        //                              {
        //                                  Destination = new CashlessDestinationRecord
        //                                  {
        //                                      AccountNumber = g.FirstOrDefault()?.CashlessDestination.AccountNumber,
        //                                      FromDate = orderedGroup.FirstOrDefault().CashlessDestination.FromDate,
        //                                      ToDate = orderedGroup.LastOrDefault().CashlessDestination.ToDate,
        //                                      OpeningBalance = orderedGroup.FirstOrDefault()?.CashlessDestination.OpeningBalance,
        //                                      ClosingBalance = orderedGroup.LastOrDefault()?.CashlessDestination.ClosingBalance,
        //                                      DebitTurnover = g.Sum(r =>
        //                                          decimal.Parse(r.CashlessDestination.DebitTurnover
        //                                              .Replace(" ", string.Empty), CultureInfo.InvariantCulture)).ToString(),
        //                                      CreditTurnover = g.Sum(r =>
        //                                          decimal.Parse(r.CashlessDestination.CreditTurnover
        //                                              .Replace(" ", string.Empty), CultureInfo.InvariantCulture)).ToString()
        //                                  },
        //                                  CounterpartyGroups =
        //                                      (from r in g.SelectMany(r => r.CashlessRecords)
        //                                       group r by r.PayeeITN == "0000000000" ? r.PayeeName : r.PayeeITN
        //                                          into cg
        //                                       select new CashlessImportCounterpartyGroup
        //                                       {
        //                                           Name = cg.FirstOrDefault().PayeeName,
        //                                           ITN = cg.FirstOrDefault().PayeeITN,
        //                                           Records = cg.Select(cr => cr).ToList()
        //                                       }).ToList()
        //                              }).ToList();

        //    await CheckMissingCounterparties(importViewDataList);

        //    var model = new CashlessImportViewModel
        //    {
        //        FinancialAccountImportData = importViewDataList,
        //        CounterpartyTypes = (from t in _db.CounterpartyTypes.Where(t => t.IsActive).OrderBy(t => t.Description)
        //                             select new SelectListItem
        //                             {
        //                                 Value = t.CounterpartyTypeId.ToString(),
        //                                 Text = t.Description
        //                             }).ToList()
        //    };

        //    return PartialView("ImportCashlessData", model);
        //}

        [HttpPost]
        public async Task<IActionResult> AddFinancialAccountOperations([FromBody] CashlessImportViewData model)
        {
            if (!ModelState.IsValid) return new BadRequestResult();

            var financialAccount =
                     _db.FinancialAccounts.Include(a => a.Organization).ThenInclude(o => o.Counterparty)
                         .FirstOrDefault(a => a.Description == model.Destination.AccountNumber);
            if (financialAccount == null) return new BadRequestResult();

            var corpReceiptPrefixes = _db.Organizations.Where(o => o.CorpReceiptPrefix != null)
                .Select(o => o.CorpReceiptPrefix.ToLower()).Distinct().ToList();

            foreach (var counterpartyGroup in model.CounterpartyGroups)
            {
                string userId = null;
                if (counterpartyGroup.IsUserITN)
                {
                    var user = _db.Users.FirstOrDefault(u =>
                        u.UserITN == counterpartyGroup.ITN || u.Name == counterpartyGroup.Name);
                    if (user == null) continue;
                    userId = user.Id;
                }

                foreach (var record in counterpartyGroup.Records)
                {
                    FinancialAccount transferAccount = null;
                    if (counterpartyGroup.IsTransferAccount)
                    {
                        transferAccount = _db.FinancialAccounts.FirstOrDefault(a => a.Description == record.PayeeAccount);
                    }

                    var dAmount = decimal.Parse(record.Amount.Replace(" ", string.Empty), CultureInfo.InvariantCulture);
                    var isCashRequest = (record.PaymentDescription.ToLower().Contains("внесение наличных") ||
                                         record.PaymentDescription.ToLower()
                                             .Contains("от реализации платных услуг") ||
                                         record.PaymentDescription.ToLower()
                                             .Contains("поступление выручки") ||
                                         record.PaymentDescription.ToLower()
                                             .Contains("внес.ср.")) && dAmount > 0;

                    var operationExists = _db.FinancialAccountOperations.Any(fao =>
                        fao.OrderNumber == record.Number &&
                        fao.FinancialAccountId == financialAccount.FinancialAccountId &&
                        ((isCashRequest || counterpartyGroup.IsTransferAccount && fao.TransferFinancialAccountId == transferAccount.FinancialAccountId) ||
                         (fao.CounterpartyId == record.PayeeITN || (userId != null && fao.UserId == userId))) &&
                        fao.Amount == dAmount &&
                        fao.OperationDateTime.Date == record.Date.ToLocalTime().Date);
                    if (operationExists) continue;

                    var operation = new FinancialAccountOperation
                    {
                        FinancialAccountId = financialAccount.FinancialAccountId,
                        Amount = dAmount,
                        OperationDateTime = record.Date.ToLocalTime(),
                        InsertDateTime = DateTime.Now,
                        Description = record.PaymentDescription,
                        OrderNumber = record.Number
                    };

                    CorporatorReceiptMultiPayment multiPaymentData = null;
                    if (counterpartyGroup.IsUserITN)
                    {
                        operation.UserId = userId;

                        var lowerPaymentDescription = record.PaymentDescription.ToLower();
                        if (lowerPaymentDescription.Contains("аванс") || lowerPaymentDescription.Contains("зарплата") ||
                             lowerPaymentDescription.Contains("заработная плата"))
                        {
                            var incomingExpenditure = new IncomingExpenditure
                            {
                                Amount = -operation.Amount,
                                IsProcessed = false,
                                FinancialAccountOperation = operation
                            };

                            _db.IncomingExpenditures.Add(incomingExpenditure);
                        }
                    }
                    else if (!isCashRequest && counterpartyGroup.IsTransferAccount)
                    {
                        operation.TransferAccount = transferAccount;

                        var transferOperation = new FinancialAccountOperation
                        {
                            FinancialAccountId = transferAccount.FinancialAccountId,
                            Amount = -dAmount,
                            OperationDateTime = record.Date.ToLocalTime(),
                            InsertDateTime = DateTime.Now,
                            Description = record.PaymentDescription,
                            OrderNumber = record.Number,
                            TransferAccount = financialAccount
                        };

                        transferAccount.Balance -= operation.Amount;

                        _db.FinancialAccountOperations.Add(transferOperation);
                    }
                    else
                    {
                        operation.CounterpartyId = record.PayeeITN;

                        Counterparty counterparty;
                        if (operation.Amount >= 0)
                        {
                            counterparty = _db.Counterparties.Include(c => c.LoanGroup)
                                .Include(c => c.SubagentData).FirstOrDefault(c => c.ITN == operation.CounterpartyId);
                            if (counterparty?.LoanGroup != null)
                            {
                                counterparty.LoanGroup.Balance -= operation.Amount;
                            }
                            else if (counterparty?.SubagentData != null)
                            {
                                counterparty.SubagentData.Balance += operation.Amount;
                            }
                        }
                        else
                        {
                            counterparty = _db.Counterparties.Include(c => c.ProviderBalance).FirstOrDefault(c => c.ITN == operation.CounterpartyId);
                            if (counterparty?.ProviderBalance != null && operation.CounterpartyId != "5448100656")
                            {
                                counterparty.ProviderBalance.Balance -= operation.Amount;
                            }
                        }

                        var corpClient = _db.Counterparties.Include(c => c.CorporatorAccount).FirstOrDefault(c => c.ITN == operation.CounterpartyId && c.Type.Description == "Корпоратор");
                        var lowerPaymentDescription = record.PaymentDescription.ToLower();
                        if (corpClient != null)
                        {
                            var isUnrecognizedOperation = false;
                            var multiPaymentType = CorporatorReceiptMultiPayment.CRMPType.CorpClient;
                            var foundPrefix = corpReceiptPrefixes.FirstOrDefault(p =>
                                lowerPaymentDescription.Contains(p.ToLower() + "-") ||
                                lowerPaymentDescription.Contains(p.ToLower() + " -"));
                            if (!string.IsNullOrEmpty(foundPrefix))
                            {
                                if(lowerPaymentDescription.Contains(foundPrefix + " -"))
                                {
                                    lowerPaymentDescription = lowerPaymentDescription.Remove(
                                        Regex.Matches(lowerPaymentDescription, foundPrefix + " -")[0].Index + 2, 1);
                                }

                                var foundIndexes = new List<int>();

                                var offset = 3;
                                var matches = Regex.Matches(lowerPaymentDescription, foundPrefix + "-");
                                foreach (Match match in matches)
                                {
                                    foundIndexes.Add(match.Index);
                                }

                                if (foundIndexes.Count > 0)
                                {
                                    var hasUnrecognizedReceipts = false;
                                    var receiptList = new List<CorporatorReceipt>();                                    
                                    string errorString = "";
                                    var hasPaidReceipts = false;
                                    foreach (var index in foundIndexes)
                                    {
                                        var beginStr = new string(lowerPaymentDescription.Skip(index + offset).ToArray());
                                        var endIndex = 6;
                                        var receiptStr = beginStr.Substring(0, endIndex).Trim().Split(' ')[0];
                                        var receiptNumber =
                                            int.Parse(new string(receiptStr.Where(char.IsDigit).Take(7).ToArray()), CultureInfo.InvariantCulture);

                                        var receipt = _db.CorporatorReceipts
                                            .FirstOrDefault(cr =>
                                                cr.ReceiptNumber == receiptNumber &&
                                                cr.CorporatorId == operation.CounterpartyId);

                                        if (receipt != null)
                                        {
                                            switch (receipt.TypeId)
                                            {
                                                case CorporatorReceipt.CRType.WebSite:
                                                    multiPaymentType = CorporatorReceiptMultiPayment.CRMPType
                                                        .CorpReceipt;
                                                    break;
                                                case CorporatorReceipt.CRType.CorpClient:
                                                    multiPaymentType = CorporatorReceiptMultiPayment.CRMPType
                                                        .CorpClient;
                                                    break;
                                                default:
                                                    throw new ArgumentOutOfRangeException();
                                            }
                                        }

                                        if (receipt == null)
                                        {
                                            errorString += "Неопознанный счет " + receiptNumber.ToString() + ". ";
                                            hasUnrecognizedReceipts = true;
                                        }
                                        else if (receipt.StatusId == CorporatorReceipt.CRPaymentStatus.Paid)
                                        {
                                            if (multiPaymentType == CorporatorReceiptMultiPayment.CRMPType.CorpClient)
                                            {
                                                hasPaidReceipts = true;
                                            }
                                            else
                                            {
                                                errorString += "Счет " + receiptNumber.ToString() + " уже оплачен. ";
                                                hasUnrecognizedReceipts = true;
                                            }
                                        }
                                        else
                                        { 
                                            receiptList.Add(receipt);
                                        }
                                    }

                                    var reminder = operation.Amount;
                                    decimal paidTotal = 0;
                                    foreach (var receipt in receiptList)
                                    {
                                        var receiptPaymentNeeded = receipt.StatusId == CorporatorReceipt.CRPaymentStatus.Partial ?
                                            receipt.Amount.GetValueOrDefault(0m) - receipt.PaidAmount.GetValueOrDefault(0m) :
                                            receipt.Amount.GetValueOrDefault(0m);
                                        var paidAmount = reminder >= receiptPaymentNeeded ? receiptPaymentNeeded : reminder;
                                        reminder -= paidAmount;
                                        paidTotal += paidAmount;
                                        receipt.PaidAmount = receipt.PaidAmount.GetValueOrDefault(0m) + paidAmount;
                                        receipt.StatusId = receipt.PaidAmount.GetValueOrDefault(0m) < receipt.Amount.GetValueOrDefault(0m) ?
                                                CorporatorReceipt.CRPaymentStatus.Partial :
                                                CorporatorReceipt.CRPaymentStatus.Paid;
                                        receipt.PaidDateTime = operation.OperationDateTime;
                                        if (multiPaymentType == CorporatorReceiptMultiPayment.CRMPType.CorpClient && 
                                            corpClient.CorporatorAccount != null)
                                        {
                                            AddCorporatorAccountPayment(corpClient, operation, paidAmount);
                                        }
                                        hasPaidReceipts = true;
                                    }

                                    if (reminder > 0)
                                    {
                                        if (hasPaidReceipts)
                                        {
                                            ProcessCorporatorDeposit(operation, reminder);
                                        }
                                        else if (hasUnrecognizedReceipts == false)
                                        {
                                            hasUnrecognizedReceipts = true;
                                            errorString = "Неоплаченный остаток";
                                        }
                                    }

                                    if (hasUnrecognizedReceipts && reminder >= 0)
                                    {
                                        multiPaymentData = new CorporatorReceiptMultiPayment
                                        {
                                            ErrorString = errorString,
                                            Amount = reminder,
                                            TypeId = multiPaymentType
                                        };
                                    }
                                }
                                else
                                {
                                    multiPaymentType = CorporatorReceiptMultiPayment.CRMPType.CorpClient;
                                    isUnrecognizedOperation = true;
                                }
                            }
                            else if ((lowerPaymentDescription.Contains("аванс") || lowerPaymentDescription.Contains("депозит")) &&
                                (lowerPaymentDescription.Contains("договор") || lowerPaymentDescription.Contains("дог.")) &&
                                (lowerPaymentDescription.Contains("предоплата") || lowerPaymentDescription.Contains("оплата")))
                            {
                                ProcessCorporatorDeposit(operation, operation.Amount);                                
                            }
                            else
                            {
                                isUnrecognizedOperation = true;
                            }

                            if (isUnrecognizedOperation)
                            {
                                multiPaymentData = new CorporatorReceiptMultiPayment
                                {
                                    ErrorString = "Нераспознанная операция",
                                    Amount = operation.Amount,
                                    TypeId = multiPaymentType
                                };
                            }
                        }
                        else if(!isCashRequest && counterparty?.ProviderBalance == null &&
                            counterparty?.SubagentData == null &&
                            counterparty?.LoanGroup == null &&
                            operation.Amount < 0)
                        {   
                            var incomingExpenditure = new IncomingExpenditure
                            {
                                Amount = -operation.Amount,
                                IsProcessed = false,
                                FinancialAccountOperation = operation
                            };

                            if (counterparty?.ExpenditureDeskGroupId != null && counterparty?.ExpenditureObjectId != null &&
                                !(lowerPaymentDescription.Contains("аванс") || lowerPaymentDescription.Contains("зарплата") ||
                                lowerPaymentDescription.Contains("заработная плата")))
                            {
                                var expenditure = new Expenditure
                                {
                                    Name = incomingExpenditure.FinancialAccountOperation.Description,
                                    Amount = incomingExpenditure.Amount.Value,
                                    DeskGroupId = counterparty.ExpenditureDeskGroupId.Value,
                                    TypeId = _db.ExpenditureTypes.FirstOrDefault(et => et.Description == "Расход").ExpenditureTypeId,
                                    ObjectId = counterparty.ExpenditureObjectId.Value,
                                    PaymentTypeId = PaymentTypes.Cashless,
                                    IncomingExpenditure = incomingExpenditure
                                };

                                var expenditureOperation = new ExpenditureOperation
                                {
                                    Expenditure = expenditure,
                                    OperationDateTime = DateTime.Now,
                                    OperationTypeId = ExpenditureOperation.EOType.New
                                };

                                incomingExpenditure.IsProcessed = true;

                                _db.ExpenditureOperations.Add(expenditureOperation);
                            }
                            else
                            {
                                _db.IncomingExpenditures.Add(incomingExpenditure);
                            }
                        }
                    }

                    financialAccount.LastUploadDate = DateTime.Now;
                    financialAccount.Balance += operation.Amount;

                    _db.FinancialAccountOperations.Add(operation);                    

                    if (isCashRequest)
                    {
                        var officeRole = _db.Roles.SingleOrDefault(r => r.Name.Contains("Офис"));
                        var office = _db.Users.FirstOrDefault(u => u.Roles.Any(r => r.RoleId == officeRole.Id));

                        office.Balance -= operation.Amount;
                    }

                    if (multiPaymentData != null)
                    {
                        multiPaymentData.FinancialAccountOperationId = operation.FinancialAccountOperationId;
                        _db.CorporatorReceiptMultiPayments.Add(multiPaymentData);
                    }
                }
            }

            var curUser = await _userManager.FindByIdAsync(_userManager.GetUserId(User));

            using (var transaction = await _db.Database.BeginTransactionAsync())
            {
                _db.SetUserContext(curUser.Id);
                await _db.SaveChangesAsync();

                transaction.Commit();
            }

            var hasMultiPayments = _db.CorporatorReceiptMultiPayments.Any(mp =>
                mp.TypeId == CorporatorReceiptMultiPayment.CRMPType.CorpClient && mp.IsProcessed == false);

            return Json(new { message = 
                hasMultiPayments ?
                    "hasMultiPayment" : 
                    await _viewRenderService.RenderToStringAsync("Data/RecordsAdded") });
        }

        private void AddCorporatorAccountPayment(Counterparty corpClient, FinancialAccountOperation operation, decimal paymentAmount)
        {
            var transaction = new CorporatorAccountTransaction
            {
                CorporatorAccount = corpClient.CorporatorAccount,
                OldBalance = corpClient.CorporatorAccount.Balance,
                Amount = paymentAmount,
                TransactionDateTime = DateTime.Now,
                TransactionItemId = operation.FinancialAccountOperationId,
                TypeId = CorporatorAccountTransaction.CATType.Payment
            };
            _db.CorporatorAccountTransactions.Add(transaction);
            corpClient.CorporatorAccount.Balance += paymentAmount;
            corpClient.CorporatorAccount.LastPaymentDate = operation.OperationDateTime;
        }

        private void ProcessCorporatorDeposit(FinancialAccountOperation operation, decimal amountReminder)
        {
            var corpClient = _db.Counterparties.Include(c => c.CorporatorAccount)
                .FirstOrDefault(c => c.ITN == operation.CounterpartyId && c.Type.Description == "Корпоратор");
            if (corpClient != null && corpClient.CorporatorAccount != null)
            {
                AddCorporatorAccountPayment(corpClient, operation, amountReminder);

                var reminder = amountReminder;
                var unpaidReceipts = _db.CorporatorReceipts
                    .Where(cr => cr.CorporatorId == corpClient.ITN &&
                                cr.StatusId != CorporatorReceipt.CRPaymentStatus.Paid && cr.Amount > 0)
                    .OrderBy(cr => cr.IssuedDateTime).ToList();
                while(unpaidReceipts.Count > 0 && reminder > 0)
                {
                    var receipt = unpaidReceipts[0];
                    unpaidReceipts.RemoveAt(0);

                    var receiptPaymentNeeded = receipt.StatusId == CorporatorReceipt.CRPaymentStatus.Partial ?
                                        receipt.Amount.GetValueOrDefault(0m) - receipt.PaidAmount.GetValueOrDefault(0m) :
                                        receipt.Amount.GetValueOrDefault(0m);
                    var paidAmount = reminder >= receiptPaymentNeeded ? receiptPaymentNeeded : reminder;
                    reminder -= paidAmount;
                    receipt.PaidAmount = receipt.PaidAmount.GetValueOrDefault(0m) + paidAmount;
                    receipt.StatusId = receipt.PaidAmount.GetValueOrDefault(0m) < receipt.Amount.GetValueOrDefault(0m) ?
                            CorporatorReceipt.CRPaymentStatus.Partial :
                            CorporatorReceipt.CRPaymentStatus.Paid;
                    receipt.PaidDateTime = operation.OperationDateTime;
                };
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddMissingCounterparty(CashlessImportCounterpartyGroup counterpartyGroup)
        {
            if (!ModelState.IsValid) return new BadRequestResult();

            _db.Counterparties.Add(counterpartyGroup.MissingCounterparty);
            await _db.SaveChangesAsync();

            return PartialView("CounterpartyAdded");
        }

        private async Task CheckMissingCounterparties(IReadOnlyCollection<CashlessImportViewData> dataList)
        {
            if (!dataList.Any()) return;

            var knownCounterpartyList = _db.Counterparties.ToList();
            var userList = _db.Users.ToList();

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://suggestions.dadata.ru/");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", _configuration["DaData:ApiKey"]);

            foreach (var importData in dataList)
            {
                foreach (var counterpartyGroup in importData.CounterpartyGroups)
                {
                    var dataRecord = counterpartyGroup.Records.FirstOrDefault();
                    if (dataRecord == null) continue;

                    var transferAccount = _db.FinancialAccounts.FirstOrDefault(a =>
                        a.BIK == dataRecord.PayeeBankBIC && a.Organization.CounterpartyId == dataRecord.PayeeITN);
                    if (transferAccount != null)
                    {
                        counterpartyGroup.IsKnownCounterparty = true;
                        counterpartyGroup.IsTransferAccount = true;
                        counterpartyGroup.ITN = transferAccount.FinancialAccountId.ToString();
                        continue;
                    }

                    counterpartyGroup.IsKnownCounterparty = knownCounterpartyList.Any(c => c.ITN == dataRecord.PayeeITN);
                    if (counterpartyGroup.IsKnownCounterparty)
                    {
                        var cparty = _db.Counterparties.FirstOrDefault(c => c.ITN == dataRecord.PayeeITN);
                        if (cparty.CorrespondentAccount == null)
                        {
                            cparty.CorrespondentAccount = dataRecord.PayeeCorrAccount;
                        }
                        if (cparty.KPP == null && dataRecord.PayeeKPP != null)
                        {
                            cparty.KPP = dataRecord.PayeeKPP;
                        }
                        if (cparty.BIK == null)
                        {
                            cparty.BIK = dataRecord.PayeeBankBIC;
                        }
                        if (cparty.BankAccount == null)
                        {
                            cparty.BankAccount = dataRecord.PayeeAccount;
                        }
                        if (cparty.BankName == null)
                        {
                            cparty.BankName = dataRecord.PayeeBankName;
                        }

                        await _db.SaveChangesAsync();
                        continue;
                    }

                    var ITNUser = userList.FirstOrDefault(u =>
                        u.UserITN == dataRecord.PayeeITN || dataRecord.PayeeName.Contains(u.Name));
                    if (ITNUser != null)
                    {
                        counterpartyGroup.IsKnownCounterparty = true;
                        counterpartyGroup.IsUserITN = true;
                        counterpartyGroup.ITN = ITNUser.UserITN;
                        continue;
                    }

                    var response = await client.PostAsJsonAsync("suggestions/api/4_1/rs/findById/party", new
                    {
                        query = dataRecord.PayeeITN
                    });
                    var httpClientTask = response.Content.ReadAsStringAsync();

                    var party = new Counterparty
                    {
                        ITN = dataRecord.PayeeITN,
                        Name = dataRecord.PayeeName,
                        BIK = dataRecord.PayeeBankBIC,
                        BankName = dataRecord.PayeeBankName,
                        BankAccount = dataRecord.PayeeAccount
                    };

                    var json = JObject.Parse(await httpClientTask);
                    if (json["suggestions"].Any())
                    {
                        //party.CorrespondentAccount = string.Empty;
                        party.KPP = (string)json["suggestions"][0]["data"]["kpp"];
                        party.OGRN = (string)json["suggestions"][0]["data"]["ogrn"];
                        //party.Phone = string.Empty;
                        party.Address = (string)json["suggestions"][0]["data"]["address"]["value"];
                        //party.Email = string.Empty;
                        if (json["suggestions"][0]["data"]["management"] != null && json["suggestions"][0]["data"]["management"].HasValues)
                        {
                            party.ManagementName = (string)json["suggestions"][0]["data"]["management"]["name"];
                            party.ManagementPosition = (string)json["suggestions"][0]["data"]["management"]["post"];
                        }
                    }
                    else
                    {
                        var userNameStrings = party.Name.Split(' ');
                        var user = new AppUser
                        {
                            UserName = $"{userNameStrings[1][0]}.{userNameStrings[0]}".ToLower(),
                            Name = party.Name,
                            UserITN = party.ITN == "0000000000" ? null : party.ITN
                        };
                        user.NormalizedUserName = user.UserName.ToUpper();

                        _db.Users.Add(user);
                        await _db.SaveChangesAsync();

                        counterpartyGroup.IsKnownCounterparty = true;
                        counterpartyGroup.IsUserITN = true;
                    }

                    counterpartyGroup.MissingCounterparty = party;
                }
            }
        }

        [HttpGet]
        public IActionResult CreateCashlessRecord()
        {
            var model = new CreateCashlessRecordViewModel
            {
                Counterparties = (from c in _db.Counterparties
                                  select c.Name).ToList(),
                Organizations = (from org in _db.Organizations
                                 select org.Description).ToList()
            };

            return PartialView(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCashlessRecord([FromBody] CreateCashlessRecordCreateViewModel model)
        {
            Counterparty counterparty = null;
            FinancialAccount financialAccount;
            FinancialAccount payeeFinancialAccount = null;
            if (model.PayeeBankName != null && model.PayerBankName != null)
            {
                financialAccount = (from fa in _db.FinancialAccounts
                                    join o in _db.Organizations on fa.OrganizationId equals o.OrganizationId
                                    where o.Description == model.PayerName && fa.BankName == model.PayerBankName
                                    select fa).FirstOrDefault();
                if (financialAccount == null)
                {
                    return Json(new { success = false, message = "" });
                }
                model.Amount = (-decimal.Parse(model.Amount.Replace(" ", string.Empty), CultureInfo.InvariantCulture)).ToString();

                payeeFinancialAccount = (from fa in _db.FinancialAccounts
                                         join o in _db.Organizations on fa.OrganizationId equals o.OrganizationId
                                         where o.Description == model.PayeeName && fa.BankName == model.PayeeBankName
                                         select fa).FirstOrDefault();
                if (payeeFinancialAccount == null)
                {
                    return Json(new { success = false, message = "" });
                }

                var opAmount = -decimal.Parse(model.Amount.Replace(" ", string.Empty), CultureInfo.InvariantCulture);
                var operationExists = _db.FinancialAccountOperations.Any(fao =>
                        fao.OrderNumber == model.OrderNumber &&
                        fao.FinancialAccountId == financialAccount.FinancialAccountId &&
                        fao.CounterpartyId == payeeFinancialAccount.Organization.Counterparty.ITN &&
                        fao.Amount == opAmount);
                if (!operationExists)
                {
                    var payeeOperation = new FinancialAccountOperation
                    {
                        FinancialAccountId = payeeFinancialAccount.FinancialAccountId,
                        Amount = opAmount,
                        OperationDateTime = DateTime.Now,
                        InsertDateTime = DateTime.Now,
                        Description = model.Description,
                        OrderNumber = model.OrderNumber,
                        TransferAccount = financialAccount
                    };

                    payeeFinancialAccount.Balance += payeeOperation.Amount;

                    _db.FinancialAccountOperations.Add(payeeOperation);
                }
            }
            else if (model.PayerBankName != null)
            {

                financialAccount = (from fa in _db.FinancialAccounts
                                    join o in _db.Organizations on fa.OrganizationId equals o.OrganizationId
                                    where o.Description == model.PayerName && fa.BankName == model.PayerBankName
                                    select fa).FirstOrDefault();
                if (financialAccount == null)
                {
                    return Json(new { success = false, message = "" });
                }
                model.Amount = (-decimal.Parse(model.Amount.Replace(" ", string.Empty), CultureInfo.InvariantCulture)).ToString();
                counterparty = _db.Counterparties.Include(c => c.LoanGroup).Include(c => c.SubagentData)
                    .Include(c => c.ProviderBalance)
                    .FirstOrDefault(c => c.Name == model.PayeeName);
            }
            else if (model.PayeeBankName != null)
            {
                financialAccount = (from fa in _db.FinancialAccounts
                                    join o in _db.Organizations on fa.OrganizationId equals o.OrganizationId
                                    where o.Description == model.PayeeName && fa.BankName == model.PayeeBankName
                                    select fa).FirstOrDefault();
                if (financialAccount == null)
                {
                    return Json(new { success = false, message = "" });
                }

                counterparty = _db.Counterparties.Include(c => c.LoanGroup).Include(c => c.SubagentData)
                    .Include(c => c.ProviderBalance)
                    .FirstOrDefault(c => c.Name == model.PayerName);
            }
            else
            {
                return Json(new { success = false, message = "" });
            }

            var faopAmount = decimal.Parse(model.Amount.Replace(" ", string.Empty), CultureInfo.InvariantCulture);
            var faoperationExists = false;
            if (model.PayeeBankName != null && model.PayerBankName != null)
            {
                faoperationExists = _db.FinancialAccountOperations.Any(fao =>
                    fao.OrderNumber == model.OrderNumber &&
                    fao.FinancialAccountId == financialAccount.FinancialAccountId &&
                    fao.CounterpartyId == payeeFinancialAccount.Organization.Counterparty.ITN &&
                    fao.Amount == faopAmount);
            }

            if (!faoperationExists)
            {
                var operation = new FinancialAccountOperation
                {
                    FinancialAccountId = financialAccount.FinancialAccountId,
                    Amount = faopAmount,
                    OperationDateTime = DateTime.Now,
                    InsertDateTime = DateTime.Now,
                    Description = model.Description,
                    OrderNumber = model.OrderNumber
                };

                if (payeeFinancialAccount != null)
                {
                    operation.TransferAccount = payeeFinancialAccount;
                }
                else
                {
                    operation.CounterpartyId = counterparty?.ITN;

                    if (operation.Amount >= 0)
                    {
                        if (counterparty?.LoanGroup != null)
                        {
                            counterparty.LoanGroup.Balance -= operation.Amount;
                        }
                        else if (counterparty?.SubagentData != null)
                        {
                            counterparty.SubagentData.Balance += operation.Amount;
                        }
                    }
                    else
                    {
                        if (counterparty?.ProviderBalance != null)
                        {
                            counterparty.ProviderBalance.Balance -= operation.Amount;
                        }
                    }
                }

                financialAccount.Balance += operation.Amount;

                _db.FinancialAccountOperations.Add(operation);
                await _db.SaveChangesAsync();
            }

            return Json(new { message = await _viewRenderService.RenderToStringAsync("Data/Index") });
        }

        [HttpGet]
        public IActionResult OrganizationFinancialAccounts(string orgName)
        {
            var org = _db.Organizations.Include(o => o.Accounts).FirstOrDefault(o => o.Description == orgName);
            if (org == null)
            {
                return Json(new { success = false, message = "" });
            }

            var model = new OrganizationFinancialAccountsViewModel
            {
                Accounts = (from a in org.Accounts
                            where a.IsActive
                            select new KeyValuePair<string,string>(a.FinancialAccountId.ToString(), a.BankName)).ToList()
            };

            return PartialView(model);
        }

        [HttpGet]
        public IActionResult CreateCounterparty()
        {
            var model = new CreateCounterpartyViewModel
            {
                CounterpartyTypes = (from t in _db.CounterpartyTypes.Where(t => t.IsActive).OrderBy(t => t.Description)
                                     select new SelectListItem
                                     {
                                         Value = t.CounterpartyTypeId.ToString(),
                                         Text = t.Description
                                     }).ToList()
            };
            return PartialView(model);
        }

        [HttpGet]
        public async Task<IActionResult> FindCounterparty(string ITN)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://suggestions.dadata.ru/");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", _configuration["DaData:ApiKey"]);

            var response = await client.PostAsJsonAsync("suggestions/api/4_1/rs/findById/party", new
            {
                query = ITN
            });
            var httpClientTask = response.Content.ReadAsStringAsync();

            var party = new Counterparty
            {
                ITN = ITN
            };

            var json = JObject.Parse(await httpClientTask);
            if (json["suggestions"].Any())
            {
                party.Name = (string)json["suggestions"][0]["data"]["name"]["short_with_opf"];
                party.KPP = (string)json["suggestions"][0]["data"]["kpp"];
                party.OGRN = (string)json["suggestions"][0]["data"]["ogrn"];
                party.Address = (string)json["suggestions"][0]["data"]["address"]["value"];
                if (json["suggestions"][0]["data"]["management"] != null && json["suggestions"][0]["data"]["management"].HasValues)
                {
                    party.ManagementName = (string)json["suggestions"][0]["data"]["management"]["name"];
                    party.ManagementPosition = (string)json["suggestions"][0]["data"]["management"]["post"];
                }
            }

            return Json(party);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCounterparty(CreateCounterpartyViewModel model)
        {
            if (model.Counterparty?.Name != null)
            {
                var dbCounterparty = _db.Counterparties.FirstOrDefault(c => c.ITN == model.Counterparty.ITN);
                if (dbCounterparty == null)
                {
                    _db.Counterparties.Add(model.Counterparty);
                }
                else
                {
                    dbCounterparty.KPP = model.Counterparty.KPP;
                    dbCounterparty.ManagementName = model.Counterparty.ManagementName;
                    dbCounterparty.ManagementPosition = model.Counterparty.ManagementPosition;
                    dbCounterparty.Name = model.Counterparty.Name;
                    dbCounterparty.OGRN = model.Counterparty.OGRN;
                    dbCounterparty.Phone = model.Counterparty.Phone;
                    dbCounterparty.TypeId = model.Counterparty.TypeId;
                    dbCounterparty.Address = model.Counterparty.Address;
                    dbCounterparty.BankAccount = model.Counterparty.BankAccount;
                    dbCounterparty.BankName = model.Counterparty.BankName;
                    dbCounterparty.BIK = model.Counterparty.BIK;
                    dbCounterparty.CorrespondentAccount = model.Counterparty.CorrespondentAccount;
                    dbCounterparty.Email = model.Counterparty.Email;
                }

                
                if (model.Counterparty.TypeId == 1)
                {
                    var corpAccount = _db.CorporatorAccounts.FirstOrDefault(ca => ca.ITN == model.Counterparty.ITN);
                    if (corpAccount == null)
                    {
                        var account = new CorporatorAccount
                        {
                            BankName = model.Counterparty.BankName,
                            BIK = model.Counterparty.BIK,
                            CorrespondentAccount = model.Counterparty.CorrespondentAccount,
                            Description = model.Counterparty.BankAccount,
                            IsActive = true,
                            OffBankName = model.Counterparty.BankName,
                            ITN = model.Counterparty.ITN,                            
                            Balance = 0
                        };
                        _db.CorporatorAccounts.Add(account);
                    }
                }

                await _db.SaveChangesAsync();
            }

            return RedirectToAction("CreateCashlessRecord");
        }

        [HttpGet]
        public async Task<IActionResult> ProviderDeposit()
        {
            var model = new DepositViewModel
            {
                Deposits = await (from c in _db.Counterparties
                        .Include(c => c.ProviderBalance)
                                  where c.Type.Description == "Провайдер услуг"
                                  select new DepositItem
                                  {
                                      ITN = c.ITN,
                                      Name = c.Name,
                                      Deposit = c.ProviderBalance.Deposit,
                                      Type = DepositItem.DepositItemType.Provider
                                  }).ToListAsync()
            };

            model.Deposits.AddRange(await (from c in _db.Counterparties
                        .Include(c => c.SubagentData)
                                           where c.Type.Description == "Субагент Р"
                                           select new DepositItem
                                           {
                                               ITN = c.ITN,
                                               Name = c.Name,
                                               Deposit = c.SubagentData.Deposit,
                                               Type = DepositItem.DepositItemType.Subagent
                                           }).ToListAsync()
            );

            return PartialView(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProviderDeposit(DepositViewModel model)
        {
            foreach (var item in model.Deposits)
            {
                var counterparty = _db.Counterparties.Include(c => c.ProviderBalance).Include(c => c.SubagentData)
                    .FirstOrDefault(c => c.ITN == item.ITN);

                if (counterparty == null) continue;
                switch (item.Type)
                {
                    case DepositItem.DepositItemType.Provider:
                        if (counterparty.ProviderBalance.Deposit != item.Deposit)
                        {
                            counterparty.ProviderBalance.Deposit = item.Deposit;
                        }
                        break;
                    case DepositItem.DepositItemType.Subagent:
                        if (counterparty.SubagentData.Deposit != item.Deposit)
                        {
                            counterparty.SubagentData.Deposit = item.Deposit;
                        }
                        break;
                }
            }

            await _db.SaveChangesAsync();

            return RedirectToAction("ProviderDeposit");
        }


        public MultiPaymentViewModel GetMultiPaymentModel(CorporatorReceiptMultiPayment.CRMPType type)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            return new MultiPaymentViewModel
            {
                Items = (from p in _db.CorporatorReceiptMultiPayments.Include(p => p.FinancialAccountOperation)
                            .ThenInclude(fao => fao.Counterparty)
                         where p.TypeId == type
                         orderby p.FinancialAccountOperation.InsertDateTime descending
                         select new MultiPaymentItem
                         {
                             Amount = p.Amount.Value.ToString("#,0.00", nfi),
                             Description = p.FinancialAccountOperation.Description,
                             PaymentId = p.CorporatorReceiptMultiPaymentId,
                             CreatedDateTime = p.FinancialAccountOperation.OperationDateTime,
                             CounterpartyName = p.FinancialAccountOperation.Counterparty.Name,
                             IsProcessed = p.IsProcessed
                         }).ToList(),
                Type = type
            };
        }

        [HttpGet]
        public ActionResult MultiPayment(CorporatorReceiptMultiPayment.CRMPType type)
        {
            return PartialView(GetMultiPaymentModel(type));
        }

        [HttpGet]
        public ActionResult MultiPaymentProcess(int paymentId)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var model = new MultiPaymentProcessViewModel
            {
                Payment = _db.CorporatorReceiptMultiPayments.FirstOrDefault(p => p.CorporatorReceiptMultiPaymentId == paymentId),
                Counterparties = (from c in _db.Counterparties
                                  where c.Type.Description == "Корпоратор"
                                  select new KeyValuePair<string, string>(c.ITN, c.Name)).ToList()
            };            
            model.Receipts = (from r in _db.CorporatorReceipts
                              where r.StatusId != CorporatorReceipt.CRPaymentStatus.Paid && r.Amount > 0 &&
                                 ((model.Payment.TypeId == CorporatorReceiptMultiPayment.CRMPType.CorpClient && r.TypeId == CorporatorReceipt.CRType.CorpClient) ||
                                 (model.Payment.TypeId == CorporatorReceiptMultiPayment.CRMPType.CorpReceipt && r.TypeId == CorporatorReceipt.CRType.WebSite))
                              orderby r.IssuedDateTime descending
                              select new MultiPaymentReceipt
                              {
                                  AmountStr = ((r.Amount.GetValueOrDefault(0m)) - (r.PaidAmount.GetValueOrDefault(0m)))
                                    .ToString("#,0.00", nfi),
                                  ReceiptNumber = r.ReceiptNumber.Value,
                                  ReceiptId = r.CorporatorReceiptId,
                                  CorpName = r.Corporator.Name
                              }).ToList();

            return PartialView(model);
        }

        [HttpPost]
        public async Task<ActionResult> MultiPaymentProcess([FromBody]MultiPaymentProcessPostViewModel model)
        {
            var payment = _db.CorporatorReceiptMultiPayments.Include(mp => mp.FinancialAccountOperation)
                .ThenInclude(fao => fao.Counterparty)
                .FirstOrDefault(p => p.CorporatorReceiptMultiPaymentId == model.PaymentId);

            foreach (var item in model.Receipts)
            {
                var receipt = _db.CorporatorReceipts
                    .Include(cr => cr.Corporator).ThenInclude(c => c.CorporatorAccount)
                    .FirstOrDefault(cr => cr.CorporatorReceiptId == item.ReceiptId);

                receipt.PaidAmount = receipt.PaidAmount.GetValueOrDefault(0m) + item.Amount;
                receipt.StatusId = receipt.PaidAmount.GetValueOrDefault(0m) < receipt.Amount.GetValueOrDefault(0m) ?
                        CorporatorReceipt.CRPaymentStatus.Partial :
                        CorporatorReceipt.CRPaymentStatus.Paid;
                receipt.PaidDateTime = payment.FinancialAccountOperation.InsertDateTime;
                if (receipt.Corporator.CorporatorAccount != null)
                {
                    receipt.Corporator.CorporatorAccount.Balance += item.Amount;
                    receipt.Corporator.CorporatorAccount.LastPaymentDate = payment.FinancialAccountOperation.InsertDateTime;
                }
            }

            payment.IsProcessed = true;

            await _db.SaveChangesAsync();

            return Json(new { message = "Ok" });
        }

        [HttpPost]
        public async Task<ActionResult> MultiPaymentAddToBalance(int PaymentId, string PayerId, decimal Amount)
        {
            var payment = _db.CorporatorReceiptMultiPayments.Include(mp => mp.FinancialAccountOperation)
                .FirstOrDefault(p => p.CorporatorReceiptMultiPaymentId == PaymentId);

            var corporator = _db.Counterparties.Include(c => c.CorporatorAccount).FirstOrDefault(c => c.ITN == PayerId);
            
            if (corporator != null)
            {
                corporator.CorporatorAccount.Balance += Amount;
                corporator.CorporatorAccount.LastPaymentDate = DateTime.Now;
            }

            payment.IsProcessed = true;

            await _db.SaveChangesAsync();

            return Json(new { message = "Ok" });
        }

        [HttpPost]
        public async Task<ActionResult> TransferMultiPayment(int paymentId)
        {
            var payment = _db.CorporatorReceiptMultiPayments.FirstOrDefault(p => p.CorporatorReceiptMultiPaymentId == paymentId);
            payment.TypeId = CorporatorReceiptMultiPayment.CRMPType.CorpReceipt;

            await _db.SaveChangesAsync();

            return RedirectToAction("MultiPayment", new { type = CorporatorReceiptMultiPayment.CRMPType.CorpClient });
        }
    }
}