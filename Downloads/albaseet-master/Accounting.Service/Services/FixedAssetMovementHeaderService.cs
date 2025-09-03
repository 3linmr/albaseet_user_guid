using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Contracts.FixedAssets;
using Shared.CoreOne.Models.Domain.FixedAssets;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.CoreOne.Models.Dtos.ViewModels.FixedAssets;
using Shared.CoreOne;
using Shared.Helper.Models.Dtos;
using Shared.Service.Logic.Approval;
using Shared.Service.Services.FixedAssets;
using Shared.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Accounting.CoreOne.Models.Domain;
using Accounting.CoreOne.Models.Dtos.ViewModels;
using Shared.Helper.Identity;
using Accounting.Service.Validators;
using Accounting.CoreOne.Contracts;
using Shared.CoreOne.Models.Domain.Approval;
using Shared.CoreOne.Contracts.Settings;
using Shared.CoreOne.Models.StaticData;
using Shared.Service.Logic.Tree;
using Shared.CoreOne.Models.Domain.Modules;

namespace Accounting.Service.Services
{
    public class FixedAssetMovementHeaderService : BaseService<FixedAssetMovementHeader>, IFixedAssetMovementHeaderService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer<FixedAssetMovementHeaderService> _localizer;
        private readonly IApplicationSettingService _applicationSettingService;

        public FixedAssetMovementHeaderService(
            IRepository<FixedAssetMovementHeader> repository,
            IHttpContextAccessor httpContextAccessor,
            IStringLocalizer<FixedAssetMovementHeaderService> localizer,
            IApplicationSettingService applicationSettingService) : base(repository)
        {
            _httpContextAccessor = httpContextAccessor;
            _localizer = localizer;
            _applicationSettingService = applicationSettingService;
        }

        public async Task<ResponseDto> SaveFixedAssetMovementHeader(FixedAssetMovementHeaderDto fixedAssetMovementHeader, bool hasApprove, bool approved, int? requestId)
        {
            var separateYears = await _applicationSettingService.SeparateYears(fixedAssetMovementHeader.StoreId);
            var fixedAssetMovementHeaderExist = await IsFixedAssetMovementHeaderExist(fixedAssetMovementHeader.FixedAssetMovementHeaderId);
            if (fixedAssetMovementHeaderExist.Success)
            {
                return new ResponseDto() { Id = fixedAssetMovementHeaderExist.Id, Success = false, Message = _localizer["FixedAssetMovementHeaderAlreadyExist"] };
            }
            else
            {
                if (fixedAssetMovementHeader.FixedAssetMovementHeaderId == 0)
                {
                    return await CreateFixedAssetMovementHeader(fixedAssetMovementHeader, hasApprove, approved, requestId, separateYears);
                }
                else
                {
                    return await UpdateFixedAssetMovementHeader(fixedAssetMovementHeader);
                }
            }
        }

        public async Task<int> GetNextId()
        {
            int id = 1;
            try { id = await _repository.GetAll().MaxAsync(a => a.FixedAssetMovementHeaderId) + 1; } catch { id = 1; }
            return id;
        }
        public async Task<int> GetNextFixedAssetMovementCode(int storeId, bool separateYears, DateTime documentDate, string? prefix, string? suffix)
        {
            int id = 1;
            try { id = await _repository.GetAll().Where(x => x.StoreId == storeId && (!separateYears || x.DocumentDate.Year == documentDate.Year) && x.Prefix == prefix && x.Suffix == suffix).MaxAsync(a => a.DocumentCode) + 1; } catch { id = 1; }
            return id;
        }
        public async Task<ResponseDto> IsFixedAssetMovementHeaderExist(int fixedAssetMovementHeaderId)
        {
            var fixedAssetHeader = await _repository.GetAll().FirstOrDefaultAsync(x => x.FixedAssetMovementHeaderId == fixedAssetMovementHeaderId);
            if (fixedAssetHeader != null)
            {
                return new ResponseDto() { Id = fixedAssetHeader.FixedAssetMovementHeaderId, Success = true };
            }
            return new ResponseDto() { Id = 0, Success = false };
        }

