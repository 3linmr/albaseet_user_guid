using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Accounts;
using Shared.CoreOne.Models.Dtos.ViewModels.Basics;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.Helper.Models.Dtos;

namespace Shared.CoreOne.Contracts.Modules
{
	public interface IBankService : IBaseService<Bank>
	{
		IQueryable<BankDto> GetAllBanks();
		IQueryable<BankDto> GetUserBanks();
        IQueryable<BankDropDownDto> GetBanksDropDown();
		Task<BankDto?> GetBankById(int id);
		Task<List<BankAutoCompleteDto>> GetBanksAutoComplete(string term);
		Task<List<BankAutoCompleteDto>> GetBanksAutoCompleteByStoreIds(string term, List<int> storeIds);
		Task<ResponseDto> LinkWithBankAccount(AccountDto account, bool update);
		Task<bool> UnLinkWithBankAccount(int accountId);
		Task<ResponseDto> SaveBank(BankDto bank);
		Task<ResponseDto> DeleteBank(int id);
	}
}
