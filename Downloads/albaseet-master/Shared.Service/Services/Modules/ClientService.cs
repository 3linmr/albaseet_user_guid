using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Accounts;
using Shared.CoreOne.Contracts.Basics;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Accounts;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Helper.Models.Dtos;
using Shared.Service.Validators;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using static Shared.Helper.Models.StaticData.LanguageData;

namespace Shared.Service.Services.Modules
{
	public class ClientService : BaseService<Client>, IClientService
	{
		private readonly ICountryService _countryService;
		private readonly IStateService _stateService;
		private readonly ICityService _cityService;
		private readonly IDistrictService _districtService;
		private readonly ICompanyService _companyService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IAccountService _accountService;
		private readonly IStringLocalizer<ClientService> _localizer;
		private readonly ISellerService _sellerService;
		private readonly IStoreService _storeService;
		private readonly IBranchService _branchService;

		private const string fullDelimiter = " - ";

		public ClientService(IRepository<Client> repository, ICountryService countryService, IStateService stateService, ICityService cityService, IDistrictService districtService, ICompanyService companyService, IHttpContextAccessor httpContextAccessor,IAccountService accountService, IStringLocalizer<ClientService> localizer, ISellerService sellerService, IStoreService storeService, IBranchService branchService) : base(repository)
		{
			_countryService = countryService;
			_stateService = stateService;
			_cityService = cityService;
			_districtService = districtService;
			_companyService = companyService;
			_httpContextAccessor = httpContextAccessor;
			_accountService = accountService;
			_localizer = localizer;
			_sellerService = sellerService;
			_storeService = storeService;
			_branchService = branchService;
		}

		public IQueryable<ClientDto> GetAllClients()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var data =
				from client in _repository.GetAll()
				from company in _companyService.GetAll().Where(x => x.CompanyId == client.CompanyId)
				from country in _countryService.GetAll().Where(x => x.CountryId == client.CountryId).DefaultIfEmpty()
				from state in _stateService.GetAll().Where(x => x.StateId == client.StateId).DefaultIfEmpty()
				from city in _cityService.GetAll().Where(x => x.CityId == client.CityId).DefaultIfEmpty()
				from district in _districtService.GetAll().Where(x => x.DistrictId == client.DistrictId).DefaultIfEmpty()
				from account in _accountService.GetAll().Where(x=>x.AccountId == client.AccountId).DefaultIfEmpty()
				from seller in _sellerService.GetAll().Where(x => x.SellerId == client.SellerId).DefaultIfEmpty()
				select new ClientDto()
				{
					ClientId = client.ClientId,
					ClientCode = client.ClientCode,
					ClientNameAr = client.ClientNameAr,
					ClientNameEn = client.ClientNameEn,
					CompanyId = client.CompanyId,
					ClientName = language == LanguageCode.Arabic ? client.ClientNameAr : client.ClientNameEn,
					CompanyName = language == LanguageCode.Arabic ? company.CompanyNameAr : company.CompanyNameEn,
					EmployeeId = client.EmployeeId,
					Address1 = client.Address1,
					Address2 = client.Address2,
					Address3 = client.Address3,
					Address4 = client.Address4,
					AdditionalNumber = client.AdditionalNumber,
					BuildingNumber = client.BuildingNumber,
					CityId = client.CityId,
					CommercialRegister = client.CommercialRegister,
					CountryId = client.CountryId,
					CountryCode = client.CountryCode,
					DistrictId = client.DistrictId,
					StateId = client.StateId,
					Street1 = client.Street1,
					Street2 = client.Street2,
					PostalCode = client.PostalCode,
					CountryName = country != null ? (language == LanguageCode.Arabic ? country.CountryNameAr : country.CountryNameEn) : "",
					StateName = state != null ? (language == LanguageCode.Arabic ? state.StateNameAr : state.StateNameEn) : "",
					CityName = city != null ? (language == LanguageCode.Arabic ? city.CityNameAr : city.CityNameEn) : "",
					DistrictName = district != null ? (language == LanguageCode.Arabic ? district.DistrictNameAr : district.DistrictNameEn) : "",
					ContractDate = client.ContractDate,
					IsCredit = client.IsCredit,
					IsActive = client.IsActive,
					InActiveReasons = client.InActiveReasons,
					IsActiveName = (bool)client.IsActive ? _localizer["Active"].Value : _localizer["InActive"].Value,
					TaxCode = client.TaxCode,
					CreditLimitDays = client.CreditLimitDays,
					DebitLimitDays = client.DebitLimitDays,
					SellerId = client.SellerId,
					SellerCode = seller != null ? seller.SellerCode : null,
					SellerName = seller != null ? (language == LanguageCode.Arabic ? seller.SellerNameAr : seller.SellerNameEn) : "",
					CreditLimitValues = client.CreditLimitValues,
					ModifiedAt = client.ModifiedAt,
					CreatedAt = client.CreatedAt,
					UserNameCreated = client.UserNameCreated,
					UserNameModified = client.UserNameModified,
					ArchiveHeaderId = client.ArchiveHeaderId,
					FirstResponsibleName = client.FirstResponsibleName,
					FirstResponsiblePhone = client.FirstResponsiblePhone,
					FirstResponsibleEmail = client.FirstResponsibleEmail,
					SecondResponsibleName = client.SecondResponsibleName,
					SecondResponsiblePhone = client.SecondResponsiblePhone,
					SecondResponsibleEmail = client.SecondResponsibleEmail,
					AccountId = client.AccountId,
					AccountCode = account != null ? account.AccountCode : null,
					AccountName = account != null ? (language == LanguageCode.Arabic ? account.AccountNameAr : account.AccountNameEn) + " (" + account.AccountCode + ")" : null
				};
			return data;
		}

