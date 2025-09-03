using Compound.CoreOne.Contracts.Reports.Suppliers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Compound.API.Models;
using DevExtreme.AspNet.Data;
using Compound.Service.Services.Reports.Suppliers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
namespace Compound.API.Controllers.Reports.Suppliers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SuppliersExceedCreditLimitController : ControllerBase
    {
        private readonly ISuppliersExceedCreditLimitReportService _suppliersExceedCreditLimitReportService;

        public SuppliersExceedCreditLimitController(ISuppliersExceedCreditLimitReportService suppliersExceedCreditLimitReportService)
        {
            _suppliersExceedCreditLimitReportService = suppliersExceedCreditLimitReportService;
        }

        [HttpGet]
        [Route("ReadSuppliersExceedCreditLimitReport")]
        public async Task<IActionResult> ReadSuppliersExceedCreditLimitReport(DataSourceLoadOptions loadOptions, int companyId, int? supplierId, bool isDay)
        {
            var data = _suppliersExceedCreditLimitReportService.GetSuppliersExceedCreditLimitReport(companyId, supplierId, isDay);
            try
            {
                return Ok(await DataSourceLoader.LoadAsync(data, loadOptions));
            }
            catch
            {
                return Ok(DataSourceLoader.Load(await data.ToListAsync(), loadOptions));
            }
        }
    }
}
