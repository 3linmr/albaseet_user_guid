using Compound.CoreOne.Contracts.InvoiceSettlement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Compound.API.Models;
using Accounting.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Database;
using Shared.Helper.Models.Dtos;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Approval;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Compound.Service.Services.InvoiceSettlement;
using Shared.Helper.Extensions;
using Shared.Helper.Identity;

namespace Compound.API.Controllers.InvoiceSettlement
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class PurchaseInvoiceSettlementController : ControllerBase
	{
		private readonly IPurchaseInvoiceSettlementService _purchaseInvoiceSettlementService;
		private readonly IDatabaseTransaction _databaseTransaction;
		private readonly IApprovalSystemService _approvalSystemService;
		private readonly IStringLocalizer<HandelException> _exLocalizer;
		private readonly IHandleApprovalRequestService _handleApprovalRequestService;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public PurchaseInvoiceSettlementController(IPurchaseInvoiceSettlementService saleInvoiceSettlementService, IDatabaseTransaction databaseTransaction, IApprovalSystemService approvalSystemService, IStringLocalizer<HandelException> exLocalizer, IHandleApprovalRequestService handleApprovalRequestService, IHttpContextAccessor httpContextAccessor)
		{
			_purchaseInvoiceSettlementService = saleInvoiceSettlementService;
			_databaseTransaction = databaseTransaction;
			_approvalSystemService = approvalSystemService;
			_exLocalizer = exLocalizer;
			_handleApprovalRequestService = handleApprovalRequestService;
			_httpContextAccessor = httpContextAccessor;
		}

		[HttpGet]
		[Route("GetUnSettledInvoices")]
		public async Task<IActionResult> GetUnSettledInvoices(int supplierId, int storeId)
		{
			var data = await _purchaseInvoiceSettlementService.GetUnSettledInvoices(supplierId, storeId).ToListAsync();
			return Ok(data);
		}

		[HttpGet]
		[Route("GetPurchaseInvoiceSettledValue")]
		public async Task<ActionResult<decimal>> GetPurchaseInvoiceSettledValue(int purchaseInvoiceHeaderId)
		{
			var settleValue = await _purchaseInvoiceSettlementService.GetPurchaseInvoiceSettledValue(purchaseInvoiceHeaderId);
			return Ok(settleValue);
		}

		[HttpGet]
		[Route("GetInvoicesForPaymentVoucher")]
		public async Task<IActionResult> GetInvoicesForPaymentVoucher(int paymentVoucherHeaderId)
		{
			var data = await (await _purchaseInvoiceSettlementService.GetPurchaseInvoicesForPaymentVoucher(paymentVoucherHeaderId)).ToListAsync();
			return Ok(data);
		}

		[HttpGet]
		[Route("GetPaymentVoucherWithAllUnSettledPurchaseInvoices")]
		public async Task<ActionResult<PaymentVoucherDto>> GetPaymentVoucherWithAllUnSettledPurchaseInvoices(int paymentVoucherHeaderId)
		{
			var paymentVoucher = await _purchaseInvoiceSettlementService.GetPaymentVoucherWithAllUnSettledPurchaseInvoices(paymentVoucherHeaderId);
			var userCanLook = await _httpContextAccessor.UserCanLook(0, paymentVoucher.PaymentVoucherHeader?.StoreId ?? 0);
			return Ok(userCanLook ? paymentVoucher : new PaymentVoucherDto());
		}

		[HttpGet]
		[Route("GetPaymentVoucherWithPurchaseInvoices")]
		public async Task<ActionResult<PaymentVoucherDto>> GetPaymentVoucherWithPurchaseInvoices(int paymentVoucherHeaderId)
		{
			var paymentVoucher = await _purchaseInvoiceSettlementService.GetPaymentVoucherWithPurchaseInvoices(paymentVoucherHeaderId);
			var userCanLook = await _httpContextAccessor.UserCanLook(0, paymentVoucher.PaymentVoucherHeader?.StoreId ?? 0);
			return Ok(userCanLook ? paymentVoucher : new PaymentVoucherDto());
		}

		[HttpGet]
		[Route("IsSettlementOnInvoiceStarted")]
		public async Task<ActionResult<ResponseDto>> IsSettlementOnInvoiceStarted(int purchaseInvoiceHeaderId)
		{
			return Ok(await _purchaseInvoiceSettlementService.IsSettlementOnInvoiceStarted(purchaseInvoiceHeaderId));
		}

		[HttpGet]
		[Route("GetRequestDataInfo")]
		public async Task<IActionResult> GetRequestDataInfo(int requestId)
		{
			var data = await _handleApprovalRequestService.GetRequestData(requestId);
			if (data != null && data.CanBeConverted<PaymentVoucherDto>())
			{
				var paymentVoucher = data.ConvertToType<PaymentVoucherDto>();

				var userCanLook = await _httpContextAccessor.UserCanLook(0, paymentVoucher?.PaymentVoucherHeader?.StoreId ?? 0);
				if (!userCanLook) return Ok(new PaymentVoucherDto());

				if (paymentVoucher?.PaymentVoucherHeader == null || paymentVoucher?.PaymentVoucherHeader?.SupplierId == null) return Ok(paymentVoucher);
				
				paymentVoucher.PurchaseInvoiceSettlements = await _purchaseInvoiceSettlementService.GetPurchaseInvoicesForPaymentVoucherRequest(
					paymentVoucher.PaymentVoucherHeader.PaymentVoucherHeaderId,
					(int)paymentVoucher.PaymentVoucherHeader.SupplierId,
					paymentVoucher.PaymentVoucherHeader.StoreId,
					paymentVoucher.PurchaseInvoiceSettlements,
					false);

				return Ok(paymentVoucher);
			}
			return Ok(new PaymentVoucherDto());
		}

		[HttpGet]
		[Route("GetRequestDataEdit")]
		public async Task<IActionResult> GetRequestDataEdit(int requestId)
		{
			var data = await _handleApprovalRequestService.GetRequestData(requestId);
			if (data != null && data.CanBeConverted<PaymentVoucherDto>())
			{
				var paymentVoucher = data.ConvertToType<PaymentVoucherDto>();

				var userCanLook = await _httpContextAccessor.UserCanLook(0, paymentVoucher?.PaymentVoucherHeader?.StoreId ?? 0);
				if (!userCanLook) return Ok(new PaymentVoucherDto());

				if (paymentVoucher?.PaymentVoucherHeader == null || paymentVoucher?.PaymentVoucherHeader?.SupplierId == null) return Ok(paymentVoucher);

				paymentVoucher.PurchaseInvoiceSettlements = await _purchaseInvoiceSettlementService.GetPurchaseInvoicesForPaymentVoucherRequest(
					paymentVoucher.PaymentVoucherHeader.PaymentVoucherHeaderId,
					(int)paymentVoucher.PaymentVoucherHeader.SupplierId,
					paymentVoucher.PaymentVoucherHeader.StoreId,
					paymentVoucher.PurchaseInvoiceSettlements,
					true);

				return Ok(paymentVoucher);
			}
			return Ok(new PaymentVoucherDto());
		}


		[HttpPost]
		[Route("SavePaymentVoucherWithPurchaseInvoiceSettlements")]
		public async Task<IActionResult> SavePaymentVoucherWithPurchaseInvoiceSettlements([FromBody] PaymentVoucherDto model, int requestId)
		{
			ResponseDto response = new ResponseDto();
			try
			{
				if (model.PaymentVoucherHeader != null)
				{
					var hasApprove = await _approvalSystemService.IsMenuHasApprove(MenuCodeData.PaymentVoucher);
					if (hasApprove.HasApprove)
					{
						if (hasApprove.OnAdd && model.PaymentVoucherHeader!.PaymentVoucherHeaderId == 0)
						{
							response = await SendApproveRequest(model, null, requestId, 0, false);
						}
						else if (hasApprove.OnEdit && model.PaymentVoucherHeader!.PaymentVoucherHeaderId > 0)
						{
							var oldValue = await _purchaseInvoiceSettlementService.GetPaymentVoucherWithAllUnSettledPurchaseInvoices(model.PaymentVoucherHeader!.PaymentVoucherHeaderId);
							response = await SendApproveRequest(model, oldValue, requestId, model.PaymentVoucherHeader!.PaymentVoucherHeaderId, false);
						}
						else
						{
							response = await SavePaymentVoucher(model, hasApprove.HasApprove, false, requestId);
						}
					}
					else
					{
						response = await SavePaymentVoucher(model, hasApprove.HasApprove, false, requestId);
					}
				}
				else
				{
					response.Message = "Header must not be null";
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
		private async Task<ResponseDto> SavePaymentVoucher(PaymentVoucherDto model, bool hasApprove, bool approved, int? requestId)
		{
			await _databaseTransaction.BeginTransaction();
			var response = await _purchaseInvoiceSettlementService.SavePaymentVoucherWithInvoiceSettlements(model, hasApprove, approved, requestId);
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
		[Route("DeletePaymentVoucherWithInvoiceSettlements")]
		public async Task<IActionResult> DeletePaymentVoucherWithInvoiceSettlements(int paymentVoucherId, int requestId)
		{
			ResponseDto response;
			try
			{
				var hasApprove = await _approvalSystemService.IsMenuHasApprove(MenuCodeData.PaymentVoucher);
				if (hasApprove.HasApprove && hasApprove.OnDelete)
				{
					var model = await _purchaseInvoiceSettlementService.GetPaymentVoucherWithAllUnSettledPurchaseInvoices(paymentVoucherId);
					response = await SendApproveRequest(null, model, requestId, paymentVoucherId, true);
				}
				else
				{
					await _databaseTransaction.BeginTransaction();
					response = await _purchaseInvoiceSettlementService.DeletePaymentVoucherWithInvoiceSettlements(paymentVoucherId);
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

		private async Task<ResponseDto> SendApproveRequest([FromQuery] PaymentVoucherDto? newModel, [FromQuery] PaymentVoucherDto? oldModel, int requestId, int paymentVoucherHeaderId, bool isDelete)
		{
			var changes = new List<RequestChangesDto>();
			var requestTypeId = isDelete ? ApproveRequestTypeData.Delete : (paymentVoucherHeaderId == 0 ? ApproveRequestTypeData.Add : ApproveRequestTypeData.Edit);
			if (requestTypeId == ApproveRequestTypeData.Edit)
			{
				changes = _purchaseInvoiceSettlementService.GetRequestChangesWithPurchaseInvoiceSettlements(oldModel!, newModel!);
			}

			var request = new NewApproveRequestDto()
			{
				RequestId = requestId,
				MenuCode = MenuCodeData.PaymentVoucher,
				ApproveRequestTypeId = requestTypeId,
				ReferenceId = paymentVoucherHeaderId,
				ReferenceCode = paymentVoucherHeaderId.ToString(),
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
