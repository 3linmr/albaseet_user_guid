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
    public class ClientQuotationApprovalsController : ControllerBase
    {
        private readonly IDatabaseTransaction _databaseTransaction;
        private readonly IStringLocalizer<HandelException> _exLocalizer;
        private readonly IClientQuotationApprovalHeaderService _clientQuotationApprovalHeaderService;
        private readonly IClientQuotationApprovalService _clientQuotationApprovalService;
        private readonly IClientQuotationApprovalDetailService _clientQuotationApprovalDetailService;
        private readonly IHandleApprovalRequestService _handleApprovalRequestService;
        private readonly IApprovalSystemService _approvalSystemService;
        private readonly IGenericMessageService _genericMessageService;
		private readonly IHttpContextAccessor _httpContextAccessor;

        public ClientQuotationApprovalsController(IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exLocalizer, IHandleApprovalRequestService handleApprovalRequestService, IApprovalSystemService approvalSystemService, IClientQuotationApprovalHeaderService clientQuotationApprovalHeaderService, IClientQuotationApprovalService clientQuotationApprovalService, IClientQuotationApprovalDetailService clientQuotationApprovalDetailService, IGenericMessageService genericMessageService, IHttpContextAccessor httpContextAccessor)
        {
            _databaseTransaction = databaseTransaction;
            _exLocalizer = exLocalizer;
            _clientQuotationApprovalHeaderService = clientQuotationApprovalHeaderService;
            _clientQuotationApprovalService = clientQuotationApprovalService;
            _clientQuotationApprovalDetailService = clientQuotationApprovalDetailService;
            _handleApprovalRequestService = handleApprovalRequestService;
            _approvalSystemService = approvalSystemService;
            _genericMessageService = genericMessageService;
			_httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("ReadClientQuotationApprovals")]
        public async Task<IActionResult> ReadClientQuotationApprovals(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var data = await DataSourceLoader.LoadAsync(_clientQuotationApprovalHeaderService.GetUserClientQuotationApprovalHeaders(), loadOptions);
                return Ok(data);
            }catch
            {
                var model = _clientQuotationApprovalHeaderService.GetUserClientQuotationApprovalHeaders();
                var data = DataSourceLoader.Load(model.ToList(), loadOptions);
                return Ok(data);
            }
        }

        [HttpGet]
        [Route("ReadClientQuotationApprovalsByStoreId")]
        public async Task<IActionResult> ReadClientQuotationApprovalsByStoreId(DataSourceLoadOptions loadOptions, int storeId, int? clientId, int clientQuotationApprovalHeaderId)
        {
            try
            {
                var data = await DataSourceLoader.LoadAsync(_clientQuotationApprovalHeaderService.GetClientQuotationApprovalHeadersByStoreId(storeId, clientId, clientQuotationApprovalHeaderId), loadOptions);
                return Ok(data);
            }catch
            {
                var model = _clientQuotationApprovalHeaderService.GetClientQuotationApprovalHeadersByStoreId(storeId, clientId, clientQuotationApprovalHeaderId);
                var data = DataSourceLoader.Load(model.ToList(), loadOptions);
                return Ok(data);
            }
        }

        [HttpGet]
        [Route("GetClientQuotationApprovalFromClientQuotation")]
        public async Task<IActionResult> GetClientQuotationApprovalFromClientQuotation(int clientQuotationId)
        {
            var data = await _clientQuotationApprovalService.GetClientQuotationApprovalFromClientQuotation(clientQuotationId);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetClientQuotationApprovalCode")]
        public async Task<IActionResult> GetClientQuotationApprovalCode(int storeId, DateTime documentDate)
        {
            var data = await _clientQuotationApprovalHeaderService.GetClientQuotationApprovalCode(storeId, documentDate);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetRequestData")]
        public async Task<IActionResult> GetRequestData(int requestId)
        {
            var data = await _handleApprovalRequestService.GetRequestData(requestId);
            if (data != null)
            {
                if (data.CanBeConverted<ClientQuotationApprovalDto>())
                {
					var clientQuotationApproval = data.ConvertToType<ClientQuotationApprovalDto>();
					var userCanLook = await _httpContextAccessor.UserCanLook(0, clientQuotationApproval?.ClientQuotationApprovalHeader?.StoreId ?? 0);
                    return Ok(userCanLook ? clientQuotationApproval : new ClientQuotationApprovalDto());
                }
            }
            return Ok(new ClientQuotationApprovalDto());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetClientQuotationApproval(int id)
        {
            var clientQuotationApproval = await _clientQuotationApprovalService.GetClientQuotationApproval(id);
			var userCanLook = await _httpContextAccessor.UserCanLook(0, clientQuotationApproval.ClientQuotationApprovalHeader?.StoreId ?? 0);
            return Ok(userCanLook ? clientQuotationApproval : new ClientQuotationApprovalDto());
        }

        [HttpGet]
        [Route("GetClientQuotationApprovalDetail")]
        public async Task<IActionResult> GetClientQuotationApprovalDetail(int id)
        {
            var clientQuotationApprovalDetail = await _clientQuotationApprovalDetailService.GetClientQuotationApprovalDetails(id);
            return Ok(clientQuotationApprovalDetail);
        }

        [HttpGet]
        [Route("GetClientQuotationApprovalHeader")]
        public async Task<IActionResult> GetClientQuotationApprovalHeader(int id)
        {
            var clientQuotationApprovalHeader = await _clientQuotationApprovalHeaderService.GetClientQuotationApprovalHeaderById(id);
            return Ok(clientQuotationApprovalHeader);
        }

        [HttpPost]
        [Route("UpdateIsClosed")]
        public async Task<IActionResult> UpdateIsClosed(int clientQuotationApprovalHeaderId, bool isClosed)
        {
            return Ok(await _clientQuotationApprovalHeaderService.UpdateClosed(clientQuotationApprovalHeaderId, isClosed));
        }

        [HttpPost]
        [Route("Save")]
        public async Task<IActionResult> Save([FromBody] ClientQuotationApprovalDto model, int requestId)
        {
            var response = new ResponseDto();
            try
            {
                if (model.ClientQuotationApprovalHeader != null)
                {
                    var hasApprove = await _approvalSystemService.IsMenuHasApprove(MenuCodeData.ClientQuotationApproval);
                    if (hasApprove.HasApprove)
                    {
                        if (hasApprove.OnAdd && model.ClientQuotationApprovalHeader!.ClientQuotationApprovalHeaderId == 0)
                        {
                            response = await SendApproveRequest(model, null, requestId, 0, false);
                        }
                        else if (hasApprove.OnEdit && model.ClientQuotationApprovalHeader!.ClientQuotationApprovalHeaderId > 0)
                        {
                            var oldValue = await _clientQuotationApprovalService.GetClientQuotationApproval(model.ClientQuotationApprovalHeader!.ClientQuotationApprovalHeaderId);
                            response = await SendApproveRequest(model, oldValue, requestId, model.ClientQuotationApprovalHeader!.ClientQuotationApprovalHeaderId, false);
                        }
                        else
                        {
                            response = await SaveClientQuotationApproval(model, hasApprove.HasApprove, false, requestId);
                        }
                    }
                    else
                    {
                        response = await SaveClientQuotationApproval(model, hasApprove.HasApprove, false, requestId);
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
        [Route("SaveClientQuotationApproval")]
        public async Task<ResponseDto> SaveClientQuotationApproval(ClientQuotationApprovalDto model, bool hasApprove, bool approved, int? requestId)
        {
            await _databaseTransaction.BeginTransaction();
            var response = await _clientQuotationApprovalService.SaveClientQuotationApproval(model, hasApprove, approved, requestId);
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
        [Route("DeleteClientQuotationApproval")]
        public async Task<IActionResult> DeleteClientQuotationApproval(int clientQuotationApprovalHeaderId, int requestId)
        {
            var response = new ResponseDto();
            try
            {
                var model = await _clientQuotationApprovalService.GetClientQuotationApproval(clientQuotationApprovalHeaderId);
                if (model.ClientQuotationApprovalHeader != null)
                {
                    var hasApprove = await _approvalSystemService.IsMenuHasApprove(MenuCodeData.ClientQuotationApproval);
                    if (hasApprove.HasApprove && hasApprove.OnDelete)
                    {
                        response = await SendApproveRequest(null, model, requestId, clientQuotationApprovalHeaderId, true);
                    }
                    else
                    {
                        await _databaseTransaction.BeginTransaction();
                        response = await _clientQuotationApprovalService.DeleteClientQuotationApproval(clientQuotationApprovalHeaderId);
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
                    response.Id = clientQuotationApprovalHeaderId;
                    response.Message = await _genericMessageService.GetMessage(MenuCodeData.ClientQuotationApproval, GenericMessageData.NotFound);
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
        public async Task<ResponseDto> SendApproveRequest([FromQuery] ClientQuotationApprovalDto? newModel, [FromQuery] ClientQuotationApprovalDto? oldModel, int requestId, int clientQuotationApprovalHeaderId, bool isDelete)
        {
            var changes = new List<RequestChangesDto>();
            var menuCode = MenuCodeData.ClientQuotationApproval;
            var requestTypeId = isDelete ? ApproveRequestTypeData.Delete : (clientQuotationApprovalHeaderId == 0 ? ApproveRequestTypeData.Add : ApproveRequestTypeData.Edit);
            if (requestTypeId == ApproveRequestTypeData.Edit && oldModel != null)
            {
                changes = _clientQuotationApprovalService.GetClientQuotationApprovalRequestChanges(oldModel, newModel!);
            }

            var request = new NewApproveRequestDto()
            {
                RequestId = requestId,
                MenuCode = (short)menuCode,
                ApproveRequestTypeId = requestTypeId,
                ReferenceId = clientQuotationApprovalHeaderId,
                ReferenceCode = clientQuotationApprovalHeaderId.ToString(),
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
