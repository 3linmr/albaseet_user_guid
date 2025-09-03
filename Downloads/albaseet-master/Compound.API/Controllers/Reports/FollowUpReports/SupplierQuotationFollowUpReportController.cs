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
    public class SupplierQuotationFollowUpReportController : ControllerBase
    {
        private readonly ISupplierQuotationFollowUpReportService _supplierQuotationFollowUpReportService;
		private readonly ISupplierQuotationFollowUpDetailReportService _supplierQuotationFollowUpDetailReportService;
		private readonly ISupplierQuotationFollowUpCombinedReportService _supplierQuotationFollowUpCombinedReportService;

        public SupplierQuotationFollowUpReportController(ISupplierQuotationFollowUpReportService supplierQuotationFollowUpReportService, ISupplierQuotationFollowUpDetailReportService supplierQuotationFollowUpDetailReportService, ISupplierQuotationFollowUpCombinedReportService supplierQuotationFollowUpCombinedReportService)
        {
            _supplierQuotationFollowUpReportService = supplierQuotationFollowUpReportService;
			_supplierQuotationFollowUpDetailReportService = supplierQuotationFollowUpDetailReportService;
			_supplierQuotationFollowUpCombinedReportService = supplierQuotationFollowUpCombinedReportService;
        }

        [HttpGet]
        [Route("ReadSupplierQuotationFollowUpReport")]
        public async Task<IActionResult> ReadSupplierQuotationFollowUpReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = _supplierQuotationFollowUpReportService.GetSupplierQuotationFollowUpReport(storeIdsList!, fromDate, toDate);

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
        [Route("ReadSupplierQuotationFollowUpDetailReport")]
        public async Task<IActionResult> ReadSupplierQuotationFollowUpDetailReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = await _supplierQuotationFollowUpDetailReportService.GetSupplierQuotationFollowUpDetailReport(storeIdsList!, fromDate, toDate);

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
        [Route("ReadSupplierQuotationFollowUpCombinedReport")]
        public async Task<IActionResult> ReadSupplierQuotationFollowUpCombinedReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate, bool isDetail)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = await _supplierQuotationFollowUpCombinedReportService.GetSupplierQuotationFollowUpCombinedReport(storeIdsList!, fromDate, toDate, isDetail);

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
