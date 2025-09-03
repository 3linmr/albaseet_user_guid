using Compound.CoreOne.Contracts.Reports;
using Compound.CoreOne.Models.Dtos.Reports;
using Compound.CoreOne.Contracts.Reports.CostCenters;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne.Contracts.CostCenters;
using Shared.CoreOne.Contracts.Journal;
using Shared.CoreOne.Models.Domain.CostCenters;
using Shared.CoreOne.Models.Domain.Journal;
using Shared.CoreOne.Models.Dtos.ViewModels.Journal;
using Shared.Helper.Identity;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Compound.CoreOne.Models.Dtos.Reports.CostCenters;

namespace Compound.Service.Services.Reports.Accounting
{
    public class MainCostCenterReportService : IMainCostCenterReportService
    {
        private readonly IIndividualCostCenterReportService _individualCostCenterReportService;
        private readonly ICostCenterService _costCenterService;
		private readonly IHttpContextAccessor _httpContextAccessor;

        public MainCostCenterReportService(IIndividualCostCenterReportService individualCostCenterReportService, ICostCenterService costCenterService, IHttpContextAccessor httpContextAccessor)
        {
            _individualCostCenterReportService = individualCostCenterReportService;
            _costCenterService = costCenterService;
			_httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<MainCostCenterReportDto>> GetMainCostCenterReport(int companyId, int? mainCostCenterId, DateTime? fromDate, DateTime? toDate, bool debitOnly, bool includeItems)
        {
            var individualCostCenters = await _individualCostCenterReportService.GetIndividualCostCenterReport(companyId, fromDate, toDate, debitOnly, includeItems).ToListAsync();
            var allCostCenters = await _costCenterService.GetAll().Where(x => x.CompanyId == companyId).ToListAsync();

            var language = _httpContextAccessor.GetProgramCurrentLanguage();
            var allCostCentersInitialized = (from costCenter in allCostCenters
                                             from mainCostCenter in allCostCenters.Where(x => x.CostCenterId == costCenter.MainCostCenterId).DefaultIfEmpty()
                                             from individualCostCenter in individualCostCenters.Where(x => x.CostCenterId == costCenter.CostCenterId).DefaultIfEmpty()
                                             orderby costCenter.CostCenterLevel //the algorithm below relies on parent cost centers appearing before child cost centers
                                             select new MainCostCenterReportDto
                                             {
                                                 CostCenterId = costCenter.CostCenterId,
                                                 CostCenterCode = costCenter.CostCenterCode,
                                                 CostCenterName = language == LanguageCode.Arabic ? costCenter.CostCenterNameAr : costCenter.CostCenterNameEn,
                                                 CostCenterLevel = costCenter.CostCenterLevel,
                                                 IsMainCostCenter = costCenter.IsMainCostCenter,
                                                 MainCostCenterId = costCenter.MainCostCenterId,
                                                 MainCostCenterCode = individualCostCenter?.CostCenterCode,
                                                 MainCostCenterName = language == LanguageCode.Arabic ? mainCostCenter?.CostCenterNameAr : mainCostCenter?.CostCenterNameEn,
                                                 OpenBalance = individualCostCenter?.OpenBalance ?? 0,
                                                 DebitValue = individualCostCenter?.DebitValue ?? 0,
                                                 CreditValue = individualCostCenter?.CreditValue ?? 0,
                                                 CurrentBalance = individualCostCenter?.CurrentBalance ?? 0,
                                                 CreatedAt = costCenter.CreatedAt,
                                                 UserNameCreated = costCenter.UserNameCreated,
                                                 ModifiedAt = costCenter.ModifiedAt,
                                                 UserNameModified = costCenter.UserNameModified
                                             }).ToDictionary(x => x.CostCenterId);


            foreach (var costCenter in allCostCentersInitialized.Values)
            {
                var totalCredit = costCenter.CreditValue;
                var totalDebit = costCenter.DebitValue;
                var totalOpenBalance = costCenter.OpenBalance;
                var totalCurrentBalance = costCenter.CurrentBalance;
                var currentParentId = costCenter.MainCostCenterId;

                while ((currentParentId ?? 0) != 0 && (totalCredit != 0 || totalDebit != 0 || totalOpenBalance != 0 || totalCurrentBalance != 0))
                {
                    var mainCostCenter = allCostCentersInitialized.GetValueOrDefault(currentParentId ?? 0);
                    if (mainCostCenter != null)
                    {
                        mainCostCenter.OpenBalance += totalOpenBalance;
                        mainCostCenter.DebitValue += totalDebit;
                        mainCostCenter.CreditValue += totalCredit;
                        mainCostCenter.CurrentBalance += totalCurrentBalance;
                        currentParentId = mainCostCenter.MainCostCenterId;
                    }
                }
            }

            return allCostCentersInitialized.Values.Where(x => x.MainCostCenterId == mainCostCenterId).ToList();
        }
    }
}
