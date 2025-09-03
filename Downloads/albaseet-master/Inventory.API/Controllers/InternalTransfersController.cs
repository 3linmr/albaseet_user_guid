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
    public class InternalTransfersController : ControllerBase
    {
        private readonly IDatabaseTransaction _databaseTransaction;
        private readonly IStringLocalizer<HandelException> _exLocalizer;
        private readonly IInternalTransferHeaderService _internalTransferHeaderService;
        private readonly IInternalTransferService _internalTransferService;
        private readonly IInternalTransferDetailService _internalTransferDetailService;
        private readonly IHandleApprovalRequestService _handleApprovalRequestService;
        private readonly IApprovalSystemService _approvalSystemService;
        private readonly IGenericMessageService _genericMessageService;
		private readonly IHttpContextAccessor _httpContextAccessor;

        public InternalTransfersController(IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exLocalizer, IHandleApprovalRequestService handleApprovalRequestService, IApprovalSystemService approvalSystemService, IInternalTransferHeaderService internalTransferHeaderService, IInternalTransferService internalTransferService,IInternalTransferDetailService internalTransferDetailService, IGenericMessageService genericMessageService, IHttpContextAccessor httpContextAccessor)
        {
            _databaseTransaction = databaseTransaction;
            _exLocalizer = exLocalizer;
            _internalTransferHeaderService = internalTransferHeaderService;
            _internalTransferService = internalTransferService;
            _internalTransferDetailService = internalTransferDetailService;
            _handleApprovalRequestService = handleApprovalRequestService;
            _approvalSystemService = approvalSystemService;
            _genericMessageService = genericMessageService;
			_httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("ReadInternalTransfers")]
        public async Task<IActionResult> ReadInternalTransfer(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var data = await DataSourceLoader.LoadAsync(_internalTransferHeaderService.GetUserInternalTransferHeaders(), loadOptions);
                return Ok(data);
            }
            catch
            {
                var model = _internalTransferHeaderService.GetUserInternalTransferHeaders();
                var data = DataSourceLoader.Load(model.ToList(), loadOptions);
                return Ok(data);
            }
        }

        [HttpGet]
        [Route("ReadClosedInternalTransfers")]
        public async Task<IActionResult> ReadClosedInternalTransfers(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var data = await DataSourceLoader.LoadAsync(_internalTransferHeaderService.GetClosedInternalTransfers(), loadOptions);
                return Ok(data);
            }
            catch
            {
                var model = _internalTransferHeaderService.GetClosedInternalTransfers();
                var data = DataSourceLoader.Load(model.ToList(), loadOptions);
                return Ok(data);
            }
        }

        [HttpGet]
        [Route("ReadPendingInternalTransfers")]
        public async Task<IActionResult> ReadPendingInternalTransfers(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var data = await DataSourceLoader.LoadAsync(_internalTransferHeaderService.GetPendingInternalTransfers(), loadOptions);
                return Ok(data);
            }
            catch
            {
                var model = _internalTransferHeaderService.GetPendingInternalTransfers();
                var data = DataSourceLoader.Load(model.ToList(), loadOptions);
                return Ok(data);
            }
        }

		[HttpGet]
        [Route("GetInternalTransferCode")]
        public async Task<IActionResult> GetInternalTransferCode(int storeId, DateTime stockDate)
        {
            var data = await _internalTransferHeaderService.GetInternalTransferCode(storeId, stockDate);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetRequestData")]
        public async Task<IActionResult> GetRequestData(int requestId)
        {
            var data = await _handleApprovalRequestService.GetRequestData(requestId);
            if (data != null && data.CanBeConverted<InternalTransferDto>())
            {
                var internalTransfer = data.ConvertToType<InternalTransferDto>();
                if (internalTransfer?.InternalTransferHeader == null) return Ok(internalTransfer);
				
				var userCanLook = await _httpContextAccessor.UserCanLook(0, internalTransfer.InternalTransferHeader.FromStoreId);
				if (!userCanLook) return Ok(new InternalTransferDto());

                await _internalTransferService.ModifyInternalTransferDetailsWithStoreIdAndAvaialbleBalance(internalTransfer.InternalTransferHeader.InternalTransferHeaderId, internalTransfer.InternalTransferHeader.FromStoreId, internalTransfer.InternalTransferDetails, true);

				return Ok(internalTransfer);
			}
			return Ok(new InternalTransferDto());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetInternalTransfer(int id)
        {
            var internalTransfer = await _internalTransferService.GetInternalTransfer(id);
			var userCanLook = await _httpContextAccessor.UserCanLook(0, internalTransfer.InternalTransferHeader?.FromStoreId ?? 0);
            return Ok(userCanLook ? internalTransfer : new InternalTransferDto());
        }
       
        [HttpGet]
        [Route("GetInternalTransferHeader")]
        public async Task<IActionResult> GetInternalTransferHeader(int id)
        {
            var internalTransferHeader = await _internalTransferHeaderService.GetInternalTransferHeaderById(id);
            return Ok(internalTransferHeader);
        }

        [HttpGet]
        [Route("GetInternalTransferDetail")]
        public async Task<IActionResult> GetInternalTransferDetail(int id)
        {
	        var internalTransferDetails = await _internalTransferDetailService.GetInternalTransferDetails(id);
	        return Ok(internalTransferDetails);
        }

		[HttpPost]
        [Route("Save")]
        public async Task<IActionResult> Save([FromBody] InternalTransferDto model, int requestId)
        {
            var response = new ResponseDto();
            try
            {
                if (model.InternalTransferHeader != null)
                {
                    var hasApprove = await _approvalSystemService.IsMenuHasApprove(MenuCodeData.InternalTransferItems);
                    if (hasApprove.HasApprove)
                    {
                        if (hasApprove.OnAdd && model.InternalTransferHeader!.InternalTransferHeaderId == 0)
                        {
                            response = await SendApproveRequest(model, null, requestId, 0, false);
                        }
                        else if (hasApprove.OnEdit && model.InternalTransferHeader!.InternalTransferHeaderId > 0)
                        {
                            var oldValue = await _internalTransferService.GetInternalTransfer(model.InternalTransferHeader!.InternalTransferHeaderId);
                            response = await SendApproveRequest(model, oldValue, requestId, model.InternalTransferHeader!.InternalTransferHeaderId, false);
                        }
                        else
                        {
                            response = await SaveInternalTransfer(model, hasApprove.HasApprove, false, requestId);
                        }
                    }
                    else
                    {
                        response = await SaveInternalTransfer(model, hasApprove.HasApprove, false, requestId);
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
        [Route("SaveInternalTransfer")]
        public async Task<ResponseDto> SaveInternalTransfer(InternalTransferDto model, bool hasApprove, bool approved, int? requestId)
        {
            await _databaseTransaction.BeginTransaction();
            var response = await _internalTransferService.SaveInternalTransfer(model, hasApprove, approved, requestId);
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
        [Route("DeleteInternalTransfer")]
        public async Task<IActionResult> DeleteInternalTransfer(int internalTransferHeaderId, int requestId)
        {
            var response = new ResponseDto();
            try
            {
                var model = await _internalTransferService.GetInternalTransfer(internalTransferHeaderId);
                if (model.InternalTransferHeader != null)
                {
                    var hasApprove = await _approvalSystemService.IsMenuHasApprove(MenuCodeData.InternalTransferItems);
                    if (hasApprove.HasApprove && hasApprove.OnDelete)
                    {
                        response = await SendApproveRequest(null, model, requestId, internalTransferHeaderId, true);
                    }
                    else
                    {
                        await _databaseTransaction.BeginTransaction();
                        response = await _internalTransferService.DeleteInternalTransfer(internalTransferHeaderId);
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
                    response.Id = internalTransferHeaderId;
                    response.Message = await _genericMessageService.GetMessage(MenuCodeData.InternalTransferItems, GenericMessageData.NotFound);
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
        public async Task<ResponseDto> SendApproveRequest([FromQuery] InternalTransferDto? newModel, [FromQuery] InternalTransferDto? oldModel, int requestId, int internalTransferHeaderId, bool isDelete)
        {
            var changes = new List<RequestChangesDto>();
            var menuCode = MenuCodeData.InternalTransferItems;
            var requestTypeId = isDelete ? ApproveRequestTypeData.Delete : (internalTransferHeaderId == 0 ? ApproveRequestTypeData.Add : ApproveRequestTypeData.Edit);
            if (requestTypeId == ApproveRequestTypeData.Edit && oldModel != null)
            {
                changes = _internalTransferService.GetInternalTransferRequestChanges(oldModel, newModel!);
            }

            var request = new NewApproveRequestDto()
            {
                RequestId = requestId,
                MenuCode = (short)menuCode,
                ApproveRequestTypeId = requestTypeId,
                ReferenceId = internalTransferHeaderId,
                ReferenceCode = internalTransferHeaderId.ToString(),
                OldValue = oldModel,
                NewValue = newModel,
                Changes = changes
            };
            await _databaseTransaction.BeginTransaction();

			if (requestTypeId != ApproveRequestTypeData.Delete)
			{
				var validationResult = await _internalTransferService.CheckInventoryZeroStock(newModel!.InternalTransferHeader!.FromStoreId, newModel.InternalTransferDetails, oldModel?.InternalTransferDetails ?? []);
				if (validationResult.Success == false)
				{
					await _databaseTransaction.Rollback();
					return validationResult;
				}
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
    }
}
