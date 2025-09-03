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
    public class StockInController : ControllerBase
    {
        private readonly IDatabaseTransaction _databaseTransaction;
        private readonly IStringLocalizer<HandelException> _exLocalizer;
        private readonly IStockInHeaderService _stockInHeaderService;
        private readonly IStockInService _stockInService;
        private readonly IStockInHandlingService _stockInHandlingService;
        private readonly IHandleApprovalRequestService _handleApprovalRequestService;
        private readonly IApprovalSystemService _approvalSystemService;
        private readonly IGenericMessageService _genericMessageService;
		private readonly IHttpContextAccessor _httpContextAccessor;

        public StockInController(IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exLocalizer, IHandleApprovalRequestService handleApprovalRequestService, IApprovalSystemService approvalSystemService, IStockInHeaderService stockInHeaderService, IStockInService stockInService, IStockInHandlingService stockInHandlingService, IGenericMessageService genericMessageService, IHttpContextAccessor httpContextAccessor)
        {
            _databaseTransaction = databaseTransaction;
            _exLocalizer = exLocalizer;
            _stockInHeaderService = stockInHeaderService;
            _stockInService = stockInService;
            _stockInHandlingService = stockInHandlingService;
            _handleApprovalRequestService = handleApprovalRequestService;
            _approvalSystemService = approvalSystemService;
            _genericMessageService = genericMessageService;
			_httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("ReadStockIns")]
        public async Task<IActionResult> ReadStockIns(DataSourceLoadOptions loadOptions, int stockTypeId)
        {
            try
            {
                var data = await DataSourceLoader.LoadAsync(_stockInHeaderService.GetUserStockInHeaders(stockTypeId), loadOptions);
                return Ok(data);
            }
            catch
            {
                var model = _stockInHeaderService.GetUserStockInHeaders(stockTypeId);
                var data = DataSourceLoader.Load(model.ToList(), loadOptions);
                return Ok(data);
            }
        }

        [HttpGet]
        [Route("ReadStockInsByStoreId")]
        public async Task<IActionResult> ReadStockInsByStoreId(DataSourceLoadOptions loadOptions, int storeId, int? supplierId, int stockTypeId, int stockInHeaderId)
        {
            try
            {
                var data = await DataSourceLoader.LoadAsync(_stockInHandlingService.GetStockInHeadersByStoreId(storeId, supplierId, stockTypeId, stockInHeaderId), loadOptions);
                return Ok(data);
            }
            catch
            {
                var model = _stockInHandlingService.GetStockInHeadersByStoreId(storeId, supplierId, stockTypeId, stockInHeaderId);
                var data = DataSourceLoader.Load(model.ToList(), loadOptions);
                return Ok(data);
            }
        }

        [HttpGet]
        [Route("GetStockInCode")]
        public async Task<IActionResult> GetStockInCode(int storeId, DateTime documentDate, int stockTypeId)
        {
            var data = await _stockInHeaderService.GetStockInCode(storeId, documentDate, stockTypeId);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetStockInFromPurchaseOrder")]
        public async Task<IActionResult> GetStockInFromPurchaseOrder(int purchaseOrderHeaderId)
        {
            var data = await _stockInHandlingService.GetStockInFromPurchaseOrder(purchaseOrderHeaderId);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetStockInFromPurchaseInvoice")]
        public async Task<IActionResult> GetStockInFromPurchaseInvoice(int purchaseInvoiceHeaderId)
        {
            var data = await _stockInHandlingService.GetStockInFromPurchaseInvoice(purchaseInvoiceHeaderId);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetRequestData")]
        public async Task<IActionResult> GetRequestData(int requestId)
        {
			var data = await _handleApprovalRequestService.GetRequestData(requestId);
			if (data != null && data.CanBeConverted<StockInDto>())
			{
				var stockIn = data.ConvertToType<StockInDto>();
				if (stockIn?.StockInHeader == null) return Ok(new StockInDto());
				
				var userCanLook = await _httpContextAccessor.UserCanLook(0, stockIn.StockInHeader.StoreId);
				if (!userCanLook) return Ok(new StockInDto());

				var stockInHeader = stockIn.StockInHeader;
				stockIn!.StockInDetails = await _stockInHandlingService.GetStockInDetailsCalculated(
					stockInHeader.StockInHeaderId, 
                    stockInHeader.PurchaseOrderHeaderId,
                    stockInHeader.PurchaseInvoiceHeaderId,
                    stockIn.StockInDetails);

				return Ok(stockIn);
			}
			return Ok(new StockInDto());
		}

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetStockIn(int id)
        {
            var stockIn = await _stockInHandlingService.GetStockIn(id);
			var userCanLook = await _httpContextAccessor.UserCanLook(0, stockIn.StockInHeader?.StoreId ?? 0);
            return Ok(userCanLook ? stockIn : new StockInDto());
        }

        [HttpGet]
        [Route("GetStockInDetail")]
        public async Task<IActionResult> GetStockInDetail(int id)
        {
            var stockInDetail = await _stockInHandlingService.GetStockInDetailsCalculated(id);
            return Ok(stockInDetail);
        }

        [HttpGet]
        [Route("GetStockInHeader")]
        public async Task<IActionResult> GetStockInHeader(int id)
        {
            var stockInHeader = await _stockInHeaderService.GetStockInHeaderById(id);
            return Ok(stockInHeader);
        }

        [HttpPost]
        [Route("Save")]
        public async Task<IActionResult> Save([FromBody] StockInDto model, int requestId)
        {
            var response = new ResponseDto();
            try
            {
                if (model.StockInHeader != null)
                {
                    var hasApprove = await _approvalSystemService.IsMenuHasApprove(StockTypeData.ToMenuCode(model.StockInHeader!.StockTypeId));
                    if (hasApprove.HasApprove)
                    {
                        if (hasApprove.OnAdd && model.StockInHeader!.StockInHeaderId == 0)
                        {
                            response = await SendApproveRequest(model, null, requestId, 0, false);
                        }
                        else if (hasApprove.OnEdit && model.StockInHeader!.StockInHeaderId > 0)
                        {
                            var oldValue = await _stockInHandlingService.GetStockIn(model.StockInHeader!.StockInHeaderId);
                            response = await SendApproveRequest(model, oldValue, requestId, model.StockInHeader!.StockInHeaderId, false);
                        }
                        else
                        {
                            response = await SaveStockIn(model, hasApprove.HasApprove, false, requestId);
                        }
                    }
                    else
                    {
                        response = await SaveStockIn(model, hasApprove.HasApprove, false, requestId);
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
        [Route("SaveStockIn")]
        public async Task<ResponseDto> SaveStockIn(StockInDto model, bool hasApprove, bool approved, int? requestId)
        {
            await _databaseTransaction.BeginTransaction();
            var response = await _stockInHandlingService.SaveStockIn(model, hasApprove, approved, requestId);
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
        [Route("DeleteStockIn")]
        public async Task<IActionResult> DeleteStockIn(int stockInHeaderId, int requestId)
        {
            var response = new ResponseDto();
            try
            {
                var model = await _stockInHandlingService.GetStockIn(stockInHeaderId);
                if (model.StockInHeader != null)
                {
                    var hasApprove = await _approvalSystemService.IsMenuHasApprove(StockTypeData.ToMenuCode(model.StockInHeader.StockTypeId));
                    if (hasApprove.HasApprove && hasApprove.OnDelete)
                    {
                        response = await SendApproveRequest(null, model, requestId, stockInHeaderId, true);
                    }
                    else
                    {
                        await _databaseTransaction.BeginTransaction();
                        response = await _stockInHandlingService.DeleteStockIn(stockInHeaderId, StockTypeData.ToMenuCode(model.StockInHeader.StockTypeId));
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
                    response.Id = stockInHeaderId;
                    response.Message = await _genericMessageService.GetMessage(MenuCodeData.ReceiptStatement , GenericMessageData.NotFound); /*Have to pick some default menu code in this case*/
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
        public async Task<ResponseDto> SendApproveRequest([FromQuery] StockInDto? newModel, [FromQuery] StockInDto? oldModel, int requestId, int stockInHeaderId, bool isDelete)
        {
            var changes = new List<RequestChangesDto>();
            var menuCode = 0;
            var requestTypeId = isDelete ? ApproveRequestTypeData.Delete : (stockInHeaderId == 0 ? ApproveRequestTypeData.Add : ApproveRequestTypeData.Edit);
            if (requestTypeId == ApproveRequestTypeData.Edit || requestTypeId == ApproveRequestTypeData.Delete)
            {
                if (oldModel != null)
                {
                    if (newModel != null)
                    {
                        changes = _stockInService.GetStockInRequestChanges(oldModel, newModel);
                    }

                    if (oldModel.StockInHeader != null)
                    {
                        menuCode = StockTypeData.ToMenuCode(oldModel.StockInHeader.StockTypeId);

                    }
                }
            }
            else
            {
                if (newModel != null)
                {
                    if (newModel.StockInHeader != null)
                    {
                        menuCode = StockTypeData.ToMenuCode(newModel.StockInHeader.StockTypeId);

                    }
                }
            }

            var request = new NewApproveRequestDto()
            {
                RequestId = requestId,
                MenuCode = (short)menuCode,
                ApproveRequestTypeId = requestTypeId,
                ReferenceId = stockInHeaderId,
                ReferenceCode = stockInHeaderId.ToString(),
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
