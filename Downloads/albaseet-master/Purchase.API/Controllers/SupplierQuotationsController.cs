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
    public class SupplierQuotationsController : ControllerBase
    {
        private readonly IDatabaseTransaction _databaseTransaction;
        private readonly IStringLocalizer<HandelException> _exLocalizer;
        private readonly ISupplierQuotationHeaderService _supplierQuotationHeaderService;
        private readonly ISupplierQuotationService _supplierQuotationService;
        private readonly ISupplierQuotationDetailService _supplierQuotationDetailService;
        private readonly IHandleApprovalRequestService _handleApprovalRequestService;
        private readonly IApprovalSystemService _approvalSystemService;
        private readonly IGenericMessageService _genericMessageService;
		private readonly IHttpContextAccessor _httpContextAccessor;

        public SupplierQuotationsController(IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exLocalizer, IHandleApprovalRequestService handleApprovalRequestService, IApprovalSystemService approvalSystemService, ISupplierQuotationHeaderService supplierQuotationHeaderService, ISupplierQuotationService supplierQuotationService, ISupplierQuotationDetailService supplierQuotationDetailService, IGenericMessageService genericMessageService, IHttpContextAccessor httpContextAccessor)
        {
            _databaseTransaction = databaseTransaction;
            _exLocalizer = exLocalizer;
            _supplierQuotationHeaderService = supplierQuotationHeaderService;
            _supplierQuotationService = supplierQuotationService;
            _supplierQuotationDetailService = supplierQuotationDetailService;
            _handleApprovalRequestService = handleApprovalRequestService;
            _approvalSystemService = approvalSystemService;
            _genericMessageService = genericMessageService;
			_httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("ReadSupplierQuotations")]
        public async Task<IActionResult> ReadSupplierQuotations(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var data = await DataSourceLoader.LoadAsync(_supplierQuotationHeaderService.GetUserSupplierQuotationHeaders(), loadOptions);
                return Ok(data);
            }catch
            {
                var model = _supplierQuotationHeaderService.GetUserSupplierQuotationHeaders();
                var data = DataSourceLoader.Load(model.ToList(), loadOptions);
                return Ok(data);
            }
        }

        [HttpGet]
        [Route("ReadSupplierQuotationsByStoreId")]
        public async Task<IActionResult> ReadSupplierQuotationsByStoreId(DataSourceLoadOptions loadOptions, int storeId, int? supplierId, int supplierQuotationHeaderId)
        {
            try
            {
                var data = await DataSourceLoader.LoadAsync(_supplierQuotationHeaderService.GetSupplierQuotationHeadersByStoreId(storeId, supplierId, supplierQuotationHeaderId), loadOptions);
                return Ok(data);
            }catch
            {
                var model = _supplierQuotationHeaderService.GetSupplierQuotationHeadersByStoreId(storeId, supplierId, supplierQuotationHeaderId);
                var data = DataSourceLoader.Load(model.ToList(), loadOptions);
                return Ok(data);
            }
        }

        [HttpGet]
        [Route("GetSupplierQuotationCode")]
        public async Task<IActionResult> GetSupplierQuotationCode(int storeId, DateTime documentDate)
        {
            var data = await _supplierQuotationHeaderService.GetSupplierQuotationCode(storeId, documentDate);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetSupplierQuotationFromProductRequestPrice")]
        public async Task<IActionResult> GetSupplierQuotationFromProductRequestPrice(int productRequestPriceHeaderId)
        {
            var data = await _supplierQuotationService.GetSupplierQuotationFromProductRequestPrice(productRequestPriceHeaderId);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetRequestData")]
        public async Task<IActionResult> GetRequestData(int requestId)
        {
            var data = await _handleApprovalRequestService.GetRequestData(requestId);
            if (data != null)
            {
                if (data.CanBeConverted<SupplierQuotationDto>())
                {
					var supplierQuotation = data.ConvertToType<SupplierQuotationDto>();
					var userCanLook = await _httpContextAccessor.UserCanLook(0, supplierQuotation?.SupplierQuotationHeader?.StoreId ?? 0);
                    return Ok(userCanLook ? supplierQuotation : new SupplierQuotationDto());
                }
            }
            return Ok(new SupplierQuotationDto());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetSupplierQuotation(int id)
        {
            var supplierQuotation = await _supplierQuotationService.GetSupplierQuotation(id);
			var userCanLook = await _httpContextAccessor.UserCanLook(0, supplierQuotation.SupplierQuotationHeader?.StoreId ?? 0);
            return Ok(userCanLook ? supplierQuotation : new SupplierQuotationDto());
        }

        [HttpGet]
        [Route("GetSupplierQuotationDetail")]
        public async Task<IActionResult> GetSupplierQuotationDetail(int id)
        {
            var supplierQuotationDetail = await _supplierQuotationDetailService.GetSupplierQuotationDetails(id);
            return Ok(supplierQuotationDetail);
        }

        [HttpGet]
        [Route("GetSupplierQuotationHeader")]
        public async Task<IActionResult> GetSupplierQuotationHeader(int id)
        {
            var supplierQuotationHeader = await _supplierQuotationHeaderService.GetSupplierQuotationHeaderById(id);
            return Ok(supplierQuotationHeader);
        }

        [HttpPost]
        [Route("UpdateIsClosed")]
        public async Task<IActionResult> UpdateIsClosed(int supplierQuotationHeaderId, bool isClosed)
        {
            return Ok(await _supplierQuotationHeaderService.UpdateClosed(supplierQuotationHeaderId, isClosed));
        }

        [HttpPost]
        [Route("Save")]
        public async Task<IActionResult> Save([FromBody] SupplierQuotationDto model, int requestId)
        {
            var response = new ResponseDto();
            try
            {
                if (model.SupplierQuotationHeader != null)
                {
                    var hasApprove = await _approvalSystemService.IsMenuHasApprove(MenuCodeData.SupplierQuotation);
                    if (hasApprove.HasApprove)
                    {
                        if (hasApprove.OnAdd && model.SupplierQuotationHeader!.SupplierQuotationHeaderId == 0)
                        {
                            response = await SendApproveRequest(model, null, requestId, 0, false);
                        }
                        else if (hasApprove.OnEdit && model.SupplierQuotationHeader!.SupplierQuotationHeaderId > 0)
                        {
                            var oldValue = await _supplierQuotationService.GetSupplierQuotation(model.SupplierQuotationHeader!.SupplierQuotationHeaderId);
                            response = await SendApproveRequest(model, oldValue, requestId, model.SupplierQuotationHeader!.SupplierQuotationHeaderId, false);
                        }
                        else
                        {
                            response = await SaveSupplierQuotation(model, hasApprove.HasApprove, false, requestId);
                        }
                    }
                    else
                    {
                        response = await SaveSupplierQuotation(model, hasApprove.HasApprove, false, requestId);
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
        [Route("SaveSupplierQuotation")]
        public async Task<ResponseDto> SaveSupplierQuotation(SupplierQuotationDto model, bool hasApprove, bool approved, int? requestId)
        {
            await _databaseTransaction.BeginTransaction();
            var response = await _supplierQuotationService.SaveSupplierQuotation(model, hasApprove, approved, requestId);
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
        [Route("DeleteSupplierQuotation")]
        public async Task<IActionResult> DeleteSupplierQuotation(int supplierQuotationHeaderId, int requestId)
        {
            var response = new ResponseDto();
            try
            {
                var model = await _supplierQuotationService.GetSupplierQuotation(supplierQuotationHeaderId);
                if (model.SupplierQuotationHeader != null)
                {
                    var hasApprove = await _approvalSystemService.IsMenuHasApprove(MenuCodeData.SupplierQuotation);
                    if (hasApprove.HasApprove && hasApprove.OnDelete)
                    {
                        response = await SendApproveRequest(null, model, requestId, supplierQuotationHeaderId, true);
                    }
                    else
                    {
                        await _databaseTransaction.BeginTransaction();
                        response = await _supplierQuotationService.DeleteSupplierQuotation(supplierQuotationHeaderId);
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
                    response.Id = supplierQuotationHeaderId;
                    response.Message = await _genericMessageService.GetMessage(MenuCodeData.SupplierQuotation, GenericMessageData.NotFound);
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
        public async Task<ResponseDto> SendApproveRequest([FromQuery] SupplierQuotationDto? newModel, [FromQuery] SupplierQuotationDto? oldModel, int requestId, int supplierQuotationHeaderId, bool isDelete)
        {
            var changes = new List<RequestChangesDto>();
            var menuCode = MenuCodeData.SupplierQuotation;
            var requestTypeId = isDelete ? ApproveRequestTypeData.Delete : (supplierQuotationHeaderId == 0 ? ApproveRequestTypeData.Add : ApproveRequestTypeData.Edit);
            if (requestTypeId == ApproveRequestTypeData.Edit && oldModel != null)
            {
                changes = _supplierQuotationService.GetSupplierQuotationRequestChanges(oldModel, newModel!);
            }

            var request = new NewApproveRequestDto()
            {
                RequestId = requestId,
                MenuCode = (short)menuCode,
                ApproveRequestTypeId = requestTypeId,
                ReferenceId = supplierQuotationHeaderId,
                ReferenceCode = supplierQuotationHeaderId.ToString(),
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
