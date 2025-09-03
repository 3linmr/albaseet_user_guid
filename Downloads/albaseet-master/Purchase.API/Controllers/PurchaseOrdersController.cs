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
using Shared.CoreOne.Contracts.Menus;
using Shared.Helper.Identity;

namespace Purchases.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PurchaseOrdersController : ControllerBase
    {
        private readonly IDatabaseTransaction _databaseTransaction;
        private readonly IStringLocalizer<HandelException> _exLocalizer;
        private readonly IPurchaseOrderHeaderService _purchaseOrderHeaderService;
        private readonly IPurchaseOrderService _purchaseOrderService;
        private readonly IPurchaseOrderDetailService _PurchaseOrderDetailService;
        private readonly IHandleApprovalRequestService _handleApprovalRequestService;
        private readonly IApprovalSystemService _approvalSystemService;
        private readonly IGenericMessageService _genericMessageService;
		private readonly IHttpContextAccessor _httpContextAccessor;

        public PurchaseOrdersController(IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exLocalizer, IHandleApprovalRequestService handleApprovalRequestService, IApprovalSystemService approvalSystemService, IPurchaseOrderHeaderService purchaseOrderHeaderService, IPurchaseOrderService purchaseOrderService, IPurchaseOrderDetailService purchaseOrderDetailService, IGenericMessageService genericMessageService, IHttpContextAccessor httpContextAccessor)
        {
            _databaseTransaction = databaseTransaction;
            _exLocalizer = exLocalizer;
            _purchaseOrderHeaderService = purchaseOrderHeaderService;
            _purchaseOrderService = purchaseOrderService;
            _PurchaseOrderDetailService = purchaseOrderDetailService;
            _handleApprovalRequestService = handleApprovalRequestService;
            _approvalSystemService = approvalSystemService;
            _genericMessageService = genericMessageService;
			_httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("ReadPurchaseOrders")]
        public async Task<IActionResult> ReadPurchaseOrders(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var data = await DataSourceLoader.LoadAsync(_purchaseOrderHeaderService.GetUserPurchaseOrderHeaders(), loadOptions);
                return Ok(data);
            }
            catch
            {
                var model = _purchaseOrderHeaderService.GetUserPurchaseOrderHeaders();
                var data = DataSourceLoader.Load(model.ToList(), loadOptions);
                return Ok(data);
            }
        }

        [HttpGet]
        [Route("ReadPurchaseOrdersByStoreId")]
        public async Task<IActionResult> ReadPurchaseOrdersByStoreId(DataSourceLoadOptions loadOptions, int storeId, int? supplierId, int purchaseOrderHeaderId)
        {
            try
            {
                var data = await DataSourceLoader.LoadAsync(_purchaseOrderHeaderService.GetPurchaseOrderHeadersByStoreId(storeId, supplierId, purchaseOrderHeaderId), loadOptions);
                return Ok(data);
            }
            catch
            {
                var model = _purchaseOrderHeaderService.GetPurchaseOrderHeadersByStoreId(storeId, supplierId, purchaseOrderHeaderId);
                var data = DataSourceLoader.Load(model.ToList(), loadOptions);
                return Ok(data);
            }
        }

        [HttpGet]
        [Route("ReadPurchaseOrdersByStoreIdAndMenuCode")]
        public async Task<IActionResult> ReadPurchaseOrdersByStoreIdAndMenuCode(DataSourceLoadOptions loadOptions, int storeId, int? supplierId, int menuCode, int purchaseOrderHeaderId)
        {
            try
            {
                var data = await DataSourceLoader.LoadAsync(_purchaseOrderHeaderService.GetPurchaseOrderHeadersByStoreIdAndMenuCode(storeId, supplierId, menuCode, purchaseOrderHeaderId), loadOptions);
                return Ok(data);
            }
            catch
            {
                var model = _purchaseOrderHeaderService.GetPurchaseOrderHeadersByStoreIdAndMenuCode(storeId, supplierId, menuCode, purchaseOrderHeaderId);
                var data = DataSourceLoader.Load(model.ToList(), loadOptions);
                return Ok(data);
            }
        }

        [HttpGet]
        [Route("GetPurchaseOrderCode")]
        public async Task<IActionResult> GetPurchaseOrderCode(int storeId, DateTime documentDate)
        {
            var data = await _purchaseOrderHeaderService.GetPurchaseOrderCode(storeId, documentDate);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetPurchaseOrderFromSupplierQuotation")]
        public async Task<IActionResult> GetPurchaseOrderFromSupplierQuotation(int supplierQuotationHeaderId)
        {
            var data = await _purchaseOrderService.GetPurchaseOrderFromSupplierQuotation(supplierQuotationHeaderId);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetRequestData")]
        public async Task<IActionResult> GetRequestData(int requestId)
        {
            var data = await _handleApprovalRequestService.GetRequestData(requestId);
            if (data != null)
            {
                if (data.CanBeConverted<PurchaseOrderDto>())
                {
					var purchaseOrder = data.ConvertToType<PurchaseOrderDto>();
					var userCanLook = await _httpContextAccessor.UserCanLook(0, purchaseOrder?.PurchaseOrderHeader?.StoreId ?? 0);
                    return Ok(userCanLook ? purchaseOrder : new PurchaseOrderDto());
                }
            }
            return Ok(new PurchaseOrderDto());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPurchaseOrder(int id)
        {
            var purchaseOrder = await _purchaseOrderService.GetPurchaseOrder(id);
			var userCanLook = await _httpContextAccessor.UserCanLook(0, purchaseOrder.PurchaseOrderHeader?.StoreId ?? 0);
            return Ok(userCanLook ? purchaseOrder : new PurchaseOrderDto());
        }

        [HttpGet]
        [Route("GetPurchaseOrderDetail")]
        public async Task<IActionResult> GetPurchaseOrderDetail(int id)
        {
            var purchaseOrderDetail = await _PurchaseOrderDetailService.GetPurchaseOrderDetails(id);
            return Ok(purchaseOrderDetail);
        }

        [HttpGet]
        [Route("GetPurchaseOrderHeader")]
        public async Task<IActionResult> GetPurchaseOrderHeader(int id)
        {
            var purchaseOrderHeader = await _purchaseOrderHeaderService.GetPurchaseOrderHeaderById(id);
            return Ok(purchaseOrderHeader);
        }

        [HttpPost]
        [Route("UpdateIsBlocked")]
        public async Task<IActionResult> UpdateIsBlocked(int purchaseOrderHeaderId, bool isBlocked)
        {
            var result = await _purchaseOrderService.UpdateBlocked(purchaseOrderHeaderId, isBlocked);
            return Ok(result);
        }

        [HttpPost]
        [Route("Save")]
        public async Task<IActionResult> Save([FromBody] PurchaseOrderDto model, int requestId)
        {
            var response = new ResponseDto();
            try
            {
                if (model.PurchaseOrderHeader != null)
                {
                    var hasApprove = await _approvalSystemService.IsMenuHasApprove(MenuCodeData.PurchaseOrder);
                    if (hasApprove.HasApprove)
                    {
                        if (hasApprove.OnAdd && model.PurchaseOrderHeader!.PurchaseOrderHeaderId == 0)
                        {
                            response = await SendApproveRequest(model, null, requestId, 0, false);
                        }
                        else if (hasApprove.OnEdit && model.PurchaseOrderHeader!.PurchaseOrderHeaderId > 0)
                        {
                            var oldValue = await _purchaseOrderService.GetPurchaseOrder(model.PurchaseOrderHeader!.PurchaseOrderHeaderId);
                            response = await SendApproveRequest(model, oldValue, requestId, model.PurchaseOrderHeader!.PurchaseOrderHeaderId, false);
                        }
                        else
                        {
                            response = await SavePurchaseOrder(model, hasApprove.HasApprove, false, requestId);
                        }
                    }
                    else
                    {
                        response = await SavePurchaseOrder(model, hasApprove.HasApprove, false, requestId);
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
        [Route("SavePurchaseOrder")]
        public async Task<ResponseDto> SavePurchaseOrder(PurchaseOrderDto model, bool hasApprove, bool approved, int? requestId)
        {
            await _databaseTransaction.BeginTransaction();
            var response = await _purchaseOrderService.SavePurchaseOrder(model, hasApprove, approved, requestId);
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
        [Route("DeletePurchaseOrder")]
        public async Task<IActionResult> DeletePurchaseOrder(int purchaseOrderHeaderId, int requestId)
        {
            var response = new ResponseDto();
            try
            {
                var model = await _purchaseOrderService.GetPurchaseOrder(purchaseOrderHeaderId);
                if (model.PurchaseOrderHeader != null)
                {
                    var hasApprove = await _approvalSystemService.IsMenuHasApprove(MenuCodeData.PurchaseOrder);
                    if (hasApprove.HasApprove && hasApprove.OnDelete)
                    {
                        response = await SendApproveRequest(null, model, requestId, purchaseOrderHeaderId, true);
                    }
                    else
                    {
                        await _databaseTransaction.BeginTransaction();
                        response = await _purchaseOrderService.DeletePurchaseOrder(purchaseOrderHeaderId);
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
                    response.Id = purchaseOrderHeaderId;
                    response.Message = await _genericMessageService.GetMessage(MenuCodeData.PurchaseOrder, GenericMessageData.NotFound);
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
        public async Task<ResponseDto> SendApproveRequest([FromQuery] PurchaseOrderDto? newModel, [FromQuery] PurchaseOrderDto? oldModel, int requestId, int purchaseOrderHeaderId, bool isDelete)
        {
            var changes = new List<RequestChangesDto>();
            var menuCode = MenuCodeData.PurchaseOrder;
            var requestTypeId = isDelete ? ApproveRequestTypeData.Delete : (purchaseOrderHeaderId == 0 ? ApproveRequestTypeData.Add : ApproveRequestTypeData.Edit);
            if (requestTypeId == ApproveRequestTypeData.Edit && oldModel != null)
            {
                changes = _purchaseOrderService.GetPurchaseOrderRequestChanges(oldModel, newModel!);
            }

            var request = new NewApproveRequestDto()
            {
                RequestId = requestId,
                MenuCode = (short)menuCode,
                ApproveRequestTypeId = requestTypeId,
                ReferenceId = purchaseOrderHeaderId,
                ReferenceCode = purchaseOrderHeaderId.ToString(),
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
