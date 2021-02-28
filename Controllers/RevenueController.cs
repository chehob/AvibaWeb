using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AvibaWeb.DomainModels;
using AvibaWeb.Infrastructure;
using AvibaWeb.Models;
using AvibaWeb.ViewModels.RevenueViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace AvibaWeb.Controllers
{
    public class RevenueController : Controller
    {
        private readonly AppIdentityDbContext _db;
        private readonly IViewRenderService _viewRenderService;
        private const int PageSize = 10;

        public RevenueController(AppIdentityDbContext db, IViewRenderService viewRenderService)
        {
            _db = db;
            _viewRenderService = viewRenderService;
        }

        public IActionResult Index()
        {
            return PartialView();
        }

        [HttpGet]
        public IActionResult AddProviderAgentFee(string id)
        {
            var model = new AddProviderAgentFeeViewModel
            {
                ProviderId = id
            };
            
            return PartialView(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddProviderAgentFee([FromBody]AddProviderAgentFeePostModel model)
        {
            var provider = _db.Counterparties.Include(c => c.ProviderBalance).FirstOrDefault(c => c.ITN == model.ProviderId);

            if (provider == null)
            {
                return Json(new { message = "No provider" });
            }

            var feeAmount = decimal.Parse(model.FeeAmount, CultureInfo.InvariantCulture);

            var record = new ProviderAgentFeeTransaction
            {
                OldAgentFee = provider.ProviderBalance.AgentFee,
                Amount = feeAmount,
                TransactionDateTime = DateTime.Now,
                Comment = model.Comment,
                ProviderId = model.ProviderId
            };

            provider.ProviderBalance.AgentFee += feeAmount;
            provider.ProviderBalance.Balance += feeAmount;

            _db.ProviderAgentFeeTransactions.Add(record);

            await _db.SaveChangesAsync();

            return Json(new { message = "Ok" });
        }

        [HttpGet]
        public IActionResult ProviderAgentFees()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> ProviderAgentFeeList([FromBody]ProviderAgentFeePostModel postModel)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var queryToDate = postModel.ToDate ?? DateTime.Now;
            var queryFromDate = postModel.FromDate ?? queryToDate.AddDays(-30);

            var transactionGroups = from c in _db.Counterparties.Where(c => c.Type.Description == "Провайдер услуг")
                                    join t in _db.ProviderAgentFeeTransactions
                                        .Where(t => t.TransactionDateTime.Date >= queryFromDate.Date &&
                                            t.TransactionDateTime.Date <= queryToDate.Date) on c.ITN equals t.ProviderId into gt
                                    from st in gt.DefaultIfEmpty()
                                    group new { c, st } by c.ITN into g
                                    select g;

            var model = new ProviderAgentFeeListModel
            {
                Providers = (from tg in transactionGroups
                             select new ProviderData
                             {
                                 ProviderId = tg.FirstOrDefault().c.ITN,
                                 Name = tg.FirstOrDefault().c.Name,
                                 FeeAmount = tg.Sum(g => g.st == null ? 0 : g.st.Amount).ToString("#,0.00", nfi)
                             }).ToList()
            };

            return Json(new { message = await _viewRenderService.RenderToStringAsync("Revenue/ProviderAgentFeeList", model) });
        }

        [HttpGet]
        public IActionResult ProviderAgentFeeTransactions(string id)
        {
            var model = new ProviderAgentFeeTransactionsViewModel
            {
                ProviderId = id
            };

            return PartialView(model);
        }

        [HttpPost]
        public async Task<IActionResult> ProviderAgentFeeTransactionList([FromBody]ProviderAgentFeeTransactionPostModel postModel)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var queryToDate = postModel.ToDate ?? DateTime.Now;
            var queryFromDate = postModel.FromDate ?? queryToDate.AddDays(-30);

            var transactions = from t in _db.ProviderAgentFeeTransactions
                               where (string.IsNullOrEmpty(postModel.ProviderId) || t.ProviderId == postModel.ProviderId) &&
                                t.TransactionDateTime.Date >= queryFromDate.Date && t.TransactionDateTime.Date <= queryToDate.Date
                               select t;

           var model = new ProviderAgentFeeTransactionListModel
            {
                TotalAmount = transactions.Sum(t => t.Amount).ToString("#,0.00", nfi),
                Records = (from t in transactions
                           select new ProviderAgentFeeTransactionData
                           {
                               Amount = t.Amount.ToString("#,0.00", nfi),
                               Comment = t.Comment,
                               TransactionDateTime = t.TransactionDateTime.ToString("d")
                           }).ToList()
            };

            return Json(new { message = await _viewRenderService.RenderToStringAsync("Revenue/ProviderAgentFeeTransactionList", model) });
        }
    }
}