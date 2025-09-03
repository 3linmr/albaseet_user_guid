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
using Shared.Service.Services.Approval;
using Shared.Service;
using Sales.CoreOne.Models.Domain;
using Shared.CoreOne.Contracts.Menus;
using Shared.Helper.Identity;

namespace Sales.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ClientCreditMemosController : Controller
    {
        private readonly IClientCreditMemoService _clientCreditMemoService;
        private readonly IClientCreditMemoHandlingService _clientCreditMemoHandlingService;
        private readonly IHandleApprovalRequestService _handleApprovalRequestService;
        private readonly IApprovalSystemService _approvalSystemService;
        private readonly IDatabaseTransaction _databaseTransaction;
        private readonly IStringLocalizer<HandelException> _exLocalizer;
        private readonly IGenericMessageService _genericMessageService;
		private readonly IHttpContextAccessor _httpContextAccessor;

        public ClientCreditMemosController(IClientCreditMemoService clientCreditMemoService, IClientCreditMemoHandlingService clientCreditMemoHandlingService, IHandleApprovalRequestService handleApprovalRequestService, IApprovalSystemService approvalSystemService, IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exLocalizer, IGenericMessageService genericMessageService, IHttpContextAccessor httpContextAccessor)
        {
            _clientCreditMemoService = clientCreditMemoService;
            _clientCreditMemoHandlingService = clientCreditMemoHandlingService;
            _handleApprovalRequestService = handleApprovalRequestService;
            _approvalSystemService = approvalSystemService;
            _databaseTransaction = databaseTransaction;
            _exLocalizer = exLocalizer;
            _genericMessageService = genericMessageService;
			_httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("ReadClientCreditMemos")]
        public async Task<IActionResult> ReadClientCreditMemos(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var data = await DataSourceLoader.LoadAsync(_clientCreditMemoService.GetUserClientCreditMemos(), loadOptions);
                return Ok(data);
            }
            catch
            {
                var model = _clientCreditMemoService.GetUserClientCreditMemos();
                var data = DataSourceLoader.Load(model.ToList(), loadOptions);
                return Ok(data);
            }
        }

        [HttpGet]
        [Route("GetClientCreditMemoCode")]
        public async Task<IActionResult> GetClientCreditMemoCode(int storeId, DateTime documentDate)
        {
            return Ok(await _clientCreditMemoService.GetClientCreditMemoCode(storeId, documentDate));
        }

        [HttpGet]
        [Route("GetClientCreditMemoFromSalesInvoice")]
        public async Task<IActionResult> GetClientCreditMemoFromSalesInvoice(int salesInvoiceHeaderId)
        {
            var data = await _clientCreditMemoHandlingService.GetClientCreditMemoFromSalesInvoice(salesInvoiceHeaderId);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetRequestData")]
        public async Task<IActionResult> GetRequestData(int requestId)
        {
            var data = await _handleApprovalRequestService.GetRequestData(requestId);
            if (data != null)
            {
                if (data.CanBeConverted<ClientCreditMemoVm>())
                {
				    var clientCreditMemo = data.ConvertToType<ClientCreditMemoVm>();
				    var userCanLook = await _httpContextAccessor.UserCanLook(0, clientCreditMemo?.ClientCreditMemo?.StoreId ?? 0);
                    return Ok(userCanLook ? clientCreditMemo : new ClientCreditMemoVm());
                }
            }
            return Ok(new ClientCreditMemoVm());
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetClientCreditMemo(int id)
        {
            var data = await _clientCreditMemoHandlingService.GetClientCreditMemo(id);
			var userCanLook = await _httpContextAccessor.UserCanLook(0, data.ClientCreditMemo?.StoreId ?? 0);
            return Ok(userCanLook ? data : new ClientCreditMemoVm());
        }

        [HttpGet]
        [Route("GetClientCreditMemoWithoutJournal")]
        public async Task<IActionResult> GetClientCreditMemoWithoutJournal(int id)
        {
            var clientCreditMemo = await _clientCreditMemoService.GetClientCreditMemoById(id);
            return Ok(clientCreditMemo);
        }

        [HttpPost]
        [Route("Save")]
        public async Task<IActionResult> Save([FromBody] ClientCreditMemoVm model, int requestId)
        {
            var response = new ResponseDto();
            try
            {
                if (model.ClientCreditMemo != null)
                {
                    var hasApprove = await _approvalSystemService.IsMenuHasApprove(MenuCodeData.ClientCreditMemo);
                    if (hasApprove.HasApprove)
                    {
                        if (hasApprove.OnAdd && model.ClientCreditMemo!.ClientCreditMemoId == 0)
                        {
                            response = await SendApproveRequest(model, null, requestId, 0, false);
                        }
                        else if (hasApprove.OnEdit && model.ClientCreditMemo!.ClientCreditMemoId > 0)
                        {
                            var oldValue = await _clientCreditMemoHandlingService.GetClientCreditMemo(model.ClientCreditMemo!.ClientCreditMemoId);
                            response = await SendApproveRequest(model, oldValue, requestId, model.ClientCreditMemo!.ClientCreditMemoId, false);
                        }
                        else
                        {
                            response = await SaveClientCreditMemo(model, hasApprove.HasApprove, false, requestId);
                        }
                    }
                    else
                    {
                        response = await SaveClientCreditMemo(model, hasApprove.HasApprove, false, requestId);
                    }
                }
                else
                {
                    response.Message = "Memo should not be null";
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

        private async Task<ResponseDto> SaveClientCreditMemo(ClientCreditMemoVm model, bool hasApprove, bool approved, int? requestId)
        {
            await _databaseTransaction.BeginTransaction();
            var response = await _clientCreditMemoHandlingService.SaveClientCreditMemoInFull(model, hasApprove, approved, requestId);
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
        [Route("DeleteClientCreditMemo")]
        public async Task<IActionResult> DeleteClientCreditMemo(int clientCreditMemoId, int requestId)
        {
            var response = new ResponseDto();
            try
            {
                var model = await _clientCreditMemoHandlingService.GetClientCreditMemo(clientCreditMemoId);
                if (model.ClientCreditMemo != null)
                {
                    var hasApprove = await _approvalSystemService.IsMenuHasApprove(MenuCodeData.ClientCreditMemo);
                    if (hasApprove.HasApprove && hasApprove.OnDelete)
                    {
                        response = await SendApproveRequest(null, model, requestId, clientCreditMemoId, true);
                    }
                    else
                    {
                        await _databaseTransaction.BeginTransaction();
                        response = await _clientCreditMemoHandlingService.DeleteClientCreditMemoInFull(clientCreditMemoId);
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
                    response.Id = clientCreditMemoId;
                    response.Message = await _genericMessageService.GetMessage(MenuCodeData.ClientCreditMemo, GenericMessageData.NotFound);
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

        private async Task<ResponseDto> SendApproveRequest(ClientCreditMemoVm? newModel, ClientCreditMemoVm? oldModel, int requestId, int clientCreditMemoId, bool isDelete)
        {
            var changes = new List<RequestChangesDto>();
            var menuCode = MenuCodeData.ClientCreditMemo;
            var requestTypeId = isDelete ? ApproveRequestTypeData.Delete : (clientCreditMemoId == 0 ? ApproveRequestTypeData.Add : ApproveRequestTypeData.Edit);
            if (requestTypeId == ApproveRequestTypeData.Edit && oldModel != null)
            {
                changes = _clientCreditMemoHandlingService.GetClientCreditMemoRequestChanges(oldModel, newModel!);
            }

            var request = new NewApproveRequestDto()
            {
                RequestId = requestId,
                MenuCode = (short)menuCode,
                ApproveRequestTypeId = requestTypeId,
                ReferenceId = clientCreditMemoId,
                ReferenceCode = clientCreditMemoId.ToString(),
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
