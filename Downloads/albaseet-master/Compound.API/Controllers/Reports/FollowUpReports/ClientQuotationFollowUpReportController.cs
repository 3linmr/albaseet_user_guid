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
    public class ClientQuotationFollowUpReportController : ControllerBase
    {
        private readonly IClientQuotationFollowUpReportService _clientQuotationFollowUpReportService;
		private readonly IClientQuotationFollowUpDetailReportService _clientQuotationFollowUpDetailReportService;
		private readonly IClientQuotationFollowUpCombinedReportService _clientQuotationFollowUpCombinedReportService;

        public ClientQuotationFollowUpReportController(IClientQuotationFollowUpReportService clientQuotationFollowUpReportService, IClientQuotationFollowUpDetailReportService clientQuotationFollowUpDetailReportService, IClientQuotationFollowUpCombinedReportService clientQuotationFollowUpCombinedReportService)
        {
            _clientQuotationFollowUpReportService = clientQuotationFollowUpReportService;
			_clientQuotationFollowUpDetailReportService = clientQuotationFollowUpDetailReportService;
			_clientQuotationFollowUpCombinedReportService = clientQuotationFollowUpCombinedReportService;
        }

        [HttpGet]
        [Route("ReadClientQuotationFollowUpReport")]
        public async Task<IActionResult> ReadClientQuotationFollowUpReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = _clientQuotationFollowUpReportService.GetClientQuotationFollowUpReport(storeIdsList!, fromDate, toDate);

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
        [Route("ReadClientQuotationFollowUpDetailReport")]
        public async Task<IActionResult> ReadClientQuotationFollowUpDetailReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = await _clientQuotationFollowUpDetailReportService.GetClientQuotationFollowUpDetailReport(storeIdsList!, fromDate, toDate);

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
        [Route("ReadClientQuotationFollowUpCombinedReport")]
        public async Task<IActionResult> ReadClientQuotationFollowUpCombinedReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate, bool isDetail)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = await _clientQuotationFollowUpCombinedReportService.GetClientQuotationFollowUpCombinedReport(storeIdsList!, fromDate, toDate, isDetail);

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
