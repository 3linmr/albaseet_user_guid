using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Accounts;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.Helper.Models.Dtos;

namespace Shared.CoreOne.Contracts.Modules
{
	public interface IClientService : IBaseService<Client>
	{
		IQueryable<ClientDto> GetAllClients();
		IQueryable<ClientDto> GetClientsByCompanyId(int companyId);
		IQueryable<ClientDropDownDto> GetClientsDropDown();
		IQueryable<ClientDropDownDto> GetClientsByCompanyIdDropDown(int companyId);
		IQueryable<ClientDto> GetUserClients();
		Task<ClientDto?> GetClientById(int id);
		Task<ClientDto?> GetClientByAccountId(int id);
		Task<List<ClientAutoCompleteDto>> GetClientsAutoComplete(string term);
		Task<List<ClientAutoCompleteDto>> GetClientsAutoCompleteByStoreIds(string term, List<int> storeIds);
		Task<string> GetClientFullAddress(int id);
		Task<string> GetClientFullResponsiblePhone(int id);
        Task<string> GetClientFullResponsibleName(int id);
        Task<ResponseDto> LinkWithClientAccount(AccountDto account,bool update);
		Task<bool> UnLinkWithClientAccount(int accountId);
		Task<ResponseDto> SaveClient(ClientDto client);
		Task<ResponseDto> DeleteClient(int id);
	}
}
