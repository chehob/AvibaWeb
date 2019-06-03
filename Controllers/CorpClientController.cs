﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AvibaWeb.DomainModels;
using AvibaWeb.Infrastructure;
using AvibaWeb.Models;
using AvibaWeb.ViewModels.AdminViewModels;
using AvibaWeb.ViewModels.CorpClientViewModels;
using AvibaWeb.ViewModels.CorpReceiptViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AvibaWeb.Controllers
{
    public class CorpClientController : Controller
    {
        private readonly AppIdentityDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly IViewRenderService _viewRenderService;
        private readonly IHostingEnvironment _hostingEnvironment;

        public CorpClientController(AppIdentityDbContext db, UserManager<AppUser> usrMgr,
            IViewRenderService viewRenderService, IHostingEnvironment hostingEnvironment)
        {
            _db = db;
            _userManager = usrMgr;
            _viewRenderService = viewRenderService;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User);

            var model = new IndexViewModel
            {
                IsUserLoggedInKRS =
                    _db.UserCheckIns.FirstOrDefault(
                        uci => uci.CheckInDateTime >= DateTime.Today && uci.UserId == userId) != null
            };

            return PartialView(model);
        }

        [HttpGet]
        public IActionResult Receipts()
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";
            var dateTimeLimit = new DateTime(DateTime.Now.Year - 1, 1, 1);

            var model = (from cr in _db.CorporatorReceipts.Where(cc => cc.TypeId == CorporatorReceipt.CRType.CorpClient)
                    .Include(c => c.PayeeAccount.Organization)
                from operation in _db.CorporatorReceiptOperations
                    .Where(cro =>
                        cro.OperationDateTime >= dateTimeLimit && cro.CorporatorReceiptId == cr.CorporatorReceiptId)
                    .OrderByDescending(o => o.OperationDateTime).Take(1)
                orderby cr.IssuedDateTime descending
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

            //var model = (from cr in _db.CorporatorReceipts.Where(cc => cc.TypeId == CorporatorReceipt.CRType.CorpClient)
            //        .Include(c => c.PayeeAccount.Organization)
            //    join operation in _db.CorporatorReceiptOperations.Where(cro => cro.OperationDateTime >= dateTimeLimit) on cr.CorporatorReceiptId
            //        equals
            //        operation.CorporatorReceiptId into operations
            //    from operation in operations.OrderByDescending(o => o.OperationDateTime).Take(1)
            //    orderby cr.CorporatorReceiptId descending
            //    select new ReceiptsViewModel
            //    {
            //        ReceiptNumber = cr.ReceiptNumber.ToString(),
            //        ReceiptId = cr.CorporatorReceiptId,
            //        CreatedDate = operation.OperationDateTime.ToString("d"),
            //        IssuedDateTime = cr.IssuedDateTime != null ? cr.IssuedDateTime.Value.ToString("d") : "",
            //        PaidDateTime = cr.PaidDateTime != null ? cr.PaidDateTime.Value.ToString("d") : "",
            //        PayeeOrgName = cr.PayeeAccount.Organization.Description,
            //        PayeeBankName = cr.PayeeAccount.BankName,
            //        PayerOrgName = cr.Corporator.Name,
            //        TotalStr = cr.Amount.Value.ToString("#,0.00", nfi),
            //        Status = cr.StatusId
            //    }).ToList();

            return PartialView(model);
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
                                              where item.CorporatorReceiptId == cr.CorporatorReceiptId && ti.ReceiptItemTypeId == item.TypeId
                                              select new TicketListViewModel
                                              {
                                                  TicketOperationId = item.TicketOperationId.Value,
                                                  TicketNumber = ti.BSONumber,
                                                  Route = item.Route ?? ti.Route,
                                                  ExecutionDateTime = ti.ExecutionDateTime,
                                                  Payment = item.Amount.ToString("#,0.00", nfi),
                                                  FeeRate = item.FeeRate.ToString("#,0.00", nfi),
                                                  OperationType = ti.OperationTypeID,
                                                  PassengerName = item.PassengerName ?? ti.PassengerName,
                                                  SegCount = ti.SegCount,
                                                  PerSegment = item.PerSegment,
                                                  IsPercent = item.IsPercent,
                                                  TypeId = item.TypeId
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
            Counterparty corporator;
            CorporatorReceipt receipt;
            if (model.ReceiptId != null && model.ReceiptId != 0)
            {
                receipt = _db.CorporatorReceipts.Include(cr => cr.Items)
                    .FirstOrDefault(cr => cr.CorporatorReceiptId == model.ReceiptId);
                receipt.CorporatorId = (from c in _db.Counterparties
                                        where c.Name == model.PayerName
                                        select c.ITN).FirstOrDefault();

                corporator = _db.Counterparties.Include(c => c.CorporatorAccount).FirstOrDefault(c => c.ITN == receipt.CorporatorId);
                if (receipt.Items.Count > 0)
                {
                    corporator.CorporatorAccount.Balance += receipt.Amount.Value;
                }                

                receipt.PayeeAccount = (from fa in _db.FinancialAccounts
                                        join o in _db.Organizations on fa.OrganizationId equals o.OrganizationId
                                        where o.Description == model.PayeeName && fa.BankName == model.PayeeBankName
                                        select fa).FirstOrDefault();
                receipt.FeeRate = string.IsNullOrEmpty(model.FeeRate) ? 0 : decimal.Parse(model.FeeRate);                
                receipt.Amount = 0;
                receipt.StatusId = CorporatorReceipt.CRPaymentStatus.Unpaid;
                receipt.TypeId = CorporatorReceipt.CRType.CorpClient;

                if (model.IssuedDateTime != null)
                {
                    receipt.IssuedDateTime = DateTime.Parse(model.IssuedDateTime);
                }
                else
                {
                    receipt.IssuedDateTime = DateTime.Now;
                }

                var corpClient = _db.Counterparties.Include(c => c.CorporatorAccount).FirstOrDefault(c => c.ITN == receipt.CorporatorId);
                corpClient.CorporatorAccount.LastReceiptDate = DateTime.Now;

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
                            TicketOperationId = int.Parse(i.TicketOperationId),
                            Amount = i.Amount,
                            PassengerName = i.PassengerName,
                            Route = i.Route,
                            FeeRate = i.FeeRate,
                            TypeId = i.TypeId,
                            PerSegment = i.PerSegment,
                            IsPercent = i.IsPercent
                        };

                        var itemFee = (item.IsPercent ? 
                            item.Amount * item.FeeRate / 100 : 
                            (item.PerSegment ? 
                                i.SegCount * item.FeeRate : 
                                item.FeeRate));
                        receipt.Amount += item.Amount + itemFee;

                        receipt.Items.Add(item);
                        viewModelItems.Add(item);

                        var ticketOperationId = new SqlParameter("@TicketOperationId",
                            int.Parse(i.TicketOperationId));
                        _db.Database.ExecuteSqlCommand(
                            @"update pay
	                        set	pay.PaymentType = 'ПП'
	                        from BookingDB.dbo.Payments pay
	                        join BookingDB.dbo.TicketOperations tio on pay.TicketID = tio.TicketID
	                        where tio.ID = @TicketOperationId and pay.PaymentType <> 'ПП'

                            update k
	                        set	k.IsCanceled = 1, k.DateCanceled = getdate()
	                        from BookingDB.dbo.KRSs k
	                        join BookingDB.dbo.TicketOperations tio on k.TicketID = tio.TicketID
	                        where tio.ID = @TicketOperationId and k.IsCanceled <> 1 and
                            ((tio.OperationTypeID = 1 and k.OperationTypeID = 1) or (tio.OperationTypeID in (2,6) and k.OperationTypeID = 2))

                            update kt
	                        set	kt.IsCanceled = 1
	                        from BookingDB.dbo.KRSTaxes kt
                            join BookingDB.dbo.KRSs k on kt.KRSID = k.ID
	                        join BookingDB.dbo.TicketOperations tio on k.TicketID = tio.TicketID
	                        where tio.ID = @TicketOperationId and
                            ((tio.OperationTypeID = 1 and k.OperationTypeID = 1) or (tio.OperationTypeID in (2,6) and k.OperationTypeID = 2))",
                            ticketOperationId);
                    }

                    if (model.Items.Count == 0)
                    {
                        receipt.Amount = model.ReceiptTotal;
                    }
                }
                else
                {
                    receipt.Amount = model.ReceiptTotal;
                }

                var itemsToRemove = receiptOldItems.Except(viewModelItems);
                foreach (var i in itemsToRemove)
                {
                    receipt.Items.Remove(i);
                    _db.Entry(i).State = EntityState.Deleted;
                }

                if (model.Items.Count > 0)
                {
                    corporator.CorporatorAccount.Balance -= receipt.Amount.Value;
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
                    FeeRate = string.IsNullOrEmpty(model.FeeRate) ? 0 : decimal.Parse(model.FeeRate),
                    Amount = 0,
                    StatusId = CorporatorReceipt.CRPaymentStatus.Unpaid,
                    TypeId = CorporatorReceipt.CRType.CorpClient
                };

                if (model.IssuedDateTime != null)
                {
                    receipt.IssuedDateTime = DateTime.Parse(model.IssuedDateTime);
                }
                else
                {
                    receipt.IssuedDateTime = DateTime.Now;
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
                        FeeRate = item.FeeRate,
                        TypeId = item.TypeId,
                        PerSegment = item.PerSegment,
                        IsPercent = item.IsPercent
                    };
                    _db.CorporatorReceiptItems.Add(receiptItem);

                    var itemFee = (receiptItem.IsPercent ?
                            receiptItem.Amount * receiptItem.FeeRate / 100 :
                            (receiptItem.PerSegment ?
                                item.SegCount * receiptItem.FeeRate :
                                receiptItem.FeeRate));
                    receipt.Amount += receiptItem.Amount + itemFee;

                    var ticketOperationId = new SqlParameter("@TicketOperationId",
                        int.Parse(item.TicketOperationId));
                    _db.Database.ExecuteSqlCommand(
                        @"update pay
	                    set	pay.PaymentType = 'ПП'
	                    from BookingDB.dbo.Payments pay
	                    join BookingDB.dbo.TicketOperations tio on pay.TicketID = tio.TicketID
	                    where tio.ID = @TicketOperationId and pay.PaymentType <> 'ПП'

                        update k
	                    set	k.IsCanceled = 1, k.DateCanceled = getdate()
	                    from BookingDB.dbo.KRSs k
	                    join BookingDB.dbo.TicketOperations tio on k.TicketID = tio.TicketID
	                    where tio.ID = @TicketOperationId and k.IsCanceled <> 1 and
                        ((tio.OperationTypeID = 1 and k.OperationTypeID = 1) or (tio.OperationTypeID in (2,6) and k.OperationTypeID = 2))

                        update kt
	                    set	kt.IsCanceled = 1
	                    from BookingDB.dbo.KRSTaxes kt
                        join BookingDB.dbo.KRSs k on kt.KRSID = k.ID
	                    join BookingDB.dbo.TicketOperations tio on k.TicketID = tio.TicketID
	                    where tio.ID = @TicketOperationId and
                        ((tio.OperationTypeID = 1 and k.OperationTypeID = 1) or (tio.OperationTypeID in (2,6) and k.OperationTypeID = 2))",
                        ticketOperationId);
                }

                if (model.Items.Count == 0)
                {
                    receipt.Amount = model.ReceiptTotal;
                }

                var operation = new CorporatorReceiptOperation
                {
                    Receipt = receipt,
                    OperationDateTime = DateTime.Now,
                    OperationTypeId = CorporatorReceiptOperation.CROType.New
                };

                _db.CorporatorReceiptOperations.Add(operation);

                corporator = _db.Counterparties.Include(c => c.CorporatorAccount).FirstOrDefault(c => c.ITN == receipt.CorporatorId);

                if (model.Items.Count > 0)
                {
                    corporator.CorporatorAccount.Balance -= receipt.Amount.Value;
                }
            }

            if (corporator != null)
            {
                corporator.CorporatorAccount.LastReceiptDate = DateTime.Now;
                if (corporator.CorporatorAccount.Balance >= 0 || receipt.Amount < 0)
                {
                    receipt.StatusId = CorporatorReceipt.CRPaymentStatus.Paid;
                    receipt.PaidAmount = receipt.Amount;
                    receipt.PaidDateTime = DateTime.Now;
                }
                else
                {
                    receipt.StatusId = CorporatorReceipt.CRPaymentStatus.Unpaid;
                    receipt.PaidAmount = null;
                    receipt.PaidDateTime = null;
                }
            }            

            await _db.SaveChangesAsync();

            return Json(new { message = "Ok" });
        }

        [HttpPost]
        public async Task<IActionResult> TicketList([FromBody]TicketListRequest request)
        {
            var userId = _userManager.GetUserId(User);
            var isUserAdmin = User.IsInRole("Administrators");

            var deskIdList = _db.UserCheckIns.Where(uci => uci.CheckInDateTime >= DateTime.Today && uci.UserId == userId).ToList();

            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            //var exceptOperationsId = request.ExceptItems.Select(x => int.Parse(x.TicketOperationId)).ToArray();
            var model = (from tio in _db.VTicketOperations
                         where tio.ExecutionDateTime >= DateTime.Parse(request.fromDate) && tio.ExecutionDateTime < DateTime.Parse(request.toDate).AddDays(1) &&
                               //!exceptOperationsId.Contains(tio.TicketOperationId) &&
                               !_db.CorporatorReceiptItems.Any(i => i.TicketOperationId == tio.TicketOperationId)
                               && (isUserAdmin || deskIdList.Any( d => d.DeskId == tio.DeskId))
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
                             SegCount = tio.SegCount,
                             OperationTypeId = (int)tio.OperationTypeID,
                             TicketTypeId = tio.TicketTypeId ?? 1,
                             TypeId = tio.ReceiptItemTypeId
                         }).ToList();

            return Json(new { message = await _viewRenderService.RenderToStringAsync("CorpClient/TicketList", model) });
        }

        [HttpGet]
        public async Task<IActionResult> CorpFeeList(string PayerName, string PayeeName)
        {
            var feeGroups = from cfr in _db.CorporatorFeeRates
                join o in _db.Organizations on cfr.OrganizationId equals o.OrganizationId
                join c in _db.Counterparties on cfr.ITN equals c.ITN
                where o.Description == PayeeName && c.Name == PayerName
                group cfr by new {cfr.OperationTypeId, cfr.TicketTypeId}
                into groups
                select groups.OrderByDescending(p => p.StartDate).First();

            var model = (from fg in feeGroups
                select new CorpFeeListViewModel
                {
                    TicketType = (CorpFeeListViewModel.CFTicketType)fg.TicketTypeId,
                    OperationType = (CorpFeeListViewModel.CFOpType)fg.OperationTypeId,
                    TicketTypeId = fg.TicketTypeId,
                    OperationTypeId = fg.OperationTypeId,
                    Rate = fg.Rate,
                    PerSegment = fg.PerSegment,
                    IsPercent = fg.IsPercent
                }).ToList();

            return Json(new { message = await _viewRenderService.RenderToStringAsync("CorpClient/CorpFeeList", model) });
        }

        [HttpPost]
        public async Task<IActionResult> ReceiptList([FromBody]TicketListRequest request)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";
            //var dateTimeLimit = new DateTime(DateTime.Now.Year - 1, 1, 1);

            var model = (from cr in _db.CorporatorReceipts.Where(cc => cc.TypeId == CorporatorReceipt.CRType.CorpClient)
                    .Include(c => c.PayeeAccount.Organization)
                from operation in _db.CorporatorReceiptOperations
                    .Where(cro =>
                        cro.OperationDateTime >= DateTime.Parse(request.fromDate) &&
                        cro.OperationDateTime < DateTime.Parse(request.toDate).AddDays(1) &&
                        cro.CorporatorReceiptId == cr.CorporatorReceiptId)
                    .OrderByDescending(o => o.OperationDateTime).Take(1)
                orderby cr.IssuedDateTime descending
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
                    Status = cr.StatusId,
                    TicketsToPDFCount = (from cri in _db.CorporatorReceiptItems
                        join rti in _db.VTicketOperations on cri.TicketOperationId equals rti.TicketOperationId
                        where cr.CorporatorReceiptId == cri.CorporatorReceiptId &&
                              (rti.TicketTypeId == null || rti.TicketTypeId == 1)
                        select 1).Sum(i => i)
                }).ToList();

            return Json(new { message = await _viewRenderService.RenderToStringAsync("CorpClient/ReceiptList", model) });
        }

        [HttpGet]
        public IActionResult Corporators()
        {
            return PartialView(_db.Counterparties.Where(c => c.Type.Description == "Корпоратор").OrderBy(c => c.Name));
        }

        [HttpGet]
        public IActionResult EditCorporator(string id)
        {
            var counterparty = _db.Counterparties.FirstOrDefault(c => c.ITN == id);
            if (counterparty == null) return RedirectToAction("Corporators");

            var model = new CounterpartyEditModel(counterparty)
            {
                Types = from t in _db.CounterpartyTypes.Where(t => t.IsActive).OrderBy(t => t.Description)
                    select new SelectListItem
                    {
                        Value = t.CounterpartyTypeId.ToString(),
                        Text = t.Description
                    }
            };
            return PartialView(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditCorporator(CounterpartyEditModel model)
        {
            if (!ModelState.IsValid) return PartialView(model);
            var counterparty = _db.Counterparties.FirstOrDefault(c => c.ITN == model.Item.ITN);
            if (counterparty != null)
            {
                counterparty.Name = model.Item.Name;
                counterparty.CorrespondentAccount = model.Item.CorrespondentAccount;
                counterparty.KPP = model.Item.KPP;
                counterparty.BIK = model.Item.BIK;
                counterparty.OGRN = model.Item.OGRN;
                counterparty.Phone = model.Item.Phone;
                counterparty.Address = model.Item.Address;
                counterparty.Email = model.Item.Email;
                counterparty.BankName = model.Item.BankName;
                counterparty.BankAccount = model.Item.BankAccount;
                counterparty.ManagementName = model.Item.ManagementName;
                counterparty.ManagementPosition = model.Item.ManagementPosition;
                counterparty.TypeId = model.Item.TypeId;

                await _db.SaveChangesAsync();

                return RedirectToAction("Corporators");
            }

            ModelState.AddModelError("", "Корпоратор не найден");

            return PartialView(model);
        }

        [HttpGet]
        public IActionResult CorporatorDocuments(string id)
        {
            var model = new ViewModels.CorpClientViewModels.CorporatorDocumentsViewModel
            {
                ITN = id,
                Documents = (from cd in _db.CorporatorDocuments
                    join c in _db.Counterparties on cd.ITN equals c.ITN
                    where c.ITN == id
                    select new ViewModels.CorpClientViewModels.DocumentListViewModel
                    {
                        CorporatorDocumentId = cd.CorporatorDocumentId,
                        Organization = cd.Organization.Description,
                        Document = cd.Document,
                        Date = cd.Date
                    }).ToList()
            };

            return PartialView(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditCorporatorDocument(string corpITN, int? id)
        {
            var model = new ViewModels.CorpClientViewModels.CreateDocumentViewModel
            {
                ITN = corpITN,
                Organizations = (from org in _db.Organizations
                                 where org.IsActive
                                 select org.Description).ToList()
            };

            if (id != null && id != 0)
            {
                var doc = (from cd in _db.CorporatorDocuments.Include(c => c.Corporator).Include(c => c.Organization)
                           where cd.CorporatorDocumentId == id
                           select cd).FirstOrDefault();

                model.ITN = doc.ITN;
                model.Document = new DocumentEditData
                {
                    DocumentId = doc.CorporatorDocumentId,
                    OrganizationName = doc.Organization.Description,
                    CorporatorName = doc.Corporator.Name,
                    Date = doc.Date.ToString("d"),
                    Doc = doc.Document
                };
            }
            else
            {
                var counterparty = _db.Counterparties.FirstOrDefault(c => c.ITN == corpITN);

                model.Document = new DocumentEditData
                {
                    CorporatorName = counterparty.Name
                };
            }

            return Json(new { message = await _viewRenderService.RenderToStringAsync("CorpClient/EditCorporatorDocument", model) });
        }

        [HttpPost]
        public async Task<IActionResult> EditCorporatorDocument([FromBody]ViewModels.CorpClientViewModels.CreateDocumentPostViewModel model)
        {
            CorporatorDocument document;
            if (model.DocumentId != null && model.DocumentId != 0)
            {
                document = _db.CorporatorDocuments.FirstOrDefault(cr => cr.CorporatorDocumentId == model.DocumentId);
                document.ITN = model.ITN;
                document.Organization = (from o in _db.Organizations
                                         where o.Description == model.OrgName
                                         select o).FirstOrDefault();
                document.Document = model.Document;
                if (model.IssuedDateTime != null)
                {
                    document.Date = System.DateTime.Parse(model.IssuedDateTime);
                }
            }
            else
            {
                document = new CorporatorDocument
                {
                    ITN = model.ITN,
                    Organization = (from o in _db.Organizations
                                    where o.Description == model.OrgName
                                    select o).FirstOrDefault(),
                    Document = model.Document,
                    Date = System.DateTime.Parse(model.IssuedDateTime)
                };

                _db.CorporatorDocuments.Add(document);
            }

            await _db.SaveChangesAsync();

            return Json(new { message = "Ok" });
        }

        [HttpGet]
        public async Task<IActionResult> EditCorporatorDocumentTaxes(int id)
        {
            var model = new ViewModels.CorpClientViewModels.EditDocumentTaxesViewModel();

            var doc = (from cd in _db.CorporatorDocuments.Include(c => c.Corporator).Include(c => c.Organization)
                       where cd.CorporatorDocumentId == id
                       select cd).FirstOrDefault();

            if (doc == null)
            {

            }
            else
            {
                model.OrganizationId = doc.OrganizationId;
                model.CorporatorId = doc.ITN;

                var feeGroups = from cfr in _db.CorporatorFeeRates
                                join o in _db.Organizations on cfr.OrganizationId equals o.OrganizationId
                                join c in _db.Counterparties on cfr.ITN equals c.ITN
                                where cfr.ITN == doc.ITN && cfr.OrganizationId == doc.OrganizationId
                                group cfr by new { cfr.OperationTypeId, cfr.TicketTypeId }
                    into groups
                                select groups.OrderByDescending(p => p.StartDate).First();

                var feeRates = new List<CorpFeeListViewModel>();

                var item = new CorpFeeListViewModel
                {
                    TicketType = CorpFeeListViewModel.CFTicketType.Avia,
                    TicketTypeId = (int)CorpFeeListViewModel.CFTicketType.Avia,
                    OperationType = CorpFeeListViewModel.CFOpType.Sale,
                    OperationTypeId = (int)CorpFeeListViewModel.CFOpType.Sale,
                    Rate = 0,
                    PerSegment = true,
                    IsPercent = false
                };
                feeRates.Add(item);

                item = new CorpFeeListViewModel
                {
                    TicketType = CorpFeeListViewModel.CFTicketType.Avia,
                    TicketTypeId = (int)CorpFeeListViewModel.CFTicketType.Avia,
                    OperationType = CorpFeeListViewModel.CFOpType.Refund,
                    OperationTypeId = (int)CorpFeeListViewModel.CFOpType.Refund,
                    Rate = 0,
                    PerSegment = true,
                    IsPercent = false
                };
                feeRates.Add(item);

                item = new CorpFeeListViewModel
                {
                    TicketType = CorpFeeListViewModel.CFTicketType.Avia,
                    TicketTypeId = (int)CorpFeeListViewModel.CFTicketType.Avia,
                    OperationType = CorpFeeListViewModel.CFOpType.Exchange,
                    OperationTypeId = (int)CorpFeeListViewModel.CFOpType.Exchange,
                    Rate = 0,
                    PerSegment = true,
                    IsPercent = false
                };
                feeRates.Add(item);

                item = new CorpFeeListViewModel
                {
                    TicketType = CorpFeeListViewModel.CFTicketType.Rail,
                    TicketTypeId = (int)CorpFeeListViewModel.CFTicketType.Rail,
                    OperationType = CorpFeeListViewModel.CFOpType.Sale,
                    OperationTypeId = (int)CorpFeeListViewModel.CFOpType.Sale,
                    Rate = 0,
                    PerSegment = true,
                    IsPercent = false
                };
                feeRates.Add(item);

                item = new CorpFeeListViewModel
                {
                    TicketType = CorpFeeListViewModel.CFTicketType.Rail,
                    TicketTypeId = (int)CorpFeeListViewModel.CFTicketType.Rail,
                    OperationType = CorpFeeListViewModel.CFOpType.Refund,
                    OperationTypeId = (int)CorpFeeListViewModel.CFOpType.Refund,
                    Rate = 0,
                    PerSegment = true,
                    IsPercent = false
                };
                feeRates.Add(item);

                item = new CorpFeeListViewModel
                {
                    TicketType = CorpFeeListViewModel.CFTicketType.Rail,
                    TicketTypeId = (int)CorpFeeListViewModel.CFTicketType.Rail,
                    OperationType = CorpFeeListViewModel.CFOpType.Exchange,
                    OperationTypeId = (int)CorpFeeListViewModel.CFOpType.Exchange,
                    Rate = 0,
                    PerSegment = true,
                    IsPercent = false
                };
                feeRates.Add(item);

                item = new CorpFeeListViewModel
                {
                    TicketType = CorpFeeListViewModel.CFTicketType.EMD,
                    TicketTypeId = (int)CorpFeeListViewModel.CFTicketType.EMD,
                    OperationType = CorpFeeListViewModel.CFOpType.Sale,
                    OperationTypeId = (int)CorpFeeListViewModel.CFOpType.Sale,
                    Rate = 0,
                    PerSegment = true,
                    IsPercent = true
                };
                feeRates.Add(item);

                model.FeeRates = feeRates;

                if (feeGroups.Any())
                {
                    var dbFeeRates = (from fg in feeGroups.OrderBy(g => g.TicketTypeId).ThenBy(g => g.OperationTypeId)
                                      select new CorpFeeListViewModel
                                      {
                                          TicketType = (CorpFeeListViewModel.CFTicketType)fg.TicketTypeId,
                                          OperationType = (CorpFeeListViewModel.CFOpType)fg.OperationTypeId,
                                          TicketTypeId = fg.TicketTypeId,
                                          OperationTypeId = fg.OperationTypeId,
                                          Rate = fg.Rate,
                                          PerSegment = fg.PerSegment,
                                          IsPercent = fg.IsPercent
                                      }).ToList();

                    model.FeeRates.ForEach(fr => {
                        var dbFeeRate = dbFeeRates.FirstOrDefault(dfr => dfr.TicketType == fr.TicketType && dfr.OperationType == fr.OperationType);
                        if (dbFeeRate == null) return;
                        fr.Rate = dbFeeRate.Rate;
                        fr.PerSegment = dbFeeRate.PerSegment;
                        fr.IsPercent = dbFeeRate.IsPercent;
                    });
                }
            }

            return Json(new { message = await _viewRenderService.RenderToStringAsync("CorpClient/EditCorporatorDocumentTaxes", model) });
        }

        [HttpPost]
        public async Task<IActionResult> EditCorporatorDocumentTaxes([FromBody]ViewModels.CorpClientViewModels.EditDocumentTaxesViewModel model)
        {
            var feeGroups = (from cfr in _db.CorporatorFeeRates
                             join o in _db.Organizations on cfr.OrganizationId equals o.OrganizationId
                             join c in _db.Counterparties on cfr.ITN equals c.ITN
                             where cfr.ITN == model.CorporatorId && cfr.OrganizationId == model.OrganizationId
                             group cfr by new { cfr.OperationTypeId, cfr.TicketTypeId }
                into groups
                             select groups.OrderByDescending(p => p.StartDate).First()).ToList();

            foreach (var feeRate in model.FeeRates)
            {
                var dbFeeRate = feeGroups.FirstOrDefault(cfr =>
                    cfr.OperationTypeId == feeRate.OperationTypeId && cfr.TicketTypeId == feeRate.TicketTypeId);

                if (dbFeeRate != null && dbFeeRate.Rate == feeRate.Rate && dbFeeRate.PerSegment == feeRate.PerSegment &&
                    dbFeeRate.IsPercent == feeRate.IsPercent)
                {
                    continue;
                }

                var rate = new CorporatorFeeRate
                {
                    OrganizationId = model.OrganizationId,
                    ITN = model.CorporatorId,
                    OperationTypeId = feeRate.OperationTypeId,
                    TicketTypeId = feeRate.TicketTypeId,
                    Rate = feeRate.Rate,
                    PerSegment = feeRate.PerSegment,
                    StartDate = System.DateTime.Now,
                    IsPercent = feeRate.IsPercent
                };
                _db.CorporatorFeeRates.Add(rate);
            }

            await _db.SaveChangesAsync();

            return Json(new { message = "Ok" });
        }

        [HttpGet]
        public IActionResult CorporatorAccounts(string id)
        {
            var model = new CorporatorAccountsViewModel
            {
                ITN = id,
                Accounts = (from ca in _db.CorporatorAccounts
                    where ca.ITN == id
                    select ca).ToList()
            };

            return PartialView(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditCorporatorAccount(string corpITN, int? id)
        {
            var model = new CreateAccountViewModel
            {
                ITN = corpITN
            };

            if (id != null && id != 0)
            {
                var account = (from ca in _db.CorporatorAccounts
                    where ca.CorporatorAccountId == id
                    select ca).FirstOrDefault();

                model.ITN = account.ITN;
                model.Account = account;
            }
            else
            {
                model.Account = new CorporatorAccount();
            }

            return Json(new { message = await _viewRenderService.RenderToStringAsync("CorpClient/EditCorporatorAccount", model) });
        }

        [HttpPost]
        public async Task<IActionResult> EditCorporatorAccount([FromBody]CreateAccountViewModel model)
        {
            if (model.Account.CorporatorAccountId != 0)
            {
                var dbAccount = _db.CorporatorAccounts.FirstOrDefault(ca => ca.CorporatorAccountId == model.Account.CorporatorAccountId);
                dbAccount.ITN = model.ITN;
                dbAccount.Description = model.Account.Description;
                dbAccount.BankName = model.Account.BankName;
                dbAccount.OffBankName = model.Account.OffBankName;
                dbAccount.BIK = model.Account.BIK;
                dbAccount.CorrespondentAccount = model.Account.CorrespondentAccount;
                dbAccount.IsActive = model.Account.IsActive;
            }
            else
            {
                var account = new CorporatorAccount
                {
                    ITN = model.ITN,
                    Description = model.Account.Description,
                    BankName = model.Account.BankName,
                    OffBankName = model.Account.OffBankName,
                    BIK = model.Account.BIK,
                    CorrespondentAccount = model.Account.CorrespondentAccount,
                    IsActive = model.Account.IsActive
            };

                _db.CorporatorAccounts.Add(account);
            }

            await _db.SaveChangesAsync();

            return Json(new { message = "Ok" });
        }

        [HttpGet]
        public IActionResult ReviseReport()
        {
            var model = new ReviseReportViewModel
            {
                Counterparties = (from c in _db.Counterparties
                    where c.Type.Description == "Корпоратор"
                    select c.Name).ToList(),
                Organizations = (from org in _db.Organizations
                    where org.IsActive
                    select org.Description).ToList()
            };

            return PartialView(model);
        }

        [HttpPost]
        public IActionResult ReviseReport([FromBody]ReviseReportRequest requestData)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var oldBalance = (from cr in _db.CorporatorReceipts
                    .Include(c => c.PayeeAccount.Organization).ThenInclude(o => o.Counterparty)
                let cro = _db.CorporatorReceiptOperations.Where(cro =>
                        cro.OperationDateTime < DateTime.Parse(requestData.fromDate) &&
                        cr.CorporatorReceiptId == cro.CorporatorReceiptId)
                    .OrderByDescending(o => o.OperationDateTime)
                    .FirstOrDefault()
                where cr.PayeeAccount.Organization.Description == requestData.payeeName &&
                      cr.PayeeAccount.BankName == requestData.payeeBankName &&
                      cr.Corporator.Name == requestData.payerName &&
                      cr.PaidAmount > 0 && cr.PaidAmount != null &&
                      cr.TypeId == CorporatorReceipt.CRType.CorpClient &&
                      cro != null
                select cr.PaidAmount.Value).Sum();

            var model = new ReviseReportPDFViewModel
            {
                FromDate = DateTime.Parse(requestData.fromDate).ToString("d"),
                ToDate = DateTime.Parse(requestData.toDate).ToString("d"),
                OrgName = requestData.payeeName,
                PayerName = requestData.payerName,
                OldDebit = oldBalance >= 0 ? oldBalance.ToString("#,0.00", nfi) : "",
                OldCredit = oldBalance < 0 ? oldBalance.ToString("#,0.00", nfi) : "",
                Items = (from item in _db.CorporatorReceiptItems
                    join ti in _db.VReceiptTicketInfo on item.TicketOperationId equals ti.TicketOperationId
                    join cr in _db.CorporatorReceipts.Include(c => c.PayeeAccount.Organization) on item
                        .CorporatorReceiptId equals cr.CorporatorReceiptId
                    let cro = _db.CorporatorReceiptOperations.Where(cro =>
                            cro.OperationDateTime >= DateTime.Parse(requestData.fromDate) &&
                            cro.OperationDateTime < DateTime.Parse(requestData.toDate).AddDays(1) &&
                            cr.CorporatorReceiptId == cro.CorporatorReceiptId)
                        .OrderByDescending(o => o.OperationDateTime)
                        .FirstOrDefault()
                    where cr.PayeeAccount.Organization.Description == requestData.payeeName &&
                          cr.PayeeAccount.BankName == requestData.payeeBankName &&
                          cr.Corporator.Name == requestData.payerName &&
                          cro != null &&
                          cr.TypeId == CorporatorReceipt.CRType.CorpClient
                    select new ReviseReportPDFItem
                    {
                        Date = cro.OperationDateTime.ToString("d"),
                        ReceiptNumber = cr.ReceiptNumber.ToString(),
                        Amount = "0"
                    }).ToList()
            };

            return Json(model);
        }

        [HttpPost]
        public IActionResult TicketPDFData(int id)
        {
            var enUS = new CultureInfo("en-US");
            var ruRU = new CultureInfo("ru-RU");

            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();

            var model = new TicketPDFViewModel
            {
                Tickets = (from cr in _db.CorporatorReceipts
                    join cri in _db.CorporatorReceiptItems on cr.CorporatorReceiptId equals cri.CorporatorReceiptId
                    join tinfo in _db.VTicketPDFInfo on cri.TicketOperationId equals tinfo.TicketOperationID
                    where cr.CorporatorReceiptId == id && (tinfo.TypeID == null || tinfo.TypeID == 1 || tinfo.TypeID == 2)
                    select new TicketPDFData
                    {
                        TicketId = tinfo.TicketID,
                        TicketNumber = tinfo.AirlineCode + " " + tinfo.BSONumber,
                        PassengerName = (tinfo.LastName + " " + tinfo.FirstName).ToUpper(),
                        DocType = tinfo.DocumentType,
                        Doc = tinfo.Passport,
                        IssuedBy = tinfo.AirlineName,
                        pnr = tinfo.PNRID,
                        DateOfIssue = tinfo.DealDateTime.ToString("dd MMMM yyyy", ruRU).ToUpper(),
                        Stamp = tinfo.Stamp + " " + (tinfo.Number == null ? "0000" : tinfo.Number.ToString().PadLeft(4, '0')) + " 0",
                        Seg = new List<SegmentPDFData>(),
                        Qr = tinfo.LastName + "/" + tinfo.FirstName + " " + tinfo.DealDateTime.ToString("ddMMMyyyy", enUS).ToUpper() + " " + tinfo.AirlineCode + " " + tinfo.BSONumber,
                        FareCalc = "ТАРИФ / FARE " + tinfo.Fare.ToString("#.00", nfi) + "РУБ ",
                        Endorsements = tinfo.Endorsements,
                        Payment = "ПП",
                        Fare = tinfo.Fare,
                        IsInfant = tinfo.AgeDiff < 2,
                        IsExchange = tinfo.IsExchange == 1,
                        AddFee = tinfo.AddFee.Value,
                        ZZFee = tinfo.ZZFee.Value,
                        ExTicketNumber = tinfo.ExBSONumber
                    }).ToList()
            };

            var fullTerminal = "(Терминал ";
            var shortTerminal = "(Терм ";

            foreach (var ticket in model.Tickets)
            {
                var segData = (from si in _db.VTicketSegmentPDFInfo
                    where si.TicketID == ticket.TicketId
                    select si).ToList();

                // 1
                if (segData.Count >= 2)
                {
                    var segPDFData = new SegmentPDFData
                    {
                        Origin = segData[0].OriginName + " " + (segData[0].OriginAirportName ?? ""),
                        Destination = segData[0].DestinationName + " " + (segData[0].DestinationAirportName ?? ""),
                        Flight = segData[0].AirlineName + " " + segData[0].FlightNumber,
                        Departure = segData[0].FlightDate.ToString("dd MMM hh:mm", ruRU),
                        Arrival = segData[0].ArrivalDate.ToString("dd MMM hh:mm", ruRU),
                    };

                    var termStr = "";
                    if (segData[0].Term1 != null && segData[0].Term1.Length > 1)
                    {
                        termStr = segPDFData.Origin.Length > 21 ? shortTerminal : fullTerminal;
                        termStr += segData[0].Term1 + ")";
                    }

                    segPDFData.Origin += termStr;

                    termStr = "";
                    if (segData[0].Term2 != null && segData[0].Term2.Length > 1)
                    {
                        termStr = segPDFData.Destination.Length > 21 ? shortTerminal : fullTerminal;
                        termStr += segData[0].Term2 + ")";
                    }

                    segPDFData.Destination += termStr;

                    ticket.Seg.Add(segPDFData);
                    ticket.Qr += " " + segData[0].QRSeg + " " + segData[0].AirlineName + "-" + segData[0].FlightNumber;
                }
                else
                {
                    ticket.Seg.Add(new SegmentPDFData
                    {

                    });
                }

                // 2
                if (segData.Count == 1)
                {
                    var segPDFData = new SegmentPDFData
                    {
                        Origin = segData[0].OriginName + " " + (segData[0].OriginAirportName ?? ""),
                        Destination = segData[0].DestinationName + " " + (segData[0].DestinationAirportName ?? ""),
                        Flight = segData[0].AirlineName + " " + segData[0].FlightNumber,
                        Departure = segData[0].FlightDate.ToString("dd MMM hh:mm", ruRU),
                        Arrival = segData[0].ArrivalDate.ToString("dd MMM hh:mm", ruRU),
                    };

                    var termStr = "";
                    if (segData[0].Term1 != null && segData[0].Term1.Length > 1)
                    {
                        termStr = segPDFData.Origin.Length > 21 ? shortTerminal : fullTerminal;
                        termStr += segData[0].Term1 + ")";
                    }

                    segPDFData.Origin += termStr;

                    termStr = "";
                    if (segData[0].Term2 != null && segData[0].Term2.Length > 1)
                    {
                        termStr = segPDFData.Destination.Length > 21 ? shortTerminal : fullTerminal;
                        termStr += segData[0].Term2 + ")";
                    }

                    segPDFData.Destination += termStr;

                    ticket.Seg.Add(segPDFData);
                    ticket.Qr += " " + segData[0].QRSeg + " " + segData[0].AirlineName + "-" + segData[0].FlightNumber;
                }
                else if (segData.Count > 2)
                {
                    var segPDFData = new SegmentPDFData
                    {
                        Origin = segData[1].OriginName + " " + (segData[1].OriginAirportName ?? ""),
                        Destination = segData[1].DestinationName + " " + (segData[1].DestinationAirportName ?? ""),
                        Flight = segData[1].AirlineName + " " + segData[1].FlightNumber,
                        Departure = segData[1].FlightDate.ToString("dd MMM hh:mm", ruRU),
                        Arrival = segData[1].ArrivalDate.ToString("dd MMM hh:mm", ruRU),
                    };

                    var termStr = "";
                    if (segData[1].Term1 != null && segData[1].Term1.Length > 1)
                    {
                        termStr = segPDFData.Origin.Length > 21 ? shortTerminal : fullTerminal;
                        termStr += segData[1].Term1 + ")";
                    }

                    segPDFData.Origin += termStr;

                    termStr = "";
                    if (segData[1].Term2 != null && segData[1].Term2.Length > 1)
                    {
                        termStr = segPDFData.Destination.Length > 21 ? shortTerminal : fullTerminal;
                        termStr += segData[1].Term2 + ")";
                    }

                    segPDFData.Destination += termStr;

                    ticket.Seg.Add(segPDFData);
                    ticket.Qr += " " + segData[1].QRSeg + " " + segData[1].AirlineName + "-" + segData[1].FlightNumber;
                }
                else
                {
                    ticket.Seg.Add(new SegmentPDFData
                    {

                    });
                }

                // 3
                if (segData.Count == 2)
                {
                    var segPDFData = new SegmentPDFData
                    {
                        Origin = segData[1].OriginName + " " + (segData[1].OriginAirportName ?? ""),
                        Destination = segData[1].DestinationName + " " + (segData[1].DestinationAirportName ?? ""),
                        Flight = segData[1].AirlineName + " " + segData[1].FlightNumber,
                        Departure = segData[1].FlightDate.ToString("dd MMM hh:mm", ruRU),
                        Arrival = segData[1].ArrivalDate.ToString("dd MMM hh:mm", ruRU),
                    };

                    var termStr = "";
                    if (segData[1].Term1 != null && segData[1].Term1.Length > 1)
                    {
                        termStr = segPDFData.Origin.Length > 21 ? shortTerminal : fullTerminal;
                        termStr += segData[1].Term1 + ")";
                    }

                    segPDFData.Origin += termStr;

                    termStr = "";
                    if (segData[1].Term2 != null && segData[1].Term2.Length > 1)
                    {
                        termStr = segPDFData.Destination.Length > 21 ? shortTerminal : fullTerminal;
                        termStr += segData[1].Term2 + ")";
                    }

                    segPDFData.Destination += termStr;

                    ticket.Seg.Add(segPDFData);
                    ticket.Qr += " " + segData[1].QRSeg + " " + segData[1].AirlineName + "-" + segData[1].FlightNumber;
                }
                else if (segData.Count > 2)
                {
                    var segPDFData = new SegmentPDFData
                    {
                        Origin = segData[2].OriginName + " " + (segData[2].OriginAirportName ?? ""),
                        Destination = segData[2].DestinationName + " " + (segData[2].DestinationAirportName ?? ""),
                        Flight = segData[2].AirlineName + " " + segData[2].FlightNumber,
                        Departure = segData[2].FlightDate.ToString("dd MMM hh:mm", ruRU),
                        Arrival = segData[2].ArrivalDate.ToString("dd MMM hh:mm", ruRU),
                    };

                    var termStr = "";
                    if (segData[2].Term1 != null && segData[2].Term1.Length > 1)
                    {
                        termStr = segPDFData.Origin.Length > 21 ? shortTerminal : fullTerminal;
                        termStr += segData[2].Term1 + ")";
                    }

                    segPDFData.Origin += termStr;

                    termStr = "";
                    if (segData[2].Term2 != null && segData[2].Term2.Length > 1)
                    {
                        termStr = segPDFData.Destination.Length > 21 ? shortTerminal : fullTerminal;
                        termStr += segData[2].Term2 + ")";
                    }

                    segPDFData.Destination += termStr;

                    ticket.Seg.Add(segPDFData);
                    ticket.Qr += " " + segData[2].QRSeg + " " + segData[2].AirlineName + "-" + segData[2].FlightNumber;
                }
                else
                {
                    ticket.Seg.Add(new SegmentPDFData
                    {

                    });
                }

                // 4
                if (segData.Count > 2)
                {
                    var segPDFData = new SegmentPDFData
                    {
                        Origin = segData[3].OriginName + " " + (segData[3].OriginAirportName ?? ""),
                        Destination = segData[3].DestinationName + " " + (segData[3].DestinationAirportName ?? ""),
                        Flight = segData[3].AirlineName + " " + segData[3].FlightNumber,
                        Departure = segData[3].FlightDate.ToString("dd MMM hh:mm", ruRU),
                        Arrival = segData[3].ArrivalDate.ToString("dd MMM hh:mm", ruRU),
                    };

                    var termStr = "";
                    if (segData[3].Term1 != null && segData[3].Term1.Length > 1)
                    {
                        termStr = segPDFData.Origin.Length > 21 ? shortTerminal : fullTerminal;
                        termStr += segData[3].Term1 + ")";
                    }

                    segPDFData.Origin += termStr;

                    termStr = "";
                    if (segData[3].Term2 != null && segData[3].Term2.Length > 1)
                    {
                        termStr = segPDFData.Destination.Length > 21 ? shortTerminal : fullTerminal;
                        termStr += segData[3].Term2 + ")";
                    }

                    segPDFData.Destination += termStr;

                    ticket.Seg.Add(segPDFData);
                    ticket.Qr += " " + segData[3].QRSeg + " " + segData[3].AirlineName + "-" + segData[3].FlightNumber;
                }
                else
                {
                    ticket.Seg.Add(new SegmentPDFData
                    {

                    });
                }

                if (segData[0].IsVoid == "F")
                {
                    ticket.Status = "OK";
                }

                var taxData = (from ti in _db.VTicketTaxPDFInfo
                    where ti.TicketID == ticket.TicketId
                    select ti).ToList();

                var total = ticket.Fare;
                if (taxData.Count > 0)
                {
                    ticket.FareCalc += "СБОР / TAX / FEE ";

                    foreach (var td in taxData)
                    {
                        total += td.TaxAmount;
                        ticket.FareCalc += td.TaxType + td.TaxAmount.ToString("#.00", nfi) + "РУБ ";
                    }
                }

                if (ticket.IsExchange && ticket.AddFee > 0)
                {
                    ticket.FareCalc += "ЗА ОБМЕН " + ticket.AddFee.ToString("#.00", nfi) + "РУБ ";
                    total = ticket.ZZFee + ticket.AddFee;
                }

                ticket.Class = segData[0].Class;
                ticket.Qr += " RUB" + total.ToString("#.00", nfi);
                ticket.Total = total.ToString("#.00", nfi) + "РУБ";
                
                var baggageStr = segData[0].Bag;
                if (baggageStr != null && baggageStr.Length > 1)
                {
                    var ind = Regex.Match(baggageStr,"[A-Za-z]+").Index;
                    var number = baggageStr.Substring(0, ind);
                    var qualifier = baggageStr.Substring(ind);
                    if (Regex.IsMatch(number, "\\d*"))
                    {
                        if (number == "0")
                        {
                            baggageStr = "ТОЛЬКО РУЧНАЯ КЛАДЬ";
                        }
                        else if (((qualifier == "P" || qualifier == "PC") && number == "1") || qualifier == "K" || qualifier == "PK")
                        {
                            var placeStr = "НЕ БОЛЕЕ";
                            if (qualifier != "K" && qualifier != "PK")
                            {
                                if (number == "1")
                                {
                                    placeStr = "1 МЕСТО";
                                }
                                else if (number == "2")
                                {
                                    placeStr = "2 МЕСТА";
                                }
                            }

                            if (ticket.Endorsements.Contains("BAGGAGE"))
                            {
                                var startIndex = ticket.Endorsements.IndexOf("BAGGAGE");
                                startIndex = startIndex + 12;
                                number = ticket.Endorsements.Substring(startIndex, 2);
                            }
                            else if (ticket.Endorsements.Contains("БАГАЖ"))
                            {
                                var startIndex = ticket.Endorsements.IndexOf("БАГАЖ");
                                startIndex = startIndex + 9;
                                number = ticket.Endorsements.Substring(startIndex, 2);
                            }
                            else if (qualifier != "K" && qualifier != "PK")
                            {
                                if ((ticket.IssuedBy == "ВИМ-Авиа" && segData[0].BasicFare.Contains("PR")) ||
                                    (ticket.IssuedBy == "АЛРОСА" &&
                                        (segData[0].BasicFare.Contains("OOW") ||
                                         segData[0].BasicFare.Contains("EOW") ||
                                         segData[0].BasicFare.Contains("GOW") ||
                                         segData[0].BasicFare.Contains("UOW"))) ||
                                    (ticket.IssuedBy == "Аэрофлот" && segData[0].BasicFare.Contains("BPXRTRF/IN00")))
                                {
                                    number = "10";
                                }
                                else if (ticket.IssuedBy == "ЮТэйр")
                                {
                                    number = "20";
                                }
                                else if (ticket.IssuedBy == "Саратовские авиалинии")
                                {
                                    bool _15KGRule = false;
                                    foreach(var sd in segData)
                                    {
                                        if (sd.OriginName.Contains("Владивосток") ||
                                            sd.DestinationName.Contains("Благовещенск"))
                                        {
                                            _15KGRule = true;
                                            break;
                                        }
                                    }

                                    if (segData[0].Class == "I" || segData[0].Class == "D" || segData[0].Class == "C" || segData[0].Class == "W")
                                    {
                                        number = "30";
                                    }
                                    else
                                    {
                                        if (_15KGRule)
                                        {
                                            number = "15";
                                        }
                                        else
                                        {
                                            number = "23";
                                        }
                                    }
                                }
                                else if (ticket.IssuedBy == "Северный ветер")
                                {
                                    bool _10KGRule = true;
                                    foreach (var sd in segData)
                                    {
                                        if (((sd.OriginName.Contains("Белгород") ||
                                              sd.DestinationName.Contains("Белгород")) &&
                                            (sd.OriginName.Contains("Симферополь") ||
                                             sd.DestinationName.Contains("Симферополь"))) ||
                                            ((sd.OriginName.Contains("Москва") ||
                                              sd.DestinationName.Contains("Москва")) &&
                                            (sd.OriginName.Contains("Симферополь") ||
                                             sd.DestinationName.Contains("Симферополь"))) ||
                                            ((sd.OriginName.Contains("Москва") ||
                                              sd.DestinationName.Contains("Москва")) &&
                                            (sd.OriginName.Contains("Сочи") ||
                                             sd.DestinationName.Contains("Сочи"))))
                                        {
                                            _10KGRule = false;
                                            break;
                                        }
                                    }

                                    if (segData[0].Class == "O" || segData[0].Class == "P")
                                    {
                                        if (_10KGRule)
                                        {
                                            number = "10";
                                        }
                                        else
                                        {
                                            number = "15";
                                        }
                                    }
                                    else
                                    {
                                        number = "20";
                                    }
                                }
                                else if (ticket.IssuedBy == "Georgian Airways" &&
                                    segData[0].BasicFare.Contains("XOW1"))
                                {
                                    number = "25";
                                }
                                else if (ticket.IssuedBy == "РусЛайн")
                                {
                                    if (segData[0].BasicFare.Contains("CL") || segData[0].BasicFare.Contains("OP") ||
                                        segData[0].BasicFare.Contains("TVKO7R"))
                                    {
                                        number = "23";
                                    }
                                    else if (segData[0].BasicFare.Contains("DPMOW"))
                                    {
                                        number = "30";
                                    }
                                    else if (segData[0].BasicFare.Contains("PM"))
                                    {
                                        number = "32";
                                    }
                                    else
                                    {
                                        number = "10";
                                    }
                                }
                                else if (ticket.IssuedBy == "Air Baltic" &&
                                         segData[0].BasicFare.Contains("PRM"))
                                {
                                    number = "20";
                                }
                                else if (ticket.IssuedBy == "S7 Airlines" &&
                                         segData[0].BasicFare.Contains("FLOW"))
                                {
                                    number = "23";
                                }
                                else if (ticket.IssuedBy == "Уральские Авиалинии" &&
                                         segData[0].BasicFare.Contains("ECOW"))
                                {
                                    bool _23KGRule = false;
                                    foreach (var sd in segData)
                                    {
                                        if (((sd.OriginName.Contains("Сочи") ||
                                              sd.DestinationName.Contains("Сочи")) &&
                                                (sd.OriginName.Contains("Тель-Авив") ||
                                                 sd.DestinationName.Contains("Тель-Авив"))) ||
                                                ((sd.OriginName.Contains("Москва") ||
                                                  sd.DestinationName.Contains("Москва")) &&
                                                (sd.OriginName.Contains("Сочи") ||
                                                 sd.DestinationName.Contains("Сочи"))) ||
                                                ((sd.OriginName.Contains("Москва") ||
                                                  sd.DestinationName.Contains("Москва")) &&
                                                (sd.OriginName.Contains("Челябинск") ||
                                                 sd.DestinationName.Contains("Челябинск"))) ||
                                                ((sd.OriginName.Contains("Владивосток") ||
                                                  sd.DestinationName.Contains("Владивосток")) &&
                                                (sd.OriginName.Contains("Новосибирск") ||
                                                 sd.DestinationName.Contains("Новосибирск"))) ||
                                                ((sd.OriginName.Contains("Москва") ||
                                                  sd.DestinationName.Contains("Москва")) &&
                                                (sd.OriginName.Contains("Екатеринбург") ||
                                                 sd.DestinationName.Contains("Екатеринбург"))) ||
                                                ((sd.OriginName.Contains("Москва") ||
                                                  sd.DestinationName.Contains("Москва")) &&
                                                (sd.OriginName.Contains("Бишкек") ||
                                                 sd.DestinationName.Contains("Бишкек"))) ||
                                                ((sd.OriginName.Contains("Москва") ||
                                                  sd.DestinationName.Contains("Москва")) &&
                                                (sd.OriginName.Contains("Иркутск") ||
                                                 sd.DestinationName.Contains("Иркутск"))))
                                        {
                                            _23KGRule = true;
                                            break;

                                        }
                                    }

                                    if (_23KGRule)
                                    {
                                        number = "23";
                                    }
                                    else
                                    {
                                        number = "20";
                                    }
                                }
                                else if (ticket.IsInfant)
                                {
                                    number = "10";
                                }
                                else
                                {
                                    number = "23";
                                }
                            }

                            baggageStr = placeStr + " " + number + " КГ";
                        }
                    }
                }
                else
                {
                    baggageStr = "Нет багажа";
                }
                ticket.Luggage = baggageStr;

                ticket.BlankImage = new Func<string>(() =>
                {
                    var path = _hostingEnvironment.WebRootPath + "/img/corpImages/AvibaBlank.png";
                    var b = System.IO.File.ReadAllBytes(path);
                    return "data:image/png;base64," + Convert.ToBase64String(b);
                })();

                ticket.FareCalc += "\nПЕРЕДАТОЧ. НАДПИСИ / ENDORSEMENTS / RESTRICTIONS:\n" + ticket.Endorsements;
            }

            return Json(model);
        }
    }
}