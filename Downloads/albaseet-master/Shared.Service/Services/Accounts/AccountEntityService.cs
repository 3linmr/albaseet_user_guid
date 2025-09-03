using Shared.CoreOne.Contracts.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Accounts;
using Shared.Helper.Models.Dtos;
using Shared.CoreOne.Models.StaticData;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using static Shared.Helper.Models.StaticData.LanguageData;
using Shared.CoreOne.Models.Domain.Accounts;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.Helper.Identity;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Shared.Service.Services.Accounts
{
    public class AccountEntityService : IAccountEntityService
    {
        private readonly IClientService _clientService;
        private readonly ISupplierService _supplierService;
        private readonly IBankService _bankService;
        private readonly IAccountService _accountService;
        private readonly ICompanyService _companyService;
        private readonly IStringLocalizer<AccountEntityService> _localizer;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountEntityService(IClientService clientService, ISupplierService supplierService, IBankService bankService, IAccountService accountService, ICompanyService companyService, IStringLocalizer<AccountEntityService> localizer,IHttpContextAccessor httpContextAccessor)
        {
            _clientService = clientService;
            _supplierService = supplierService;
            _bankService = bankService;
            _accountService = accountService;
            _companyService = companyService;
            _localizer = localizer;
            _httpContextAccessor = httpContextAccessor;
        }

        public IQueryable<AccountEntityDto> GetEntities()
        {
	        var language = _httpContextAccessor.GetProgramCurrentLanguage();
            var clientsName = _localizer["Clients"].Value;
            var supplierName = _localizer["Suppliers"].Value;
            var banksName = _localizer["Banks"].Value;
            var companyId = _httpContextAccessor.GetCurrentUserCompany();

            var clients = _clientService.GetAll().Where(x=>x.CompanyId == companyId && x.IsActive).Select(x => new AccountEntityDto()
            {
                EntityTypeName = Convert.ToString(clientsName),
				EntityId = Convert.ToString(x.ClientId),
                EntityName = language == LanguageCode.Arabic ? x.ClientNameAr : x.ClientNameEn,
				EntityNameAr = Convert.ToString(x.ClientNameAr),
                EntityNameEn = Convert.ToString(x.ClientNameEn),
                TaxCode = Convert.ToString(x.TaxCode),
                EntityEmail = Convert.ToString(x.FirstResponsibleEmail)
            });
            var suppliers = _supplierService.GetAll().Where(x => x.CompanyId == companyId && x.IsActive).Select(x => new AccountEntityDto()
            {
                EntityTypeName = Convert.ToString(supplierName),
                EntityId = Convert.ToString(x.SupplierId),
                EntityName = language == LanguageCode.Arabic ? x.SupplierNameAr : x.SupplierNameEn,
				EntityNameAr = Convert.ToString(x.SupplierNameAr),
                EntityNameEn = Convert.ToString(x.SupplierNameEn),
                TaxCode = Convert.ToString(x.TaxCode),
                EntityEmail = Convert.ToString(x.FirstResponsibleEmail)
            });

            var banks = _bankService.GetAll().Where(x => x.CompanyId == companyId && x.IsActive).Select(x => new AccountEntityDto()
            {
                EntityTypeName = Convert.ToString(banksName),
                EntityId = Convert.ToString(x.BankId),
                EntityName = language == LanguageCode.Arabic ? x.BankNameAr : x.BankNameEn,
				EntityNameAr = Convert.ToString(x.BankNameAr),
                EntityNameEn = Convert.ToString(x.BankNameEn),
                TaxCode = Convert.ToString(x.TaxCode),
                EntityEmail = Convert.ToString(x.Email)
            });

            return clients.Union(suppliers).Union(banks);
        }

        public async Task<AccountEntityDto> GetAccountEntityByAccountId(int accountId)
        {
            if (accountId != 0)
            {
                var accountDb = await _accountService.GetAll().FirstOrDefaultAsync(x => x.AccountId == accountId);
                if (accountDb != null)
                {
                    if (accountDb.AccountTypeId == AccountTypeData.Clients)
                    {
                        var client = await GetClientByAccountId(accountId);
                        return new AccountEntityDto() { EntityNameAr = client.ClientNameAr, EntityNameEn = client.ClientNameEn, TaxCode = client.TaxCode, EntityEmail = client.FirstResponsibleEmail };
                    }
                    else if (accountDb.AccountTypeId == AccountTypeData.Suppliers)
                    {
                        var supplier = await GetSupplierByAccountId(accountId);
                        return new AccountEntityDto() { EntityNameAr = supplier.SupplierNameAr, EntityNameEn = supplier.SupplierNameEn, TaxCode = supplier.TaxCode, EntityEmail = supplier.FirstResponsibleEmail };
                    }
                    else if (accountDb.AccountTypeId == AccountTypeData.Banks)
                    {
                        var bank = await GetBankByAccountId(accountId);
                        return new AccountEntityDto() { EntityNameAr = bank.BankNameAr, EntityNameEn = bank.BankNameEn, TaxCode = bank.TaxCode, EntityEmail = bank.Email };
                    }
                    else
                    {
                        return new AccountEntityDto();
                    }
                }

            }
            return new AccountEntityDto();
        }

        public Task<int> GetClientIdByAccountId(int accountId)
        {
            return _clientService.GetAll().AsQueryable().Where(x => x.AccountId == accountId).Select(x => x.ClientId).FirstOrDefaultAsync();
        }

        public Task<int> GetSupplierIdByAccountId(int accountId)
        {
            return _supplierService.GetAll().AsQueryable().Where(x => x.AccountId == accountId).Select(x => x.SupplierId).FirstOrDefaultAsync();
        }

        public Task<int> GetBankIdByAccountId(int accountId)
        {
            return _bankService.GetAll().AsQueryable().Where(x => x.AccountId == accountId).Select(x => x.BankId).FirstOrDefaultAsync();
        }

        public async Task<ClientDto> GetClientByAccountId(int accountId)
        {
            return await _clientService.GetAllClients().FirstOrDefaultAsync(x => x.AccountId == accountId) ?? new ClientDto();
        }

        public async Task<SupplierDto> GetSupplierByAccountId(int accountId)
        {
            return await _supplierService.GetAllSuppliers().FirstOrDefaultAsync(x => x.AccountId == accountId) ?? new SupplierDto();
        }

        public async Task<BankDto> GetBankByAccountId(int accountId)
        {
            return await _bankService.GetAllBanks().FirstOrDefaultAsync(x => x.AccountId == accountId) ?? new BankDto();
        }

        public async Task<ResponseDto> CreateAccountFromSupplier(SupplierDto supplier)
        {
            var company = await _companyService.GetCompanyById(supplier.CompanyId);
            var nextAccountCode = await _accountService.GetNextAccountCode(supplier.CompanyId, supplier.MainAccountId.GetValueOrDefault(), false);
            var account = new AccountDto()
            {
                AccountCode = nextAccountCode,
                AccountNameAr = supplier.SupplierNameAr,
                AccountNameEn = supplier.SupplierNameEn,
                MainAccountId = supplier.MainAccountId,
                IsActive = true,
                AccountCategoryId = StaticData.AccountCategoryData.Liabilities,
                AccountTypeId = AccountTypeData.Suppliers,
                CompanyId = supplier.CompanyId,
                IsLastLevel = true,
                CurrencyId = company!.CurrencyId
            };
            return await _accountService.SaveAccount(account);
        }

        public async Task<ResponseDto> CreateAccountFromClient(ClientDto client)
        {
            var company = await _companyService.GetCompanyById(client.CompanyId);
            var nextAccountCode = await _accountService.GetNextAccountCode(client.CompanyId, client.MainAccountId.GetValueOrDefault(), false);
            var account = new AccountDto()
            {
                AccountCode = nextAccountCode,
                AccountNameAr = client.ClientNameAr,
                AccountNameEn = client.ClientNameEn,
                MainAccountId = client.MainAccountId,
                IsActive = true,
                AccountCategoryId = StaticData.AccountCategoryData.Assets,
                AccountTypeId = AccountTypeData.Clients,
                CompanyId = client.CompanyId,
                IsLastLevel = true,
                CurrencyId = company!.CurrencyId
            };
            return await _accountService.SaveAccount(account);
        }

        public async Task<ResponseDto> CreateAccountFromBank(BankDto bank)
        {
            var company = await _companyService.GetCompanyById(bank.CompanyId);
            var nextAccountCode = await _accountService.GetNextAccountCode(bank.CompanyId, bank.MainAccountId.GetValueOrDefault(), false);
            var account = new AccountDto()
            {
                AccountCode = nextAccountCode,
                AccountNameAr = bank.BankNameAr,
                AccountNameEn = bank.BankNameEn,
                MainAccountId = bank.MainAccountId,
                IsActive = true,
                AccountCategoryId = StaticData.AccountCategoryData.Assets,
                AccountTypeId = AccountTypeData.Banks,
                CompanyId = bank.CompanyId,
                IsLastLevel = true,
                CurrencyId = company!.CurrencyId
            };
            return await _accountService.SaveAccount(account);
        }

        public async Task<ResponseDto> SaveAccount(AccountDto model)
        {
            var isUpdate = model.AccountId > 0;
            var response = await _accountService.SaveAccount(model);

            if (response.Success)
            {
                model.AccountId = response.Id;
                if (model.AccountTypeId == AccountTypeData.Clients && !model.IsMainAccount)
                {
                    model.ClientId = isUpdate ? await GetClientIdByAccountId(model.AccountId) : model.ClientId;
                    var result = await _clientService.LinkWithClientAccount(model, isUpdate);
                    if (!result.Success)
                    {
                        response = result;
                    }
                }

                if (model.AccountTypeId == AccountTypeData.Suppliers && !model.IsMainAccount)
                {
                    model.SupplierId = isUpdate ? await GetSupplierIdByAccountId(model.AccountId) : model.SupplierId;
                    var result = await _supplierService.LinkWithSupplierAccount(model, isUpdate);
                    if (!result.Success)
                    {
                        response = result;
                    }
                }

                if (model.AccountTypeId == AccountTypeData.Banks && !model.IsMainAccount)
                {
                    model.BankId = isUpdate ? await GetBankIdByAccountId(model.AccountId) : model.BankId;
                    var result = await _bankService.LinkWithBankAccount(model, isUpdate);
                    if (!result.Success)
                    {
                        response = result;
                    }
                }
            }
            return response;
        }

        public async Task<ResponseDto> DeleteAccount(int accountId)
        {
            var accountDb = await _accountService.GetAll().FirstOrDefaultAsync(x => x.AccountId == accountId);

            if (accountDb != null)
            {
                if (accountDb.IsMainAccount)
                {
                    var accounts = await _accountService.RetrievingSubAccounts(accountDb.AccountId, accountDb.CompanyId);
                    var accountsToDeleted = await _accountService.GetAll().Where(x => accounts.Contains(x.AccountId)).ToListAsync();
                    foreach (var account in accountsToDeleted)
                    {
                        if (account.AccountTypeId == AccountTypeData.Clients)
                        {
                            await _clientService.UnLinkWithClientAccount(account.AccountId);
                        }
                        if (account.AccountTypeId == AccountTypeData.Suppliers)
                        {
                            await _supplierService.UnLinkWithSupplierAccount(account.AccountId);
                        }
                        if (account.AccountTypeId == AccountTypeData.Banks)
                        {
                            await _bankService.UnLinkWithBankAccount(account.AccountId);
                        }
                        await _accountService.DeleteAccount(account);
                    }
                    return new ResponseDto() { Id = 0, Success = true, Message = _localizer["DeleteAccountsSuccessMessage"] };
                }
                else
                {
                    if (accountDb.AccountTypeId == AccountTypeData.Clients)
                    {
                        await _clientService.UnLinkWithClientAccount(accountId);
                    }
                    if (accountDb.AccountTypeId == AccountTypeData.Suppliers)
                    {
                        await _supplierService.UnLinkWithSupplierAccount(accountId);
                    }
                    if (accountDb.AccountTypeId == AccountTypeData.Banks)
                    {
                        await _bankService.UnLinkWithBankAccount(accountId);
                    }
                    return await _accountService.DeleteAccount(accountDb);
                }
            }
            return new ResponseDto() { Id = accountId, Success = false, Message = _localizer["NoAccountFound"] };
        }

        public async Task<ResponseDto> SaveSupplier(SupplierDto supplier)
        {
            if (supplier.SupplierId == 0 && supplier.CreateNewAccount)
            {
                var result = await CreateAccountFromSupplier(supplier);
                if (result.Success)
                {
                    supplier.AccountId = result.Id;
                    return await _supplierService.SaveSupplier(supplier);
                }
                return result;
            }
            return await _supplierService.SaveSupplier(supplier);
        }

        public async Task<ResponseDto> SaveClient(ClientDto client)
        {

            if (client.ClientId == 0 && client.CreateNewAccount)
            {
                var result = await CreateAccountFromClient(client);
                if (result.Success)
                {
                    client.AccountId = result.Id;
                    return await _clientService.SaveClient(client);
                }
                return result;
            }
            return await _clientService.SaveClient(client);
        }

        public async Task<ResponseDto> SaveBank(BankDto bank)
        {
            if (bank.BankId == 0 && bank.CreateNewAccount)
            {
                var result = await CreateAccountFromBank(bank);
                if (result.Success)
                {
                    bank.AccountId = result.Id;
                    return await _bankService.SaveBank(bank);
                }
                return result;
            }
            return await _bankService.SaveBank(bank);
        }

        public async Task<ResponseDto> DeleteSupplier(int supplierId)
        {
            var supplier = await _supplierService.GetAll().Where(x => x.SupplierId == supplierId).AsNoTracking().FirstOrDefaultAsync();
            if (supplier != null)
            {
                if (supplier.AccountId != null)
                {
                    var result = await DeleteAccount(supplier.AccountId.GetValueOrDefault());
                    if (result.Success)
                    {
                        return await _supplierService.DeleteSupplier(supplierId);
                    }
                    return result;
                }
                return await _supplierService.DeleteSupplier(supplierId);
            }
            return new ResponseDto() { Message = _localizer["SupplierNotFound"] };
        }

        public async Task<ResponseDto> DeleteClient(int clientId)
        {
            var client = await _clientService.GetAll().Where(x => x.ClientId == clientId).AsNoTracking().FirstOrDefaultAsync();
            if (client != null)
            {
                if (client.AccountId != null)
                {
                    var result = await DeleteAccount(client.AccountId.GetValueOrDefault());
                    if (result.Success)
                    {
                        return await _clientService.DeleteClient(clientId);
                    }
                    return result;
                }
                return await _clientService.DeleteClient(clientId);
            }
            return new ResponseDto() { Message = _localizer["ClientNotFound"] };
        }

        public async Task<ResponseDto> DeleteBank(int bankId)
        {
            var bank = await _bankService.GetAll().Where(x => x.BankId == bankId).AsNoTracking().FirstOrDefaultAsync();
            if (bank != null)
            {
                if (bank.AccountId != null)
                {
                    var result = await DeleteBank(bank.AccountId.GetValueOrDefault());
                    if (result.Success)
                    {
                        return await _bankService.DeleteBank(bankId);
                    }
                    return result;
                }
                return await _bankService.DeleteBank(bankId);
            }
            return new ResponseDto() { Message = _localizer["BankNotFound"] };
        }
    }
}
