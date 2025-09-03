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
    public class TopSellingClientsInvoicesController : ControllerBase
    {
        private readonly ITopSellingClientsInvoicesReportService _topSellingClientsInvoicesReportService;

        public TopSellingClientsInvoicesController(ITopSellingClientsInvoicesReportService topSellingClientsInvoicesReportService)
        {
         _topSellingClientsInvoicesReportService= topSellingClientsInvoicesReportService;
        }

        [HttpGet]
        [Route("TopSellingClientsInvoicesReport")]
        public async Task<IActionResult> TopSellingClientsInvoicesReport(DataSourceLoadOptions loadOptions, int companyId, DateTime? fromDate, DateTime? toDate)
        {
            var data = await _topSellingClientsInvoicesReportService.GetTopSellingClientsInvoicesReport(companyId, fromDate, toDate);
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
