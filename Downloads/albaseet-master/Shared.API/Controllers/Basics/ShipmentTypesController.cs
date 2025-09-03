using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.API.Models;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Basics;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.Helper.Database;
using Shared.Helper.Models.Dtos;
using Shared.Service.Services.Modules;
using Shared.Helper.Identity;

namespace Shared.API.Controllers.Basics
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class ShipmentTypesController : ControllerBase
	{
		private readonly IShipmentTypeService _shipmentTypeService;
		private readonly IDatabaseTransaction _databaseTransaction;
		private readonly IStringLocalizer<HandelException> _exLocalizer;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public ShipmentTypesController(IShipmentTypeService shipmentTypeService,IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exLocalizer, IHttpContextAccessor httpContextAccessor)
		{
			_shipmentTypeService = shipmentTypeService;
			_databaseTransaction = databaseTransaction;
			_exLocalizer = exLocalizer;
			_httpContextAccessor = httpContextAccessor;
		}

		[HttpGet]
		[Route("ReadShipmentTypes")]
		public IActionResult ReadShipmentTypes(DataSourceLoadOptions loadOptions)
		{
			var data = DataSourceLoader.Load(_shipmentTypeService.GetShipmentTypes(), loadOptions);
			return Ok(data);
		}

		[HttpGet]
		[Route("GetShipmentTypes")]
		public async Task<ActionResult<IEnumerable<ShipmentTypeDto>>> GetShipmentTypes()
		{
			var shipmentTypes = await _shipmentTypeService.GetShipmentTypes().ToListAsync();
			return Ok(shipmentTypes);
		}

		[HttpGet("{id:int}")]
		public async Task<IActionResult> GetShipmentTypeById(int id)
		{
			var shipmentTypeDb = await _shipmentTypeService.GetShipmentTypeById(id) ?? new ShipmentTypeDto();
			var userCanLook = await _httpContextAccessor.UserCanLook(shipmentTypeDb.CompanyId ?? 0, 0);
			return Ok(userCanLook ? shipmentTypeDb : new ShipmentTypeDto());
		}

		[HttpPost]
		public async Task<IActionResult> Save([FromBody] ShipmentTypeDto model)
		{
			ResponseDto response;
			try
			{
				await _databaseTransaction.BeginTransaction();
				response = await _shipmentTypeService.SaveShipmentType(model);
				if (response.Success)
				{
					await _databaseTransaction.Commit();
				}
				else
				{
					response.Success = false;
					response.Message = response.Message;
				}
			}
			catch (Exception e)
			{
				await _databaseTransaction.Rollback();
				var handleException = new HandelException(_exLocalizer);
				return Ok(handleException.Handle(e));
			}
			return Ok(response);
		}

		[HttpDelete("{id:int}")]
		public async Task<IActionResult> DeleteShipmentType(int id)
		{
			ResponseDto response;
			try
			{
				await _databaseTransaction.BeginTransaction();
				response = await _shipmentTypeService.DeleteShipmentType(id);
				if (response.Success)
				{
					await _databaseTransaction.Commit();
				}
				else
				{
					await _databaseTransaction.Rollback();
				}
			}
			catch (Exception e)
			{
				await _databaseTransaction.Rollback();
				var handleException = new HandelException(_exLocalizer);
				return Ok(handleException.Handle(e));
			}
			return Ok(response);
		}
	}
}
