using Compound.CoreOne.Contracts.Reports.Accounting;
using Compound.CoreOne.Models.Dtos.Reports.Accounting;
using Shared.CoreOne.Contracts.Journal;
using Shared.CoreOne.Contracts.Accounts;
using System;
using System.Linq;

namespace Compound.Service.Services.Reports.Accounting
{
    public class IncomeStatementService : IIncomeStatementService
    {
        private readonly IJournalHeaderService _journalHeaderService;
        private readonly IJournalDetailService _journalDetailService;

        public IncomeStatementService(IJournalHeaderService journalHeaderService, IJournalDetailService journalDetailService)
        {
            _journalHeaderService = journalHeaderService;
            _journalDetailService = journalDetailService;
        }

        public IQueryable<IncomeStatementDto> GetIncomeStatement(int storeId, DateTime? fromDate, DateTime? toDate)
        {
            var journalHeaders = _journalHeaderService.GetAll()
                .Where(x => x.StoreId == storeId && x.TicketDate >= fromDate && x.TicketDate <= toDate);

            var journalDetails = _journalDetailService.GetAll()
                .Where(x => journalHeaders.Select(jh => jh.JournalHeaderId).Contains(x.JournalHeaderId));

            var revenue = journalDetails
                .Where(x => x.AccountId == /* Revenue Account ID */)
                .Sum(x => x.CreditValue - x.DebitValue);

            var cogs = journalDetails
                .Where(x => x.AccountId == /* COGS Account ID */)
                .Sum(x => x.DebitValue - x.CreditValue);

            var operatingExpenses = journalDetails
                .Where(x => x.AccountId == /* Operating Expenses Account ID */)
                .Sum(x => x.DebitValue - x.CreditValue);

            var otherIncome = journalDetails
                .Where(x => x.AccountId == /* Other Income Account ID */)
                .Sum(x => x.CreditValue - x.DebitValue);

            var otherExpenses = journalDetails
                .Where(x => x.AccountId == /* Other Expenses Account ID */)
                .Sum(x => x.DebitValue - x.CreditValue);

            return new[] {
                new IncomeStatementDto
                {
                    Revenue = revenue,
                    CostOfGoodsSold = cogs,
                    OperatingExpenses = operatingExpenses,
                    OtherIncome = otherIncome,
                    OtherExpenses = otherExpenses
                }
            }.AsQueryable();
        }
    }
}
