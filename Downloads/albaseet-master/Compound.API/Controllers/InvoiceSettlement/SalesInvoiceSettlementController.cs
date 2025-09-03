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
using Shared.Helper.Extensions;
using Shared.Helper.Identity;

namespace Compound.API.Controllers.InvoiceSettlement
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class SalesInvoiceSettlementController : ControllerBase
	{
		private readonly ISalesInvoiceSettlementService _salesInvoiceSettlementService;
		private readonly IDatabaseTransaction _databaseTransaction;
		private readonly IApprovalSystemService _approvalSystemService;
		private readonly IStringLocalizer<HandelException> _exLocalizer;
		private readonly IHandleApprovalRequestService _handleApprovalRequestService;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public SalesInvoiceSettlementController(ISalesInvoiceSettlementService saleInvoiceSettlementService, IDatabaseTransaction databaseTransaction, IApprovalSystemService approvalSystemService, IStringLocalizer<HandelException> exLocalizer, IHandleApprovalRequestService handleApprovalRequestService, IHttpContextAccessor httpContextAccessor)
		{
			_salesInvoiceSettlementService = saleInvoiceSettlementService;
			_databaseTransaction = databaseTransaction;
			_approvalSystemService = approvalSystemService;
			_exLocalizer = exLocalizer;
			_handleApprovalRequestService = handleApprovalRequestService;
			_httpContextAccessor = httpContextAccessor;
		}

		[HttpGet]
		[Route("GetUnSettledInvoices")]
		public async Task<IActionResult> GetUnSettledInvoices(int clientId, int storeId)
		{
			var data = await _salesInvoiceSettlementService.GetUnSettledInvoices(clientId, storeId).ToListAsync();
			return Ok(data);
		}

		[HttpGet]
		[Route("GetSalesInvoiceSettledValue")]
		public async Task<ActionResult<decimal>> GetSalesInvoiceSettledValue(int salesInvoiceHeaderId)
		{
			var settleValue = await _salesInvoiceSettlementService.GetSalesInvoiceSettledValue(salesInvoiceHeaderId);
			return Ok(settleValue);
		}

		[HttpGet]
		[Route("GetInvoicesForReceiptVoucher")]
		public async Task<IActionResult> GetInvoicesForReceiptVoucher(int receiptVoucherHeaderId)
		{
			var data = await (await _salesInvoiceSettlementService.GetSalesInvoicesForReceiptVoucher(receiptVoucherHeaderId)).ToListAsync();
			return Ok(data);
		}

		[HttpGet]
		[Route("GetReceiptVoucherWithAllUnSettledSalesInvoices")]
		public async Task<ActionResult<ReceiptVoucherDto>> GetReceiptVoucherWithAllUnSettledSalesInvoices(int receiptVoucherHeaderId)
		{
			var receiptVoucher = await _salesInvoiceSettlementService.GetReceiptVoucherWithAllUnSettledSalesInvoices(receiptVoucherHeaderId);
			var userCanLook = await _httpContextAccessor.UserCanLook(0, receiptVoucher?.ReceiptVoucherHeader?.StoreId ?? 0);
			return Ok(userCanLook ? receiptVoucher : new ReceiptVoucherDto());
		}

		[HttpGet]
		[Route("GetReceiptVoucherWithSalesInvoices")]
		public async Task<ActionResult<ReceiptVoucherDto>> GetReceiptVoucherWithSalesInvoices(int receiptVoucherHeaderId)
		{
			var receiptVoucher = await _salesInvoiceSettlementService.GetReceiptVoucherWithSalesInvoices(receiptVoucherHeaderId);
			var userCanLook = await _httpContextAccessor.UserCanLook(0, receiptVoucher?.ReceiptVoucherHeader?.StoreId ?? 0);
			return Ok(userCanLook ? receiptVoucher : new ReceiptVoucherDto());
		}

		[HttpGet]
		[Route("IsSettlementOnInvoiceStarted")]
		public async Task<ActionResult<ResponseDto>> IsSettlementOnInvoiceStarted(int salesInvoiceHeaderId)
		{
			return Ok(await _salesInvoiceSettlementService.IsSettlementOnInvoiceStarted(salesInvoiceHeaderId));
		}

		[HttpGet]
		[Route("GetRequestDataInfo")]
		public async Task<IActionResult> GetRequestDataInfo(int requestId)
		{
			var data = await _handleApprovalRequestService.GetRequestData(requestId);
			if (data != null && data.CanBeConverted<ReceiptVoucherDto>())
			{
				var receiptVoucher = data.ConvertToType<ReceiptVoucherDto>();
				
				var userCanLook = await _httpContextAccessor.UserCanLook(0, receiptVoucher?.ReceiptVoucherHeader?.StoreId ?? 0);
				if (!userCanLook) return Ok(new ReceiptVoucherDto());

				if (receiptVoucher?.ReceiptVoucherHeader == null || receiptVoucher?.ReceiptVoucherHeader?.ClientId == null) return Ok(receiptVoucher);

				receiptVoucher.SalesInvoiceSettlements = await _salesInvoiceSettlementService.GetSalesInvoicesForReceiptVoucherRequest(
					receiptVoucher.ReceiptVoucherHeader.ReceiptVoucherHeaderId,
					(int)receiptVoucher.ReceiptVoucherHeader.ClientId,
					receiptVoucher.ReceiptVoucherHeader.StoreId,
					receiptVoucher.SalesInvoiceSettlements,
					false);

				return Ok(receiptVoucher);
			}
			return Ok(new ReceiptVoucherDto());
		}

		[HttpGet]
		[Route("GetRequestDataEdit")]
		public async Task<IActionResult> GetRequestDataEdit(int requestId)
		{
			var data = await _handleApprovalRequestService.GetRequestData(requestId);
			if (data != null && data.CanBeConverted<ReceiptVoucherDto>())
			{
				var receiptVoucher = data.ConvertToType<ReceiptVoucherDto>();
				
				var userCanLook = await _httpContextAccessor.UserCanLook(0, receiptVoucher?.ReceiptVoucherHeader?.StoreId ?? 0);
				if (!userCanLook) return Ok(new ReceiptVoucherDto());

				if (receiptVoucher?.ReceiptVoucherHeader == null || receiptVoucher?.ReceiptVoucherHeader?.ClientId == null) return Ok(receiptVoucher);

				receiptVoucher.SalesInvoiceSettlements = await _salesInvoiceSettlementService.GetSalesInvoicesForReceiptVoucherRequest(
					receiptVoucher.ReceiptVoucherHeader.ReceiptVoucherHeaderId,
					(int)receiptVoucher.ReceiptVoucherHeader.ClientId,
					receiptVoucher.ReceiptVoucherHeader.StoreId,
					receiptVoucher.SalesInvoiceSettlements,
					true);

				return Ok(receiptVoucher);
			}
			return Ok(new ReceiptVoucherDto());
		}

		[HttpPost]
		[Route("SaveReceiptVoucherWithSalesInvoiceSettlements")]
		public async Task<IActionResult> SaveReceiptVoucherWithSalesInvoiceSettlements([FromBody] ReceiptVoucherDto model, int requestId)
		{
			ResponseDto response = new ResponseDto();
			try
			{
				if (model.ReceiptVoucherHeader != null)
				{
					var hasApprove = await _approvalSystemService.IsMenuHasApprove(MenuCodeData.ReceiptVoucher);
					if (hasApprove.HasApprove)
					{
						if (hasApprove.OnAdd && model.ReceiptVoucherHeader!.ReceiptVoucherHeaderId == 0)
						{
							response = await SendApproveRequest(model, null, requestId, 0, false);
						}
						else if (hasApprove.OnEdit && model.ReceiptVoucherHeader!.ReceiptVoucherHeaderId > 0)
						{
							var oldValue = await _salesInvoiceSettlementService.GetReceiptVoucherWithAllUnSettledSalesInvoices(model.ReceiptVoucherHeader!.ReceiptVoucherHeaderId);
							response = await SendApproveRequest(model, oldValue, requestId, model.ReceiptVoucherHeader!.ReceiptVoucherHeaderId, false);
						}
						else
						{
							response = await SaveReceiptVoucher(model, hasApprove.HasApprove, false, requestId);
						}
					}
					else
					{
						response = await SaveReceiptVoucher(model, hasApprove.HasApprove, false, requestId);
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
		private async Task<ResponseDto> SaveReceiptVoucher(ReceiptVoucherDto model, bool hasApprove, bool approved, int? requestId)
		{
			await _databaseTransaction.BeginTransaction();
			var response = await _salesInvoiceSettlementService.SaveReceiptVoucherWithInvoiceSettlements(model, hasApprove, approved, requestId);
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
		[Route("DeleteReceiptVoucherWithInvoiceSettlements")]
		public async Task<IActionResult> DeleteReceiptVoucherWithInvoiceSettlements(int receiptVoucherId, int requestId)
		{
			ResponseDto response;
			try
			{
				var hasApprove = await _approvalSystemService.IsMenuHasApprove(MenuCodeData.ReceiptVoucher);
				if (hasApprove.HasApprove && hasApprove.OnDelete)
				{
					var model = await _salesInvoiceSettlementService.GetReceiptVoucherWithAllUnSettledSalesInvoices(receiptVoucherId);
					response = await SendApproveRequest(null, model, requestId, receiptVoucherId, true);
				}
				else
				{
					await _databaseTransaction.BeginTransaction();
					response = await _salesInvoiceSettlementService.DeleteReceiptVoucherWithInvoiceSettlements(receiptVoucherId);
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

		private async Task<ResponseDto> SendApproveRequest([FromQuery] ReceiptVoucherDto? newModel, [FromQuery] ReceiptVoucherDto? oldModel, int requestId, int receiptVoucherHeaderId, bool isDelete)
		{
			var changes = new List<RequestChangesDto>();
			var requestTypeId = isDelete ? ApproveRequestTypeData.Delete : (receiptVoucherHeaderId == 0 ? ApproveRequestTypeData.Add : ApproveRequestTypeData.Edit);
			if (requestTypeId == ApproveRequestTypeData.Edit)
			{
				changes = _salesInvoiceSettlementService.GetRequestChangesWithSalesInvoiceSettlements(oldModel!, newModel!);
			}

			var request = new NewApproveRequestDto()
			{
				RequestId = requestId,
				MenuCode = MenuCodeData.ReceiptVoucher,
				ApproveRequestTypeId = requestTypeId,
				ReferenceId = receiptVoucherHeaderId,
				ReferenceCode = receiptVoucherHeaderId.ToString(),
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
