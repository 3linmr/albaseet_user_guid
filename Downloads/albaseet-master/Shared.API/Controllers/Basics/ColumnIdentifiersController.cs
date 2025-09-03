using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne.Contracts.Basics;
using Shared.CoreOne.Models.Dtos.ViewModels.Basics;
using Shared.Service.Services.Basics;

namespace Shared.API.Controllers.Basics
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class ColumnIdentifiersController : ControllerBase
	{
		private readonly IColumnIdentifierService _columnIdentifierService;

		public ColumnIdentifiersController(IColumnIdentifierService columnIdentifierService)
		{
			_columnIdentifierService = columnIdentifierService;
		}


		[HttpGet]
		[Route("GetColumnIdentifiers")]
		public async Task<ActionResult<IEnumerable<ColumnIdentifierDto>>> GetColumnIdentifiers()
		{
			var identifiers = await _columnIdentifierService.GetColumnIdentifiers().ToListAsync();
			return Ok(identifiers);
		}

		[HttpGet]
		[Route("GetColumnIdentifiersDropDown")]
		public async Task<ActionResult<IEnumerable<CountryDropDownDto>>> GetColumnIdentifiersDropDown()
		{
			var identifiers = await _columnIdentifierService.GetColumnIdentifiersDropDown().ToListAsync();
			return Ok(identifiers);
		}
	}
}
