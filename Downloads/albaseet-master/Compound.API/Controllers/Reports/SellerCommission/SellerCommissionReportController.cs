using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Compound.API.Models;
using Compound.CoreOne.Contracts.Reports.SellerCommission;
using Microsoft.EntityFrameworkCore;

namespace Compound.API.Controllers.Reports.SellerCommission
{
	[Route("api/[controller]")]
	[Authorize]
	[ApiController]
	public class SellerCommissionReportController : Controller // تقرير عمولات المناديب
	{
		private readonly ISellerCommissionReportService _sellerCommissionReportService;

		public SellerCommissionReportController(ISellerCommissionReportService sellerCommissionReportService)
		{
			_sellerCommissionReportService = sellerCommissionReportService; 
		}

		[HttpGet]
		[Route("ReadSellerCommissionReport")]
		public async Task<IActionResult> ReadSellerCommissionReport(DataSourceLoadOptions loadOptions, int companyId, DateTime? fromDate, DateTime? toDate)
		{
			var data = _sellerCommissionReportService.GetSellerCommissionReport(companyId, fromDate, toDate);
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
