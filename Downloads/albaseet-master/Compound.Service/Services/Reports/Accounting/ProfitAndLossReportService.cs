using Compound.CoreOne.Contracts.Reports.Accounting;
using Compound.CoreOne.Models.Dtos.Reports.Accounting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Contracts.Accounts;
using Shared.CoreOne.Contracts.Journal;
using Shared.CoreOne.Models.Domain.Accounts;
using Shared.CoreOne.Models.StaticData;
using Shared.Service.Services.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Shared.CoreOne.Models.StaticData.StaticData;

namespace Compound.Service.Services.Reports.Accounting
{
    public class ProfitAndLossReportService : IProfitAndLossReportService
    {
        private readonly IAccountService _accountService;
        private readonly IAccountBalanceReportService _accountBalanceReportService;
        private readonly IStringLocalizer<ProfitAndLossReportService> _localizer;

        public ProfitAndLossReportService(IAccountService accountService, IAccountBalanceReportService accountBalanceReportService, IStringLocalizer<ProfitAndLossReportService> localizer)
        {
            _accountService = accountService;
            _accountBalanceReportService = accountBalanceReportService;
            _localizer = localizer;
        }

		public async Task<List<ProfitAndLossReportDto>> GetProfitAndLossReport(int companyId, DateTime? fromDate, DateTime? toDate, int? level, int? mainAccountId)
		{
            if (mainAccountId == null || mainAccountId == 0)
            {
                return await GetBaseLevelProfitAndLossReport(companyId, fromDate, toDate, level, mainAccountId);
            }
            else
            {
                return await GetSubLevelProfitAndLossReport(companyId, fromDate, toDate, (int)mainAccountId);
            }
		}

        private async Task<List<ProfitAndLossReportDto>> GetSubLevelProfitAndLossReport(int companyId, DateTime? fromDate, DateTime? toDate, int mainAccountId)
        {
            var accountBalances = await _accountBalanceReportService.GetAccountBalanceReport(companyId, fromDate, toDate, mainAccountId);

            var result = accountBalances.Select((x, index) => new ProfitAndLossReportDto
            {
                Serial = index,
                AccountId = x.AccountId,
                AccountName = x.AccountName,
                AccountNameAr = x.AccountNameAr,
                AccountNameEn = x.AccountNameEn,
                TotalValue = 0,
                Value = x.CreditValue - x.DebitValue,
                MainAccountId = x.MainAccountId,
            }).ToList();

            return result;
        }

