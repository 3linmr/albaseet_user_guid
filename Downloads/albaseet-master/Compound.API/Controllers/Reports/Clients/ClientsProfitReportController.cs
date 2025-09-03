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
	[Authorize]
	[ApiController]
	public class ClientsProfitReportController : Controller //تقرير ارباح العملاء
	{
		private readonly IClientsProfitReportService _clientsProfitReportService;

		public ClientsProfitReportController(IClientsProfitReportService clientsProfitReportService)
		{
			_clientsProfitReportService = clientsProfitReportService; 
		}

		[HttpGet]
		[Route("ReadClientsProfitReport")]
		public async Task<IActionResult> ReadClientsProfitReport(DataSourceLoadOptions loadOptions, int companyId, DateTime? fromDate, DateTime? toDate)
		{
			var data = await _clientsProfitReportService.GetClientsProfitReport(companyId, fromDate, toDate);
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
