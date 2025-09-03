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
using Shared.CoreOne.Models.Domain.Journal;
using Shared.CoreOne.Models.Domain.Modules;
using Accounting.Service.Validators;
using Accounting.CoreOne.Contracts;
using Shared.Helper.Logic;

namespace Accounting.Service.Services
{
    public class FixedAssetMovementDetailService : BaseService<FixedAssetMovementDetail>, IFixedAssetMovementDetailService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer<FixedAssetMovementDetailService> _localizer;

        public FixedAssetMovementDetailService(
            IRepository<FixedAssetMovementDetail> repository,
            IHttpContextAccessor httpContextAccessor,
            IStringLocalizer<FixedAssetMovementDetailService> localizer) : base(repository)
        {
            _httpContextAccessor = httpContextAccessor;
            _localizer = localizer;
        }
        public async Task<FixedAssetMovementDetailResponseDto> SaveFixedAssetMovementDetail(List<FixedAssetMovementDetailDto> details, int headerId)
        {
            if (details != null)
            {
                if (details.Count != 0)
                {
                    await DeleteFixedAssetMovementDetail(details, headerId);
                    var responseDetails = await AddFixedAssetMovementDetail(details, headerId);
                    return new FixedAssetMovementDetailResponseDto()
                    {
                        Response = new ResponseDto() { Success = true },
                        FixedAssetMovementDetails = responseDetails
                    };
                }
            }
            return new FixedAssetMovementDetailResponseDto()
            {
                Response = new ResponseDto() { Success = false }
            };
        }

