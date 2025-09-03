using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Compound.CoreOne.Contracts.Reports.FollowUpReports;
using DevExtreme.AspNet.Data;
using System.Text.Json;
using System.Threading.Tasks;
using Compound.API.Models;
using Compound.CoreOne.Contracts.Reports.FollowUpDetailReports;
using Microsoft.EntityFrameworkCore;
using Compound.CoreOne.Contracts.Reports.FollowUpCombinedReports;

namespace Compound.API.Controllers.Reports.FollowUpReports
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReservationInvoiceFollowUpReportController : ControllerBase
    {
        private readonly IReservationInvoiceFollowUpReportService _reservationInvoiceFollowUpReportService;
        private readonly IReservationInvoiceFollowUpDetailReportService _reservationInvoiceFollowUpDetailReportService;
        private readonly IReservationInvoiceFollowUpCombinedReportService _reservationInvoiceFollowUpCombinedReportService;

        public ReservationInvoiceFollowUpReportController(IReservationInvoiceFollowUpReportService reservationInvoiceFollowUpReportService, IReservationInvoiceFollowUpDetailReportService reservationInvoiceFollowUpDetailReportService, IReservationInvoiceFollowUpCombinedReportService reservationInvoiceFollowUpCombinedReportService)
        {
            _reservationInvoiceFollowUpReportService = reservationInvoiceFollowUpReportService;
            _reservationInvoiceFollowUpDetailReportService = reservationInvoiceFollowUpDetailReportService;
            _reservationInvoiceFollowUpCombinedReportService = reservationInvoiceFollowUpCombinedReportService;
        }

        [HttpGet]
        [Route("ReadReservationInvoiceFollowUpReport")]
        public async Task<IActionResult> ReadReservationInvoiceFollowUpReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = _reservationInvoiceFollowUpReportService.GetReservationInvoiceFollowUpReport(storeIdsList!, fromDate, toDate);

            try
            {
                return Ok(await DataSourceLoader.LoadAsync(data, loadOptions));
            }
            catch
            {
                return Ok(DataSourceLoader.Load(await data.ToListAsync(), loadOptions));
            }
        }

        [HttpGet]
        [Route("ReadReservationInvoiceFollowUpDetailReport")]
        public async Task<IActionResult> ReadReservationInvoiceFollowUpDetailReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = await _reservationInvoiceFollowUpDetailReportService.GetReservationInvoiceFollowUpDetailReport(storeIdsList!, fromDate, toDate);

            try
            {
                return Ok(await DataSourceLoader.LoadAsync(data, loadOptions));
            }
            catch
            {
                return Ok(DataSourceLoader.Load(await data.ToListAsync(), loadOptions));
            }
        }

        [HttpGet]
        [Route("ReadReservationInvoiceFollowUpCombinedReport")]
        public async Task<IActionResult> ReadReservationInvoiceFollowUpCombinedReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate, bool isDetail)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = await _reservationInvoiceFollowUpCombinedReportService.GetReservationInvoiceFollowUpCombinedReport(storeIdsList!, fromDate, toDate, isDetail);

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