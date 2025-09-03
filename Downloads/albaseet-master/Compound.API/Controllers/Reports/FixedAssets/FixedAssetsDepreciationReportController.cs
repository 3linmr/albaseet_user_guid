using Accounting.CoreOne.Contracts;
using Compound.API.Models;
using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Compound.API.Controllers.Reports.FixedAssets
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class FixedAssetsDepreciationReportController : ControllerBase
    {
        private readonly IFixedAssetVoucherService _fixedAssetVoucherService;

        public FixedAssetsDepreciationReportController(IFixedAssetVoucherService fixedAssetVoucherService)
        {
            _fixedAssetVoucherService = fixedAssetVoucherService;
        }
        [HttpGet]
        [Route("GetFixedAssetDepreciation")]
        public async Task<IActionResult> GetFixedAssetDepreciation(DataSourceLoadOptions loadOptions, int fixedAssetId, DateTime? fromDate, DateTime? toDate)
        {
            var data = await _fixedAssetVoucherService.GetFixedAssetDepreciation(fixedAssetId, fromDate, toDate);
            return Ok(DataSourceLoader.Load(data, loadOptions));
        }
        [HttpGet]
        [Route("GetAllFixedAssetsDepreciation")]
        public async Task<IActionResult> GetAllFixedAssetsDepreciation(DataSourceLoadOptions loadOptions, DateTime? fromDate, DateTime? toDate)
        {
            var data = await _fixedAssetVoucherService.GetAllFixedAssetsDepreciation(fromDate, toDate);
            return Ok(DataSourceLoader.Load(data, loadOptions));
        }
    }
}
