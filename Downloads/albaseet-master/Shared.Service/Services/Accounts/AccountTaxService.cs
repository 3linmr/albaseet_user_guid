using Shared.CoreOne.Contracts.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Contracts.Taxes;
using Shared.CoreOne.Models.Dtos.ViewModels.Accounts;
using Shared.CoreOne.Models.Domain.Modules;

namespace Shared.Service.Services.Accounts
{
	public class AccountTaxService : IAccountTaxService
	{
		private readonly IAccountService _accountService;
		private readonly ITaxService _taxService;
		private readonly ITaxPercentService _taxPercentService;
		private readonly ICurrencyService _currencyService;
		private readonly ICompanyService _companyService;
		private readonly IBranchService _branchService;
		private readonly IStoreService _storeService;

		public AccountTaxService(IAccountService accountService,ITaxService taxService,ITaxPercentService taxPercentService,ICurrencyService currencyService,ICompanyService companyService,IBranchService branchService,IStoreService storeService)
		{
			_accountService = accountService;
			_taxService = taxService;
			_taxPercentService = taxPercentService;
			_currencyService = currencyService;
			_companyService = companyService;
			_branchService = branchService;
			_storeService = storeService;
		}
		public async Task<AccountTaxDto> GetAccountTax(int storeId, int taxId,bool isDebit)
		{
			var companyData =
				from company in _companyService.GetAll()
				from branch in _branchService.GetAll().Where(x => x.CompanyId == company.CompanyId)
				from store in _storeService.GetAll().Where(x => x.BranchId == branch.BranchId && x.StoreId == storeId)
				select company;
			var data =
				 await (from tax in _taxService.GetAll().Where(x => x.TaxId == taxId)
						from taxPercent in _taxPercentService.GetAll().Where(x => x.TaxId == taxId && x.FromDate <= DateTime.Today)
							.OrderByDescending(x => x.FromDate).Take(1)
						from companyTax in companyData.Where(x=>x.CompanyId == tax.CompanyId)
						from account in _accountService.GetAll().Where(x => x.AccountId == (isDebit ? tax.DrAccount : tax.CrAccount)).DefaultIfEmpty()
						select new AccountTaxDto
						{
							TaxPercent = taxPercent.Percent,
							TaxTypeId = tax.TaxTypeId,
							AccountId = account != null ? account.AccountId : null
						}).FirstOrDefaultAsync();
			return data ?? new AccountTaxDto();
		}
	}
}
