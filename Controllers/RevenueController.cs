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

            var feeAmount = decimal.Parse(model.FeeAmount);

            var record = new ProviderAgentFeeTransaction
            {
                OldAgentFee = provider.ProviderBalance.AgentFee,
                Amount = feeAmount,
                TransactionDateTime = DateTime.Now,
                Comment = model.Comment,
                ProviderId = model.ProviderId
            };

            provider.ProviderBalance.AgentFee += feeAmount;

            _db.ProviderAgentFeeTransactions.Add(record);

            await _db.SaveChangesAsync();

            return Json(new { message = "Ok" });
        }

        [HttpGet]
        public IActionResult ProviderAgentFees()
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var model = new ProviderAgentFeeViewModel
            {
                Providers = (from c in _db.Counterparties.Include(c => c.ProviderBalance)
                            where c.Type.Description == "Провайдер услуг"
                            select new ProviderData
                            {
                                ProviderId = c.ITN,
                                Name = c.Name,
                                FeeAmount = c.ProviderBalance.AgentFee.ToString("#,0.00", nfi)
                            }).ToList()
            };

            return PartialView(model);
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