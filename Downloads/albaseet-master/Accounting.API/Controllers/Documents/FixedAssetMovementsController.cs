using Accounting.CoreOne.Contracts;
using Accounting.CoreOne.Models.Dtos.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Contracts.Approval;
using Shared.CoreOne;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Database;
using Shared.Helper.Models.Dtos;
using Accounting.Service.Services;
using Shared.Service.Services.Approval;
using Shared.Service;
using Shared.Helper.Identity;
using DevExtreme.AspNet.Data;
using Accounting.API.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Shared.Helper.Extensions;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne.Models.Dtos.ViewModels.FixedAssets;

namespace Accounting.API.Controllers.Documents
{
    [Route("api/[controller]")]
    [ApiController]
    public class FixedAssetMovementsController : ControllerBase
    {
        private readonly IFixedAssetMovementHeaderService _fixedAssetMovementHeaderService;
        private readonly IFixedAssetMovementService _fixedAssetMovementService;
        private readonly IHandleApprovalRequestService _handleApprovalRequestService;
        private readonly IApprovalSystemService _approvalSystemService;
        private readonly IDatabaseTransaction _databaseTransaction;
        private readonly IStringLocalizer<HandelException> _exLocalizer;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public FixedAssetMovementsController(
            IFixedAssetMovementHeaderService fixedAssetMovementHeaderService,
            IFixedAssetMovementService fixedAssetMovementService, 
            IHandleApprovalRequestService handleApprovalRequestService, 
            IApprovalSystemService approvalSystemService, 
            IDatabaseTransaction databaseTransaction, 
            IStringLocalizer<HandelException> exLocalizer, 
            IHttpContextAccessor httpContextAccessor)
        {
            _fixedAssetMovementHeaderService = fixedAssetMovementHeaderService;
            _fixedAssetMovementService = fixedAssetMovementService;
            _handleApprovalRequestService = handleApprovalRequestService;
            _approvalSystemService = approvalSystemService;
            _databaseTransaction = databaseTransaction;
            _exLocalizer = exLocalizer;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("GetAllFixedAssetMovements")]
        public async Task<ActionResult> GetAllFixedAssetMovements()
        {
            var data = await _fixedAssetMovementHeaderService.GetAllFixedAssetMovementHeaders().ToListAsync();
            return Ok(data);
        }