        public async Task<ResponseDto> CreateFixedAssetMovementHeader(FixedAssetMovementHeaderDto fixedAssetMovementHeader, bool hasApprove, bool approved, int? requestId, bool separateYears)
        {
            var documentCode = 0;
            if (hasApprove)
            {
                if (approved)
                {
                    documentCode = await GetNextFixedAssetMovementCode(fixedAssetMovementHeader.StoreId, separateYears, fixedAssetMovementHeader.MovementDate, fixedAssetMovementHeader.Prefix, fixedAssetMovementHeader.Suffix);
                }
            }
            else
            {
                documentCode = fixedAssetMovementHeader.DocumentCode;
            }
            var fixedAssetMovementId = await GetNextId();

            var newFixedAssetMovementHeader = new FixedAssetMovementHeader()
            {
                FixedAssetMovementHeaderId = fixedAssetMovementId,
                Prefix = fixedAssetMovementHeader.Prefix,
                DocumentCode = fixedAssetMovementHeader.DocumentCode,
                Suffix = fixedAssetMovementHeader.Suffix,
                DocumentDate = fixedAssetMovementHeader.MovementDate,
                EntryDate = fixedAssetMovementHeader.EntryDate,
                DocumentReference = fixedAssetMovementHeader.DocumentReference,
                StoreId = fixedAssetMovementHeader.StoreId,
                CostCenterToId = fixedAssetMovementHeader.CostCenterToId,
                MovementDate = fixedAssetMovementHeader.MovementDate,
                RemarksAr = fixedAssetMovementHeader.RemarksAr,
                RemarksEn = fixedAssetMovementHeader.RemarksEn,
                CreatedAt = DateTime.Now,
                UserNameCreated = await _httpContextAccessor!.GetUserName(),
                IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
                Hide = false
            };

            await _repository.Insert(newFixedAssetMovementHeader);
            await _repository.SaveChanges();
            return new ResponseDto() { Id = newFixedAssetMovementHeader.FixedAssetMovementHeaderId, Success = true, Message = _localizer["NewFixedAssetMovementHeaderSuccessMessage", newFixedAssetMovementHeader.FixedAssetMovementHeaderId!] };
        }

