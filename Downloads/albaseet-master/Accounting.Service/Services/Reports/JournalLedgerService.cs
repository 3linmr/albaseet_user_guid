using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accounting.CoreOne.Contracts.Reports;
using Accounting.CoreOne.Models.Dtos.Reports;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne.Contracts.Accounts;
using Shared.CoreOne.Contracts.Journal;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.Helper.Identity;
using Shared.Helper.Models.StaticData;
using LanguageData = Shared.CoreOne.Models.StaticData.LanguageData;

namespace Accounting.Service.Services.Reports
{
	public class JournalLedgerService : IJournalLedgerService
	{
		private readonly IJournalHeaderService _journalHeaderService;
		private readonly IJournalDetailService _journalDetailService;
		private readonly IAccountService _accountService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IPaymentMethodService _paymentMethodService;

		public JournalLedgerService(IJournalHeaderService journalHeaderService,IJournalDetailService journalDetailService,IAccountService accountService,IHttpContextAccessor httpContextAccessor,IPaymentMethodService paymentMethodService)
		{
			_journalHeaderService = journalHeaderService;
			_journalDetailService = journalDetailService;
			_accountService = accountService;
			_httpContextAccessor = httpContextAccessor;
			_paymentMethodService = paymentMethodService;
		}

		public async Task<List<LedgerDto>> GetList(DateTime fromDate, DateTime toDate, int storeId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var data = await 
				 (from journalHeader in _journalHeaderService.GetAll().Where(x=>x.StoreId == storeId && x.TicketDate >= fromDate && x.TicketDate <= toDate)
				from journalDetail in _journalDetailService.GetAll().Where(x=>x.JournalHeaderId == journalHeader.JournalHeaderId).DefaultIfEmpty()
				from account in _accountService.GetAll().Where(x=>x.AccountId == journalDetail.AccountId).DefaultIfEmpty()
					select new LedgerDto
					{
						Id = journalHeader.JournalHeaderId,
						AccountNameAr = account != null ? account.AccountNameAr:"",
						AccountName = language == LanguageData.LanguageCode.Arabic ? account.AccountNameAr : account.AccountNameEn
					}).ToListAsync();


			var data11 =
				from paymentMethod in _paymentMethodService.GetAll().Where(x => x.PaymentTypeId == 1)
				from account in _accountService.GetAll().Where(x => x.AccountId == paymentMethod.PaymentAccountId)
				from journalDetail in _journalDetailService.GetAll().Where(x => x.AccountId == account.AccountId)
				group new { paymentMethod, account, journalDetail } by new { paymentMethod.PaymentMethodId, account.AccountId }
			into g
				select new
				{
					//Name = paymentMethod.
					value = g.Sum(x => x.journalDetail.DebitValue)
				};


			//var data2 =
			//	(from journalHeader in _journalHeaderService.GetAll().Where(x => x.StoreId == storeId && x.TicketDate >= fromDate && x.TicketDate <= toDate)
			//		from journalDetail in _journalDetailService.GetAll().Where(x => x.JournalHeaderId == journalHeader.JournalHeaderId)
			//		select new LedgerDto
			//		{
			//			Id = journalHeader.JournalHeaderId,
			//			Name = journalDetail.EntityEmail
			//		});

			//var data3 = 


			return data;
		}
	}
}
