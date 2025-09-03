using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Compound.CoreOne.Contracts.Reports.Clients;
using Microsoft.EntityFrameworkCore;
using Compound.API.Models;
using DevExtreme.AspNet.Data;
using System.Text.Json;

namespace Compound.API.Controllers.Reports.Clients
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TopSellingClientsWithAmountsController : ControllerBase
    {
        private readonly ITopSellingClientsWithAmountsReportService _topSellingClientsReportService;

        public TopSellingClientsWithAmountsController(ITopSellingClientsWithAmountsReportService topSellingClientsReportService)
        {
            _topSellingClientsReportService = topSellingClientsReportService;
        }

        [HttpGet]
        [Route("TopSellingClientsWithAmountsReport")]
        public async Task<IActionResult> TopSellingClientsWithAmountsReport(DataSourceLoadOptions loadOptions, int companyId, DateTime? fromDate, DateTime? toDate)
        {
            var data = _topSellingClientsReportService.GetTopSellingClientsWithAmountsReport(companyId, fromDate, toDate);
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