        public async Task<ResponseDto> UpdateFixedAssetMovementHeader(FixedAssetMovementHeaderDto fixedAssetMovementHeader)
        {
            var fixedAssetMovementHeaderDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.FixedAssetMovementHeaderId == fixedAssetMovementHeader.FixedAssetMovementHeaderId);
            if (fixedAssetMovementHeaderDb != null)
            {
                fixedAssetMovementHeaderDb.Prefix = fixedAssetMovementHeader.Prefix;
                fixedAssetMovementHeaderDb.DocumentCode = fixedAssetMovementHeader.DocumentCode;
                fixedAssetMovementHeaderDb.Suffix = fixedAssetMovementHeader.Suffix;
                fixedAssetMovementHeaderDb.DocumentDate = fixedAssetMovementHeader.DocumentDate;
                fixedAssetMovementHeaderDb.EntryDate = fixedAssetMovementHeader.EntryDate;
                fixedAssetMovementHeaderDb.DocumentReference = fixedAssetMovementHeader.DocumentReference;
                fixedAssetMovementHeaderDb.StoreId = fixedAssetMovementHeader.StoreId;
                fixedAssetMovementHeaderDb.CostCenterToId = fixedAssetMovementHeader.CostCenterToId;
                fixedAssetMovementHeaderDb.MovementDate = fixedAssetMovementHeader.MovementDate;
                fixedAssetMovementHeaderDb.RemarksAr = fixedAssetMovementHeader.RemarksAr;
                fixedAssetMovementHeaderDb.RemarksEn = fixedAssetMovementHeader.RemarksEn;
                fixedAssetMovementHeaderDb.ModifiedAt = DateTime.Now;
                fixedAssetMovementHeaderDb.UserNameModified = await _httpContextAccessor!.GetUserName();
                fixedAssetMovementHeaderDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();

                _repository.Update(fixedAssetMovementHeaderDb);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = fixedAssetMovementHeaderDb.FixedAssetMovementHeaderId, Success = true, Message = _localizer["UpdateFixedAssetMovementHeaderSuccessMessage"] };
            }
            return new ResponseDto() { Id = 0, Success = false, Message = _localizer["NoFixedAssetMovementHeaderFound"] };
        }

        public async Task<ResponseDto> DeleteFixedAssetMovementHeader(int id)
        {
            var fixedAssetHeaderDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.FixedAssetMovementHeaderId == id);
            if (fixedAssetHeaderDb != null)
            {
                _repository.Delete(fixedAssetHeaderDb);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = id, Success = true, Message = _localizer["DeleteFixedAssetMovementHeaderSuccessMessage"] };
            }
            return new ResponseDto() { Id = id, Success = false, Message = _localizer["NoFixedAssetMovementHeaderFound"] };
        }

        public IQueryable<FixedAssetMovementHeaderDto> GetAllFixedAssetMovementHeaders()
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();
            var data =
                from fixedAssetMovementHeader in _repository.GetAll()
                select new FixedAssetMovementHeaderDto()
                {
                    FixedAssetMovementHeaderId = fixedAssetMovementHeader.FixedAssetMovementHeaderId,
                    Prefix = fixedAssetMovementHeader.Prefix,
                    DocumentCode = fixedAssetMovementHeader.DocumentCode,
                    Suffix = fixedAssetMovementHeader.Suffix,
                    DocumentDate = fixedAssetMovementHeader.DocumentDate,
                    EntryDate = fixedAssetMovementHeader.EntryDate,
                    DocumentReference = fixedAssetMovementHeader.DocumentReference,
                    StoreId = fixedAssetMovementHeader.StoreId,
                    CostCenterToId = fixedAssetMovementHeader.CostCenterToId,
                    CostCenterToName = fixedAssetMovementHeader.CostCenterTo == null ? null : (language == LanguageCode.Arabic ? fixedAssetMovementHeader.CostCenterTo.CostCenterNameAr : fixedAssetMovementHeader.CostCenterTo.CostCenterNameEn),
                    MovementDate = fixedAssetMovementHeader.MovementDate,
                    RemarksAr = fixedAssetMovementHeader.RemarksAr,
                    RemarksEn = fixedAssetMovementHeader.RemarksEn
                };
            return data;
        }

        public async Task<string> GetNextFixedAssetMovementHeaderCodeByStoreId(int storeId)
        {
            var nextFixedAssetMovementHeader = await _repository.GetAll().CountAsync(x => x.StoreId == storeId) + 1;
            var codes = await _repository.GetAll().Where(x => x.StoreId == storeId).Select(x => Convert.ToString(x.DocumentCode)).ToListAsync();

            return TreeLogic.GenerateNextCode(codes!, "", true, "0", "0", nextFixedAssetMovementHeader);
        }
        public async Task<IQueryable<FixedAssetMovementHeaderDto>> GetUserFixedAssetMovementHeaders()
        {
            var userStore = await _httpContextAccessor.GetUserStores();
            return GetAllFixedAssetMovementHeaders().Where(x => userStore.Contains(x.StoreId));
        }

        public async Task<FixedAssetMovementHeaderDto> GetFixedAssetMovementHeaderById(int fixedAssetMovementHeaderId)
        {
            if (fixedAssetMovementHeaderId != 0)
            {
                var fixedAssetMovementHeaderDb = await GetAllFixedAssetMovementHeaders().FirstOrDefaultAsync(x => x.FixedAssetMovementHeaderId == fixedAssetMovementHeaderId);
                if (fixedAssetMovementHeaderDb != null)
                {
                    return fixedAssetMovementHeaderDb;
                }
            }
            return new FixedAssetMovementHeaderDto();
        }
    }
}
