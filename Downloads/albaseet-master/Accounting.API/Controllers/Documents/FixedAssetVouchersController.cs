using Accounting.API.Models;
using Accounting.CoreOne.Contracts;
using Accounting.CoreOne.Models.Domain;
using Accounting.CoreOne.Models.Dtos.ViewModels;
using Accounting.Service.Services;
using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Approval;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Database;
using Shared.Helper.Extensions;
using Shared.Helper.Models.Dtos;
using Shared.Helper.Identity;
using Shared.CoreOne.Models.Dtos.ViewModels.FixedAssets;
using Shared.Helper.Models.UserDetail;
using Microsoft.EntityFrameworkCore;

namespace Accounting.API.Controllers.Documents
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FixedAssetVouchersController : ControllerBase
    {
        private readonly IFixedAssetVoucherHeaderService _fixedAssetVoucherHeaderService;
        private readonly IFixedAssetVoucherDetailService _fixedAssetVoucherDetailService;
        private readonly IFixedAssetVoucherDetailPaymentService _fixedAssetVoucherDetailPaymentService;
        private readonly IFixedAssetVoucherService _fixedAssetVoucherService;
        private readonly IHandleApprovalRequestService _handleApprovalRequestService;
        private readonly IApprovalSystemService _approvalSystemService;
        private readonly IDatabaseTransaction _databaseTransaction;
        private readonly IStringLocalizer<HandelException> _exLocalizer;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FixedAssetVouchersController(
            IFixedAssetVoucherHeaderService fixedAssetVoucherHeaderService,
            IFixedAssetVoucherDetailService fixedAssetVoucherDetailService,
            IFixedAssetVoucherDetailPaymentService fixedAssetVoucherDetailPaymentService,
            IFixedAssetVoucherService fixedAssetVoucherService, 
            IHandleApprovalRequestService handleApprovalRequestService, 
            IApprovalSystemService approvalSystemService, 
            IDatabaseTransaction databaseTransaction, 
            IStringLocalizer<HandelException> exLocalizer, 
            IHttpContextAccessor httpContextAccessor)
        {
            _fixedAssetVoucherHeaderService = fixedAssetVoucherHeaderService;
            _fixedAssetVoucherDetailService = fixedAssetVoucherDetailService;
            _fixedAssetVoucherDetailPaymentService = fixedAssetVoucherDetailPaymentService;
            _fixedAssetVoucherService = fixedAssetVoucherService;
            _handleApprovalRequestService = handleApprovalRequestService;
            _approvalSystemService = approvalSystemService;
            _databaseTransaction = databaseTransaction;
            _exLocalizer = exLocalizer;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("ReadFixedAssetVouchers")]
        public async Task<IActionResult> ReadFixedAssetVouchers(DataSourceLoadOptions loadOptions)
        {
            var data = await DataSourceLoader.LoadAsync(await _fixedAssetVoucherHeaderService.GetUserFixedAssetVoucherHeaders(), loadOptions);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetRequestData")]
        public async Task<IActionResult> GetRequestData(int requestId)
        {
            var data = await _handleApprovalRequestService.GetRequestData(requestId);
            if (data != null)
            {
                if (data.CanBeConverted<FixedAssetVoucherDto>())
                {
                    var fixedAssetVoucher = data.ConvertToType<FixedAssetVoucherDto>();
                    var userCanLook = await _httpContextAccessor.UserCanLook(0, fixedAssetVoucher?.FixedAssetVoucherHeader?.StoreId ?? 0);
                    return Ok(userCanLook ? fixedAssetVoucher : new FixedAssetVoucherDto());
                }
            }
            return Ok(new FixedAssetVoucherDto());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetFixedAssetVoucher(int id)
        {
            var fixedAssetVoucher = await _fixedAssetVoucherService.GetFixedAssetVoucher(id);
            var canLook = await _httpContextAccessor.UserCanLook(0, fixedAssetVoucher.FixedAssetVoucherHeader?.StoreId ?? 0);
            return Ok(canLook ? fixedAssetVoucher : new FixedAssetVoucherDto());
        }
        [HttpGet]
        [Route("GetNextFixedAssetAdditionCodeByStoreId")]
        public async Task<ActionResult> GetNextFixedAssetAdditionCodeByStoreId(int storeId)
        {
            var data = await _fixedAssetVoucherHeaderService.GetNextFixedAssetAdditionCodeByStoreId(storeId);
            return Ok(new { data = data });
        }
        [HttpGet]
        [Route("GetNextFixedAssetExclusionCodeByStoreId")]
        public async Task<ActionResult> GetNextFixedAssetExclusionCodeByStoreId(int storeId)
        {
            var data = await _fixedAssetVoucherHeaderService.GetNextFixedAssetExclusionCodeByStoreId(storeId);
            return Ok(new { data = data });
        }
        [HttpGet]
        [Route("GetFixedAssetAdditionRequestData")]
        public async Task<IActionResult> GetFixedAssetAdditionRequestData(int requestId)
        {
            var data = await _handleApprovalRequestService.GetRequestData(requestId);
            if (data != null)
            {
                if (data.CanBeConverted<FixedAssetAdditionDto>())
                {
                    var convertedData = data.ConvertToType<FixedAssetAdditionDto>();
                    var userCanLook = await _httpContextAccessor.UserCanLook(0, convertedData?.StoreId ?? 0);
                    return Ok(userCanLook ? convertedData : new FixedAssetAdditionDto());
                }
            }
            return Ok(new FixedAssetAdditionDto());
        }
        [HttpGet]
        [Route("GetFixedAssetExclusionRequestData")]
        public async Task<IActionResult> GetFixedAssetExclusionRequestData(int requestId)
        {
            var data = await _handleApprovalRequestService.GetRequestData(requestId);
            if (data != null)
            {
                if (data.CanBeConverted<FixedAssetExclusionDto>())
                {
                    var convertedData = data.ConvertToType<FixedAssetExclusionDto>();
                    var userCanLook = await _httpContextAccessor.UserCanLook(0, convertedData?.StoreId ?? 0);
                    return Ok(userCanLook ? convertedData : new FixedAssetExclusionDto());
                }
            }
            return Ok(new FixedAssetExclusionDto());
        }

        [HttpGet]
        [Route("GetFixedAssetAdditionById")]
        public async Task<IActionResult> GetFixedAssetAdditionById(int id, int storeId)
        {
            var fixedAssetVoucherDetails = await _fixedAssetVoucherDetailService.GetFixedAssetVoucherDetail(id);
            var canLook = await _httpContextAccessor.UserCanLook(0, storeId);
            return Ok(canLook ? 
                fixedAssetVoucherDetails!.Select(x => 
                new FixedAssetAdditionDto() 
                { 
                    AdditionDate = x.DocumentDate, 
                    AdditionValue = x.DetailValue,
                    DocumentCode = x.DocumentCode,
                    FixedAssetId = x.FixedAssetId,
                    FixedAssetName = x.FixedAssetName,
                    FixedAssetVoucherHeaderId = x.FixedAssetVoucherHeaderId,
                    Prefix = x.Prefix,
                    StoreId = x.StoreId,
                    Suffix = x.Suffix,
                    FixedAssetVoucherDetailPayments = x.FixedAssetVoucherDetailPayments
                }).FirstOrDefault() : new FixedAssetAdditionDto());
        }
        [HttpGet]
        [Route("GetFixedAssetExclusionById")]
        public async Task<IActionResult> GetFixedAssetExclusionById(int id, int storeId)
        {
            var fixedAssetVoucherDetails = await _fixedAssetVoucherDetailService.GetFixedAssetVoucherDetail(id);
            var canLook = await _httpContextAccessor.UserCanLook(0, storeId);
            return Ok(canLook ?
                fixedAssetVoucherDetails!.Select(x =>
                new FixedAssetExclusionDto()
                {
                    ExclusionDate = x.DocumentDate,
                    ExclusionToValue = x.DetailValue,
                    DocumentCode = x.DocumentCode,
                    FixedAssetId = x.FixedAssetId,
                    FixedAssetName = x.FixedAssetName,
                    FixedAssetVoucherHeaderId = x.FixedAssetVoucherHeaderId,
                    Prefix = x.Prefix,
                    StoreId = x.StoreId,
                    Suffix = x.Suffix,
                    FixedAssetVoucherDetailPayments = x.FixedAssetVoucherDetailPayments
                }).FirstOrDefault() : new FixedAssetExclusionDto());
        }

        [HttpGet]
        [Route("GetAllFixedAssetAdditions")]
        public async Task<IActionResult> GetAllFixedAssetAdditions(int storeId)
        {
            var fixedAssetVoucherDetails = await _fixedAssetVoucherDetailService.GetAllFixedAssetVoucherDetail().Where(x => 
            x.StoreId == storeId &&
            x.FixedAssetVoucherTypeId == FixedAssetVoucherTypeData.Addition
            ).ToListAsync();
            var canLook = await _httpContextAccessor.UserCanLook(0, storeId);
            return Ok(canLook ?
                fixedAssetVoucherDetails!.Select(x =>
                new FixedAssetAdditionDto()
                {
                    AdditionDate = x.DocumentDate,
                    AdditionValue = x.DetailValue,
                    DocumentCode = x.DocumentCode,
                    FixedAssetId = x.FixedAssetId,
                    FixedAssetName = x.FixedAssetName,
                    FixedAssetVoucherHeaderId = x.FixedAssetVoucherHeaderId,
                    Prefix = x.Prefix,
                    StoreId = x.StoreId,
                    Suffix = x.Suffix,
                    FixedAssetVoucherDetailPayments = x.FixedAssetVoucherDetailPayments
                }).ToList() : new List<FixedAssetAdditionDto>());
        }
        [HttpGet]
        [Route("GetAllFixedAssetDepreciations")]
        public async Task<IActionResult> GetAllFixedAssetDepreciations(int storeId)
        {
            var fixedAssetVoucherDetails = await _fixedAssetVoucherDetailService.GetAllFixedAssetVoucherDetail().Where(x =>
            x.StoreId == storeId &&
            x.FixedAssetVoucherTypeId == FixedAssetVoucherTypeData.Depreciation
            ).ToListAsync();
            var canLook = await _httpContextAccessor.UserCanLook(0, storeId);
            return Ok(canLook ?
                fixedAssetVoucherDetails!.GroupBy(x => x.DocumentCode).Select(x =>
                new FixedAssetDepreciationDto()
                {
                    DepreciationDate = x.Max(d => d.DocumentDate),
                    DepreciationValue = x.Sum(d => d.DetailValue),
                    DocumentCode = x.Key,
                    FixedAssetVoucherHeaderId = x.Max(d => d.FixedAssetVoucherHeaderId),
                    Prefix = x.Max(d => d.Prefix),
                    StoreId = x.Max(d => d.StoreId),
                    Suffix = x.Max(d => d.Suffix)
                }).ToList() : new List<FixedAssetDepreciationDto>());
        }
        [HttpGet]
        [Route("GetAllFixedAssetExclusions")]
        public async Task<IActionResult> GetAllFixedAssetExclusions(int storeId)
        {
            var fixedAssetVoucherDetails = await _fixedAssetVoucherDetailService.GetAllFixedAssetVoucherDetail().Where(x =>
            x.StoreId == storeId &&
            x.FixedAssetVoucherTypeId == FixedAssetVoucherTypeData.Exclusion
            ).ToListAsync();
            var canLook = await _httpContextAccessor.UserCanLook(0, storeId);
            return Ok(canLook ?
                fixedAssetVoucherDetails!.Select(x =>
                new FixedAssetExclusionDto()
                {
                    ExclusionDate = x.DocumentDate,
                    ExclusionToValue = x.DetailValue,
                    DocumentCode = x.DocumentCode,
                    FixedAssetId = x.FixedAssetId,
                    FixedAssetName = x.FixedAssetName,
                    FixedAssetVoucherHeaderId = x.FixedAssetVoucherHeaderId,
                    Prefix = x.Prefix,
                    StoreId = x.StoreId,
                    Suffix = x.Suffix,
                    FixedAssetVoucherDetailPayments = x.FixedAssetVoucherDetailPayments
                }).ToList() : new List<FixedAssetExclusionDto>());
        }
        [HttpPost]
        [Route("SaveFixedAssetAdditionVoucher")]
        public async Task<ResponseDto> SaveFixedAssetAdditionVoucher(FixedAssetAdditionDto model, bool hasApprove, bool approved, int? requestId)
        {
            await _databaseTransaction.BeginTransaction();
            var response = await _fixedAssetVoucherService.SaveFixedAssetAdditionVoucher(model, hasApprove, approved, requestId);
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
        [Route("SaveAllFixedAssetDepreciationVoucher")]
        public async Task<ResponseDto> SaveAllFixedAssetDepreciationVoucher(FixedAssetDepreciationDto depreciationVoucher, bool hasApprove, bool approved, int? requestId)
        {
            await _databaseTransaction.BeginTransaction();
            var response = await _fixedAssetVoucherService.SaveAllFixedAssetDepreciationVoucher(depreciationVoucher.DepreciationDate ?? DateTime.UtcNow, depreciationVoucher.StoreId, hasApprove, approved, requestId);
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
        [Route("SaveFixedAssetExclusionVoucher")]
        public async Task<ResponseDto> SaveFixedAssetExclusionVoucher(FixedAssetExclusionDto model, bool hasApprove, bool approved, int? requestId)
        {
            await _databaseTransaction.BeginTransaction();
            var response = await _fixedAssetVoucherService.SaveFixedAssetExclusionVoucher(model, hasApprove, approved, requestId);
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
        [Route("SendAdditionApproveRequest")]
        public async Task<ResponseDto> SendAdditionApproveRequest([FromQuery] FixedAssetVoucherDto? newModel, [FromQuery] FixedAssetVoucherDto? oldModel, int requestId, int fixedAssetVoucherHeaderId, bool isDelete)
        {
            var changes = new List<RequestChangesDto>();
            var requestTypeId = isDelete ? ApproveRequestTypeData.Delete : (fixedAssetVoucherHeaderId == 0 ? ApproveRequestTypeData.Add : ApproveRequestTypeData.Edit);
            if (requestTypeId == ApproveRequestTypeData.Edit)
            {
                changes = _fixedAssetVoucherService.GetFixedAssetVoucherRequestChanges(oldModel!, newModel!);
            }

            var request = new NewApproveRequestDto()
            {
                RequestId = requestId,
                MenuCode = MenuCodeData.FixedAssetAddition,
                ApproveRequestTypeId = requestTypeId,
                ReferenceId = fixedAssetVoucherHeaderId,
                ReferenceCode = fixedAssetVoucherHeaderId.ToString(),
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
        public async Task<ResponseDto> SendDepreciationApproveRequest([FromQuery] FixedAssetVoucherDto? newModel, [FromQuery] FixedAssetVoucherDto? oldModel, int requestId, int fixedAssetVoucherHeaderId, bool isDelete)
        {
            var changes = new List<RequestChangesDto>();
            var requestTypeId = isDelete ? ApproveRequestTypeData.Delete : (fixedAssetVoucherHeaderId == 0 ? ApproveRequestTypeData.Add : ApproveRequestTypeData.Edit);
            if (requestTypeId == ApproveRequestTypeData.Edit)
            {
                changes = _fixedAssetVoucherService.GetFixedAssetVoucherRequestChanges(oldModel!, newModel!);
            }

            var request = new NewApproveRequestDto()
            {
                RequestId = requestId,
                MenuCode = MenuCodeData.FixedAssetDepreciation,
                ApproveRequestTypeId = requestTypeId,
                ReferenceId = fixedAssetVoucherHeaderId,
                ReferenceCode = fixedAssetVoucherHeaderId.ToString(),
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
        public async Task<ResponseDto> SendExclusionApproveRequest([FromQuery] FixedAssetVoucherDto? newModel, [FromQuery] FixedAssetVoucherDto? oldModel, int requestId, int fixedAssetVoucherHeaderId, bool isDelete)
        {
            var changes = new List<RequestChangesDto>();
            var requestTypeId = isDelete ? ApproveRequestTypeData.Delete : (fixedAssetVoucherHeaderId == 0 ? ApproveRequestTypeData.Add : ApproveRequestTypeData.Edit);
            if (requestTypeId == ApproveRequestTypeData.Edit)
            {
                changes = _fixedAssetVoucherService.GetFixedAssetVoucherRequestChanges(oldModel!, newModel!);
            }

            var request = new NewApproveRequestDto()
            {
                RequestId = requestId,
                MenuCode = MenuCodeData.FixedAssetExclusion,
                ApproveRequestTypeId = requestTypeId,
                ReferenceId = fixedAssetVoucherHeaderId,
                ReferenceCode = fixedAssetVoucherHeaderId.ToString(),
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
        [Route("ApproveFixedAssetVoucher")]
        public async Task<ResponseDto> ApproveFixedAssetVoucher(ApproveResponseDto request)
        {
            var result = new ResponseDto();
            var data = request.UserRequest;
            if (data != null)
            {
                if (data.CanBeConverted<FixedAssetVoucherDto>())
                {
                    var sendModel = data.ConvertToType<FixedAssetVoucherDto>();
                    if (sendModel != null)
                    {
                        try
                        {
                            if (request.ApproveRequestTypeId == ApproveRequestTypeData.Delete)
                            {
                                await _databaseTransaction.BeginTransaction();
                                result = await _fixedAssetVoucherService.DeleteFixedAssetVoucher(sendModel.FixedAssetVoucherHeader!.FixedAssetVoucherHeaderId ?? 0);
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
                                result = await _fixedAssetVoucherService.SaveFixedAssetVoucher(sendModel, true, true, request.RequestId);
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

        [HttpDelete]
        [Route("DeleteFixedAssetVoucher")]
        public async Task<IActionResult> DeleteFixedAssetVoucher(int fixedAssetVoucherHeaderId, int requestId)
        {
            ResponseDto response;
            try
            {
                var hasApprove = await _approvalSystemService.IsMenuHasApprove(MenuCodeData.PaymentVoucher);
                if (hasApprove.HasApprove && hasApprove.OnDelete)
                {
                    var model = await _fixedAssetVoucherService.GetFixedAssetVoucher(fixedAssetVoucherHeaderId);
                    response = await SendApproveRequest(null, model, requestId, fixedAssetVoucherHeaderId, true);
                }
                else
                {
                    await _databaseTransaction.BeginTransaction();
                    response = await _fixedAssetVoucherService.DeleteFixedAssetVoucher(fixedAssetVoucherHeaderId);
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
        public async Task<ResponseDto> SendApproveRequest([FromQuery] FixedAssetVoucherDto? newModel, [FromQuery] FixedAssetVoucherDto? oldModel, int requestId, int fixedAssetVoucherHeaderId, bool isDelete)
        {
            var changes = new List<RequestChangesDto>();
            var requestTypeId = isDelete ? ApproveRequestTypeData.Delete : (fixedAssetVoucherHeaderId == 0 ? ApproveRequestTypeData.Add : ApproveRequestTypeData.Edit);
            if (requestTypeId == ApproveRequestTypeData.Edit)
            {
                changes = _fixedAssetVoucherService.GetFixedAssetVoucherRequestChanges(oldModel!, newModel!);
            }

            var request = new NewApproveRequestDto()
            {
                RequestId = requestId,
                MenuCode = MenuCodeData.PaymentVoucher,
                ApproveRequestTypeId = requestTypeId,
                ReferenceId = fixedAssetVoucherHeaderId,
                ReferenceCode = fixedAssetVoucherHeaderId.ToString(),
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