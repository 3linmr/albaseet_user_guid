using Compound.CoreOne.Contracts.Reports.Accounting;
using Compound.CoreOne.Models.Dtos.Reports;
using Compound.CoreOne.Models.Dtos.Reports.Accounting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne.Contracts.Accounts;
using Shared.CoreOne.Contracts.Journal;
using Shared.CoreOne.Models.Domain.Journal;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Identity;
using System;
using System.Linq;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using static Shared.CoreOne.Models.StaticData.StaticData;

namespace Compound.Service.Services.Reports.Accounting
{
    public class BalanceSheetReportService : IBalanceSheetReportService
    {
        private readonly IAccountBalanceReportService _accountBalanceReportService;

        public BalanceSheetReportService(IAccountBalanceReportService accountBalanceReportService)
        {
            _accountBalanceReportService = accountBalanceReportService;
        }

        private enum DisplayMode {
            CreditOrZero,
            DebitOrZero,
            CreditOnly,
            DebitOnly,
        }

        public async Task<List<BalanceSheetReportDto>> GetBalanceSheetReport(int companyId, DateTime? fromDate, DateTime? toDate, int? level, int? parentSerial)
        {
            var accountTree = await _accountBalanceReportService.GetAccountDataQueryable(companyId, fromDate, toDate, false).GroupBy(x => x.MainAccountId ?? 0).ToDictionaryAsync(x => x.Key, x => x.ToList());

            var rootAssetAccount = accountTree[0].Where(x => x.AccountCategoryId == AccountCategoryData.Assets).FirstOrDefault();
            var rootLiabilityAccount = accountTree[0].Where(x => x.AccountCategoryId == AccountCategoryData.Liabilities).FirstOrDefault();
            var netProfit = accountTree.SelectMany(x => x.Value).Where(x => x.AccountCategoryId == AccountCategoryData.Revenues || x.AccountCategoryId == AccountCategoryData.Expenses).Sum(x => x.CurrentBalance);

            var resultList = new List<BalanceSheetReportDto>();
            int serial = 1;

            AddHeadingToResultList(resultList, ref serial, "الحسابات المدينة", "Debit Accounts", BalanceSheetFormatType.Heading, level);

            resultList.AddRange(CalculateAndOrderBalanceSheetReport(accountTree, DisplayMode.DebitOrZero, rootAssetAccount, out var assetDebitTotal, ref serial, level));
            AddTotalToResultList(resultList, ref serial, "مجموع الاصول", "Assets Total", assetDebitTotal, level);

            AddHeadingToResultList(resultList, ref serial, "الخصوم (مدين)", "Liabilities (Debit)", BalanceSheetFormatType.SubHeading, level);

            resultList.AddRange(CalculateAndOrderBalanceSheetReport(accountTree, DisplayMode.DebitOnly, rootLiabilityAccount, out var liabilityDebitTotal, ref serial, level));
            AddTotalToResultList(resultList, ref serial, "مجموع الخصوم (مدين)", "Total Liabilities (Debit)", liabilityDebitTotal, level);

            AddNetValueToResultList(resultList, ref serial, "صافي الحسابات المدينة", "Debit Accounts Total", assetDebitTotal + liabilityDebitTotal, level);

            AddHeadingToResultList(resultList, ref serial, "الحسابات الدائنة", "Credit Accounts", BalanceSheetFormatType.Heading, level);

            resultList.AddRange(CalculateAndOrderBalanceSheetReport(accountTree, DisplayMode.CreditOrZero, rootLiabilityAccount, out var liabilityCreditTotal, ref serial, level));
            AddTotalToResultList(resultList, ref serial, "مجموع الخصوم", "Liabilities Total", liabilityCreditTotal, level);

            AddHeadingToResultList(resultList, ref serial, "الأصول (دائن)", "Assets (Credit)", BalanceSheetFormatType.SubHeading, level);

            resultList.AddRange(CalculateAndOrderBalanceSheetReport(accountTree, DisplayMode.CreditOnly, rootAssetAccount, out var assetCreditTotal, ref serial, level));
            AddTotalToResultList(resultList, ref serial, "مجموع الأصول (دائن)", "Total Assets (Credit)", assetCreditTotal, level);

            AddNetProfitToResultList(resultList, ref serial, "صافي الربح", "Net Profit", netProfit, level);

            AddNetValueToResultList(resultList, ref serial, "صافي الحسابات الدائنة", "Credit Accounts Total", assetCreditTotal + liabilityCreditTotal + netProfit, level);
            serial++;

            return resultList.Where(x => (parentSerial == null && x.ParentSerial == 0) || (parentSerial == 0) || x.ParentSerial == parentSerial).ToList();
        }

