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
    public class ClientQuotationApprovalFollowUpReportController : ControllerBase
    {
        private readonly IClientQuotationApprovalFollowUpReportService _clientQuotationApprovalFollowUpReportService;
        private readonly IClientQuotationApprovalFollowUpDetailReportService _clientQuotationApprovalFollowUpDetailReportService;
        private readonly IClientQuotationApprovalFollowUpCombinedReportService _clientQuotationApprovalFollowUpCombinedReportService;

        public ClientQuotationApprovalFollowUpReportController(IClientQuotationApprovalFollowUpReportService clientQuotationApprovalFollowUpReportService, IClientQuotationApprovalFollowUpDetailReportService clientQuotationApprovalFollowUpDetailReportService, IClientQuotationApprovalFollowUpCombinedReportService clientQuotationApprovalFollowUpCombinedReportService)
        {
            _clientQuotationApprovalFollowUpReportService = clientQuotationApprovalFollowUpReportService;
            _clientQuotationApprovalFollowUpDetailReportService = clientQuotationApprovalFollowUpDetailReportService;
            _clientQuotationApprovalFollowUpCombinedReportService = clientQuotationApprovalFollowUpCombinedReportService;
        }

        [HttpGet]
        [Route("ReadClientQuotationApprovalFollowUpReport")]
        public async Task<IActionResult> ReadClientQuotationApprovalFollowUpReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = _clientQuotationApprovalFollowUpReportService.GetClientQuotationApprovalFollowUpReport(storeIdsList!, fromDate, toDate);

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
        [Route("ReadClientQuotationApprovalFollowUpDetailReport")]
        public async Task<IActionResult> ReadClientQuotationApprovalFollowUpDetailReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = await _clientQuotationApprovalFollowUpDetailReportService.GetClientQuotationApprovalFollowUpDetailReport(storeIdsList!, fromDate, toDate);

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
        [Route("ReadClientQuotationApprovalFollowUpCombinedReport")]
        public async Task<IActionResult> ReadClientQuotationApprovalFollowUpCombinedReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate, bool isDetail)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = await _clientQuotationApprovalFollowUpCombinedReportService.GetClientQuotationApprovalFollowUpCombinedReport(storeIdsList!, fromDate, toDate, isDetail);

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