        private async Task<List<ProfitAndLossReportDto>> GetBaseLevelProfitAndLossReport(int companyId, DateTime? fromDate, DateTime? toDate, int? level, int? mainAccountId)
        {
            var accountBalances = await _accountBalanceReportService.GetAccountBalanceReport(companyId, fromDate, toDate, 0);
            var rootSalesAccountId = await _accountService.GetAll().Where(x => x.CompanyId == companyId && x.AccountTypeId == AccountTypeData.Sales && x.AccountLevel == 2).Select(x => x.AccountId).FirstOrDefaultAsync();
            var rootRevenueCostsAccountId = await _accountService.GetAll().Where(x => x.CompanyId == companyId && x.AccountTypeId == AccountTypeData.RevenuesCost && x.AccountLevel == 2).Select(x => x.AccountId).FirstOrDefaultAsync();

            var salesAccountEntry = accountBalances.Where(x => x.AccountId == rootSalesAccountId).Select(x => new ProfitAndLossReportDto
            {
                AccountId = x.AccountId,
                Level = x.AccountLevel - 1,
                AccountName = x.AccountName,
                AccountNameAr = x.AccountNameAr,
                AccountNameEn = x.AccountNameEn,
                TotalValue = x.CreditValue - x.DebitValue,
                Value = 0,
                MainAccountId = 0,
            }).First();

            var revenueCostsAccountEntry = accountBalances.Where(x => x.AccountId == rootRevenueCostsAccountId).Select(x => new ProfitAndLossReportDto
            {
                AccountId = x.AccountId,
                Level = x.AccountLevel - 1,
                AccountName = x.AccountName,
                AccountNameAr = x.AccountNameAr,
                AccountNameEn = x.AccountNameEn,
                TotalValue = x.CreditValue - x.DebitValue,
                Value = 0,
                MainAccountId = 0,
            }).First();

            var grossProfit = new ProfitAndLossReportDto
            {
                AccountId = null,
                Level = 1,
                AccountName = _localizer["GrossProfits"],
                AccountNameAr = _localizer["GrossProfitsAr"],
                AccountNameEn = _localizer["GrossProfitsEn"],
                TotalValue = salesAccountEntry.TotalValue + revenueCostsAccountEntry.TotalValue,
                Value = salesAccountEntry.TotalValue + revenueCostsAccountEntry.TotalValue,
                MainAccountId = 0
            };

            var otherIncomeAccountsEntry = accountBalances.Where(x => x.AccountCategoryId == AccountCategoryData.Revenues && x.AccountId != rootSalesAccountId && x.AccountLevel == 2).Select(x => new ProfitAndLossReportDto
            {
				AccountId = x.AccountId, 
                Level = x.AccountLevel - 1,
				AccountName = x.AccountName,
				AccountNameAr = x.AccountNameAr,
				AccountNameEn = x.AccountNameEn,
				TotalValue = x.CreditValue - x.DebitValue,
				Value = 0,
				MainAccountId = 0,
			});

            var otherExpensesAccountsEntry = accountBalances.Where(x => x.AccountCategoryId == AccountCategoryData.Expenses && x.AccountId != rootRevenueCostsAccountId && x.AccountLevel == 2).Select(x => new ProfitAndLossReportDto
            {
				AccountId = x.AccountId, 
                Level = x.AccountLevel - 1,
				AccountName = x.AccountName,
				AccountNameAr = x.AccountNameAr,
				AccountNameEn = x.AccountNameEn,
				TotalValue = x.CreditValue - x.DebitValue,
				Value = 0,
				MainAccountId = 0,
			});

            var netProfit = new ProfitAndLossReportDto
            {
                AccountId = null,
                Level = 1,
                AccountName = _localizer["NetProfits"],
                AccountNameAr = _localizer["NetProfitsAr"],
                AccountNameEn = _localizer["NetProfitsEn"],
                TotalValue = grossProfit.TotalValue + otherIncomeAccountsEntry.Sum(x => x.TotalValue) + otherExpensesAccountsEntry.Sum(x => x.TotalValue),
                Value = grossProfit.TotalValue + otherIncomeAccountsEntry.Sum(x => x.TotalValue) + otherExpensesAccountsEntry.Sum(x => x.TotalValue),
                MainAccountId = 0
            };

            var finalReport = new List<ProfitAndLossReportDto>().Append(salesAccountEntry).Append(revenueCostsAccountEntry).Append(grossProfit).Concat(otherIncomeAccountsEntry).Concat(otherExpensesAccountsEntry).Append(netProfit).ToList();

            if (mainAccountId == 0)
            {
                //add the rest of the accounts if mainAccountId == 0
                var accountIdsToAdd = accountBalances.Where(x => x.AccountLevel >= 3 && (x.AccountCategoryId == AccountCategoryData.Expenses || x.AccountCategoryId == AccountCategoryData.Revenues)).Select(x => x.AccountId).Except(finalReport.Select(x => x.AccountId ?? 0));
                finalReport.AddRange(accountBalances.Where(x => accountIdsToAdd.Contains(x.AccountId)).Select(x => new ProfitAndLossReportDto
                {
                    AccountId = x.AccountId,
                    Level = x.AccountLevel - 1,
                    AccountName = x.AccountName,
                    AccountNameAr = x.AccountNameAr,
                    AccountNameEn = x.AccountNameEn,
                    TotalValue = x.CreditValue - x.DebitValue,
                    Value = 0,
                    MainAccountId = x.MainAccountId,
                }));
            }

            if (level != null)
            {
                finalReport = finalReport.Where(x => x.Level <= level).ToList();
            }

            int serial = 0;
            finalReport.ForEach(x => x.Serial = serial++);
            return finalReport;
        }
    }
}
