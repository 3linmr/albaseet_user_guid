using Compound.CoreOne.Contracts.Reports.FollowUpCombinedReports;
using Compound.CoreOne.Contracts.Reports.FollowUpDetailReports;
using Compound.CoreOne.Contracts.Reports.FollowUpReports;
using Compound.CoreOne.Models.Dtos.Reports.FollowUpReports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Compound.Service.Services.Reports.FollowUpCombinedReports
{
    public class ReservationInvoiceFollowUpCombinedReportService : IReservationInvoiceFollowUpCombinedReportService
    {
        private readonly IReservationInvoiceFollowUpReportService _reservationInvoiceFollowUpReportService;
        private readonly IReservationInvoiceFollowUpDetailReportService _reservationInvoiceFollowUpDetailReportService;

        public ReservationInvoiceFollowUpCombinedReportService(IReservationInvoiceFollowUpReportService reservationInvoiceFollowUpReportService, IReservationInvoiceFollowUpDetailReportService reservationInvoiceFollowUpDetailReportService)
        {
            _reservationInvoiceFollowUpReportService = reservationInvoiceFollowUpReportService;
            _reservationInvoiceFollowUpDetailReportService = reservationInvoiceFollowUpDetailReportService;
        }

        public async Task<IQueryable<ReservationInvoiceFollowUpReportVm>> GetReservationInvoiceFollowUpCombinedReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate, bool isDetail)
        {
            return isDetail
                ? (await _reservationInvoiceFollowUpDetailReportService.GetReservationInvoiceFollowUpDetailReport(storeIds, fromDate, toDate))
                    .Select(x => new ReservationInvoiceFollowUpReportVm { ReservationInvoiceFollowUpDetailReportDto = x })
                : _reservationInvoiceFollowUpReportService.GetReservationInvoiceFollowUpReport(storeIds, fromDate, toDate)
                    .Select(x => new ReservationInvoiceFollowUpReportVm { ReservationInvoiceFollowUpReportDto = x });
        }
    }
}