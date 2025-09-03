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
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne.Contracts.Menus;
using Shared.Helper.Identity;

namespace Purchases.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StockInReturnController : ControllerBase
    {
        private readonly IDatabaseTransaction _databaseTransaction;
        private readonly IStringLocalizer<HandelException> _exLocalizer;
        private readonly IStockInReturnHeaderService _stockInReturnHeaderService;
        private readonly IStockInReturnService _stockInReturnService;
        private readonly IStockInReturnHandlingService _stockInReturnHandlingService;
        private readonly IHandleApprovalRequestService _handleApprovalRequestService;
        private readonly IApprovalSystemService _approvalSystemService;
        private readonly IGenericMessageService _genericMessageService;
		private readonly IHttpContextAccessor _httpContextAccessor;

        public StockInReturnController(IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exLocalizer, IHandleApprovalRequestService handleApprovalRequestService, IApprovalSystemService approvalSystemService, IStockInReturnHeaderService stockInReturnHeaderService, IStockInReturnService stockInReturnService, IStockInReturnHandlingService stockInReturnHandlingService, IGenericMessageService genericMessageService, IHttpContextAccessor httpContextAccessor)
        {
            _databaseTransaction = databaseTransaction;
            _exLocalizer = exLocalizer;
            _stockInReturnHeaderService = stockInReturnHeaderService;
            _stockInReturnService = stockInReturnService;
            _stockInReturnHandlingService = stockInReturnHandlingService;
            _handleApprovalRequestService = handleApprovalRequestService;
            _approvalSystemService = approvalSystemService;
            _genericMessageService = genericMessageService;
			_httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("ReadStockInReturns")]
        public async Task<IActionResult> ReadStockInReturns(DataSourceLoadOptions loadOptions, int stockTypeId)
        {
            try
            {
                var data = await DataSourceLoader.LoadAsync(_stockInReturnHeaderService.GetUserStockInReturnHeaders(stockTypeId), loadOptions);
                return Ok(data);
            }
            catch
            {
                var model = _stockInReturnHeaderService.GetUserStockInReturnHeaders(stockTypeId);
                var data = DataSourceLoader.Load(model.ToList(), loadOptions);
                return Ok(data);
            }
        }

        [HttpGet]
        [Route("ReadStockInReturnsByStoreId")]
        public async Task<IActionResult> ReadStockInReturnsByStoreId(DataSourceLoadOptions loadOptions, int storeId, int? supplierId, int stockTypeId, int stockInReturnHeaderId)
        {
            try
            {
                var data = await DataSourceLoader.LoadAsync(_stockInReturnHeaderService.GetStockInReturnHeadersByStoreId(storeId, supplierId, stockTypeId, stockInReturnHeaderId), loadOptions);
                return Ok(data);
            }
            catch
            {
                var model = _stockInReturnHeaderService.GetStockInReturnHeadersByStoreId(storeId, supplierId, stockTypeId, stockInReturnHeaderId);
                var data = DataSourceLoader.Load(model.ToList(), loadOptions);
                return Ok(data);
            }
        }

        [HttpGet]
        [Route("GetStockInReturnCode")]
        public async Task<IActionResult> GetStockInReturnCode(int storeId, DateTime documentDate, int stockTypeId)
        {
            var data = await _stockInReturnHeaderService.GetStockInReturnCode(storeId, documentDate, stockTypeId);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetStockInReturnFromStockIn")]
        public async Task<IActionResult> GetStockInReturnFromStockIn(int stockInHeaderId)
        {
            var data = await _stockInReturnHandlingService.GetStockInReturnFromStockIn(stockInHeaderId);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetStockInReturnFromPurchaseInvoice")]
        public async Task<IActionResult> GetStockInReturnFromPurchaseInvoice(int purchaseInvoiceHeaderId)
        {
            var data = await _stockInReturnHandlingService.GetStockInReturnFromPurchaseInvoice(purchaseInvoiceHeaderId);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetRequestData")]
		public async Task<IActionResult> GetRequestData(int requestId)
		{
			var data = await _handleApprovalRequestService.GetRequestData(requestId);
			if (data != null && data.CanBeConverted<StockInReturnDto>())
			{
				var stockInReturn = data.ConvertToType<StockInReturnDto>();
				if (stockInReturn?.StockInReturnHeader == null) return Ok(new StockInReturnDto());
				
				var userCanLook = await _httpContextAccessor.UserCanLook(0, stockInReturn.StockInReturnHeader.StoreId);
				if (!userCanLook) return Ok(new StockInReturnDto());

				var stockInReturnHeader = stockInReturn.StockInReturnHeader;
				stockInReturn!.StockInReturnDetails = await _stockInReturnHandlingService.GetStockInReturnDetailsCalculated(
					stockInReturnHeader.StockInReturnHeaderId,
					stockInReturnHeader.StockInHeaderId,
					stockInReturnHeader.PurchaseInvoiceHeaderId,
					stockInReturn.StockInReturnDetails);

				return Ok(stockInReturn);
			}
			return Ok(new StockInReturnDto());
		}

		[HttpGet("{id:int}")]
        public async Task<IActionResult> GetStockInReturn(int id)
        {
            var stockInReturn = await _stockInReturnHandlingService.GetStockInReturn(id);
			var userCanLook = await _httpContextAccessor.UserCanLook(0, stockInReturn.StockInReturnHeader?.StoreId ?? 0);
            return Ok(userCanLook ? stockInReturn : new StockInReturnDto());
        }

        [HttpGet]
        [Route("GetStockInReturnDetail")]
        public async Task<IActionResult> GetStockInReturnDetail(int id)
        {
            var stockInReturnDetail = await _stockInReturnHandlingService.GetStockInReturnDetailsCalculated(id);
            return Ok(stockInReturnDetail);
        }

        [HttpGet]
        [Route("GetStockInReturnHeader")]
        public async Task<IActionResult> GetStockInReturnHeader(int id)
        {
            var stockInReturnHeader = await _stockInReturnHeaderService.GetStockInReturnHeaderById(id);
            return Ok(stockInReturnHeader);
        }

        [HttpPost]
        [Route("Save")]
        public async Task<IActionResult> Save([FromBody] StockInReturnDto model, int requestId)
        {
            var response = new ResponseDto();
            try
            {
                if (model.StockInReturnHeader != null)
                {
                    var hasApprove = await _approvalSystemService.IsMenuHasApprove(StockTypeData.ToMenuCode(model.StockInReturnHeader.StockTypeId));
                    if (hasApprove.HasApprove)
                    {
                        if (hasApprove.OnAdd && model.StockInReturnHeader!.StockInReturnHeaderId == 0)
                        {
                            response = await SendApproveRequest(model, null, requestId, 0, false);
                        }
                        else if (hasApprove.OnEdit && model.StockInReturnHeader!.StockInReturnHeaderId > 0)
                        {
                            var oldValue = await _stockInReturnHandlingService.GetStockInReturn(model.StockInReturnHeader!.StockInReturnHeaderId);
                            response = await SendApproveRequest(model, oldValue, requestId, model.StockInReturnHeader!.StockInReturnHeaderId, false);
                        }
                        else
                        {
                            response = await SaveStockInReturn(model, hasApprove.HasApprove, false, requestId);
                        }
                    }
                    else
                    {
                        response = await SaveStockInReturn(model, hasApprove.HasApprove, false, requestId);
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
        [Route("SaveStockInReturn")]
        public async Task<ResponseDto> SaveStockInReturn(StockInReturnDto model, bool hasApprove, bool approved, int? requestId)
        {
            await _databaseTransaction.BeginTransaction();
            var response = await _stockInReturnHandlingService.SaveStockInReturn(model, hasApprove, approved, requestId);
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
        [Route("DeleteStockInReturn")]
        public async Task<IActionResult> DeleteStockInReturn(int stockInReturnHeaderId, int requestId)
        {
            var response = new ResponseDto();
            try
            {
                var model = await _stockInReturnHandlingService.GetStockInReturn(stockInReturnHeaderId);
                if (model.StockInReturnHeader != null)
                {
                    var hasApprove = await _approvalSystemService.IsMenuHasApprove(StockTypeData.ToMenuCode(model.StockInReturnHeader.StockTypeId));
                    if (hasApprove.HasApprove && hasApprove.OnDelete)
                    {
                        response = await SendApproveRequest(null, model, requestId, stockInReturnHeaderId, true);
                    }
                    else
                    {
                        await _databaseTransaction.BeginTransaction();
                        response = await _stockInReturnHandlingService.DeleteStockInReturn(stockInReturnHeaderId, StockTypeData.ToMenuCode(model.StockInReturnHeader.StockTypeId));
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
                    response.Id = stockInReturnHeaderId;
                    response.Message = await _genericMessageService.GetMessage(MenuCodeData.ReceiptStatementReturn, GenericMessageData.NotFound); /*Have to pick some default menu code in this case*/
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
        public async Task<ResponseDto> SendApproveRequest([FromQuery] StockInReturnDto? newModel, [FromQuery] StockInReturnDto? oldModel, int requestId, int stockInReturnHeaderId, bool isDelete)
        {
            var changes = new List<RequestChangesDto>();
            var menuCode = 0;
            var requestTypeId = isDelete ? ApproveRequestTypeData.Delete : (stockInReturnHeaderId == 0 ? ApproveRequestTypeData.Add : ApproveRequestTypeData.Edit);
            if (requestTypeId == ApproveRequestTypeData.Edit || requestTypeId == ApproveRequestTypeData.Delete)
            {
                if (oldModel != null)
                {
                    if (newModel != null)
                    {
                        changes = _stockInReturnService.GetStockInReturnRequestChanges(oldModel, newModel!);
                    }

                    if (oldModel.StockInReturnHeader != null)
                    {
                        menuCode = StockTypeData.ToMenuCode(oldModel.StockInReturnHeader.StockTypeId);

                    }
                }
            }
            else
            {
                if (newModel != null)
                {
                    if (newModel.StockInReturnHeader != null)
                    {
                        menuCode = StockTypeData.ToMenuCode(newModel.StockInReturnHeader.StockTypeId);

                    }
                }
            }

            var request = new NewApproveRequestDto()
            {
                RequestId = requestId,
                MenuCode = (short)menuCode,
                ApproveRequestTypeId = requestTypeId,
                ReferenceId = stockInReturnHeaderId,
                ReferenceCode = stockInReturnHeaderId.ToString(),
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
