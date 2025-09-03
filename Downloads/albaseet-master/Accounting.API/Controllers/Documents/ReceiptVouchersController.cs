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
using Shared.CoreOne.Models.Dtos.ViewModels.Accounts;
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
	public class ReceiptVouchersController : ControllerBase
	{
		private readonly IReceiptVoucherHeaderService _receiptVoucherHeaderService;
		private readonly IReceiptVoucherService _receiptVoucherService;
		private readonly IHandleApprovalRequestService _handleApprovalRequestService;
		private readonly IApprovalSystemService _approvalSystemService;
		private readonly IDatabaseTransaction _databaseTransaction;
		private readonly IStringLocalizer<HandelException> _exLocalizer;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public ReceiptVouchersController(IReceiptVoucherHeaderService receiptVoucherHeaderService,IReceiptVoucherService receiptVoucherService,IHandleApprovalRequestService handleApprovalRequestService,IApprovalSystemService approvalSystemService,IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exLocalizer, IHttpContextAccessor httpContextAccessor)
		{
			_receiptVoucherHeaderService = receiptVoucherHeaderService;
			_receiptVoucherService = receiptVoucherService;
			_handleApprovalRequestService = handleApprovalRequestService;
			_approvalSystemService = approvalSystemService;
			_databaseTransaction = databaseTransaction;
			_exLocalizer = exLocalizer;
			_httpContextAccessor = httpContextAccessor;
		}

		[HttpGet]
		[Route("ReadReceiptVouchers")]
		public async Task<IActionResult> ReadReceiptVouchers(DataSourceLoadOptions loadOptions)
		{
			try
			{
				var data = await DataSourceLoader.LoadAsync(_receiptVoucherHeaderService.GetUserReceiptVoucherHeaders(), loadOptions);
				return Ok(data);
			}
			catch
			{
				var model = await _receiptVoucherHeaderService.GetUserReceiptVoucherHeaders().ToListAsync();
				var data = DataSourceLoader.Load(model, loadOptions);
				return Ok(data);
			}
		}

		[HttpGet]
		[Route("GetReceiptVoucherCode")]
		public async Task<IActionResult> GetReceiptVoucherCode(int storeId, DateTime documentDate)
		{
			var data = await _receiptVoucherHeaderService.GetReceiptVoucherCode(storeId, documentDate);
			return Ok(data);
		}
		[HttpGet]
		[Route("GetRequestData")]
		public async Task<IActionResult> GetRequestData(int requestId)
		{
			var data = await _handleApprovalRequestService.GetRequestData(requestId);
			if (data != null)
			{
				if (data.CanBeConverted<ReceiptVoucherDto>())
				{
					var receiptVoucher = data.ConvertToType<ReceiptVoucherDto>();
					var userCanLook = await _httpContextAccessor.UserCanLook(0, receiptVoucher?.ReceiptVoucherHeader?.StoreId ?? 0);
					return Ok(userCanLook ? receiptVoucher : new ReceiptVoucherDto());
				}
			}
			return Ok(new ReceiptVoucherDto());
		}

		[HttpGet("{id:int}")]
		public async Task<IActionResult> GetReceiptVoucher(int id)
		{
			var receiptVoucher = await _receiptVoucherService.GetReceiptVoucher(id);
			var userCanLook = await _httpContextAccessor.UserCanLook(0, receiptVoucher.ReceiptVoucherHeader?.StoreId ?? 0);
			return Ok(userCanLook ? receiptVoucher : new ReceiptVoucherDto());
		}

		[HttpPost]
		[Route("Save")]
		public async Task<IActionResult> Save([FromBody] ReceiptVoucherDto model, int requestId)
		{
			ResponseDto response;
			try
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
						var oldValue = await _receiptVoucherService.GetReceiptVoucher(model.ReceiptVoucherHeader!.ReceiptVoucherHeaderId);
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
			catch (Exception e)
			{
				await _databaseTransaction.Rollback();
				var handleException = new HandelException(_exLocalizer);
				return Ok(handleException.Handle(e));
			}
			return Ok(response);
		}

		[HttpPost]
		[Route("SaveReceiptVoucher")]
		public async Task<ResponseDto> SaveReceiptVoucher(ReceiptVoucherDto model, bool hasApprove, bool approved, int? requestId)
		{
			await _databaseTransaction.BeginTransaction();
			var response = await _receiptVoucherService.SaveReceiptVoucher(model, hasApprove, approved, requestId);
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
		[Route("DeleteReceiptVoucher")]
		public async Task<IActionResult> DeleteReceiptVoucher(int receiptVoucherId, int requestId)
		{
			ResponseDto response;
			try
			{
				var hasApprove = await _approvalSystemService.IsMenuHasApprove(MenuCodeData.ReceiptVoucher);
				if (hasApprove.HasApprove && hasApprove.OnDelete)
				{
					var model = await _receiptVoucherService.GetReceiptVoucher(receiptVoucherId);
					response = await SendApproveRequest(null, model, requestId, receiptVoucherId, true);
				}
				else
				{
					await _databaseTransaction.BeginTransaction();
					response = await _receiptVoucherService.DeleteReceiptVoucher(receiptVoucherId);
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
		public async Task<ResponseDto> SendApproveRequest([FromQuery] ReceiptVoucherDto? newModel, [FromQuery] ReceiptVoucherDto? oldModel, int requestId, int receiptVoucherHeaderId, bool isDelete)
		{
			var changes = new List<RequestChangesDto>();
			var requestTypeId = isDelete ? ApproveRequestTypeData.Delete : (receiptVoucherHeaderId == 0 ? ApproveRequestTypeData.Add : ApproveRequestTypeData.Edit);
			if (requestTypeId == ApproveRequestTypeData.Edit)
			{
				changes = _receiptVoucherService.GetReceiptVoucherRequestChanges(oldModel!, newModel!);
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

		[HttpPost]
		[Route("ApproveReceiptVoucher")]
		public async Task<ResponseDto> ApproveReceiptVoucher(ApproveResponseDto request)
		{
			var result = new ResponseDto();
			var data = request.UserRequest;
			if (data != null)
			{
				if (data.CanBeConverted<ReceiptVoucherDto>())
				{
					var sendModel = data.ConvertToType<ReceiptVoucherDto>();
					if (sendModel != null)
					{
						try
						{
							if (request.ApproveRequestTypeId == ApproveRequestTypeData.Delete)
							{
								await _databaseTransaction.BeginTransaction();
								result = await _receiptVoucherService.DeleteReceiptVoucher(sendModel.ReceiptVoucherHeader!.ReceiptVoucherHeaderId);
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
								result = await _receiptVoucherService.SaveReceiptVoucher(sendModel, true, true, request.RequestId);
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
