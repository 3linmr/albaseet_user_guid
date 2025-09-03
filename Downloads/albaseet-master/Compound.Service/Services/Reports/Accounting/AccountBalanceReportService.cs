using Compound.CoreOne.Contracts.Reports.Accounting;
using Compound.CoreOne.Models.Dtos.Reports;
using Compound.CoreOne.Models.Dtos.Reports.Accounting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne.Contracts.Accounts;
using Shared.CoreOne.Contracts.Journal;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Domain.Accounts;
using Shared.CoreOne.Models.Domain.Journal;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Journal;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Identity;
using Shared.Service.Services.Accounts;
using Shared.Service.Services.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Compound.Service.Services.Reports.Accounting
{
    public class AccountBalanceReportService : IAccountBalanceReportService
    {
        private readonly IAccountCategoryService _accountCategoryService;
        private readonly IJournalDetailService _journalDetailService;
        private readonly IAccountService _accountService;
        private readonly IJournalHeaderService _journalHeaderService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAccountTypeService _accountTypeService;
        private readonly ICompanyService _companyService;

        public AccountBalanceReportService(IAccountCategoryService accountCategoryService, IJournalDetailService journalDetailService, IAccountService accountService, IJournalHeaderService journalHeaderService, IHttpContextAccessor httpContextAccessor, IAccountTypeService accountTypeService, ICompanyService companyService)
        {
            _accountCategoryService = accountCategoryService;
            _journalDetailService = journalDetailService;
            _accountService = accountService;
            _journalHeaderService = journalHeaderService;
            _httpContextAccessor = httpContextAccessor;
            _accountTypeService = accountTypeService;
            _companyService = companyService;
        }

        public async Task<List<BalanceReportDto>> GetAccountBalanceReport(int companyId, DateTime? fromDate, DateTime? toDate, int? mainAccountId)
        {
            var accounts = await GetAccountDataQueryable(companyId, fromDate, toDate, false)
                .OrderBy(x => x.AccountLevel)  //parents must be before children in the list for the algorithm below to be correct
                .ToListAsync();

            var accountDictionary = accounts.ToDictionary(x => x.AccountId);

            foreach (var account in accounts)
            {
                var totalCredit = account.CreditValue;
                var totalDebit = account.DebitValue;
                var totalOpenBalance = account.OpenBalance;
                var totalCurrentBalance = account.CurrentBalance;
                var totalCurrentCreditBalance = account.CurrentCreditBalance;
                var totalCurrentDebitBalance = account.CurrentDebitBalance;
                var totalOpenCreditBalance = account.OpenCreditBalance;
                var totalOpenDebitBalance = account.OpenDebitBalance;
                var totalDifference = account.Difference;
                var currentParentId = account.MainAccountId;

                while ((currentParentId ?? 0) != 0 && (totalCredit != 0 || totalDebit != 0 || totalOpenBalance != 0 || totalCurrentBalance != 0 || totalCurrentCreditBalance != 0 || totalCurrentDebitBalance != 0 || totalOpenCreditBalance != 0 || totalOpenDebitBalance != 0 || totalDifference != 0))
                {
                    var mainAccount = accountDictionary.GetValueOrDefault(currentParentId ?? 0);
                    if (mainAccount != null)
                    {
                        mainAccount.OpenBalance += totalOpenBalance;
                        mainAccount.DebitValue += totalDebit;
                        mainAccount.CreditValue += totalCredit;
                        mainAccount.CurrentBalance += totalCurrentBalance;
                        mainAccount.CurrentCreditBalance += totalCurrentCreditBalance;
                        mainAccount.CurrentDebitBalance += totalCurrentDebitBalance;
                        mainAccount.OpenCreditBalance += totalOpenCreditBalance;
                        mainAccount.OpenDebitBalance += totalOpenDebitBalance;
                        mainAccount.Difference += totalDifference;
                        currentParentId = mainAccount.MainAccountId;
                    }
                }
            }

            return accounts.Where(x => mainAccountId == 0 || (mainAccountId == null && (x.MainAccountId == 0)) || (x.MainAccountId == mainAccountId)).ToList();
        }

		public IQueryable<BalanceReportDto> GetAccountDataQueryable(int companyId, DateTime? fromDate, DateTime? toDate, bool onlyNonMainAccounts)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var accountsQuery = _accountService.GetAll()
				.Where(account => account.CompanyId == companyId && (!onlyNonMainAccounts || !account.IsMainAccount));

			var journalHeaderQuery = _journalHeaderService.GetAll()
				.Where(journalHeader => toDate == null || journalHeader.TicketDate <= toDate);

			var journalAggregation = from journalDetail in _journalDetailService.GetAll()
									 join journalHeader in journalHeaderQuery
										 on journalDetail.JournalHeaderId equals journalHeader.JournalHeaderId
									 group new { journalDetail, journalHeader } by journalDetail.AccountId into accountGroup
									 select new
									 {
										 AccountId = accountGroup.Key,
										 OpenDebitBalance = fromDate != null ? accountGroup.Where(x => x.journalHeader.TicketDate < fromDate).Sum(x => (decimal?)x.journalDetail.DebitValue) ?? 0 : 0,
										 OpenCreditBalance = fromDate != null ? accountGroup.Where(x => x.journalHeader.TicketDate < fromDate).Sum(x => (decimal?)x.journalDetail.CreditValue) ?? 0 : 0,
										 PeriodDebitValue = accountGroup.Where(x =>
											 (fromDate == null || x.journalHeader.TicketDate >= fromDate) &&
											 (toDate == null || x.journalHeader.TicketDate <= toDate)
										 ).Sum(x => (decimal?)x.journalDetail.DebitValue) ?? 0,
										 PeriodCreditValue = accountGroup.Where(x =>
											 (fromDate == null || x.journalHeader.TicketDate >= fromDate) &&
											 (toDate == null || x.journalHeader.TicketDate <= toDate)
										 ).Sum(x => (decimal?)x.journalDetail.CreditValue) ?? 0,
										 CurrentDebitBalance = accountGroup.Where(x => toDate == null || x.journalHeader.TicketDate <= toDate).Sum(x => (decimal?)x.journalDetail.DebitValue) ?? 0,
										 CurrentCreditBalance = accountGroup.Where(x => toDate == null || x.journalHeader.TicketDate <= toDate).Sum(x => (decimal?)x.journalDetail.CreditValue) ?? 0,
									 };

			var query = from account in accountsQuery
						join accountCategory in _accountCategoryService.GetAll() on account.AccountCategoryId equals accountCategory.AccountCategoryId
						join company in _companyService.GetAll() on account.CompanyId equals company.CompanyId
						join accountType in _accountTypeService.GetAll() on account.AccountTypeId equals accountType.AccountTypeId into accountTypeGroup
						from accountType in accountTypeGroup.DefaultIfEmpty()
						join journalSum in journalAggregation on account.AccountId equals journalSum.AccountId into journalSumGroup
						from journalSum in journalSumGroup.DefaultIfEmpty()
						select new BalanceReportDto
						{
							AccountId = account.AccountId,
							AccountCode = account.AccountCode ?? "",
							CompanyId = company.CompanyId,
							CompanyName = language == LanguageData.LanguageCode.Arabic ? company.CompanyNameAr : company.CompanyNameEn,
							AccountName = language == LanguageData.LanguageCode.Arabic ? account.AccountNameAr : account.AccountNameEn,
							AccountNameAr = account.AccountNameAr ?? "",
							AccountNameEn = account.AccountNameEn ?? "",
							AccountTypeName = language == LanguageData.LanguageCode.Arabic
								? (accountType != null ? accountType.AccountTypeNameAr : "")
								: (accountType != null ? accountType.AccountTypeNameEn : ""),
							AccountCategoryId = accountCategory.AccountCategoryId,
							AccountCategoryName = language == LanguageData.LanguageCode.Arabic
								? accountCategory.AccountCategoryNameAr
								: accountCategory.AccountCategoryNameEn,
							AccountLevel = account.AccountLevel,
							MainAccountId = account.MainAccountId ?? 0,

							OpenBalance = (decimal?)journalSum.OpenCreditBalance - (decimal?)journalSum.OpenDebitBalance ?? 0,
							OpenDebitBalance = (decimal?)journalSum.OpenDebitBalance ?? 0,
							OpenCreditBalance = (decimal?)journalSum.OpenCreditBalance ?? 0,

							DebitValue = (decimal?)journalSum.PeriodDebitValue ?? 0,
							CreditValue = (decimal?)journalSum.PeriodCreditValue ?? 0,
							Difference = ((decimal?)journalSum.PeriodCreditValue - (decimal?)journalSum.PeriodDebitValue) ?? 0,

							CurrentBalance = ((decimal?)journalSum.CurrentCreditBalance - (decimal?)journalSum.CurrentDebitBalance) ?? 0,
							CurrentDebitBalance = (decimal?)journalSum.CurrentDebitBalance ?? 0,
							CurrentCreditBalance = (decimal?)journalSum.CurrentCreditBalance ?? 0,
						};

			return query;
		}
	}
}
