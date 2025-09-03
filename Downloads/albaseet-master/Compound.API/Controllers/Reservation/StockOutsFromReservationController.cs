using Microsoft.AspNetCore.Http;
using Compound.API.Models;
using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Sales.CoreOne.Contracts;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Approval;
using Shared.CoreOne.Contracts.Menus;
using Shared.Helper.Database;
using Sales.CoreOne.Models.Dtos.ViewModels;
using Shared.Helper.Extensions;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Models.Dtos;
using Compound.CoreOne.Contracts.Reservation;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Microsoft.AspNetCore.Authorization;
using Shared.Helper.Identity;

namespace Compound.API.Controllers.Reservation
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class StockOutsFromReservationController : ControllerBase
	{
		private readonly IStockOutHandlingService _stockOutHandlingService;
		private readonly IStockOutService _stockOutService;
		private readonly IStockOutHeaderService _stockOutHeaderService;
		private readonly IStockOutDetailService _stockOutDetailService;
		private readonly IDatabaseTransaction _databaseTransaction;
		private readonly IStringLocalizer<HandelException> _exLocalizer;
		private readonly IGenericMessageService _genericMessageService;
		private readonly IApprovalSystemService _approvalSystemService;
		private readonly IHandleApprovalRequestService _handleApprovalRequestService;
		private readonly IStockOutFromReservationService _stockOutFromReservationService;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public StockOutsFromReservationController(IStockOutHandlingService stockOutHandlingService, IStockOutService stockOutService, IStockOutHeaderService stockOutHeaderService, IStockOutDetailService stockOutDetailService, IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exLocalizer, IGenericMessageService genericMessageService, IApprovalSystemService approvalSystemService, IHandleApprovalRequestService handleApprovalRequestService, IStockOutFromReservationService stockOutFromReservationService, IHttpContextAccessor httpContextAccessor)
		{
			_stockOutHandlingService = stockOutHandlingService;
			_stockOutService = stockOutService;
			_stockOutHeaderService = stockOutHeaderService;
			_stockOutDetailService = stockOutDetailService;
			_databaseTransaction = databaseTransaction;
			_exLocalizer = exLocalizer;
			_genericMessageService = genericMessageService;
			_approvalSystemService = approvalSystemService;
			_handleApprovalRequestService = handleApprovalRequestService;
			_stockOutFromReservationService = stockOutFromReservationService;
			_httpContextAccessor = httpContextAccessor;
		}

		[HttpGet]
		[Route("ReadStockOuts")]
		public async Task<IActionResult> ReadStockOuts(DataSourceLoadOptions loadOptions, int stockTypeId)
		{
			try
			{
				var data = await DataSourceLoader.LoadAsync(_stockOutHeaderService.GetUserStockOutHeaders(stockTypeId), loadOptions);
				return Ok(data);
			}
			catch
			{
				var model = _stockOutHeaderService.GetUserStockOutHeaders(stockTypeId);
				var data = DataSourceLoader.Load(model.ToList(), loadOptions);
				return Ok(data);
			}
		}

		[HttpGet]
		[Route("ReadStockOutsByStoreId")]
		public async Task<IActionResult> ReadStockOutsByStoreId(DataSourceLoadOptions loadOptions, int storeId, int? clientId, int stockTypeId, int stockOutHeaderId)
		{
			try
			{
				var data = await DataSourceLoader.LoadAsync(_stockOutHandlingService.GetStockOutHeadersByStoreId(storeId, clientId, stockTypeId, stockOutHeaderId), loadOptions);
				return Ok(data);
			}
			catch
			{
				var model = _stockOutHandlingService.GetStockOutHeadersByStoreId(storeId, clientId, stockTypeId, stockOutHeaderId);
				var data = DataSourceLoader.Load(model.ToList(), loadOptions);
				return Ok(data);
			}
		}

		[HttpGet]
		[Route("GetStockOutFromSalesInvoice")]
		public async Task<IActionResult> GetStockOutFromSalesInvoice(int salesInvoiceHeaderId)
		{
			var data = await _stockOutHandlingService.GetStockOutFromSalesInvoice(salesInvoiceHeaderId);
			return Ok(data);
		}

		[HttpGet]
		[Route("GetStockOutCode")]
		public async Task<IActionResult> GetStockOutCode(int storeId, DateTime documentDate, int stockTypeId)
		{
			var data = await _stockOutHeaderService.GetStockOutCode(storeId, documentDate, stockTypeId);
			return Ok(data);
		}

		[HttpGet]
		[Route("GetRequestData")]
		public async Task<IActionResult> GetRequestData(int requestId)
		{
			var data = await _handleApprovalRequestService.GetRequestData(requestId);
			if (data != null && data.CanBeConverted<StockOutDto>())
			{
				var stockOut = data.ConvertToType<StockOutDto>();
				if (stockOut?.StockOutHeader == null) return Ok(new StockOutDto());
				
				var userCanLook = await _httpContextAccessor.UserCanLook(0, stockOut.StockOutHeader.StoreId);
				if (!userCanLook) return Ok(new StockOutDto());

				var stockOutHeader = stockOut.StockOutHeader;
				stockOut!.StockOutDetails = await _stockOutHandlingService.ModifyStockOutDetailsWithLiveAvailableQuantity(
					stockOutHeader.StockOutHeaderId,
					stockOutHeader.ProformaInvoiceHeaderId,
					stockOutHeader.SalesInvoiceHeaderId, stockOut.StockOutDetails);
				
				//no need to show available balance here since it will not affect main store

				return Ok(stockOut);
			}
			return Ok(new StockOutDto());
		}	

		[HttpGet("{id:int}")]
		public async Task<IActionResult> GetStockOut(int id)
		{
			var stockOut = await _stockOutHandlingService.GetStockOut(id);
			var userCanLook = await _httpContextAccessor.UserCanLook(0, stockOut.StockOutHeader?.StoreId ?? 0);
			return Ok(userCanLook ? stockOut : new StockOutDto());
		}

		[HttpGet]
		[Route("GetStockOutDetail")]
		public async Task<IActionResult> GetStockOutDetail(int id)
		{
			var stockOutDetail = await _stockOutHandlingService.GetStockOutDetailsCalculated(id);
			return Ok(stockOutDetail);
		}

		[HttpGet]
		[Route("GetStockOutHeader")]
		public async Task<IActionResult> GetStockOutHeader(int id)
		{
			var stockOutHeader = await _stockOutHeaderService.GetStockOutHeaderById(id);
			return Ok(stockOutHeader);
		}

		[HttpPost]
		[Route("Save")]
		public async Task<IActionResult> Save([FromBody] StockOutDto model, int requestId)
		{
			var response = new ResponseDto();
			try
			{
				if (model.StockOutHeader != null)
				{
					var hasApprove = await _approvalSystemService.IsMenuHasApprove(MenuCodeData.StockOutFromReservation);
					if (hasApprove.HasApprove)
					{
						if (hasApprove.OnAdd && model.StockOutHeader!.StockOutHeaderId == 0)
						{
							response = await SendApproveRequest(model, null, requestId, 0, false);
						}
						else if (hasApprove.OnEdit && model.StockOutHeader!.StockOutHeaderId > 0)
						{
							var oldValue = await _stockOutHandlingService.GetStockOut(model.StockOutHeader!.StockOutHeaderId);
							response = await SendApproveRequest(model, oldValue, requestId, model.StockOutHeader!.StockOutHeaderId, false);
						}
						else
						{
							response = await SaveStockOut(model, hasApprove.HasApprove, false, requestId);
						}
					}
					else
					{
						response = await SaveStockOut(model, hasApprove.HasApprove, false, requestId);
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

		private async Task<ResponseDto> SaveStockOut(StockOutDto model, bool hasApprove, bool approved, int? requestId)
		{
			await _databaseTransaction.BeginTransaction();
			var response = await _stockOutFromReservationService.SaveStockOutFromReservation(model, hasApprove, approved, requestId);
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
		[Route("DeleteStockOut")]
		public async Task<IActionResult> DeleteStockOut(int stockOutHeaderId, int requestId)
		{
			var response = new ResponseDto();
			try
			{
				var model = await _stockOutHandlingService.GetStockOut(stockOutHeaderId);
				if (model.StockOutHeader != null)
				{
					var hasApprove = await _approvalSystemService.IsMenuHasApprove(MenuCodeData.StockOutFromReservation);
					if (hasApprove.HasApprove && hasApprove.OnDelete)
					{
						response = await SendApproveRequest(null, model, requestId, stockOutHeaderId, true);
					}
					else
					{
						await _databaseTransaction.BeginTransaction();
						response = await _stockOutFromReservationService.DeleteStockOutFromReservation(stockOutHeaderId, MenuCodeData.StockOutFromReservation);
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
					response.Id = stockOutHeaderId;
					response.Message = await _genericMessageService.GetMessage(MenuCodeData.StockOutFromReservation, GenericMessageData.NotFound); //must use a default menuCode here
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

		private async Task<ResponseDto> SendApproveRequest([FromQuery] StockOutDto? newModel, [FromQuery] StockOutDto? oldModel, int requestId, int stockOutHeaderId, bool isDelete)
		{
			var changes = new List<RequestChangesDto>();
			var menuCode = MenuCodeData.StockOutFromReservation;
			var requestTypeId = isDelete ? ApproveRequestTypeData.Delete : (stockOutHeaderId == 0 ? ApproveRequestTypeData.Add : ApproveRequestTypeData.Edit);
			if (requestTypeId == ApproveRequestTypeData.Edit && oldModel != null)
			{
				changes = _stockOutService.GetStockOutRequestChanges(oldModel, newModel!);
			}

			var request = new NewApproveRequestDto()
			{
				RequestId = requestId,
				MenuCode = (short)menuCode,
				ApproveRequestTypeId = requestTypeId,
				ReferenceId = stockOutHeaderId,
				ReferenceCode = stockOutHeaderId.ToString(),
				OldValue = oldModel,
				NewValue = newModel,
				Changes = changes
			};
			await _databaseTransaction.BeginTransaction();

			var validationResult = await CheckStockOutIsValid(newModel, oldModel, menuCode);
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

		private async Task<ResponseDto> CheckStockOutIsValid(StockOutDto? newModel, StockOutDto? oldModel, int menuCode)
		{
			if (newModel != null)
			{
				var parentMenuCode = await _stockOutHandlingService.GetParentMenuCode(newModel.StockOutHeader!.ProformaInvoiceHeaderId, newModel.StockOutHeader!.SalesInvoiceHeaderId);
				return await _stockOutHandlingService.CheckStockOutIsValidForSave(newModel!, menuCode, parentMenuCode);
			}
			else if (oldModel != null)
			{
				return await _stockOutHandlingService.CheckStockOutIsValidForDelete(
					oldModel.StockOutHeader!.StockOutHeaderId,
					oldModel.StockOutHeader!.ProformaInvoiceHeaderId,
					oldModel.StockOutHeader!.SalesInvoiceHeaderId,
					oldModel.StockOutHeader!.IsBlocked,
					oldModel.StockOutHeader!.IsEnded,
					menuCode);
			}
			else
			{
				return new ResponseDto { Success = true };
			}
		}
	}
}
