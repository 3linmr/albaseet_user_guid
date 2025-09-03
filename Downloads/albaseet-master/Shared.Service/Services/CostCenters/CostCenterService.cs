using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.CostCenters;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Contracts.Settings;
using Shared.CoreOne.Models.Domain.Accounts;
using Shared.CoreOne.Models.Domain.CostCenters;
using Shared.CoreOne.Models.Domain.Items;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Accounts;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.CoreOne.Models.Dtos.ViewModels.CostCenters;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Helper.Models.Dtos;
using Shared.Helper.Models.UserDetail;
using Shared.Service.Logic.Approval;
using Shared.Service.Logic.Tree;
using Shared.Service.Validators;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Shared.Service.Services.CostCenters
{
	public class CostCenterService : BaseService<CostCenter>, ICostCenterService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IApplicationSettingService _applicationSettingService;
		private readonly IStringLocalizer<CostCenterService> _localizer;
		private readonly IMenuNoteService _menuNoteService;
		private readonly ICompanyService _companyService;
		private readonly IBranchService _branchService;
		private readonly IStoreService _storeService;

		private List<int> _deletedCostCenters = new();
		private List<CostCenterAutoCompleteDto> _treeName = new();
		private List<CostCenter> _costCenters = new();


		public CostCenterService(IRepository<CostCenter> repository, IHttpContextAccessor httpContextAccessor, IApplicationSettingService applicationSettingService, IStringLocalizer<CostCenterService> localizer, IMenuNoteService menuNoteService,ICompanyService companyService,IBranchService branchService,IStoreService storeService) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
			_applicationSettingService = applicationSettingService;
			_localizer = localizer;
			_menuNoteService = menuNoteService;
			_companyService = companyService;
			_branchService = branchService;
			_storeService = storeService;
		}

		public IQueryable<CostCenterDropDownDto> GetCostCentersDropDown()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var companyId = _httpContextAccessor.GetCurrentUserCompany();
			return _repository.GetAll().Where(x => x.IsActive && x.CompanyId == companyId).Select(x => new CostCenterDropDownDto()
			{
				CostCenterId = x.CostCenterId,
				CostCenterName = language == LanguageCode.Arabic ? x.CostCenterNameAr : x.CostCenterNameEn
			});
		}

		public IQueryable<CostCenterDropDownDto> GetIndividualCostCentersByCompanyId(int companyId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			return _repository.GetAll().Where(x => x.IsActive && x.CompanyId == companyId && !x.IsMainCostCenter).Select(x => new CostCenterDropDownDto()
			{
				CostCenterId = x.CostCenterId,
				CostCenterName = language == LanguageCode.Arabic ? x.CostCenterNameAr : x.CostCenterNameEn
			});
		}

		public IQueryable<CostCenterDropDownDto> GetIndividualCostCentersByStoreId(int storeId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var data =
				(from costCenter in _repository.GetAll().Where(x => !x.IsMainCostCenter && x.IsActive)
					from company in _companyService.GetAll().Where(x => x.CompanyId == costCenter.CompanyId && x.IsActive)
					from branch in _branchService.GetAll().Where(x => x.CompanyId == company.CompanyId && x.IsActive)
					from store in _storeService.GetAll().Where(x => x.BranchId == branch.BranchId && x.StoreId == storeId && x.IsActive)
					select new CostCenterDropDownDto
					{
						CostCenterId = costCenter.CostCenterId,
						CostCenterCode = costCenter.CostCenterCode,
						CostCenterName = language == LanguageCode.Arabic ? costCenter.CostCenterNameAr : costCenter.CostCenterNameEn,
						CurrencyId = company.CurrencyId
					});
			return data;
		}
		public IQueryable<CostCenterDropDownDto> GetMainCostCentersByCompanyId(int companyId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			return _repository.GetAll().Where(x => x.IsActive && x.CompanyId == companyId && x.IsMainCostCenter).Select(x => new CostCenterDropDownDto()
			{
				CostCenterId = x.CostCenterId,
				CostCenterName = language == LanguageCode.Arabic ? x.CostCenterNameAr : x.CostCenterNameEn
			});
		}

		public IQueryable<CostCenterDropDownDto> GetMainCostCentersByStoreId(int storeId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var data =
				(from costCenter in _repository.GetAll().Where(x => x.IsMainCostCenter && x.IsActive)
					from company in _companyService.GetAll().Where(x => x.CompanyId == costCenter.CompanyId && x.IsActive)
					from branch in _branchService.GetAll().Where(x => x.CompanyId == company.CompanyId && x.IsActive)
					from store in _storeService.GetAll().Where(x => x.BranchId == branch.BranchId && x.StoreId == storeId && x.IsActive)
					select new CostCenterDropDownDto
					{
						CostCenterId = costCenter.CostCenterId,
						CostCenterCode = costCenter.CostCenterCode,
						CostCenterName = language == LanguageCode.Arabic ? costCenter.CostCenterNameAr : costCenter.CostCenterNameEn,
						CurrencyId = company.CurrencyId
					});
			return data;
		}

		public IQueryable<CostCenterTreeDto> GetCostCentersTree(int companyId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var data =
				from costCenter in _repository.GetAll().Where(x => x.CompanyId == companyId)
				from mainCostCenter in _repository.GetAll().Where(x => x.CostCenterId == costCenter.MainCostCenterId).DefaultIfEmpty()
				select new CostCenterTreeDto
				{
					CostCenterId = costCenter.CostCenterId,
					CostCenterCode = costCenter.CostCenterCode,
					CostCenterNameAr = costCenter.CostCenterNameAr,
					CostCenterNameEn = costCenter.CostCenterNameEn,
					IsMainCostCenter = costCenter.IsMainCostCenter,
					MainCostCenterId = costCenter.MainCostCenterId ?? 0,
					MainCostCenterCode = mainCostCenter != null ? mainCostCenter.CostCenterCode : "",
					CostCenterLevel = costCenter.CostCenterLevel,
					IsLastLevel = costCenter.IsLastLevel,
					Order = costCenter.Order
				};
			return data;
		}

		public async Task<string> GetNextCostCenterCode(int companyId, int mainCostCenterId, bool isMainCostCenter)
		{
			var mainLength = await _applicationSettingService.GetApplicationSettingValueByCompanyId(companyId, ApplicationSettingDetailData.MainCostCenter) ?? "";
			var individualLength = await _applicationSettingService.GetApplicationSettingValueByCompanyId(companyId, ApplicationSettingDetailData.IndividualCostCenter) ?? "";

			if (mainCostCenterId != 0)
			{
				var mainCostCenter = await _repository.GetAll().FirstOrDefaultAsync(x => x.CostCenterId == mainCostCenterId);
				var nextCode = await _repository.GetAll().CountAsync(x => x.MainCostCenterId == mainCostCenterId && x.IsMainCostCenter == isMainCostCenter && x.CompanyId == companyId) + 1;
				if (mainCostCenter != null)
				{
					var mainCode = mainCostCenter.CostCenterCode ?? "";
					var codes = await _repository.GetAll().Where(x => x.CompanyId == companyId).Select(x => x.CostCenterCode).ToListAsync();

					return TreeLogic.GenerateNextCode(codes, mainCode, isMainCostCenter, mainLength, individualLength, nextCode);
				}
				return "";
			}
			else
			{
				var nextCostCenterWithoutParent = await _repository.GetAll().CountAsync(x => x.MainCostCenterId == null && x.CompanyId == companyId) + 1;
				var codes = await _repository.GetAll().Where(x => x.CompanyId == companyId).Select(x => x.CostCenterCode).ToListAsync();

				return TreeLogic.GenerateNextCode(codes, "", isMainCostCenter, mainLength, individualLength, nextCostCenterWithoutParent);
			}
		}

		public async Task<List<CostCenterAutoCompleteDto>> GetMainCostCentersByCostCenterCode(int companyId, string costCenterCode)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			return await _repository.GetAll().Where(x => x.CompanyId == companyId && x.IsMainCostCenter && x.CostCenterCode!.ToLower().Contains(costCenterCode.Trim().ToLower())).Select(s => new CostCenterAutoCompleteDto
			{
				CostCenterCode = s.CostCenterCode,
				CostCenterId = s.CostCenterId,
				CostCenterName = language == LanguageCode.Arabic ? s.CostCenterNameAr : s.CostCenterNameEn
			}).Take(10).ToListAsync();
		}

		public async Task<List<CostCenterAutoCompleteDto>> GetMainCostCentersByCostCenterName(int companyId, string costCenterName)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			if (language == LanguageCode.Arabic)
			{
				return await _repository.GetAll().Where(x => x.CompanyId == companyId && x.IsMainCostCenter && x.CostCenterNameAr!.ToLower().Contains(costCenterName.Trim().ToLower()) && x.IsActive).Select(s => new CostCenterAutoCompleteDto
				{
					CostCenterCode = s.CostCenterCode,
					CostCenterId = s.CostCenterId,
					CostCenterName = s.CostCenterNameAr
				}).Take(10).ToListAsync();
			}
			else
			{
				return await _repository.GetAll().Where(x => x.CompanyId == companyId && x.IsMainCostCenter && x.CostCenterNameEn!.ToLower().Contains(costCenterName.Trim().ToLower()) && x.IsActive).Select(s => new CostCenterAutoCompleteDto
				{
					CostCenterCode = s.CostCenterCode,
					CostCenterId = s.CostCenterId,
					CostCenterName = s.CostCenterNameEn
				}).Take(10).ToListAsync();
			}
		}

		public IQueryable<CostCenterDto> GetAllCostCenters()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var data =
				from costCenter in _repository.GetAll()
				from mainCostCenter in _repository.GetAll().Where(x => x.CostCenterId == costCenter.MainCostCenterId).DefaultIfEmpty()
				select new CostCenterDto()
				{
					CostCenterId = costCenter.CostCenterId,
					CostCenterCode = costCenter.CostCenterCode,
					CompanyId = costCenter.CompanyId,
					CostCenterNameAr = costCenter.CostCenterNameAr,
					CostCenterNameEn = costCenter.CostCenterNameEn,
					IsMainCostCenter = costCenter.IsMainCostCenter,
					MainCostCenterId = costCenter.MainCostCenterId ?? 0,
					MainCostCenterCode = mainCostCenter != null ? mainCostCenter.CostCenterCode : null,
					MainCostCenterName = mainCostCenter != null ? (language == LanguageCode.Arabic ? mainCostCenter.CostCenterNameAr : mainCostCenter.CostCenterNameEn) : "",
					Order = costCenter.Order,
					CostCenterLevel = costCenter.CostCenterLevel,
					HasRemarks = costCenter.HasRemarks,
					RemarksAr = costCenter.RemarksAr,
					RemarksEn = costCenter.RemarksEn,
					NotesAr = costCenter.NotesAr,
					NotesEn = costCenter.NotesEn,
					IsPrivate = costCenter.IsPrivate,
					IsNonEditable = costCenter.IsNonEditable,
					IsLastLevel = costCenter.IsLastLevel,
					IsActive = costCenter.IsActive,
					InActiveReasons = costCenter.InActiveReasons,
					Description = costCenter.Description
				};
			return data;
		}

		public IQueryable<CostCenterDto> GetCompanyCostCenters(int companyId)
		{
			return GetAllCostCenters().Where(x => x.CompanyId == companyId);
		}

		public async Task<CostCenterDto> GetCostCenterByCostCenterId(int costCenterId)
		{
			var costCenterDb = await GetAllCostCenters().FirstOrDefaultAsync(x => x.CostCenterId == costCenterId) ?? new CostCenterDto();
			costCenterDb.HasChildren = await IsCostCenterHasChildren(costCenterId);
			return costCenterDb;
		}

		public async Task<CostCenterDto> GetCostCenterByCostCenterCode(int companyId, string costCenterCode)
		{
			return await GetAllCostCenters().FirstOrDefaultAsync(x => x.CompanyId == companyId && x.CostCenterCode!.Trim() == costCenterCode.Trim()) ?? new CostCenterDto();
		}

		public async Task<bool> IsCostCenterHasChildren(int costCenterId)
		{
			return await _repository.GetAll().AnyAsync(x => x.MainCostCenterId == costCenterId);
		}

		public List<RequestChangesDto> GetCostCenterRequestChanges(CostCenterDto oldItem, CostCenterDto newItem)
		{
			return CompareLogic.GetDifferences(oldItem, newItem);
		}

		public async Task<ResponseDto> SaveCostCenter(CostCenterDto costCenter)
		{
			var costCenterExist = await IsCostCenterExist(costCenter.CostCenterId, costCenter.CostCenterNameAr, costCenter.CostCenterNameEn, costCenter.CompanyId, costCenter.CostCenterCode!);
			if (costCenterExist.Success)
			{
				return new ResponseDto() { Id = costCenterExist.Id, Success = false, Message = _localizer["CostCenterAlreadyExist"] };
			}
			else
			{
				if (costCenter.CostCenterId == 0)
				{
					return await CreateCostCenter(costCenter);
				}
				else
				{
					return await UpdateCostCenter(costCenter);
				}
			}
		}

		public async Task<ResponseDto> SaveCostCenterInFull(CostCenterDto costCenter)
		{
			var result = await SaveCostCenter(costCenter);
			if (result.Success)
			{
				if (costCenter.MenuNotes != null) await _menuNoteService.SaveMenuNotes(costCenter.MenuNotes, result.Id);
			}
			return result;
		}

		public async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.CostCenterId) + 1; } catch { id = 1; }
			return id;
		}

		public async Task<ResponseDto> IsCostCenterExist(int id, string? nameAr, string? nameEn, int companyId, string costCenterCode)
		{
			var costCenter = await _repository.GetAll().FirstOrDefaultAsync(x => (x.CostCenterNameAr == nameAr || x.CostCenterNameEn == nameEn || x.CostCenterNameAr == nameEn || x.CostCenterNameEn == nameAr || x.CostCenterCode!.Trim() == costCenterCode.Trim()) && x.CostCenterId != id && x.CompanyId == companyId);
			if (costCenter != null)
			{
				return new ResponseDto() { Id = costCenter.CostCenterId, Success = true };
			}
			return new ResponseDto() { Id = 0, Success = false };
		}
		public async Task<ResponseDto> CreateCostCenter(CostCenterDto costCenter)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var mainAccountDb = await _repository.GetAll().AsNoTracking().FirstOrDefaultAsync(x => x.CostCenterId == costCenter.MainCostCenterId);

			var newCostCenter = new CostCenter()
			{
				CostCenterId = await GetNextId(),
				CostCenterNameAr = costCenter?.CostCenterNameAr?.Trim(),
				CostCenterNameEn = costCenter?.CostCenterNameEn?.Trim(),
				IsMainCostCenter = costCenter!.IsMainCostCenter,
				MainCostCenterId = costCenter.MainCostCenterId != 0 ? costCenter.MainCostCenterId : null,
				CostCenterCode = costCenter!.CostCenterCode?.Trim(),
				CompanyId = costCenter!.CompanyId,
				Order = costCenter.Order,
				CostCenterLevel = (byte)(mainAccountDb != null ? mainAccountDb.CostCenterLevel + 1 : 1),
				HasRemarks = costCenter.HasRemarks,
				IsLastLevel = !costCenter.IsMainCostCenter,
				IsPrivate = costCenter.IsPrivate,
				RemarksAr = costCenter.RemarksAr?.Trim(),
				RemarksEn = costCenter.RemarksEn?.Trim(),
				IsNonEditable = costCenter.IsNonEditable,
				NotesAr = costCenter.NotesAr?.Trim(),
				NotesEn = costCenter.NotesEn?.Trim(),
				IsActive = costCenter!.IsActive,
				InActiveReasons = costCenter?.InActiveReasons,
				CreatedAt = DateHelper.GetDateTimeNow(),
				UserNameCreated = await _httpContextAccessor!.GetUserName(),
				IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
				Hide = false,
				Description = costCenter?.Description
			};

			var validator = await new CostCenterValidator(_localizer).ValidateAsync(newCostCenter);
			var validationResult = validator.IsValid;
			if (validationResult)
			{
				await _repository.Insert(newCostCenter);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = newCostCenter.CostCenterId, Success = true, Message = _localizer["NewCostCenterSuccessMessage", ((language == LanguageCode.Arabic ? newCostCenter.CostCenterNameAr : newCostCenter.CostCenterNameEn) ?? ""), newCostCenter.CostCenterCode!] };
			}
			else
			{
				return new ResponseDto() { Id = newCostCenter.CostCenterId, Success = false, Message = validator.ToString("~") };
			}
		}

		public async Task<ResponseDto> UpdateCostCenter(CostCenterDto costCenter)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var costCenterDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.CostCenterId == costCenter.CostCenterId);
			if (costCenterDb != null)
			{
				var mainCostCenterDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.CostCenterId == costCenter.MainCostCenterId);
				costCenterDb.CostCenterNameAr = costCenter.CostCenterNameAr?.Trim();
				costCenterDb.CostCenterNameEn = costCenter.CostCenterNameEn?.Trim();
				costCenterDb.Hide = false;
				costCenterDb.IsActive = (bool)costCenter?.IsActive;
				costCenterDb.InActiveReasons = costCenter?.InActiveReasons;
				costCenterDb.IsMainCostCenter = costCenter!.IsMainCostCenter;
				costCenterDb.MainCostCenterId = costCenter.MainCostCenterId != 0 ? costCenter.MainCostCenterId : null;
				costCenterDb.CostCenterCode = costCenter!.CostCenterCode?.Trim();
				costCenterDb.CompanyId = costCenter!.CompanyId;
				costCenterDb.Order = costCenter.Order;
				costCenterDb.CostCenterLevel = (byte)(mainCostCenterDb != null ? mainCostCenterDb.CostCenterLevel + 1 : costCenter.CostCenterLevel);
				costCenterDb.HasRemarks = costCenter.HasRemarks;
				costCenterDb.IsLastLevel = !costCenter.IsMainCostCenter;
				costCenterDb.IsPrivate = costCenter.IsPrivate;
				costCenterDb.RemarksAr = costCenter.RemarksAr?.Trim();
				costCenterDb.RemarksEn = costCenter.RemarksEn?.Trim();
				costCenterDb.Description = costCenter?.Description;
				costCenterDb.IsNonEditable = costCenter!.IsNonEditable;
				costCenterDb.NotesAr = costCenter?.NotesAr?.Trim();
				costCenterDb.NotesEn = costCenter?.NotesEn?.Trim();
				costCenterDb.ModifiedAt = DateHelper.GetDateTimeNow();
				costCenterDb.UserNameModified = await _httpContextAccessor!.GetUserName();
				costCenterDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();

				var validator = await new CostCenterValidator(_localizer).ValidateAsync(costCenterDb);
				var validationResult = validator.IsValid;
				if (validationResult)
				{
					_repository.Update(costCenterDb);
					await _repository.SaveChanges();
					await ReorderCostCenterLevels(costCenterDb.CostCenterId, costCenterDb.CompanyId);
					return new ResponseDto() { Id = costCenterDb.CostCenterId, Success = true, Message = _localizer["UpdateCostCenterSuccessMessage", ((language == LanguageCode.Arabic ? costCenterDb.CostCenterNameAr : costCenterDb.CostCenterNameEn) ?? "")] };
				}
				else
				{
					return new ResponseDto() { Id = 0, Success = false, Message = validator.ToString("~") };
				}
			}
			return new ResponseDto() { Id = 0, Success = false, Message = _localizer["NoCostCenterFound"] };
		}

		public async Task<bool> ReorderCostCenterLevels(int costCenterId, int companyId)
		{
			var nextLevel = 1;
			var accountDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.CostCenterId == costCenterId);
			if (accountDb != null)
			{
				var mainCostCenterId = accountDb.MainCostCenterId;
				var parent = await _repository.GetAll().FirstOrDefaultAsync(x => x.CostCenterId == mainCostCenterId);
				if (parent != null)
				{
					nextLevel = parent.CostCenterLevel + 1;
				}
				var costCentersDb = await _repository.GetAll().Where(x => x.CompanyId == companyId).ToListAsync();

				var tree = GetCostCenterTreeByCostCenterId(costCentersDb, costCenterId);
				 LoopThroughCostCenters(costCentersDb, tree, nextLevel);
				 if (_costCenters.Any())
				 {
					 _repository.UpdateRange(_costCenters);
					await _repository.SaveChanges();
				 }
			}
			return true;
		}

		public void LoopThroughCostCenters(List<CostCenter> costCenters,List<CostCenterTreeVm> tree ,int nextLevel)
		{
			foreach (var item in tree)
			{
				var costCentersDb = costCenters.FirstOrDefault(x => x.CostCenterId == item.CostCenterId);
				if (costCentersDb != null)
				{
					costCentersDb.CostCenterLevel = (byte)nextLevel;
					_costCenters.Add(costCentersDb);
				}
			}

			foreach (var item in tree)
			{
				if (item.Children != null)
				{
					if (item.Children.Any())
					{
						nextLevel++;
						LoopThroughCostCenters(costCenters, item.Children, nextLevel);
					}
				}
			}
		}
		public List<CostCenterTreeVm> GetCostCenterTreeByCostCenterId(List<CostCenter> costCenters,int costCenterId)
		{
			var data = BuildTree(costCenters, costCenterId);
			return data;
		}

		public async Task<ResponseDto> DeleteCostCenter(int id)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var costCenterDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.CostCenterId == id);
			if (costCenterDb != null)
			{
				if (costCenterDb.IsMainCostCenter)
				{
					var costCenters = await RetrievingSubCostCenters(costCenterDb.CostCenterId, costCenterDb.CompanyId);
					var costCentersToDeleted = await _repository.GetAll().Where(x => costCenters.Contains(x.CostCenterId)).ToListAsync();
					_repository.RemoveRange(costCentersToDeleted);
				}
				else
				{
					_repository.Delete(costCenterDb);
				}
				await _repository.SaveChanges();
				return new ResponseDto() { Id = id, Success = true, Message = _localizer["DeleteCostCenterSuccessMessage", ((language == LanguageCode.Arabic ? costCenterDb.CostCenterNameAr : costCenterDb.CostCenterNameEn) ?? "")] };
			}
			return new ResponseDto() { Id = id, Success = false, Message = _localizer["NoCostCenterFound"] };
		}

		public async Task<ResponseDto> DeleteCostCenterInFull(int id)
		{
			var result = await DeleteCostCenter(id);
			if (result.Success)
			{
				await _menuNoteService.DeleteMenuNotes(MenuCodeData.CostCenter, id);
			}
			return result;
		}

		public async Task<List<CostCenterAutoCompleteDto>> RetrievingMainCostCenters(int mainCostCenterId, int companyId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var allCostCenters = await _repository.GetAll().Where(x => x.CompanyId == companyId).ToListAsync();
			var tree = GetTreeMain(allCostCenters, mainCostCenterId);
			foreach (var costCenter in tree)
			{
				var costCenterDb = allCostCenters.FirstOrDefault(x => x.CostCenterId == costCenter.CostCenterId);
				if (costCenterDb != null)
				{
					_treeName.Add(new CostCenterAutoCompleteDto()
					{
						CostCenterId = costCenterDb.CostCenterId,
						CostCenterCode = costCenterDb.CostCenterCode,
						CostCenterName = language == LanguageCode.Arabic ? costCenterDb.CostCenterNameAr : costCenterDb.CostCenterNameEn,
						CostCenterLevel = costCenterDb.CostCenterLevel
					});
				}

				if (costCenter!.List!.Any())
				{
					DeepIntoMainCostCenters(costCenter.List);
				}
			}
			return _treeName;
		}
		public async Task<List<int>> RetrievingSubCostCenters(int mainCostCenterId, int companyId)
		{
			var allCostCenters = await _repository.GetAll().Where(x => x.CompanyId == companyId).ToListAsync();
			var tree = GetTreeNodes(allCostCenters, mainCostCenterId);

			_deletedCostCenters.Add(mainCostCenterId);

			foreach (var costCenter in tree)
			{
				var costCenterDb = allCostCenters.FirstOrDefault(x => x.CostCenterId == costCenter.CostCenterId);
				if (costCenterDb != null)
				{
					_deletedCostCenters.Add(costCenterDb.CostCenterId);
				}

				if (costCenter!.List!.Any())
				{
					DeepIntoSubCostCenters(costCenter.List);
				}
			}
			return _deletedCostCenters;
		}

		public void DeepIntoMainCostCenters(List<CostCenterSimpleTreeDto> list)
		{

			foreach (var costCenter in list)
			{
				var costCenterDb = list.FirstOrDefault(x => x.MainCostCenterId == costCenter.MainCostCenterId);
				if (costCenterDb != null)
				{
					_treeName.Add(new CostCenterAutoCompleteDto()
					{
						CostCenterId = costCenterDb.CostCenterId,
						CostCenterCode = costCenterDb.CostCenterCode,
						CostCenterName = costCenterDb.CostCenterName,
						CostCenterLevel = costCenterDb.CostCenterLevel
					});
				}

				if (costCenter!.List!.Any())
				{
					DeepIntoMainCostCenters(costCenter.List);
				}
			}
		}

		public void DeepIntoSubCostCenters(List<CostCenterSimpleTreeDto> list)
		{
			foreach (var costCenter in list)
			{
				if (costCenter!.List!.Any())
				{
					DeepIntoSubCostCenters(costCenter.List);
				}
				else
				{
					var costCenterDb = list.FirstOrDefault(x => x.CostCenterId == costCenter.CostCenterId);
					if (costCenterDb != null)
					{
						_deletedCostCenters.Add(costCenterDb.CostCenterId);
					}
				}
			}
		}

		public List<CostCenterSimpleTreeDto> GetTreeMain(List<CostCenter> list, int? parent)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			return list.Where(x => x.CostCenterId == parent).Select(x => new CostCenterSimpleTreeDto
			{
				CostCenterId = x.CostCenterId,
				CostCenterCode = x.CostCenterCode,
				CostCenterName = language == LanguageCode.Arabic ? x.CostCenterNameAr : x.CostCenterNameEn,
				CostCenterLevel = x.CostCenterLevel,
				MainCostCenterId = x.MainCostCenterId,
				List = GetTreeMain(list, x.MainCostCenterId)
			}).ToList();
		}
		public List<CostCenterSimpleTreeDto> GetTreeNodes(List<CostCenter> list, int parent)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			return list.Where(x => x.MainCostCenterId == parent).Select(x => new CostCenterSimpleTreeDto
			{
				CostCenterId = x.CostCenterId,
				CostCenterCode = x.CostCenterCode,
				CostCenterName = language == LanguageCode.Arabic ? x.CostCenterNameAr : x.CostCenterNameEn,
				List = GetTreeNodes(list, x.CostCenterId)
			}).ToList();
		}

		public async Task<List<CostCenterAutoCompleteDto>> GetTreeList(int costCenterId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var costCenterDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.CostCenterId == costCenterId);
			if (costCenterDb != null)
			{
				_treeName.Add(new CostCenterAutoCompleteDto()
				{
					CostCenterId = costCenterDb.CostCenterId,
					CostCenterCode = costCenterDb.CostCenterCode,
					CostCenterName = costCenterDb.IsLastLevel ? (language == LanguageCode.Arabic ? $"{costCenterDb.CostCenterNameAr} ({costCenterDb.CostCenterCode} )" : $"{costCenterDb.CostCenterNameEn} ({costCenterDb.CostCenterCode} )") : (language == LanguageCode.Arabic ? costCenterDb.CostCenterNameAr : costCenterDb.CostCenterNameEn),
					CostCenterLevel = costCenterDb.CostCenterLevel
				});

				if (costCenterDb.MainCostCenterId != null)
				{
					await RetrievingMainCostCenters(costCenterDb.MainCostCenterId.GetValueOrDefault(), costCenterDb.CompanyId);
				}
			}
			return _treeName.OrderBy(x => x.CostCenterLevel).ThenBy(x => x.CostCenterCode).ToList();
		}

		public async Task<string?> GetCostCenterTreeName(int costCenterId)
		{
			var costCenterTreeName = "";
			var treeList = await GetTreeList(costCenterId);
			foreach (var costCenter in treeList)
			{
				costCenterTreeName += costCenter.CostCenterName + " ›› ";
			}
			return costCenterTreeName.Substring(0, costCenterTreeName.Length - 4);
		}

		private List<CostCenterTreeVm> BuildTree(List<CostCenter> costCenters, int? costCenterId)
		{
			if (costCenters.Any())
			{
				var costCentersInOrder = costCenters.Where(x => x.CostCenterId == costCenterId)
					.Select(x => new CostCenterTreeVm
					{
						CostCenterId = x.CostCenterId,
						CostCenterNameAr = x.CostCenterNameAr,
						CostCenterNameEn = x.CostCenterNameEn,
						MainCostCenterId = x.MainCostCenterId,
						CostCenterCode = x.CostCenterCode,
						IsMainCostCenter = x.IsMainCostCenter,
						IsLastLevel = x.IsLastLevel,
						CostCenterLevel = x.CostCenterLevel,
						Children = GetChildren(costCenters, x.CostCenterId)
					}).ToList();
				return costCentersInOrder;
			}
			return new List<CostCenterTreeVm>();

		}

		private List<CostCenterTreeVm> GetChildren(List<CostCenter> costCenters, int? parentId)
		{
			return costCenters.Where(x => x.MainCostCenterId == parentId)
				.Select(x => new CostCenterTreeVm
				{
					CostCenterId = x.CostCenterId,
					CostCenterNameAr = x.CostCenterNameAr,
					CostCenterNameEn = x.CostCenterNameEn,
					MainCostCenterId = x.MainCostCenterId,
					CostCenterCode = x.CostCenterCode,
					IsMainCostCenter = x.IsMainCostCenter,
					IsLastLevel = x.IsLastLevel,
					CostCenterLevel = x.CostCenterLevel,
					Children = GetChildren(costCenters, x.CostCenterId)
				}).ToList();
		}
	}
}
