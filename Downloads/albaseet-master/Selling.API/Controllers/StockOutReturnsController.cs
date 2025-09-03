using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using DevExtreme.AspNet.Data;
using Shared.Helper.Models.Dtos;
using Shared.CoreOne.Models.StaticData;
using Shared.CoreOne.Contracts.Approval;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.CoreOne;
using Shared.Helper.Database;
using Shared.Helper.Extensions;
using Sales.API.Models;
using Sales.CoreOne.Contracts;
using Sales.CoreOne.Models.Dtos.ViewModels;
using Sales.Service.Services;
using Shared.CoreOne.Contracts.Menus;
using Shared.Helper.Identity;

namespace Sales.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StockOutReturnsController: ControllerBase
    {
        private readonly IDatabaseTransaction _databaseTransaction;
        private readonly IStringLocalizer<HandelException> _exLocalizer;
        private readonly IStockOutReturnHeaderService _stockOutReturnHeaderService;
        private readonly IStockOutReturnHandlingService _stockOutReturnHandlingService;
        private readonly IStockOutReturnService _stockOutReturnService;
        private readonly IStockOutReturnDetailService _stockOutReturnDetailService;
        private readonly IHandleApprovalRequestService _handleApprovalRequestService;
        private readonly IApprovalSystemService _approvalSystemService;
        private readonly IGenericMessageService _genericMessageService;
		private readonly IHttpContextAccessor _httpContextAccessor;

        public StockOutReturnsController(IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exLocalizer, IHandleApprovalRequestService handleApprovalRequestService, IApprovalSystemService approvalSystemService, IStockOutReturnHeaderService stockOutReturnHeaderService, IStockOutReturnHandlingService stockOutReturnHandlingService, IStockOutReturnService stockOutReturnService, IStockOutReturnDetailService stockOutReturnDetailService, IGenericMessageService genericMessageService, IHttpContextAccessor httpContextAccessor)
        {
            _databaseTransaction = databaseTransaction;
            _exLocalizer = exLocalizer;
            _stockOutReturnHeaderService = stockOutReturnHeaderService;
            _stockOutReturnHandlingService = stockOutReturnHandlingService;
            _stockOutReturnService = stockOutReturnService;
            _stockOutReturnDetailService = stockOutReturnDetailService;
            _handleApprovalRequestService = handleApprovalRequestService;
            _approvalSystemService = approvalSystemService;
            _genericMessageService = genericMessageService;
			_httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("ReadStockOutReturns")]
        public async Task<IActionResult> ReadStockOutReturns(DataSourceLoadOptions loadOptions, int stockTypeId)
        {
            try
            {
                var data = await DataSourceLoader.LoadAsync(_stockOutReturnHeaderService.GetUserStockOutReturnHeaders(stockTypeId), loadOptions);
                return Ok(data);
            }
            catch
            {
                var model = _stockOutReturnHeaderService.GetUserStockOutReturnHeaders(stockTypeId);
                var data = DataSourceLoader.Load(model.ToList(), loadOptions);
                return Ok(data);
            }
        }

        [HttpGet]
        [Route("ReadStockOutReturnsByStoreId")]
        public async Task<IActionResult> ReadStockOutReturnsByStoreId(DataSourceLoadOptions loadOptions, int storeId, int? clientId, int stockTypeId, int stockOutReturnHeaderId)
        {
            try
            {
                var data = await DataSourceLoader.LoadAsync(_stockOutReturnHeaderService.GetStockOutReturnHeadersByStoreId(storeId, clientId, stockTypeId, stockOutReturnHeaderId), loadOptions);
                return Ok(data);
            }
            catch
            {
                var model = _stockOutReturnHeaderService.GetStockOutReturnHeadersByStoreId(storeId, clientId, stockTypeId, stockOutReturnHeaderId);
                var data = DataSourceLoader.Load(model.ToList(), loadOptions);
                return Ok(data);
            }
        }

        [HttpGet]
        [Route("GetStockOutReturnFromStockOut")]
        public async Task<IActionResult> GetStockOutReturnFromStockOut(int stockOutHeaderId)
        {
            var data = await _stockOutReturnHandlingService.GetStockOutReturnFromStockOut(stockOutHeaderId);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetStockOutReturnFromSalesInvoice")]
        public async Task<IActionResult> GetStockOutReturnFromSalesInvoice(int salesInvoiceHeaderId)
        {
            var data = await _stockOutReturnHandlingService.GetStockOutReturnFromSalesInvoice(salesInvoiceHeaderId);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetStockOutReturnCode")]
        public async Task<IActionResult> GetStockOutReturnCode(int storeId, DateTime documentDate, int stockTypeId)
        {
            var data = await _stockOutReturnHeaderService.GetStockOutReturnCode(storeId, documentDate, stockTypeId);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetRequestData")]
		public async Task<IActionResult> GetRequestData(int requestId)
		{
			var data = await _handleApprovalRequestService.GetRequestData(requestId);
			if (data != null && data.CanBeConverted<StockOutReturnDto>())
			{
				var stockOutReturn = data.ConvertToType<StockOutReturnDto>();
				if (stockOutReturn?.StockOutReturnHeader == null) return Ok(new StockOutReturnDto());
				
				var userCanLook = await _httpContextAccessor.UserCanLook(0, stockOutReturn.StockOutReturnHeader.StoreId);
				if (!userCanLook) return Ok(new StockOutReturnDto());

				var stockOutReturnHeader = stockOutReturn.StockOutReturnHeader;
				stockOutReturn!.StockOutReturnDetails = await _stockOutReturnHandlingService.ModifyStockOutReturnDetailsWithLiveAvailableQuantity(
					stockOutReturnHeader.StockOutReturnHeaderId,
					stockOutReturnHeader.StockOutHeaderId,
					stockOutReturnHeader.SalesInvoiceHeaderId, stockOutReturn.StockOutReturnDetails);

				return Ok(stockOutReturn);
			}
			return Ok(new StockOutReturnDto());
		}

		[HttpGet("{id:int}")]
        public async Task<IActionResult> GetStockOutReturn(int id)
        {
            var stockOutReturn = await _stockOutReturnHandlingService.GetStockOutReturn(id);
			var userCanLook = await _httpContextAccessor.UserCanLook(0, stockOutReturn.StockOutReturnHeader?.StoreId ?? 0);
            return Ok(userCanLook ? stockOutReturn : new StockOutReturnDto());
        }

        [HttpGet]
        [Route("GetStockOutReturnDetail")]
        public async Task<IActionResult> GetStockOutReturnDetail(int id)
        {
            var stockOutReturnDetail = await _stockOutReturnHandlingService.GetStockOutReturnDetailsCalculated(id);
            return Ok(stockOutReturnDetail);
        }

        [HttpGet]
        [Route("GetStockOutReturnHeader")]
        public async Task<IActionResult> GetStockOutReturnHeader(int id)
        {
            var stockOutReturnHeader = await _stockOutReturnHeaderService.GetStockOutReturnHeaderById(id);
            return Ok(stockOutReturnHeader);
        }

        [HttpPost]
        [Route("Save")]
        public async Task<IActionResult> Save([FromBody] StockOutReturnDto model, int requestId)
        {
            var response = new ResponseDto();
            try
            {
                if (model.StockOutReturnHeader != null)
                {
                    var hasApprove = await _approvalSystemService.IsMenuHasApprove(StockTypeData.ToMenuCode(model.StockOutReturnHeader.StockTypeId));
                    if (hasApprove.HasApprove)
                    {
                        if (hasApprove.OnAdd && model.StockOutReturnHeader!.StockOutReturnHeaderId == 0)
                        {
                            response = await SendApproveRequest(model, null, requestId, 0, false);
                        }
                        else if (hasApprove.OnEdit && model.StockOutReturnHeader!.StockOutReturnHeaderId > 0)
                        {
                            var oldValue = await _stockOutReturnHandlingService.GetStockOutReturn(model.StockOutReturnHeader!.StockOutReturnHeaderId);
                            response = await SendApproveRequest(model, oldValue, requestId, model.StockOutReturnHeader!.StockOutReturnHeaderId, false);
                        }
                        else
                        {
                            response = await SaveStockOutReturn(model, hasApprove.HasApprove, false, requestId);
                        }
                    }
                    else
                    {
                        response = await SaveStockOutReturn(model, hasApprove.HasApprove, false, requestId);
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
        [Route("SaveStockOutReturn")]
        public async Task<ResponseDto> SaveStockOutReturn(StockOutReturnDto model, bool hasApprove, bool approved, int? requestId)
        {
            await _databaseTransaction.BeginTransaction();
            var response = await _stockOutReturnHandlingService.SaveStockOutReturn(model, hasApprove, approved, requestId);
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
        [Route("DeleteStockOutReturn")]
        public async Task<IActionResult> DeleteStockOutReturn(int stockOutReturnHeaderId, int requestId)
        {
            var response = new ResponseDto();
            try
            {
                var model = await _stockOutReturnHandlingService.GetStockOutReturn(stockOutReturnHeaderId);
                if (model.StockOutReturnHeader != null)
                {
                    var hasApprove = await _approvalSystemService.IsMenuHasApprove(StockTypeData.ToMenuCode(model.StockOutReturnHeader.StockTypeId));
                    if (hasApprove.HasApprove && hasApprove.OnDelete)
                    {
                        response = await SendApproveRequest(null, model, requestId, stockOutReturnHeaderId, true);
                    }
                    else
                    {
                        await _databaseTransaction.BeginTransaction();
                        response = await _stockOutReturnHandlingService.DeleteStockOutReturn(stockOutReturnHeaderId, StockTypeData.ToMenuCode(model.StockOutReturnHeader.StockTypeId));
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
                    response.Id = stockOutReturnHeaderId;
                    response.Message = await _genericMessageService.GetMessage(MenuCodeData.StockOutReturnFromStockOut, GenericMessageData.NotFound);
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
        public async Task<ResponseDto> SendApproveRequest([FromQuery] StockOutReturnDto? newModel, [FromQuery] StockOutReturnDto? oldModel, int requestId, int stockOutReturnHeaderId, bool isDelete)
        {
            var changes = new List<RequestChangesDto>();
            var menuCode = 0;
            var requestTypeId = isDelete ? ApproveRequestTypeData.Delete : (stockOutReturnHeaderId == 0 ? ApproveRequestTypeData.Add : ApproveRequestTypeData.Edit);
            if (requestTypeId == ApproveRequestTypeData.Edit || requestTypeId == ApproveRequestTypeData.Delete)
            {
                if (oldModel != null)
                {
                    if (newModel != null)
                    {
                        changes = _stockOutReturnService.GetStockOutReturnRequestChanges(oldModel, newModel);
                    }

                    if (oldModel.StockOutReturnHeader != null)
                    {
                        menuCode = StockTypeData.ToMenuCode(oldModel.StockOutReturnHeader.StockTypeId);

                    }
                }
            }
            else
            {
                if (newModel != null)
                {
                    if (newModel.StockOutReturnHeader != null)
                    {
                        menuCode = StockTypeData.ToMenuCode(newModel.StockOutReturnHeader.StockTypeId);

                    }
                }
            }

            var request = new NewApproveRequestDto()
            {
                RequestId = requestId,
                MenuCode = (short)menuCode,
                ApproveRequestTypeId = requestTypeId,
                ReferenceId = stockOutReturnHeaderId,
                ReferenceCode = stockOutReturnHeaderId.ToString(),
                OldValue = oldModel,
                NewValue = newModel,
                Changes = changes
            };
            await _databaseTransaction.BeginTransaction();

			var validationResult = await CheckStockOutReturnIsValid(newModel, oldModel, menuCode);
			if (validationResult.Success == false)
			{
				await _databaseTransaction.Rollback();
				return validationResult;
			}

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

		private async Task<ResponseDto> CheckStockOutReturnIsValid(StockOutReturnDto? newModel, StockOutReturnDto? oldModel, int menuCode)
		{
			if (newModel != null)
			{
				var parentMenuCode = await _stockOutReturnHandlingService.GetParentMenuCode(newModel.StockOutReturnHeader!.StockOutHeaderId, newModel.StockOutReturnHeader!.SalesInvoiceHeaderId);
                var grandParentMenuCode = await _stockOutReturnHandlingService.GetGrandParentMenuCode(newModel.StockOutReturnHeader!.StockOutHeaderId);
				return await _stockOutReturnHandlingService.CheckStockOutReturnIsValidForSave(newModel!, menuCode, parentMenuCode, grandParentMenuCode);
			}
			else if (oldModel != null)
			{
				var parentMenuCode = await _stockOutReturnHandlingService.GetParentMenuCode(oldModel.StockOutReturnHeader!.StockOutHeaderId, oldModel.StockOutReturnHeader!.SalesInvoiceHeaderId);
				var grandParentMenuCode = await _stockOutReturnHandlingService.GetGrandParentMenuCode(oldModel.StockOutReturnHeader!.StockOutHeaderId);

				return await _stockOutReturnHandlingService.CheckStockOutReturnIsValidForDelete(
                    oldModel.StockOutReturnHeader!.StockOutReturnHeaderId,
					oldModel.StockOutReturnHeader!.StoreId,
					oldModel.StockOutReturnHeader!.StockOutHeaderId,
					oldModel.StockOutReturnHeader!.IsEnded,
					oldModel.StockOutReturnHeader!.IsBlocked,
					menuCode,
                    parentMenuCode,
                    grandParentMenuCode);
			}
			else
			{
				return new ResponseDto { Success = true };
			}
		}
	}
}
