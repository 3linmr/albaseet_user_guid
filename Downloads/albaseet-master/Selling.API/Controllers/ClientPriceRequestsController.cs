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
    public class ClientPriceRequestsController : ControllerBase
    {
        private readonly IDatabaseTransaction _databaseTransaction;
        private readonly IStringLocalizer<HandelException> _exLocalizer;
        private readonly IClientPriceRequestHeaderService _clientPriceRequestHeaderService;
        private readonly IClientPriceRequestService _clientPriceRequestService;
        private readonly IClientPriceRequestDetailService _clientPriceRequestDetailService;
        private readonly IHandleApprovalRequestService _handleApprovalRequestService;
        private readonly IApprovalSystemService _approvalSystemService;
        private readonly IGenericMessageService _genericMessageService;
		private readonly IHttpContextAccessor _httpContextAccessor;

        public ClientPriceRequestsController(IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exLocalizer, IHandleApprovalRequestService handleApprovalRequestService, IApprovalSystemService approvalSystemService, IClientPriceRequestHeaderService clientPriceRequestHeaderService, IClientPriceRequestService clientPriceRequestService, IClientPriceRequestDetailService clientPriceRequestDetailService, IGenericMessageService genericMessageService, IHttpContextAccessor httpContextAccessor)
        {
            _databaseTransaction = databaseTransaction;
            _exLocalizer = exLocalizer;
            _clientPriceRequestHeaderService = clientPriceRequestHeaderService;
            _clientPriceRequestService = clientPriceRequestService;
            _clientPriceRequestDetailService = clientPriceRequestDetailService;
            _handleApprovalRequestService = handleApprovalRequestService;
            _approvalSystemService = approvalSystemService;
            _genericMessageService = genericMessageService;
			_httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("ReadClientPriceRequests")]
        public async Task<IActionResult> ReadClientPriceRequests(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var data = await DataSourceLoader.LoadAsync(_clientPriceRequestHeaderService.GetUserClientPriceRequestHeaders(), loadOptions);
                return Ok(data);
            }catch
            {
                var model = _clientPriceRequestHeaderService.GetUserClientPriceRequestHeaders();
                var data = DataSourceLoader.Load(model.ToList(), loadOptions);
                return Ok(data);
            }
        }

        [HttpGet]
        [Route("ReadClientPriceRequestsByStoreId")]
        public async Task<IActionResult> ReadClientPriceRequestsByStoreId(DataSourceLoadOptions loadOptions, int storeId, int? clientId, int clientPriceRequestHeaderId)
        {
            try
            {
                var data = await DataSourceLoader.LoadAsync(_clientPriceRequestHeaderService.GetClientPriceRequestHeadersByStoreId(storeId, clientId, clientPriceRequestHeaderId), loadOptions);
                return Ok(data);
            }catch
            {
                var model = _clientPriceRequestHeaderService.GetClientPriceRequestHeadersByStoreId(storeId, clientId, clientPriceRequestHeaderId);
                var data = DataSourceLoader.Load(model.ToList(), loadOptions);
                return Ok(data);
            }
        }

        [HttpGet]
        [Route("GetClientPriceRequestCode")]
        public async Task<IActionResult> GetClientPriceRequestCode(int storeId, DateTime documentDate)
        {
            var data = await _clientPriceRequestHeaderService.GetClientPriceRequestCode(storeId, documentDate);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetRequestData")]
        public async Task<IActionResult> GetRequestData(int requestId)
        {
            var data = await _handleApprovalRequestService.GetRequestData(requestId);
            if (data != null)
            {
                if (data.CanBeConverted<ClientPriceRequestDto>())
                {
					var clientPriceRequest = data.ConvertToType<ClientPriceRequestDto>();
					var userCanLook = await _httpContextAccessor.UserCanLook(0, clientPriceRequest?.ClientPriceRequestHeader?.StoreId ?? 0);
                    return Ok(userCanLook ? clientPriceRequest : new ClientPriceRequestDto());
                }
            }
            return Ok(new ClientPriceRequestDto());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetClientPriceRequest(int id)
        {
            var clientPriceRequest = await _clientPriceRequestService.GetClientPriceRequest(id);
			var userCanLook = await _httpContextAccessor.UserCanLook(0, clientPriceRequest.ClientPriceRequestHeader?.StoreId ?? 0);
            return Ok(userCanLook ? clientPriceRequest : new ClientPriceRequestDto());
        }

        [HttpGet]
        [Route("GetClientPriceRequestDetail")]
        public async Task<IActionResult> GetClientPriceRequestDetail(int id)
        {
            var clientPriceRequestDetail = await _clientPriceRequestDetailService.GetClientPriceRequestDetails(id);
            return Ok(clientPriceRequestDetail);
        }

        [HttpGet]
        [Route("GetClientPriceRequestHeader")]
        public async Task<IActionResult> GetClientPriceRequestHeader(int id)
        {
            var clientPriceRequestHeader = await _clientPriceRequestHeaderService.GetClientPriceRequestHeaderById(id);
            return Ok(clientPriceRequestHeader);
        }

        [HttpPost]
        [Route("UpdateIsClosed")]
        public async Task<IActionResult> UpdateIsClosed(int clientPriceRequestHeaderId, bool isClosed)
        {
            return Ok(await _clientPriceRequestHeaderService.UpdateClosed(clientPriceRequestHeaderId, isClosed));
        }

        [HttpPost]
        [Route("Save")]
        public async Task<IActionResult> Save([FromBody] ClientPriceRequestDto model, int requestId)
        {
            var response = new ResponseDto();
            try
            {
                if (model.ClientPriceRequestHeader != null)
                {
                    var hasApprove = await _approvalSystemService.IsMenuHasApprove(MenuCodeData.ClientPriceRequest);
                    if (hasApprove.HasApprove)
                    {
                        if (hasApprove.OnAdd && model.ClientPriceRequestHeader!.ClientPriceRequestHeaderId == 0)
                        {
                            response = await SendApproveRequest(model, null, requestId, 0, false);
                        }
                        else if (hasApprove.OnEdit && model.ClientPriceRequestHeader!.ClientPriceRequestHeaderId > 0)
                        {
                            var oldValue = await _clientPriceRequestService.GetClientPriceRequest(model.ClientPriceRequestHeader!.ClientPriceRequestHeaderId);
                            response = await SendApproveRequest(model, oldValue, requestId, model.ClientPriceRequestHeader!.ClientPriceRequestHeaderId, false);
                        }
                        else
                        {
                            response = await SaveClientPriceRequest(model, hasApprove.HasApprove, false, requestId);
                        }
                    }
                    else
                    {
                        response = await SaveClientPriceRequest(model, hasApprove.HasApprove, false, requestId);
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
        [Route("SaveClientPriceRequest")]
        public async Task<ResponseDto> SaveClientPriceRequest(ClientPriceRequestDto model, bool hasApprove, bool approved, int? requestId)
        {
            await _databaseTransaction.BeginTransaction();
            var response = await _clientPriceRequestService.SaveClientPriceRequest(model, hasApprove, approved, requestId);
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
        [Route("DeleteClientPriceRequest")]
        public async Task<IActionResult> DeleteClientPriceRequest(int clientPriceRequestHeaderId, int requestId)
        {
            var response = new ResponseDto();
            try
            {
                var model = await _clientPriceRequestService.GetClientPriceRequest(clientPriceRequestHeaderId);
                if (model.ClientPriceRequestHeader != null)
                {
                    var hasApprove = await _approvalSystemService.IsMenuHasApprove(MenuCodeData.ClientPriceRequest);
                    if (hasApprove.HasApprove && hasApprove.OnDelete)
                    {
                        response = await SendApproveRequest(null, model, requestId, clientPriceRequestHeaderId, true);
                    }
                    else
                    {
                        await _databaseTransaction.BeginTransaction();
                        response = await _clientPriceRequestService.DeleteClientPriceRequest(clientPriceRequestHeaderId);
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
                    response.Id = clientPriceRequestHeaderId;
                    response.Message = await _genericMessageService.GetMessage(MenuCodeData.ClientPriceRequest, GenericMessageData.NotFound);
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
        public async Task<ResponseDto> SendApproveRequest([FromQuery] ClientPriceRequestDto? newModel, [FromQuery] ClientPriceRequestDto? oldModel, int requestId, int clientPriceRequestHeaderId, bool isDelete)
        {
            var changes = new List<RequestChangesDto>();
            var menuCode = MenuCodeData.ClientPriceRequest;
            var requestTypeId = isDelete ? ApproveRequestTypeData.Delete : (clientPriceRequestHeaderId == 0 ? ApproveRequestTypeData.Add : ApproveRequestTypeData.Edit);
            if (requestTypeId == ApproveRequestTypeData.Edit && oldModel != null)
            {
                changes = _clientPriceRequestService.GetClientPriceRequestRequestChanges(oldModel, newModel!);
            }

            var request = new NewApproveRequestDto()
            {
                RequestId = requestId,
                MenuCode = (short)menuCode,
                ApproveRequestTypeId = requestTypeId,
                ReferenceId = clientPriceRequestHeaderId,
                ReferenceCode = clientPriceRequestHeaderId.ToString(),
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
