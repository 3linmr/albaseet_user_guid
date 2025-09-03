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
using Shared.CoreOne.Models.Dtos.ViewModels.Items;
using Purchases.Service.Services;
using Shared.CoreOne.Models.Domain.Inventory;
using Purchases.CoreOne.Models.StaticData;
using Shared.CoreOne.Contracts.Menus;
using Sales.CoreOne.Models.Dtos.ViewModels;
using Shared.Helper.Identity;

namespace Purchases.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PurchaseInvoicesController : ControllerBase
    {
        private readonly IDatabaseTransaction _databaseTransaction;
        private readonly IStringLocalizer<HandelException> _exLocalizer;
        private readonly IPurchaseInvoiceHeaderService _purchaseInvoiceHeaderService;
        private readonly IPurchaseInvoiceService _purchaseInvoiceService;
        private readonly IPurchaseInvoiceDetailService _PurchaseInvoiceDetailService;
        private readonly IPurchaseInvoiceHandlingService _purchaseInvoiceHandlingService;
        private readonly IHandleApprovalRequestService _handleApprovalRequestService;
        private readonly IApprovalSystemService _approvalSystemService;
        private readonly IGenericMessageService _genericMessageService;
        private readonly IGetPurchaseInvoiceSettleValueService _getPurchaseInvoiceSettleValueService;
		private readonly IHttpContextAccessor _httpContextAccessor;

        public PurchaseInvoicesController(IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exLocalizer, IHandleApprovalRequestService handleApprovalRequestService, IApprovalSystemService approvalSystemService, IPurchaseInvoiceHeaderService purchaseInvoiceHeaderService, IPurchaseInvoiceService purchaseInvoiceService, IPurchaseInvoiceDetailService purchaseInvoiceDetailService, IPurchaseInvoiceHandlingService purchaseInvoiceHandlingService, IGenericMessageService genericMessageService, IGetPurchaseInvoiceSettleValueService getPurchaseInvoiceSettleValueService, IHttpContextAccessor httpContextAccessor)
        {
            _databaseTransaction = databaseTransaction;
            _exLocalizer = exLocalizer;
            _purchaseInvoiceHeaderService = purchaseInvoiceHeaderService;
            _purchaseInvoiceService = purchaseInvoiceService;
            _PurchaseInvoiceDetailService = purchaseInvoiceDetailService;
            _handleApprovalRequestService = handleApprovalRequestService;
            _approvalSystemService = approvalSystemService;
            _purchaseInvoiceHandlingService = purchaseInvoiceHandlingService;
            _genericMessageService = genericMessageService;
            _getPurchaseInvoiceSettleValueService = getPurchaseInvoiceSettleValueService;
			_httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("GetLastPurchasePrice")]
        public async Task<IActionResult> GetLastPurchasePrice(int itemId, int itemPackageId)
        {
            var data = await _purchaseInvoiceService.GetLastPurchasePrice(itemId, itemPackageId);
            return Ok(data);
        }

        [HttpGet]
        [Route("ReadPurchaseInvoices")]
        public async Task<IActionResult> ReadPurchaseInvoices(DataSourceLoadOptions loadOptions, int menuCode)
        {
            try
            {
                var data = await DataSourceLoader.LoadAsync(_purchaseInvoiceHeaderService.GetUserPurchaseInvoiceHeaders(menuCode), loadOptions);
                return Ok(data);
            }
            catch
            {
                var model = _purchaseInvoiceHeaderService.GetUserPurchaseInvoiceHeaders(menuCode);
                var data = DataSourceLoader.Load(model.ToList(), loadOptions);
                return Ok(data);
            }
        }

        [HttpGet]
        [Route("ReadPurchaseInvoicesByStoreId")]
        public async Task<IActionResult> ReadPurchaseInvoicesByStoreId(DataSourceLoadOptions loadOptions, int storeId, int? supplierId, int purchaseInvoiceHeaderId, bool? isOnTheWay = null)
        {
            try
            {
                var data = await DataSourceLoader.LoadAsync(_purchaseInvoiceHeaderService.GetPurchaseInvoiceHeadersByStoreId(storeId, supplierId, purchaseInvoiceHeaderId, isOnTheWay), loadOptions);
                return Ok(data);
            }
            catch
            {
                var model = _purchaseInvoiceHeaderService.GetPurchaseInvoiceHeadersByStoreId(storeId, supplierId, purchaseInvoiceHeaderId, isOnTheWay);
                var data = DataSourceLoader.Load(model.ToList(), loadOptions);
                return Ok(data);
            }
        }

        [HttpGet]
        [Route("ReadPurchaseInvoicesByStoreIdAndMenuCode")]
        public async Task<IActionResult> ReadPurchaseInvoicesByStoreIdAndMenuCode(DataSourceLoadOptions loadOptions, int storeId, int? supplierId, int menuCode, int purchaseInvoiceHeaderId)
        {
            try
            {
                var data = await DataSourceLoader.LoadAsync(_purchaseInvoiceHandlingService.GetPurchaseInvoiceHeadersByStoreIdAndMenuCode(storeId, supplierId, menuCode, purchaseInvoiceHeaderId), loadOptions);
                return Ok(data);
            }
            catch
            {
                var model = _purchaseInvoiceHandlingService.GetPurchaseInvoiceHeadersByStoreIdAndMenuCode(storeId, supplierId, menuCode, purchaseInvoiceHeaderId);
                var data = DataSourceLoader.Load(model.ToList(), loadOptions);
                return Ok(data);
            }
		}

		[HttpGet]
        [Route("GetAvailableValueForSupplierDebitMemo")]
		public async Task<IActionResult> GetAvailableValueForSupplierDebitMemo(int purchaseInvoiceHeaderId, int supplierDebitMemoId)
		{
			var availableValue = await _getPurchaseInvoiceSettleValueService.GetPurchaseInvoiceValueMinusSettledValue(purchaseInvoiceHeaderId, supplierDebitMemoId);
			return Ok(availableValue);
		}

		[HttpGet]
        [Route("GetPurchaseInvoiceCode")]
        public async Task<IActionResult> GetPurchaseInvoiceCode(int storeId, DateTime documentDate, bool isOnTheWay, bool isDirectInvoice, bool creditPayment)
        {
            var data = await _purchaseInvoiceHeaderService.GetPurchaseInvoiceCode(storeId, documentDate, isOnTheWay, isDirectInvoice, creditPayment);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetPurchaseInvoiceFromPurchaseOrder")]
        public async Task<IActionResult> GetPurchaseInvoiceFromPurchaseOrder(int purchaseOrderHeaderId)
        {
            var data = await _purchaseInvoiceHandlingService.GetPurchaseInvoiceFromPurchaseOrder(purchaseOrderHeaderId);
            return Ok(data);
        }

		[HttpGet]
		[Route("GetPurchaseInvoiceFromSupplierQuotation")]
		public async Task<IActionResult> GetPurchaseInvoiceFromSupplierQuotation(int supplierQuotationHeaderId)
		{
			var data = await _purchaseInvoiceHandlingService.GetPurchaseInvoiceFromSupplierQuotation(supplierQuotationHeaderId);
			return Ok(data);
		}

		[HttpGet]
        [Route("GetRequestData")]
        public async Task<IActionResult> GetRequestData(int requestId)
        {
            var data = await _handleApprovalRequestService.GetRequestData(requestId);
            if (data != null)
            {
                if (data.CanBeConverted<PurchaseInvoiceDto>())
                {
					var purchaseInvoice = data.ConvertToType<PurchaseInvoiceDto>();
					var userCanLook = await _httpContextAccessor.UserCanLook(0, purchaseInvoice?.PurchaseInvoiceHeader?.StoreId ?? 0);
                    return Ok(userCanLook ? purchaseInvoice : new PurchaseInvoiceDto());
                }
            }
            return Ok(new PurchaseInvoiceDto());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPurchaseInvoice(int id)
        {
            var purchaseInvoice = await _purchaseInvoiceService.GetPurchaseInvoice(id);
			var userCanLook = await _httpContextAccessor.UserCanLook(0, purchaseInvoice.PurchaseInvoiceHeader?.StoreId ?? 0);
            return Ok(userCanLook ? purchaseInvoice : new PurchaseInvoiceDto());
        }

        [HttpGet]
        [Route("GetPurchaseInvoiceHeader")]
        public async Task<IActionResult> GetPurchaseInvoiceHeader(int id)
        {
            var purchaseInvoiceHeader = await _purchaseInvoiceHeaderService.GetPurchaseInvoiceHeaderById(id);
            return Ok(purchaseInvoiceHeader);
        }

        [HttpGet]
        [Route("GetPurchaseInvoiceDetail")]
        public async Task<IActionResult> GetPurchaseInvoiceDetail(int id)
        {
            var purchaseInvoiceDetail = await _PurchaseInvoiceDetailService.GetPurchaseInvoiceDetails(id);
            return Ok(purchaseInvoiceDetail);
        }

        [HttpPost]
        [Route("Save")]
        public async Task<IActionResult> Save([FromBody] PurchaseInvoiceDto model, int requestId)
        {
            var response = new ResponseDto();
            try
            {
                if (model.PurchaseInvoiceHeader != null)
                {
                    var hasApprove = await _approvalSystemService.IsMenuHasApprove(PurchaseInvoiceMenuCodeHelper.GetMenuCode(model.PurchaseInvoiceHeader));
                    if (hasApprove.HasApprove)
                    {
                        if (hasApprove.OnAdd && model.PurchaseInvoiceHeader!.PurchaseInvoiceHeaderId == 0)
                        {
                            response = await SendApproveRequest(model, null, requestId, 0, false);
                        }
                        else if (hasApprove.OnEdit && model.PurchaseInvoiceHeader!.PurchaseInvoiceHeaderId > 0)
                        {
                            var oldValue = await _purchaseInvoiceService.GetPurchaseInvoice(model.PurchaseInvoiceHeader!.PurchaseInvoiceHeaderId);
                            response = await SendApproveRequest(model, oldValue, requestId, model.PurchaseInvoiceHeader!.PurchaseInvoiceHeaderId, false);
                        }
                        else
                        {
                            response = await SavePurchaseInvoice(model, hasApprove.HasApprove, false, requestId);
                        }
                    }
                    else
                    {
                        response = await SavePurchaseInvoice(model, hasApprove.HasApprove, false, requestId);
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
        [Route("SavePurchaseInvoice")]
        public async Task<ResponseDto> SavePurchaseInvoice(PurchaseInvoiceDto model, bool hasApprove, bool approved, int? requestId)
        {
            await _databaseTransaction.BeginTransaction();
            var response = await _purchaseInvoiceHandlingService.SavePurchaseInvoice(model, hasApprove, approved, requestId);
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
        [Route("DeletePurchaseInvoice")]
        public async Task<IActionResult> DeletePurchaseInvoice(int purchaseInvoiceHeaderId, int requestId)
        {
            var response = new ResponseDto();
            try
            {
                var model = await _purchaseInvoiceService.GetPurchaseInvoice(purchaseInvoiceHeaderId);
                if (model.PurchaseInvoiceHeader != null)
                {
                    var hasApprove = await _approvalSystemService.IsMenuHasApprove(PurchaseInvoiceMenuCodeHelper.GetMenuCode(model.PurchaseInvoiceHeader));
                    if (hasApprove.HasApprove && hasApprove.OnDelete)
                    {
                        response = await SendApproveRequest(null, model, requestId, purchaseInvoiceHeaderId, true);
                    }
                    else
                    {
                        await _databaseTransaction.BeginTransaction();
                        response = await _purchaseInvoiceHandlingService.DeletePurchaseInvoice(purchaseInvoiceHeaderId, PurchaseInvoiceMenuCodeHelper.GetMenuCode(model.PurchaseInvoiceHeader));
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
                    response.Id = purchaseInvoiceHeaderId;
                    response.Message = await _genericMessageService.GetMessage(MenuCodeData.CashPurchaseInvoice, GenericMessageData.NotFound);
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
        public async Task<ResponseDto> SendApproveRequest([FromQuery] PurchaseInvoiceDto? newModel, [FromQuery] PurchaseInvoiceDto? oldModel, int requestId, int purchaseInvoiceHeaderId, bool isDelete)
        {
            var changes = new List<RequestChangesDto>();
            var menuCode = 0;
            var requestTypeId = isDelete ? ApproveRequestTypeData.Delete : (purchaseInvoiceHeaderId == 0 ? ApproveRequestTypeData.Add : ApproveRequestTypeData.Edit);

            if (requestTypeId == ApproveRequestTypeData.Edit || requestTypeId == ApproveRequestTypeData.Delete)
            {
                if (oldModel != null)
                {
                    if(newModel != null) 
                    {
                        changes = _purchaseInvoiceService.GetPurchaseInvoiceRequestChanges(oldModel, newModel);
                    }

                    if (oldModel.PurchaseInvoiceHeader != null)
                    {
                        menuCode = PurchaseInvoiceMenuCodeHelper.GetMenuCode(oldModel.PurchaseInvoiceHeader);
                    }
                }
            }
            else
            {
                if (newModel != null)
                {
                    if (newModel.PurchaseInvoiceHeader != null)
                    {
                        menuCode = PurchaseInvoiceMenuCodeHelper.GetMenuCode(newModel.PurchaseInvoiceHeader);
					}
                }
            }

            var request = new NewApproveRequestDto()
            {
                RequestId = requestId,
                MenuCode = (short)menuCode,
                ApproveRequestTypeId = requestTypeId,
                ReferenceId = purchaseInvoiceHeaderId,
                ReferenceCode = purchaseInvoiceHeaderId.ToString(),
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
