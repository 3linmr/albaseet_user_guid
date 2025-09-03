using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Contracts.Settings;
using Shared.CoreOne;
using Shared.Helper.Models.Dtos;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Shared.CoreOne.Models.Domain.FixedAssets;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne.Contracts.FixedAssets;
using Shared.CoreOne.Models.Dtos.ViewModels.FixedAssets;
using Shared.Helper.Identity;
using Shared.Service.Validators;
using Microsoft.AspNetCore.Http.HttpResults;
using Shared.CoreOne.Models.Domain.Accounts;
using Shared.CoreOne.Models.Domain.Archive;
using Shared.CoreOne.Models.Dtos.ViewModels.Accounts;
using Shared.CoreOne.Models.StaticData;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.Helper.Logic;
using Shared.Service.Logic.Approval;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.CoreOne.Contracts.Accounts;
using Shared.CoreOne.Models.Dtos.ViewModels.CostCenters;
using Shared.Service.Logic.Tree;
using Shared.Service.Services.Settings;
using Shared.CoreOne.Models.Domain.CostCenters;
using Shared.Service.Services.Modules;

namespace Shared.Service.Services.FixedAssets
{
    public class FixedAssetService : BaseService<FixedAsset>, IFixedAssetService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer<FixedAssetService> _localizer;
        private readonly IAccountService _accountService;
        private readonly IApplicationSettingService _applicationSettingService;
        private readonly IMenuNoteService _menuNoteService;

        private List<int> _deletedFixedAssets = new();
        private List<FixedAssetAutoCompleteDto> _treeName = new();
        private List<FixedAsset> _fixedAssets = new();
        public FixedAssetService(
            IRepository<FixedAsset> repository, 
            IHttpContextAccessor httpContextAccessor, 
            IStringLocalizer<FixedAssetService> localizer,
            IAccountService accountService,
            IApplicationSettingService applicationSettingService,
            IMenuNoteService menuNoteService) : base(repository)
        {
            _httpContextAccessor = httpContextAccessor;
            _localizer = localizer;
            _accountService = accountService;
            _applicationSettingService = applicationSettingService;
            _menuNoteService = menuNoteService;
        }

        public async Task<ResponseDto> SaveFixedAsset(FixedAssetDto fixedAsset)
        {
            var fixedAssetExist = await IsFixedAssetExist(fixedAsset.FixedAssetId, fixedAsset.FixedAssetNameAr, fixedAsset.FixedAssetNameEn, fixedAsset.CompanyId);
            if (fixedAssetExist.Success)
            {
                return new ResponseDto() { Id = fixedAssetExist.Id, Success = false, Message = _localizer["FixedAssetAlreadyExist"] };
            }
            else
            {
                if (fixedAsset.FixedAssetId == 0)
                {
                    return await CreateFixedAsset(fixedAsset);
                }
                else
                {
                    return await UpdateFixedAsset(fixedAsset);
                }
            }
        }

        public async Task<int> GetNextId()
        {
            int id = 1;
            try { id = await _repository.GetAll().MaxAsync(a => a.FixedAssetId) + 1; } catch { id = 1; }
            return id;
        }

        public async Task<ResponseDto> IsFixedAssetExist(int id, string? nameAr, string? nameEn, int companyId)
        {
            var fixedAsset = await _repository.GetAll().FirstOrDefaultAsync(x => (x.FixedAssetNameAr == nameAr || x.FixedAssetNameEn == nameEn || x.FixedAssetNameAr == nameEn || x.FixedAssetNameEn == nameAr) && x.FixedAssetId != id && x.CompanyId == companyId);
            if (fixedAsset != null)
            {
                return new ResponseDto() { Id = fixedAsset.FixedAssetId, Success = true };
            }
            return new ResponseDto() { Id = 0, Success = false };
        }

