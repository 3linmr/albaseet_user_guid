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
    public class PurchaseOrderFollowUpReportController : ControllerBase
    {
        private readonly IPurchaseOrderFollowUpReportService _purchaseOrderFollowUpReportService;
		private readonly IPurchaseOrderFollowUpDetailReportService _purchaseOrderFollowUpDetailReportService;
		private readonly IPurchaseOrderFollowUpCombinedReportService _purchaseOrderFollowUpCombinedReportService;

        public PurchaseOrderFollowUpReportController(IPurchaseOrderFollowUpReportService purchaseOrderFollowUpReportService, IPurchaseOrderFollowUpDetailReportService purchaseOrderFollowUpDetailReportService, IPurchaseOrderFollowUpCombinedReportService purchaseOrderFollowUpCombinedReportService)
        {
            _purchaseOrderFollowUpReportService = purchaseOrderFollowUpReportService;
			_purchaseOrderFollowUpDetailReportService = purchaseOrderFollowUpDetailReportService;
			_purchaseOrderFollowUpCombinedReportService = purchaseOrderFollowUpCombinedReportService;
        }

        [HttpGet]
        [Route("ReadPurchaseOrderFollowUpReport")]
        public async Task<IActionResult> ReadPurchaseOrderFollowUpReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = _purchaseOrderFollowUpReportService.GetPurchaseOrderFollowUpReport(storeIdsList!, fromDate, toDate);

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
        [Route("ReadPurchaseOrderFollowUpDetailReport")]
        public async Task<IActionResult> ReadPurchaseOrderFollowUpDetailReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = await _purchaseOrderFollowUpDetailReportService.GetPurchaseOrderFollowUpDetailReport(storeIdsList!, fromDate, toDate);

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
        [Route("ReadPurchaseOrderFollowUpCombinedReport")]
        public async Task<IActionResult> ReadPurchaseOrderFollowUpCombinedReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate, bool isDetail)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = await _purchaseOrderFollowUpCombinedReportService.GetPurchaseOrderFollowUpCombinedReport(storeIdsList!, fromDate, toDate, isDetail);

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
