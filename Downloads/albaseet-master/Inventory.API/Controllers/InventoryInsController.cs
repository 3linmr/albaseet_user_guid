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
using Inventory.CoreOne.Models.Domain;
using Shared.CoreOne.Contracts.Menus;
using Shared.Helper.Identity;

namespace Inventory.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InventoryInsController : ControllerBase
    {
        private readonly IDatabaseTransaction _databaseTransaction;
        private readonly IStringLocalizer<HandelException> _exLocalizer;
        private readonly IInventoryInHeaderService _inventoryInHeaderService;
        private readonly IInventoryInService _inventoryInService;
        private readonly IHandleApprovalRequestService _handleApprovalRequestService;
        private readonly IApprovalSystemService _approvalSystemService;
        private readonly IGenericMessageService _genericMessageService;
		private readonly IHttpContextAccessor _httpContextAccessor;

        public InventoryInsController(IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exLocalizer, IHandleApprovalRequestService handleApprovalRequestService, IApprovalSystemService approvalSystemService, IInventoryInHeaderService inventoryInHeaderService, IInventoryInService inventoryInService, IGenericMessageService genericMessageService, IHttpContextAccessor httpContextAccessor)
        {
            _databaseTransaction = databaseTransaction;
            _exLocalizer = exLocalizer;
            _inventoryInHeaderService = inventoryInHeaderService;
            _inventoryInService = inventoryInService;
            _handleApprovalRequestService = handleApprovalRequestService;
            _approvalSystemService = approvalSystemService;
            _genericMessageService = genericMessageService;
			_httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("ReadInventoryIns")]
        public async Task<IActionResult> ReadInventoryIns(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var data = await DataSourceLoader.LoadAsync(_inventoryInHeaderService.GetUserInventoryInHeaders(), loadOptions);
                return Ok(data);
            }
            catch
            {
                var model = _inventoryInHeaderService.GetUserInventoryInHeaders();
                var data = DataSourceLoader.Load(model.ToList(), loadOptions);
                return Ok(data);
            }
        }

        [HttpGet]
        [Route("GetInventoryInCode")]
        public async Task<IActionResult> GetInventoryInCode(int storeId, DateTime stockDate)
        {
            var data = await _inventoryInHeaderService.GetInventoryInCode(storeId, stockDate);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetRequestData")]
        public async Task<IActionResult> GetRequestData(int requestId)
        {
            var data = await _handleApprovalRequestService.GetRequestData(requestId);
            if (data != null)
            {
                if (data.CanBeConverted<InventoryInDto>())
                {
					var inventoryIn = data.ConvertToType<InventoryInDto>();
					var userCanLook = await _httpContextAccessor.UserCanLook(0, inventoryIn?.InventoryInHeader?.StoreId ?? 0);
                    return Ok(userCanLook ? inventoryIn : new InventoryInDto());
                }
            }
            return Ok(new InventoryInDto());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetInventoryIn(int id)
        {
            var inventoryIn = await _inventoryInService.GetInventoryIn(id);
			var userCanLook = await _httpContextAccessor.UserCanLook(0, inventoryIn.InventoryInHeader?.StoreId ?? 0);
            return Ok(userCanLook ? inventoryIn : new InventoryInDto());
        }

        [HttpGet]
        [Route("GetInventoryInDetail")]
        public async Task<IActionResult> GetInventoryInDetail(int id)
        {
            var inventoryIn = await _inventoryInService.GetInventoryIn(id);
            return Ok(inventoryIn.InventoryInDetails);
        }

        [HttpPost]
        [Route("Save")]
        public async Task<IActionResult> Save([FromBody] InventoryInDto model, int requestId)
        {
            var response = new ResponseDto();
            try
            {
                if (model.InventoryInHeader != null)
                {
                    var hasApprove = await _approvalSystemService.IsMenuHasApprove(MenuCodeData.InventoryIn);
                    if (hasApprove.HasApprove)
                    {
                        if (hasApprove.OnAdd && model.InventoryInHeader!.InventoryInHeaderId == 0)
                        {
                            response = await SendApproveRequest(model, null, requestId, 0, false);
                        }
                        else if (hasApprove.OnEdit && model.InventoryInHeader!.InventoryInHeaderId > 0)
                        {
                            var oldValue = await _inventoryInService.GetInventoryIn(model.InventoryInHeader!.InventoryInHeaderId);
                            response = await SendApproveRequest(model, oldValue, requestId, model.InventoryInHeader!.InventoryInHeaderId, false);
                        }
                        else
                        {
                            response = await SaveInventoryIn(model, hasApprove.HasApprove, false, requestId);
                        }
                    }
                    else
                    {
                        response = await SaveInventoryIn(model, hasApprove.HasApprove, false, requestId);
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
        [Route("SaveInventoryIn")]
        public async Task<ResponseDto> SaveInventoryIn(InventoryInDto model, bool hasApprove, bool approved, int? requestId)
        {
            await _databaseTransaction.BeginTransaction();
            var response = await _inventoryInService.SaveInventoryIn(model, hasApprove, approved, requestId);
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
        [Route("DeleteInventoryIn")]
        public async Task<IActionResult> DeleteInventoryIn(int inventoryInHeaderId, int requestId)
        {
            var response = new ResponseDto();
            try
            {
                var model = await _inventoryInService.GetInventoryIn(inventoryInHeaderId);
                if (model.InventoryInHeader != null)
                {
                    var hasApprove = await _approvalSystemService.IsMenuHasApprove(MenuCodeData.InventoryIn);
                    if (hasApprove.HasApprove && hasApprove.OnDelete)
                    {
                        response = await SendApproveRequest(null, model, requestId, inventoryInHeaderId, true);
                    }
                    else
                    {
                        await _databaseTransaction.BeginTransaction();
                        response = await _inventoryInService.DeleteInventoryIn(inventoryInHeaderId);
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
                    response.Id = inventoryInHeaderId;
                    response.Message = await _genericMessageService.GetMessage(MenuCodeData.InventoryIn, GenericMessageData.NotFound);
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
        public async Task<ResponseDto> SendApproveRequest([FromQuery] InventoryInDto? newModel, [FromQuery] InventoryInDto? oldModel, int requestId, int inventoryInHeaderId, bool isDelete)
        {
            var changes = new List<RequestChangesDto>();
            var menuCode = MenuCodeData.InventoryIn;
            var requestTypeId = isDelete ? ApproveRequestTypeData.Delete : (inventoryInHeaderId == 0 ? ApproveRequestTypeData.Add : ApproveRequestTypeData.Edit);
            if (requestTypeId == ApproveRequestTypeData.Edit && oldModel != null)
            {
                changes = _inventoryInService.GetInventoryInRequestChanges(oldModel, newModel!);
            }

            var request = new NewApproveRequestDto()
            {
                RequestId = requestId,
                MenuCode = (short)menuCode,
                ApproveRequestTypeId = requestTypeId,
                ReferenceId = inventoryInHeaderId,
                ReferenceCode = inventoryInHeaderId.ToString(),
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