        private void AddNetValueToResultList(List<BalanceSheetReportDto> resultList, ref int serial, string accountNameAr, string accountNameEn, decimal netValue, int? level)
        {
			if (level >= 1 || level == null)
            {
                resultList.Add(new BalanceSheetReportDto
                {
                    Level = 1,
					Serial = serial,
                    AccountId = null,
                    MainAccountId = null,
                    AccountNameAr = accountNameAr,
                    AccountNameEn = accountNameEn,
                    Value = null,
                    SubTotal = null,
                    NetValue = netValue,
                    BalanceSheetFormatType = BalanceSheetFormatType.NetValue
                });
            }
            serial++; //note that serial has to be incremented even if the level is not 1, because it is used to order the result
		}

        private void AddTotalToResultList(List<BalanceSheetReportDto> resultList, ref int serial, string accountNameAr, string accountNameEn, decimal total, int? level)
        {
			if (level >= 1 || level == null)
			{
				resultList.Add(new BalanceSheetReportDto
				{
					Level = 1,
					Serial = serial,
					AccountId = null,
					MainAccountId = null,
					AccountNameAr = accountNameAr,
					AccountNameEn = accountNameEn,
					Value = null,
					SubTotal = total,
					NetValue = null,
					BalanceSheetFormatType = BalanceSheetFormatType.SubTotal
				});
			}
			serial++;
        }

        private void AddNetProfitToResultList(List<BalanceSheetReportDto> resultList, ref int serial, string accountNameAr, string accountNameEn, decimal netProfit, int? level)
        {
			if (level >= 1 || level == null)
			{
				resultList.Add(new BalanceSheetReportDto
				{
					Level = 1,
					Serial = serial,
					AccountId = null,
					MainAccountId = null,
					AccountNameAr = accountNameAr,
					AccountNameEn = accountNameEn,
					Value = null,
					SubTotal = netProfit,
					NetValue = null,
					BalanceSheetFormatType = netProfit > 0 ? BalanceSheetFormatType.NetProfit : BalanceSheetFormatType.NetLoss
				});
			}
			serial++;
        }

        private void AddHeadingToResultList(List<BalanceSheetReportDto> resultList, ref int serial, string accountNameAr, string accountNameEn, string formatType, int? level)
        {
			if (level >= 1 || level == null)
            {
                resultList.Add(new BalanceSheetReportDto
                {
                    Level = 1,
                    Serial = serial,
                    AccountId = null,
                    MainAccountId = null,
                    AccountNameAr = accountNameAr,
                    AccountNameEn = accountNameEn,
                    Value = null,
                    SubTotal = null,
                    NetValue = null,
                    BalanceSheetFormatType = formatType
                });
            }
            serial++;
        }

