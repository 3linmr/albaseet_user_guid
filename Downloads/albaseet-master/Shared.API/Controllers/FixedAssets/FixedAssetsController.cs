using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Approval;
using Shared.CoreOne.Contracts.FixedAssets;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.CoreOne.Models.Dtos.ViewModels.FixedAssets;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Database;
using Shared.Helper.Models.Dtos;
using Shared.Helper.Identity;
using Shared.CoreOne.Models.Dtos.ViewModels.CostCenters;
using Shared.Service.Services.FixedAssets;
using Shared.Helper.Extensions;
using DevExtreme.AspNet.Data;
using Shared.API.Models;
using Shared.CoreOne.Models.Domain.FixedAssets;
using Compound.CoreOne.Models.Dtos.ViewModels;

namespace Shared.API.Controllers.FixedAssets
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FixedAssetsController : ControllerBase
    {
        private readonly IDatabaseTransaction _databaseTransaction;
        private readonly IStringLocalizer<HandelException> _exLocalizer;
        private readonly IFixedAssetService _fixedAssetService;
        private readonly IHandleApprovalRequestService _handleApprovalRequestService;
        private readonly IApprovalSystemService _approvalSystemService;
		private readonly IHttpContextAccessor _httpContextAccessor;

        public FixedAssetsController(IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exLocalizer, IFixedAssetService fixedAssetService, IHandleApprovalRequestService handleApprovalRequestService, IApprovalSystemService approvalSystemService, IHttpContextAccessor httpContextAccessor)
        {
            _databaseTransaction = databaseTransaction;
            _exLocalizer = exLocalizer;
            _fixedAssetService = fixedAssetService;
            _handleApprovalRequestService = handleApprovalRequestService;
            _approvalSystemService = approvalSystemService;
			_httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("GetAllFixedAssets")]
        public async Task<ActionResult> GetAllFixedAssets()
        {
            var data = await _fixedAssetService.GetAllFixedAssets().ToListAsync();
            return Ok(data);
        }
        [HttpGet]
        [Route("GetFixedAssetsDropDown")]
        public async Task<ActionResult<IEnumerable<FixedAssetDropDownDto>>> GetFixedAssetsDropDown(int companyId)
        {
            var fixedAssets = await _fixedAssetService.GetFixedAssetsDropDownByCompanyId(companyId);
            return Ok(fixedAssets);
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetFixedAssetByFixedAssetId(int id)
        {
            var countryDb = await _fixedAssetService.GetFixedAssetByFixedAssetId(id);
			var userCanLook = await _httpContextAccessor.UserCanLook(countryDb.CompanyId, 0);
            return Ok(userCanLook ? countryDb : new FixedAssetDto());
        }

        [HttpGet]
        [Route("GetMainFixedAssetsByFixedAssetCodeAutoComplete")]
        public async Task<ActionResult<IEnumerable<FixedAssetAutoCompleteDto>>> GetMainFixedAssetsByFixedAssetCodeAutoComplete(int companyId, string fixedAssetCode)
        {
            var data = await _fixedAssetService.GetMainFixedAssetsByFixedAssetCode(companyId, fixedAssetCode);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetMainFixedAssetsByFixedAssetNameAutoComplete")]
        public async Task<ActionResult<IEnumerable<FixedAssetAutoCompleteDto>>> GetMainFixedAssetsByFixedAssetNameAutoComplete(int companyId, string fixedAssetName)
        {
            var data = await _fixedAssetService.GetMainFixedAssetsByFixedAssetName(companyId, fixedAssetName);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetNextFixedAssetCode")]
        public async Task<ActionResult> GetNextFixedAssetCode(int companyId, int mainFixedAssetId, bool isMainFixedAsset)
        {
            var data = await _fixedAssetService.GetNextFixedAssetCode(companyId, mainFixedAssetId, isMainFixedAsset);
            return Ok(new { data = data });
        }

        [HttpGet]
        [Route("GetRequestData")]
        public async Task<IActionResult> GetRequestData(int requestId)
        {
            var data = await _handleApprovalRequestService.GetRequestData(requestId);
            if (data != null)
            {
                if (data.CanBeConverted<FixedAssetDto>())
                {
                    var convertedData = data.ConvertToType<FixedAssetDto>();
                    var userCanLook = await _httpContextAccessor.UserCanLook(convertedData?.CompanyId ?? 0, 0);
                    return Ok(userCanLook ? convertedData : new FixedAssetDto());
                }
            }
            return Ok(new FixedAssetDto());
        }

        [HttpGet]
        [Route("ReadFixedAssetsTree")]
        public IActionResult ReadFixedAssetsTree(DataSourceLoadOptions loadOptions, int companyId)
        {
            var data = DataSourceLoader.Load(_fixedAssetService.GetFixedAssetsTree(companyId), loadOptions);
            return Ok(data);
        }

        [HttpGet]
        [Route("ReadFixedAssetsAutoComplete")]
        public IActionResult ReadFixedAssetsAutoComplete(DataSourceLoadOptions loadOptions, int companyId)
        {
            var data = DataSourceLoader.Load(_fixedAssetService.GetAllFixedAssets().Where(x => x.CompanyId == companyId), loadOptions);
            return Ok(data);
        }
        [HttpGet]
        [Route("GetFixedAssetTreeName")]
        public async Task<IActionResult> GetFixedAssetTreeName(int fixedAssetId)
        {
            var data = await _fixedAssetService.GetFixedAssetTreeName(fixedAssetId);
            return Ok(new { data = data });
        }

        [HttpGet]
        [Route("GetFixedAssetSearchByFixedAssetId")]
        public async Task<ActionResult<FixedAssetAutoCompleteDto>> GetFixedAssetSearchByFixedAssetId(int fixedAssetId, int companyId)
        {
            var data = await _fixedAssetService.GetMainFixedAssetsByFixedAssetId(companyId, fixedAssetId);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetFixedAssetSearchByFixedAssetCode")]
        public async Task<ActionResult<FixedAssetAutoCompleteDto>> GetFixedAssetSearchByFixedAssetCode(string? fixedAssetCode, int companyId)
        {
            var data = await _fixedAssetService.GetMainFixedAssetsByFixedAssetCode(companyId, fixedAssetCode ?? "");
            return Ok(data);
        }

        [HttpPost]
        [Route("Save")]
        public async Task<IActionResult> Save([FromBody] FixedAssetDto model, int requestId)
        {
            ResponseDto response;
            try
            {
                var hasApprove = await _approvalSystemService.IsMenuHasApprove(MenuCodeData.FixedAsset);
                if (hasApprove.HasApprove)
                {
                    if (hasApprove.OnAdd && model.FixedAssetId == 0)
                    {
                        response = await SendApproveRequest(model, null, requestId, 0, false);
                    }
                    else if (hasApprove.OnEdit && model.FixedAssetId > 0)
                    {
                        var oldValue = await _fixedAssetService.GetFixedAssetByFixedAssetId(model.FixedAssetId);
                        response = await SendApproveRequest(model, oldValue, requestId, model.FixedAssetId, false);
                    }
                    else
                    {
                        response = await SaveFixedAsset(model);
                    }
                }
                else
                {
                    response = await SaveFixedAsset(model);
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
        [Route("SaveFixedAsset")]
        public async Task<ResponseDto> SaveFixedAsset(FixedAssetDto model)
        {
            await _databaseTransaction.BeginTransaction();
            var response = await _fixedAssetService.SaveFixedAsset(model);
            await _databaseTransaction.Commit();
            return response;
        }

        [HttpDelete]
        [Route("DeleteFixedAsset")]
        public async Task<IActionResult> DeleteFixedAsset(int fixedAssetId, int requestId)
        {
            ResponseDto response;
            try
            {
                var hasApprove = await _approvalSystemService.IsMenuHasApprove(MenuCodeData.FixedAsset);
                if (hasApprove.HasApprove && hasApprove.OnDelete)
                {
                    var model = await _fixedAssetService.GetFixedAssetByFixedAssetId(fixedAssetId);
                    response = await SendApproveRequest(null, model, requestId, fixedAssetId, true);
                }
                else
                {
                    await _databaseTransaction.BeginTransaction();
                    response = await _fixedAssetService.DeleteFixedAssetInFull(fixedAssetId);
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
        public async Task<ResponseDto> SendApproveRequest([FromQuery] FixedAssetDto? newModel, [FromQuery] FixedAssetDto? oldModel, int requestId, int itemId, bool isDelete)
        {
            var changes = new List<RequestChangesDto>();
            var requestTypeId = isDelete ? ApproveRequestTypeData.Delete : (itemId == 0 ? ApproveRequestTypeData.Add : ApproveRequestTypeData.Edit);
            if (requestTypeId == ApproveRequestTypeData.Edit)
            {
                changes = _fixedAssetService.GetFixedAssetRequestChanges(oldModel!, newModel!);
            }

            var request = new NewApproveRequestDto()
            {
                RequestId = requestId,
                MenuCode = MenuCodeData.FixedAsset,
                ApproveRequestTypeId = requestTypeId,
                ReferenceId = itemId,
                ReferenceCode = itemId.ToString(),
                OldValue = oldModel,
                NewValue = newModel,
                Changes = changes
            };
            await _databaseTransaction.BeginTransaction();

            var response = await _handleApprovalRequestService.SaveRequest(request);
            await _databaseTransaction.Commit();
            return response;
        }
    }
}
