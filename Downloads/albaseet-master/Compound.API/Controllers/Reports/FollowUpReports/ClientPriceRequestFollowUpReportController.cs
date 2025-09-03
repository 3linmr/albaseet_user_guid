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
    public class ClientPriceRequestFollowUpReportController : ControllerBase
    {
        private readonly IClientPriceRequestFollowUpReportService _clientPriceRequestFollowUpReportService;
		private readonly IClientPriceRequestFollowUpDetailReportService _clientPriceRequestFollowUpDetailReportService;
		private readonly IClientPriceRequestFollowUpCombinedReportService _clientPriceRequestFollowUpCombinedReportService;

        public ClientPriceRequestFollowUpReportController(IClientPriceRequestFollowUpReportService clientPriceRequestFollowUpReportService, IClientPriceRequestFollowUpDetailReportService clientPriceRequestFollowUpDetailReportService, IClientPriceRequestFollowUpCombinedReportService clientPriceRequestFollowUpCombinedReportService)
        {
            _clientPriceRequestFollowUpReportService = clientPriceRequestFollowUpReportService;
			_clientPriceRequestFollowUpDetailReportService = clientPriceRequestFollowUpDetailReportService;
			_clientPriceRequestFollowUpCombinedReportService = clientPriceRequestFollowUpCombinedReportService;
        }

        [HttpGet]
        [Route("ReadClientPriceRequestFollowUpReport")]
        public async Task<IActionResult> ReadClientPriceRequestFollowUpReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = _clientPriceRequestFollowUpReportService.GetClientPriceRequestFollowUpReport(storeIdsList!, fromDate, toDate);

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
        [Route("ReadClientPriceRequestFollowUpDetailReport")]
        public async Task<IActionResult> ReadClientPriceRequestFollowUpDetailReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = await _clientPriceRequestFollowUpDetailReportService.GetClientPriceRequestFollowUpDetailReport(storeIdsList!, fromDate, toDate);

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
        [Route("ReadClientPriceRequestFollowUpCombinedReport")]
        public async Task<IActionResult> ReadClientPriceRequestFollowUpCombinedReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate, bool isDetail)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = await _clientPriceRequestFollowUpCombinedReportService.GetClientPriceRequestFollowUpCombinedReport(storeIdsList!, fromDate, toDate, isDetail);

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
