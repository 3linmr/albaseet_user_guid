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
    public class ClientDebitMemosController : Controller
    {
        private readonly IClientDebitMemoService _clientDebitMemoService;
        private readonly IClientDebitMemoHandlingService _clientDebitMemoHandlingService;
        private readonly IHandleApprovalRequestService _handleApprovalRequestService;
        private readonly IApprovalSystemService _approvalSystemService;
        private readonly IDatabaseTransaction _databaseTransaction;
        private readonly IStringLocalizer<HandelException> _exLocalizer;
        private readonly IGenericMessageService _genericMessageService;
		private readonly IHttpContextAccessor _httpContextAccessor;

        public ClientDebitMemosController(IClientDebitMemoService clientDebitMemoService, IClientDebitMemoHandlingService clientDebitMemoHandlingService, IHandleApprovalRequestService handleApprovalRequestService, IApprovalSystemService approvalSystemService, IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exLocalizer, IGenericMessageService genericMessageService, IHttpContextAccessor httpContextAccessor)
        {
            _clientDebitMemoService = clientDebitMemoService;
            _clientDebitMemoHandlingService = clientDebitMemoHandlingService;
            _handleApprovalRequestService = handleApprovalRequestService;
            _approvalSystemService = approvalSystemService;
            _databaseTransaction = databaseTransaction;
            _exLocalizer = exLocalizer;
            _genericMessageService = genericMessageService;
			_httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("ReadClientDebitMemos")]
        public async Task<IActionResult> ReadClientDebitMemos(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var data = await DataSourceLoader.LoadAsync(_clientDebitMemoService.GetUserClientDebitMemos(), loadOptions);
                return Ok(data);
            }
            catch
            {
                var model = _clientDebitMemoService.GetUserClientDebitMemos();
                var data = DataSourceLoader.Load(model.ToList(), loadOptions);
                return Ok(data);
            }
        }

        [HttpGet]
        [Route("GetClientDebitMemoCode")]
        public async Task<IActionResult> GetClientDebitMemoCode(int storeId, DateTime documentDate)
        {
            return Ok(await _clientDebitMemoService.GetClientDebitMemoCode(storeId, documentDate));
        }

        [HttpGet]
        [Route("GetClientDebitMemoFromSalesInvoice")]
        public async Task<IActionResult> GetClientDebitMemoFromSalesInvoice(int salesInvoiceHeaderId)
        {
            var data = await _clientDebitMemoHandlingService.GetClientDebitMemoFromSalesInvoice(salesInvoiceHeaderId);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetRequestData")]
        public async Task<IActionResult> GetRequestData(int requestId)
        {
            var data = await _handleApprovalRequestService.GetRequestData(requestId);
            if (data != null)
            {
                if (data.CanBeConverted<ClientDebitMemoVm>())
                {
				    var clientDebitMemo = data.ConvertToType<ClientDebitMemoVm>();
				    var userCanLook = await _httpContextAccessor.UserCanLook(0, clientDebitMemo?.ClientDebitMemo?.StoreId ?? 0);
                    return Ok(userCanLook ? clientDebitMemo : new ClientDebitMemoVm());
                }
            }
            return Ok(new ClientDebitMemoVm());
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetClientDebitMemo(int id)
        {
            var data = await _clientDebitMemoHandlingService.GetClientDebitMemo(id);
			var userCanLook = await _httpContextAccessor.UserCanLook(0, data.ClientDebitMemo?.StoreId ?? 0);
            return Ok(userCanLook ? data : new ClientDebitMemoVm());
        }

        [HttpGet]
        [Route("GetClientDebitMemoWithoutJournal")]
        public async Task<IActionResult> GetClientDebitMemoWithoutJournal(int id)
        {
            var clientDebitMemo = await _clientDebitMemoService.GetClientDebitMemoById(id);
            return Ok(clientDebitMemo);
        }

        [HttpPost]
        [Route("Save")]
        public async Task<IActionResult> Save([FromBody] ClientDebitMemoVm model, int requestId)
        {
            var response = new ResponseDto();
            try
            {
                if (model.ClientDebitMemo != null)
                {
                    var hasApprove = await _approvalSystemService.IsMenuHasApprove(MenuCodeData.ClientDebitMemo);
                    if (hasApprove.HasApprove)
                    {
                        if (hasApprove.OnAdd && model.ClientDebitMemo!.ClientDebitMemoId == 0)
                        {
                            response = await SendApproveRequest(model, null, requestId, 0, false);
                        }
                        else if (hasApprove.OnEdit && model.ClientDebitMemo!.ClientDebitMemoId > 0)
                        {
                            var oldValue = await _clientDebitMemoHandlingService.GetClientDebitMemo(model.ClientDebitMemo!.ClientDebitMemoId);
                            response = await SendApproveRequest(model, oldValue, requestId, model.ClientDebitMemo!.ClientDebitMemoId, false);
                        }
                        else
                        {
                            response = await SaveClientDebitMemo(model, hasApprove.HasApprove, false, requestId);
                        }
                    }
                    else
                    {
                        response = await SaveClientDebitMemo(model, hasApprove.HasApprove, false, requestId);
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

        private async Task<ResponseDto> SaveClientDebitMemo(ClientDebitMemoVm model, bool hasApprove, bool approved, int? requestId)
        {
            await _databaseTransaction.BeginTransaction();
            var response = await _clientDebitMemoHandlingService.SaveClientDebitMemoInFull(model, hasApprove, approved, requestId);
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
        [Route("DeleteClientDebitMemo")]
        public async Task<IActionResult> DeleteClientDebitMemo(int clientDebitMemoId, int requestId)
        {
            var response = new ResponseDto();
            try
            {
                var model = await _clientDebitMemoHandlingService.GetClientDebitMemo(clientDebitMemoId);
                if (model.ClientDebitMemo != null)
                {
                    var hasApprove = await _approvalSystemService.IsMenuHasApprove(MenuCodeData.ClientDebitMemo);
                    if (hasApprove.HasApprove && hasApprove.OnDelete)
                    {
                        response = await SendApproveRequest(null, model, requestId, clientDebitMemoId, true);
                    }
                    else
                    {
                        await _databaseTransaction.BeginTransaction();
                        response = await _clientDebitMemoHandlingService.DeleteClientDebitMemoInFull(clientDebitMemoId);
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
                    response.Id = clientDebitMemoId;
                    response.Message = await _genericMessageService.GetMessage(MenuCodeData.ClientDebitMemo, GenericMessageData.NotFound);
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

        private async Task<ResponseDto> SendApproveRequest(ClientDebitMemoVm? newModel, ClientDebitMemoVm? oldModel, int requestId, int clientDebitMemoId, bool isDelete)
        {
            var changes = new List<RequestChangesDto>();
            var menuCode = MenuCodeData.ClientDebitMemo;
            var requestTypeId = isDelete ? ApproveRequestTypeData.Delete : (clientDebitMemoId == 0 ? ApproveRequestTypeData.Add : ApproveRequestTypeData.Edit);
            if (requestTypeId == ApproveRequestTypeData.Edit && oldModel != null)
            {
                changes = _clientDebitMemoHandlingService.GetClientDebitMemoRequestChanges(oldModel, newModel!);
            }

            var request = new NewApproveRequestDto()
            {
                RequestId = requestId,
                MenuCode = (short)menuCode,
                ApproveRequestTypeId = requestTypeId,
                ReferenceId = clientDebitMemoId,
                ReferenceCode = clientDebitMemoId.ToString(),
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
