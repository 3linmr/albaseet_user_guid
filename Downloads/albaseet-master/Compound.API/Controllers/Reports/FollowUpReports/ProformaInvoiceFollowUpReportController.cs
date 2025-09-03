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
    public class ProformaInvoiceFollowUpReportController : ControllerBase
    {
        private readonly IProformaInvoiceFollowUpReportService _proformaInvoiceFollowUpReportService;
        private readonly IProformaInvoiceFollowUpDetailReportService _proformaInvoiceFollowUpDetailReportService;
        private readonly IProformaInvoiceFollowUpCombinedReportService _proformaInvoiceFollowUpCombinedReportService;

        public ProformaInvoiceFollowUpReportController(IProformaInvoiceFollowUpReportService proformaInvoiceFollowUpReportService, IProformaInvoiceFollowUpDetailReportService proformaInvoiceFollowUpDetailReportService, IProformaInvoiceFollowUpCombinedReportService proformaInvoiceFollowUpCombinedReportService)
        {
            _proformaInvoiceFollowUpReportService = proformaInvoiceFollowUpReportService;
            _proformaInvoiceFollowUpDetailReportService = proformaInvoiceFollowUpDetailReportService;
            _proformaInvoiceFollowUpCombinedReportService = proformaInvoiceFollowUpCombinedReportService;
        }

        [HttpGet]
        [Route("ReadProformaInvoiceFollowUpReport")]
        public async Task<IActionResult> ReadProformaInvoiceFollowUpReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = _proformaInvoiceFollowUpReportService.GetProformaInvoiceFollowUpReport(storeIdsList!, fromDate, toDate);

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
        [Route("ReadProformaInvoiceFollowUpDetailReport")]
        public async Task<IActionResult> ReadProformaInvoiceFollowUpDetailReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = await _proformaInvoiceFollowUpDetailReportService.GetProformaInvoiceFollowUpDetailReport(storeIdsList!, fromDate, toDate);

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
        [Route("ReadProformaInvoiceFollowUpCombinedReport")]
        public async Task<IActionResult> ReadProformaInvoiceFollowUpCombinedReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate, bool isDetail)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = await _proformaInvoiceFollowUpCombinedReportService.GetProformaInvoiceFollowUpCombinedReport(storeIdsList!, fromDate, toDate, isDetail);

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
