using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Accounts;
using Shared.CoreOne.Models.Dtos.ViewModels.Accounts;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.CoreOne.Models.Dtos.ViewModels.Items;
using Shared.Helper.Models.Dtos;

namespace Shared.CoreOne.Contracts.Accounts
{
	public interface IAccountService : IBaseService<Account>
	{
		IQueryable<AccountTreeDto> GetAccountsTree(int companyId);
		Task<List<AccountTreeDto>> GetAccountsSimpleTreeByCompanyId(int companyId,int? mainAccountId);
		Task<List<AccountTreeDto>> GetAccountsSimpleTreeByStoreId(int companyId, int? mainAccountId);
		IQueryable<AccountNameDto> GetMainAccountsByAccountTypeId(int accountTypeId);
		Task<AccountNameDto> GetMainAccountIdByAccountType(int accountTypeId);
		Task<string> GetNextAccountCode(int companyId,int mainAccountId, bool isMainAccount);
		Task<List<AccountAutoCompleteDto>> GetMainAccountsByAccountCode(int companyId,string accountCode);
		Task<List<AccountAutoCompleteDto>> GetMainAccountsByAccountName(int companyId, string accountName);
		Task<List<AccountAutoCompleteDto>> GetIndividualAccountsByAccountCode(int storeId, string accountCode);
		Task<List<AccountAutoCompleteDto>> GetIndividualAccountsByAccountName(int storeId, string accountName);
		IQueryable<AccountNameDto> GetIndividualAccountsByCompanyId(int companyId);
		IQueryable<AccountNameDto> GetIndividualAccounts(int storeId);
		Task<ResponseDto> CreateMainAccounts(int companyId, short currencyId);
		IQueryable<AccountDto> GetAllAccounts();
		IQueryable<AccountDto> GetCompanyAccounts(int companyId);
		Task<AccountDto> GetAccountByAccountId(int accountId);
		Task<AccountDto> GetAccountByAccountCode(int companyId , string accountCode);
		List<RequestChangesDto> GetAccountRequestChanges(AccountDto oldItem, AccountDto newItem);
		Task<AccountDto> GetRootAccountByCatgoryId(int companyId, byte accountCategoryId);
		Task<AccountDto> GetFractionalApproximationDifferenceAccount(int companyId);
		Task<AccountDto> GetAllowedDiscountAccount(int companyId);
		Task<AccountDto> GetFractionalApproximationDifferenceAccountByStoreId(int storeId);
		Task<AccountDto> GetAllowedDiscountAccountByStoreId(int storeId);
		Task<ResponseDto> SaveAccount(AccountDto account);
		Task<FixedAssetAccountReturnDto> SaveFixedAssetAccount(FixedAssetAccountDto account);
		Task<ResponseDto> DeleteAccount(Account account);
		Task<List<AccountAutoCompleteDto>> GetTreeList(int accountId);
		Task<List<int>> RetrievingSubAccounts(int mainAccountId, int companyId);
		Task<string?> GetAccountTreeName(int accountId);
	}
}
