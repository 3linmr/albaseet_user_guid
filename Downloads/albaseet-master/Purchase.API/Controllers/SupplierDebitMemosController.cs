using DevExtreme.AspNet.Data;
using Purchases.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Contracts.Approval;
using Shared.CoreOne;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Database;
using Shared.Helper.Extensions;
using Shared.Helper.Models.Dtos;
using Purchases.CoreOne.Models.Dtos.ViewModels;
using Purchases.CoreOne.Contracts;
using Purchases.Service.Services;
using Shared.Service.Services.Approval;
using Shared.Service;
using Purchases.CoreOne.Models.Domain;
using Shared.CoreOne.Contracts.Menus;
using Shared.Helper.Identity;

namespace Purchases.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SupplierDebitMemosController : Controller
    {
        private readonly ISupplierDebitMemoService _supplierDebitMemoService;
        private readonly ISupplierDebitMemoHandlingService _supplierDebitMemoHandlingService;
        private readonly IHandleApprovalRequestService _handleApprovalRequestService;
        private readonly IApprovalSystemService _approvalSystemService;
        private readonly IDatabaseTransaction _databaseTransaction;
        private readonly IStringLocalizer<HandelException> _exLocalizer;
        private readonly IGenericMessageService _genericMessageService;
		private readonly IHttpContextAccessor _httpContextAccessor;

        public SupplierDebitMemosController(ISupplierDebitMemoService supplierDebitMemoService, ISupplierDebitMemoHandlingService supplierDebitMemoHandlingService, IHandleApprovalRequestService handleApprovalRequestService, IApprovalSystemService approvalSystemService, IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exLocalizer, IGenericMessageService genericMessageService, IHttpContextAccessor httpContextAccessor)
        {
            _supplierDebitMemoService = supplierDebitMemoService;
            _supplierDebitMemoHandlingService = supplierDebitMemoHandlingService;
            _handleApprovalRequestService = handleApprovalRequestService;
            _approvalSystemService = approvalSystemService;
            _databaseTransaction = databaseTransaction;
            _exLocalizer = exLocalizer;
            _genericMessageService = genericMessageService;
			_httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("ReadSupplierDebitMemos")]
        public async Task<IActionResult> ReadSupplierDebitMemos(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var data = await DataSourceLoader.LoadAsync(_supplierDebitMemoService.GetUserSupplierDebitMemos(), loadOptions);
                return Ok(data);
            }
            catch
            {
                var model = _supplierDebitMemoService.GetUserSupplierDebitMemos();
                var data = DataSourceLoader.Load(model.ToList(), loadOptions);
                return Ok(data);
            }
        }

        [HttpGet]
        [Route("ReadSupplierDebitMemosByStoreId")]
        public async Task<IActionResult> ReadSupplierDebitMemosByStoreId(DataSourceLoadOptions loadOptions, int storeId, int? supplierId, int supplierDebitMemoId)
        {
            try
            {
                var data = await DataSourceLoader.LoadAsync(_supplierDebitMemoService.GetSupplierDebitMemosByStoreId(storeId, supplierId, supplierDebitMemoId), loadOptions);
                return Ok(data);
            }
            catch
            {
                var model = _supplierDebitMemoService.GetSupplierDebitMemosByStoreId(storeId, supplierId, supplierDebitMemoId);
                var data = DataSourceLoader.Load(model.ToList(), loadOptions);
                return Ok(data);
            }
        }

        [HttpGet]
        [Route("GetSupplierDebitMemoCode")]
        public async Task<IActionResult> GetSupplierDebitMemoCode(int storeId, DateTime documentDate)
        {
            return Ok(await _supplierDebitMemoService.GetSupplierDebitMemoCode(storeId, documentDate));
        }

        [HttpGet]
        [Route("GetSupplierDebitMemoFromPurchaseInvoice")]
        public async Task<IActionResult> GetSupplierDebitMemoFromPurchaseInvoice(int purchaseInvoiceHeaderId)
        {
            var data = await _supplierDebitMemoHandlingService.GetSupplierDebitMemoFromPurchaseInvoice(purchaseInvoiceHeaderId);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetRequestData")]
        public async Task<IActionResult> GetRequestData(int requestId)
        {
            var data = await _handleApprovalRequestService.GetRequestData(requestId);
            if (data != null)
            {
				var supplierDebitMemo = data.ConvertToType<SupplierDebitMemoVm>();
				var userCanLook = await _httpContextAccessor.UserCanLook(0, supplierDebitMemo?.SupplierDebitMemo?.StoreId ?? 0);
                return Ok(userCanLook ? supplierDebitMemo : new SupplierDebitMemoVm());
            }
            return Ok(new SupplierDebitMemoVm());
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetSupplierDebitMemo(int id)
        {
            var data = await _supplierDebitMemoHandlingService.GetSupplierDebitMemo(id);
			var userCanLook = await _httpContextAccessor.UserCanLook(0, data.SupplierDebitMemo?.StoreId ?? 0);
            return Ok(userCanLook ? data : new SupplierDebitMemoVm());
        }

        [HttpGet]
        [Route("GetSupplierDebitMemoWithoutJournal")]
        public async Task<IActionResult> GetSupplierDebitMemoWithoutJournal(int id)
        {
            var supplierDebitMemo = await _supplierDebitMemoService.GetSupplierDebitMemoById(id);
            return Ok(supplierDebitMemo);
        }

        [HttpPost]
        [Route("Save")]
        public async Task<IActionResult> Save([FromBody] SupplierDebitMemoVm model, int requestId)
        {
            var response = new ResponseDto();
            try
            {
                if (model.SupplierDebitMemo != null)
                {
                    var hasApprove = await _approvalSystemService.IsMenuHasApprove(MenuCodeData.SupplierDebitMemo);
                    if (hasApprove.HasApprove)
                    {
                        if (hasApprove.OnAdd && model.SupplierDebitMemo!.SupplierDebitMemoId == 0)
                        {
                            response = await SendApproveRequest(model, null, requestId, 0, false);
                        }
                        else if (hasApprove.OnEdit && model.SupplierDebitMemo!.SupplierDebitMemoId > 0)
                        {
                            var oldValue = await _supplierDebitMemoHandlingService.GetSupplierDebitMemo(model.SupplierDebitMemo!.SupplierDebitMemoId);
                            response = await SendApproveRequest(model, oldValue, requestId, model.SupplierDebitMemo!.SupplierDebitMemoId, false);
                        }
                        else
                        {
                            response = await SaveSupplierDebitMemo(model, hasApprove.HasApprove, false, requestId);
                        }
                    }
                    else
                    {
                        response = await SaveSupplierDebitMemo(model, hasApprove.HasApprove, false, requestId);
                    }
                }
                else
                {
                    response.Message = "Memo should not be null";
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

        private async Task<ResponseDto> SaveSupplierDebitMemo(SupplierDebitMemoVm model, bool hasApprove, bool approved, int? requestId)
        {
            await _databaseTransaction.BeginTransaction();
            var response = await _supplierDebitMemoHandlingService.SaveSupplierDebitMemoInFull(model, hasApprove, approved, requestId);
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
        [Route("DeleteSupplierDebitMemo")]
        public async Task<IActionResult> DeleteSupplierDebitMemo(int supplierDebitMemoId, int requestId)
        {
            var response = new ResponseDto();
            try
            {
                var model = await _supplierDebitMemoHandlingService.GetSupplierDebitMemo(supplierDebitMemoId);
                if (model.SupplierDebitMemo != null)
                {
                    var hasApprove = await _approvalSystemService.IsMenuHasApprove(MenuCodeData.SupplierDebitMemo);
                    if (hasApprove.HasApprove && hasApprove.OnDelete)
                    {
                        response = await SendApproveRequest(null, model, requestId, supplierDebitMemoId, true);
                    }
                    else
                    {
                        await _databaseTransaction.BeginTransaction();
                        response = await _supplierDebitMemoHandlingService.DeleteSupplierDebitMemoInFull(supplierDebitMemoId);
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
                    response.Id = supplierDebitMemoId;
                    response.Message = await _genericMessageService.GetMessage(MenuCodeData.SupplierDebitMemo, GenericMessageData.NotFound);
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

        private async Task<ResponseDto> SendApproveRequest(SupplierDebitMemoVm? newModel, SupplierDebitMemoVm? oldModel, int requestId, int supplierDebitMemoId, bool isDelete)
        {
            var changes = new List<RequestChangesDto>();
            var menuCode = MenuCodeData.SupplierDebitMemo;
            var requestTypeId = isDelete ? ApproveRequestTypeData.Delete : (supplierDebitMemoId == 0 ? ApproveRequestTypeData.Add : ApproveRequestTypeData.Edit);
            if (requestTypeId == ApproveRequestTypeData.Edit && oldModel != null)
            {
                changes = _supplierDebitMemoHandlingService.GetSupplierDebitMemoRequestChanges(oldModel, newModel!);
            }

            var request = new NewApproveRequestDto()
            {
                RequestId = requestId,
                MenuCode = (short)menuCode,
                ApproveRequestTypeId = requestTypeId,
                ReferenceId = supplierDebitMemoId,
                ReferenceCode = supplierDebitMemoId.ToString(),
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
