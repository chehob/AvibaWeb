﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using AvibaWeb.DomainModels;
using AvibaWeb.Infrastructure;
using AvibaWeb.Models;
using AvibaWeb.ViewModels.BookingManagement;
using AvibaWeb.ViewModels.CorpReceiptViewModels;
using AvibaWeb.ViewModels.DataViewModels;
using DocXToPdfConverter;
using DocXToPdfConverter.DocXToPdfHandlers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AvibaWeb.Controllers
{
    public class CorpReceiptController : Controller
    {
        private readonly AppIdentityDbContext _db;
        private readonly IViewRenderService _viewRenderService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;

        public CorpReceiptController(AppIdentityDbContext db, IViewRenderService viewRenderService,
            IHostingEnvironment hostingEnvironment, IConfiguration configuration)
        {
            _db = db;
            _viewRenderService = viewRenderService;
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
        }

        // GET: /<controller>/
        public IActionResult Index(bool isMobile = false)
        {
            if (isMobile)
            {
                return PartialView("../Mobile/CorpReceipt/Index");
            }
            else
            {
                return PartialView();
            }
        }

        public JsonResult OrganizationSelect(int? subGroupId = null)
        {
            SelectResult result = new SelectResult
            {
                results = new List<SelectResultItem>
                {
                    new SelectResultItem
                    {
                        id = "0",
                        text = "Организации",
                        children = (from org in _db.Organizations
                                where org.IsActive && (subGroupId == null || org.ReceiptNumberGroupId == subGroupId)
                                select new SelectResultItem
                                {
                                    id = org.OrganizationId.ToString(),
                                    text = org.Description
                                }).ToList()
                    }
                }
            };

            return this.Json(result);
        }

        public JsonResult CorporatorSelect()
        {
            SelectResult result = new SelectResult
            {
                results = new List<SelectResultItem>
                {
                    new SelectResultItem
                    {
                        id = "0",
                        text = "Корпораторы",
                        children = (from c in _db.Counterparties
                                  where c.Type.Description == "Корпоратор"
                                select new SelectResultItem
                                {
                                    id = c.ITN,
                                    text = c.Name
                                }).ToList()
                    }
                }
            };

            return this.Json(result);
        }

        public JsonResult CounterpartySelect()
        {
            SelectResult result = new SelectResult
            {
                results = new List<SelectResultItem>
                {
                    new SelectResultItem
                    {
                        id = "0",
                        text = "Корпораторы",
                        children = (from c in _db.Counterparties
                            select new SelectResultItem
                            {
                                id = c.ITN,
                                text = c.Name
                            }).ToList()
                    }
                }
            };

            return this.Json(result);
        }

        public JsonResult OrganizationAndCounterpartySelect(int? subGroupId = null)
        {
            SelectResult result = new SelectResult
            {
                results = new List<SelectResultItem>
                {
                    new SelectResultItem
                    {
                        id = "0",
                        text = "Корпораторы",
                        children = (from c in _db.Counterparties
                            select new SelectResultItem
                            {
                                id = c.ITN,
                                text = c.Name
                            }).ToList()
                    },
                    new SelectResultItem
                    {
                        id = "0",
                        text = "Организации",
                        children = (from org in _db.Organizations
                            where org.IsActive && (subGroupId == null || org.ReceiptNumberGroupId == subGroupId)
                            select new SelectResultItem
                            {
                                id = org.OrganizationId.ToString(),
                                text = org.Description
                            }).ToList()
                    }
                }
            };

            return this.Json(result);
        }

        public JsonResult OrganizationFinancialAccountSelect(string orgName)
        {
            SelectResult result = new SelectResult();
            var org = _db.Organizations.Include(o => o.Accounts).FirstOrDefault(o => o.Description == orgName);
            if (org != null)
            {
                result = new SelectResult
                {
                    results = (from a in org.Accounts
                               where a.IsActive
                               select new SelectResultItem
                               {
                                   id = a.FinancialAccountId.ToString(),
                                   text = a.BankName
                               }).ToList()
                };
            }

            return this.Json(result);
        }

        [HttpGet]
        public JsonResult OrganizationCorporatorsSelect(string orgName, bool isMobile = false)
        {
            SelectResult result = new SelectResult
            {
                results = new List<SelectResultItem>
                {
                    new SelectResultItem
                    {
                        id = "0",
                        text = "Корпораторы",
                        children = orgName == null
                            ? (from c in _db.Counterparties
                               where c.Type.Description == "Корпоратор"
                               select new SelectResultItem
                                {
                                    id = c.ITN,
                                    text = c.Name
                                }).ToList()
                            : (from o in _db.Organizations
                               join cd in _db.CorporatorDocuments on o.OrganizationId equals cd.OrganizationId
                               join c in _db.Counterparties on cd.ITN equals c.ITN
                               where o.Description == orgName
                               select new SelectResultItem
                                {
                                    id = c.ITN,
                                    text = c.Name
                                }).Distinct().ToList()
                    }
                }
            };

            return this.Json(result);
        }

        [HttpGet]
        public JsonResult CorporatorOrganizationsSelect(string corpName, bool isMobile = false)
        {
            SelectResult result = new SelectResult
            {
                results = new List<SelectResultItem>
                {
                    new SelectResultItem
                    {
                        id = "0",
                        text = "Корпораторы",
                        children = corpName == null
                            ? (from org in _db.Organizations
                               where org.IsActive
                               select new SelectResultItem
                                        {
                                            id = org.OrganizationId.ToString(),
                                            text = org.Description
                                        }).ToList()
                            : (from c in _db.Counterparties
                               join cd in _db.CorporatorDocuments on c.ITN equals cd.ITN
                               join org in _db.Organizations on cd.OrganizationId equals org.OrganizationId
                               where c.Name == corpName
                               select new SelectResultItem
                                        {
                                            id = org.OrganizationId.ToString(),
                                            text = org.Description
                                        }).Distinct().ToList()
                    }
                }
            };

            return this.Json(result);
        }

        [HttpGet]
        public IActionResult CreateReceipt(int? id, int subGroupId, bool isMobile = false, bool isVirtual = false)
        {
            var model = new CreateReceiptViewModel
            {
                SubGroupId = subGroupId,
                IsVirtual = isVirtual
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
                        CorporatorId = cr.Corporator == null ? "" : cr.Corporator.ITN,
                        CorporatorName = cr.Corporator == null ? "" : cr.Corporator.Name,
                        OrganizationId = cr.PayeeAccount == null || cr.PayeeAccount.Organization == null
                            ? 0
                            : cr.PayeeAccount.Organization.OrganizationId,
                        OrganizationName = cr.PayeeAccount == null || cr.PayeeAccount.Organization == null
                            ? ""
                            : cr.PayeeAccount.Organization.Description,
                        BankId = cr.PayeeAccount == null ? 0 : cr.PayeeAccount.FinancialAccountId,
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
                        ReceiptNumber = cr.ReceiptNumber != null ? cr.ReceiptNumber.ToString() : "",
                        StatusId = cr.StatusId,
                        VirtualSegCount = cr.VirtualSegCount.ToString(),
                        VirtualAmount = cr.Amount.ToString()
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

            if (isMobile)
            {
                return PartialView("../Mobile/CorpReceipt/CreateReceipt", model);
            }
            else
            {
                return PartialView(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateReceipt([FromBody]CreateReceiptPostViewModel model)
        {
            CorporatorReceipt receipt;

            if (model.ReceiptId != null && model.ReceiptId != 0)
            {
                var orgId = 0;
                receipt = _db.CorporatorReceipts.Include(cr => cr.Items)
                    .FirstOrDefault(cr => cr.CorporatorReceiptId == model.ReceiptId);
                receipt.CorporatorId = string.IsNullOrEmpty(model.PayerId) ? null : model.PayerId;
                receipt.PayeeAccount = (from fa in _db.FinancialAccounts
                    join o in _db.Organizations on fa.OrganizationId equals o.OrganizationId
                    where o.OrganizationId == (int.TryParse(model.PayeeId, out orgId) ? orgId : 0) && fa.BankName == model.PayeeBankName
                    select fa).FirstOrDefault();
                receipt.FeeRate = model.FeeRate.Length == 0 ? 0 : decimal.Parse(model.FeeRate, CultureInfo.InvariantCulture);
                receipt.StatusId = model.StatusId;
                if (receipt.StatusId == CorporatorReceipt.CRPaymentStatus.Paid)
                {
                    receipt.PaidDateTime = DateTime.Now;
                }
                receipt.Amount = 0;
                receipt.TypeId = CorporatorReceipt.CRType.WebSite;
                receipt.WebSiteSubGroupId = model.SubGroupId;

                if (model.IssuedDateTime != null)
                {
                    receipt.IssuedDateTime = DateTime.Parse(model.IssuedDateTime);
                }

                await _db.SaveChangesAsync();

                var viewModelItems = new List<CorporatorReceiptItem>();
                var receiptOldItems = receipt.Items.ToList();

                if (model.Items != null)
                {
                    foreach (var i in model.Items)
                    {
                        var item = new CorporatorReceiptItem
                        {
                            Receipt = receipt,
                            TicketOperationId = int.Parse(i.TicketOperationId, CultureInfo.InvariantCulture),
                            Amount = i.Amount,
                            PassengerName = i.PassengerName,
                            Route = i.Route,
                            FeeRate = receipt.FeeRate.Value,
                            TypeId = i.TypeId
                        };

                        var itemFee = (item.IsPercent
                            ? item.Amount * item.FeeRate / 100
                            : (item.PerSegment ? i.SegCount * item.FeeRate : item.FeeRate));
                        receipt.Amount += item.Amount + itemFee;

                        receipt.Items.Add(item);
                        viewModelItems.Add(item);
                    }
                }
                
                if (model.VirtualSegCount > 0)
                {
                    receipt.VirtualSegCount = model.VirtualSegCount;
                    receipt.Amount = model.ReceiptTotal;
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
                var orgId = 0;
                receipt = new CorporatorReceipt
                {
                    CorporatorId = string.IsNullOrEmpty(model.PayerId) ? null : model.PayerId,
                    PayeeAccount = (from fa in _db.FinancialAccounts
                                    join o in _db.Organizations on fa.OrganizationId equals o.OrganizationId
                                    where o.OrganizationId == (int.TryParse(model.PayeeId, out orgId) ? orgId : 0) && fa.BankName == model.PayeeBankName
                                    select fa).FirstOrDefault(),
                    FeeRate = model.FeeRate.Length == 0 ? 0 : decimal.Parse(model.FeeRate, CultureInfo.InvariantCulture),
                    Amount = 0,
                    StatusId = model.StatusId,
                    TypeId = CorporatorReceipt.CRType.WebSite,
                    WebSiteSubGroupId = model.SubGroupId,
                    VirtualSegCount = model.VirtualSegCount
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
                        TicketOperationId = int.Parse(item.TicketOperationId, CultureInfo.InvariantCulture),
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

                if (model.VirtualSegCount > 0)
                {
                    receipt.VirtualSegCount = model.VirtualSegCount;
                    receipt.Amount = model.ReceiptTotal;
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

            if (request.isMobile)
            {
                return Json(new { message = await _viewRenderService.RenderToStringAsync("../Mobile/CorpReceipt/TicketList", model) });
            }
            else
            {
                return Json(new { message = await _viewRenderService.RenderToStringAsync("CorpReceipt/TicketList", model) });
            }
            
        }

        [HttpGet]
        public IActionResult Receipts(bool isMobile = false)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var model = new CorpReceiptsViewModel
            {
                Items = (from cr in _db.CorporatorReceipts
                        .Include(c => c.PayeeAccount.Organization).ThenInclude(o => o.DocTemplates)
                    join operation in _db.CorporatorReceiptOperations on cr.CorporatorReceiptId equals operation
                        .CorporatorReceiptId into operations
                    from operation in operations.OrderByDescending(o => o.OperationDateTime).Take(1)
                    orderby cr.CorporatorReceiptId descending
                    where cr.TypeId == CorporatorReceipt.CRType.WebSite
                    select new CorpReceiptsItem
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
                        Status = cr.StatusId,
                        DocTemplates = cr.PayeeAccount.Organization.DocTemplates.ToList()
                    }).ToList()
            };

            if (isMobile)
            {
                return PartialView("../Mobile/CorpReceipt/Receipts", model);
            }
            else
            {
                return PartialView(model);
            }
        }

        [HttpGet]
        public IActionResult Receipts1(int subGroupId, bool isMobile = false)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var model = new CorpReceiptsViewModel
            {
                Items = (from cr in _db.CorporatorReceipts
                        .Include(c => c.PayeeAccount.Organization).ThenInclude(o => o.DocTemplates)
                    join operation in _db.CorporatorReceiptOperations on cr.CorporatorReceiptId equals operation
                        .CorporatorReceiptId into operations
                    from operation in operations.OrderByDescending(o => o.OperationDateTime).Take(1)
                    orderby cr.CorporatorReceiptId descending
                    where cr.TypeId == CorporatorReceipt.CRType.WebSite && cr.WebSiteSubGroupId == subGroupId
                    select new CorpReceiptsItem
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
                        Status = cr.StatusId,
                        DocTemplates = cr.PayeeAccount.Organization.DocTemplates.ToList(),
                        IsVirtual = cr.VirtualSegCount > 0
                    }).ToList(),
                SubGroupId = subGroupId
            };

            if (isMobile)
            {
                return PartialView("../Mobile/CorpReceipt/Receipts1", model);
            }
            else
            {
                return PartialView(model);
            }
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
                let ca = _db.CorporatorAccounts.Where(cai => cr.CorporatorId == cai.ITN).FirstOrDefault()
                where cr.CorporatorReceiptId == id
                let org = cr.PayeeAccount.Organization
                let orgc = org.Counterparty
                let cd = _db.CorporatorDocuments.Where(cdi => cr.CorporatorId == cdi.ITN && cr.PayeeAccount.OrganizationId == cdi.OrganizationId).OrderByDescending(cdi => cdi.Date).FirstOrDefault()
                select new ReceiptPDFViewModel
                {
                    TotalAmount = cr.Amount.Value,
                    ReceiptNumber = cr.PayeeAccount.Organization.CorpReceiptPrefix + "-" + cr.ReceiptNumber.ToString(),
                    PayerNameWithITN = $"{cr.Corporator.Name} ИНН: {cr.Corporator.ITN} ,КПП {cr.Corporator.KPP}",
                    PayerName = cr.Corporator.Name,
                    PayerAddress = cr.Corporator.Address,
                    PayerCorrAccount = ca == null ? cr.Corporator.CorrespondentAccount : ca.CorrespondentAccount,
                    PayerFinancialAccount = ca == null ? cr.Corporator.BankAccount : ca.Description,
                    PayerBankName = ca == null ? cr.Corporator.BankName : ca.OffBankName,
                    PayerBIK = ca == null ? cr.Corporator.BIK : ca.BIK,
                    PayerITN = cr.Corporator.ITN,
                    PayerKPP = cr.Corporator.KPP,
                    PayerHeadTitle = string.IsNullOrEmpty(cr.Corporator.ManagementPosition) ? "" : cr.Corporator.ManagementPosition,
                    PayerHeadName = cr.Corporator.ManagementName,
                    Items = (from item in _db.CorporatorReceiptItems
                        join ti in _db.VReceiptTicketInfo on item.TicketOperationId equals ti.TicketOperationId
                        where item.CorporatorReceiptId == cr.CorporatorReceiptId && item.TypeId == CorporatorReceiptItem.CRIType.Ticket
                        select new ReceiptPDFItem
                        {
                            Amount = item.Amount,
                            AmountStr = item.Amount.ToString("#,0.00", nfi),
                            TicketLabel = $"{ti.TicketLabel} {item.Route ?? ti.TicketRoute} {ti.BSOLabel}\n{item.PassengerName ?? ti.PassengerName}",
                            SegCount = ti.SegCount,
                            AmountLabelStr = (ti.TicketType == null || ti.TicketType != 3) ? "полетный\nсегмент" : "шт."
                        }).ToList(),
                    LuggageItems = (from item in _db.CorporatorReceiptItems
                                    join ti in _db.VReceiptLuggageInfo on item.TicketOperationId equals ti.TicketOperationId
                                    where item.CorporatorReceiptId == cr.CorporatorReceiptId && item.TypeId == CorporatorReceiptItem.CRIType.Luggage
                                    select new ReceiptPDFItem
                                    {
                                        Amount = item.Amount,
                                        AmountStr = item.Amount.ToString("#,0.00", nfi),
                                        TicketLabel = $"{ti.TicketLabel} №{ti.LuggageNumber} к билету {ti.TicketNumber}",
                                        SegCount = 1
                                    }).ToList(),
                    Taxes = (from item in _db.CorporatorReceiptItems
                        join ti in _db.VReceiptTicketInfo on item.TicketOperationId equals ti.TicketOperationId
                        where item.CorporatorReceiptId == cr.CorporatorReceiptId && item.Amount >= 0 && item.TypeId == CorporatorReceiptItem.CRIType.Ticket
                        group new { item, ti } by item.FeeRate
                        into groups
                        select new ReceiptTaxItem
                        {
                            FeeStr = (groups.FirstOrDefault().item.IsPercent ? groups.FirstOrDefault().item.Amount * groups.FirstOrDefault().item.FeeRate / 100 : groups.FirstOrDefault().item.FeeRate).ToString("#,0.00", nfi),
                            Amount = groups.Sum( g => g.item.IsPercent ? g.item.Amount * g.item.FeeRate / 100 : g.item.PerSegment ? g.item.FeeRate * g.ti.SegCount : g.item.FeeRate),
                            AmountStr = groups.Sum(g => g.item.IsPercent ? g.item.Amount * g.item.FeeRate / 100 : g.item.PerSegment ? g.item.FeeRate * g.ti.SegCount : g.item.FeeRate).ToString("#,0.00", nfi),
                            SegCount = groups.Sum( g => g.item.PerSegment ? g.ti.SegCount : 1 ),
                            AmountLabelStr = "шт.",
                            TicketLabel = "Сервисный сбор за оформление билета"
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
                    OrgAccountWarningStr = cr.PayeeAccount.Description == "40702810510160006058" ? "Внимание! Изменились реквизиты с 28.07.2020" : "",
                    FeeRate = cr.FeeRate.Value,
                    FeeRateStr = cr.FeeRate.Value.ToString("#,0.00", nfi),
                    IssuedDateTime = operation.OperationDateTime.ToShortDateString(),
                    PaymentTemplateLabelStr = "Образец заполнения назначения платежа:",
                    PaymentTemplateStr = $"Оплата по счету {cr.PayeeAccount.Organization.CorpReceiptPrefix}-{cr.ReceiptNumber.ToString()} от {operation.OperationDateTime.ToShortDateString()} за билеты и сбор за оформление билетов. Без НДС",
                    VirtualSegCount = cr.VirtualSegCount,
                    CounterpartyDocument = cd == null ? "" : cd.Document,
                    CounterpartyDocumentDate = cd == null ? "" : cd.Date.ToShortDateString()
                }).FirstOrDefault();

            //model.Taxes.AddRange(
            //    (from item in _db.CorporatorReceiptItems
            //    join ti in _db.VReceiptLuggageInfo on item.TicketOperationId equals ti.TicketOperationId
            //    where item.CorporatorReceiptId == id && item.Amount >= 0 && item.TypeId == CorporatorReceiptItem.CRIType.Luggage
            //    group new { item, ti } by item.FeeRate
            //    into groups
            //    select new ReceiptTaxItem
            //    {
            //        FeeStr = (groups.FirstOrDefault().item.IsPercent ? groups.FirstOrDefault().item.Amount * groups.FirstOrDefault().item.FeeRate / 100 : groups.FirstOrDefault().item.FeeRate).ToString("#,0.00", nfi),
            //        Amount = groups.Sum(g => g.item.IsPercent ? g.item.Amount * g.item.FeeRate / 100 : g.item.FeeRate),
            //        AmountStr = groups.Sum(g => g.item.IsPercent ? g.item.Amount * g.item.FeeRate / 100 : g.item.FeeRate).ToString("#,0.00", nfi),
            //        SegCount = 1,
            //        AmountLabelStr = "шт.",
            //        TicketLabel = groups.FirstOrDefault().ti.TicketLabel
            //    }).ToList());

            var luggageTaxes = (from item in _db.CorporatorReceiptItems
                                join ti in _db.VReceiptLuggageInfo on item.TicketOperationId equals ti.TicketOperationId
                                where item.CorporatorReceiptId == id && item.Amount >= 0 && item.TypeId == CorporatorReceiptItem.CRIType.Luggage
                                select new ReceiptTaxItem
                                {
                                    FeeStr = (item.IsPercent ? item.Amount * item.FeeRate / 100 : item.FeeRate).ToString("#,0.00", nfi),
                                    Amount = item.IsPercent ? item.Amount * item.FeeRate / 100 : item.FeeRate,
                                    AmountStr = (item.IsPercent ? item.Amount * item.FeeRate / 100 : item.FeeRate).ToString("#,0.00", nfi),
                                    SegCount = 1,
                                    AmountLabelStr = "шт.",
                                    TicketLabel = ti.TicketLabel
                                }).ToList();

            model.Taxes.AddRange(
                (from tax in luggageTaxes
                 group tax by tax.FeeStr
                 into groups
                 select new ReceiptTaxItem
                 {
                     FeeStr = groups.FirstOrDefault().FeeStr,
                     Amount = groups.Sum(g => g.Amount),
                     AmountStr = groups.Sum(g => g.Amount).ToString("#,0.00", nfi),
                     SegCount = groups.Count(),
                     AmountLabelStr = "шт.",
                     TicketLabel = groups.FirstOrDefault().TicketLabel
                 }).ToList());

            model.Taxes.AddRange(
                (from item in _db.CorporatorReceiptItems
                 join ti in _db.VReceiptTicketInfo on item.TicketOperationId equals ti.TicketOperationId
                 where item.CorporatorReceiptId == id && item.Amount < 0
                 select new ReceiptTaxItem
                 {
                     FeeStr = (item.IsPercent ? item.Amount * item.FeeRate / 100 : item.FeeRate).ToString("#,0.00", nfi),
                     Amount = item.IsPercent ? item.Amount * item.FeeRate / 100 : item.PerSegment ? item.FeeRate * ti.SegCount : item.FeeRate,
                     AmountStr = (item.IsPercent ? item.Amount * item.FeeRate / 100 : item.PerSegment ? item.FeeRate * ti.SegCount : item.FeeRate).ToString("#,0.00", nfi),
                     SegCount = item.PerSegment ? ti.SegCount : 1,
                     AmountLabelStr = "шт.",
                     TicketLabel = $"Сервисный сбор за возврат билета\n{item.Route ?? ti.TicketRoute} {ti.BSOLabel}\n{item.PassengerName ?? ti.PassengerName}"
                 }).ToList());

            if (model.VirtualSegCount > 0)
            {
                model.SegCountTotal = model.VirtualSegCount;
                model.FeeTotal = model.FeeRate * model.VirtualSegCount;
                model.ItemTotal = model.TotalAmount - model.FeeTotal;
            }
            else
            {
                model.ItemTotal = model.Items.Sum(i => i.Amount) + model.LuggageItems.Sum(i => i.Amount);
                model.SegCountTotal = model.Items.Sum(i => i.SegCount) + model.LuggageItems.Sum(i => i.SegCount);
                model.FeeTotal = model.Taxes.Sum(t => t.Amount);
            }

            model.ItemTotalStr = model.ItemTotal.ToString("#,0.00", nfi);
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
            model.HeaderImage = new Func<string>(() =>
            {
                var path = _hostingEnvironment.WebRootPath + "/img/corpImages/headerImage.png";
                var b = System.IO.File.ReadAllBytes(path);
                return "data:image/png;base64," + Convert.ToBase64String(b);
            })();

            return Json(model);
        }

        [HttpGet]
        public IActionResult CreatePDFFromTemplate(int id, int receiptId)
        {
            var template = _db.CorporatorReceiptTemplates.FirstOrDefault(crt => crt.CorporatorReceiptTemplateId == id);
            var receipt = _db.CorporatorReceipts.Include(cr => cr.PayeeAccount).ThenInclude(a => a.Organization)
                .ThenInclude(o => o.Counterparty)
                .Include(cr => cr.Corporator)
                .FirstOrDefault(cr => cr.CorporatorReceiptId == receiptId);
            if (template == null || receipt == null)
            {
                return new EmptyResult();
            }

            var path = _hostingEnvironment.WebRootPath + "/img/reportTemplate/" + template.FileName + ".docx";
            var path2 = _hostingEnvironment.WebRootPath + "/img/reportTemplate/" + Guid.NewGuid() + ".pdf";

            var locationOfLibreOfficeSoffice = _configuration["LibreOffice:Path"];

            var placeholders = new Placeholders();
            placeholders.TablePlaceholderStartTag = "==";
            placeholders.TablePlaceholderEndTag = "==";

            var receiptOperation = _db.CorporatorReceiptOperations
                .Where(cro => cro.CorporatorReceiptId == receipt.CorporatorReceiptId)
                .OrderByDescending(cro => cro.OperationDateTime).FirstOrDefault();

            placeholders.TextPlaceholders = new Dictionary<string, string>
            {
                {"PayeeITN", receipt.PayeeAccount.Organization.Counterparty.ITN },
                {"PayeeKPP", receipt.PayeeAccount.Organization.Counterparty.KPP },
                {"PayeeName", receipt.PayeeAccount.Organization.Description },
                {"PayeeAddress", receipt.PayeeAccount.Organization.Counterparty.Address },
                {"PayeeBankName", receipt.PayeeAccount.OffBankName },
                {"PayeeAccount", receipt.PayeeAccount.Description },
                {"PayeeBIK", receipt.PayeeAccount.BIK },
                {"PayeeCorrAccount", receipt.PayeeAccount.CorrespondentAccount },

                {"ReceiptNumber", receipt.PayeeAccount.Organization.CorpReceiptPrefix + "-" + receipt.ReceiptNumber },
                {"ReceiptDate", receiptOperation?.OperationDateTime.ToShortDateString() },

                {"PayerITN", receipt.Corporator.ITN },
                {"PayerKPP", receipt.Corporator.KPP },
                {"PayerName", receipt.Corporator.Name },
                {"PayerAddress", receipt.Corporator.Address },
            };

            placeholders.TablePlaceholders = new List<Dictionary<string, string[]>>
            {

                new Dictionary<string, string[]>()
                {
                    {"Name", new string[]{ "Homer Simpson", "Mr. Burns", "Mr. Smithers" }},
                }
            };

            placeholders.ImagePlaceholders = new Dictionary<string, ImageElement>
            {
                {
                    "AvibaHeader",
                    new ImageElement
                    {
                        Dpi = 150,
                        memStream = StreamHandler.GetFileAsMemoryStream(
                            _hostingEnvironment.WebRootPath + "/img/corpImages/headerImage.png")
                    }
                }
            };

            var test = new ReportGenerator(locationOfLibreOfficeSoffice);

            //Convert from DOCX to PDF
            test.Convert(path, path2, placeholders);

            var bytes = System.IO.File.ReadAllBytes(path2);
            System.IO.File.Delete(path2);

            var encodedOutputFileName =
                HttpUtility.UrlEncode($"Счет №{receipt.ReceiptNumber}.pdf", System.Text.Encoding.UTF8)
                    .Replace("+", " ");
            
            Response.Headers.Add("Content-Disposition", $"inline; filename={encodedOutputFileName}");
            return File(bytes, "application/pdf");
        }
        [HttpGet]
        public IActionResult CreatePDFFromTemplateTest(int id, int receiptId)
        {
            var template = _db.CorporatorReceiptTemplates.FirstOrDefault(crt => crt.CorporatorReceiptTemplateId == id);
            var receipt = _db.CorporatorReceipts.Include(cr => cr.PayeeAccount).ThenInclude(a => a.Organization)
                .ThenInclude(o => o.Counterparty)
                .Include(cr => cr.Corporator)
                .FirstOrDefault(cr => cr.CorporatorReceiptId == receiptId);
            if (template == null || receipt == null)
            {
                return new EmptyResult();
            }

            var path = _hostingEnvironment.WebRootPath + "/img/reportTemplate/" + template.FileName + ".docx";
            var outputFile = _hostingEnvironment.WebRootPath + "/img/reportTemplate/" + Guid.NewGuid();
            var path2 = outputFile + ".pdf";
            //var path2 = "C:/deploy/test/" + Guid.NewGuid() + ".pdf";

            var locationOfLibreOfficeSoffice = _configuration["LibreOffice:Path"];

            var placeholders = new Placeholders();
            placeholders.TablePlaceholderStartTag = "==";
            placeholders.TablePlaceholderEndTag = "==";

            var receiptOperation = _db.CorporatorReceiptOperations
                .Where(cro => cro.CorporatorReceiptId == receipt.CorporatorReceiptId)
                .OrderByDescending(cro => cro.OperationDateTime).FirstOrDefault();

            placeholders.TextPlaceholders = new Dictionary<string, string>
            {
                {"PayeeITN", receipt.PayeeAccount.Organization.Counterparty.ITN },
                {"PayeeKPP", receipt.PayeeAccount.Organization.Counterparty.KPP },
                {"PayeeName", receipt.PayeeAccount.Organization.Description },
                {"PayeeAddress", receipt.PayeeAccount.Organization.Counterparty.Address },
                {"PayeeBankName", receipt.PayeeAccount.OffBankName },
                {"PayeeAccount", receipt.PayeeAccount.Description },
                {"PayeeBIK", receipt.PayeeAccount.BIK },
                {"PayeeCorrAccount", receipt.PayeeAccount.CorrespondentAccount },

                {"ReceiptNumber", receipt.PayeeAccount.Organization.CorpReceiptPrefix + "-" + receipt.ReceiptNumber },
                {"ReceiptDate", receiptOperation?.OperationDateTime.ToShortDateString() },

                {"PayerITN", receipt.Corporator.ITN },
                {"PayerKPP", receipt.Corporator.KPP },
                {"PayerName", receipt.Corporator.Name },
                {"PayerAddress", receipt.Corporator.Address },
            };

            placeholders.TablePlaceholders = new List<Dictionary<string, string[]>>
            {

                new Dictionary<string, string[]>()
                {
                    {"Name", new string[]{ "Homer Simpson", "Mr. Burns", "Mr. Smithers" }},
                }
            };

            placeholders.ImagePlaceholders = new Dictionary<string, ImageElement>
            {
                {
                    "AvibaHeader",
                    new ImageElement
                    {
                        Dpi = 150,
                        memStream = StreamHandler.GetFileAsMemoryStream(
                            _hostingEnvironment.WebRootPath + "/img/corpImages/headerImage.png")
                    }
                }
            };

            var test = new ReportGenerator(locationOfLibreOfficeSoffice);

            //Convert from DOCX to PDF
            test.Convert(path, path2, placeholders);

            var bytes = System.IO.File.ReadAllBytes(path2);
            System.IO.File.Delete(path2);

            var encodedOutputFileName =
                HttpUtility.UrlEncode($"Счет №{receipt.ReceiptNumber}.pdf", System.Text.Encoding.UTF8)
                    .Replace("+", " ");

            Response.Headers.Add("Content-Disposition", $"inline; filename={encodedOutputFileName}");
            return File(bytes, "application/pdf");
        }


        [HttpPost]
        public IActionResult ReceiptAvibaPDFData(int id)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var model = (from cr in _db.CorporatorReceipts
                    .Include(c => c.PayeeAccount.Organization).ThenInclude(o => o.Counterparty)
                         join cro in _db.CorporatorReceiptOperations on cr.CorporatorReceiptId equals cro.CorporatorReceiptId into operations
                         from operation in operations.OrderByDescending(o => o.OperationDateTime).Take(1)
                         let ca = _db.CorporatorAccounts.Where(cai => cr.CorporatorId == cai.ITN).FirstOrDefault()
                         where cr.CorporatorReceiptId == id
                         let org = cr.PayeeAccount.Organization
                         let orgc = org.Counterparty
                         select new ReceiptPDFViewModel
                         {
                             TotalAmount = cr.Amount.Value,
                             ReceiptNumber = cr.PayeeAccount.Organization.CorpReceiptPrefix + "-" + cr.ReceiptNumber.ToString(),
                             PayerNameWithITN = $"{cr.Corporator.Name} ИНН: {cr.Corporator.ITN} ,КПП {cr.Corporator.KPP}",
                             PayerName = cr.Corporator.Name,
                             PayerAddress = cr.Corporator.Address,
                             PayerCorrAccount = ca == null ? cr.Corporator.CorrespondentAccount : ca.CorrespondentAccount,
                             PayerFinancialAccount = ca == null ? cr.Corporator.BankAccount : ca.Description,
                             PayerBankName = ca == null ? cr.Corporator.BankName : ca.OffBankName,
                             PayerBIK = ca == null ? cr.Corporator.BIK : ca.BIK,
                             PayerITN = cr.Corporator.ITN,
                             PayerKPP = cr.Corporator.KPP,
                             PayerHeadTitle = string.IsNullOrEmpty(cr.Corporator.ManagementPosition) ? "" : cr.Corporator.ManagementPosition,
                             PayerHeadName = cr.Corporator.ManagementName,
                             Items = (from item in _db.CorporatorReceiptItems
                                      join ti in _db.VReceiptTicketInfo on item.TicketOperationId equals ti.TicketOperationId
                                      where item.CorporatorReceiptId == cr.CorporatorReceiptId && item.TypeId == CorporatorReceiptItem.CRIType.Ticket
                                      select new ReceiptPDFItem
                                      {
                                          Amount = item.Amount,
                                          AmountStr = item.Amount.ToString("#,0.00", nfi),
                                          TicketLabel = $"{ti.TicketLabel} {item.Route ?? ti.TicketRoute}",
                                          SegCount = ti.SegCount,
                                          AmountLabelStr = (ti.TicketType == null || ti.TicketType != 3) ? "полетный\nсегмент" : "шт."
                                      }).ToList(),
                             LuggageItems = (from item in _db.CorporatorReceiptItems
                                             join ti in _db.VReceiptLuggageInfo on item.TicketOperationId equals ti.TicketOperationId
                                             where item.CorporatorReceiptId == cr.CorporatorReceiptId && item.TypeId == CorporatorReceiptItem.CRIType.Luggage
                                             select new ReceiptPDFItem
                                             {
                                                 Amount = item.Amount,
                                                 AmountStr = item.Amount.ToString("#,0.00", nfi),
                                                 TicketLabel = $"{ti.TicketLabel} №{ti.LuggageNumber} к билету {ti.TicketNumber}",
                                                 SegCount = 1
                                             }).ToList(),
                             Taxes = (from item in _db.CorporatorReceiptItems
                                      join ti in _db.VReceiptTicketInfo on item.TicketOperationId equals ti.TicketOperationId
                                      where item.CorporatorReceiptId == cr.CorporatorReceiptId && item.Amount >= 0 && item.TypeId == CorporatorReceiptItem.CRIType.Ticket
                                      group new { item, ti } by item.FeeRate
                                 into groups
                                      select new ReceiptTaxItem
                                      {
                                          FeeStr = (groups.FirstOrDefault().item.IsPercent ? groups.FirstOrDefault().item.Amount * groups.FirstOrDefault().item.FeeRate / 100 : groups.FirstOrDefault().item.FeeRate).ToString("#,0.00", nfi),
                                          Amount = groups.Sum(g => g.item.IsPercent ? g.item.Amount * g.item.FeeRate / 100 : g.item.PerSegment ? g.item.FeeRate * g.ti.SegCount : g.item.FeeRate),
                                          AmountStr = groups.Sum(g => g.item.IsPercent ? g.item.Amount * g.item.FeeRate / 100 : g.item.PerSegment ? g.item.FeeRate * g.ti.SegCount : g.item.FeeRate).ToString("#,0.00", nfi),
                                          SegCount = groups.Sum(g => g.item.PerSegment ? g.ti.SegCount : 1),
                                          AmountLabelStr = "шт.",
                                          TicketLabel = "Сервисный сбор за оформление билета"
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
                             PaymentTemplateLabelStr = "Образец заполнения назначения платежа:",
                             PaymentTemplateStr = $"Оплата по счету {cr.PayeeAccount.Organization.CorpReceiptPrefix}-{cr.ReceiptNumber.ToString()} от {operation.OperationDateTime.ToShortDateString()} за билеты и сбор за оформление билетов. Без НДС",
                             VirtualSegCount = cr.VirtualSegCount
                         }).FirstOrDefault();

            //model.Taxes.AddRange(
            //    (from item in _db.CorporatorReceiptItems
            //    join ti in _db.VReceiptLuggageInfo on item.TicketOperationId equals ti.TicketOperationId
            //    where item.CorporatorReceiptId == id && item.Amount >= 0 && item.TypeId == CorporatorReceiptItem.CRIType.Luggage
            //    group new { item, ti } by item.FeeRate
            //    into groups
            //    select new ReceiptTaxItem
            //    {
            //        FeeStr = (groups.FirstOrDefault().item.IsPercent ? groups.FirstOrDefault().item.Amount * groups.FirstOrDefault().item.FeeRate / 100 : groups.FirstOrDefault().item.FeeRate).ToString("#,0.00", nfi),
            //        Amount = groups.Sum(g => g.item.IsPercent ? g.item.Amount * g.item.FeeRate / 100 : g.item.FeeRate),
            //        AmountStr = groups.Sum(g => g.item.IsPercent ? g.item.Amount * g.item.FeeRate / 100 : g.item.FeeRate).ToString("#,0.00", nfi),
            //        SegCount = 1,
            //        AmountLabelStr = "шт.",
            //        TicketLabel = groups.FirstOrDefault().ti.TicketLabel
            //    }).ToList());

            var luggageTaxes = (from item in _db.CorporatorReceiptItems
                                join ti in _db.VReceiptLuggageInfo on item.TicketOperationId equals ti.TicketOperationId
                                where item.CorporatorReceiptId == id && item.Amount >= 0 && item.TypeId == CorporatorReceiptItem.CRIType.Luggage
                                select new ReceiptTaxItem
                                {
                                    FeeStr = (item.IsPercent ? item.Amount * item.FeeRate / 100 : item.FeeRate).ToString("#,0.00", nfi),
                                    Amount = item.IsPercent ? item.Amount * item.FeeRate / 100 : item.FeeRate,
                                    AmountStr = (item.IsPercent ? item.Amount * item.FeeRate / 100 : item.FeeRate).ToString("#,0.00", nfi),
                                    SegCount = 1,
                                    AmountLabelStr = "шт.",
                                    TicketLabel = ti.TicketLabel
                                }).ToList();

            model.Taxes.AddRange(
                (from tax in luggageTaxes
                 group tax by tax.FeeStr
                 into groups
                 select new ReceiptTaxItem
                 {
                     FeeStr = groups.FirstOrDefault().FeeStr,
                     Amount = groups.Sum(g => g.Amount),
                     AmountStr = groups.Sum(g => g.Amount).ToString("#,0.00", nfi),
                     SegCount = groups.Count(),
                     AmountLabelStr = "шт.",
                     TicketLabel = groups.FirstOrDefault().TicketLabel
                 }).ToList());

            model.Taxes.AddRange(
                (from item in _db.CorporatorReceiptItems
                 join ti in _db.VReceiptTicketInfo on item.TicketOperationId equals ti.TicketOperationId
                 where item.CorporatorReceiptId == id && item.Amount < 0
                 select new ReceiptTaxItem
                 {
                     FeeStr = (item.IsPercent ? item.Amount * item.FeeRate / 100 : item.FeeRate).ToString("#,0.00", nfi),
                     Amount = item.IsPercent ? item.Amount * item.FeeRate / 100 : item.PerSegment ? item.FeeRate * ti.SegCount : item.FeeRate,
                     AmountStr = (item.IsPercent ? item.Amount * item.FeeRate / 100 : item.PerSegment ? item.FeeRate * ti.SegCount : item.FeeRate).ToString("#,0.00", nfi),
                     SegCount = item.PerSegment ? ti.SegCount : 1,
                     AmountLabelStr = "шт.",
                     TicketLabel = $"Сервисный сбор за возврат билета\n{item.Route ?? ti.TicketRoute} {ti.BSOLabel}\n{item.PassengerName ?? ti.PassengerName}"
                 }).ToList());

            if (model.VirtualSegCount > 0)
            {
                model.SegCountTotal = model.VirtualSegCount;
                model.FeeTotal = model.FeeRate * model.VirtualSegCount;
                model.ItemTotal = model.TotalAmount - model.FeeTotal;
                model.ItemCount = 1;
            }
            else
            {
                model.ItemCount = model.Items.Count + model.LuggageItems.Count;
                model.ItemTotal = model.Items.Sum(i => i.Amount) + model.LuggageItems.Sum(i => i.Amount);
                model.SegCountTotal = model.Items.Sum(i => i.SegCount) + model.LuggageItems.Sum(i => i.SegCount);
                model.FeeTotal = model.Taxes.Sum(t => t.Amount);
            }

            model.ItemTotalStr = model.ItemTotal.ToString("#,0.00", nfi);
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
            model.HeaderImage = new Func<string>(() =>
            {
                var path = _hostingEnvironment.WebRootPath + "/img/corpImages/headerImage.png";
                var b = System.IO.File.ReadAllBytes(path);
                return "data:image/png;base64," + Convert.ToBase64String(b);
            })();

            return Json(model);
        }

        [HttpPost]
        public IActionResult ReceiptUstekPDFData(int id)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var model = (from cr in _db.CorporatorReceipts
                    .Include(c => c.PayeeAccount.Organization).ThenInclude(o => o.Counterparty)
                         join cro in _db.CorporatorReceiptOperations on cr.CorporatorReceiptId equals cro.CorporatorReceiptId into operations
                         from operation in operations.OrderByDescending(o => o.OperationDateTime).Take(1)
                         let ca = _db.CorporatorAccounts.Where(cai => cr.CorporatorId == cai.ITN).FirstOrDefault()
                         where cr.CorporatorReceiptId == id
                         let org = cr.PayeeAccount.Organization
                         let orgc = org.Counterparty
                         select new ReceiptPDFViewModel
                         {
                             TotalAmount = cr.Amount.Value,
                             ReceiptNumber = cr.PayeeAccount.Organization.CorpReceiptPrefix + "-" + cr.ReceiptNumber.ToString(),
                             PayerNameWithITN = $"{cr.Corporator.Name} ИНН: {cr.Corporator.ITN} ,КПП {cr.Corporator.KPP}",
                             PayerName = cr.Corporator.Name,
                             PayerAddress = cr.Corporator.Address,
                             PayerCorrAccount = ca == null ? cr.Corporator.CorrespondentAccount : ca.CorrespondentAccount,
                             PayerFinancialAccount = ca == null ? cr.Corporator.BankAccount : ca.Description,
                             PayerBankName = ca == null ? cr.Corporator.BankName : ca.OffBankName,
                             PayerBIK = ca == null ? cr.Corporator.BIK : ca.BIK,
                             PayerITN = cr.Corporator.ITN,
                             PayerKPP = cr.Corporator.KPP,
                             PayerHeadTitle = string.IsNullOrEmpty(cr.Corporator.ManagementPosition) ? "" : cr.Corporator.ManagementPosition,
                             PayerHeadName = cr.Corporator.ManagementName,
                             Items = (from item in _db.CorporatorReceiptItems
                                      join ti in _db.VReceiptTicketInfo on item.TicketOperationId equals ti.TicketOperationId
                                      where item.CorporatorReceiptId == cr.CorporatorReceiptId && item.TypeId == CorporatorReceiptItem.CRIType.Ticket
                                      select new ReceiptPDFItem
                                      {
                                          Amount = item.Amount,
                                          AmountStr = item.Amount.ToString("#,0.00", nfi),
                                          TicketLabel = $"{ti.TicketLabel} {item.Route ?? ti.TicketRoute}",
                                          SegCount = ti.SegCount,
                                          AmountLabelStr = (ti.TicketType == null || ti.TicketType != 3) ? "полетный\nсегмент" : "шт."
                                      }).ToList(),
                             LuggageItems = (from item in _db.CorporatorReceiptItems
                                             join ti in _db.VReceiptLuggageInfo on item.TicketOperationId equals ti.TicketOperationId
                                             where item.CorporatorReceiptId == cr.CorporatorReceiptId && item.TypeId == CorporatorReceiptItem.CRIType.Luggage
                                             select new ReceiptPDFItem
                                             {
                                                 Amount = item.Amount,
                                                 AmountStr = item.Amount.ToString("#,0.00", nfi),
                                                 TicketLabel = $"{ti.TicketLabel} №{ti.LuggageNumber} к билету {ti.TicketNumber}",
                                                 SegCount = 1
                                             }).ToList(),
                             Taxes = (from item in _db.CorporatorReceiptItems
                                      join ti in _db.VReceiptTicketInfo on item.TicketOperationId equals ti.TicketOperationId
                                      where item.CorporatorReceiptId == cr.CorporatorReceiptId && item.Amount >= 0 && item.TypeId == CorporatorReceiptItem.CRIType.Ticket
                                      group new { item, ti } by item.FeeRate
                                 into groups
                                      select new ReceiptTaxItem
                                      {
                                          FeeStr = (groups.FirstOrDefault().item.IsPercent ? groups.FirstOrDefault().item.Amount * groups.FirstOrDefault().item.FeeRate / 100 : groups.FirstOrDefault().item.FeeRate).ToString("#,0.00", nfi),
                                          Amount = groups.Sum(g => g.item.IsPercent ? g.item.Amount * g.item.FeeRate / 100 : g.item.PerSegment ? g.item.FeeRate * g.ti.SegCount : g.item.FeeRate),
                                          AmountStr = groups.Sum(g => g.item.IsPercent ? g.item.Amount * g.item.FeeRate / 100 : g.item.PerSegment ? g.item.FeeRate * g.ti.SegCount : g.item.FeeRate).ToString("#,0.00", nfi),
                                          SegCount = groups.Sum(g => g.item.PerSegment ? g.ti.SegCount : 1),
                                          AmountLabelStr = "шт.",
                                          TicketLabel = "Сервисный сбор за оформление билета"
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
                             PaymentTemplateLabelStr = "Образец заполнения назначения платежа:",
                             PaymentTemplateStr = $"Оплата по счету {cr.PayeeAccount.Organization.CorpReceiptPrefix}-{cr.ReceiptNumber.ToString()} от {operation.OperationDateTime.ToShortDateString()} за билеты и сбор за оформление билетов. Без НДС",
                             VirtualSegCount = cr.VirtualSegCount
                         }).FirstOrDefault();

            var luggageTaxes = (from item in _db.CorporatorReceiptItems
                                join ti in _db.VReceiptLuggageInfo on item.TicketOperationId equals ti.TicketOperationId
                                where item.CorporatorReceiptId == id && item.Amount >= 0 && item.TypeId == CorporatorReceiptItem.CRIType.Luggage
                                select new ReceiptTaxItem
                                {
                                    FeeStr = (item.IsPercent ? item.Amount * item.FeeRate / 100 : item.FeeRate).ToString("#,0.00", nfi),
                                    Amount = item.IsPercent ? item.Amount * item.FeeRate / 100 : item.FeeRate,
                                    AmountStr = (item.IsPercent ? item.Amount * item.FeeRate / 100 : item.FeeRate).ToString("#,0.00", nfi),
                                    SegCount = 1,
                                    AmountLabelStr = "шт.",
                                    TicketLabel = ti.TicketLabel
                                }).ToList();

            model.Taxes.AddRange(
                (from tax in luggageTaxes
                 group tax by tax.FeeStr
                 into groups
                 select new ReceiptTaxItem
                 {
                     FeeStr = groups.FirstOrDefault().FeeStr,
                     Amount = groups.Sum(g => g.Amount),
                     AmountStr = groups.Sum(g => g.Amount).ToString("#,0.00", nfi),
                     SegCount = groups.Count(),
                     AmountLabelStr = "шт.",
                     TicketLabel = groups.FirstOrDefault().TicketLabel
                 }).ToList());

            model.Taxes.AddRange(
                (from item in _db.CorporatorReceiptItems
                 join ti in _db.VReceiptTicketInfo on item.TicketOperationId equals ti.TicketOperationId
                 where item.CorporatorReceiptId == id && item.Amount < 0
                 select new ReceiptTaxItem
                 {
                     FeeStr = (item.IsPercent ? item.Amount * item.FeeRate / 100 : item.FeeRate).ToString("#,0.00", nfi),
                     Amount = item.IsPercent ? item.Amount * item.FeeRate / 100 : item.PerSegment ? item.FeeRate * ti.SegCount : item.FeeRate,
                     AmountStr = (item.IsPercent ? item.Amount * item.FeeRate / 100 : item.PerSegment ? item.FeeRate * ti.SegCount : item.FeeRate).ToString("#,0.00", nfi),
                     SegCount = item.PerSegment ? ti.SegCount : 1,
                     AmountLabelStr = "шт.",
                     TicketLabel = $"Сервисный сбор за возврат билета\n{item.Route ?? ti.TicketRoute} {ti.BSOLabel}\n{item.PassengerName ?? ti.PassengerName}"
                 }).ToList());

            if (model.VirtualSegCount > 0)
            {
                model.SegCountTotal = model.VirtualSegCount;
                model.FeeTotal = model.FeeRate * model.VirtualSegCount;
                model.ItemTotal = model.TotalAmount - model.FeeTotal;
            }
            else
            {
                model.ItemTotal = model.Items.Sum(i => i.Amount) + model.LuggageItems.Sum(i => i.Amount) + model.Taxes.Sum(t => t.Amount);
                model.SegCountTotal = model.Items.Sum(i => i.SegCount) + model.LuggageItems.Sum(i => i.SegCount);
                model.FeeTotal = model.Taxes.Sum(t => t.Amount);
            }

            model.ItemTotalStr = model.ItemTotal.ToString("#,0.00", nfi);
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
        public IActionResult OrganizationCorporators(string orgName, bool isMobile = false)
        {
            var model = new OrganizationFinancialAccountsViewModel
            {
                Accounts = orgName == null
                    ? (from c in _db.Counterparties
                        where c.Type.Description == "Корпоратор"
                        select new KeyValuePair<string,string>(c.ITN, c.Name)).ToList()
                    : (from o in _db.Organizations
                        join cd in _db.CorporatorDocuments on o.OrganizationId equals cd.OrganizationId
                        join c in _db.Counterparties on cd.ITN equals c.ITN
                        where o.Description == orgName
                        select new KeyValuePair<string, string>(c.ITN, c.Name)).Distinct().ToList()
            };

            if (isMobile)
            {
                return PartialView("../Mobile/CorpReceipt/OrganizationCorporators", model);
            }
            else
            {
                return PartialView(model);
            }
        }

        [HttpGet]
        public IActionResult CorporatorOrganizations(string corpName, bool isMobile = false)
        {
            var model = new OrganizationFinancialAccountsViewModel
            {
                Accounts = corpName == null
                    ? (from org in _db.Organizations
                        where org.IsActive
                        select new KeyValuePair<string, string>(org.OrganizationId.ToString(), org.Description)).ToList()
                    : (from c in _db.Counterparties
                        join cd in _db.CorporatorDocuments on c.ITN equals cd.ITN
                        join o in _db.Organizations on cd.OrganizationId equals o.OrganizationId
                        where c.Name == corpName
                        select new KeyValuePair<string, string>(o.OrganizationId.ToString(), o.Description)).Distinct().ToList()
            };

            if (isMobile)
            {
                return PartialView("../Mobile/CorpReceipt/CorporatorOrganizations", model);
            }
            else
            {
                return PartialView(model);
            }
        }
    }
}
