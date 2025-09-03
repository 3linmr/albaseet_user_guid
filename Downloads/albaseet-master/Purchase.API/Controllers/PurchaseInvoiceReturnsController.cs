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
using Shared.Helper.Identity;

namespace Purchases.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PurchaseInvoiceReturnsController : ControllerBase
    {
        private readonly IPurchaseInvoiceHeaderService _purchaseInvoiceHeaderService;
        private readonly IDatabaseTransaction _databaseTransaction;
        private readonly IStringLocalizer<HandelException> _exLocalizer;
        private readonly IPurchaseInvoiceReturnHeaderService _purchaseInvoiceReturnHeaderService;
        private readonly IPurchaseInvoiceReturnService _purchaseInvoiceReturnService;
        private readonly IPurchaseInvoiceReturnDetailService _purchaseInvoiceReturnDetailService;
        private readonly IPurchaseInvoiceReturnHandlingService _purchaseInvoiceReturnHandlingService;
        private readonly IHandleApprovalRequestService _handleApprovalRequestService;
        private readonly IApprovalSystemService _approvalSystemService;
        private readonly IGenericMessageService _genericMessageService;
		private readonly IHttpContextAccessor _httpContextAccessor;

        public PurchaseInvoiceReturnsController(IPurchaseInvoiceHeaderService purchaseInvoiceHeaderService, IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exLocalizer, IHandleApprovalRequestService handleApprovalRequestService, IApprovalSystemService approvalSystemService, IPurchaseInvoiceReturnHeaderService purchaseInvoiceReturnHeaderService, IPurchaseInvoiceReturnService purchaseInvoiceReturnService, IPurchaseInvoiceReturnDetailService purchaseInvoiceReturnDetailService, IPurchaseInvoiceReturnHandlingService purchaseInvoiceReturnHandlingService, IGenericMessageService genericMessageService, IHttpContextAccessor httpContextAccessor)
        {
            _purchaseInvoiceHeaderService = purchaseInvoiceHeaderService;
            _databaseTransaction = databaseTransaction;
            _exLocalizer = exLocalizer;
            _purchaseInvoiceReturnHeaderService = purchaseInvoiceReturnHeaderService;
            _purchaseInvoiceReturnService = purchaseInvoiceReturnService;
            _purchaseInvoiceReturnDetailService = purchaseInvoiceReturnDetailService;
            _handleApprovalRequestService = handleApprovalRequestService;
            _approvalSystemService = approvalSystemService;
            _purchaseInvoiceReturnHandlingService = purchaseInvoiceReturnHandlingService;
            _genericMessageService = genericMessageService;
			_httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("ReadPurchaseInvoiceReturns")]
        public async Task<IActionResult> ReadPurchaseInvoiceReturns(DataSourceLoadOptions loadOptions, int menuCode)
        {
            try
            {
                var data = await DataSourceLoader.LoadAsync(_purchaseInvoiceReturnHeaderService.GetUserPurchaseInvoiceReturnHeaders(menuCode), loadOptions);
                return Ok(data);
            }
            catch
            {
                var model = _purchaseInvoiceReturnHeaderService.GetUserPurchaseInvoiceReturnHeaders(menuCode);
                var data = DataSourceLoader.Load(model.ToList(), loadOptions);
                return Ok(data);
            }
        }

        [HttpGet]
        [Route("ReadPurchaseInvoiceReturnsByStoreId")]
        public async Task<IActionResult> ReadPurchaseInvoiceReturnsByStoreId(DataSourceLoadOptions loadOptions, int storeId, int? supplierId, int purchaseInvoiceReturnHeaderId)
        {
            try
            {
                var data = await DataSourceLoader.LoadAsync(_purchaseInvoiceReturnHeaderService.GetPurchaseInvoiceReturnHeadersByStoreId(storeId, supplierId, purchaseInvoiceReturnHeaderId), loadOptions);
                return Ok(data);
            }
            catch
            {
                var model = _purchaseInvoiceReturnHeaderService.GetPurchaseInvoiceReturnHeadersByStoreId(storeId, supplierId, purchaseInvoiceReturnHeaderId);
                var data = DataSourceLoader.Load(model.ToList(), loadOptions);
                return Ok(data);
            }
        }

        [HttpGet]
        [Route("GetPurchaseInvoiceReturnCode")]
        public async Task<IActionResult> GetPurchaseInvoiceReturnCode(int storeId, DateTime documentDate, bool isOnTheWay, bool isDirectInvoice, bool creditPayment)
        {
            var data = await _purchaseInvoiceReturnHeaderService.GetPurchaseInvoiceReturnCode(storeId, documentDate, isOnTheWay, isDirectInvoice, creditPayment);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetPurchaseInvoiceReturnFromPurchaseInvoice")]
        public async Task<IActionResult> GetPurchaseInvoiceReturnFromPurchaseInvoice(int purchaseInvoiceHeaderId, bool isDirectInvoice, bool isOnTheWay)
        {
            var data = await _purchaseInvoiceReturnHandlingService.GetPurchaseInvoiceReturnFromPurchaseInvoice(purchaseInvoiceHeaderId, isDirectInvoice, isOnTheWay);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetRequestData")]
        public async Task<IActionResult> GetRequestData(int requestId)
        {
			var data = await _handleApprovalRequestService.GetRequestData(requestId);
			if (data != null && data.CanBeConverted<PurchaseInvoiceReturnDto>())
			{
				var purchaseInvoiceReturn = data.ConvertToType<PurchaseInvoiceReturnDto>();
				if (purchaseInvoiceReturn?.PurchaseInvoiceReturnHeader == null) return Ok(new PurchaseInvoiceReturnDto());
				
				var userCanLook = await _httpContextAccessor.UserCanLook(0, purchaseInvoiceReturn.PurchaseInvoiceReturnHeader.StoreId);
				if (!userCanLook) return Ok(new PurchaseInvoiceReturnDto());

				var purchaseInvoiceReturnHeader = purchaseInvoiceReturn.PurchaseInvoiceReturnHeader;
				purchaseInvoiceReturn!.PurchaseInvoiceReturnDetails = await _purchaseInvoiceReturnService.GetPurchaseInvoiceReturnDetailsCalculated(
					purchaseInvoiceReturnHeader.PurchaseInvoiceReturnHeaderId,
					purchaseInvoiceReturnHeader,
					purchaseInvoiceReturn.PurchaseInvoiceReturnDetails);

				return Ok(purchaseInvoiceReturn);
			}
			return Ok(new PurchaseInvoiceReturnDto());
		}

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPurchaseInvoiceReturn(int id)
        {
            var purchaseInvoiceReturn = await _purchaseInvoiceReturnService.GetPurchaseInvoiceReturn(id);
			var userCanLook = await _httpContextAccessor.UserCanLook(0, purchaseInvoiceReturn.PurchaseInvoiceReturnHeader?.StoreId ?? 0);
            return Ok(userCanLook ? purchaseInvoiceReturn : new PurchaseInvoiceReturnDto());
        }

        [HttpGet]
        [Route("GetPurchaseInvoiceReturnDetail")]
        public async Task<IActionResult> GetPurchaseInvoiceReturnDetail(int id)
        {
            var purchaseInvoiceReturnDetail = await _purchaseInvoiceReturnService.GetPurchaseInvoiceReturnDetailsCalculated(id);
            return Ok(purchaseInvoiceReturnDetail);
        }

        [HttpGet]
        [Route("GetPurchaseInvoiceReturnHeader")]
        public async Task<IActionResult> GetPurchaseInvoiceReturnHeader(int id)
        {
            var purchaseInvoiceReturnHeader = await _purchaseInvoiceReturnHeaderService.GetPurchaseInvoiceReturnHeaderById(id);
            return Ok(purchaseInvoiceReturnHeader);
        }

        [HttpPost]
        [Route("Save")]
        public async Task<IActionResult> Save([FromBody] PurchaseInvoiceReturnDto model, int requestId)
        {
            var response = new ResponseDto();
            try
            {
                if (model.PurchaseInvoiceReturnHeader != null)
                {
                    var menuCode = PurchaseInvoiceReturnMenuCodeHelper.GetMenuCode(model.PurchaseInvoiceReturnHeader);
					var hasApprove = await _approvalSystemService.IsMenuHasApprove(menuCode);
                    if (hasApprove.HasApprove)
                    {
                        if (hasApprove.OnAdd && model.PurchaseInvoiceReturnHeader!.PurchaseInvoiceReturnHeaderId == 0)
                        {
                            response = await SendApproveRequest(model, null, requestId, 0, false, menuCode);
                        }
                        else if (hasApprove.OnEdit && model.PurchaseInvoiceReturnHeader!.PurchaseInvoiceReturnHeaderId > 0)
                        {
                            var oldValue = await _purchaseInvoiceReturnService.GetPurchaseInvoiceReturn(model.PurchaseInvoiceReturnHeader!.PurchaseInvoiceReturnHeaderId);
                            response = await SendApproveRequest(model, oldValue, requestId, model.PurchaseInvoiceReturnHeader!.PurchaseInvoiceReturnHeaderId, false, menuCode);
                        }
                        else
                        {
                            response = await SavePurchaseInvoiceReturn(model, hasApprove.HasApprove, false, requestId);
                        }
                    }
                    else
                    {
                        response = await SavePurchaseInvoiceReturn(model, hasApprove.HasApprove, false, requestId);
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
        [Route("SavePurchaseInvoiceReturn")]
        public async Task<ResponseDto> SavePurchaseInvoiceReturn(PurchaseInvoiceReturnDto model, bool hasApprove, bool approved, int? requestId)
        {
            await _databaseTransaction.BeginTransaction();
            var response = await _purchaseInvoiceReturnHandlingService.SavePurchaseInvoiceReturn(model, hasApprove, approved, requestId);
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
        [Route("DeletePurchaseInvoiceReturn")]
        public async Task<IActionResult> DeletePurchaseInvoiceReturn(int purchaseInvoiceReturnHeaderId, int requestId)
        {
            var response = new ResponseDto();
            try
            {
                var model = await _purchaseInvoiceReturnService.GetPurchaseInvoiceReturn(purchaseInvoiceReturnHeaderId);
                if (model.PurchaseInvoiceReturnHeader != null)
                {
                    var menuCode = PurchaseInvoiceReturnMenuCodeHelper.GetMenuCode(model.PurchaseInvoiceReturnHeader);
					var hasApprove = await _approvalSystemService.IsMenuHasApprove(menuCode);
                    if (hasApprove.HasApprove && hasApprove.OnDelete)
                    {
                        response = await SendApproveRequest(null, model, requestId, purchaseInvoiceReturnHeaderId, true, menuCode);
                    }
                    else
                    {
                        await _databaseTransaction.BeginTransaction();
                        response = await _purchaseInvoiceReturnHandlingService.DeletePurchaseInvoiceReturn(purchaseInvoiceReturnHeaderId, menuCode);
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
                    response.Id = purchaseInvoiceReturnHeaderId;
                    response.Message = await _genericMessageService.GetMessage(MenuCodeData.PurchaseInvoiceReturn, GenericMessageData.NotFound);
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
        public async Task<ResponseDto> SendApproveRequest([FromQuery] PurchaseInvoiceReturnDto? newModel, [FromQuery] PurchaseInvoiceReturnDto? oldModel, int requestId, int purchaseInvoiceReturnHeaderId, bool isDelete, int menuCode)
        {
            var changes = new List<RequestChangesDto>();
            var requestTypeId = isDelete ? ApproveRequestTypeData.Delete : (purchaseInvoiceReturnHeaderId == 0 ? ApproveRequestTypeData.Add : ApproveRequestTypeData.Edit);

            if (requestTypeId == ApproveRequestTypeData.Edit || requestTypeId == ApproveRequestTypeData.Delete)
            {
                if (oldModel != null && newModel != null)
                {
                     changes = _purchaseInvoiceReturnService.GetPurchaseInvoiceReturnRequestChanges(oldModel, newModel);
                }
            }

            var request = new NewApproveRequestDto()
            {
                RequestId = requestId,
                MenuCode = (short)menuCode,
                ApproveRequestTypeId = requestTypeId,
                ReferenceId = purchaseInvoiceReturnHeaderId,
                ReferenceCode = purchaseInvoiceReturnHeaderId.ToString(),
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
