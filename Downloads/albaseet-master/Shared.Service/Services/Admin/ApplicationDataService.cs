using Shared.CoreOne.Contracts.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne.Contracts.Approval;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Admin;
using Shared.Helper.Models.StaticData;
using Microsoft.AspNetCore.Http;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using System.Reflection.PortableExecutable;
using Microsoft.Extensions.Configuration;
using Shared.Helper.Identity;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Accounts;
using Shared.CoreOne.Contracts.Basics;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using static Shared.CoreOne.Models.StaticData.StaticData;

namespace Shared.Service.Services.Admin
{
	public class ApplicationDataService : IApplicationDataService
	{
		private readonly ICompanyService _companyService;
		private readonly IBranchService _branchService;
		private readonly IStoreService _storeService;
		private readonly IApproveService _approveService;
		private readonly IApproveStepService _approveStepService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IAccountService _accountService;
		private readonly ICountryService _countryService;
		private readonly IConfiguration _configuration;

		public ApplicationDataService(ICompanyService companyService, IBranchService branchService, IStoreService storeService, IApproveService approveService, IApproveStepService approveStepService, IHttpContextAccessor httpContextAccessor,IAccountService accountService,ICountryService countryService,IConfiguration configuration)
		{
			_companyService = companyService;
			_branchService = branchService;
			_storeService = storeService;
			_approveService = approveService;
			_approveStepService = approveStepService;
			_httpContextAccessor = httpContextAccessor;
			_accountService = accountService;
			_countryService = countryService;
			_configuration = configuration;
		}
		public async Task<ApplicationValidationDataDto> GetApplicationValidationData()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var businesses = await _companyService.GetAll().AsQueryable().Select(x => new ApplicationBusinessDto()
			{
				ApplicationId = ApplicationData.ApplicationId,
				BusinessId = x.CompanyId,
				BusinessName = language == LanguageCode.Arabic ? x.CompanyNameAr : x.CompanyNameEn

			}).ToListAsync();

			var branches = await _branchService.GetAll().AsQueryable().Select(x => new ApplicationBranchDto()
			{
				ApplicationId = ApplicationData.ApplicationId,
				BranchId = x.BranchId,
				BranchName = language == LanguageCode.Arabic ? x.BranchNameAr : x.BranchNameEn

			}).ToListAsync();


			var stores = await _storeService.GetAll().AsQueryable().Select(x => new ApplicationStoreDto()
			{
				ApplicationId = ApplicationData.ApplicationId,
				StoreId = x.StoreId,
				StoreName = language == LanguageCode.Arabic ? x.StoreNameAr : x.StoreNameEn

			}).ToListAsync();

			var steps =
				await (from approve in _approveService.GetAll()
					   from approveStep in _approveStepService.GetAll().Where(x => x.ApproveId == approve.ApproveId)
					   select new ApplicationApproveStepDto
					   {
						   ApplicationId = ApplicationData.ApplicationId,
						   ApproveStepId = approveStep.ApproveStepId,
						   ApproveStepName = language == LanguageCode.Arabic ? $"{approve.ApproveNameAr} - {approveStep.StepNameAr}" : $"{approve.ApproveNameEn} - {approveStep.StepNameEn}",
						   BusinessId = approve.CompanyId
					   }).ToListAsync();

			return new ApplicationValidationDataDto()
			{
				ApplicationBusinesses = businesses,
				ApplicationBranches = branches,
				ApplicationStores = stores,
				ApplicationApproveSteps = steps
			};
		}

		public async Task<bool> CreateFirstCompany(CompanyIdentityDto model)
		{
			var currencyId = await _countryService.GetAllCountries().Where(x=>x.CountryId == model.CountryId).Select(s=>s.CurrencyId).FirstOrDefaultAsync();
			var modelDb = new CompanyDto()
			{
				CompanyNameAr = "نشاط 1",
				CompanyNameEn = "Business 1",
				CurrencyId = currencyId ?? CurrencyData.SaudiRiyal,
				IsActive = true,
				Address = model.Address,
				Email = model.Email,
				Phone = model.PhoneNumber,
				TaxCode = model.VatNumber,
			};
			var response = await _companyService.SaveCompany(modelDb,true);
			if (response.Success)
			{
				await _accountService.CreateMainAccounts(response.Id, modelDb.CurrencyId);
				modelDb.CompanyNameAr = "فرع 1"; model.CompanyNameEn = "Branch 1";
				var branch = await _branchService.CreateBranchFromCompany(modelDb, response.Id);
				modelDb.CompanyNameAr = "موقع 1"; model.CompanyNameEn = "Store 1";
				await _storeService.CreateStoreFromBranch(modelDb, branch!.Id, true);
			}
			return true;
		}

		public string GetStructureDatabase()
		{
			return _configuration.GetSection("Application:StructureDatabase").Value ?? "";
		}

		public SshOptionsDto GetSshOptions()
		{
			var host = _configuration.GetSection("SSH:Host").Value;
			var port = _configuration.GetSection("SSH:Port").Value;
			var username = _configuration.GetSection("SSH:Username").Value;
			var password = _configuration.GetSection("SSH:Password").Value;
			return new SshOptionsDto()
			{
				Host = host,
				Port = Convert.ToInt32(port),
				Username = username,
				Password = password
			};
		}
	}
}
