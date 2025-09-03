using DevExtreme.AspNet.Data;
using Compound.API.Models;
using Compound.CoreOne.Contracts.Reservation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Sales.CoreOne.Contracts;
using Sales.CoreOne.Models.Dtos.ViewModels;
using Sales.CoreOne.Models.StaticData;
using Sales.Service.Services;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Approval;
using Shared.CoreOne.Contracts.Menus;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Database;
using Shared.Helper.Models.Dtos;
using Shared.Helper.Extensions;
using Shared.Helper.Identity;
using System.Diagnostics;

namespace Compound.API.Controllers.Reservation
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class ReservationInvoicesController : ControllerBase
	{
		private readonly IReservationInvoiceService _reservationInvoiceService;
		private readonly IDatabaseTransaction _databaseTransaction;
		private readonly ISalesInvoiceService _salesInvoiceService;
		private readonly IApprovalSystemService _approvalSystemService;
		private readonly IStringLocalizer<HandelException> _exLocalizer;
		private readonly IHandleApprovalRequestService _handleApprovalRequestService;
		private readonly IGenericMessageService _genericMessageService;
		private readonly ISalesInvoiceHeaderService _salesInvoiceHeaderService;
		private readonly ISalesInvoiceHandlingService _salesInvoiceHandlingService;
		private readonly ISalesInvoiceDetailService _salesInvoiceDetailService;
		private readonly IGetSalesInvoiceSettleValueService _getSalesInvoiceSettleValueService;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public ReservationInvoicesController(IReservationInvoiceService reservationInvoiceService, IDatabaseTransaction databaseTransaction, ISalesInvoiceService salesInvoiceService, IApprovalSystemService approvalSystemService, IStringLocalizer<HandelException> exLocalizer, IHandleApprovalRequestService handleApprovalRequestService, IGenericMessageService genericMessageService, ISalesInvoiceHeaderService salesInvoiceHeaderService, ISalesInvoiceHandlingService salesInvoiceHandlingService, ISalesInvoiceDetailService salesInvoiceDetailService, IGetSalesInvoiceSettleValueService getSalesInvoiceSettleValueService, IHttpContextAccessor httpContextAccessor)
		{
			_reservationInvoiceService = reservationInvoiceService;
			_databaseTransaction = databaseTransaction;
			_salesInvoiceService = salesInvoiceService;
			_approvalSystemService = approvalSystemService;
			_exLocalizer = exLocalizer;
			_handleApprovalRequestService = handleApprovalRequestService;
			_genericMessageService = genericMessageService;
			_salesInvoiceHeaderService = salesInvoiceHeaderService;
			_salesInvoiceHandlingService = salesInvoiceHandlingService;
			_salesInvoiceDetailService = salesInvoiceDetailService;
			_getSalesInvoiceSettleValueService = getSalesInvoiceSettleValueService;
			_httpContextAccessor = httpContextAccessor;
		}

		[HttpGet]
		[Route("ReadSalesInvoices")]
		public async Task<IActionResult> ReadSalesInvoices(DataSourceLoadOptions loadOptions, int menuCode)
		{
			try
			{
				var data = await DataSourceLoader.LoadAsync(_salesInvoiceHeaderService.GetUserSalesInvoiceHeaders(menuCode), loadOptions);
				return Ok(data);
			}
			catch
			{
				var model = _salesInvoiceHeaderService.GetUserSalesInvoiceHeaders(menuCode);
				var data = DataSourceLoader.Load(model.ToList(), loadOptions);
				return Ok(data);
			}
		}

		[HttpGet]
		[Route("ReadSalesInvoicesByStoreId")]
		public async Task<IActionResult> ReadSalesInvoicesByStoreId(DataSourceLoadOptions loadOptions, int storeId, int? clientId, int salesInvoiceHeaderId)
		{
			try
			{
				var data = await DataSourceLoader.LoadAsync(_salesInvoiceHeaderService.GetSalesInvoiceHeadersByStoreId(storeId, clientId, salesInvoiceHeaderId, true), loadOptions);
				return Ok(data);
			}
			catch
			{
				var model = _salesInvoiceHeaderService.GetSalesInvoiceHeadersByStoreId(storeId, clientId, salesInvoiceHeaderId, true);
				var data = DataSourceLoader.Load(model.ToList(), loadOptions);
				return Ok(data);
			}
		}

		[HttpGet]
		[Route("ReadSalesInvoicesByStoreIdAndMenuCode")]
		public async Task<IActionResult> ReadSalesInvoicesByStoreIdAndMenuCode(DataSourceLoadOptions loadOptions, int storeId, int? clientId, int menuCode, int salesInvoiceHeaderId)
		{
			try
			{
				var data = await DataSourceLoader.LoadAsync(_salesInvoiceHandlingService.GetSalesInvoiceHeadersByStoreIdAndMenuCode(storeId, clientId, menuCode, salesInvoiceHeaderId), loadOptions);
				return Ok(data);
			}
			catch
			{
				var model = _salesInvoiceHandlingService.GetSalesInvoiceHeadersByStoreIdAndMenuCode(storeId, clientId, menuCode, salesInvoiceHeaderId);
				var data = DataSourceLoader.Load(model.ToList(), loadOptions);
				return Ok(data);
			}
		}

		[HttpGet]
		[Route("GetDefaultValidReturnDate")]
		public async Task<ActionResult<ExpirationDaysAndDateDto>> GetDefaultValidReturnDate(int storeId)
		{
			return Ok(await _salesInvoiceHandlingService.GetDefaultValidReturnDate(storeId));
		}

		[HttpGet]
		[Route("GetAvailableValueForClientCreditMemo")]
		public async Task<IActionResult> GetAvailableValueForClientCreditMemo(int salesInvoiceHeaderId, int clientCreditMemoId)
		{
			var availableValue = await _getSalesInvoiceSettleValueService.GetSalesInvoiceValueMinusSettledValue(salesInvoiceHeaderId, clientCreditMemoId);
			return Ok(availableValue);
		}

		[HttpGet]
		[Route("GetSalesInvoiceCode")]
		public async Task<IActionResult> GetSalesInvoiceCode(int storeId, DateTime documentDate, bool creditPayment, int? sellerId)
		{
			var data = await _salesInvoiceHeaderService.GetSalesInvoiceCode(storeId, documentDate, true, true, creditPayment, sellerId);
			return Ok(data);
		}

		[HttpGet]
		[Route("GetRequestData")]
		public async Task<IActionResult> GetRequestData(int requestId)
		{
			var data = await _handleApprovalRequestService.GetRequestData(requestId);
			if (data != null && data.CanBeConverted<SalesInvoiceDto>())
			{
				var salesInvoice = data.ConvertToType<SalesInvoiceDto>();
				if (salesInvoice!.SalesInvoiceHeader == null) return Ok(new SalesInvoiceDto());
				
				var userCanLook = await _httpContextAccessor.UserCanLook(0, salesInvoice.SalesInvoiceHeader.StoreId);
				if (!userCanLook) return Ok(new SalesInvoiceDto());

				if (salesInvoice!.SalesInvoiceHeader!.CreditPayment == false)
				{
					salesInvoice!.SalesInvoiceCollections = await _salesInvoiceService.AddNonincludedPaymentMethods(salesInvoice.SalesInvoiceCollections, salesInvoice.SalesInvoiceHeader!.StoreId);
				}

				await _salesInvoiceService.ModifySalesInvoiceDetailsWithStoreIdAndAvaialbleBalance(salesInvoice.SalesInvoiceHeader.SalesInvoiceHeaderId, salesInvoice.SalesInvoiceHeader.StoreId, salesInvoice.SalesInvoiceDetails, true, false);
                await _salesInvoiceService.ModifySalesInvoiceCreditLimits(salesInvoice.SalesInvoiceHeader);

				return Ok(salesInvoice);
			}
			return Ok(new SalesInvoiceDto());
		}

		[HttpGet("{id:int}")]
		public async Task<IActionResult> GetSalesInvoice(int id)
		{
			var salesInvoice = await _salesInvoiceService.GetSalesInvoice(id);
			var userCanLook = await _httpContextAccessor.UserCanLook(0, salesInvoice.SalesInvoiceHeader?.StoreId ?? 0);
			return Ok(userCanLook ? salesInvoice : new SalesInvoiceDto());
		}

		[HttpGet]
		[Route("GetSalesInvoiceHeader")]
		public async Task<IActionResult> GetSalesInvoiceHeader(int id)
		{
			var salesInvoiceHeader = await _salesInvoiceHeaderService.GetSalesInvoiceHeaderById(id);
			return Ok(salesInvoiceHeader);
		}

		[HttpGet]
		[Route("GetSalesInvoiceDetail")]
		public async Task<IActionResult> GetSalesInvoiceDetail(int id)
		{
			var salesInvoiceDetail = await _salesInvoiceDetailService.GetSalesInvoiceDetails(id);
			return Ok(salesInvoiceDetail);
		}

		[HttpPost]
		[Route("Save")]
		public async Task<IActionResult> Save([FromBody] SalesInvoiceDto model, int requestId)
		{
			var response = new ResponseDto();
			try
			{
				if (model.SalesInvoiceHeader != null)
				{
					var hasApprove = await _approvalSystemService.IsMenuHasApprove(SalesInvoiceMenuCodeHelper.GetMenuCode(model.SalesInvoiceHeader));
					if (hasApprove.HasApprove)
					{
						if (hasApprove.OnAdd && model.SalesInvoiceHeader!.SalesInvoiceHeaderId == 0)
						{
							response = await SendApproveRequest(model, null, requestId, 0, false);
						}
						else if (hasApprove.OnEdit && model.SalesInvoiceHeader!.SalesInvoiceHeaderId > 0)
						{
							var oldValue = await _salesInvoiceService.GetSalesInvoice(model.SalesInvoiceHeader!.SalesInvoiceHeaderId);
							response = await SendApproveRequest(model, oldValue, requestId, model.SalesInvoiceHeader!.SalesInvoiceHeaderId, false);
						}
						else
						{
							response = await SaveReservationInvoiceInternal(model, hasApprove.HasApprove, false, requestId);
						}
					}
					else
					{
						response = await SaveReservationInvoiceInternal(model, hasApprove.HasApprove, false, requestId);
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

		[HttpPost]
		[Route("SaveReservationInvoice")]
		private async Task<ResponseDto> SaveReservationInvoiceInternal(SalesInvoiceDto model, bool hasApprove, bool approved, int? requestId)
		{
			await _databaseTransaction.BeginTransaction();
			var response = await _reservationInvoiceService.SaveReservationInvoice(model, hasApprove, approved, requestId);
			if (response.Success == true)
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
		[Route("DeleteSalesInvoice")]
		public async Task<IActionResult> DeleteSalesInvoice(int salesInvoiceHeaderId, int requestId)
		{
			var response = new ResponseDto();
			try
			{
				var model = await _salesInvoiceService.GetSalesInvoice(salesInvoiceHeaderId);
				if (model.SalesInvoiceHeader != null)
				{
					var menuCode = SalesInvoiceMenuCodeHelper.GetMenuCode(model.SalesInvoiceHeader);
					var hasApprove = await _approvalSystemService.IsMenuHasApprove(menuCode);
					if (hasApprove.HasApprove && hasApprove.OnDelete)
					{
						response = await SendApproveRequest(null, model, requestId, salesInvoiceHeaderId, true);
					}
					else
					{
						await _databaseTransaction.BeginTransaction();
						response = await _reservationInvoiceService.DeleteReservationInvoice(salesInvoiceHeaderId, menuCode);
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
					response.Id = salesInvoiceHeaderId;
					response.Message = await _genericMessageService.GetMessage(MenuCodeData.CashReservationInvoice, GenericMessageData.NotFound);
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

		private async Task<ResponseDto> SendApproveRequest(SalesInvoiceDto? newModel, SalesInvoiceDto? oldModel, int requestId, int salesInvoiceHeaderId, bool isDelete)
		{
			var changes = new List<RequestChangesDto>();
			var menuCode = 0;
			var requestTypeId = isDelete ? ApproveRequestTypeData.Delete : (salesInvoiceHeaderId == 0 ? ApproveRequestTypeData.Add : ApproveRequestTypeData.Edit);

			if (requestTypeId == ApproveRequestTypeData.Edit || requestTypeId == ApproveRequestTypeData.Delete)
			{
				if (oldModel != null)
				{
					if (newModel != null)
					{
						changes = _salesInvoiceService.GetSalesInvoiceRequestChanges(oldModel, newModel);
					}

					if (oldModel.SalesInvoiceHeader != null)
					{
						menuCode = SalesInvoiceMenuCodeHelper.GetMenuCode(oldModel.SalesInvoiceHeader);
					}
				}
			}
			else
			{
				if (newModel != null)
				{
					if (newModel.SalesInvoiceHeader != null)
					{
						menuCode = SalesInvoiceMenuCodeHelper.GetMenuCode(newModel.SalesInvoiceHeader);
					}
				}
			}

			var request = new NewApproveRequestDto()
			{
				RequestId = requestId,
				MenuCode = (short)menuCode,
				ApproveRequestTypeId = requestTypeId,
				ReferenceId = salesInvoiceHeaderId,
				ReferenceCode = salesInvoiceHeaderId.ToString(),
				OldValue = oldModel,
				NewValue = newModel,
				Changes = changes
			};
			await _databaseTransaction.BeginTransaction();

			var validationResult = await CheckSalesInvoiceIsValid(newModel, oldModel, menuCode);
			if (validationResult.Success == false)
			{
				await _databaseTransaction.Rollback();
				return validationResult;
			}

			var response = await _handleApprovalRequestService.SaveRequest(request);
			await _databaseTransaction.Commit();
			return response;
		}

		private async Task<ResponseDto> CheckSalesInvoiceIsValid(SalesInvoiceDto? newModel, SalesInvoiceDto? oldModel, int menuCode)
		{
			if (newModel != null)
			{
				return await _salesInvoiceHandlingService.CheckSalesInvoiceIsValidForSave(newModel!, menuCode);
			}
			else if (oldModel != null)
			{
				return await _salesInvoiceHandlingService.CheckSalesInvoiceIsValidForDelete(
					oldModel.SalesInvoiceHeader!.IsEnded,
					oldModel.SalesInvoiceHeader!.IsBlocked,
					oldModel.SalesInvoiceHeader!.HasSettlement,
					menuCode);
			}
			else
			{
				return new ResponseDto { Success = true };
			}
		}
	}
}
