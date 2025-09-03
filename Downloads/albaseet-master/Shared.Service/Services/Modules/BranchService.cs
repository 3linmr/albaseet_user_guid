using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Domain.Basics;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Basics;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Shared;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Helper.Models.Dtos;
using Shared.Helper.Models.StaticData;
using Shared.Service.Validators;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Shared.Service.Services.Modules
{
	public class BranchService : BaseService<Branch>, IBranchService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IStringLocalizer<BranchService> _localizer;
		private readonly ICompanyService _companyService;
		private readonly ICurrencyService _currencyService;

		public BranchService(IRepository<Branch> repository,IHttpContextAccessor httpContextAccessor,IStringLocalizer<BranchService> localizer,ICompanyService companyService,ICurrencyService currencyService) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
			_localizer = localizer;
			_companyService = companyService;
			_currencyService = currencyService;
		}

		public IQueryable<BranchDto> GetAllBranches()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var data =
				from branch in _repository.GetAll()
				from company in _companyService.GetAll().Where(x => x.CompanyId == branch.CompanyId)
				select new BranchDto()
				{
					BranchId = branch.BranchId,
					CompanyId = branch.CompanyId,
					CompanyName = language == LanguageCode.Arabic ? company.CompanyNameAr : company.CompanyNameEn,
					BranchAddress = branch.BranchAddress,
					BranchEmail = branch.BranchEmail,
					BranchNameAr = branch.BranchNameAr,
					BranchNameEn = branch.BranchNameEn,
					BranchPhone = branch.BranchPhone,
					BranchWebsite = branch.BranchWebsite,
					BranchWhatsApp = branch.BranchWhatsApp,
					IsActive = branch.IsActive,
					InActiveReasons = branch.InActiveReasons,
					ResponsibleAddress = branch.ResponsibleAddress,
					ResponsibleEmail = branch.ResponsibleEmail,
					ResponsibleNameAr = branch.ResponsibleNameAr,
					ResponsibleNameEn = branch.ResponsibleNameEn,
					ResponsiblePhone = branch.ResponsiblePhone,
					ResponsibleWhatsApp = branch.ResponsibleWhatsApp,
					IsActiveName = branch.IsActive ? _localizer["Active"].Value : _localizer["InActive"].Value,
				};
			return data;
		}

		public IQueryable<BranchDropDownDto> GetAllBranchesDropDown()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var data = GetAllBranches().Where(x=>x.IsActive).Select(x => new BranchDropDownDto()
			{
				BranchId = x.BranchId,
				CompanyId = x.CompanyId,
				BranchName = language == LanguageCode.Arabic ? x.BranchNameAr : x.BranchNameEn
			}).OrderBy(x => x.BranchName);
			return data;
		}

        public async Task<List<BranchDropDownDto>> GetUserBranchesDropDown()
        {
            var userBranches = await _httpContextAccessor.GetUserBranches();
            var branches = await GetAllBranchesDropDown().Where(x => userBranches.Contains(x.BranchId)).ToListAsync();
            return branches;
        }

        public IQueryable<BranchDropDownDto> GetBranchesByCompanyIdDropDown(int companyId)
		{
			return GetAllBranchesDropDown().Where(x => x.CompanyId == companyId);
		}

		public IQueryable<NameDto> GetBranchesDropDownForAdmin(int companyId)
		{
			return GetAllBranchesDropDown().Where(x => x.CompanyId == companyId).Select(x=> new NameDto()
			{
				Id = x.BranchId,
				Name = x.BranchName
			});
		}

		public Task<BranchDto?> GetBranchById(int id)
		{
			return GetAllBranches().FirstOrDefaultAsync(x => x.BranchId == id);
		}

		public async Task<ResponseDto?> CreateBranchFromCompany(CompanyDto company, int companyId)
		{
			var branch = new BranchDto()
			{
				CompanyId = companyId,
				CurrencyId = company.CurrencyId,
				BranchNameAr = company.CompanyNameAr,
				BranchNameEn = company.CompanyNameEn,
				IsActive = true
			};
			return await SaveBranch(branch);
		}

		public async Task<ResponseDto> SaveBranch(BranchDto branch)
		{
			var branchExist = await IsBranchExist(branch.BranchId, branch.BranchNameAr, branch.BranchNameEn);
			if (branchExist.Success)
			{
				return new ResponseDto() { Id = branchExist.Id, Success = false, Message = _localizer["BranchAlreadyExist"] };
			}
			else
			{
				if (branch.BranchId == 0)
				{
					return await CreateBranch(branch);
				}
				else
				{
					return await UpdateBranch(branch);
				}
			}
		}

		public async Task<ResponseDto> IsBranchExist(int id, string? nameAr, string? nameEn)
		{
			var branch = await _repository.GetAll().FirstOrDefaultAsync(x => (x.BranchNameAr == nameAr || x.BranchNameEn == nameEn || x.BranchNameAr == nameEn || x.BranchNameEn == nameAr) && x.BranchId != id);
			if (branch != null)
			{
				return new ResponseDto() { Id = branch.BranchId, Success = true };
			}
			return new ResponseDto() { Id = 0, Success = false };
		}
		public async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.BranchId) + 1; } catch { id = 1; }
			return id;
		}

		public async Task<ResponseDto> CreateBranch(BranchDto branch)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var newBranch = new Branch()
			{
				BranchId = await GetNextId(),
				BranchNameAr = branch?.BranchNameAr?.Trim(),
				BranchNameEn = branch?.BranchNameEn?.Trim(),
				CompanyId = branch!.CompanyId,
				BranchAddress = branch.BranchAddress,
				BranchEmail = branch.BranchEmail,
				BranchPhone = branch.BranchPhone,
				BranchWebsite = branch.BranchWebsite,
				BranchWhatsApp = branch.BranchWhatsApp,
				IsActive = branch.IsActive,
				InActiveReasons = branch.InActiveReasons,
				ResponsibleAddress = branch.ResponsibleAddress,
				ResponsibleEmail = branch.ResponsibleEmail,
				ResponsibleNameAr = branch.ResponsibleNameAr,
				ResponsibleNameEn = branch.ResponsibleNameEn,
				ResponsiblePhone = branch.ResponsiblePhone,
				ResponsibleWhatsApp = branch.ResponsibleWhatsApp,
				CreatedAt = DateHelper.GetDateTimeNow(),
				UserNameCreated = await _httpContextAccessor!.GetUserName(),
				IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
				Hide = false
			};

			var branchValidator = await new BranchValidator(_localizer).ValidateAsync(newBranch);
			var validationResult = branchValidator.IsValid;
			if (validationResult)
			{
				await _repository.Insert(newBranch);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = newBranch.BranchId, Success = true, Message = _localizer["NewBranchSuccessMessage", ((language == LanguageCode.Arabic ? newBranch.BranchNameAr : newBranch.BranchNameEn) ?? ""), newBranch.BranchId] };
			}
			else
			{
				return new ResponseDto() { Id = newBranch.BranchId, Success = false, Message = branchValidator.ToString("~") };
			}
		}

		public async Task<ResponseDto> UpdateBranch(BranchDto branch)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var branchDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.BranchId == branch.BranchId);
			if (branchDb != null)
			{
				branchDb.BranchNameAr = branch.BranchNameAr?.Trim();
				branchDb.BranchNameEn = branch.BranchNameEn?.Trim();
				branchDb.CompanyId = branch!.CompanyId;
				branchDb.BranchAddress = branch.BranchAddress;
				branchDb.BranchEmail = branch.BranchEmail;
				branchDb.BranchPhone = branch.BranchPhone;
				branchDb.BranchWebsite = branch.BranchWebsite;
				branchDb.BranchWhatsApp = branch.BranchWhatsApp;
				branchDb.IsActive = branch.IsActive;
				branchDb.InActiveReasons = branch.InActiveReasons;
				branchDb.ResponsibleAddress = branch.ResponsibleAddress;
				branchDb.ResponsibleEmail = branch.ResponsibleEmail;
				branchDb.ResponsibleNameAr = branch.ResponsibleNameAr;
				branchDb.ResponsibleNameEn = branch.ResponsibleNameEn;
				branchDb.ResponsiblePhone = branch.ResponsiblePhone;
				branchDb.ResponsibleWhatsApp = branch.ResponsibleWhatsApp;
				branchDb.ModifiedAt = DateHelper.GetDateTimeNow();
				branchDb.UserNameModified = await _httpContextAccessor!.GetUserName();
				branchDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();

				var branchValidator = await new BranchValidator(_localizer).ValidateAsync(branchDb);
				var validationResult = branchValidator.IsValid;
				if (validationResult)
				{
					_repository.Update(branchDb);
					await _repository.SaveChanges();
					return new ResponseDto() { Id = branchDb.BranchId, Success = true, Message = _localizer["UpdateBranchSuccessMessage", ((language == LanguageCode.Arabic ? branchDb.BranchNameAr : branchDb.BranchNameEn) ?? "")] };
				}
				else
				{
					return new ResponseDto() { Id = 0, Success = false, Message = branchValidator.ToString("~") };
				}
			}
			return new ResponseDto() { Id = 0, Success = false, Message = _localizer["NoBranchFound"] };
		}

		public async Task<ResponseDto> DeleteBranch(int id)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var branch = await _repository.GetAll().FirstOrDefaultAsync(x => x.BranchId == id);
			if (branch != null)
			{
				_repository.Delete(branch);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = id, Success = true, Message = _localizer["DeleteBranchSuccessMessage", ((language == LanguageCode.Arabic ? branch.BranchNameAr : branch.BranchNameEn) ?? "")] };
			}
			return new ResponseDto() { Id = id, Success = false, Message = _localizer["NoBranchFound"] };
		}
	}
}
