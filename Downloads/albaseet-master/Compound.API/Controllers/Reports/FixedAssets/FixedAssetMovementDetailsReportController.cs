using Accounting.CoreOne.Contracts;
using Accounting.Service.Services;
using Compound.API.Models;
using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne.Models.Domain.FixedAssets;

namespace Compound.API.Controllers.Reports.FixedAssets
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class FixedAssetMovementDetailsReportController : ControllerBase
    {
        private readonly IFixedAssetMovementDetailService _fixedAssetMovementDetailService;

        public FixedAssetMovementDetailsReportController(IFixedAssetMovementDetailService fixedAssetMovementDetailService)
        {
            _fixedAssetMovementDetailService = fixedAssetMovementDetailService;
        }
        [HttpGet]
        [Route("GetFixedAssetMovementDetail")]
        public async Task<IActionResult> GetFixedAssetMovementDetail(DataSourceLoadOptions loadOptions, int fixedAssetId, DateTime? fromDate, DateTime? toDate)
        {
            var data = await _fixedAssetMovementDetailService.GetAllFixedAssetMovementDetails()
                .Where(x =>
                (x.FixedAssetId == fixedAssetId) &&
                (fromDate == null || x.MovementDate >= fromDate) &&
                (toDate == null || x.MovementDate <= toDate)
                )
                .ToListAsync();
            return Ok(DataSourceLoader.Load(data, loadOptions));
        }
        [HttpGet]
        [Route("GetAllFixedAssetsMovementDetail")]
        public async Task<IActionResult> GetAllFixedAssetsMovementDetail(DataSourceLoadOptions loadOptions, DateTime? fromDate, DateTime? toDate)
        {
            var data = await _fixedAssetMovementDetailService.GetAllFixedAssetMovementDetails()
                .Where(x =>
                (fromDate == null || x.MovementDate >= fromDate) &&
                (toDate == null || x.MovementDate <= toDate)
                )
                .ToListAsync();
            return Ok(DataSourceLoader.Load(data, loadOptions));
        }
    }
}
