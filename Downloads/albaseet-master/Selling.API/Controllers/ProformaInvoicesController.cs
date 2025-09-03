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
using Sales.Service.Services;
using Shared.CoreOne.Contracts.Menus;
using Shared.Helper.Identity;

namespace Sales.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProformaInvoicesController : ControllerBase
    {
        private readonly IDatabaseTransaction _databaseTransaction;
        private readonly IStringLocalizer<HandelException> _exLocalizer;
        private readonly IProformaInvoiceHeaderService _proformaInvoiceHeaderService;
        private readonly IProformaInvoiceService _proformaInvoiceService;
        private readonly IProformaInvoiceDetailService _proformaInvoiceDetailService;
        private readonly IHandleApprovalRequestService _handleApprovalRequestService;
        private readonly IApprovalSystemService _approvalSystemService;
        private readonly IGenericMessageService _genericMessageService;
		private readonly IHttpContextAccessor _httpContextAccessor;

        public ProformaInvoicesController(IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exLocalizer, IHandleApprovalRequestService handleApprovalRequestService, IApprovalSystemService approvalSystemService, IProformaInvoiceHeaderService proformaInvoiceHeaderService, IProformaInvoiceService proformaInvoiceService, IProformaInvoiceDetailService proformaInvoiceDetailService, IGenericMessageService genericMessageService, IHttpContextAccessor httpContextAccessor)
        {
            _databaseTransaction = databaseTransaction;
            _exLocalizer = exLocalizer;
            _proformaInvoiceHeaderService = proformaInvoiceHeaderService;
            _proformaInvoiceService = proformaInvoiceService;
            _proformaInvoiceDetailService = proformaInvoiceDetailService;
            _handleApprovalRequestService = handleApprovalRequestService;
            _approvalSystemService = approvalSystemService;
            _genericMessageService = genericMessageService;
			_httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("ReadProformaInvoices")]
        public async Task<IActionResult> ReadProformaInvoices(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var data = await DataSourceLoader.LoadAsync(_proformaInvoiceHeaderService.GetUserProformaInvoiceHeaders(), loadOptions);
                return Ok(data);
            }catch
            {
                var model = _proformaInvoiceHeaderService.GetUserProformaInvoiceHeaders();
                var data = DataSourceLoader.Load(model.ToList(), loadOptions);
                return Ok(data);
            }
        }

        [HttpGet]
        [Route("ReadProformaInvoicesByStoreId")]
        public async Task<IActionResult> ReadProformaInvoicesByStoreId(DataSourceLoadOptions loadOptions, int storeId, int? clientId, int proformaInvoiceHeaderId)
        {
            try
            {
                var data = await DataSourceLoader.LoadAsync(_proformaInvoiceHeaderService.GetProformaInvoiceHeadersByStoreId(storeId, clientId, proformaInvoiceHeaderId), loadOptions);
                return Ok(data);
            }catch
            {
                var model = _proformaInvoiceHeaderService.GetProformaInvoiceHeadersByStoreId(storeId, clientId, proformaInvoiceHeaderId);
                var data = DataSourceLoader.Load(model.ToList(), loadOptions);
                return Ok(data);
            }
        }

        [HttpGet]
        [Route("ReadProformaInvoicesByStoreIdAndMenuCode")]
        public async Task<IActionResult> ReadProformaInvoicesByStoreIdAndMenuCode(DataSourceLoadOptions loadOptions, int storeId, int? clientId, int menuCode, int proformaInvoiceHeaderId)
        {
            try
            {
                var data = await DataSourceLoader.LoadAsync(_proformaInvoiceService.GetProformaInvoiceHeadersByStoreIdAndMenuCode(storeId, clientId, menuCode, proformaInvoiceHeaderId), loadOptions);
                return Ok(data);
            }
            catch
            {
                var model = _proformaInvoiceService.GetProformaInvoiceHeadersByStoreIdAndMenuCode(storeId, clientId, menuCode, proformaInvoiceHeaderId);
                var data = DataSourceLoader.Load(model.ToList(), loadOptions);
                return Ok(data);
            }
        }

        [HttpGet]
        [Route("GetProformaInvoiceFromClientQuotationApproval")]
        public async Task<IActionResult> GetProformaInvoiceFromClientQuotationApproval(int clientQuotationApprovalId)
        {
            var data = await _proformaInvoiceService.GetProformaInvoiceFromClientQuotationApproval(clientQuotationApprovalId);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetProformaInvoiceCode")]
        public async Task<IActionResult> GetProformaInvoiceCode(int storeId, DateTime documentDate)
        {
            var data = await _proformaInvoiceHeaderService.GetProformaInvoiceCode(storeId, documentDate);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetRequestData")]
        public async Task<IActionResult> GetRequestData(int requestId)
        {
            var data = await _handleApprovalRequestService.GetRequestData(requestId);
            if (data != null && data.CanBeConverted<ProformaInvoiceDto>())
            {
                var proformaInvoice = data.ConvertToType<ProformaInvoiceDto>();
                if (proformaInvoice?.ProformaInvoiceHeader == null) return Ok(proformaInvoice);
				
				var userCanLook = await _httpContextAccessor.UserCanLook(0, proformaInvoice.ProformaInvoiceHeader.StoreId);
				if (!userCanLook) return Ok(new ProformaInvoiceDto());

                await _proformaInvoiceService.ModifyProformaInvoiceDetailsWithStoreIdAndAvaialbleBalance(proformaInvoice.ProformaInvoiceHeader.StoreId, proformaInvoice.ProformaInvoiceDetails);
				return Ok(proformaInvoice);
            }
            return Ok(new ProformaInvoiceDto());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProformaInvoice(int id)
        {
            var proformaInvoice = await _proformaInvoiceService.GetProformaInvoice(id);
			var userCanLook = await _httpContextAccessor.UserCanLook(0, proformaInvoice.ProformaInvoiceHeader?.StoreId ?? 0);
            return Ok(userCanLook ? proformaInvoice : new ProformaInvoiceDto());
        }

        [HttpGet]
        [Route("GetProformaInvoiceDetail")]
        public async Task<IActionResult> GetProformaInvoiceDetail(int id)
        {
            var proformaInvoiceDetail = await _proformaInvoiceDetailService.GetProformaInvoiceDetails(id);
            return Ok(proformaInvoiceDetail);
        }

        [HttpGet]
        [Route("GetProformaInvoiceHeader")]
        public async Task<IActionResult> GetProformaInvoiceHeader(int id)
        {
            var proformaInvoiceHeader = await _proformaInvoiceHeaderService.GetProformaInvoiceHeaderById(id);
            return Ok(proformaInvoiceHeader);
        }

        [HttpPost]
        [Route("UpdateIsBlocked")]
        public async Task<IActionResult> UpdateIsBlocked(int proformaInvoiceHeaderId, bool isBlocked)
        {
            return Ok(await _proformaInvoiceService.UpdateBlocked(proformaInvoiceHeaderId, isBlocked));
        }

        [HttpPost]
        [Route("UpdateShippingStatus")]
        public async Task<IActionResult> UpdateShippingStatus(int proformaInvoiceHeaderId, int shippingStatusId)
        {
            return Ok(await _proformaInvoiceHeaderService.UpdateShippingStatus(proformaInvoiceHeaderId, shippingStatusId));
        }

        [HttpPost]
        [Route("Save")]
        public async Task<IActionResult> Save([FromBody] ProformaInvoiceDto model, int requestId)
        {
            var response = new ResponseDto();
            try
            {
                if (model.ProformaInvoiceHeader != null)
                {
                    var hasApprove = await _approvalSystemService.IsMenuHasApprove(MenuCodeData.ProformaInvoice);
                    if (hasApprove.HasApprove)
                    {
                        if (hasApprove.OnAdd && model.ProformaInvoiceHeader!.ProformaInvoiceHeaderId == 0)
                        {
                            response = await SendApproveRequest(model, null, requestId, 0, false);
                        }
                        else if (hasApprove.OnEdit && model.ProformaInvoiceHeader!.ProformaInvoiceHeaderId > 0)
                        {
                            var oldValue = await _proformaInvoiceService.GetProformaInvoice(model.ProformaInvoiceHeader!.ProformaInvoiceHeaderId);
                            response = await SendApproveRequest(model, oldValue, requestId, model.ProformaInvoiceHeader!.ProformaInvoiceHeaderId, false);
                        }
                        else
                        {
                            response = await SaveProformaInvoice(model, hasApprove.HasApprove, false, requestId);
                        }
                    }
                    else
                    {
                        response = await SaveProformaInvoice(model, hasApprove.HasApprove, false, requestId);
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
        [Route("SaveProformaInvoice")]
        public async Task<ResponseDto> SaveProformaInvoice(ProformaInvoiceDto model, bool hasApprove, bool approved, int? requestId)
        {
            await _databaseTransaction.BeginTransaction();
            var response = await _proformaInvoiceService.SaveProformaInvoice(model, hasApprove, approved, requestId);
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
        [Route("DeleteProformaInvoice")]
        public async Task<IActionResult> DeleteProformaInvoice(int proformaInvoiceHeaderId, int requestId)
        {
            var response = new ResponseDto();
            try
            {
                var model = await _proformaInvoiceService.GetProformaInvoice(proformaInvoiceHeaderId);
                if (model.ProformaInvoiceHeader != null)
                {
                    var hasApprove = await _approvalSystemService.IsMenuHasApprove(MenuCodeData.ProformaInvoice);
                    if (hasApprove.HasApprove && hasApprove.OnDelete)
                    {
                        response = await SendApproveRequest(null, model, requestId, proformaInvoiceHeaderId, true);
                    }
                    else
                    {
                        await _databaseTransaction.BeginTransaction();
                        response = await _proformaInvoiceService.DeleteProformaInvoice(proformaInvoiceHeaderId);
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
                    response.Id = proformaInvoiceHeaderId;
                    response.Message = await _genericMessageService.GetMessage(MenuCodeData.ProformaInvoice, GenericMessageData.NotFound);
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
        public async Task<ResponseDto> SendApproveRequest([FromQuery] ProformaInvoiceDto? newModel, [FromQuery] ProformaInvoiceDto? oldModel, int requestId, int proformaInvoiceHeaderId, bool isDelete)
        {
            var changes = new List<RequestChangesDto>();
            var menuCode = MenuCodeData.ProformaInvoice;
            var requestTypeId = isDelete ? ApproveRequestTypeData.Delete : (proformaInvoiceHeaderId == 0 ? ApproveRequestTypeData.Add : ApproveRequestTypeData.Edit);
            if (requestTypeId == ApproveRequestTypeData.Edit && oldModel != null)
            {
                changes = _proformaInvoiceService.GetProformaInvoiceRequestChanges(oldModel, newModel!);
            }

            var request = new NewApproveRequestDto()
            {
                RequestId = requestId,
                MenuCode = (short)menuCode,
                ApproveRequestTypeId = requestTypeId,
                ReferenceId = proformaInvoiceHeaderId,
                ReferenceCode = proformaInvoiceHeaderId.ToString(),
                OldValue = oldModel,
                NewValue = newModel,
                Changes = changes
            };
            await _databaseTransaction.BeginTransaction();

            var validationResult = await CheckProformaInvoiceIsValid(newModel, oldModel);
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

		private async Task<ResponseDto> CheckProformaInvoiceIsValid(ProformaInvoiceDto? newModel, ProformaInvoiceDto? oldModel)
		{
			if (newModel != null)
			{
				return await _proformaInvoiceService.CheckProformaInvoiceIsValidForSave(newModel!);
			}
			else if (oldModel != null)
			{
                return await _proformaInvoiceService.CheckProformaInvoiceIsValidForDelete(oldModel.ProformaInvoiceHeader!.ProformaInvoiceHeaderId);
			}
			else
			{
				return new ResponseDto { Success = true };
			}
		}
	}
}
