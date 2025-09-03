using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Compound.CoreOne.Contracts.Reports.FollowUpReports;
using DevExtreme.AspNet.Data;
using System.Text.Json;
using System.Threading.Tasks;
using Compound.API.Models;
using Compound.CoreOne.Contracts.Reports.FollowUpDetailReports;
using Compound.CoreOne.Contracts.Reports.FollowUpCombinedReports;
using Microsoft.EntityFrameworkCore;

namespace Compound.API.Controllers.Reports.FollowUpReports
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StockOutFromProformaInvoiceFollowUpReportController : ControllerBase
    {
        private readonly IStockOutFromProformaInvoiceFollowUpReportService _stockOutFromProformaInvoiceFollowUpReportService;
        private readonly IStockOutFromProformaInvoiceFollowUpDetailReportService _stockOutFromProformaInvoiceFollowUpDetailReportService;
        private readonly IStockOutFromProformaInvoiceFollowUpCombinedReportService _stockOutFromProformaInvoiceFollowUpCombinedReportService;

        public StockOutFromProformaInvoiceFollowUpReportController(IStockOutFromProformaInvoiceFollowUpReportService stockOutFromProformaInvoiceFollowUpReportService, IStockOutFromProformaInvoiceFollowUpDetailReportService stockOutFromProformaInvoiceFollowUpDetailReportService, IStockOutFromProformaInvoiceFollowUpCombinedReportService stockOutFromProformaInvoiceFollowUpCombinedReportService)
        {
            _stockOutFromProformaInvoiceFollowUpReportService = stockOutFromProformaInvoiceFollowUpReportService;
            _stockOutFromProformaInvoiceFollowUpDetailReportService = stockOutFromProformaInvoiceFollowUpDetailReportService;
            _stockOutFromProformaInvoiceFollowUpCombinedReportService = stockOutFromProformaInvoiceFollowUpCombinedReportService;
        }

        [HttpGet]
        [Route("ReadStockOutFromProformaInvoiceFollowUpReport")]
        public async Task<IActionResult> ReadStockOutFromProformaInvoiceFollowUpReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = _stockOutFromProformaInvoiceFollowUpReportService.GetStockOutFromProformaInvoiceFollowUpReport(storeIdsList!, fromDate, toDate);

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
        [Route("ReadStockOutFromProformaInvoiceFollowUpDetailReport")]
        public async Task<IActionResult> ReadStockOutFromProformaInvoiceFollowUpDetailReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = await _stockOutFromProformaInvoiceFollowUpDetailReportService.GetStockOutFromProformaInvoiceFollowUpDetailReport(storeIdsList!, fromDate, toDate);

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
        [Route("ReadStockOutFromProformaInvoiceFollowUpCombinedReport")]
        public async Task<IActionResult> ReadStockOutFromProformaInvoiceFollowUpCombinedReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate, bool isDetail)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = await _stockOutFromProformaInvoiceFollowUpCombinedReportService.GetStockOutFromProformaInvoiceFollowUpCombinedReport(storeIdsList!, fromDate, toDate, isDetail);

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
