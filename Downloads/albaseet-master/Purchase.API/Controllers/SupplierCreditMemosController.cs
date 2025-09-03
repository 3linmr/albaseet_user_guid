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
    public class SupplierCreditMemosController : Controller
    {
        private readonly ISupplierCreditMemoService _supplierCreditMemoService;
        private readonly ISupplierCreditMemoHandlingService _supplierCreditMemoHandlingService;
        private readonly IHandleApprovalRequestService _handleApprovalRequestService;
        private readonly IApprovalSystemService _approvalSystemService;
        private readonly IDatabaseTransaction _databaseTransaction;
        private readonly IStringLocalizer<HandelException> _exLocalizer;
        private readonly IGenericMessageService _genericMessageService;
		private readonly IHttpContextAccessor _httpContextAccessor;

        public SupplierCreditMemosController(ISupplierCreditMemoService supplierCreditMemoService, ISupplierCreditMemoHandlingService supplierCreditMemoHandlingService, IHandleApprovalRequestService handleApprovalRequestService, IApprovalSystemService approvalSystemService, IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exLocalizer, IGenericMessageService genericMessageService, IHttpContextAccessor httpContextAccessor)
        {
            _supplierCreditMemoService = supplierCreditMemoService;
            _supplierCreditMemoHandlingService = supplierCreditMemoHandlingService;
            _handleApprovalRequestService = handleApprovalRequestService;
            _approvalSystemService = approvalSystemService;
            _databaseTransaction = databaseTransaction;
            _exLocalizer = exLocalizer;
            _genericMessageService = genericMessageService;
			_httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("ReadSupplierCreditMemos")]
        public async Task<IActionResult> ReadSupplierCreditMemos(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var data = await DataSourceLoader.LoadAsync(_supplierCreditMemoService.GetUserSupplierCreditMemos(), loadOptions);
                return Ok(data);
            }
            catch
            {
                var model = _supplierCreditMemoService.GetUserSupplierCreditMemos();
                var data = DataSourceLoader.Load(model.ToList(), loadOptions);
                return Ok(data);
            }
        }

        [HttpGet]
        [Route("ReadSupplierCreditMemosByStoreId")]
        public async Task<IActionResult> ReadSupplierCreditMemosByStoreId(DataSourceLoadOptions loadOptions, int storeId, int? supplierId, int supplierCreditMemoId)
        {
            try
            {
                var data = await DataSourceLoader.LoadAsync(_supplierCreditMemoService.GetSupplierCreditMemosByStoreId(storeId, supplierId, supplierCreditMemoId), loadOptions);
                return Ok(data);
            }
            catch
            {
                var model = _supplierCreditMemoService.GetSupplierCreditMemosByStoreId(storeId, supplierId, supplierCreditMemoId);
                var data = DataSourceLoader.Load(model.ToList(), loadOptions);
                return Ok(data);
            }
        }

        [HttpGet]
        [Route("GetSupplierCreditMemoCode")]
        public async Task<IActionResult> GetSupplierCreditMemoCode(int storeId, DateTime documentDate)
        {
            return Ok(await _supplierCreditMemoService.GetSupplierCreditMemoCode(storeId, documentDate));
        }

        [HttpGet]
        [Route("GetSupplierCreditMemoFromPurchaseInvoice")]
        public async Task<IActionResult> GetSupplierCreditMemoFromPurchaseInvoice(int purchaseInvoiceHeaderId)
        {
            var data = await _supplierCreditMemoHandlingService.GetSupplierCreditMemoFromPurchaseInvoice(purchaseInvoiceHeaderId);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetRequestData")]
        public async Task<IActionResult> GetRequestData(int requestId)
        {
            var data = await _handleApprovalRequestService.GetRequestData(requestId);
            if (data != null)
            {
                if (data.CanBeConverted<SupplierCreditMemoVm>())
                {
					var supplierCreditMemo = data.ConvertToType<SupplierCreditMemoVm>();
					var userCanLook = await _httpContextAccessor.UserCanLook(0, supplierCreditMemo?.SupplierCreditMemo?.StoreId ?? 0);
                    return Ok(userCanLook ? supplierCreditMemo : new SupplierCreditMemoVm());
                }
            }
            return Ok(new SupplierCreditMemoVm());
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetSupplierCreditMemo(int id)
        {
            var data = await _supplierCreditMemoHandlingService.GetSupplierCreditMemo(id);
			var userCanLook = await _httpContextAccessor.UserCanLook(0, data.SupplierCreditMemo?.StoreId ?? 0);
            return Ok(userCanLook ? data : new SupplierCreditMemoVm());
        }

        [HttpGet]
        [Route("GetSupplierCreditMemoWithoutJournal")]
        public async Task<IActionResult> GetSupplierCreditMemoWithoutJournal(int id)
        {
            var supplierCreditMemo = await _supplierCreditMemoService.GetSupplierCreditMemoById(id);
            return Ok(supplierCreditMemo);
        }

        [HttpPost]
        [Route("Save")]
        public async Task<IActionResult> Save([FromBody] SupplierCreditMemoVm model, int requestId)
        {
            var response = new ResponseDto();
            try
            {
                if (model.SupplierCreditMemo != null)
                {
                    var hasApprove = await _approvalSystemService.IsMenuHasApprove(MenuCodeData.SupplierCreditMemo);
                    if (hasApprove.HasApprove)
                    {
                        if (hasApprove.OnAdd && model.SupplierCreditMemo!.SupplierCreditMemoId == 0)
                        {
                            response = await SendApproveRequest(model, null, requestId, 0, false);
                        }
                        else if (hasApprove.OnEdit && model.SupplierCreditMemo!.SupplierCreditMemoId > 0)
                        {
                            var oldValue = await _supplierCreditMemoHandlingService.GetSupplierCreditMemo(model.SupplierCreditMemo!.SupplierCreditMemoId);
                            response = await SendApproveRequest(model, oldValue, requestId, model.SupplierCreditMemo!.SupplierCreditMemoId, false);
                        }
                        else
                        {
                            response = await SaveSupplierCreditMemo(model, hasApprove.HasApprove, false, requestId);
                        }
                    }
                    else
                    {
                        response = await SaveSupplierCreditMemo(model, hasApprove.HasApprove, false, requestId);
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

        private async Task<ResponseDto> SaveSupplierCreditMemo(SupplierCreditMemoVm model, bool hasApprove, bool approved, int? requestId)
        {
            await _databaseTransaction.BeginTransaction();
            var response = await _supplierCreditMemoHandlingService.SaveSupplierCreditMemoInFull(model, hasApprove, approved, requestId);
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
        [Route("DeleteSupplierCreditMemo")]
        public async Task<IActionResult> DeleteSupplierCreditMemo(int supplierCreditMemoId, int requestId)
        {
            var response = new ResponseDto();
            try
            {
                var model = await _supplierCreditMemoHandlingService.GetSupplierCreditMemo(supplierCreditMemoId);
                if (model.SupplierCreditMemo != null)
                {
                    var hasApprove = await _approvalSystemService.IsMenuHasApprove(MenuCodeData.SupplierCreditMemo);
                    if (hasApprove.HasApprove && hasApprove.OnDelete)
                    {
                        response = await SendApproveRequest(null, model, requestId, supplierCreditMemoId, true);
                    }
                    else
                    {
                        await _databaseTransaction.BeginTransaction();
                        response = await _supplierCreditMemoHandlingService.DeleteSupplierCreditMemoInFull(supplierCreditMemoId);
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
                    response.Id = supplierCreditMemoId;
                    response.Message = await _genericMessageService.GetMessage(MenuCodeData.SupplierCreditMemo, GenericMessageData.NotFound);
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

        private async Task<ResponseDto> SendApproveRequest(SupplierCreditMemoVm? newModel, SupplierCreditMemoVm? oldModel, int requestId, int supplierCreditMemoId, bool isDelete)
        {
            var changes = new List<RequestChangesDto>();
            var menuCode = MenuCodeData.SupplierCreditMemo;
            var requestTypeId = isDelete ? ApproveRequestTypeData.Delete : (supplierCreditMemoId == 0 ? ApproveRequestTypeData.Add : ApproveRequestTypeData.Edit);
            if (requestTypeId == ApproveRequestTypeData.Edit && oldModel != null)
            {
                changes = _supplierCreditMemoHandlingService.GetSupplierCreditMemoRequestChanges(oldModel, newModel!);
            }

            var request = new NewApproveRequestDto()
            {
                RequestId = requestId,
                MenuCode = (short)menuCode,
                ApproveRequestTypeId = requestTypeId,
                ReferenceId = supplierCreditMemoId,
                ReferenceCode = supplierCreditMemoId.ToString(),
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
