using DevExtreme.AspNet.Data;
using Inventory.API.Models;
using Inventory.CoreOne.Contracts;
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
using Inventory.CoreOne.Models.Dtos.ViewModels;
using Inventory.Service.Services;
using Shared.CoreOne.Contracts.Menus;
using Shared.Helper.Identity;

namespace Inventory.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InternalTransferReceivesController : ControllerBase
    {
        private readonly IDatabaseTransaction _databaseTransaction;
        private readonly IStringLocalizer<HandelException> _exLocalizer;
        private readonly IGenericMessageService _genericMessageService;
        private readonly IInternalTransferReceiveHeaderService _internalTransferReceiveHeaderService;
        private readonly IInternalTransferReceiveDetailService _internalTransferReceiveDetailService;
        private readonly IInternalTransferReceiveService _internalTransferReceiveService;
        private readonly IInternalTransferService _internalTransferService;
        private readonly IHandleApprovalRequestService _handleApprovalRequestService;
        private readonly IApprovalSystemService _approvalSystemService;
		private readonly IHttpContextAccessor _httpContextAccessor;

        public InternalTransferReceivesController(IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exLocalizer, IGenericMessageService genericMessageService, IHandleApprovalRequestService handleApprovalRequestService, IApprovalSystemService approvalSystemService, IInternalTransferReceiveHeaderService internalTransferReceiveHeaderService,IInternalTransferReceiveDetailService internalTransferReceiveDetailService, IInternalTransferReceiveService internalTransferReceiveService, IInternalTransferService internalTransferService, IHttpContextAccessor httpContextAccessor)
        {
            _databaseTransaction = databaseTransaction;
            _exLocalizer = exLocalizer;
            _genericMessageService = genericMessageService;
            _internalTransferReceiveHeaderService = internalTransferReceiveHeaderService;
            _internalTransferReceiveDetailService = internalTransferReceiveDetailService;
            _internalTransferReceiveService = internalTransferReceiveService;
            _handleApprovalRequestService = handleApprovalRequestService;
            _approvalSystemService = approvalSystemService;
            _internalTransferService = internalTransferService;
			_httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("ReadInternalTransferReceives")]
        public async Task<IActionResult> ReadInternalTransferReceive(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var data = await DataSourceLoader.LoadAsync(_internalTransferReceiveHeaderService.GetUserInternalTransferReceiveHeaders(), loadOptions);
                return Ok(data);
            }
            catch
            {
                var model = _internalTransferReceiveHeaderService.GetUserInternalTransferReceiveHeaders();
                var data = DataSourceLoader.Load(model.ToList(), loadOptions);
                return Ok(data);
            }
        }

        [HttpGet]
        [Route("GetInternalTransferReceiveCode")]
        public async Task<IActionResult> GetInternalTransferReceiveCode(int storeId, DateTime stockDate)
        {
            var data = await _internalTransferReceiveHeaderService.GetInternalTransferReceiveCode(storeId, stockDate);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetRequestData")]
        public async Task<IActionResult> GetRequestData(int requestId)
        {
            var data = await _handleApprovalRequestService.GetRequestData(requestId);
            if (data != null)
            {
                if (data.CanBeConverted<InternalTransferReceiveDto>())
                {
					var internalTransferReceive = data.ConvertToType<InternalTransferReceiveDto>();
					var userCanLook = await _httpContextAccessor.UserCanLook(0, internalTransferReceive?.InternalTransferReceiveHeader?.ToStoreId ?? 0);
                    return Ok(userCanLook ? internalTransferReceive : new InternalTransferReceiveDto());
                }
            }
            return Ok(new InternalTransferReceiveDto());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetInternalTransferReceive(int id)
        {
            var internalTransferReceive = await _internalTransferReceiveService.GetInternalTransferReceive(id);
			var userCanLook = await _httpContextAccessor.UserCanLook(0, internalTransferReceive.InternalTransferReceiveHeader?.ToStoreId ?? 0);
            return Ok(userCanLook ? internalTransferReceive : new InternalTransferReceiveDto());
        }

        [HttpGet]
        [Route("GetInternalTransferReceiveHeader")]
        public async Task<IActionResult> GetInternalTransferReceiveHeader(int id)
        {
            var internalTransferReceiveHeader = await _internalTransferReceiveHeaderService.GetInternalTransferReceiveHeaderById(id);
            return Ok(internalTransferReceiveHeader);
        }

        [HttpGet]
        [Route("GetInternalTransferReceiveDetail")]
        public async Task<IActionResult> GetInternalTransferReceiveDetail(int id)
        {
	        var internalTransferReceiveDetails = await _internalTransferReceiveDetailService.GetInternalTransferReceiveDetails(id);
	        return Ok(internalTransferReceiveDetails);
        }

        [HttpGet]
        [Route("GetTransferReceiveFromInternalTransfer")]
        public async Task<IActionResult> GetTransferReceiveFromInternalTransfer(int internalTransferHeaderId)
        {
            var data = await _internalTransferReceiveService.GetTransferReceiveFromInternalTransfer(internalTransferHeaderId);
            return Ok(data);
        }

        [HttpPost]
        [Route("Save")]
        public async Task<IActionResult> Save([FromBody] InternalTransferReceiveDto model, int requestId)
        {
            var response = new ResponseDto();
            try
            {
                var hasApprove = await _approvalSystemService.IsMenuHasApprove(MenuCodeData.InternalReceiveItems);
                if (hasApprove.HasApprove)
                {
                    if (hasApprove.OnAdd)
                    {
                        response = await SendApproveRequest(model, null, requestId, 0, false);
                    }
                    else
                    {
                        response = await SaveInternalTransferReceive(model, hasApprove.HasApprove, false, requestId);
                    }
                }
                else
                {
                    response = await SaveInternalTransferReceive(model, hasApprove.HasApprove, false, requestId);
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
        [Route("SaveInternalTransferReceive")]
        public async Task<ResponseDto> SaveInternalTransferReceive(InternalTransferReceiveDto model, bool hasApprove, bool approved, int? requestId)
        {
            await _databaseTransaction.BeginTransaction();
            var response = await _internalTransferReceiveService.SaveInternalTransferReceive(model, hasApprove, approved, requestId);
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

        [HttpPost]
        [Route("SendApproveRequest")]
        public async Task<ResponseDto> SendApproveRequest([FromQuery] InternalTransferReceiveDto? newModel, [FromQuery] InternalTransferReceiveDto? oldModel, int requestId, int internalTransferReceiveHeaderId, bool isDelete)
        {
            var changes = new List<RequestChangesDto>();
            var menuCode = MenuCodeData.InternalReceiveItems;
            var requestTypeId = ApproveRequestTypeData.Add;

            if (newModel != null && newModel.InternalTransferReceiveHeader != null)
            {
                var internalTransferExists = await _internalTransferService.UpdateClosed(newModel.InternalTransferReceiveHeader.InternalTransferHeaderId);
                if (!internalTransferExists)
                {
                    return new ResponseDto { Id = 0, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.InternalReceiveItems, GenericMessageData.NotFound) };
                }
            }
            else
            {
                return new ResponseDto();
            }

            var request = new NewApproveRequestDto()
            {
                RequestId = requestId,
                MenuCode = (short)menuCode,
                ApproveRequestTypeId = requestTypeId,
                ReferenceId = internalTransferReceiveHeaderId,
                ReferenceCode = internalTransferReceiveHeaderId.ToString(),
                OldValue = null,
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
