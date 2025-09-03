using Compound.CoreOne.Contracts.Reports.CostCenters;
using Microsoft.AspNetCore.Http;
using Shared.CoreOne.Contracts.CostCenters;
using Shared.CoreOne.Contracts.Journal;
using Shared.Helper.Identity;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Compound.CoreOne.Models.Dtos.Reports.CostCenters;
using Shared.CoreOne.Contracts.Accounts;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.StaticData;
using Compound.CoreOne.Contracts.Reports.Shared;
using Shared.CoreOne.Contracts.Menus;
using Shared.CoreOne.Contracts.Items;

namespace Compound.Service.Services.Reports.CostCenters
{
    public class CostCenterAccountsReportService : ICostCenterAccountsReportService
    {
        private readonly ICostCenterJournalReportService _costCenterJournalReportService;
		private readonly ICostCenterService _costCenterService;

        public CostCenterAccountsReportService(ICostCenterJournalReportService costCenterJournalReportService, ICostCenterService costCenterService)
        {
            _costCenterJournalReportService = costCenterJournalReportService;
			_costCenterService = costCenterService;
        }

        public IQueryable<CostCenterAccountsReportDto> GetCostCenterAccountsReport(int? costCenterId, int companyId, DateTime? fromDate, DateTime? toDate, bool debitOnly, bool includeItems)
        {
            var costCenterAccounts = from costCenterJournal in _costCenterJournalReportService.GetCostCenterJournalReport(costCenterId, companyId, fromDate, toDate, debitOnly, includeItems)
                                     from costCenter in _costCenterService.GetAll().Where(x => x.CostCenterId == costCenterJournal.CostCenterId)
									 group costCenterJournal by new { costCenterJournal.CostCenterId, costCenterJournal.CostCenterCode, costCenterJournal.CostCenterName, costCenterJournal.AccountCode, costCenterJournal.AccountName, costCenterJournal.CompanyId, costCenterJournal.CompanyName, costCenter.CreatedAt, costCenter.UserNameCreated, costCenter.ModifiedAt, costCenter.UserNameModified } into g
                                     select new CostCenterAccountsReportDto
                                     {
                                         CostCenterId = g.Key.CostCenterId,
                                         CostCenterCode = g.Key.CostCenterCode,
                                         CostCenterName = g.Key.CostCenterName,

                                         AccountCode = g.Key.AccountCode,
                                         AccountName = g.Key.AccountName,

                                         CreditValue = g.Sum(x => x.CreditValue),
                                         DebitValue = g.Sum(x => x.DebitValue),
                                         ProfitAndLossValue = g.Sum(x => x.ProfitAndLossValue),

                                         Quantity = g.Sum(x => x.Quantity),

                                         CompanyId = g.Key.CompanyId,
                                         CompanyName = g.Key.CompanyName,

                                         CreatedAt = g.Key.CreatedAt,
                                         UserNameCreated = g.Key.UserNameCreated,
                                         ModifiedAt = g.Key.ModifiedAt,
                                         UserNameModified = g.Key.UserNameModified,
                                     };

            return costCenterAccounts;
        }
    }
}
