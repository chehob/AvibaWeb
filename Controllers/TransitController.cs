using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AvibaWeb.DomainModels;
using AvibaWeb.Models;
using AvibaWeb.ViewModels.CollectionViewModels;
using AvibaWeb.ViewModels.TransitViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace AvibaWeb.Controllers
{
    public class TransitController : Controller
    {
        private readonly AppIdentityDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        private const int PageSize = 10;

        public TransitController(AppIdentityDbContext db,
            UserManager<AppUser> usrMgr)
        {
            _db = db;
            _userManager = usrMgr;
        }

        public IActionResult Index()
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var model = new TransitAccountViewModel
            {
                Balance = _db.TransitAccounts.FirstOrDefault().Balance.ToString("#,0.00", nfi),
                LoanGroups = _db.LoanGroups.Where(g => g.IsActive).ToList()
            };
            return PartialView(model);
        }

        [HttpGet]
        public IActionResult Debit()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<ActionResult> Debit(TransitAccountDebit debit)
        {
            var user = await _userManager.FindByIdAsync(_userManager.GetUserId(User));

            var officeRole = _db.Roles.SingleOrDefault(r => r.Name.Contains("Офис"));
            var office = _db.Users.FirstOrDefault(u => u.Roles.Any(r => r.RoleId == officeRole.Id));
            if (office == null) return RedirectToAction("Index", "Expenditure");

            var account = _db.TransitAccounts.FirstOrDefault();
            if (account == null) return RedirectToAction("Index", "Expenditure");

            user.Balance -= debit.Amount;
            account.Balance += debit.Amount;

            debit.Account = account;
            debit.OperationDateTime = DateTime.Now;

            var collection = new Collection
            {
                Amount = debit.Amount,
                ProviderId = user.Id,
                CollectorId = office.Id
            };

            var operation = new CollectionOperation
            {
                Collection = collection,
                OperationDateTime = DateTime.Now,
                OperationTypeId = CollectionOperationType.COType.Accepted
            };

            _db.CollectionOperations.Add(operation);
            _db.TransitAccountDebits.Add(debit);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index", "Expenditure");
        }

        [HttpGet]
        public IActionResult CreateCredit(int? id)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var account = _db.TransitAccounts.FirstOrDefault();
            if (account == null) return RedirectToAction("Index");

            var model = new CreateCreditViewModel
            {
                Credit = new TransitAccountCredit
                {
                    LoanGroupId = id,
                    Account = account
                },
                TransitBalance = _db.TransitAccounts.FirstOrDefault().Balance.ToString("#,0.00", nfi)
            };
            return PartialView(model);
        }

        [HttpPost]
        public async Task<ActionResult> CreateCredit(CreateCreditViewModel model)
        {
            var user = await _userManager.FindByIdAsync(_userManager.GetUserId(User));

            var account = _db.TransitAccounts.FirstOrDefault();
            if (account == null) return RedirectToAction("Index");

            var loanGroup = _db.LoanGroups.FirstOrDefault(g => g.LoanGroupId == model.Credit.LoanGroupId);
            
            account.Balance -= model.Credit.AccountAmount;
            if (loanGroup == null)
            {
                user.Balance += model.Credit.AccountAmount;
            }
            else
            {
                loanGroup.Balance += model.Credit.AccountAmount + model.Credit.AddAmount;
            }

            model.Credit.LoanGroup = loanGroup;
            model.Credit.Account = account;

            var operation = new TransitAccountCreditOperation
            {
                TransitAccountCredit = model.Credit,
                OperationDateTime = DateTime.Now,
                OperationTypeId = TransitAccountCreditOperation.TACOType.New
            };

            _db.TransitAccountCreditOperations.Add(operation);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> CancelCredit(int id)
        {
            var credit = await _db.TransitAccountCredits
                .Include(c => c.LoanGroup).Include(c => c.Account)
                .FirstOrDefaultAsync(c => c.TransitAccountCreditId == id);
            if (credit == null) return PartialView("Error", new[] { "Операция не найдена" });

            credit.Account.Balance += credit.AccountAmount;
            credit.LoanGroup.Balance -= credit.AccountAmount + credit.AddAmount;

            var operation = new TransitAccountCreditOperation
            {
                TransitAccountCredit = credit,
                OperationDateTime = DateTime.Now,
                OperationTypeId = TransitAccountCreditOperation.TACOType.Cancelled
            };

            _db.TransitAccountCreditOperations.Add(operation);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult IssuedCredits(int loanGroupId)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var creditList = (from credit in _db.TransitAccountCredits
                             where credit.LoanGroupId == loanGroupId
                             join taco in _db.TransitAccountCreditOperations on credit.TransitAccountCreditId equals taco.TransitAccountCreditId into operations
                             from operation in operations.OrderByDescending(o => o.OperationDateTime).Take(1)
                             orderby operation.OperationDateTime descending
                             select new { credit, operation }).ToList();

            var issuedCredits = (from c in creditList
                select new TransitAccountIssuedCredit
                {
                    CreditId = c.credit.TransitAccountCreditId,
                    LoanGroupId = loanGroupId,
                    Amount = c.credit.AccountAmount,
                    AmountStr = c.credit.AccountAmount.ToString("#,0.00", nfi),
                    AddAmount = c.credit.AddAmount,
                    AddAmountStr = c.credit.AddAmount.ToString("#,0.00", nfi),
                    IssuedDateTime = c.operation.OperationDateTime,
                    Status = c.operation.OperationTypeId,
                    Comment = c.credit.Comment
                }).ToList();

            var financialOperations = (from fao in _db.FinancialAccountOperations
                    .Include(f => f.Counterparty)
                where fao.Counterparty.LoanGroupId == loanGroupId &&
                      fao.Amount >= 0
                select fao).ToList();

            issuedCredits.AddRange((from f in financialOperations
                select new TransitAccountIssuedCredit
                {
                    CreditId = 0,
                    LoanGroupId = f.Counterparty.LoanGroupId.Value,
                    Amount = -f.Amount,
                    AmountStr = (-f.Amount).ToString("#,0.00", nfi),
                    IssuedDateTime = f.OperationDateTime,
                    Status = 0,
                    Payer = f.Counterparty.Name,
                    Comment = f.Description
                }).ToList());

            issuedCredits = issuedCredits.OrderByDescending(t => t.IssuedDateTime).ToList();
            var model = new TransitAccountIssuedCreditsViewModel
            {
                Credits = issuedCredits,
                FinancialOperationsTotal =
                    issuedCredits.Where(c => c.Amount < 0).Sum(c => c.Amount).ToString("#,0.00", nfi),
                CreditsTotal = issuedCredits.Where(c => c.Amount > 0).Sum(c => c.Amount).ToString("#,0.00", nfi),
                AddTotal = issuedCredits.Where(c => c.AddAmount > 0).Sum(c => c.AddAmount).ToString("#,0.00", nfi)
            };
            return PartialView(model);
        }

        [HttpGet]
        public IActionResult CreateCreditAdd(int? id)
        {
            var account = _db.TransitAccounts.FirstOrDefault();
            if (account == null) return RedirectToAction("Index");

            var model = new CreateCreditViewModel
            {
                Credit = new TransitAccountCredit
                {
                    LoanGroupId = id,
                    Account = account
                }
            };
            return PartialView(model);
        }

        [HttpGet]
        public IActionResult TransitAccountOperations(int? page)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var creditList = (from credit in _db.TransitAccountCredits
                    .Include(c => c.LoanGroup)
                join taco in _db.TransitAccountCreditOperations on credit.TransitAccountCreditId equals taco
                    .TransitAccountCreditId into operations
                from operation in operations.OrderByDescending(o => o.OperationDateTime).Take(1)
                orderby operation.OperationDateTime descending
                select new {credit, operation}).ToList();

            var transitAccountOperations = (from c in creditList
                                 select new TransitAccountIssuedCredit
                                 {
                                     CreditId = c.credit.TransitAccountCreditId,
                                     LoanGroupId = c.credit.LoanGroupId ?? 0,
                                     LoanGroup = c.credit.LoanGroup != null ? c.credit.LoanGroup.Description : "",
                                     Amount = -c.credit.AccountAmount,
                                     AmountStr = (-c.credit.AccountAmount).ToString("#,0.00", nfi),
                                     AddAmount = -c.credit.AddAmount,
                                     AddAmountStr = (-c.credit.AddAmount).ToString("#,0.00", nfi),
                                     IssuedDateTime = c.operation.OperationDateTime,
                                     Status = c.operation.OperationTypeId,
                                     Comment = c.credit.Comment
                                 }).ToList();

            var debitList = (from debit in _db.TransitAccountDebits
                orderby debit.OperationDateTime descending
                select debit).ToList();

            transitAccountOperations.AddRange((from d in debitList
                                    select new TransitAccountIssuedCredit
                                    {
                                        CreditId = 0,
                                        //LoanGroupId = f.Counterparty.LoanGroupId.Value,
                                        Amount = d.Amount,
                                        AmountStr = d.Amount.ToString("#,0.00", nfi),
                                        IssuedDateTime = d.OperationDateTime,
                                        Status = 0,
                                        Comment = d.Comment
                                    }).ToList());

            transitAccountOperations = transitAccountOperations.OrderByDescending(t => t.IssuedDateTime).ToList();
            var pageNumber = (page ?? 1);
            //var model = new TransitAccountIssuedCreditsViewModel
            //{
            //    Credits = issuedCredits.ToPagedList(pageNumber, PageSize),
            //    FinancialOperationsTotal =
            //        issuedCredits.Where(c => c.Amount < 0).Sum(c => c.Amount).ToString("#,0.00", nfi),
            //    CreditsTotal = issuedCredits.Where(c => c.Amount > 0).Sum(c => c.Amount).ToString("#,0.00", nfi),
            //    AddTotal = issuedCredits.Where(c => c.AddAmount > 0).Sum(c => c.AddAmount).ToString("#,0.00", nfi)
            //};
            return PartialView(transitAccountOperations.ToPagedList(pageNumber, PageSize));
        }
    }
}