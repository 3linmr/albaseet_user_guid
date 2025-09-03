using DevExtreme.AspNet.Data;
using Sales.API.Models;
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
using Sales.CoreOne.Models.Dtos.ViewModels;
using Sales.CoreOne.Contracts;
using Shared.CoreOne.Models.Dtos.ViewModels.Items;
using Sales.Service.Services;
using Shared.CoreOne.Models.Domain.Inventory;
using Shared.CoreOne.Contracts.Menus;
using Sales.CoreOne.Models.StaticData;
using Shared.Helper.Identity;

namespace Sales.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SalesInvoiceReturnsController : ControllerBase
    {
        private readonly IDatabaseTransaction _databaseTransaction;
        private readonly IStringLocalizer<HandelException> _exLocalizer;
        private readonly ISalesInvoiceReturnHeaderService _salesInvoiceReturnHeaderService;
        private readonly ISalesInvoiceReturnService _salesInvoiceReturnService;
        private readonly ISalesInvoiceReturnDetailService _salesInvoiceReturnDetailService;
        private readonly ISalesInvoiceReturnHandlingService _salesInvoiceReturnHandlingService;
        private readonly IHandleApprovalRequestService _handleApprovalRequestService;
        private readonly IApprovalSystemService _approvalSystemService;
        private readonly IGenericMessageService _genericMessageService;
		private readonly IHttpContextAccessor _httpContextAccessor;

        public SalesInvoiceReturnsController(IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exLocalizer, IHandleApprovalRequestService handleApprovalRequestService, IApprovalSystemService approvalSystemService, ISalesInvoiceReturnHeaderService salesInvoiceReturnHeaderService, ISalesInvoiceReturnService salesInvoiceReturnService, ISalesInvoiceReturnDetailService salesInvoiceReturnDetailService, ISalesInvoiceReturnHandlingService salesInvoiceReturnHandlingService, IGenericMessageService genericMessageService, IHttpContextAccessor httpContextAccessor)
        {
            _databaseTransaction = databaseTransaction;
            _exLocalizer = exLocalizer;
            _salesInvoiceReturnHeaderService = salesInvoiceReturnHeaderService;
            _salesInvoiceReturnService = salesInvoiceReturnService;
            _salesInvoiceReturnDetailService = salesInvoiceReturnDetailService;
            _handleApprovalRequestService = handleApprovalRequestService;
            _approvalSystemService = approvalSystemService;
            _salesInvoiceReturnHandlingService = salesInvoiceReturnHandlingService;
            _genericMessageService = genericMessageService;
			_httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("ReadSalesInvoiceReturns")]
        public async Task<IActionResult> ReadSalesInvoiceReturns(DataSourceLoadOptions loadOptions, int menuCode)
        {
            try
            {
                var data = await DataSourceLoader.LoadAsync(_salesInvoiceReturnHeaderService.GetUserSalesInvoiceReturnHeaders(menuCode), loadOptions);
                return Ok(data);
            }
            catch
            {
                var model = _salesInvoiceReturnHeaderService.GetUserSalesInvoiceReturnHeaders(menuCode);
                var data = DataSourceLoader.Load(model.ToList(), loadOptions);
                return Ok(data);
            }
        }

        [HttpGet]
        [Route("ReadSalesInvoiceReturnsByStoreId")]
        public async Task<IActionResult> ReadSalesInvoiceReturnsByStoreId(DataSourceLoadOptions loadOptions, int storeId, int? clientId, int salesInvoiceReturnHeaderId, bool? isOnTheWay = null)
        {
            try
            {
                var data = await DataSourceLoader.LoadAsync(_salesInvoiceReturnHeaderService.GetSalesInvoiceReturnHeadersByStoreId(storeId, clientId, salesInvoiceReturnHeaderId), loadOptions);
                return Ok(data);
            }
            catch
            {
                var model = _salesInvoiceReturnHeaderService.GetSalesInvoiceReturnHeadersByStoreId(storeId, clientId, salesInvoiceReturnHeaderId);
                var data = DataSourceLoader.Load(model.ToList(), loadOptions);
                return Ok(data);
            }
        }

        [HttpGet]
        [Route("GetSalesInvoiceReturnCode")]
        public async Task<IActionResult> GetSalesInvoiceReturnCode(int storeId, DateTime documentDate, bool isDirectInvoice, bool creditPayment)
        {
            var data = await _salesInvoiceReturnHeaderService.GetSalesInvoiceReturnCode(storeId, documentDate, false, isDirectInvoice, creditPayment);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetSalesInvoiceReturnFromSalesInvoice")]
        public async Task<IActionResult> GetSalesInvoiceReturnFromSalesInvoice(int salesInvoiceHeaderId, bool isDirectInvoice)
        {
            var data = await _salesInvoiceReturnHandlingService.GetSalesInvoiceReturnFromSalesInvoice(salesInvoiceHeaderId, isDirectInvoice, false);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetRequestData")]
        public async Task<IActionResult> GetRequestData(int requestId)
        {
			var data = await _handleApprovalRequestService.GetRequestData(requestId);
			if (data != null && data.CanBeConverted<SalesInvoiceReturnDto>())
			{
				var salesInvoiceReturn = data.ConvertToType<SalesInvoiceReturnDto>();
                if (salesInvoiceReturn!.SalesInvoiceReturnHeader == null) return Ok(new SalesInvoiceReturnDto());
				
				var userCanLook = await _httpContextAccessor.UserCanLook(0, salesInvoiceReturn.SalesInvoiceReturnHeader.StoreId);
				if (!userCanLook) return Ok(new SalesInvoiceReturnDto());

				if (salesInvoiceReturn!.SalesInvoiceReturnHeader!.CreditPayment == false)
				{
					salesInvoiceReturn!.SalesInvoiceReturnPayments = await _salesInvoiceReturnService.AddNonincludedPaymentMethods(salesInvoiceReturn.SalesInvoiceReturnPayments, salesInvoiceReturn.SalesInvoiceReturnHeader!.StoreId);
				}

				var salesInvoiceReturnHeader = salesInvoiceReturn.SalesInvoiceReturnHeader;
				salesInvoiceReturn!.SalesInvoiceReturnDetails = await _salesInvoiceReturnService.GetSalesInvoiceReturnDetailsCalculated(
					salesInvoiceReturnHeader.SalesInvoiceReturnHeaderId,
					salesInvoiceReturnHeader,
					salesInvoiceReturn.SalesInvoiceReturnDetails);

				return Ok(salesInvoiceReturn);
			}
			return Ok(new SalesInvoiceReturnDto());
		}

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetSalesInvoiceReturn(int id)
        {
            var salesInvoiceReturn = await _salesInvoiceReturnService.GetSalesInvoiceReturn(id);
			var userCanLook = await _httpContextAccessor.UserCanLook(0, salesInvoiceReturn.SalesInvoiceReturnHeader?.StoreId ?? 0);
            return Ok(userCanLook ? salesInvoiceReturn : new SalesInvoiceReturnDto());
        }

        [HttpGet]
        [Route("GetSalesInvoiceReturnHeader")]
        public async Task<IActionResult> GetSalesInvoiceReturnHeader(int id)
        {
            var salesInvoiceReturnHeader = await _salesInvoiceReturnHeaderService.GetSalesInvoiceReturnHeaderById(id);
            return Ok(salesInvoiceReturnHeader);
        }

        [HttpGet]
        [Route("GetSalesInvoiceReturnDetail")]
        public async Task<IActionResult> GetSalesInvoiceReturnDetail(int id)
        {
            var salesInvoiceReturnDetail = await _salesInvoiceReturnService.GetSalesInvoiceReturnDetailsCalculated(id);
            return Ok(salesInvoiceReturnDetail);
        }

        [HttpPost]
        [Route("Save")]
        public async Task<IActionResult> Save([FromBody] SalesInvoiceReturnDto model, int requestId)
        {
            var response = new ResponseDto();
            try
            {
                if (model.SalesInvoiceReturnHeader != null)
                {
                    var hasApprove = await _approvalSystemService.IsMenuHasApprove(SalesInvoiceReturnMenuCodeHelper.GetMenuCode(model.SalesInvoiceReturnHeader));
                    if (hasApprove.HasApprove)
                    {
                        if (hasApprove.OnAdd && model.SalesInvoiceReturnHeader!.SalesInvoiceReturnHeaderId == 0)
                        {
                            response = await SendApproveRequest(model, null, requestId, 0, false);
                        }
                        else if (hasApprove.OnEdit && model.SalesInvoiceReturnHeader!.SalesInvoiceReturnHeaderId > 0)
                        {
                            var oldValue = await _salesInvoiceReturnService.GetSalesInvoiceReturn(model.SalesInvoiceReturnHeader!.SalesInvoiceReturnHeaderId);
                            response = await SendApproveRequest(model, oldValue, requestId, model.SalesInvoiceReturnHeader!.SalesInvoiceReturnHeaderId, false);
                        }
                        else
                        {
                            response = await SaveSalesInvoiceReturn(model, hasApprove.HasApprove, false, requestId);
                        }
                    }
                    else
                    {
                        response = await SaveSalesInvoiceReturn(model, hasApprove.HasApprove, false, requestId);
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
        [Route("SaveSalesInvoiceReturn")]
        public async Task<ResponseDto> SaveSalesInvoiceReturn(SalesInvoiceReturnDto model, bool hasApprove, bool approved, int? requestId)
        {
            await _databaseTransaction.BeginTransaction();
            var response = await _salesInvoiceReturnHandlingService.SaveSalesInvoiceReturn(model, hasApprove, approved, requestId);
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
        [Route("DeleteSalesInvoiceReturn")]
        public async Task<IActionResult> DeleteSalesInvoiceReturn(int salesInvoiceReturnHeaderId, int requestId)
        {
            var response = new ResponseDto();
            try
            {
                var model = await _salesInvoiceReturnService.GetSalesInvoiceReturn(salesInvoiceReturnHeaderId);
                if (model.SalesInvoiceReturnHeader != null)
                {
                    var menuCode = SalesInvoiceReturnMenuCodeHelper.GetMenuCode(model.SalesInvoiceReturnHeader);
                    var hasApprove = await _approvalSystemService.IsMenuHasApprove(menuCode);
                    if (hasApprove.HasApprove && hasApprove.OnDelete)
                    {
                        response = await SendApproveRequest(null, model, requestId, salesInvoiceReturnHeaderId, true);
                    }
                    else
                    {
                        await _databaseTransaction.BeginTransaction();
                        response = await _salesInvoiceReturnHandlingService.DeleteSalesInvoiceReturn(salesInvoiceReturnHeaderId, menuCode);
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
                    response.Id = salesInvoiceReturnHeaderId;
                    response.Message = await _genericMessageService.GetMessage(MenuCodeData.SalesInvoiceReturn, GenericMessageData.NotFound);
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
        public async Task<ResponseDto> SendApproveRequest([FromQuery] SalesInvoiceReturnDto? newModel, [FromQuery] SalesInvoiceReturnDto? oldModel, int requestId, int salesInvoiceReturnHeaderId, bool isDelete)
        {
            var changes = new List<RequestChangesDto>();
            var menuCode = 0;
            var requestTypeId = isDelete ? ApproveRequestTypeData.Delete : (salesInvoiceReturnHeaderId == 0 ? ApproveRequestTypeData.Add : ApproveRequestTypeData.Edit);

            if (requestTypeId == ApproveRequestTypeData.Edit || requestTypeId == ApproveRequestTypeData.Delete)
            {
                if (oldModel != null)
                {
                    if(newModel != null) 
                    {
                        changes = _salesInvoiceReturnService.GetSalesInvoiceReturnRequestChanges(oldModel, newModel);
                    }

                    if (oldModel.SalesInvoiceReturnHeader != null)
                    {
                        menuCode = SalesInvoiceReturnMenuCodeHelper.GetMenuCode(oldModel.SalesInvoiceReturnHeader);
                    }
                }
            }
            else
            {
                if (newModel != null)
                {
                    if (newModel.SalesInvoiceReturnHeader != null)
                    {
                        menuCode = SalesInvoiceReturnMenuCodeHelper.GetMenuCode(newModel.SalesInvoiceReturnHeader);
                    }
                }
            }

            var request = new NewApproveRequestDto()
            {
                RequestId = requestId,
                MenuCode = (short)menuCode,
                ApproveRequestTypeId = requestTypeId,
                ReferenceId = salesInvoiceReturnHeaderId,
                ReferenceCode = salesInvoiceReturnHeaderId.ToString(),
                OldValue = oldModel,
                NewValue = newModel,
                Changes = changes
            };
            await _databaseTransaction.BeginTransaction();

			var validationResult = await CheckSalesInvoiceReturnIsValid(newModel, oldModel, menuCode);
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

		private async Task<ResponseDto> CheckSalesInvoiceReturnIsValid(SalesInvoiceReturnDto? newModel, SalesInvoiceReturnDto? oldModel, int menuCode)
		{
			if (newModel != null)
			{
                var parentMenuCode = await _salesInvoiceReturnHandlingService.GetParentMenuCode(newModel.SalesInvoiceReturnHeader!.SalesInvoiceHeaderId);
				return await _salesInvoiceReturnHandlingService.CheckSalesInvoiceReturnIsValidForSave(newModel, menuCode, parentMenuCode);
			}
			else if (oldModel != null)
			{
                var parentMenuCode = await _salesInvoiceReturnHandlingService.GetParentMenuCode(oldModel.SalesInvoiceReturnHeader!.SalesInvoiceHeaderId);
				return await _salesInvoiceReturnHandlingService.CheckSalesInvoiceReturnIsValidForDelete(
					oldModel.SalesInvoiceReturnHeader!.SalesInvoiceReturnHeaderId,
					oldModel.SalesInvoiceReturnHeader!.SalesInvoiceHeaderId,
					oldModel.SalesInvoiceReturnHeader!.StoreId,
					oldModel.SalesInvoiceReturnHeader!.IsBlocked,
					oldModel.SalesInvoiceReturnHeader!.IsOnTheWay,
					menuCode,
                    parentMenuCode);
			}
			else
			{
				return new ResponseDto { Success = true };
			}
		}
	}
}
