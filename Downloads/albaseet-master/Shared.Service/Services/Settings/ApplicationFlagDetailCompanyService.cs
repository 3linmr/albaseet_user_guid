using Shared.CoreOne.Contracts.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Shared.CoreOne;
using Shared.CoreOne.Models.Domain.Settings;
using Shared.CoreOne.Models.Dtos.ViewModels.Settings;
using Shared.Helper.Models.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Identity;
using Shared.Helper.Logic;


namespace Shared.Service.Services.Settings
{
	public class ApplicationFlagDetailCompanyService : BaseService<ApplicationFlagDetailCompany>, IApplicationFlagDetailCompanyService
	{
		private readonly IStringLocalizer<ApplicationFlagDetailCompanyService> _localizer;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly ICompanyService _companyService;
		private readonly IBranchService _branchService;
		private readonly IStoreService _storeService;
		private readonly IApplicationFlagDetailImageService _applicationFlagDetailImageService;

		public ApplicationFlagDetailCompanyService(IRepository<ApplicationFlagDetailCompany> repository,IStringLocalizer<ApplicationFlagDetailCompanyService> localizer,IHttpContextAccessor httpContextAccessor,ICompanyService companyService,IBranchService branchService,IStoreService storeService,IApplicationFlagDetailImageService applicationFlagDetailImageService) : base(repository)
		{
			_localizer = localizer;
			_httpContextAccessor = httpContextAccessor;
			_companyService = companyService;
			_branchService = branchService;
			_storeService = storeService;
			_applicationFlagDetailImageService = applicationFlagDetailImageService;
		}

		public async Task<string?> GetApplicationFlagDetailCompanyValue(int companyId, int applicationFlagDetailId)
		{
			return await _repository.GetAll().Where(x => x.ApplicationFlagDetailId == applicationFlagDetailId && x.CompanyId == companyId).Select(x => x.FlagValue).FirstOrDefaultAsync();
		}

		public async Task<string?> GetApplicationFlagDetailCompanyValueByStoreId(int storeId, int applicationFlagDetailId)
		{
			var companyId=
				 await (from store in _storeService.GetAll().Where(x=>x.StoreId == storeId)
				from branch in _branchService.GetAll().Where(x=>x.BranchId == store.BranchId)
				from company in _companyService.GetAll().Where(x=>x.CompanyId == branch.CompanyId)
				select company.CompanyId).FirstOrDefaultAsync();

			return await GetApplicationFlagDetailCompanyValue(companyId, applicationFlagDetailId);
		}

		public async Task<ResponseDto> SaveApplicationCompanySetting(SaveApplicationSettingDto model)
		{
			if (model.ApplicationFlagDetailId != 0)
			{
				var modelDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.ApplicationFlagDetailId == model.ApplicationFlagDetailId && x.CompanyId == model.CompanyId);
				if (modelDb != null)
				{
					modelDb.FlagValue =model.ApplicationFlagTypeId == ApplicationFlagTypeData.UploadImage? (model.File != null ? model.File.FileName : "") : model.FlagValue;
					modelDb.ModifiedAt = DateHelper.GetDateTimeNow();
					modelDb.UserNameModified = await _httpContextAccessor!.GetUserName();
					modelDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();
					_repository.Update(modelDb);
					await _repository.SaveChanges();
					if (model.ApplicationFlagTypeId == ApplicationFlagTypeData.UploadImage)
					{
						return await HandleBlobImages(modelDb.ApplicationFlagDetailCompanyId, model);
					}
					return new ResponseDto() { Id = modelDb.ApplicationFlagDetailCompanyId, Message = _localizer["SaveSuccess", (model.FlagName) ?? ""], Success = true };
				}
				else
				{
					var newModel = new ApplicationFlagDetailCompany()
					{
						CompanyId = model.CompanyId.GetValueOrDefault(),
						FlagValue = model.ApplicationFlagTypeId == ApplicationFlagTypeData.UploadImage ? (model.File != null ? model.File.FileName : "") : model.FlagValue,
						ApplicationFlagDetailId = model.ApplicationFlagDetailId,
						ApplicationFlagDetailCompanyId = await GetNextId(),
						CreatedAt = DateHelper.GetDateTimeNow(),
						UserNameCreated = await _httpContextAccessor!.GetUserName(),
						IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
						Hide = false
					};
					await _repository.Insert(newModel);
					await _repository.SaveChanges();

					await _repository.SaveChanges();
					if (model.ApplicationFlagTypeId == ApplicationFlagTypeData.UploadImage)
					{
						return await HandleBlobImages(newModel.ApplicationFlagDetailCompanyId, model);
					}
					return new ResponseDto() { Id = newModel.ApplicationFlagDetailCompanyId, Message = _localizer["SaveSuccess", (model.FlagName) ?? ""], Success = true };
				}
			}
			return new ResponseDto() { Id = model.ApplicationFlagDetailId, Message = _localizer["Error"], Success = false };
		}

		public async Task<ResponseDto> HandleBlobImages(int applicationFlagDetailCompanyId, SaveApplicationSettingDto model)
		{
			if (model.File != null)
			{
				var modelDb = new ApplicationFlagDetailImageDto
				{
					Image = model.File,
					FileType = model.File.ContentType,
					FileName = model.File.FileName,
					ApplicationFlagDetailCompanyId = applicationFlagDetailCompanyId
				};
				await _applicationFlagDetailImageService.SaveApplicationFlagImage(modelDb);
				return new ResponseDto() { Id = model.ApplicationFlagDetailId, Message = _localizer["ImageSaved", (model.FlagName) ?? ""], Success = true };
			}
			return new ResponseDto() { Id = model.ApplicationFlagDetailId, Message = _localizer["ImageNotFound", (model.FlagName) ?? ""], Success = true };
		}

		public async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.ApplicationFlagDetailCompanyId) + 1; } catch { id = 1; }
			return id;
		}
	}
}
