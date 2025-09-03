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
    public class SalesInvoiceInterimFollowUpReportController : ControllerBase
    {
        private readonly ISalesInvoiceInterimFollowUpReportService _salesInvoiceInterimFollowUpReportService;
        private readonly ISalesInvoiceInterimFollowUpDetailReportService _salesInvoiceInterimFollowUpDetailReportService;
        private readonly ISalesInvoiceInterimFollowUpCombinedReportService _salesInvoiceInterimFollowUpCombinedReportService;

        public SalesInvoiceInterimFollowUpReportController(ISalesInvoiceInterimFollowUpReportService salesInvoiceInterimFollowUpReportService, ISalesInvoiceInterimFollowUpDetailReportService salesInvoiceInterimFollowUpDetailReportService, ISalesInvoiceInterimFollowUpCombinedReportService salesInvoiceInterimFollowUpCombinedReportService)
        {
            _salesInvoiceInterimFollowUpReportService = salesInvoiceInterimFollowUpReportService;
            _salesInvoiceInterimFollowUpDetailReportService = salesInvoiceInterimFollowUpDetailReportService;
            _salesInvoiceInterimFollowUpCombinedReportService = salesInvoiceInterimFollowUpCombinedReportService;
        }

        [HttpGet]
        [Route("ReadSalesInvoiceInterimFollowUpReport")]
        public async Task<IActionResult> ReadSalesInvoiceInterimFollowUpReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = _salesInvoiceInterimFollowUpReportService.GetSalesInvoiceInterimFollowUpReport(storeIdsList!, fromDate, toDate);

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
        [Route("ReadSalesInvoiceInterimFollowUpDetailReport")]
        public async Task<IActionResult> ReadSalesInvoiceInterimFollowUpDetailReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = await _salesInvoiceInterimFollowUpDetailReportService.GetSalesInvoiceInterimFollowUpDetailReport(storeIdsList!, fromDate, toDate);

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
        [Route("ReadSalesInvoiceInterimFollowUpCombinedReport")]
        public async Task<IActionResult> ReadSalesInvoiceInterimFollowUpCombinedReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate, bool isDetail)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = await _salesInvoiceInterimFollowUpCombinedReportService.GetSalesInvoiceInterimFollowUpCombinedReport(storeIdsList!, fromDate, toDate, isDetail);

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
