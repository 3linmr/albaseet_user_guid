using Accounting.CoreOne.Contracts.Reports;
using Accounting.CoreOne.Models.Dtos.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.API.Controllers.Reports
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class JournalLedgersController : ControllerBase
	{
		private readonly IJournalLedgerService _journalLedgerService;

		public JournalLedgersController(IJournalLedgerService journalLedgerService)
		{
			_journalLedgerService = journalLedgerService;
		}


		[HttpGet]
		[Route("GetLedgers")]
		public async Task<List<LedgerDto>> GetLedgers(DateTime fromDate, DateTime toDate, int storeId)
		{
			var data = await _journalLedgerService.GetList(fromDate, toDate, storeId);
			return data;
		} 
	}
}
