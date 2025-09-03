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
    public class PurchaseInvoiceInterimFollowUpReportController : ControllerBase
    {
        private readonly IPurchaseInvoiceInterimFollowUpReportService _purchaseInvoiceInterimFollowUpReportService;
		private readonly IPurchaseInvoiceInterimFollowUpDetailReportService _purchaseInvoiceInterimFollowUpDetailReportService;
		private readonly IPurchaseInvoiceInterimFollowUpCombinedReportService _purchaseInvoiceInterimFollowUpCombinedReportService;

        public PurchaseInvoiceInterimFollowUpReportController(IPurchaseInvoiceInterimFollowUpReportService purchaseInvoiceInterimFollowUpReportService, IPurchaseInvoiceInterimFollowUpDetailReportService purchaseInvoiceInterimFollowUpDetailReportService, IPurchaseInvoiceInterimFollowUpCombinedReportService purchaseInvoiceInterimFollowUpCombinedReportService)
        {
            _purchaseInvoiceInterimFollowUpReportService = purchaseInvoiceInterimFollowUpReportService;
			_purchaseInvoiceInterimFollowUpDetailReportService = purchaseInvoiceInterimFollowUpDetailReportService;
			_purchaseInvoiceInterimFollowUpCombinedReportService = purchaseInvoiceInterimFollowUpCombinedReportService;
        }

        [HttpGet]
        [Route("ReadPurchaseInvoiceInterimFollowUpReport")]
        public async Task<IActionResult> ReadPurchaseInvoiceInterimFollowUpReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = _purchaseInvoiceInterimFollowUpReportService.GetPurchaseInvoiceInterimFollowUpReport(storeIdsList!, fromDate, toDate);

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
        [Route("ReadPurchaseInvoiceInterimFollowUpDetailReport")]
        public async Task<IActionResult> ReadPurchaseInvoiceInterimFollowUpDetailReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = await _purchaseInvoiceInterimFollowUpDetailReportService.GetPurchaseInvoiceInterimFollowUpDetailReport(storeIdsList!, fromDate, toDate);

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
        [Route("ReadPurchaseInvoiceInterimFollowUpCombinedReport")]
        public async Task<IActionResult> ReadPurchaseInvoiceInterimFollowUpCombinedReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate, bool isDetail)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = await _purchaseInvoiceInterimFollowUpCombinedReportService.GetPurchaseInvoiceInterimFollowUpCombinedReport(storeIdsList!, fromDate, toDate, isDetail);

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