		public IQueryable<ClientDto> GetClientsByCompanyId(int companyId)
		{
			return GetAllClients().Where(x => x.CompanyId == companyId);
		}

		public IQueryable<ClientDropDownDto> GetClientsDropDown()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var companyId = _httpContextAccessor.GetCurrentUserCompany();
			return GetAllClients().Where(x => x.IsActive && x.CompanyId == companyId ).Select(x => new ClientDropDownDto()
			{
				ClientId = x.ClientId,
				ClientCode = x.ClientCode,
				ClientName = language == LanguageCode.Arabic ? x.ClientNameAr : x.ClientNameEn,
				MobileNumber = $"{x.FirstResponsiblePhone} - {x.SecondResponsiblePhone}",
				AccountId = x.AccountId,
				AccountCode = x.AccountCode,
				AccountName = x.AccountName,
			}).OrderBy(x => x.ClientName);
		}

		public IQueryable<ClientDropDownDto> GetClientsByCompanyIdDropDown(int companyId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			return GetAllClients().Where(x => x.IsActive && x.CompanyId == companyId).Select(x => new ClientDropDownDto()
			{
				ClientId = x.ClientId,
				ClientCode = x.ClientCode,
				ClientName = language == LanguageCode.Arabic ? x.ClientNameAr : x.ClientNameEn,
				MobileNumber = $"{x.FirstResponsiblePhone} - {x.SecondResponsiblePhone}",
				AccountId = x.AccountId,
				AccountCode = x.AccountCode,
				AccountName = x.AccountName,
			}).OrderBy(x => x.ClientName);
		}

		public IQueryable<ClientDto> GetUserClients()
        {
			var companyId = _httpContextAccessor.GetCurrentUserCompany();
			return GetAllClients().Where(x => x.CompanyId == companyId);
        }

        public async Task<ClientDto?> GetClientById(int id)
		{
			var client = await GetAllClients().FirstOrDefaultAsync(x => x.ClientId == id);
			if (client == null) return null;

			client.FullAddress = GetClientFullAddressInternal(client);
			client.FullResponsibleName = GetClientFullResponsibleNameInternal(client);
			client.FullResponsiblePhone = GetClientFullResponsiblePhoneInternal(client);

            return client;
		}

		public async Task<ClientDto?> GetClientByAccountId(int id)
		{
			var client = await GetAllClients().FirstOrDefaultAsync(x => x.AccountId == id);
			if (client == null) return null;

			client.FullAddress = GetClientFullAddressInternal(client);
			client.FullResponsibleName = GetClientFullResponsibleNameInternal(client);
			client.FullResponsiblePhone = GetClientFullResponsiblePhoneInternal(client);

			return client;
		}

		public async Task<List<ClientAutoCompleteDto>> GetClientsAutoComplete(string term)
		{
			var companyId = _httpContextAccessor.GetCurrentUserCompany();
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			if (language == LanguageCode.Arabic)
			{
				return await _repository.GetAll().Where(x => x.CompanyId == companyId && x.IsActive && (x.ClientNameAr!.ToLower().Contains(term.Trim().ToLower()) || x.ClientCode.ToString().Contains(term.Trim().ToLower()))).Select(s => new ClientAutoCompleteDto
				{
					ClientId = s.ClientId,
					ClientCode = s.ClientCode,
					ClientName = $"{s.ClientCode} - {s.ClientNameAr}",
					ClientNameAr = s.ClientNameAr,
					ClientNameEn = s.ClientNameEn
				}).Take(10).ToListAsync();
			}
			else
			{
				return await _repository.GetAll().Where(x => x.CompanyId == companyId && x.IsActive && (x.ClientNameEn!.ToLower().Contains(term.Trim().ToLower()) || x.ClientCode.ToString().Contains(term.Trim().ToLower())) ).Select(s => new ClientAutoCompleteDto
				{
					ClientId = s.ClientId,
					ClientCode = s.ClientCode,
					ClientName = $"{s.ClientCode} - {s.ClientNameEn}",
					ClientNameAr = s.ClientNameAr,
					ClientNameEn = s.ClientNameEn
				}).Take(10).ToListAsync();
			}
		}

		public async Task<List<ClientAutoCompleteDto>> GetClientsAutoCompleteByStoreIds(string term, List<int> storeIds)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var companyIds = _storeService.GetAllStores().Where(x => storeIds.Contains(x.StoreId)).Select(x => x.CompanyId);

			if (language == LanguageCode.Arabic)
			{
				return await (from client in _repository.GetAll()
							  where companyIds.Contains(client.CompanyId) && client.IsActive && (client.ClientNameAr!.ToLower().Contains(term.Trim().ToLower()) || client.ClientCode.ToString().Contains(term.Trim().ToLower()))
							  select new ClientAutoCompleteDto
							  {
								  ClientId = client.ClientId,
								  ClientCode = client.ClientCode,
								  ClientName = $"{client.ClientCode} - {client.ClientNameAr}",
								  ClientNameAr = client.ClientNameAr,
								  ClientNameEn = client.ClientNameEn
							  }).Take(10).ToListAsync();
			}
			else
			{
				return await (from client in _repository.GetAll()
							  where companyIds.Contains(client.CompanyId) && client.IsActive && (client.ClientNameEn!.ToLower().Contains(term.Trim().ToLower()) || client.ClientCode.ToString().Contains(term.Trim().ToLower()))
							  select new ClientAutoCompleteDto
							  {
								  ClientId = client.ClientId,
								  ClientCode = client.ClientCode,
								  ClientName = $"{client.ClientCode} - {client.ClientNameEn}",
								  ClientNameAr = client.ClientNameAr,
								  ClientNameEn = client.ClientNameEn
							  }).Take(10).ToListAsync();
			}
		}

        public async Task<string> GetClientFullAddress(int id)
		{
			var client = await GetAllClients().Where(x => x.ClientId == id).Select(x => new ClientDto { CountryName = x.CountryName, StateName = x.StateName, CityName = x.CityName, DistrictName = x.DistrictName, Street1 = x.Street1, Street2 = x.Street2, Address1 = x.Address1, Address2 = x.Address2, Address3 = x.Address3, Address4 = x.Address4, BuildingNumber = x.BuildingNumber, PostalCode = x.PostalCode }).FirstOrDefaultAsync();
			if (client == null) return "";

            return GetClientFullAddressInternal(client);
        }

        public string GetClientFullAddressInternal(ClientDto client)
        {
            List<string> addressParts = [];

            if (!string.IsNullOrEmpty(client.CountryName)) addressParts.Add(client.CountryName);
            if (!string.IsNullOrEmpty(client.StateName)) addressParts.Add(client.StateName);
            if (!string.IsNullOrEmpty(client.CityName)) addressParts.Add(client.CityName);
            if (!string.IsNullOrEmpty(client.DistrictName)) addressParts.Add(client.DistrictName);
            if (!string.IsNullOrEmpty(client.Street1)) addressParts.Add(client.Street1);
            if (!string.IsNullOrEmpty(client.Street2)) addressParts.Add(client.Street2);
            if (!string.IsNullOrEmpty(client.Address1)) addressParts.Add(client.Address1);
            if (!string.IsNullOrEmpty(client.Address2)) addressParts.Add(client.Address2);
            if (!string.IsNullOrEmpty(client.Address3)) addressParts.Add(client.Address3);
            if (!string.IsNullOrEmpty(client.Address4)) addressParts.Add(client.Address4);
            if (!string.IsNullOrEmpty(client.BuildingNumber)) addressParts.Add(client.BuildingNumber);
            if (!string.IsNullOrEmpty(client.PostalCode)) addressParts.Add(client.PostalCode);

            return string.Join(fullDelimiter, addressParts.ToArray());
        }

		public async Task<string> GetClientFullResponsibleName(int id)
		{
            var client = await GetAllClients().Where(x => x.ClientId == id).Select(x => new ClientDto { FirstResponsibleName = x.FirstResponsibleName, SecondResponsibleName = x.SecondResponsibleName}).FirstOrDefaultAsync();
            if (client == null) return "";

            return GetClientFullResponsibleNameInternal(client);
        }

		public string GetClientFullResponsibleNameInternal(ClientDto client)
		{
			List<string> responsibleNameParts = [];

			if (!string.IsNullOrEmpty(client.FirstResponsibleName)) responsibleNameParts.Add(client.FirstResponsibleName);
			if (!string.IsNullOrEmpty(client.SecondResponsibleName)) responsibleNameParts.Add(client.SecondResponsibleName);

			return string.Join(fullDelimiter, responsibleNameParts.ToArray());
		}

        public async Task<string> GetClientFullResponsiblePhone(int id)
        {
            var client = await GetAllClients().Where(x => x.ClientId == id).Select(x => new ClientDto { FirstResponsiblePhone = x.FirstResponsiblePhone, SecondResponsiblePhone = x.SecondResponsiblePhone }).FirstOrDefaultAsync();
            if (client == null) return "";

            return GetClientFullResponsiblePhoneInternal(client);
        }

        public string GetClientFullResponsiblePhoneInternal(ClientDto client)
        {
            List<string> responsiblePhoneParts = [];

            if (!string.IsNullOrEmpty(client.FirstResponsiblePhone)) responsiblePhoneParts.Add(client.FirstResponsiblePhone);
            if (!string.IsNullOrEmpty(client.SecondResponsiblePhone)) responsiblePhoneParts.Add(client.SecondResponsiblePhone);

            return string.Join(fullDelimiter, responsiblePhoneParts.ToArray());
        }

        public async Task<ResponseDto> LinkWithClientAccount(AccountDto account, bool update)
		{
			if (update)
			{
				var clientDb = await GetClientById(account.ClientId.GetValueOrDefault());
				if (clientDb != null)
				{
					clientDb.ClientNameAr = account.AccountNameAr;
					clientDb.ClientNameEn = account.AccountNameEn;
					return await SaveClient(clientDb);
				}
			}
			else
			{
				if (account.CreateNewClient)
				{
					var client = new ClientDto() { AccountId = account.AccountId, ClientNameAr = account.AccountNameAr, ClientNameEn = account.AccountNameEn, ContractDate = DateTime.Today, CompanyId = account.CompanyId, IsActive = true };
					return await SaveClient(client);
				}
				else
				{
					if (account.ClientId != null)
					{
						var clientDb = await GetClientById(account.ClientId.GetValueOrDefault());
						if (clientDb != null)
						{
							var isClientNotLinkedBefore = await IsClientNotLinkedBefore(account.AccountId);
							if (isClientNotLinkedBefore.Success)
							{
								clientDb.AccountId = account.AccountId;
								return await SaveClient(clientDb);
							}
							else
							{
								return isClientNotLinkedBefore;
							}

						}
					}
				}
			}

			return new ResponseDto() { Success = false, Message = _localizer["NoClientFound"] };
		}

		public async Task<bool> UnLinkWithClientAccount(int accountId)
		{
			var client = await _repository.GetAll().FirstOrDefaultAsync(x => x.AccountId == accountId);
			if (client != null)
			{
				client.AccountId = null;
				_repository.Update(client);
				await _repository.SaveChanges();
				return true;
			}
			return false;
		}

		public async Task<ResponseDto> IsClientNotLinkedBefore(int? accountId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var client = await _repository.GetAll().FirstOrDefaultAsync(x => x.AccountId == accountId);
			if (client == null)
			{
				return new ResponseDto() { Success = true };
			}
			else
			{
				return new ResponseDto() { Success = false, Id = client.ClientId, Message = _localizer["ClientAlreadyLinked", ((language == LanguageCode.Arabic ? client.ClientNameAr : client.ClientNameEn) ?? ""), client.ClientId] };
			}
		}
		public async Task<ResponseDto> SaveClient(ClientDto client)
		{
			var clientExist = await IsClientExist(client.ClientId, client.ClientNameAr, client.ClientNameEn);
			if (clientExist.Success)
			{
				return new ResponseDto() { Id = clientExist.Id, Success = false, Message = _localizer["ClientAlreadyExist"] };
			}
			else
			{
				if (client.ClientId == 0)
				{
					return await CreateClient(client);
				}
				else
				{
					return await UpdateClient(client);
				}
			}
		}

		public async Task<ResponseDto> IsClientExist(int id, string? nameAr, string? nameEn)
		{
			var companyId = _httpContextAccessor.GetCurrentUserCompany();
			var client = await _repository.GetAll().FirstOrDefaultAsync(x => (x.ClientNameAr == nameAr || x.ClientNameEn == nameEn || x.ClientNameAr == nameEn || x.ClientNameEn == nameAr) && x.ClientId != id && x.CompanyId == companyId);
			if (client != null)
			{
				return new ResponseDto() { Id = client.ClientId, Success = true };
			}
			return new ResponseDto() { Id = 0, Success = false };
		}
		public async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.ClientId) + 1; } catch { id = 1; }
			return id;
		}

		public async Task<int> GetNextCode(int companyId)
		{
			int id = 1;
			try { id = await _repository.GetAll().Where(x => x.CompanyId == companyId).MaxAsync(a => a.ClientCode) + 1; } catch { id = 1; }
			return id;
		}

		public async Task<ResponseDto> CreateClient(ClientDto client)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var newClient = new Client()
			{
				ClientId = await GetNextId(),
				ClientCode = await GetNextCode(client.CompanyId),
				ClientNameAr = client.ClientNameAr,
				ClientNameEn = client.ClientNameEn,
				CompanyId = client.CompanyId,
				EmployeeId = client.EmployeeId,
				AccountId = client.AccountId,
				IsActive = client.IsActive,
				InActiveReasons = client.InActiveReasons,
				IsCredit = client.IsCredit,
				CountryId = client.CountryId,
				StateId = client.StateId,
				CityId = client.CityId,
				DistrictId = client.DistrictId,
				Address1 = client.Address1,
				Address2 = client.Address2,
				Address3 = client.Address3,
				Address4 = client.Address4,
				AdditionalNumber = client.AdditionalNumber,
				BuildingNumber = client.BuildingNumber,
				CommercialRegister = client.CommercialRegister,
				CountryCode = client.CountryCode,
				Street1 = client.Street1,
				Street2 = client.Street2,
				ContractDate = client.ContractDate,
				CreditLimitDays = client.CreditLimitDays,
				CreditLimitValues = client.CreditLimitValues,
                DebitLimitDays = client.DebitLimitDays,
                SellerId = client.SellerId,
                TaxCode = client.TaxCode,
				PostalCode = client.PostalCode,
				FirstResponsibleName = client.FirstResponsibleName,
				FirstResponsiblePhone = client.FirstResponsiblePhone,
				FirstResponsibleEmail = client.FirstResponsibleEmail,
				SecondResponsibleName = client.SecondResponsibleName,
				SecondResponsiblePhone = client.SecondResponsiblePhone,
				SecondResponsibleEmail = client.SecondResponsibleEmail,
				CreatedAt = DateHelper.GetDateTimeNow(),
				UserNameCreated = await _httpContextAccessor!.GetUserName(),
				IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
				Hide = false,
			};

			var clientValidator = await new ClientValidator(_localizer).ValidateAsync(newClient);
			var validationResult = clientValidator.IsValid;
			if (validationResult)
			{
				await _repository.Insert(newClient);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = newClient.ClientId, Success = true, Message = _localizer["NewClientSuccessMessage", ((language == LanguageCode.Arabic ? newClient.ClientNameAr : newClient.ClientNameEn) ?? ""), newClient.ClientCode] };
			}
			else
			{
				return new ResponseDto() { Id = newClient.ClientId, Success = false, Message = clientValidator.ToString("~") };
			}
		}

		public async Task<ResponseDto> UpdateClient(ClientDto client)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var clientDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.ClientId == client.ClientId);
			if (clientDb != null)
			{
				clientDb.ClientNameAr = client.ClientNameAr ?? clientDb.ClientNameAr;
				clientDb.ClientNameEn = client.ClientNameEn ?? clientDb.ClientNameEn;
				clientDb.AccountId = client.AccountId ?? clientDb.AccountId;
				clientDb.CompanyId = client.CompanyId;
				clientDb.IsActive = client.IsActive;
				clientDb.InActiveReasons = client.InActiveReasons;
				clientDb.IsCredit = client.IsCredit;
				clientDb.CountryId = client.CountryId;
				clientDb.StateId = client.StateId;
				clientDb.CityId = client.CityId;
				clientDb.DistrictId = client.DistrictId;
				clientDb.Address1 = client.Address1;
				clientDb.Address2 = client.Address2;
				clientDb.Address3 = client.Address3;
				clientDb.Address4 = client.Address4;
				clientDb.AdditionalNumber = client.AdditionalNumber;
				clientDb.BuildingNumber = client.BuildingNumber;
				clientDb.CommercialRegister = client.CommercialRegister;
				clientDb.CountryCode = client.CountryCode;
				clientDb.Street1 = client.Street1;
				clientDb.Street2 = client.Street2;
				clientDb.ContractDate = client.ContractDate;
				clientDb.CreditLimitDays = client.CreditLimitDays;
				clientDb.CreditLimitValues = client.CreditLimitValues;
                clientDb.DebitLimitDays = client.DebitLimitDays;
                clientDb.SellerId = client.SellerId;
				clientDb.EmployeeId = client.EmployeeId;
				clientDb.TaxCode = client.TaxCode;
				clientDb.PostalCode = client.PostalCode;
				clientDb.FirstResponsibleName = client.FirstResponsibleName;
				clientDb.FirstResponsiblePhone = client.FirstResponsiblePhone;
				clientDb.FirstResponsibleEmail = client.FirstResponsibleEmail;
				clientDb.SecondResponsibleName = client.SecondResponsibleName;
				clientDb.SecondResponsiblePhone = client.SecondResponsiblePhone;
				clientDb.SecondResponsibleEmail = client.SecondResponsibleEmail;
				clientDb.ModifiedAt = DateHelper.GetDateTimeNow();
				clientDb.UserNameModified = await _httpContextAccessor!.GetUserName();
				clientDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();

				var clientValidator = await new ClientValidator(_localizer).ValidateAsync(clientDb);
				var validationResult = clientValidator.IsValid;
				if (validationResult)
				{
					_repository.Update(clientDb);
					await _repository.SaveChanges();
					return new ResponseDto() { Id = clientDb.ClientId, Success = true, Message = _localizer["UpdateClientSuccessMessage", ((language == LanguageCode.Arabic ? clientDb.ClientNameAr : clientDb.ClientNameEn) ?? "")] };
				}
				else
				{
					return new ResponseDto() { Id = 0, Success = false, Message = clientValidator.ToString("~") };
				}
			}
			return new ResponseDto() { Id = 0, Success = false, Message = _localizer["NoClientFound"] };
		}

		public async Task<ResponseDto> DeleteClient(int id)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var client = await _repository.GetAll().FirstOrDefaultAsync(x => x.ClientId == id);
			if (client != null)
			{
				var isAccountExist = client.AccountId != null;
				if (isAccountExist)
				{
					return new ResponseDto() { Id = id, Success = false, Message = _localizer["ClientIsLinkedWithAccount"] };
				}
				else
				{
					_repository.Delete(client);
					await _repository.SaveChanges();
					return new ResponseDto() { Id = id, Success = true, Message = _localizer["DeleteClientSuccessMessage", ((language == LanguageCode.Arabic ? client.ClientNameAr : client.ClientNameEn) ?? "")] };
				}
			}
			return new ResponseDto() { Id = id, Success = false, Message = _localizer["NoClientFound"] };
		}
	}
}
