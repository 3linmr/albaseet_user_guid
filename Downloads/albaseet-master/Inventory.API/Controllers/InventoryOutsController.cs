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
    public class InventoryOutsController : ControllerBase
    {
        private readonly IDatabaseTransaction _databaseTransaction;
        private readonly IStringLocalizer<HandelException> _exLocalizer;
        private readonly IInventoryOutHeaderService _inventoryOutHeaderService;
        private readonly IInventoryOutService _inventoryOutService;
        private readonly IHandleApprovalRequestService _handleApprovalRequestService;
        private readonly IApprovalSystemService _approvalSystemService;
        private readonly IGenericMessageService _genericMessageService;
		private readonly IHttpContextAccessor _httpContextAccessor;

        public InventoryOutsController(IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exLocalizer, IHandleApprovalRequestService handleApprovalRequestService, IApprovalSystemService approvalSystemService, IInventoryOutHeaderService inventoryOutHeaderService, IInventoryOutService inventoryOutService, IGenericMessageService genericMessageService, IHttpContextAccessor httpContextAccessor)
        {
            _databaseTransaction = databaseTransaction;
            _exLocalizer = exLocalizer;
            _inventoryOutHeaderService = inventoryOutHeaderService;
            _inventoryOutService = inventoryOutService;
            _handleApprovalRequestService = handleApprovalRequestService;
            _approvalSystemService = approvalSystemService;
            _genericMessageService = genericMessageService;
			_httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("ReadInventoryOuts")]
        public async Task<IActionResult> ReadInventoryOuts(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var data = await DataSourceLoader.LoadAsync(_inventoryOutHeaderService.GetUserInventoryOutHeaders(), loadOptions);
                return Ok(data);
            }
            catch
            {
                var model = _inventoryOutHeaderService.GetUserInventoryOutHeaders();
                var data = DataSourceLoader.Load(model.ToList(), loadOptions);
                return Ok(data);
            }
        }

        [HttpGet]
        [Route("GetInventoryOutCode")]
        public async Task<IActionResult> GetInventoryOutCode(int storeId, DateTime stockDate)
        {
            var data = await _inventoryOutHeaderService.GetInventoryOutCode(storeId, stockDate);
            return Ok(data);
		}

		[HttpGet]
		[Route("GetRequestData")]
		public async Task<IActionResult> GetRequestData(int requestId)
		{
			var data = await _handleApprovalRequestService.GetRequestData(requestId);
			if (data != null && data.CanBeConverted<InventoryOutDto>())
			{
				var inventoryOut = data.ConvertToType<InventoryOutDto>();
				if (inventoryOut?.InventoryOutHeader == null) return Ok(inventoryOut);
				
				var userCanLook = await _httpContextAccessor.UserCanLook(0, inventoryOut.InventoryOutHeader.StoreId);
				if (!userCanLook) return Ok(new InventoryOutDto());

				await _inventoryOutService.ModifyInventoryOutDetailsWithStoreIdAndAvaialbleBalance(inventoryOut.InventoryOutHeader.InventoryOutHeaderId, inventoryOut.InventoryOutHeader.StoreId, inventoryOut.InventoryOutDetails, true);

                return Ok(inventoryOut);
			}
			return Ok(new InventoryOutDto());
		}

		[HttpGet("{id:int}")]
        public async Task<IActionResult> GetInventoryOut(int id)
        {
            var inventoryOut = await _inventoryOutService.GetInventoryOut(id);
			var userCanLook = await _httpContextAccessor.UserCanLook(0, inventoryOut.InventoryOutHeader?.StoreId ?? 0);
            return Ok(userCanLook ? inventoryOut : new InventoryOutDto());
        }

        [HttpGet]
        [Route("GetInventoryOutDetail")]
        public async Task<IActionResult> GetInventoryOutDetail(int id)
        {
            var inventoryOut = await _inventoryOutService.GetInventoryOut(id);
            return Ok(inventoryOut.InventoryOutDetails);
        }

        [HttpPost]
        [Route("Save")]
        public async Task<IActionResult> Save([FromBody] InventoryOutDto model, int requestId)
        {
            var response = new ResponseDto();
            try
            {
                if (model.InventoryOutHeader != null)
                {
                    var hasApprove = await _approvalSystemService.IsMenuHasApprove(MenuCodeData.InventoryOut);
                    if (hasApprove.HasApprove)
                    {
                        if (hasApprove.OnAdd && model.InventoryOutHeader!.InventoryOutHeaderId == 0)
                        {
                            response = await SendApproveRequest(model, null, requestId, 0, false);
                        }
                        else if (hasApprove.OnEdit && model.InventoryOutHeader!.InventoryOutHeaderId > 0)
                        {
                            var oldValue = await _inventoryOutService.GetInventoryOut(model.InventoryOutHeader!.InventoryOutHeaderId);
                            response = await SendApproveRequest(model, oldValue, requestId, model.InventoryOutHeader!.InventoryOutHeaderId, false);
                        }
                        else
                        {
                            response = await SaveInventoryOut(model, hasApprove.HasApprove, false, requestId);
                        }
                    }
                    else
                    {
                        response = await SaveInventoryOut(model, hasApprove.HasApprove, false, requestId);
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
        [Route("SaveInventoryOut")]
        public async Task<ResponseDto> SaveInventoryOut(InventoryOutDto model, bool hasApprove, bool approved, int? requestId)
        {
            await _databaseTransaction.BeginTransaction();
            var response = await _inventoryOutService.SaveInventoryOut(model, hasApprove, approved, requestId);
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
        [Route("DeleteInventoryOut")]
        public async Task<IActionResult> DeleteInventoryOut(int inventoryOutHeaderId, int requestId)
        {
            var response = new ResponseDto();
            try
            {
                var model = await _inventoryOutService.GetInventoryOut(inventoryOutHeaderId);
                if (model.InventoryOutHeader != null)
                {
                    var hasApprove = await _approvalSystemService.IsMenuHasApprove(MenuCodeData.InventoryOut);
                    if (hasApprove.HasApprove && hasApprove.OnDelete)
                    {
                        response = await SendApproveRequest(null, model, requestId, inventoryOutHeaderId, true);
                    }
                    else
                    {
                        await _databaseTransaction.BeginTransaction();
                        response = await _inventoryOutService.DeleteInventoryOut(inventoryOutHeaderId);
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
                    response.Id = inventoryOutHeaderId;
                    response.Message = await _genericMessageService.GetMessage(MenuCodeData.InventoryOut, GenericMessageData.NotFound);
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
        public async Task<ResponseDto> SendApproveRequest([FromQuery] InventoryOutDto? newModel, [FromQuery] InventoryOutDto? oldModel, int requestId, int inventoryOutHeaderId, bool isDelete)
        {
            var changes = new List<RequestChangesDto>();
            var menuCode = MenuCodeData.InventoryOut;
            var requestTypeId = isDelete ? ApproveRequestTypeData.Delete : (inventoryOutHeaderId == 0 ? ApproveRequestTypeData.Add : ApproveRequestTypeData.Edit);
            if (requestTypeId == ApproveRequestTypeData.Edit && oldModel != null)
            {
                changes = _inventoryOutService.GetInventoryOutRequestChanges(oldModel, newModel!);
            }

            var request = new NewApproveRequestDto()
            {
                RequestId = requestId,
                MenuCode = (short)menuCode,
                ApproveRequestTypeId = requestTypeId,
                ReferenceId = inventoryOutHeaderId,
                ReferenceCode = inventoryOutHeaderId.ToString(),
                OldValue = oldModel,
                NewValue = newModel,
                Changes = changes
            };
            await _databaseTransaction.BeginTransaction();

            if (requestTypeId != ApproveRequestTypeData.Delete)
            {
                var validationResult = await _inventoryOutService.CheckInventoryZeroStock(newModel!.InventoryOutHeader!.StoreId, newModel.InventoryOutDetails, oldModel?.InventoryOutDetails ?? []);
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
