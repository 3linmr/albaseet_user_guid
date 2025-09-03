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
    public class PurchaseInvoiceOnTheWayFollowUpReportController : ControllerBase
    {
        private readonly IPurchaseInvoiceOnTheWayFollowUpReportService _purchaseInvoiceOnTheWayFollowUpReportService;
		private readonly IPurchaseInvoiceOnTheWayFollowUpDetailReportService _purchaseInvoiceOnTheWayFollowUpDetailReportService;
		private readonly IPurchaseInvoiceOnTheWayFollowUpCombinedReportService _purchaseInvoiceOnTheWayFollowUpCombinedReportService;

        public PurchaseInvoiceOnTheWayFollowUpReportController(IPurchaseInvoiceOnTheWayFollowUpReportService purchaseInvoiceOnTheWayFollowUpReportService, IPurchaseInvoiceOnTheWayFollowUpDetailReportService purchaseInvoiceOnTheWayFollowUpDetailReportService, IPurchaseInvoiceOnTheWayFollowUpCombinedReportService purchaseInvoiceOnTheWayFollowUpCombinedReportService)
        {
            _purchaseInvoiceOnTheWayFollowUpReportService = purchaseInvoiceOnTheWayFollowUpReportService;
			_purchaseInvoiceOnTheWayFollowUpDetailReportService = purchaseInvoiceOnTheWayFollowUpDetailReportService;
			_purchaseInvoiceOnTheWayFollowUpCombinedReportService = purchaseInvoiceOnTheWayFollowUpCombinedReportService;
        }

        [HttpGet]
        [Route("ReadPurchaseInvoiceOnTheWayFollowUpReport")]
        public async Task<IActionResult> ReadPurchaseInvoiceOnTheWayFollowUpReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = _purchaseInvoiceOnTheWayFollowUpReportService.GetPurchaseInvoiceOnTheWayFollowUpReport(storeIdsList!, fromDate, toDate);

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
        [Route("ReadPurchaseInvoiceOnTheWayFollowUpDetailReport")]
        public async Task<IActionResult> ReadPurchaseInvoiceOnTheWayFollowUpDetailReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = await _purchaseInvoiceOnTheWayFollowUpDetailReportService.GetPurchaseInvoiceOnTheWayFollowUpDetailReport(storeIdsList!, fromDate, toDate);

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
        [Route("ReadPurchaseInvoiceOnTheWayFollowUpCombinedReport")]
        public async Task<IActionResult> ReadPurchaseInvoiceOnTheWayFollowUpCombinedReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate, bool isDetail)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = await _purchaseInvoiceOnTheWayFollowUpCombinedReportService.GetPurchaseInvoiceOnTheWayFollowUpCombinedReport(storeIdsList!, fromDate, toDate, isDetail);

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
