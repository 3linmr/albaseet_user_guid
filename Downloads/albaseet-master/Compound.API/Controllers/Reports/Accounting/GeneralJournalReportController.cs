using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.CoreOne.Contracts.Journal;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne.Contracts.Accounts;
using Compound.API.Models;
using DevExtreme.AspNet.Data;
using Compound.CoreOne.Contracts.Reports.Accounting;
using System.Text.Json;

namespace Compound.API.Controllers.Reports.Accounting
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GeneralJournalReportController : ControllerBase
    {
        private readonly IGeneralJournalReportService _generalJournalReportService;

        public GeneralJournalReportController(IGeneralJournalReportService generalJournalReportService)
        {
            _generalJournalReportService = generalJournalReportService;
        }

        [HttpGet]
        [Route("ReadGeneralJournalReport")]
        public async Task<IActionResult> ReadGeneralJournalReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate, string? journalTypes)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>?>(storeIds ?? "[]")!;
            var journalTypesList = JsonSerializer.Deserialize<List<byte>?>(journalTypes ?? "[]")!;
            var data = _generalJournalReportService.GetGeneralJournalReport(storeIdsList, fromDate, toDate, journalTypesList);
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
        [Route("ReadGeneralJournalReportUserStores")]
        public async Task<IActionResult> ReadGeneralJournalReportUserStores(DataSourceLoadOptions loadOptions)
        {
            var data = await _generalJournalReportService.GetGeneralJournalReportUserStores();
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