        public async Task<bool> DeleteFixedAssetMovementDetail(List<FixedAssetMovementDetailDto> details, int headerId)
        {
            if (details.Count != 0)
            {
                var current = _repository.GetAll().Where(x => x.FixedAssetMovementHeaderId == headerId).ToList();
                if (current.Count != 0)
                {
                    _repository.RemoveRange(current);
                    await _repository.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public async Task<List<FixedAssetMovementDetailDto>> AddFixedAssetMovementDetail(List<FixedAssetMovementDetailDto> details, int headerId)
        {
            var modelList = new List<FixedAssetMovementDetail>();
            var newId = await GetNextId();
            foreach (var detail in details)
            {
                if (detail.FixedAssetMovementDetailId <= 0)
                {
                    detail.FixedAssetMovementDetailId = newId;
                    detail.FixedAssetMovementHeaderId = headerId;
                    var newNote = new FixedAssetMovementDetail()
                    {
                        FixedAssetMovementDetailId = newId,
                        FixedAssetMovementHeaderId = headerId,
                        FixedAssetId = detail.FixedAssetId,
                        RemarksAr = detail.RemarksAr,
                        RemarksEn = detail.RemarksEn,
                        Hide = false,
                        CreatedAt = DateHelper.GetDateTimeNow(),
                        UserNameCreated = await _httpContextAccessor!.GetUserName(),
                        IpAddressCreated = _httpContextAccessor?.GetIpAddress()
                    };
                    modelList.Add(newNote);
                    newId++;
                }

            }

            if (modelList.Count != 0)
            {
                await _repository.InsertRange(modelList);
                await _repository.SaveChanges();
            }
            return details;

        }
       
        public async Task<int> GetNextId()
        {
            int id = 1;
            try { id = await _repository.GetAll().MaxAsync(a => a.FixedAssetMovementDetailId) + 1; } catch { id = 1; }
            return id;
        }

        public async Task<ResponseDto> IsFixedAssetMovementDetailExist(int fixedAssetMovementDetailId, int fixedAssetMovementHeaderId, int fixedAssetId)
        {
            var fixedAsset = await _repository.GetAll().FirstOrDefaultAsync(x => x.FixedAssetMovementDetailId != fixedAssetMovementDetailId && x.FixedAssetMovementHeaderId == fixedAssetMovementHeaderId && x.FixedAssetId == fixedAssetId);
            if (fixedAsset != null)
            {
                return new ResponseDto() { Id = fixedAsset.FixedAssetMovementDetailId, Success = true };
            }
            return new ResponseDto() { Id = 0, Success = false };
        }

        public async Task<ResponseDto> CreateFixedAssetMovementDetail(FixedAssetMovementDetailDto fixedAssetMovementDetail)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();
            var newFixedAssetMovementDetail = new FixedAssetMovementDetail()
            {
                FixedAssetMovementDetailId = await GetNextId(),
                FixedAssetMovementHeaderId = fixedAssetMovementDetail.FixedAssetMovementHeaderId,
                FixedAssetId = fixedAssetMovementDetail.FixedAssetId,
                RemarksAr = fixedAssetMovementDetail.RemarksAr,
                RemarksEn = fixedAssetMovementDetail.RemarksEn,
                CreatedAt = DateTime.Now,
                UserNameCreated = await _httpContextAccessor!.GetUserName(),
                IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
                Hide = false
            };

            var validator = await new FixedAssetMovementDetailValidator(_localizer).ValidateAsync(newFixedAssetMovementDetail);
            var validationResult = validator.IsValid;
            if (validationResult)
            {
                await _repository.Insert(newFixedAssetMovementDetail);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = newFixedAssetMovementDetail.FixedAssetMovementDetailId, Success = true, Message = _localizer["NewFixedAssetMovementDetailSuccessMessage", newFixedAssetMovementDetail.FixedAssetMovementDetailId!] };
            }
            else
            {
                return new ResponseDto() { Id = newFixedAssetMovementDetail.FixedAssetMovementDetailId, Success = false, Message = validator.ToString("~") };
            }
        }

        public async Task<ResponseDto> UpdateFixedAssetMovementDetail(FixedAssetMovementDetailDto fixedAssetMovementDetail)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var fixedAssetMovementDetailDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.FixedAssetMovementDetailId == fixedAssetMovementDetail.FixedAssetMovementDetailId && x.FixedAssetMovementHeaderId == fixedAssetMovementDetail.FixedAssetMovementHeaderId && x.FixedAssetId == fixedAssetMovementDetail.FixedAssetId);
            if (fixedAssetMovementDetailDb != null)
            {
                fixedAssetMovementDetailDb.RemarksAr = fixedAssetMovementDetail.RemarksAr;
                fixedAssetMovementDetailDb.RemarksEn = fixedAssetMovementDetail.RemarksEn;
                fixedAssetMovementDetailDb.ModifiedAt = DateTime.Now;
                fixedAssetMovementDetailDb.UserNameModified = await _httpContextAccessor!.GetUserName();
                fixedAssetMovementDetailDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();

                var validator = await new FixedAssetMovementDetailValidator(_localizer).ValidateAsync(fixedAssetMovementDetailDb);
                var validationResult = validator.IsValid;
                if (validationResult)
                {
                    _repository.Update(fixedAssetMovementDetailDb);
                    await _repository.SaveChanges();
                    return new ResponseDto() { Id = fixedAssetMovementDetailDb.FixedAssetMovementDetailId, Success = true, Message = _localizer["UpdateFixedAssetMovementDetailSuccessMessage"] };
                }
                else
                {
                    return new ResponseDto() { Id = 0, Success = false, Message = validator.ToString("~") };
                }
            }
            return new ResponseDto() { Id = 0, Success = false, Message = _localizer["NoFixedAssetMovementDetailFound"] };
        }

        public async Task<ResponseDto> DeleteFixedAssetMovementDetail(int id)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var fixedAssetDetailDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.FixedAssetMovementDetailId == id);
            if (fixedAssetDetailDb != null)
            {
                _repository.Delete(fixedAssetDetailDb);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = id, Success = true, Message = _localizer["DeleteFixedAssetMovementDetailSuccessMessage"] };
            }
            return new ResponseDto() { Id = id, Success = false, Message = _localizer["NoFixedAssetMovementDetailFound"] };
        }

        public IQueryable<FixedAssetMovementDetailDto> GetAllFixedAssetMovementDetails()
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var data =
                from fixedAssetMovementDetail in _repository.GetAll()
                select new FixedAssetMovementDetailDto()
                {
                    FixedAssetMovementDetailId = fixedAssetMovementDetail.FixedAssetMovementDetailId,
                    FixedAssetMovementHeaderId = fixedAssetMovementDetail.FixedAssetMovementHeaderId,
                    FixedAssetId = fixedAssetMovementDetail.FixedAssetId,
                    FixedAssetCode = fixedAssetMovementDetail.FixedAsset == null ? "" : fixedAssetMovementDetail.FixedAsset.FixedAssetCode,
                    FixedAssetName = fixedAssetMovementDetail.FixedAsset == null ? "" : (language == LanguageCode.Arabic ? fixedAssetMovementDetail.FixedAsset.FixedAssetNameAr : fixedAssetMovementDetail.FixedAsset.FixedAssetNameEn),
                    MovementDate = fixedAssetMovementDetail.FixedAssetMovementHeader == null ? DateTime.MinValue : fixedAssetMovementDetail.FixedAssetMovementHeader.MovementDate,
                    CostCenterToId = fixedAssetMovementDetail.FixedAssetMovementHeader == null ? 0 : fixedAssetMovementDetail.FixedAssetMovementHeader.CostCenterToId,
                    RemarksAr = fixedAssetMovementDetail.RemarksAr,
                    RemarksEn = fixedAssetMovementDetail.RemarksEn
                };
            return data;
        }
        public async Task<List<FixedAssetMovementDetailDto>> GetFixedAssetMovementDetail(int headerId)
        {
            return await GetAllFixedAssetMovementDetails()
                .Where(x => x.FixedAssetMovementHeaderId == headerId)
                .ToListAsync();
        }

        public async Task<FixedAssetMovementDetailDto> GetFixedAssetMovementDetailById(int fixedAssetMovementDetailId)
        {
            if (fixedAssetMovementDetailId != 0)
            {
                var fixedAssetMovementDetailDb = await GetAllFixedAssetMovementDetails().FirstOrDefaultAsync(x => x.FixedAssetMovementDetailId == fixedAssetMovementDetailId);
                if (fixedAssetMovementDetailDb != null)
                {
                    return fixedAssetMovementDetailDb;
                }

            }
            return new FixedAssetMovementDetailDto();
        }
        public async Task<List<FixedAssetMovementDetailDto>> GetFixedAssetMovementDetailByFixedAssetId(int fixedAssetId)
        {
            if (fixedAssetId != 0)
            {
                var fixedAssetMovementDetailDb = await GetAllFixedAssetMovementDetails()
                    .Where(x => x.FixedAssetId == fixedAssetId)
                    .ToListAsync();
                if (fixedAssetMovementDetailDb != null)
                {
                    return fixedAssetMovementDetailDb;
                }

            }
            return new List<FixedAssetMovementDetailDto>();
        }
        public List<RequestChangesDto> GetFixedAssetMovementDetailRequestChanges(FixedAssetMovementDetailDto oldItem, FixedAssetMovementDetailDto newItem)
        {
            return CompareLogic.GetDifferences(oldItem, newItem);
        }
    }
}
