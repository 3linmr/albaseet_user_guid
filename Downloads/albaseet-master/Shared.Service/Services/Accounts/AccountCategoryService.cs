using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Accounts;
using Shared.CoreOne.Models.Domain.Accounts;
using Shared.CoreOne.Models.Dtos.ViewModels.Accounts;

namespace Shared.Service.Services.Accounts
{
	public class AccountCategoryService : BaseService<AccountCategory>, IAccountCategoryService
	{
		public AccountCategoryService(IRepository<AccountCategory> repository) : base(repository)
		{

		}

		public IQueryable<AccountCategoryDto> GetAccountCategories()
		{
			return _repository.GetAll().Select(x => new AccountCategoryDto()
			{
				AccountCategoryId = x.AccountCategoryId,
				AccountLedgerId = x.AccountLedgerId,
				AccountCategoryNameAr = x.AccountCategoryNameAr,
				AccountCategoryNameEn = x.AccountCategoryNameEn,
			});
		}

		public IQueryable<AccountTreeDto> GetAccountCategoriesTree()
		{
			return GetAccountCategories().Select(x => new AccountTreeDto()
			{
				AccountId = x.AccountCategoryId,
				AccountNameAr = x.AccountCategoryNameAr,
				AccountNameEn = x.AccountCategoryNameEn,
				IsMainAccount = x.MainAccountCategoryId == null,
				MainAccountId = x.MainAccountCategoryId ?? 0,
			});
		}

		public IQueryable<AccountCategoryDropDownDto> GetAccountCategoriesDropDown()
		{
			throw new NotImplementedException();
		}
	}
}
