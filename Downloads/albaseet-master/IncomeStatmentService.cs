using Compound.CoreOne.Contracts.Reports.Accounting;
using Compound.CoreOne.Models.Dtos.Reports.Accounting;
using Microsoft.AspNetCore.Http;
using Shared.CoreOne.Contracts.Accounts;
using Shared.CoreOne.Contracts.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Compound.Service.Services.Reports.Accounting
{
    public class IncomeStatmentService : IincomeStatmentservice
    {
        private readonly IAccountService _accountService;
        private readonly IJournalHeaderService _journalHeaderService;
        private readonly IJournalDetailService _journalDetailService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IncomeStatmentService(
            IAccountService accountService,
            IJournalHeaderService journalHeaderService,
            IJournalDetailService journalDetailService,
            IHttpContextAccessor httpContextAccessor)
        {
            _accountService = accountService;
            _journalHeaderService = journalHeaderService;
            _journalDetailService = journalDetailService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<IncomeStatementDto>> GetList(DateTime fromDate, DateTime toDate, int storeId)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var data = await (
                from account in _accountService.GetAll().Where(x => x.CompanyId == storeId)
                join journalHeader in _journalHeaderService.GetAll().Where(x => x.StoreId == storeId && x.TicketDate >= fromDate && x.TicketDate <= toDate)
                    on account.AccountId equals journalHeader.AccountId
                join journalDetail in _journalDetailService.GetAll().Where(x => x.JournalHeaderId == journalHeader.JournalHeaderId)
                    on account.AccountId equals journalDetail.AccountId into journalDetails
                from journalDetail in journalDetails.DefaultIfEmpty()
                group journalDetail by new
                {
                    account.AccountId,
                    account.AccountNameAr,
                    account.AccountNameEn,
                    account.AccountCode,
                    account.AccountLevel,
                    account.MainAccountId
                } into g
                select new IncomeStatementDto
                {
                    AccountId = g.Key.AccountId,
                    AccountName = language == LanguageData.LanguageCode.Arabic ? g.Key.AccountNameAr : g.Key.AccountNameEn,
                    CreditValue = g.Sum(x => x?.CreditValue ?? 0),
                    DebitValue = g.Sum(x => x?.DebitValue ?? 0),
                    AccountCode = g.Key.AccountCode,
                    AccountLevel = g.Key.AccountLevel,
                    MainAccountid = g.Key.MainAccountId
                }).ToListAsync();

            return data;
        }
    }
}