        public async Task<ResponseDto> CreateFixedAsset(FixedAssetDto fixedAsset)
        {
            var mainAccountDb = await _repository.GetAll().AsNoTracking().FirstOrDefaultAsync(x => x.FixedAssetId == fixedAsset.MainFixedAssetId);
            if (fixedAsset.AccountId == null)
            {
                var fixedAssetAccountResponse = await _accountService.SaveFixedAssetAccount(
                    new FixedAssetAccountDto()
                    {
                        FixedAssetNameAr = fixedAsset.FixedAssetNameAr,
                        FixedAssetNameEn = fixedAsset.FixedAssetNameEn,
                        CompanyId = fixedAsset.CompanyId,
                        AccountId = 0,
                        IsMainAccount = fixedAsset.IsMainFixedAsset,
                        MainAccountId = mainAccountDb != null ? mainAccountDb.AccountId : null
                    });
                if (fixedAssetAccountResponse.Result.Success)
                {
                    fixedAsset.AccountId = fixedAssetAccountResponse.AccountId;
                    fixedAsset.DepreciationAccountId = fixedAssetAccountResponse.DepreciationAccountId;
                    fixedAsset.CumulativeDepreciationAccountId = fixedAssetAccountResponse.CumulativeDepreciationAccountId;
                }
                else
                {
                    return fixedAssetAccountResponse.Result;
                }
            }
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var newFixedAsset = new FixedAsset()
            {
                FixedAssetId = await GetNextId(),
                FixedAssetNameAr = fixedAsset?.FixedAssetNameAr?.Trim(),
                FixedAssetNameEn = fixedAsset?.FixedAssetNameEn?.Trim(),
                AccountId = fixedAsset?.AccountId,
                DepreciationAccountId = fixedAsset?.DepreciationAccountId,
                CumulativeDepreciationAccountId = fixedAsset?.CumulativeDepreciationAccountId,
                AnnualDepreciationPercent = fixedAsset!.AnnualDepreciationPercent,
                IsMainFixedAsset = fixedAsset!.IsMainFixedAsset,
                MainFixedAssetId = fixedAsset.MainFixedAssetId != 0 ? fixedAsset.MainFixedAssetId : null,
                FixedAssetCode = fixedAsset!.FixedAssetCode?.Trim(),
                CompanyId = fixedAsset!.CompanyId,
                Order = fixedAsset.Order,
                FixedAssetLevel = (byte)(mainAccountDb != null ? mainAccountDb.FixedAssetLevel + 1 : 1),
                HasRemarks = fixedAsset.HasRemarks,
                IsLastLevel = !fixedAsset.IsMainFixedAsset,
                IsPrivate = fixedAsset.IsPrivate,
                RemarksAr = fixedAsset.RemarksAr?.Trim(),
                RemarksEn = fixedAsset.RemarksEn?.Trim(),
                IsNonEditable = fixedAsset.IsNonEditable,
                NotesAr = fixedAsset.NotesAr?.Trim(),
                NotesEn = fixedAsset.NotesEn?.Trim(),
                IsActive = fixedAsset!.IsActive ?? true,
                InActiveReasonsAr = fixedAsset?.InActiveReasonsAr,
                InActiveReasonsEn = fixedAsset?.InActiveReasonsEn,
                CreatedAt = DateHelper.GetDateTimeNow(),
                UserNameCreated = await _httpContextAccessor!.GetUserName(),
                IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
                Hide = false,
                ArchiveHeaderId = fixedAsset?.ArchiveHeaderId
            };

            var validator = await new FixedAssetValidator(_localizer).ValidateAsync(newFixedAsset);
            var validationResult = validator.IsValid;
            if (validationResult)
            {
                await _repository.Insert(newFixedAsset);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = newFixedAsset.FixedAssetId, Success = true, Message = _localizer["NewFixedAssetSuccessMessage", ((language == LanguageCode.Arabic ? newFixedAsset.FixedAssetNameAr : newFixedAsset.FixedAssetNameEn) ?? ""), newFixedAsset.FixedAssetId!] };
            }
            else
            {
                return new ResponseDto() { Id = newFixedAsset.FixedAssetId, Success = false, Message = validator.ToString("~") };
            }
        }

        public async Task<ResponseDto> UpdateFixedAsset(FixedAssetDto fixedAsset)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var fixedAssetDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.FixedAssetId == fixedAsset.FixedAssetId);
            if (fixedAssetDb != null)
            {

                var mainFixedAssetDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.FixedAssetId == fixedAsset.MainFixedAssetId);
                fixedAssetDb.FixedAssetNameAr = fixedAsset.FixedAssetNameAr?.Trim();
                fixedAssetDb.FixedAssetNameEn = fixedAsset.FixedAssetNameEn?.Trim();
                fixedAssetDb.AnnualDepreciationPercent = fixedAsset!.AnnualDepreciationPercent;
                fixedAssetDb.Hide = false;
                fixedAssetDb.IsActive = fixedAsset?.IsActive ?? true;
                fixedAssetDb.InActiveReasonsAr = fixedAsset?.InActiveReasonsAr;
                fixedAssetDb.InActiveReasonsEn = fixedAsset?.InActiveReasonsEn;
                fixedAssetDb.IsMainFixedAsset = fixedAsset!.IsMainFixedAsset;
                fixedAssetDb.MainFixedAssetId = fixedAsset.MainFixedAssetId != 0 ? fixedAsset.MainFixedAssetId : null;
                fixedAssetDb.FixedAssetCode = fixedAsset!.FixedAssetCode?.Trim();
                fixedAssetDb.CompanyId = fixedAsset!.CompanyId;
                fixedAssetDb.Order = fixedAsset.Order;
                fixedAssetDb.FixedAssetLevel = (byte)(mainFixedAssetDb != null ? mainFixedAssetDb.FixedAssetLevel + 1 : fixedAsset.FixedAssetLevel);
                fixedAssetDb.HasRemarks = fixedAsset.HasRemarks;
                fixedAssetDb.IsLastLevel = !fixedAsset.IsMainFixedAsset;
                fixedAssetDb.IsPrivate = fixedAsset.IsPrivate;
                fixedAssetDb.RemarksAr = fixedAsset.RemarksAr?.Trim();
                fixedAssetDb.RemarksEn = fixedAsset.RemarksEn?.Trim();
                fixedAssetDb.IsNonEditable = fixedAsset!.IsNonEditable;
                fixedAssetDb.NotesAr = fixedAsset?.NotesAr?.Trim();
                fixedAssetDb.NotesEn = fixedAsset?.NotesEn?.Trim();
                fixedAssetDb.ModifiedAt = DateHelper.GetDateTimeNow();
                fixedAssetDb.UserNameModified = await _httpContextAccessor!.GetUserName();
                fixedAssetDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();
                fixedAssetDb.ArchiveHeaderId = fixedAsset?.ArchiveHeaderId;

                var validator = await new FixedAssetValidator(_localizer).ValidateAsync(fixedAssetDb);
                var validationResult = validator.IsValid;
                if (validationResult)
                {
                    _repository.Update(fixedAssetDb);
                    await _repository.SaveChanges();
                    await ReorderFixedAssetLevels(fixedAssetDb.FixedAssetId, fixedAssetDb.CompanyId);
                    return new ResponseDto() { Id = fixedAssetDb.FixedAssetId, Success = true, Message = _localizer["UpdateFixedAssetSuccessMessage", ((language == LanguageCode.Arabic ? fixedAssetDb.FixedAssetNameAr : fixedAssetDb.FixedAssetNameEn) ?? "")] };
                }
                else
                {
                    return new ResponseDto() { Id = 0, Success = false, Message = validator.ToString("~") };
                }
            }
            return new ResponseDto() { Id = 0, Success = false, Message = _localizer["NoFixedAssetFound"] };
        }

        public async Task<bool> ReorderFixedAssetLevels(int fixedAssetId, int companyId)
        {
            var nextLevel = 1;
            var accountDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.FixedAssetId == fixedAssetId);
            if (accountDb != null)
            {
                var mainFixedAssetId = accountDb.MainFixedAssetId;
                var parent = await _repository.GetAll().FirstOrDefaultAsync(x => x.FixedAssetId == mainFixedAssetId);
                if (parent != null)
                {
                    nextLevel = parent.FixedAssetLevel + 1;
                }
                var fixedAssetsDb = await _repository.GetAll().Where(x => x.CompanyId == companyId).ToListAsync();

                var tree = GetFixedAssetTreeByFixedAssetId(fixedAssetsDb, fixedAssetId);
                LoopThroughFixedAssets(fixedAssetsDb, tree, nextLevel);
                if (_fixedAssets.Any())
                {
                    _repository.UpdateRange(_fixedAssets);
                    await _repository.SaveChanges();
                }
            }
            return true;
        }

        public void LoopThroughFixedAssets(List<FixedAsset> fixedAssets, List<FixedAssetTreeVm> tree, int nextLevel)
        {
            foreach (var item in tree)
            {
                var fixedAssetsDb = fixedAssets.FirstOrDefault(x => x.FixedAssetId == item.FixedAssetId);
                if (fixedAssetsDb != null)
                {
                    fixedAssetsDb.FixedAssetLevel = (byte)nextLevel;
                    _fixedAssets.Add(fixedAssetsDb);
                }
            }

            foreach (var item in tree)
            {
                if (item.Children != null)
                {
                    if (item.Children.Any())
                    {
                        nextLevel++;
                        LoopThroughFixedAssets(fixedAssets, item.Children, nextLevel);
                    }
                }
            }
        }
        public List<FixedAssetTreeVm> GetFixedAssetTreeByFixedAssetId(List<FixedAsset> fixedAssets, int fixedAssetId)
        {
            var data = BuildTree(fixedAssets, fixedAssetId);
            return data;
        }
        public async Task<ResponseDto> DeleteFixedAsset(int id)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var fixedAssetDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.FixedAssetId == id);
            if (fixedAssetDb != null)
            {
                if (fixedAssetDb.IsMainFixedAsset)
                {
                    var fixedAssets = await RetrievingSubFixedAssets(fixedAssetDb.FixedAssetId, fixedAssetDb.CompanyId);
                    var fixedAssetsToDeleted = await _repository.GetAll().Where(x => fixedAssets.Contains(x.FixedAssetId)).ToListAsync();
                    _repository.RemoveRange(fixedAssetsToDeleted);
                }
                else
                {
                    _repository.Delete(fixedAssetDb);
                }
                await _repository.SaveChanges();
                return new ResponseDto() { Id = id, Success = true, Message = _localizer["DeleteFixedAssetSuccessMessage", ((language == LanguageCode.Arabic ? fixedAssetDb.FixedAssetNameAr : fixedAssetDb.FixedAssetNameEn) ?? "")] };
            }
            return new ResponseDto() { Id = id, Success = false, Message = _localizer["NoFixedAssetFound"] };
        }

        public async Task<ResponseDto> DeleteFixedAssetInFull(int id)
        {
            var result = await DeleteFixedAsset(id);
            if (result.Success)
            {
                await _menuNoteService.DeleteMenuNotes(MenuCodeData.FixedAsset, id);
            }
            return result;
        }
        public async Task<List<FixedAssetAutoCompleteDto>> RetrievingMainFixedAssets(int mainFixedAssetId, int companyId)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var allFixedAssets = await _repository.GetAll().Where(x => x.CompanyId == companyId).ToListAsync();
            var tree = GetTreeMain(allFixedAssets, mainFixedAssetId);
            foreach (var fixedAsset in tree)
            {
                var fixedAssetDb = allFixedAssets.FirstOrDefault(x => x.FixedAssetId == fixedAsset.FixedAssetId);
                if (fixedAssetDb != null)
                {
                    _treeName.Add(new FixedAssetAutoCompleteDto()
                    {
                        FixedAssetId = fixedAssetDb.FixedAssetId,
                        FixedAssetCode = fixedAssetDb.FixedAssetCode,
                        FixedAssetName = language == LanguageCode.Arabic ? fixedAssetDb.FixedAssetNameAr : fixedAssetDb.FixedAssetNameEn,
                        FixedAssetLevel = fixedAssetDb.FixedAssetLevel
                    });
                }

                if (fixedAsset!.List!.Any())
                {
                    DeepIntoMainFixedAssets(fixedAsset.List);
                }
            }
            return _treeName;
        }
        public async Task<List<int>> RetrievingSubFixedAssets(int mainFixedAssetId, int companyId)
        {
            var allFixedAssets = await _repository.GetAll().Where(x => x.CompanyId == companyId).ToListAsync();
            var tree = GetTreeNodes(allFixedAssets, mainFixedAssetId);

            _deletedFixedAssets.Add(mainFixedAssetId);

            foreach (var fixedAsset in tree)
            {
                var fixedAssetDb = allFixedAssets.FirstOrDefault(x => x.FixedAssetId == fixedAsset.FixedAssetId);
                if (fixedAssetDb != null)
                {
                    _deletedFixedAssets.Add(fixedAssetDb.FixedAssetId);
                }

                if (fixedAsset!.List!.Any())
                {
                    DeepIntoSubFixedAssets(fixedAsset.List);
                }
            }
            return _deletedFixedAssets;
        }

        public void DeepIntoMainFixedAssets(List<FixedAssetSimpleTreeDto> list)
        {

            foreach (var fixedAsset in list)
            {
                var fixedAssetDb = list.FirstOrDefault(x => x.MainFixedAssetId == fixedAsset.MainFixedAssetId);
                if (fixedAssetDb != null)
                {
                    _treeName.Add(new FixedAssetAutoCompleteDto()
                    {
                        FixedAssetId = fixedAssetDb.FixedAssetId,
                        FixedAssetCode = fixedAssetDb.FixedAssetCode,
                        FixedAssetName = fixedAssetDb.FixedAssetName,
                        FixedAssetLevel = fixedAssetDb.FixedAssetLevel
                    });
                }

                if (fixedAsset!.List!.Any())
                {
                    DeepIntoMainFixedAssets(fixedAsset.List);
                }
            }
        }

        public void DeepIntoSubFixedAssets(List<FixedAssetSimpleTreeDto> list)
        {
            foreach (var fixedAsset in list)
            {
                if (fixedAsset!.List!.Any())
                {
                    DeepIntoSubFixedAssets(fixedAsset.List);
                }
                else
                {
                    var fixedAssetDb = list.FirstOrDefault(x => x.FixedAssetId == fixedAsset.FixedAssetId);
                    if (fixedAssetDb != null)
                    {
                        _deletedFixedAssets.Add(fixedAssetDb.FixedAssetId);
                    }
                }
            }
        }

        public List<FixedAssetSimpleTreeDto> GetTreeMain(List<FixedAsset> list, int? parent)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            return list.Where(x => x.FixedAssetId == parent).Select(x => new FixedAssetSimpleTreeDto
            {
                FixedAssetId = x.FixedAssetId,
                FixedAssetCode = x.FixedAssetCode,
                FixedAssetName = language == LanguageCode.Arabic ? x.FixedAssetNameAr : x.FixedAssetNameEn,
                FixedAssetLevel = x.FixedAssetLevel,
                MainFixedAssetId = x.MainFixedAssetId,
                List = GetTreeMain(list, x.MainFixedAssetId)
            }).ToList();
        }
        public List<FixedAssetSimpleTreeDto> GetTreeNodes(List<FixedAsset> list, int parent)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            return list.Where(x => x.MainFixedAssetId == parent).Select(x => new FixedAssetSimpleTreeDto
            {
                FixedAssetId = x.FixedAssetId,
                FixedAssetCode = x.FixedAssetCode,
                FixedAssetName = language == LanguageCode.Arabic ? x.FixedAssetNameAr : x.FixedAssetNameEn,
                List = GetTreeNodes(list, x.FixedAssetId)
            }).ToList();
        }

        public async Task<List<FixedAssetAutoCompleteDto>> GetTreeList(int fixedAssetId)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();
            var fixedAssetDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.FixedAssetId == fixedAssetId);
            if (fixedAssetDb != null)
            {
                _treeName.Add(new FixedAssetAutoCompleteDto()
                {
                    FixedAssetId = fixedAssetDb.FixedAssetId,
                    FixedAssetCode = fixedAssetDb.FixedAssetCode,
                    FixedAssetName = fixedAssetDb.IsLastLevel ? (language == LanguageCode.Arabic ? $"{fixedAssetDb.FixedAssetNameAr} ({fixedAssetDb.FixedAssetCode} )" : $"{fixedAssetDb.FixedAssetNameEn} ({fixedAssetDb.FixedAssetCode} )") : (language == LanguageCode.Arabic ? fixedAssetDb.FixedAssetNameAr : fixedAssetDb.FixedAssetNameEn),
                    FixedAssetLevel = fixedAssetDb.FixedAssetLevel
                });

                if (fixedAssetDb.MainFixedAssetId != null)
                {
                    await RetrievingMainFixedAssets(fixedAssetDb.MainFixedAssetId.GetValueOrDefault(), fixedAssetDb.CompanyId);
                }
            }
            return _treeName.OrderBy(x => x.FixedAssetLevel).ThenBy(x => x.FixedAssetCode).ToList();
        }

        public async Task<string?> GetFixedAssetTreeName(int fixedAssetId)
        {
            var fixedAssetTreeName = "";
            var treeList = await GetTreeList(fixedAssetId);
            foreach (var fixedAsset in treeList)
            {
                fixedAssetTreeName += fixedAsset.FixedAssetName + " ›› ";
            }
            return fixedAssetTreeName.Substring(0, fixedAssetTreeName.Length - 4);
        }

        private List<FixedAssetTreeVm> BuildTree(List<FixedAsset> fixedAssets, int? fixedAssetId)
        {
            if (fixedAssets.Any())
            {
                var fixedAssetsInOrder = fixedAssets.Where(x => x.FixedAssetId == fixedAssetId)
                    .Select(x => new FixedAssetTreeVm
                    {
                        FixedAssetId = x.FixedAssetId,
                        FixedAssetNameAr = x.FixedAssetNameAr,
                        FixedAssetNameEn = x.FixedAssetNameEn,
                        MainFixedAssetId = x.MainFixedAssetId,
                        FixedAssetCode = x.FixedAssetCode,
                        IsMainFixedAsset = x.IsMainFixedAsset,
                        IsLastLevel = x.IsLastLevel,
                        FixedAssetLevel = x.FixedAssetLevel,
                        Children = GetChildren(fixedAssets, x.FixedAssetId)
                    }).ToList();
                return fixedAssetsInOrder;
            }
            return new List<FixedAssetTreeVm>();

        }

        private List<FixedAssetTreeVm> GetChildren(List<FixedAsset> fixedAssets, int? parentId)
        {
            return fixedAssets.Where(x => x.MainFixedAssetId == parentId)
                .Select(x => new FixedAssetTreeVm
                {
                    FixedAssetId = x.FixedAssetId,
                    FixedAssetNameAr = x.FixedAssetNameAr,
                    FixedAssetNameEn = x.FixedAssetNameEn,
                    MainFixedAssetId = x.MainFixedAssetId,
                    FixedAssetCode = x.FixedAssetCode,
                    IsMainFixedAsset = x.IsMainFixedAsset,
                    IsLastLevel = x.IsLastLevel,
                    FixedAssetLevel = x.FixedAssetLevel,
                    Children = GetChildren(fixedAssets, x.FixedAssetId)
                }).ToList();
        }
        public IQueryable<FixedAssetTreeDto> GetFixedAssetsTree(int companyId)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();
            var data =
                from fixedAsset in _repository.GetAll().Where(x => x.CompanyId == companyId)
                from mainFixedAsset in _repository.GetAll().Where(x => x.FixedAssetId == fixedAsset.MainFixedAssetId).DefaultIfEmpty()
                select new FixedAssetTreeDto
                {
                    FixedAssetId = fixedAsset.FixedAssetId,
                    FixedAssetCode = fixedAsset.FixedAssetCode,
                    FixedAssetNameAr = fixedAsset.FixedAssetNameAr,
                    FixedAssetNameEn = fixedAsset.FixedAssetNameEn,
                    IsMainFixedAsset = fixedAsset.IsMainFixedAsset,
                    MainFixedAssetId = fixedAsset.MainFixedAssetId ?? 0,
                    MainFixedAssetCode = mainFixedAsset != null ? mainFixedAsset.FixedAssetCode : "",
                    FixedAssetLevel = fixedAsset.FixedAssetLevel,
                    IsLastLevel = fixedAsset.IsLastLevel,
                    Order = fixedAsset.Order
                };
            return data;
        }

        public async Task<string> GetNextFixedAssetCode(int companyId, int mainFixedAssetId, bool isMainFixedAsset)
        {
            //var mainLength = await _applicationSettingService.GetApplicationSettingValueByCompanyId(companyId, ApplicationSettingDetailData.MainFixedAsset) ?? "";
            //var individualLength = await _applicationSettingService.GetApplicationSettingValueByCompanyId(companyId, ApplicationSettingDetailData.IndividualFixedAsset) ?? "";

            if (mainFixedAssetId != 0)
            {
                var mainFixedAsset = await _repository.GetAll().FirstOrDefaultAsync(x => x.FixedAssetId == mainFixedAssetId);
                var nextCode = await _repository.GetAll().CountAsync(x => x.MainFixedAssetId == mainFixedAssetId && x.IsMainFixedAsset == isMainFixedAsset && x.CompanyId == companyId) + 1;
                if (mainFixedAsset != null)
                {
                    var mainCode = mainFixedAsset.FixedAssetCode ?? "";
                    var codes = await _repository.GetAll().Where(x => x.CompanyId == companyId).Select(x => x.FixedAssetCode).ToListAsync();

                    return TreeLogic.GenerateNextCode(codes, mainCode, isMainFixedAsset, "0", "0", nextCode);
                }
                return "";
            }
            else
            {
                var nextFixedAssetWithoutParent = await _repository.GetAll().CountAsync(x => x.MainFixedAssetId == null && x.CompanyId == companyId) + 1;
                var codes = await _repository.GetAll().Where(x => x.CompanyId == companyId).Select(x => x.FixedAssetCode).ToListAsync();

                return TreeLogic.GenerateNextCode(codes, "", isMainFixedAsset, "0", "0", nextFixedAssetWithoutParent);
            }
        }

        public async Task<List<FixedAssetAutoCompleteDto>> GetMainFixedAssetsByFixedAssetId(int companyId, int fixedAssetId)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();
            return await _repository.GetAll().Where(x => x.CompanyId == companyId && x.IsMainFixedAsset && x.FixedAssetId.Equals(fixedAssetId)).Select(s => new FixedAssetAutoCompleteDto
            {
                FixedAssetCode = s.FixedAssetCode,
                FixedAssetId = s.FixedAssetId,
                FixedAssetName = language == LanguageCode.Arabic ? s.FixedAssetNameAr : s.FixedAssetNameEn
            }).Take(10).ToListAsync();
        }

        public async Task<List<FixedAssetAutoCompleteDto>> GetMainFixedAssetsByFixedAssetCode(int companyId, string fixedAssetCode)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();
            return await _repository.GetAll().Where(x => x.CompanyId == companyId && x.IsMainFixedAsset && x.FixedAssetCode!.ToLower().Contains(fixedAssetCode.Trim().ToLower())).Select(s => new FixedAssetAutoCompleteDto
            {
                FixedAssetCode = s.FixedAssetCode,
                FixedAssetId = s.FixedAssetId,
                FixedAssetName = language == LanguageCode.Arabic ? s.FixedAssetNameAr : s.FixedAssetNameEn
            }).Take(10).ToListAsync();
        }

        public async Task<List<FixedAssetAutoCompleteDto>> GetMainFixedAssetsByFixedAssetName(int companyId, string fixedAssetName)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();
            if (language == LanguageCode.Arabic)
            {
                return await _repository.GetAll().Where(x => x.CompanyId == companyId && x.IsMainFixedAsset && x.FixedAssetNameAr!.ToLower().Contains(fixedAssetName.Trim().ToLower()) && x.IsActive).Select(s => new FixedAssetAutoCompleteDto
                {
                    FixedAssetCode = s.FixedAssetCode,
                    FixedAssetId = s.FixedAssetId,
                    FixedAssetName = s.FixedAssetNameAr
                }).Take(10).ToListAsync();
            }
            else
            {
                return await _repository.GetAll().Where(x => x.CompanyId == companyId && x.IsMainFixedAsset && x.FixedAssetNameEn!.ToLower().Contains(fixedAssetName.Trim().ToLower()) && x.IsActive).Select(s => new FixedAssetAutoCompleteDto
                {
                    FixedAssetCode = s.FixedAssetCode,
                    FixedAssetId = s.FixedAssetId,
                    FixedAssetName = s.FixedAssetNameEn
                }).Take(10).ToListAsync();
            }
        }
        public IQueryable<FixedAssetDto> GetAllFixedAssets()
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var data =
                from fixedAsset in _repository.GetAll()
                from mainFixedAsset in _repository.GetAll().Where(x => x.FixedAssetId == fixedAsset.MainFixedAssetId).DefaultIfEmpty()
                from account in _accountService.GetAll().Where(x => x.AccountId == fixedAsset.AccountId).DefaultIfEmpty()
                from depreciationAccount in _accountService.GetAll().Where(x => x.AccountId == fixedAsset.DepreciationAccountId).DefaultIfEmpty()
                from cumulativeAccount in _accountService.GetAll().Where(x => x.AccountId == fixedAsset.CumulativeDepreciationAccountId).DefaultIfEmpty()
                select new FixedAssetDto()
                {
                    FixedAssetId = fixedAsset.FixedAssetId,
                    FixedAssetCode = fixedAsset.FixedAssetCode,
                    CompanyId = fixedAsset.CompanyId,
                    FixedAssetName = language == LanguageCode.Arabic ? fixedAsset.FixedAssetNameAr : fixedAsset.FixedAssetNameEn,
                    FixedAssetNameAr = fixedAsset.FixedAssetNameAr,
                    FixedAssetNameEn = fixedAsset.FixedAssetNameEn,
                    AccountId = fixedAsset.AccountId,
                    CurrencyId = account.CurrencyId,
                    DepreciationAccountId = fixedAsset.DepreciationAccountId,
                    DepreciationCurrencyId = depreciationAccount.CurrencyId,
                    CumulativeDepreciationAccountId = fixedAsset.CumulativeDepreciationAccountId,
                    CumulativeCurrencyId = cumulativeAccount.CurrencyId,
                    AnnualDepreciationPercent = fixedAsset.AnnualDepreciationPercent,
                    IsMainFixedAsset = fixedAsset.IsMainFixedAsset,
                    MainFixedAssetId = fixedAsset.MainFixedAssetId ?? 0,
                    MainFixedAssetCode = mainFixedAsset != null ? mainFixedAsset.FixedAssetCode : null,
                    MainFixedAssetName = mainFixedAsset != null ? (language == LanguageCode.Arabic ? mainFixedAsset.FixedAssetNameAr : mainFixedAsset.FixedAssetNameEn) : "",
                    Order = fixedAsset.Order,
                    FixedAssetLevel = fixedAsset.FixedAssetLevel,
                    HasRemarks = fixedAsset.HasRemarks,
                    RemarksAr = fixedAsset.RemarksAr,
                    RemarksEn = fixedAsset.RemarksEn,
                    NotesAr = fixedAsset.NotesAr,
                    NotesEn = fixedAsset.NotesEn,
                    IsPrivate = fixedAsset.IsPrivate,
                    IsNonEditable = fixedAsset.IsNonEditable,
                    IsLastLevel = fixedAsset.IsLastLevel,
                    IsActive = fixedAsset.IsActive,
                    InActiveReasons = language == LanguageCode.Arabic ? fixedAsset.InActiveReasonsAr : fixedAsset.InActiveReasonsEn,
                    InActiveReasonsAr = fixedAsset.InActiveReasonsAr,
                    InActiveReasonsEn = fixedAsset.InActiveReasonsEn,
                    ArchiveHeaderId = fixedAsset.ArchiveHeaderId
                };
            return data;
        }
        public async Task<FixedAssetDto> GetFixedAssetByFixedAssetCode(int companyId, string fixedAssetCode)
        {
            return await GetAllFixedAssets().FirstOrDefaultAsync(x => x.CompanyId == companyId && x.FixedAssetCode!.Trim() == fixedAssetCode.Trim()) ?? new FixedAssetDto();
        }

        public async Task<bool> IsFixedAssetHasChildren(int fixedAssetId)
        {
            return await _repository.GetAll().AnyAsync(x => x.MainFixedAssetId == fixedAssetId);
        }
        public async Task<FixedAssetDto> GetFixedAssetByFixedAssetId(int fixedAssetId)
        {
            if (fixedAssetId != 0)
            {
                var fixedAssetDb = await GetAllFixedAssets().FirstOrDefaultAsync(x => x.FixedAssetId == fixedAssetId);
                fixedAssetDb!.HasChildren = await IsFixedAssetHasChildren(fixedAssetId);
                if (fixedAssetDb != null)
                {
                    return fixedAssetDb;
                }

            }
            return new FixedAssetDto();
        }
        public async Task<List<FixedAssetDropDownDto>> GetFixedAssetsDropDownByCompanyId(int companyId)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();
            return await GetAllFixedAssets().Where(x => x.CompanyId == companyId && !x.IsMainFixedAsset).Select(x => new FixedAssetDropDownDto()
            {
                FixedAssetId = x.FixedAssetId,
                FixedAssetName = language == LanguageCode.Arabic ? x.FixedAssetNameAr : x.FixedAssetNameEn
            }).OrderBy(x => x.FixedAssetName).ToListAsync();
        }
        public List<RequestChangesDto> GetFixedAssetRequestChanges(FixedAssetDto oldItem, FixedAssetDto newItem)
        {
            return CompareLogic.GetDifferences(oldItem, newItem);
        }

    }
}
