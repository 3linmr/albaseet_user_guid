using Shared.CoreOne.Contracts.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Shared.CoreOne;
using Shared.CoreOne.Models.Domain.Accounts;
using Shared.CoreOne.Models.Dtos.ViewModels.Accounts;
using Shared.Helper.Identity;
using Shared.CoreOne.Models.Domain.Modules;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Shared.Service.Services.Accounts
{
	public class AccountLedgerService : BaseService<AccountLedger>, IAccountLedgerService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		public AccountLedgerService(IRepository<AccountLedger> repository,IHttpContextAccessor httpContextAccessor) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		public IQueryable<AccountLedgerDto> GetAccountLedger()
		{
			return _repository.GetAll().Select(x => new AccountLedgerDto()
			{
				AccountLedgerId = x.AccountLedgerId,
				AccountLedgerNameAr = x.AccountLedgerNameAr,
				AccountLedgerNameEn = x.AccountLedgerNameEn
			});
		}

		public IQueryable<AccountLedgerDropDownDto> GetAccountLedgerDropDown()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			return GetAccountLedger().Select(x => new AccountLedgerDropDownDto()
			{
				AccountLedgerId = x.AccountLedgerId,
				AccountLedgerName = language == LanguageCode.Arabic ? x.AccountLedgerNameAr : x.AccountLedgerNameEn
			});
		}
	}
}