        private List<BalanceSheetReportDto> CalculateAndOrderBalanceSheetReport(Dictionary<int, List<BalanceReportDto>> accountTree, DisplayMode displayMode, BalanceReportDto? rootAccount, out decimal totalValue, ref int serial, int? level, int currentLevel = 1, int? parentSerial = null)
        {
            // Get the immediate children of the current account
            var children = accountTree.GetValueOrDefault(rootAccount?.AccountId ?? 0, []);
            var currentSerial = serial++;

            // Recursively process children and accumulate their results and total values
            var childResults = new List<BalanceSheetReportDto>();
            decimal childrenTotal = 0m;
            foreach (var child in children)
            {
                childResults.AddRange(CalculateAndOrderBalanceSheetReport(accountTree, displayMode, child, out var childTotal, ref serial, level, currentLevel + 1, currentSerial));
                childrenTotal += childTotal;
            }

            // Calculate the total value for the current account (including its children)
            decimal accountTotal = (rootAccount?.CurrentBalance ?? 0) + childrenTotal;

            // Prepare the DTO for the current account, if it should be included
            BalanceSheetReportDto? accountDto = null;
            if (rootAccount != null)
            {
                accountDto = CreateBalanceSheetReportDto(rootAccount, currentSerial, parentSerial, accountTotal, displayMode);
            }

			// add parent to first of result if matches the level, this is the part that filters the result based on the level
            // but if level is null return all levels
			var result = new List<BalanceSheetReportDto>();
            if (level == null || currentLevel <= level)
            {
                if (accountDto != null)
                    result.Add(accountDto);
            }
			result.AddRange(childResults);

            totalValue = accountDto?.Value ?? 0;
            return result;
        }

		private static BalanceSheetReportDto? CreateBalanceSheetReportDto(BalanceReportDto account, int currentSerial, int? parentSerial, decimal accountTotal, DisplayMode displayMode)
		{
			if (account == null) return null;
			return displayMode switch
			{
				DisplayMode.DebitOrZero => new BalanceSheetReportDto
				{
					Level = account.AccountLevel,
					Serial = currentSerial,
					ParentSerial = parentSerial ?? 0,
					AccountId = account.AccountId,
					MainAccountId = account.MainAccountId,
					AccountNameAr = account.AccountNameAr,
					AccountNameEn = account.AccountNameEn,
					Value = Math.Min(accountTotal, 0),
					SubTotal = null,
					NetValue = null,
                    BalanceSheetFormatType = BalanceSheetFormatType.Regular
				},
				DisplayMode.CreditOrZero => new BalanceSheetReportDto
				{
					Level = account.AccountLevel,
					Serial = currentSerial,
					ParentSerial = parentSerial ?? 0,
					AccountId = account.AccountId,
					MainAccountId = account.MainAccountId,
					AccountNameAr = account.AccountNameAr,
					AccountNameEn = account.AccountNameEn,
					Value = Math.Max(accountTotal, 0),
					SubTotal = null,
					NetValue = null,
                    BalanceSheetFormatType = BalanceSheetFormatType.Regular
				},
				DisplayMode.DebitOnly when accountTotal < 0 => new BalanceSheetReportDto
				{
					Level = account.AccountLevel,
					Serial = currentSerial,
					ParentSerial = parentSerial ?? 0,
					AccountId = account.AccountId,
					MainAccountId = account.MainAccountId,
					AccountNameAr = account.AccountNameAr,
					AccountNameEn = account.AccountNameEn,
					Value = accountTotal,
					SubTotal = null,
					NetValue = null,
					BalanceSheetFormatType = BalanceSheetFormatType.Regular
				},
				DisplayMode.CreditOnly when accountTotal > 0 => new BalanceSheetReportDto
				{
					Level = account.AccountLevel,
					Serial = currentSerial,
					ParentSerial = parentSerial ?? 0,
					AccountId = account.AccountId,
					MainAccountId = account.MainAccountId,
					AccountNameAr = account.AccountNameAr,
					AccountNameEn = account.AccountNameEn,
					Value = accountTotal,
					SubTotal = null,
					NetValue = null,
					BalanceSheetFormatType = BalanceSheetFormatType.Regular
				},
				_ => null
			};
		}
    }
}