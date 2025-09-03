using Accounting.API.Models;
using Accounting.CoreOne.Contracts;
using Accounting.CoreOne.Models.Dtos.ViewModels;
using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Approval;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Database;
using Shared.Helper.Extensions;
using Shared.Helper.Models.Dtos;
using Shared.Helper.Identity;

namespace Accounting.API.Controllers.Documents
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class PaymentVouchersController : ControllerBase
	{
		private readonly IPaymentVoucherHeaderService _paymentVoucherHeaderService;
		private readonly IPaymentVoucherService _paymentVoucherService;
		private readonly IHandleApprovalRequestService _handleApprovalRequestService;
		private readonly IApprovalSystemService _approvalSystemService;
		private readonly IDatabaseTransaction _databaseTransaction;
		private readonly IStringLocalizer<HandelException> _exLocalizer;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public PaymentVouchersController(IPaymentVoucherHeaderService paymentVoucherHeaderService, IPaymentVoucherService paymentVoucherService, IHandleApprovalRequestService handleApprovalRequestService, IApprovalSystemService approvalSystemService, IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exLocalizer, IHttpContextAccessor httpContextAccessor)
		{
			_paymentVoucherHeaderService = paymentVoucherHeaderService;
			_paymentVoucherService = paymentVoucherService;
			_handleApprovalRequestService = handleApprovalRequestService;
			_approvalSystemService = approvalSystemService;
			_databaseTransaction = databaseTransaction;
			_exLocalizer = exLocalizer;
			_httpContextAccessor = httpContextAccessor;
		}

		[HttpGet]
		[Route("ReadPaymentVouchers")]
		public async Task<IActionResult> ReadPaymentVouchers(DataSourceLoadOptions loadOptions)
		{
			try
			{
				var data = await DataSourceLoader.LoadAsync(_paymentVoucherHeaderService.GetUserPaymentVoucherHeaders(), loadOptions);
				return Ok(data);
			}
			catch
			{
				var model = await _paymentVoucherHeaderService.GetUserPaymentVoucherHeaders().ToListAsync();
				var data = DataSourceLoader.Load(model, loadOptions);
				return Ok(data);
			}
		}

		[HttpGet]
		[Route("GetPaymentVoucherCode")]
		public async Task<IActionResult> GetPaymentVoucherCode(int storeId, DateTime documentDate)
		{
			var data = await _paymentVoucherHeaderService.GetPaymentVoucherCode(storeId, documentDate);
			return Ok(data);
		}
		[HttpGet]
		[Route("GetRequestData")]
		public async Task<IActionResult> GetRequestData(int requestId)
		{
			var data = await _handleApprovalRequestService.GetRequestData(requestId);
			if (data != null)
			{
				if (data.CanBeConverted<PaymentVoucherDto>())
				{
					var paymentVoucher = data.ConvertToType<PaymentVoucherDto>();
					var userCanLook = await _httpContextAccessor.UserCanLook(0, paymentVoucher?.PaymentVoucherHeader?.StoreId ?? 0);
					return Ok(userCanLook ? paymentVoucher : new PaymentVoucherDto());
				}
			}
			return Ok(new PaymentVoucherDto());
		}

		[HttpGet("{id:int}")]
		public async Task<IActionResult> GetPaymentVoucher(int id)
		{
			var paymentVoucher = await _paymentVoucherService.GetPaymentVoucher(id);
			var userCanLook = await _httpContextAccessor.UserCanLook(0, paymentVoucher.PaymentVoucherHeader?.StoreId ?? 0);
			return Ok(userCanLook ? paymentVoucher : new PaymentVoucherDto());
		}

		[HttpPost]
		[Route("Save")]
		public async Task<IActionResult> Save([FromBody] PaymentVoucherDto model, int requestId)
		{
			ResponseDto response;
			try
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
						var oldValue = await _paymentVoucherService.GetPaymentVoucher(model.PaymentVoucherHeader!.PaymentVoucherHeaderId);
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
			catch (Exception e)
			{
				await _databaseTransaction.Rollback();
				var handleException = new HandelException(_exLocalizer);
				return Ok(handleException.Handle(e));
			}
			return Ok(response);
		}

		[HttpPost]
		[Route("SavePaymentVoucher")]
		public async Task<ResponseDto> SavePaymentVoucher(PaymentVoucherDto model, bool hasApprove, bool approved, int? requestId)
		{
			await _databaseTransaction.BeginTransaction();
			var response = await _paymentVoucherService.SavePaymentVoucher(model, hasApprove, approved, requestId);
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
		[Route("DeletePaymentVoucher")]
		public async Task<IActionResult> DeletePaymentVoucher(int paymentVoucherId, int requestId)
		{
			ResponseDto response;
			try
			{
				var hasApprove = await _approvalSystemService.IsMenuHasApprove(MenuCodeData.PaymentVoucher);
				if (hasApprove.HasApprove && hasApprove.OnDelete)
				{
					var model = await _paymentVoucherService.GetPaymentVoucher(paymentVoucherId);
					response = await SendApproveRequest(null, model, requestId, paymentVoucherId, true);
				}
				else
				{
					await _databaseTransaction.BeginTransaction();
					response = await _paymentVoucherService.DeletePaymentVoucher(paymentVoucherId);
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
		public async Task<ResponseDto> SendApproveRequest([FromQuery] PaymentVoucherDto? newModel, [FromQuery] PaymentVoucherDto? oldModel, int requestId, int paymentVoucherHeaderId, bool isDelete)
		{
			var changes = new List<RequestChangesDto>();
			var requestTypeId = isDelete ? ApproveRequestTypeData.Delete : (paymentVoucherHeaderId == 0 ? ApproveRequestTypeData.Add : ApproveRequestTypeData.Edit);
			if (requestTypeId == ApproveRequestTypeData.Edit)
			{
				changes = _paymentVoucherService.GetPaymentVoucherRequestChanges(oldModel!, newModel!);
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

		[HttpPost]
		[Route("ApprovePaymentVoucher")]
		public async Task<ResponseDto> ApprovePaymentVoucher(ApproveResponseDto request)
		{
			var result = new ResponseDto();
			var data = request.UserRequest;
			if (data != null)
			{
				if (data.CanBeConverted<PaymentVoucherDto>())
				{
					var sendModel = data.ConvertToType<PaymentVoucherDto>();
					if (sendModel != null)
					{
						try
						{
							if (request.ApproveRequestTypeId == ApproveRequestTypeData.Delete)
							{
								await _databaseTransaction.BeginTransaction();
								result = await _paymentVoucherService.DeletePaymentVoucher(sendModel.PaymentVoucherHeader!.PaymentVoucherHeaderId);
								if (result.Success)
								{
									await _databaseTransaction.Commit();
								}
								else
								{
									await _databaseTransaction.Rollback();
								}
							}
							else
							{
								await _databaseTransaction.BeginTransaction();
								result = await _paymentVoucherService.SavePaymentVoucher(sendModel, true, true, request.RequestId);
								if (result.Success)
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
							result = handleException.Handle(e);
						}
					}
				}
			}
			return result;
		}
	}
}