        [HttpGet]
        [Route("ReadFixedAssetMovements")]
        public async Task<IActionResult> ReadFixedAssetMovements(DataSourceLoadOptions loadOptions)
        {
            var data = await DataSourceLoader.LoadAsync(await _fixedAssetMovementHeaderService.GetUserFixedAssetMovementHeaders(), loadOptions);
            return Ok(data);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetFixedAssetMovement(int id)
        {
            var fixedAssetMovement = await _fixedAssetMovementService.GetFixedAssetMovement(id);
            var canLook = await _httpContextAccessor.UserCanLook(0, fixedAssetMovement.FixedAssetMovementHeader?.StoreId ?? 0);
            return Ok(canLook ? fixedAssetMovement : new FixedAssetMovementDto());
        }

        [HttpGet]
        [Route("GetRequestData")]
        public async Task<IActionResult> GetRequestData(int requestId)
        {
            var data = await _handleApprovalRequestService.GetRequestData(requestId);
            if (data != null)
            {
                if (data.CanBeConverted<FixedAssetMovementDto>())
                {
                    var convertedData = data.ConvertToType<FixedAssetMovementDto>();
                    var userCanLook = await _httpContextAccessor.UserCanLook(0, convertedData?.FixedAssetMovementHeader!.StoreId ?? 0);
                    return Ok(userCanLook ? convertedData : new FixedAssetMovementDto());
                }
            }
            return Ok(new FixedAssetMovementHeaderDto());
        }

        [HttpGet]
        [Route("GetNextFixedAssetMovementHeaderCodeByStoreId")]
        public async Task<ActionResult> GetNextFixedAssetMovementHeaderCodeByStoreId(int storeId)
        {
            var data = await _fixedAssetMovementHeaderService.GetNextFixedAssetMovementHeaderCodeByStoreId(storeId);
            return Ok(new { data = data });
        }
        [HttpPost]
        [Route("SaveFixedAssetMovement")]
        public async Task<IActionResult> SaveFixedAssetMovement([FromBody] FixedAssetMovementDto fixedAssetMovement, int requestId)
        {
            var result = new ResponseDto();
            await _databaseTransaction.BeginTransaction();
            result = await _fixedAssetMovementService.SaveFixedAssetMovement(fixedAssetMovement, true, true, requestId);
            if (result.Success)
            {
                await _databaseTransaction.Commit();
            }
            else
            {
                await _databaseTransaction.Rollback();
            }
            return Ok(result);
        }

        [HttpPost]
        [Route("SendAdditionApproveRequest")]
        public async Task<ResponseDto> SendAdditionApproveRequest([FromQuery] FixedAssetMovementDto? newModel, [FromQuery] FixedAssetMovementDto? oldModel, int requestId, int fixedAssetMovementHeaderId, bool isDelete)
        {
            var changes = new List<RequestChangesDto>();
            var requestTypeId = isDelete ? ApproveRequestTypeData.Delete : (fixedAssetMovementHeaderId == 0 ? ApproveRequestTypeData.Add : ApproveRequestTypeData.Edit);
            if (requestTypeId == ApproveRequestTypeData.Edit)
            {
                changes = _fixedAssetMovementService.GetFixedAssetMovementRequestChanges(oldModel!, newModel!);
            }

            var request = new NewApproveRequestDto()
            {
                RequestId = requestId,
                MenuCode = MenuCodeData.FixedAssetAddition,
                ApproveRequestTypeId = requestTypeId,
                ReferenceId = fixedAssetMovementHeaderId,
                ReferenceCode = fixedAssetMovementHeaderId.ToString(),
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

        [HttpPost]
        [Route("SendDepreciationApproveRequest")]
        public async Task<ResponseDto> SendDepreciationApproveRequest([FromQuery] FixedAssetMovementDto? newModel, [FromQuery] FixedAssetMovementDto? oldModel, int requestId, int fixedAssetMovementHeaderId, bool isDelete)
        {
            var changes = new List<RequestChangesDto>();
            var requestTypeId = isDelete ? ApproveRequestTypeData.Delete : (fixedAssetMovementHeaderId == 0 ? ApproveRequestTypeData.Add : ApproveRequestTypeData.Edit);
            if (requestTypeId == ApproveRequestTypeData.Edit)
            {
                changes = _fixedAssetMovementService.GetFixedAssetMovementRequestChanges(oldModel!, newModel!);
            }

            var request = new NewApproveRequestDto()
            {
                RequestId = requestId,
                MenuCode = MenuCodeData.FixedAssetDepreciation,
                ApproveRequestTypeId = requestTypeId,
                ReferenceId = fixedAssetMovementHeaderId,
                ReferenceCode = fixedAssetMovementHeaderId.ToString(),
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

        [HttpPost]
        [Route("SendExclusionApproveRequest")]
        public async Task<ResponseDto> SendExclusionApproveRequest([FromQuery] FixedAssetMovementDto? newModel, [FromQuery] FixedAssetMovementDto? oldModel, int requestId, int fixedAssetMovementHeaderId, bool isDelete)
        {
            var changes = new List<RequestChangesDto>();
            var requestTypeId = isDelete ? ApproveRequestTypeData.Delete : (fixedAssetMovementHeaderId == 0 ? ApproveRequestTypeData.Add : ApproveRequestTypeData.Edit);
            if (requestTypeId == ApproveRequestTypeData.Edit)
            {
                changes = _fixedAssetMovementService.GetFixedAssetMovementRequestChanges(oldModel!, newModel!);
            }

            var request = new NewApproveRequestDto()
            {
                RequestId = requestId,
                MenuCode = MenuCodeData.FixedAssetExclusion,
                ApproveRequestTypeId = requestTypeId,
                ReferenceId = fixedAssetMovementHeaderId,
                ReferenceCode = fixedAssetMovementHeaderId.ToString(),
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

        [HttpPost]
        [Route("ApproveFixedAssetMovement")]
        public async Task<ResponseDto> ApproveFixedAssetMovement(ApproveResponseDto request)
        {
            var result = new ResponseDto();
            var data = request.UserRequest;
            if (data != null)
            {
                if (data.CanBeConverted<FixedAssetMovementDto>())
                {
                    var sendModel = data.ConvertToType<FixedAssetMovementDto>();
                    if (sendModel != null)
                    {
                        try
                        {
                            if (request.ApproveRequestTypeId == ApproveRequestTypeData.Delete)
                            {
                                await _databaseTransaction.BeginTransaction();
                                result = await _fixedAssetMovementService.DeleteFixedAssetMovement(sendModel.FixedAssetMovementHeader!.FixedAssetMovementHeaderId);
                                if (result.Success)
                                {
                                    await _databaseTransaction.Commit();
                                }
                                else
                                {
                                    await _databaseTransaction.Rollback();
                                }
                            }
                            else
                            {
                                await _databaseTransaction.BeginTransaction();
                                result = await _fixedAssetMovementService.SaveFixedAssetMovement(sendModel, true, true, request.RequestId);
                                if (result.Success)
                                {
                                    await _databaseTransaction.Commit();
                                }
                                else
                                {
                                    await _databaseTransaction.Rollback();
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            await _databaseTransaction.Rollback();
                            var handleException = new HandelException(_exLocalizer);
                            result = handleException.Handle(e);
                        }
                    }
                }
            }
            return result;
        }

    }
}
