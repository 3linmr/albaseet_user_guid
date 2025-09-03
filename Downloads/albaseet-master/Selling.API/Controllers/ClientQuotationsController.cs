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
    public class ClientQuotationsController : ControllerBase
    {
        private readonly IDatabaseTransaction _databaseTransaction;
        private readonly IStringLocalizer<HandelException> _exLocalizer;
        private readonly IClientQuotationHeaderService _clientQuotationHeaderService;
        private readonly IClientQuotationService _clientQuotationService;
        private readonly IClientQuotationDetailService _clientQuotationDetailService;
        private readonly IHandleApprovalRequestService _handleApprovalRequestService;
        private readonly IApprovalSystemService _approvalSystemService;
        private readonly IGenericMessageService _genericMessageService;
		private readonly IHttpContextAccessor _httpContextAccessor;

        public ClientQuotationsController(IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exLocalizer, IHandleApprovalRequestService handleApprovalRequestService, IApprovalSystemService approvalSystemService, IClientQuotationHeaderService clientQuotationHeaderService, IClientQuotationService clientQuotationService, IClientQuotationDetailService clientQuotationDetailService, IGenericMessageService genericMessageService, IHttpContextAccessor httpContextAccessor)
        {
            _databaseTransaction = databaseTransaction;
            _exLocalizer = exLocalizer;
            _clientQuotationHeaderService = clientQuotationHeaderService;
            _clientQuotationService = clientQuotationService;
            _clientQuotationDetailService = clientQuotationDetailService;
            _handleApprovalRequestService = handleApprovalRequestService;
            _approvalSystemService = approvalSystemService;
            _genericMessageService = genericMessageService;
			_httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("ReadClientQuotations")]
        public async Task<IActionResult> ReadClientQuotations(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var data = await DataSourceLoader.LoadAsync(_clientQuotationHeaderService.GetUserClientQuotationHeaders(), loadOptions);
                return Ok(data);
            }catch
            {
                var model = _clientQuotationHeaderService.GetUserClientQuotationHeaders();
                var data = DataSourceLoader.Load(model.ToList(), loadOptions);
                return Ok(data);
            }
        }

        [HttpGet]
        [Route("ReadClientQuotationsByStoreId")]
        public async Task<IActionResult> ReadClientQuotationsByStoreId(DataSourceLoadOptions loadOptions, int storeId, int? clientId, int clientQuotationHeaderId)
        {
            try
            {
                var data = await DataSourceLoader.LoadAsync(_clientQuotationHeaderService.GetClientQuotationHeadersByStoreId(storeId, clientId, clientQuotationHeaderId), loadOptions);
                return Ok(data);
            }catch
            {
                var model = _clientQuotationHeaderService.GetClientQuotationHeadersByStoreId(storeId, clientId, clientQuotationHeaderId);
                var data = DataSourceLoader.Load(model.ToList(), loadOptions);
                return Ok(data);
            }
        }

        [HttpGet]
        [Route("GetDefaultValidDate")]
        public async Task<ActionResult<ExpirationDaysAndDateDto>> GetDefaultValidDate(int storeId)
        {
            return Ok(await _clientQuotationService.GetDefaultValidDate(storeId));
        }

        [HttpGet]
        [Route("GetClientQuotationFromClientPriceRequest")]
        public async Task<IActionResult> GetClientQuotationFromClientPriceRequest(int clientPriceRequestId)
        {
            var data = await _clientQuotationService.GetClientQuotationFromClientPriceRequest(clientPriceRequestId);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetClientQuotationCode")]
        public async Task<IActionResult> GetClientQuotationCode(int storeId, DateTime documentDate)
        {
            var data = await _clientQuotationHeaderService.GetClientQuotationCode(storeId, documentDate);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetRequestData")]
        public async Task<IActionResult> GetRequestData(int requestId)
        {
            var data = await _handleApprovalRequestService.GetRequestData(requestId);
            if (data != null)
            {
                if (data.CanBeConverted<ClientQuotationDto>())
                {
					var clientQuotation = data.ConvertToType<ClientQuotationDto>();
					var userCanLook = await _httpContextAccessor.UserCanLook(0, clientQuotation?.ClientQuotationHeader?.StoreId ?? 0);
                    return Ok(userCanLook ? clientQuotation : new ClientQuotationDto());
                }
            }
            return Ok(new ClientQuotationDto());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetClientQuotation(int id)
        {
            var clientQuotation = await _clientQuotationService.GetClientQuotation(id);
			var userCanLook = await _httpContextAccessor.UserCanLook(0, clientQuotation.ClientQuotationHeader?.StoreId ?? 0);
            return Ok(userCanLook ? clientQuotation : new ClientQuotationDto());
        }

        [HttpGet]
        [Route("GetClientQuotationDetail")]
        public async Task<IActionResult> GetClientQuotationDetail(int id)
        {
            var clientQuotationDetail = await _clientQuotationDetailService.GetClientQuotationDetails(id);
            return Ok(clientQuotationDetail);
        }

        [HttpGet]
        [Route("GetClientQuotationHeader")]
        public async Task<IActionResult> GetClientQuotationHeader(int id)
        {
            var clientQuotationHeader = await _clientQuotationHeaderService.GetClientQuotationHeaderById(id);
            return Ok(clientQuotationHeader);
        }

        [HttpPost]
        [Route("UpdateIsClosed")]
        public async Task<IActionResult> UpdateIsClosed(int clientQuotationHeaderId, bool isClosed)
        {
            return Ok(await _clientQuotationHeaderService.UpdateClosed(clientQuotationHeaderId, isClosed));
        }

        [HttpPost]
        [Route("Save")]
        public async Task<IActionResult> Save([FromBody] ClientQuotationDto model, int requestId)
        {
            var response = new ResponseDto();
            try
            {
                if (model.ClientQuotationHeader != null)
                {
                    var hasApprove = await _approvalSystemService.IsMenuHasApprove(MenuCodeData.ClientQuotation);
                    if (hasApprove.HasApprove)
                    {
                        if (hasApprove.OnAdd && model.ClientQuotationHeader!.ClientQuotationHeaderId == 0)
                        {
                            response = await SendApproveRequest(model, null, requestId, 0, false);
                        }
                        else if (hasApprove.OnEdit && model.ClientQuotationHeader!.ClientQuotationHeaderId > 0)
                        {
                            var oldValue = await _clientQuotationService.GetClientQuotation(model.ClientQuotationHeader!.ClientQuotationHeaderId);
                            response = await SendApproveRequest(model, oldValue, requestId, model.ClientQuotationHeader!.ClientQuotationHeaderId, false);
                        }
                        else
                        {
                            response = await SaveClientQuotation(model, hasApprove.HasApprove, false, requestId);
                        }
                    }
                    else
                    {
                        response = await SaveClientQuotation(model, hasApprove.HasApprove, false, requestId);
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
        [Route("SaveClientQuotation")]
        public async Task<ResponseDto> SaveClientQuotation(ClientQuotationDto model, bool hasApprove, bool approved, int? requestId)
        {
            await _databaseTransaction.BeginTransaction();
            var response = await _clientQuotationService.SaveClientQuotation(model, hasApprove, approved, requestId);
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
        [Route("DeleteClientQuotation")]
        public async Task<IActionResult> DeleteClientQuotation(int clientQuotationHeaderId, int requestId)
        {
            var response = new ResponseDto();
            try
            {
                var model = await _clientQuotationService.GetClientQuotation(clientQuotationHeaderId);
                if (model.ClientQuotationHeader != null)
                {
                    var hasApprove = await _approvalSystemService.IsMenuHasApprove(MenuCodeData.ClientQuotation);
                    if (hasApprove.HasApprove && hasApprove.OnDelete)
                    {
                        response = await SendApproveRequest(null, model, requestId, clientQuotationHeaderId, true);
                    }
                    else
                    {
                        await _databaseTransaction.BeginTransaction();
                        response = await _clientQuotationService.DeleteClientQuotation(clientQuotationHeaderId);
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
                    response.Id = clientQuotationHeaderId;
                    response.Message = await _genericMessageService.GetMessage(MenuCodeData.ClientQuotation, GenericMessageData.NotFound);
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
        public async Task<ResponseDto> SendApproveRequest([FromQuery] ClientQuotationDto? newModel, [FromQuery] ClientQuotationDto? oldModel, int requestId, int clientQuotationHeaderId, bool isDelete)
        {
            var changes = new List<RequestChangesDto>();
            var menuCode = MenuCodeData.ClientQuotation;
            var requestTypeId = isDelete ? ApproveRequestTypeData.Delete : (clientQuotationHeaderId == 0 ? ApproveRequestTypeData.Add : ApproveRequestTypeData.Edit);
            if (requestTypeId == ApproveRequestTypeData.Edit && oldModel != null)
            {
                changes = _clientQuotationService.GetClientQuotationRequestChanges(oldModel, newModel!);
            }

            var request = new NewApproveRequestDto()
            {
                RequestId = requestId,
                MenuCode = (short)menuCode,
                ApproveRequestTypeId = requestTypeId,
                ReferenceId = clientQuotationHeaderId,
                ReferenceCode = clientQuotationHeaderId.ToString(),
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
