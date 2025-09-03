using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Dtos.ViewModels.Accounts;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.Helper.Models.Dtos;

namespace Shared.CoreOne.Contracts.Accounts
{
	public interface IAccountEntityService
	{
		IQueryable<AccountEntityDto> GetEntities();
		Task<AccountEntityDto> GetAccountEntityByAccountId(int accountId);
		Task<int> GetClientIdByAccountId(int accountId);
		Task<int> GetSupplierIdByAccountId(int accountId);
		Task<int> GetBankIdByAccountId(int accountId);
		Task<ClientDto> GetClientByAccountId(int accountId);
		Task<SupplierDto> GetSupplierByAccountId(int accountId);
		Task<BankDto> GetBankByAccountId(int accountId);
		Task<ResponseDto> CreateAccountFromSupplier(SupplierDto supplier);
		Task<ResponseDto> CreateAccountFromClient(ClientDto client);
		Task<ResponseDto> CreateAccountFromBank(BankDto bank);
		Task<ResponseDto> SaveAccount(AccountDto model);
		Task<ResponseDto> DeleteAccount(int accountId);
        Task<ResponseDto> SaveSupplier(SupplierDto supplier);
        Task<ResponseDto> SaveClient(ClientDto client);
        Task<ResponseDto> SaveBank(BankDto bank);
        Task<ResponseDto> DeleteSupplier(int supplierId);
        Task<ResponseDto> DeleteClient(int clientId);
        Task<ResponseDto> DeleteBank(int bankId);
    }
}
