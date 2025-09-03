using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne.Contracts.Journal;
using Shared.CoreOne.Models.Dtos.ViewModels.Basics;
using Shared.CoreOne.Models.Dtos.ViewModels.Journal;

namespace Shared.API.Controllers.Journal
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class JournalTypesController : ControllerBase
	{
		private readonly IJournalTypeService _journalTypeService;

		public JournalTypesController(IJournalTypeService journalTypeService)
		{
			_journalTypeService = journalTypeService;
		}

		[HttpGet]
		[Route("GetJournalTypes")]
		public async Task<ActionResult<IEnumerable<JournalTypeDto>>> GetJournalTypes()
		{
			var journalTypes = await _journalTypeService.GetJournalTypes().ToListAsync();
			return Ok(journalTypes);
		}

		[HttpGet]
		[Route("GetJournalTypesDropDown")]
		public async Task<ActionResult<IEnumerable<JournalTypeDropDownDto>>> GetJournalTypesDropDown()
		{
			var journalTypes = await _journalTypeService.GetJournalTypesDropDown();
			return Ok(journalTypes);
		}

		[HttpGet]
		[Route("GetJournalTypesForJournalEntriesDropDown")]
		public async Task<ActionResult<IEnumerable<JournalTypeDropDownDto>>> GetJournalTypesForJournalEntriesDropDown()
		{
			var journalTypes = await _journalTypeService.GetJournalTypesForJournalEntriesDropDown();
			return Ok(journalTypes);
		}
	}
}
