using Compound.CoreOne.Contracts.Reports.Clients;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Compound.API.Models;
using DevExtreme.AspNet.Data;
using Compound.Service.Services.Reports.Clients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
namespace Compound.API.Controllers.Reports.Clients
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ClientsExceedCreditLimitController : ControllerBase
    {
        private readonly IClientsExceedCreditLimitReportService _clientsExceedCreditLimitReportService;

        public ClientsExceedCreditLimitController(IClientsExceedCreditLimitReportService clientsExceedCreditLimitReportService)
        {
            this._clientsExceedCreditLimitReportService = clientsExceedCreditLimitReportService;
        }

        [HttpGet]
        [Route("ReadClientsExceedCreditLimitReport")]
        public async Task<IActionResult> ReadClientsExceedCreditLimitReport(DataSourceLoadOptions loadOptions,int companyId,int?clientId,bool isDay)
        {
            var data = _clientsExceedCreditLimitReportService.GetClientsExceedCreditLimitReport(companyId,clientId, isDay);
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
