using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Contracts.Approval;
using Shared.Helper.Database;
using Shared.Helper.Extensions;
using Shared.Helper.Models.Dtos;
using Shared.CoreOne;
using Sales.CoreOne.Contracts;
using Sales.API.Models;
using Sales.Service.Services;
using Sales.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne.Models.StaticData;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.CoreOne.Contracts.Menus;
using Shared.CoreOne.Models.Domain.Menus;
using Shared.Helper.Identity;


namespace Sales.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StockOutsController: ControllerBase
    {
        private readonly IDatabaseTransaction _databaseTransaction;
        private readonly IStringLocalizer<HandelException> _exLocalizer;
        private readonly IStockOutHeaderService _stockOutHeaderService;
        private readonly IStockOutHandlingService _stockOutHandlingService;
        private readonly IStockOutService _stockOutService;
        private readonly IStockOutDetailService _stockOutDetailService;
        private readonly IHandleApprovalRequestService _handleApprovalRequestService;
        private readonly IApprovalSystemService _approvalSystemService;
        private readonly IGenericMessageService _genericMessageService;
		private readonly IHttpContextAccessor _httpContextAccessor;

        public StockOutsController(IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exLocalizer, IHandleApprovalRequestService handleApprovalRequestService, IApprovalSystemService approvalSystemService, IStockOutHeaderService stockOutHeaderService, IStockOutHandlingService stockOutHandlingService, IStockOutService stockOutService, IStockOutDetailService stockOutDetailService, IGenericMessageService genericMessageService, IHttpContextAccessor httpContextAccessor)
        {
            _databaseTransaction = databaseTransaction;
            _exLocalizer = exLocalizer;
            _stockOutHeaderService = stockOutHeaderService;
            _stockOutHandlingService = stockOutHandlingService;
            _stockOutService = stockOutService;
            _stockOutDetailService = stockOutDetailService;
            _handleApprovalRequestService = handleApprovalRequestService;
            _approvalSystemService = approvalSystemService;
            _genericMessageService = genericMessageService;
			_httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("ReadStockOuts")]
        public async Task<IActionResult> ReadStockOuts(DataSourceLoadOptions loadOptions, int stockTypeId)
        {
            try
            {
                var data = await DataSourceLoader.LoadAsync(_stockOutHeaderService.GetUserStockOutHeaders(stockTypeId), loadOptions);
                return Ok(data);
            }
            catch
            {
                var model = _stockOutHeaderService.GetUserStockOutHeaders(stockTypeId);
                var data = DataSourceLoader.Load(model.ToList(), loadOptions);
                return Ok(data);
            }
        }

        [HttpGet]
        [Route("ReadStockOutsByStoreId")]
        public async Task<IActionResult> ReadStockOutsByStoreId(DataSourceLoadOptions loadOptions, int storeId, int? clientId, int stockTypeId, int stockOutHeaderId)
        {
            try
            {
                var data = await DataSourceLoader.LoadAsync(_stockOutHandlingService.GetStockOutHeadersByStoreId(storeId, clientId, stockTypeId, stockOutHeaderId), loadOptions);
                return Ok(data);
            }
            catch
            {
                var model = _stockOutHandlingService.GetStockOutHeadersByStoreId(storeId, clientId, stockTypeId, stockOutHeaderId);
                var data = DataSourceLoader.Load(model.ToList(), loadOptions);
                return Ok(data);
            }
        }

        [HttpGet]
        [Route("GetStockOutFromProformaInvoice")]
        public async Task<IActionResult> GetStockOutFromProformaInvoice(int proformaInvoiceId)
        {
            var data = await _stockOutHandlingService.GetStockOutFromProformaInvoice(proformaInvoiceId);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetStockOutCode")]
        public async Task<IActionResult> GetStockOutCode(int storeId, DateTime documentDate, int stockTypeId)
        {
            var data = await _stockOutHeaderService.GetStockOutCode(storeId, documentDate, stockTypeId);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetRequestData")]
        public async Task<IActionResult> GetRequestData(int requestId)
        {
            var data = await _handleApprovalRequestService.GetRequestData(requestId);
            if (data != null && data.CanBeConverted<StockOutDto>())
            {
                var stockOut = data.ConvertToType<StockOutDto>();
                if (stockOut?.StockOutHeader == null) return Ok(new StockOutDto());
				
				var userCanLook = await _httpContextAccessor.UserCanLook(0, stockOut.StockOutHeader.StoreId);
				if (!userCanLook) return Ok(new StockOutDto());

				var stockOutHeader = stockOut.StockOutHeader;
                stockOut!.StockOutDetails = await _stockOutHandlingService.ModifyStockOutDetailsWithLiveAvailableQuantity(
                    stockOutHeader.StockOutHeaderId, 
                    stockOutHeader.ProformaInvoiceHeaderId, 
                    stockOutHeader.SalesInvoiceHeaderId, stockOut.StockOutDetails);

                await _stockOutHandlingService.ModifyStockOutDetailsWithStoreIdAndAvailableBalance(stockOut.StockOutHeader.StockOutHeaderId, stockOut.StockOutHeader.StoreId, stockOut.StockOutDetails, true, false);

				return Ok(stockOut);
            }
            return Ok(new StockOutDto());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetStockOut(int id)
        {
            var stockOut = await _stockOutHandlingService.GetStockOut(id);
			var userCanLook = await _httpContextAccessor.UserCanLook(0, stockOut.StockOutHeader?.StoreId ?? 0);
            return Ok(userCanLook ? stockOut : new StockOutDto());
        }

        [HttpGet]
        [Route("GetStockOutDetail")]
        public async Task<IActionResult> GetStockOutDetail(int id)
        {
            var stockOutDetail = await _stockOutHandlingService.GetStockOutDetailsCalculated(id);
            return Ok(stockOutDetail);
        }

        [HttpGet]
        [Route("GetStockOutHeader")]
        public async Task<IActionResult> GetStockOutHeader(int id)
        {
            var stockOutHeader = await _stockOutHeaderService.GetStockOutHeaderById(id);
            return Ok(stockOutHeader);
        }

        [HttpPost]
        [Route("Save")]
        public async Task<IActionResult> Save([FromBody] StockOutDto model, int requestId)
        {
            var response = new ResponseDto();
            try
            {
                if (model.StockOutHeader != null)
                {
                    var hasApprove = await _approvalSystemService.IsMenuHasApprove(StockTypeData.ToMenuCode(model.StockOutHeader.StockTypeId));
                    if (hasApprove.HasApprove)
                    {
                        if (hasApprove.OnAdd && model.StockOutHeader!.StockOutHeaderId == 0)
                        {
                            response = await SendApproveRequest(model, null, requestId, 0, false);
                        }
                        else if (hasApprove.OnEdit && model.StockOutHeader!.StockOutHeaderId > 0)
                        {
                            var oldValue = await _stockOutHandlingService.GetStockOut(model.StockOutHeader!.StockOutHeaderId);
                            response = await SendApproveRequest(model, oldValue, requestId, model.StockOutHeader!.StockOutHeaderId, false);
                        }
                        else
                        {
                            response = await SaveStockOut(model, hasApprove.HasApprove, false, requestId);
                        }
                    }
                    else
                    {
                        response = await SaveStockOut(model, hasApprove.HasApprove, false, requestId);
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
        [Route("SaveStockOut")]
        public async Task<ResponseDto> SaveStockOut(StockOutDto model, bool hasApprove, bool approved, int? requestId)
        {
            await _databaseTransaction.BeginTransaction();
            var response = await _stockOutHandlingService.SaveStockOut(model, hasApprove, approved, requestId);
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
        [Route("DeleteStockOut")]
        public async Task<IActionResult> DeleteStockOut(int stockOutHeaderId, int requestId)
        {
            var response = new ResponseDto();
            try
            {
                var model = await _stockOutHandlingService.GetStockOut(stockOutHeaderId);
                if (model.StockOutHeader != null)
                {
                    var hasApprove = await _approvalSystemService.IsMenuHasApprove(StockTypeData.ToMenuCode(model.StockOutHeader.StockTypeId));
                    if (hasApprove.HasApprove && hasApprove.OnDelete)
                    {
                        response = await SendApproveRequest(null, model, requestId, stockOutHeaderId, true);
                    }
                    else
                    {
                        await _databaseTransaction.BeginTransaction();
                        response = await _stockOutHandlingService.DeleteStockOut(stockOutHeaderId, StockTypeData.ToMenuCode(model.StockOutHeader.StockTypeId));
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
                    response.Id = stockOutHeaderId;
                    response.Message = await _genericMessageService.GetMessage(MenuCodeData.StockOutFromProformaInvoice, GenericMessageData.NotFound); //must use a default menuCode here
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
        public async Task<ResponseDto> SendApproveRequest([FromQuery] StockOutDto? newModel, [FromQuery] StockOutDto? oldModel, int requestId, int stockOutHeaderId, bool isDelete)
        {
            var changes = new List<RequestChangesDto>();
            var menuCode = 0;
            var requestTypeId = isDelete ? ApproveRequestTypeData.Delete : (stockOutHeaderId == 0 ? ApproveRequestTypeData.Add : ApproveRequestTypeData.Edit);
            if (requestTypeId == ApproveRequestTypeData.Edit || requestTypeId == ApproveRequestTypeData.Delete)
            {
                if (oldModel != null)
                {
                    if (newModel != null)
                    {
                        changes = _stockOutService.GetStockOutRequestChanges(oldModel, newModel);
                    }

                    if (oldModel.StockOutHeader != null)
                    {
                        menuCode = StockTypeData.ToMenuCode(oldModel.StockOutHeader.StockTypeId);

                    }
                }
            }
            else
            {
                if (newModel != null)
                {
                    if (newModel.StockOutHeader != null)
                    {
                        menuCode = StockTypeData.ToMenuCode(newModel.StockOutHeader.StockTypeId);

                    }
                }
            }

            var request = new NewApproveRequestDto()
            {
                RequestId = requestId,
                MenuCode = (short)menuCode,
                ApproveRequestTypeId = requestTypeId,
                ReferenceId = stockOutHeaderId,
                ReferenceCode = stockOutHeaderId.ToString(),
                OldValue = oldModel,
                NewValue = newModel,
                Changes = changes
            };
            await _databaseTransaction.BeginTransaction();

            var validationResult = await CheckStockOutIsValid(newModel, oldModel, menuCode);
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

        private async Task<ResponseDto> CheckStockOutIsValid(StockOutDto? newModel, StockOutDto? oldModel, int menuCode)
        {
            if (newModel != null)
            {
                var parentMenuCode = await _stockOutHandlingService.GetParentMenuCode(newModel.StockOutHeader!.ProformaInvoiceHeaderId, newModel.StockOutHeader!.SalesInvoiceHeaderId);
                return await _stockOutHandlingService.CheckStockOutIsValidForSave(newModel!, menuCode, parentMenuCode);
            }
            else if (oldModel != null)
            {
                return await _stockOutHandlingService.CheckStockOutIsValidForDelete(
                    oldModel.StockOutHeader!.StockOutHeaderId, 
                    oldModel.StockOutHeader!.ProformaInvoiceHeaderId, 
                    oldModel.StockOutHeader!.SalesInvoiceHeaderId, 
                    oldModel.StockOutHeader!.IsBlocked,
                    oldModel.StockOutHeader!.IsEnded, 
                    menuCode);
            }
            else
            {
                return new ResponseDto { Success = true };
            }
		}
    }
}
