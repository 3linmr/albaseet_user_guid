using System.Data.SqlClient;
using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.API.Extensions;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Basics;
using Shared.CoreOne.Models.Dtos.ViewModels.Basics;
using Shared.API.Models;
using Shared.Helper.Models.Dtos;
using Shared.Helper.Database;
using Shared.Service.Services.Basics;

namespace Shared.API.Controllers.Basics
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class InvoiceTypeController : ControllerBase
	{
		private readonly IDatabaseTransaction _databaseTransaction;
		private readonly IInvoiceTypeService _invoiceTypeService;

		public InvoiceTypeController(IDatabaseTransaction databaseTransaction, IInvoiceTypeService invoiceTypeService, IStringLocalizer<HandelException> exLocalizer)
		{
			_databaseTransaction = databaseTransaction;
			_invoiceTypeService = invoiceTypeService;
		}

		[HttpGet]
		[Route("ReadInvoiceTypes")]
		public IActionResult ReadInvoiceTypes(DataSourceLoadOptions loadOptions)
		{
			var data = DataSourceLoader.Load(_invoiceTypeService.GetAllInvoiceTypes(), loadOptions);
			return Ok(data);
		}

		[HttpGet]
		[Route("GetAllInvoiceTypesDropDown")]
		public async Task<ActionResult<IEnumerable<InvoiceTypeDropDownDto>>> GetAllInvoiceTypesDropDown()
		{
			var allInvoiceTypes = await _invoiceTypeService.GetAllInvoiceTypesDropDown().ToListAsync();
			return Ok(allInvoiceTypes);
		}
	}
}
