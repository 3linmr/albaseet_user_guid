using DevExtreme.AspNet.Data;
using Compound.API.Models;
using Compound.CoreOne.Contracts.Reservation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Sales.CoreOne.Contracts;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Approval;
using Shared.CoreOne.Contracts.Menus;
using Shared.Helper.Database;
using Shared.Helper.Extensions;
using Sales.CoreOne.Models.Dtos.ViewModels;
using Shared.Helper.Models.Dtos;
using Shared.CoreOne.Models.StaticData;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.Helper.Identity;

namespace Compound.API.Controllers.Reservation
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class StockOutReturnsFromReservationController : ControllerBase
	{
		private readonly IStockOutReturnHandlingService _stockOutReturnHandlingService;
		private readonly IStockOutReturnService _stockOutReturnService;
		private readonly IStockOutReturnHeaderService _stockOutReturnHeaderService;
		private readonly IStockOutReturnDetailService _stockOutReturnDetailService;
		private readonly IDatabaseTransaction _databaseTransaction;
		private readonly IStringLocalizer<HandelException> _exLocalizer;
		private readonly IGenericMessageService _genericMessageService;
		private readonly IApprovalSystemService _approvalSystemService;
		private readonly IHandleApprovalRequestService _handleApprovalRequestService;
		private readonly IStockOutReturnFromReservationService _stockOutReturnFromReservationService;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public StockOutReturnsFromReservationController(IStockOutReturnHandlingService stockOutReturnHandlingService, IStockOutReturnService stockOutReturnService, IStockOutReturnHeaderService stockOutReturnHeaderService, IStockOutReturnDetailService stockOutReturnDetailService, IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exLocalizer, IGenericMessageService genericMessageService, IApprovalSystemService approvalSystemService, IHandleApprovalRequestService handleApprovalRequestService, IStockOutReturnFromReservationService stockOutReturnFromReservationService, IHttpContextAccessor httpContextAccessor)
		{
			_stockOutReturnHandlingService = stockOutReturnHandlingService;
			_stockOutReturnService = stockOutReturnService;
			_stockOutReturnHeaderService = stockOutReturnHeaderService;
			_stockOutReturnDetailService = stockOutReturnDetailService;
			_databaseTransaction = databaseTransaction;
			_exLocalizer = exLocalizer;
			_genericMessageService = genericMessageService;
			_approvalSystemService = approvalSystemService;
			_handleApprovalRequestService = handleApprovalRequestService;
			_stockOutReturnFromReservationService = stockOutReturnFromReservationService;
			_httpContextAccessor = httpContextAccessor;
		}

		[HttpGet]
		[Route("ReadStockOutReturns")]
		public async Task<IActionResult> ReadStockOutReturns(DataSourceLoadOptions loadOptions, int stockTypeId)
		{
			try
			{
				var data = await DataSourceLoader.LoadAsync(_stockOutReturnHeaderService.GetUserStockOutReturnHeaders(stockTypeId), loadOptions);
				return Ok(data);
			}
			catch
			{
				var model = _stockOutReturnHeaderService.GetUserStockOutReturnHeaders(stockTypeId);
				var data = DataSourceLoader.Load(model.ToList(), loadOptions);
				return Ok(data);
			}
		}

		[HttpGet]
		[Route("ReadStockOutReturnsByStoreId")]
		public async Task<IActionResult> ReadStockOutReturnsByStoreId(DataSourceLoadOptions loadOptions, int storeId, int? clientId, int stockTypeId, int stockOutReturnHeaderId)
		{
			try
			{
				var data = await DataSourceLoader.LoadAsync(_stockOutReturnHeaderService.GetStockOutReturnHeadersByStoreId(storeId, clientId, stockTypeId, stockOutReturnHeaderId), loadOptions);
				return Ok(data);
			}
			catch
			{
				var model = _stockOutReturnHeaderService.GetStockOutReturnHeadersByStoreId(storeId, clientId, stockTypeId, stockOutReturnHeaderId);
				var data = DataSourceLoader.Load(model.ToList(), loadOptions);
				return Ok(data);
			}
		}

		[HttpGet]
		[Route("GetStockOutReturnFromStockOut")]
		public async Task<IActionResult> GetStockOutReturnFromStockOut(int stockOutHeaderId)
		{
			var data = await _stockOutReturnHandlingService.GetStockOutReturnFromStockOut(stockOutHeaderId);
			return Ok(data);
		}

		[HttpGet]
		[Route("GetStockOutReturnCode")]
		public async Task<IActionResult> GetStockOutReturnCode(int storeId, DateTime documentDate, int stockTypeId)
		{
			var data = await _stockOutReturnHeaderService.GetStockOutReturnCode(storeId, documentDate, stockTypeId);
			return Ok(data);
		}

		[HttpGet]
		[Route("GetRequestData")]
		public async Task<IActionResult> GetRequestData(int requestId)
		{
			var data = await _handleApprovalRequestService.GetRequestData(requestId);
			if (data != null && data.CanBeConverted<StockOutReturnDto>())
			{
				var stockOutReturn = data.ConvertToType<StockOutReturnDto>();
				if (stockOutReturn?.StockOutReturnHeader == null) return Ok(new StockOutReturnDto());
				
				var userCanLook = await _httpContextAccessor.UserCanLook(0, stockOutReturn.StockOutReturnHeader.StoreId);
				if (!userCanLook) return Ok(new StockOutReturnDto());

				var stockOutReturnHeader = stockOutReturn.StockOutReturnHeader;
				stockOutReturn!.StockOutReturnDetails = await _stockOutReturnHandlingService.ModifyStockOutReturnDetailsWithLiveAvailableQuantity(
					stockOutReturnHeader.StockOutReturnHeaderId,
					stockOutReturnHeader.StockOutHeaderId,
					stockOutReturnHeader.SalesInvoiceHeaderId, stockOutReturn.StockOutReturnDetails);

				return Ok(stockOutReturn);
			}
			return Ok(new StockOutReturnDto());
		}

		[HttpGet("{id:int}")]
		public async Task<IActionResult> GetStockOutReturn(int id)
		{
			var stockOutReturn = await _stockOutReturnHandlingService.GetStockOutReturn(id);
			var userCanLook = await _httpContextAccessor.UserCanLook(0, stockOutReturn.StockOutReturnHeader?.StoreId ?? 0);
			return Ok(userCanLook ? stockOutReturn : new StockOutReturnDto());
		}

		[HttpGet]
		[Route("GetStockOutReturnDetail")]
		public async Task<IActionResult> GetStockOutReturnDetail(int id)
		{
			var stockOutReturnDetail = await _stockOutReturnHandlingService.GetStockOutReturnDetailsCalculated(id);
			return Ok(stockOutReturnDetail);
		}

		[HttpGet]
		[Route("GetStockOutReturnHeader")]
		public async Task<IActionResult> GetStockOutReturnHeader(int id)
		{
			var stockOutReturnHeader = await _stockOutReturnHeaderService.GetStockOutReturnHeaderById(id);
			return Ok(stockOutReturnHeader);
		}

		[HttpPost]
		[Route("Save")]
		public async Task<IActionResult> Save([FromBody] StockOutReturnDto model, int requestId)
		{
			var response = new ResponseDto();
			try
			{
				if (model.StockOutReturnHeader != null)
				{
					var hasApprove = await _approvalSystemService.IsMenuHasApprove(MenuCodeData.StockOutReturnFromReservation);
					if (hasApprove.HasApprove)
					{
						if (hasApprove.OnAdd && model.StockOutReturnHeader!.StockOutReturnHeaderId == 0)
						{
							response = await SendApproveRequest(model, null, requestId, 0, false);
						}
						else if (hasApprove.OnEdit && model.StockOutReturnHeader!.StockOutReturnHeaderId > 0)
						{
							var oldValue = await _stockOutReturnHandlingService.GetStockOutReturn(model.StockOutReturnHeader!.StockOutReturnHeaderId);
							response = await SendApproveRequest(model, oldValue, requestId, model.StockOutReturnHeader!.StockOutReturnHeaderId, false);
						}
						else
						{
							response = await SaveStockOutReturn(model, hasApprove.HasApprove, false, requestId);
						}
					}
					else
					{
						response = await SaveStockOutReturn(model, hasApprove.HasApprove, false, requestId);
					}
				}
				else
				{
					response.Message = "Header should not be null";
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

		private async Task<ResponseDto> SaveStockOutReturn(StockOutReturnDto model, bool hasApprove, bool approved, int? requestId)
		{
			await _databaseTransaction.BeginTransaction();
			var response = await _stockOutReturnFromReservationService.SaveStockOutReturnFromReservation(model, hasApprove, approved, requestId);
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
		[Route("DeleteStockOutReturn")]
		public async Task<IActionResult> DeleteStockOutReturn(int stockOutReturnHeaderId, int requestId)
		{
			var response = new ResponseDto();
			try
			{
				var model = await _stockOutReturnHandlingService.GetStockOutReturn(stockOutReturnHeaderId);
				if (model.StockOutReturnHeader != null)
				{
					var hasApprove = await _approvalSystemService.IsMenuHasApprove(MenuCodeData.StockOutReturnFromReservation);
					if (hasApprove.HasApprove && hasApprove.OnDelete)
					{
						response = await SendApproveRequest(null, model, requestId, stockOutReturnHeaderId, true);
					}
					else
					{
						await _databaseTransaction.BeginTransaction();
						response = await _stockOutReturnFromReservationService.DeleteStockOutReturnFromReservation(stockOutReturnHeaderId, MenuCodeData.StockOutReturnFromReservation);
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
				else
				{
					response.Id = stockOutReturnHeaderId;
					response.Message = await _genericMessageService.GetMessage(MenuCodeData.StockOutReturnFromReservation, GenericMessageData.NotFound); //must use a default menuCode here
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

		private async Task<ResponseDto> SendApproveRequest([FromQuery] StockOutReturnDto? newModel, [FromQuery] StockOutReturnDto? oldModel, int requestId, int stockOutReturnHeaderId, bool isDelete)
		{
			var changes = new List<RequestChangesDto>();
			var menuCode = MenuCodeData.StockOutReturnFromReservation;
			var requestTypeId = isDelete ? ApproveRequestTypeData.Delete : (stockOutReturnHeaderId == 0 ? ApproveRequestTypeData.Add : ApproveRequestTypeData.Edit);
			if (requestTypeId == ApproveRequestTypeData.Edit && oldModel != null)
			{
				changes = _stockOutReturnService.GetStockOutReturnRequestChanges(oldModel, newModel!);
			}

			var request = new NewApproveRequestDto()
			{
				RequestId = requestId,
				MenuCode = (short)menuCode,
				ApproveRequestTypeId = requestTypeId,
				ReferenceId = stockOutReturnHeaderId,
				ReferenceCode = stockOutReturnHeaderId.ToString(),
				OldValue = oldModel,
				NewValue = newModel,
				Changes = changes
			};
			await _databaseTransaction.BeginTransaction();

			var validationResult = await CheckStockOutReturnIsValid(newModel, oldModel, menuCode);
			if (validationResult.Success == false)
			{
				await _databaseTransaction.Rollback();
				return validationResult;
			}

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

		private async Task<ResponseDto> CheckStockOutReturnIsValid(StockOutReturnDto? newModel, StockOutReturnDto? oldModel, int menuCode)
		{
			if (newModel != null)
			{
				var parentMenuCode = await _stockOutReturnHandlingService.GetParentMenuCode(newModel.StockOutReturnHeader!.StockOutHeaderId, newModel.StockOutReturnHeader!.SalesInvoiceHeaderId);
				var grandParentMenuCode = await _stockOutReturnHandlingService.GetGrandParentMenuCode(newModel.StockOutReturnHeader!.StockOutHeaderId);
				return await _stockOutReturnHandlingService.CheckStockOutReturnIsValidForSave(newModel!, menuCode, parentMenuCode, grandParentMenuCode);
			}
			else if (oldModel != null)
			{
				var parentMenuCode = await _stockOutReturnHandlingService.GetParentMenuCode(oldModel.StockOutReturnHeader!.StockOutHeaderId, oldModel.StockOutReturnHeader!.SalesInvoiceHeaderId);
				var grandParentMenuCode = await _stockOutReturnHandlingService.GetGrandParentMenuCode(oldModel.StockOutReturnHeader!.StockOutHeaderId);

				return await _stockOutReturnHandlingService.CheckStockOutReturnIsValidForDelete(
					oldModel.StockOutReturnHeader!.StockOutReturnHeaderId,
					oldModel.StockOutReturnHeader!.StoreId,
					oldModel.StockOutReturnHeader!.StockOutHeaderId,
					oldModel.StockOutReturnHeader!.IsEnded,
					oldModel.StockOutReturnHeader!.IsBlocked,
					menuCode,
					parentMenuCode,
					grandParentMenuCode);
			}
			else
			{
				return new ResponseDto { Success = true };
			}
		}
	}
}
