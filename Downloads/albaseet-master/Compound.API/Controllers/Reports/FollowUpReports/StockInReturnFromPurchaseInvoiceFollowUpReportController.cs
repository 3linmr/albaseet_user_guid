using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Compound.CoreOne.Contracts.Reports.FollowUpReports;
using DevExtreme.AspNet.Data;
using System.Text.Json;
using System.Threading.Tasks;
using Compound.API.Models;
using Compound.CoreOne.Contracts.Reports.FollowUpDetailReports;
using Microsoft.EntityFrameworkCore;
using Compound.CoreOne.Contracts.Reports.FollowUpCombinedReports;

namespace Compound.API.Controllers.Reports.FollowUpReports
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StockInReturnFromPurchaseInvoiceFollowUpReportController : ControllerBase
    {
        private readonly IStockInReturnFromPurchaseInvoiceFollowUpReportService _stockInReturnFromPurchaseInvoiceFollowUpReportService;
		private readonly IStockInReturnFromPurchaseInvoiceFollowUpDetailReportService _stockInReturnFromPurchaseInvoiceFollowUpDetailReportService;
		private readonly IStockInReturnFromPurchaseInvoiceFollowUpCombinedReportService _stockInReturnFromPurchaseInvoiceFollowUpCombinedReportService;

        public StockInReturnFromPurchaseInvoiceFollowUpReportController(IStockInReturnFromPurchaseInvoiceFollowUpReportService stockInReturnFromPurchaseInvoiceFollowUpReportService, IStockInReturnFromPurchaseInvoiceFollowUpDetailReportService stockInReturnFromPurchaseInvoiceFollowUpDetailReportService, IStockInReturnFromPurchaseInvoiceFollowUpCombinedReportService stockInReturnFromPurchaseInvoiceFollowUpCombinedReportService)
        {
            _stockInReturnFromPurchaseInvoiceFollowUpReportService = stockInReturnFromPurchaseInvoiceFollowUpReportService;
			_stockInReturnFromPurchaseInvoiceFollowUpDetailReportService = stockInReturnFromPurchaseInvoiceFollowUpDetailReportService;
			_stockInReturnFromPurchaseInvoiceFollowUpCombinedReportService = stockInReturnFromPurchaseInvoiceFollowUpCombinedReportService;
        }

        [HttpGet]
        [Route("ReadStockInReturnFromPurchaseInvoiceFollowUpReport")]
        public async Task<IActionResult> ReadStockInReturnFromPurchaseInvoiceFollowUpReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = _stockInReturnFromPurchaseInvoiceFollowUpReportService.GetStockInReturnFromPurchaseInvoiceFollowUpReport(storeIdsList!, fromDate, toDate);

            try
            {
                return Ok(await DataSourceLoader.LoadAsync(data, loadOptions));
            }
            catch
            {
                return Ok(DataSourceLoader.Load(await data.ToListAsync(), loadOptions));
            }
        }

        [HttpGet]
        [Route("ReadStockInReturnFromPurchaseInvoiceFollowUpDetailReport")]
        public async Task<IActionResult> ReadStockInReturnFromPurchaseInvoiceFollowUpDetailReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = await _stockInReturnFromPurchaseInvoiceFollowUpDetailReportService.GetStockInReturnFromPurchaseInvoiceFollowUpDetailReport(storeIdsList!, fromDate, toDate);

            try
            {
                return Ok(await DataSourceLoader.LoadAsync(data, loadOptions));
            }
            catch
            {
                return Ok(DataSourceLoader.Load(await data.ToListAsync(), loadOptions));
            }
        }

        [HttpGet]
        [Route("ReadStockInReturnFromPurchaseInvoiceFollowUpCombinedReport")]
        public async Task<IActionResult> ReadStockInReturnFromPurchaseInvoiceFollowUpCombinedReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate, bool isDetail)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = await _stockInReturnFromPurchaseInvoiceFollowUpCombinedReportService.GetStockInReturnFromPurchaseInvoiceFollowUpCombinedReport(storeIdsList!, fromDate, toDate, isDetail);

            try
            {
                return Ok(await DataSourceLoader.LoadAsync(data, loadOptions));
            }
            catch
            {
                return Ok(DataSourceLoader.Load(await data.ToListAsync(), loadOptions));
            }
        }
    }
}
