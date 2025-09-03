using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.API.Models;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.CostCenters;
using Shared.CoreOne.Contracts.Approval;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.CostCenters;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.CoreOne.Models.Dtos.ViewModels.Basics;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Database;
using Shared.Helper.Extensions;
using Shared.Helper.Models.Dtos;
using Shared.Service.Services.Modules;
using Shared.Helper.Identity;

namespace Shared.API.Controllers.CostCenters
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class CostCentersController : ControllerBase
	{
		private readonly ICostCenterService _costCenterService;
		private readonly IDatabaseTransaction _databaseTransaction;
		private readonly IStringLocalizer<HandelException> _exLocalizer;
		private readonly IHandleApprovalRequestService _handleApprovalRequestService;
		private readonly IApprovalSystemService _approvalSystemService;
		private readonly IMenuNoteService _menuNoteService;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public CostCentersController(ICostCenterService costCenterService, IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exLocalizer, IHandleApprovalRequestService handleApprovalRequestService, IApprovalSystemService approvalSystemService, IMenuNoteService menuNoteService, IHttpContextAccessor httpContextAccessor)
		{
			_costCenterService = costCenterService;
			_databaseTransaction = databaseTransaction;
			_exLocalizer = exLocalizer;
			_handleApprovalRequestService = handleApprovalRequestService;
			_approvalSystemService = approvalSystemService;
			_menuNoteService = menuNoteService;
			_httpContextAccessor = httpContextAccessor;
		}


		[HttpGet]
		[Route("ReadCostCentersTree")]
		public IActionResult ReadCostCentersTree(DataSourceLoadOptions loadOptions, int companyId)
		{
			var data = DataSourceLoader.Load(_costCenterService.GetCostCentersTree(companyId), loadOptions);
			return Ok(data);
		}

		[HttpGet]
		[Route("ReadCostCenters")]
		public IActionResult ReadCostCenters(DataSourceLoadOptions loadOptions, int storeId)
		{
			var data = DataSourceLoader.Load(_costCenterService.GetIndividualCostCentersByStoreId(storeId), loadOptions);
			return Ok(data);
		}

		[HttpGet]
		[Route("GetCostCentersTree")]
		public async Task<ActionResult<IEnumerable<CostCenterTreeDto>>> GetCostCentersTree(int companyId)
		{
			var data = await _costCenterService.GetCostCentersTree(companyId).ToListAsync();
			return Ok(data);
		}

		[HttpGet]
		[Route("GetMainCostCentersByCompanyIdDropDown")]
		public async Task<ActionResult<IEnumerable<CostCenterDropDownDto>>> GetMainCostCentersByCompanyIdDropDown(int companyId)
		{
			var data = await _costCenterService.GetMainCostCentersByCompanyId(companyId).ToListAsync();
			return Ok(data);
		}

		[HttpGet]
		[Route("GetMainCostCentersByStoreIdDropDown")]
		public async Task<ActionResult<IEnumerable<CostCenterDropDownDto>>> GetMainCostCentersByStoreIdDropDown(int storeId)
		{
			var data = await _costCenterService.GetMainCostCentersByStoreId(storeId).ToListAsync();
			return Ok(data);
		}

		[HttpGet]
		[Route("GetIndividualCostCentersByCompanyIdDropDown")]
		public async Task<ActionResult<IEnumerable<CostCenterDropDownDto>>> GetIndividualCostCentersByCompanyIdDropDown(int companyId)
		{
			var data = await _costCenterService.GetIndividualCostCentersByCompanyId(companyId).ToListAsync();
			return Ok(data);
		}

		[HttpGet]
		[Route("GetIndividualCostCentersByStoreIdDropDown")]
		public async Task<ActionResult<IEnumerable<CostCenterDropDownDto>>> GetIndividualCostCentersByStoreIdDropDown(int storeId)
		{
			var data = await _costCenterService.GetIndividualCostCentersByStoreId(storeId).ToListAsync();
			return Ok(data);
		}

		[HttpGet]
		[Route("GetCostCentersDropDown")]
		public async Task<ActionResult<IEnumerable<CostCenterDropDownDto>>> GetCostCentersDropDown()
		{
			var data = await _costCenterService.GetCostCentersDropDown().ToListAsync();
			return Ok(data);
		}

		[HttpGet]
		[Route("GetMainCostCentersByCostCenterCodeAutoComplete")]
		public async Task<ActionResult<IEnumerable<CostCenterAutoCompleteDto>>> GetMainCostCentersByCostCenterCodeAutoComplete(int companyId, string costCenterCode)
		{
			var data = await _costCenterService.GetMainCostCentersByCostCenterCode(companyId, costCenterCode);
			return Ok(data);
		}

		[HttpGet]
		[Route("GetMainCostCentersByCostCenterNameAutoComplete")]
		public async Task<ActionResult<IEnumerable<CostCenterAutoCompleteDto>>> GetMainCostCentersByCostCenterNameAutoComplete(int companyId, string costCenterName)
		{
			var data = await _costCenterService.GetMainCostCentersByCostCenterName(companyId, costCenterName);
			return Ok(data);
		}

		[HttpGet]
		[Route("GetNextCostCenterCode")]
		public async Task<ActionResult> GetNextCostCenterCode(int companyId, int mainCostCenterId, bool isMainCostCenter)
		{
			var data = await _costCenterService.GetNextCostCenterCode(companyId, mainCostCenterId, isMainCostCenter);
			return Ok(new { data = data });
		}

		[HttpGet]
		[Route("GetAllCostCenters")]
		public async Task<ActionResult> GetAllCostCenters()
		{
			var data = await _costCenterService.GetAllCostCenters().ToListAsync();
			return Ok(data);
		}

		[HttpGet]
		[Route("GetCompanyCostCenters")]
		public async Task<ActionResult> GetCompanyCostCenters(int companyId)
		{
			var data = await _costCenterService.GetCompanyCostCenters(companyId).ToListAsync();
			return Ok(data);
		}

		[HttpGet("{id:int}")]
		public async Task<IActionResult> GetCostCenterByCostCenterId(int id)
		{
			var countryDb = await _costCenterService.GetCostCenterByCostCenterId(id);
			var userCanLook = await _httpContextAccessor.UserCanLook(countryDb.CompanyId, 0);
			return Ok(userCanLook ? countryDb : new CostCenterDto());
		}

		[HttpGet]
		[Route("GetCostCenterByCostCenterCode")]
		public async Task<ActionResult> GetCostCenterByCostCenterCode(int companyId, string costCenterCode)
		{
			var data = await _costCenterService.GetCostCenterByCostCenterCode(companyId, costCenterCode);
			return Ok(data);
		}

		[HttpGet]
		[Route("GetRequestData")]
		public async Task<IActionResult> GetRequestData(int requestId)
		{
			var data = await _handleApprovalRequestService.GetRequestData(requestId);
			if (data != null)
			{
				if (data.CanBeConverted<CostCenterDto>())
				{
					var convertedData = data.ConvertToType<CostCenterDto>();
					var userCanLook = await _httpContextAccessor.UserCanLook(convertedData?.CompanyId ?? 0, 0);
					return Ok(userCanLook ? convertedData : new CostCenterDto());
				}
			}
			return Ok(new CostCenterDto());
		}

		[HttpGet]
		[Route("GetCostCenterTreeName")]
		public async Task<IActionResult> GetCostCenterTreeName(int costCenterId)
		{
			var data = await _costCenterService.GetCostCenterTreeName(costCenterId);
			return Ok(new { data = data });
		}

		[HttpPost]
		[Route("Save")]
		public async Task<IActionResult> Save([FromBody] CostCenterDto model, int requestId)
		{
			ResponseDto response;
			try
			{
				var hasApprove = await _approvalSystemService.IsMenuHasApprove(MenuCodeData.CostCenter);
				if (hasApprove.HasApprove)
				{
					if (hasApprove.OnAdd && model.CostCenterId == 0)
					{
						response = await SendApproveRequest(model, null, requestId, 0, false);
					}
					else if (hasApprove.OnEdit && model.CostCenterId > 0)
					{
						var oldValue = await _costCenterService.GetCostCenterByCostCenterId(model.CostCenterId);
						response = await SendApproveRequest(model, oldValue, requestId, model.CostCenterId, false);
					}
					else
					{
						response = await SaveCostCenter(model);
					}
				}
				else
				{
					response = await SaveCostCenter(model);
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

		[HttpPost]
		[Route("SaveCostCenter")]
		public async Task<ResponseDto> SaveCostCenter(CostCenterDto model)
		{
			await _databaseTransaction.BeginTransaction();
			var response = await _costCenterService.SaveCostCenterInFull(model);
			if (response.Success)
			{
				await _databaseTransaction.Commit();
			}
			else
			{
				await _databaseTransaction.Rollback();
			}
			return response;
		}

		[HttpDelete]
		[Route("DeleteCostCenter")]
		public async Task<IActionResult> DeleteCostCenter(int costCenterId, int requestId)
		{
			ResponseDto response;
			try
			{
				var hasApprove = await _approvalSystemService.IsMenuHasApprove(MenuCodeData.CostCenter);
				if (hasApprove.HasApprove && hasApprove.OnDelete)
				{
					var model = await _costCenterService.GetCostCenterByCostCenterId(costCenterId);
					response = await SendApproveRequest(null, model, requestId, costCenterId, true);
				}
				else
				{
					await _databaseTransaction.BeginTransaction();
					response = await _costCenterService.DeleteCostCenterInFull(costCenterId);
					if (response.Success)
					{
						await _databaseTransaction.Commit();
					}
					else
					{
						await _databaseTransaction.Rollback();
					}
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

		[HttpPost]
		[Route("SendApproveRequest")]
		public async Task<ResponseDto> SendApproveRequest([FromQuery] CostCenterDto? newModel, [FromQuery] CostCenterDto? oldModel, int requestId, int itemId, bool isDelete)
		{
			var changes = new List<RequestChangesDto>();
			var requestTypeId = isDelete ? ApproveRequestTypeData.Delete : (itemId == 0 ? ApproveRequestTypeData.Add : ApproveRequestTypeData.Edit);
			if (requestTypeId == ApproveRequestTypeData.Edit)
			{
				changes = _costCenterService.GetCostCenterRequestChanges(oldModel!, newModel!);
			}

			var request = new NewApproveRequestDto()
			{
				RequestId = requestId,
				MenuCode = MenuCodeData.CostCenter,
				ApproveRequestTypeId = requestTypeId,
				ReferenceId = itemId,
				ReferenceCode = itemId.ToString(),
				OldValue = oldModel,
				NewValue = newModel,
				Changes = changes
			};
			await _databaseTransaction.BeginTransaction();

			var response = await _handleApprovalRequestService.SaveRequest(request);
			if (response.Success)
			{
				await _databaseTransaction.Commit();
			}
			else
			{
				await _databaseTransaction.Rollback();
			}
			return response;
		}
	}
}
