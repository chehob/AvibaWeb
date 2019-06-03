using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AvibaWeb.DomainModels;
using AvibaWeb.Infrastructure;
using AvibaWeb.Models;
using AvibaWeb.ViewModels.CorpReceiptViewModels;
using AvibaWeb.ViewModels.DataViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AvibaWeb.Controllers
{
    public class CorpReceiptController : Controller
    {
        private readonly AppIdentityDbContext _db;
        private readonly IViewRenderService _viewRenderService;
        private readonly IHostingEnvironment _hostingEnvironment;

        public CorpReceiptController(AppIdentityDbContext db, IViewRenderService viewRenderService,
            IHostingEnvironment hostingEnvironment)
        {
            _db = db;
            _viewRenderService = viewRenderService;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return PartialView();
        }

        [HttpGet]
        public IActionResult CreateReceipt(int? id)
        {
            var model = new CreateReceiptViewModel
            {
                Counterparties = (from c in _db.Counterparties
                    where c.Type.Description == "Корпоратор"
                    select c.Name).ToList(),
                Organizations = (from org in _db.Organizations
                    where org.IsActive
                    select org.Description).ToList()
            };

            if (id != null)
            {
                var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
                nfi.NumberGroupSeparator = " ";

                model.Receipt = (from cr in _db.CorporatorReceipts
                                 join operation in _db.CorporatorReceiptOperations on cr.CorporatorReceiptId equals operation.CorporatorReceiptId into operations
                                 from operation in operations.OrderByDescending(o => o.OperationDateTime).Take(1)
                                 where cr.CorporatorReceiptId == id
                    select new ReceiptEditData
                    {
                        ReceiptId = cr.CorporatorReceiptId,
                        CorporatorName = cr.Corporator == null ? "" : cr.Corporator.Name,
                        OrganizationName = cr.PayeeAccount == null || cr.PayeeAccount.Organization == null
                            ? ""
                            : cr.PayeeAccount.Organization.Description,
                        BankName = cr.PayeeAccount == null ? "" : cr.PayeeAccount.BankName,
                        FeeRate = cr.FeeRate.Value,
                        Items = (from item in _db.CorporatorReceiptItems
                            join ti in _db.VTicketOperations on item.TicketOperationId equals ti.TicketOperationId
                            where item.CorporatorReceiptId == cr.CorporatorReceiptId
                            select new TicketListViewModel
                            {
                                TicketOperationId = item.TicketOperationId.Value,
                                TicketNumber = ti.BSONumber,
                                Route = item.Route ?? ti.Route,
                                ExecutionDateTime = ti.ExecutionDateTime,
                                Payment = item.Amount.ToString("#,0.00", nfi),
                                OperationType = ti.OperationTypeID,
                                PassengerName = item.PassengerName ?? ti.PassengerName,
                                SegCount = ti.SegCount                               
                            }).ToList(),
                        CreatedDateTime = operation.OperationDateTime.ToString("d"),
                        IssuedDateTime = cr.IssuedDateTime != null ? cr.IssuedDateTime.Value.ToString("d") : "",
                        PaidDateTime = cr.PaidDateTime != null ? cr.PaidDateTime.Value.ToString("d") : "",
                        ReceiptNumber = cr.ReceiptNumber.Value.ToString(),
                        StatusId = cr.StatusId
                    }).FirstOrDefault();
            }
            else
            {
                model.Receipt = new ReceiptEditData
                {
                    FeeRate = 250,
                    CreatedDateTime = DateTime.Now.ToString("d"),
                    ReceiptNumber = "-"
                };
            }

            return PartialView(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateReceipt([FromBody]CreateReceiptPostViewModel model)
        {
            CorporatorReceipt receipt;
            if (model.ReceiptId != null && model.ReceiptId != 0)
            {
                receipt = _db.CorporatorReceipts.Include(cr => cr.Items)
                    .FirstOrDefault(cr => cr.CorporatorReceiptId == model.ReceiptId);
                receipt.CorporatorId = (from c in _db.Counterparties
                    where c.Name == model.PayerName
                    select c.ITN).FirstOrDefault();
                receipt.PayeeAccount = (from fa in _db.FinancialAccounts
                    join o in _db.Organizations on fa.OrganizationId equals o.OrganizationId
                    where o.Description == model.PayeeName && fa.BankName == model.PayeeBankName
                    select fa).FirstOrDefault();
                receipt.FeeRate = model.FeeRate.Length == 0 ? 0 : decimal.Parse(model.FeeRate);
                receipt.StatusId = model.StatusId;
                if (receipt.StatusId == CorporatorReceipt.CRPaymentStatus.Paid)
                {
                    receipt.PaidDateTime = DateTime.Now;
                }
                receipt.Amount = 0;
                receipt.TypeId = CorporatorReceipt.CRType.WebSite;

                if (model.IssuedDateTime != null)
                {
                    receipt.IssuedDateTime = DateTime.Parse(model.IssuedDateTime);
                }

                await _db.SaveChangesAsync();

                var viewModelItems = new List<CorporatorReceiptItem>();
                var receiptOldItems = receipt.Items.ToList();

                if (model.Items != null)
                    foreach (var i in model.Items)
                    {
                        var item = new CorporatorReceiptItem
                        {
                            Receipt = receipt,
                            TicketOperationId = int.Parse(i.TicketOperationId),
                            Amount = i.Amount,
                            PassengerName = i.PassengerName,
                            Route = i.Route,
                            FeeRate = receipt.FeeRate.Value,
                            TypeId = i.TypeId
                        };

                        var itemFee = (item.IsPercent ?
                            item.Amount * item.FeeRate / 100 :
                            (item.PerSegment ?
                                i.SegCount * item.FeeRate :
                                item.FeeRate));
                        receipt.Amount += item.Amount + itemFee;

                        receipt.Items.Add(item);
                        viewModelItems.Add(item);
                    }

                var itemsToRemove = receiptOldItems.Except(viewModelItems);
                foreach (var i in itemsToRemove)
                {
                    receipt.Items.Remove(i);
                    _db.Entry(i).State = EntityState.Deleted;
                }
            }
            else
            {
                receipt = new CorporatorReceipt
                {
                    CorporatorId = (from c in _db.Counterparties
                        where c.Name == model.PayerName
                        select c.ITN).FirstOrDefault(),
                    PayeeAccount = (from fa in _db.FinancialAccounts
                        join o in _db.Organizations on fa.OrganizationId equals o.OrganizationId
                        where o.Description == model.PayeeName && fa.BankName == model.PayeeBankName
                        select fa).FirstOrDefault(),
                    FeeRate = model.FeeRate.Length == 0 ? 0 : decimal.Parse(model.FeeRate),
                    Amount = 0,
                    StatusId = model.StatusId,
                    TypeId = CorporatorReceipt.CRType.WebSite
                };

                if (model.IssuedDateTime != null)
                {
                    receipt.IssuedDateTime = DateTime.Parse(model.IssuedDateTime);
                }

                if (receipt.StatusId == CorporatorReceipt.CRPaymentStatus.Paid)
                {
                    receipt.PaidDateTime = DateTime.Now;
                }

                foreach (var item in model.Items)
                {
                    var receiptItem = new CorporatorReceiptItem
                    {
                        Receipt = receipt,
                        TicketOperationId = int.Parse(item.TicketOperationId),
                        Amount = item.Amount,
                        PassengerName = item.PassengerName,
                        Route = item.Route,
                        FeeRate = receipt.FeeRate.Value,
                        TypeId = item.TypeId
                    };
                    _db.CorporatorReceiptItems.Add(receiptItem);

                    var itemFee = (receiptItem.IsPercent ?
                            receiptItem.Amount * receiptItem.FeeRate / 100 :
                            (receiptItem.PerSegment ?
                                item.SegCount * receiptItem.FeeRate :
                                receiptItem.FeeRate));
                    receipt.Amount += receiptItem.Amount + itemFee;
                }

                var operation = new CorporatorReceiptOperation
                {
                    Receipt = receipt,
                    OperationDateTime = DateTime.Now,
                    OperationTypeId = CorporatorReceiptOperation.CROType.New
                };

                _db.CorporatorReceiptOperations.Add(operation);
            }

            await _db.SaveChangesAsync();

            return Json(new { message = "Ok" });
        }

        [HttpPost]
        public async Task<IActionResult> TicketList([FromBody]TicketListRequest request)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            //var exceptOperationsId = request.ExceptItems.Select(x => int.Parse(x.TicketOperationId)).ToArray();
            var model = (from tio in _db.VTicketOperations
                         where tio.ExecutionDateTime >= DateTime.Parse(request.fromDate) && tio.ExecutionDateTime < DateTime.Parse(request.toDate).AddDays(1) &&
                            //!exceptOperationsId.Contains(tio.TicketOperationId) &&
                               !_db.CorporatorReceiptItems.Any(i => i.TicketOperationId == tio.TicketOperationId)
                               && ( tio.TicketTypeId == null || tio.TicketTypeId == 1 )
                         orderby tio.ExecutionDateTime descending
                         select new TicketListViewModel
                         {
                             TicketOperationId = tio.TicketOperationId,
                             TicketNumber = tio.BSONumber,
                             Route = tio.Route,
                             ExecutionDateTime = tio.ExecutionDateTime,
                             Payment = tio.Payment.ToString("#,0.00", nfi),
                             OperationType = tio.OperationTypeID,
                             PassengerName = tio.PassengerName,
                             SegCount = tio.SegCount
                         }).ToList();

            return Json(new { message = await _viewRenderService.RenderToStringAsync("CorpReceipt/TicketList", model) });
        }

        [HttpGet]
        public IActionResult Receipts()
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var model = (from cr in _db.CorporatorReceipts
                    .Include(c => c.PayeeAccount.Organization)
                         join operation in _db.CorporatorReceiptOperations on cr.CorporatorReceiptId equals operation.CorporatorReceiptId into operations
                        from operation in operations.OrderByDescending(o => o.OperationDateTime).Take(1)
                         orderby cr.CorporatorReceiptId descending
                         where cr.TypeId == CorporatorReceipt.CRType.WebSite
                         select new ReceiptsViewModel
                         {
                             ReceiptNumber = cr.ReceiptNumber.ToString(),
                             ReceiptId = cr.CorporatorReceiptId,
                             CreatedDate = operation.OperationDateTime.ToString("d"),
                             IssuedDateTime = cr.IssuedDateTime != null ? cr.IssuedDateTime.Value.ToString("d") : "",
                             PaidDateTime = cr.PaidDateTime != null ? cr.PaidDateTime.Value.ToString("d") : "",
                             PayeeOrgName = cr.PayeeAccount.Organization.Description,
                             PayeeBankName = cr.PayeeAccount.BankName,
                             PayerOrgName = cr.Corporator.Name,
                             TotalStr = cr.Amount.Value.ToString("#,0.00", nfi),
                             Status = cr.StatusId
                         }).ToList();

            return PartialView(model);
        }

        [HttpGet]
        public IActionResult Receipts1()
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var model = (from cr in _db.CorporatorReceipts
                    .Include(c => c.PayeeAccount.Organization)
                join operation in _db.CorporatorReceiptOperations on cr.CorporatorReceiptId equals operation.CorporatorReceiptId into operations
                from operation in operations.OrderByDescending(o => o.OperationDateTime).Take(1)
                orderby cr.CorporatorReceiptId descending
                where cr.TypeId == CorporatorReceipt.CRType.WebSite
                select new ReceiptsViewModel
                {
                    ReceiptNumber = cr.ReceiptNumber.ToString(),
                    ReceiptId = cr.CorporatorReceiptId,
                    CreatedDate = operation.OperationDateTime.ToString("d"),
                    IssuedDateTime = cr.IssuedDateTime != null ? cr.IssuedDateTime.Value.ToString("d") : "",
                    PaidDateTime = cr.PaidDateTime != null ? cr.PaidDateTime.Value.ToString("d") : "",
                    PayeeOrgName = cr.PayeeAccount.Organization.Description,
                    PayeeBankName = cr.PayeeAccount.BankName,
                    PayerOrgName = cr.Corporator.Name,
                    TotalStr = cr.Amount.Value.ToString("#,0.00", nfi),
                    Status = cr.StatusId
                }).ToList();

            return PartialView(model);
        }

        [HttpPost]
        public IActionResult ReceiptPDFData(int id)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var model = (from cr in _db.CorporatorReceipts
                    .Include(c => c.PayeeAccount.Organization).ThenInclude(o => o.Counterparty)
                join cro in _db.CorporatorReceiptOperations on cr.CorporatorReceiptId equals cro.CorporatorReceiptId into operations
                from operation in operations.OrderByDescending(o => o.OperationDateTime).Take(1)
                where cr.CorporatorReceiptId == id
                let org = cr.PayeeAccount.Organization
                let orgc = org.Counterparty
                select new ReceiptPDFViewModel
                {
                    TotalAmount = cr.Amount.Value,
                    ReceiptNumber = cr.TypeId == CorporatorReceipt.CRType.CorpClient ?
                        "WR-" + cr.ReceiptNumber.ToString() :
                        cr.ReceiptNumber.ToString(),
                    PayerNameWithITN = $"{cr.Corporator.Name} ИНН: {cr.Corporator.ITN} ,КПП {cr.Corporator.KPP}",
                    PayerName = cr.Corporator.Name,
                    PayerAddress = cr.Corporator.Address,
                    PayerCorrAccount = cr.Corporator.CorrespondentAccount,
                    PayerFinancialAccount = cr.Corporator.BankAccount,
                    PayerBankName = cr.Corporator.BankName,
                    PayerBIK = cr.Corporator.BIK,
                    PayerITN = cr.Corporator.ITN,
                    PayerKPP = cr.Corporator.KPP,
                    PayerHeadTitle = cr.Corporator.ManagementPosition,
                    PayerHeadName = cr.Corporator.ManagementName,
                    Items = (from item in _db.CorporatorReceiptItems
                        join ti in _db.VReceiptTicketInfo on item.TicketOperationId equals ti.TicketOperationId
                        where item.CorporatorReceiptId == cr.CorporatorReceiptId && item.TypeId == CorporatorReceiptItem.CRIType.Ticket
                        select new ReceiptPDFItem
                        {
                            Amount = item.Amount,
                            AmountStr = item.Amount.ToString("#,0.00", nfi),
                            TicketLabel = $"{ti.TicketLabel} {item.Route ?? ti.TicketRoute} {ti.BSOLabel}\n{item.PassengerName ?? ti.PassengerName}",
                            SegCount = ti.SegCount
                        }).ToList(),
                    LuggageItems = (from item in _db.CorporatorReceiptItems
                                    join ti in _db.VReceiptLuggageInfo on item.TicketOperationId equals ti.TicketOperationId
                                    where item.CorporatorReceiptId == cr.CorporatorReceiptId && item.TypeId == CorporatorReceiptItem.CRIType.Luggage
                                    select new ReceiptPDFItem
                                    {
                                        Amount = item.Amount,
                                        AmountStr = item.Amount.ToString("#,0.00", nfi),
                                        TicketLabel = $"Оформление багажа №{ti.LuggageNumber} к билету {ti.TicketNumber}",
                                        SegCount = 1
                                    }).ToList(),
                    Taxes = (from item in _db.CorporatorReceiptItems
                        join ti in _db.VReceiptTicketInfo on item.TicketOperationId equals ti.TicketOperationId
                        where item.CorporatorReceiptId == cr.CorporatorReceiptId && item.Amount != 0
                        group new { item, ti } by item.FeeRate
                        into groups
                        select new ReceiptTaxItem
                        {
                            FeeStr = (groups.FirstOrDefault().item.IsPercent ? groups.FirstOrDefault().item.Amount * groups.FirstOrDefault().item.FeeRate / 100 : groups.FirstOrDefault().item.FeeRate).ToString("#,0.00", nfi),
                            Amount = groups.Sum( g => g.item.IsPercent ? g.item.Amount * g.item.FeeRate / 100 : g.item.PerSegment ? g.item.FeeRate * g.ti.SegCount : g.item.FeeRate),
                            AmountStr = groups.Sum(g => g.item.IsPercent ? g.item.Amount * g.item.FeeRate / 100 : g.item.PerSegment ? g.item.FeeRate * g.ti.SegCount : g.item.FeeRate).ToString("#,0.00", nfi),
                            SegCount = groups.Sum( g => g.item.PerSegment ? g.ti.SegCount : 1 )
                        }).ToList(),
                    SignatureFileName = org.SignatureFileName,
                    StampFileName = org.StampFileName,
                    OrgHeadTitle = org.HeadTitle,
                    OrgHeadName = org.HeadName,
                    OrgITN = orgc.ITN,
                    OrgKPP = orgc.KPP,
                    OrgName = org.Description,
                    OrgCorrAccount = cr.PayeeAccount.CorrespondentAccount,
                    OrgFinancialAccount = cr.PayeeAccount.Description,
                    OrgBankName = cr.PayeeAccount.OffBankName,
                    OrgBIK = cr.PayeeAccount.BIK,
                    OrgAddress = orgc.Address,
                    FeeRate = cr.FeeRate.Value,
                    FeeRateStr = cr.FeeRate.Value.ToString("#,0.00", nfi),
                    IssuedDateTime = operation.OperationDateTime.ToShortDateString(),
                    PaymentTemplateLabelStr = cr.TypeId == CorporatorReceipt.CRType.CorpClient ?
                        $"Образец заполнения назначения платежа:" :
                        "",
                    PaymentTemplateStr = cr.TypeId == CorporatorReceipt.CRType.CorpClient ?
                        $"Оплата по счету WR-{cr.ReceiptNumber.ToString()} от {operation.OperationDateTime.ToShortDateString()} за билеты и сбор за оформление билетов. Без НДС" :
                        ""
                }).FirstOrDefault();

            model.ItemTotal = model.Items.Sum(i => i.Amount) + model.LuggageItems.Sum(i => i.Amount);
            model.ItemTotalStr = model.ItemTotal.ToString("#,0.00", nfi);
            model.SegCountTotal = model.Items.Sum(i => i.SegCount) + model.LuggageItems.Sum(i => i.SegCount);
            //model.FeeTotal = model.SegCountTotal * model.FeeRate;
            //model.FeeTotalStr = model.FeeTotal.ToString("#,0.00", nfi);
            model.FeeTotal = model.Taxes.Sum(t => t.Amount);
            model.FeeTotalStr = model.FeeTotal.ToString("#,0.00", nfi);
            model.TotalAmountStr = model.TotalAmount.ToString("#,0.00", nfi);
            model.SignatureImage = new Func<string>(() =>
            {
                if (model.SignatureFileName == null) return "";

                var path = _hostingEnvironment.WebRootPath + "/img/corpImages/" + model.SignatureFileName;
                var b = System.IO.File.ReadAllBytes(path);
                return "data:image/png;base64," + Convert.ToBase64String(b);
            })();
            model.StampImage = new Func<string>(() =>
            {
                if (model.StampFileName == null) return "";

                var path = _hostingEnvironment.WebRootPath + "/img/corpImages/" + model.StampFileName;
                var b = System.IO.File.ReadAllBytes(path);
                return "data:image/png;base64," + Convert.ToBase64String(b);
            })();

            return Json(model);
        }

        [HttpPost]
        public async Task<IActionResult> ClearReceipt(int id)
        {
            CorporatorReceipt receipt = _db.CorporatorReceipts.Include(cr => cr.Items)
                    .FirstOrDefault(cr => cr.CorporatorReceiptId == id);
            receipt.CorporatorId = null;
            receipt.PayeeAccountId = null;
            receipt.StatusId = CorporatorReceipt.CRPaymentStatus.Unpaid;
            receipt.Amount = 0;

            await _db.SaveChangesAsync();

            CorporatorReceiptItem[] itemsToRemove = new CorporatorReceiptItem[receipt.Items.Count];
            receipt.Items.CopyTo(itemsToRemove, 0);
            foreach (var i in itemsToRemove)
            {
                receipt.Items.Remove(i);
                _db.Entry(i).State = EntityState.Deleted;
            }

            await _db.SaveChangesAsync();

            return Json(new { message = "Ok" });
        }

        [HttpGet]
        public IActionResult OrganizationCorporators(string orgName)
        {
            var model = new OrganizationFinancialAccountsViewModel
            {
                Accounts = orgName == null
                    ? (from c in _db.Counterparties
                        where c.Type.Description == "Корпоратор"
                        select c.Name).ToList()
                    : (from o in _db.Organizations
                        join cd in _db.CorporatorDocuments on o.OrganizationId equals cd.OrganizationId
                        join c in _db.Counterparties on cd.ITN equals c.ITN
                        where o.Description == orgName
                        select c.Name).Distinct().ToList()
            };

            return PartialView(model);
        }

        [HttpGet]
        public IActionResult CorporatorOrganizations(string corpName)
        {
            var model = new OrganizationFinancialAccountsViewModel
            {
                Accounts = corpName == null
                    ? (from org in _db.Organizations
                        where org.IsActive
                        select org.Description).ToList()
                    : (from c in _db.Counterparties
                        join cd in _db.CorporatorDocuments on c.ITN equals cd.ITN
                        join o in _db.Organizations on cd.OrganizationId equals o.OrganizationId
                        where c.Name == corpName
                        select o.Description).Distinct().ToList()
            };

            return PartialView(model);
        }
    }
}
