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
    public class StockOutReturnFromSalesInvoiceFollowUpReportController : ControllerBase
    {
        private readonly IStockOutReturnFromSalesInvoiceFollowUpReportService _stockOutReturnFromSalesInvoiceFollowUpReportService;
        private readonly IStockOutReturnFromSalesInvoiceFollowUpDetailReportService _stockOutReturnFromSalesInvoiceFollowUpDetailReportService;
        private readonly IStockOutReturnFromSalesInvoiceFollowUpCombinedReportService _stockOutReturnFromSalesInvoiceFollowUpCombinedReportService;

        public StockOutReturnFromSalesInvoiceFollowUpReportController(IStockOutReturnFromSalesInvoiceFollowUpReportService stockOutReturnFromSalesInvoiceFollowUpReportService, IStockOutReturnFromSalesInvoiceFollowUpDetailReportService stockOutReturnFromSalesInvoiceFollowUpDetailReportService, IStockOutReturnFromSalesInvoiceFollowUpCombinedReportService stockOutReturnFromSalesInvoiceFollowUpCombinedReportService)
        {
            _stockOutReturnFromSalesInvoiceFollowUpReportService = stockOutReturnFromSalesInvoiceFollowUpReportService;
            _stockOutReturnFromSalesInvoiceFollowUpDetailReportService = stockOutReturnFromSalesInvoiceFollowUpDetailReportService;
            _stockOutReturnFromSalesInvoiceFollowUpCombinedReportService = stockOutReturnFromSalesInvoiceFollowUpCombinedReportService;
        }

        [HttpGet]
        [Route("ReadStockOutReturnFromSalesInvoiceFollowUpReport")]
        public async Task<IActionResult> ReadStockOutReturnFromSalesInvoiceFollowUpReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = _stockOutReturnFromSalesInvoiceFollowUpReportService.GetStockOutReturnFromSalesInvoiceFollowUpReport(storeIdsList!, fromDate, toDate);

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
        [Route("ReadStockOutReturnFromSalesInvoiceFollowUpDetailReport")]
        public async Task<IActionResult> ReadStockOutReturnFromSalesInvoiceFollowUpDetailReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = await _stockOutReturnFromSalesInvoiceFollowUpDetailReportService.GetStockOutReturnFromSalesInvoiceFollowUpDetailReport(storeIdsList!, fromDate, toDate);

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
        [Route("ReadStockOutReturnFromSalesInvoiceFollowUpCombinedReport")]
        public async Task<IActionResult> ReadStockOutReturnFromSalesInvoiceFollowUpCombinedReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate, bool isDetail)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = await _stockOutReturnFromSalesInvoiceFollowUpCombinedReportService.GetStockOutReturnFromSalesInvoiceFollowUpCombinedReport(storeIdsList!, fromDate, toDate, isDetail);

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
