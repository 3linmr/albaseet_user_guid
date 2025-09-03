using Microsoft.EntityFrameworkCore;
using Shared.CoreOne.Contracts.Accounts;
using Shared.CoreOne.Contracts.Journal;
using Shared.CoreOne.Models.Domain.Journal;
using Shared.CoreOne.Models.Dtos.ViewModels.Accounts;
using Shared.CoreOne.Models.Dtos.ViewModels.Journal;
using Shared.Service.Services.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Service.Services.Accounts
{
	public class ClientBalanceService : IClientBalanceService
	{
		private readonly IJournalDetailService _journalDetailService;
		private readonly IJournalHeaderService _journalHeaderService;

		public ClientBalanceService(IJournalDetailService journalDetailService, IJournalHeaderService journalHeaderService)
		{
			_journalDetailService = journalDetailService;
			_journalHeaderService = journalHeaderService;
		}

		public IQueryable<ClientBalanceDto> GetClientBalanceService(DateTime? toDate, bool includeStores)
		{
			return from JournalDetail in _journalDetailService.GetAll()
				   from JournalHeader in _journalHeaderService.GetAll().Where(x => x.JournalHeaderId == JournalDetail.JournalHeaderId).DefaultIfEmpty()
				   where toDate == null || JournalHeader.TicketDate <= toDate
				   group JournalDetail by new { JournalDetail.AccountId, StoreId = includeStores ? JournalHeader.StoreId : 0 } into g
				   select new ClientBalanceDto
				   {
					   AccountId = g.Key.AccountId,
					   StoreId = g.Key.StoreId,
					   Balance = g.Sum(x => x.DebitValue - x.CreditValue)
				   };

		}

		public async Task<decimal> GetClientBalanceByAccountId(int? accountId)
		{
			return await GetClientBalanceService(null, false)
				.Where(x => x.AccountId == accountId)
				.Select(x => x.Balance)
				.FirstOrDefaultAsync() ?? 0;
		}
	}
}
