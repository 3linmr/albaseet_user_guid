using Accounting.CoreOne.Contracts;
using Accounting.CoreOne.Models.Domain;
using Accounting.CoreOne.Models.Dtos.ViewModels;
using Accounting.Service.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Contracts.Menus;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.StaticData;
using Shared.CoreOne;
using Shared.Helper.Models.Dtos;
using Shared.Helper.Models.StaticData;
using Shared.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.CoreOne.Contracts.Settings;
using Shared.Service.Logic.Tree;

namespace Accounting.Service.Services
{
    internal class FixedAssetVoucherHeaderService : BaseService<FixedAssetVoucherHeader>, IFixedAssetVoucherHeaderService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISellerService _sellerService;
        private readonly IStoreService _storeService;
        private readonly IMenuEncodingService _menuEncodingService;
        private readonly IStringLocalizer<FixedAssetVoucherHeaderService> _localizer;
        private readonly IBranchService _branchService;
        private readonly ICompanyService _companyService;
        private readonly IApplicationSettingService _applicationSettingService;

        public FixedAssetVoucherHeaderService(IRepository<FixedAssetVoucherHeader> repository, IHttpContextAccessor httpContextAccessor, ISellerService sellerService, IStoreService storeService, IMenuEncodingService menuEncodingService, IStringLocalizer<FixedAssetVoucherHeaderService> localizer, IBranchService branchService, ICompanyService companyService, IApplicationSettingService applicationSettingService) : base(repository)
        {
            _httpContextAccessor = httpContextAccessor;
            _sellerService = sellerService;
            _storeService = storeService;
            _menuEncodingService = menuEncodingService;
            _localizer = localizer;
            _branchService = branchService;
            _companyService = companyService;
            _applicationSettingService = applicationSettingService;
        }

        public IQueryable<FixedAssetVoucherHeaderDto> GetFixedAssetVoucherHeaders()
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var data =
                from fixedAssetVoucherHeader in _repository.GetAll()
                from store in _storeService.GetAll().Where(x => x.StoreId == fixedAssetVoucherHeader.StoreId)
                from branch in _branchService.GetAll().Where(x => x.BranchId == store.BranchId)
                from company in _companyService.GetAll().Where(x => x.CompanyId == branch.CompanyId)
                from seller in _sellerService.GetAll().Where(x => x.SellerId == fixedAssetVoucherHeader.SellerId).DefaultIfEmpty()
                select new FixedAssetVoucherHeaderDto
                {
                    FixedAssetVoucherHeaderId = fixedAssetVoucherHeader.FixedAssetVoucherHeaderId,
                    DocumentReference = fixedAssetVoucherHeader.DocumentReference,
                    EntryDate = fixedAssetVoucherHeader.EntryDate,
                    DocumentDate = fixedAssetVoucherHeader.DocumentDate,
                    FixedAssetReference = fixedAssetVoucherHeader.FixedAssetReference,
                    PeerReference = fixedAssetVoucherHeader.PeerReference,
                    Prefix = fixedAssetVoucherHeader.Prefix,
                    Suffix = fixedAssetVoucherHeader.Suffix,
                    JournalHeaderId = fixedAssetVoucherHeader.JournalHeaderId,
                    DocumentCode = fixedAssetVoucherHeader.DocumentCode,
                    //FixedAssetVoucherCodeFull = $"{fixedAssetVoucherHeader.Prefix}{fixedAssetVoucherHeader.FixedAssetVoucherCode}{fixedAssetVoucherHeader.Suffix}",
                    RemarksAr = fixedAssetVoucherHeader.RemarksAr,
                    RemarksEn = fixedAssetVoucherHeader.RemarksEn,
                    SellerId = fixedAssetVoucherHeader.SellerId ?? 0,
                    SellerName = seller != null ? language == LanguageCode.Arabic ? seller.SellerNameAr : seller.SellerNameEn : "",
                    StoreId = fixedAssetVoucherHeader.StoreId,
                    StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
                    ArchiveHeaderId = fixedAssetVoucherHeader.ArchiveHeaderId,
                    StoreCurrencyId = company.CurrencyId,
                };
            return data;
        }

        public async Task<IQueryable<FixedAssetVoucherHeaderDto>> GetUserFixedAssetVoucherHeaders()
        {
            var userStore = await _httpContextAccessor.GetUserStores();
            return GetFixedAssetVoucherHeaders().Where(x => userStore.Contains(x.StoreId??0));
        }

        public async Task<FixedAssetVoucherHeaderDto> GetFixedAssetVoucherHeaderById(int headerId)
        {
            return await GetFixedAssetVoucherHeaders().FirstOrDefaultAsync(x => x.FixedAssetVoucherHeaderId == headerId) ?? new FixedAssetVoucherHeaderDto();
        }

        public async Task<DateTime> GetFixedAssetVoucherMaxDate()
        {
            return await GetFixedAssetVoucherHeaders().MaxAsync(x => x.DocumentDate ?? DateTime.UtcNow);
        }
        public async Task<string> GetNextFixedAssetAdditionCodeByStoreId(int storeId)
        {
            var nextFixedAssetAdditionHeader = await _repository.GetAll().CountAsync(x => x.StoreId == storeId && x.FixedAssetVoucherTypeId == FixedAssetVoucherTypeData.Addition) + 1;
            var codes = await _repository.GetAll().Where(x => x.StoreId == storeId && x.FixedAssetVoucherTypeId == FixedAssetVoucherTypeData.Addition).Select(x => Convert.ToString(x.DocumentCode)).ToListAsync();

            return TreeLogic.GenerateNextCode(codes!, "", true, "0", "0", nextFixedAssetAdditionHeader);
        }
        public async Task<string> GetNextFixedAssetExclusionCodeByStoreId(int storeId)
        {
            var nextFixedAssetExclusionHeader = await _repository.GetAll().CountAsync(x => x.StoreId == storeId && x.FixedAssetVoucherTypeId == FixedAssetVoucherTypeData.Exclusion) + 1;
            var codes = await _repository.GetAll().Where(x => x.StoreId == storeId && x.FixedAssetVoucherTypeId == FixedAssetVoucherTypeData.Exclusion).Select(x => Convert.ToString(x.DocumentCode)).ToListAsync();

            return TreeLogic.GenerateNextCode(codes!, "", true, "0", "0", nextFixedAssetExclusionHeader);
        }
        //public async Task<DocumentCodeDto> GetFixedAssetVoucherCode(int storeId, DateTime documentDate)
        //{
        //    var separateYears = await _applicationSettingService.SeparateYears(storeId);
        //    return await GetFixedAssetVoucherCodeInternal(storeId, separateYears, documentDate);
        //}

        //public async Task<DocumentCodeDto> GetFixedAssetVoucherCodeInternal(int storeId, bool separateYears, DateTime documentDate)
        //{
        //    var encoding = await _menuEncodingService.GetMenuEncoding(storeId, MenuCodeData.FixedAssetVoucher);
        //    var code = await GetNextFixedAssetVoucherCode(storeId, separateYears, documentDate, encoding.Prefix, encoding.Suffix);
        //    return new DocumentCodeDto() { NextCode = code, Prefix = encoding.Prefix, Suffix = encoding.Suffix };
        //}

        public async Task<int> GetNextFixedAssetVoucherCode(int storeId, bool separateYears, DateTime documentDate, string? prefix, string? suffix)
        {
            int id = 1;
            try { id = await _repository.GetAll().Where(x => x.StoreId == storeId && (!separateYears || x.DocumentDate.Year == documentDate.Year) && x.Prefix == prefix && x.Suffix == suffix).MaxAsync(a => a.DocumentCode) + 1; } catch { id = 1; }
            return id;
        }

        public async Task<int> GetNextId()
        {
            int id = 1;
            try { id = await _repository.GetAll().MaxAsync(a => a.FixedAssetVoucherHeaderId) + 1; } catch { id = 1; }
            return id;
        }

        public async Task<ResponseDto> SaveFixedAssetVoucherHeader(FixedAssetVoucherHeaderDto fixedAssetVoucher, bool hasApprove, bool approved, int? requestId)
        {
            var separateYears = await _applicationSettingService.SeparateYears(fixedAssetVoucher.StoreId ?? 0);

            if (hasApprove)
            {
                if (fixedAssetVoucher.FixedAssetVoucherHeaderId == 0)
                {
                    return await CreateFixedAssetVoucher(fixedAssetVoucher, hasApprove, approved, requestId, separateYears);
                }
                else
                {
                    return await UpdateFixedAssetVoucher(fixedAssetVoucher);
                }
            }
            else
            {
                var fixedAssetVoucherExist = await IsFixedAssetVoucherCodeExist(fixedAssetVoucher.FixedAssetVoucherHeaderId ?? 0, fixedAssetVoucher.DocumentCode ?? 0, fixedAssetVoucher.StoreId ?? 0, fixedAssetVoucher.Prefix, fixedAssetVoucher.Suffix, separateYears, fixedAssetVoucher.DocumentDate ?? DateTime.UtcNow);
                if (fixedAssetVoucherExist.Success)
                {
                    return new ResponseDto() { Id = 0, ResponseType = ResponseTypeData.Confirm, Success = false, Message = _localizer["FixedAssetVoucherAlreadyExist"] };
                }
                else
                {
                    if (fixedAssetVoucher.FixedAssetVoucherHeaderId == 0)
                    {
                        return await CreateFixedAssetVoucher(fixedAssetVoucher, hasApprove, approved, requestId, separateYears);
                    }
                    else
                    {
                        return await UpdateFixedAssetVoucher(fixedAssetVoucher);
                    }
                }
            }
        }

        public async Task<bool> UpdateFixedAssetVoucherWithJournalHeaderId(int fixedAssetVoucherHeaderId, int journalHeaderId)
        {
            var fixedAssetVoucher = await _repository.GetAll().FirstOrDefaultAsync(x => x.FixedAssetVoucherHeaderId == fixedAssetVoucherHeaderId);
            if (fixedAssetVoucher != null)
            {
                fixedAssetVoucher.JournalHeaderId = journalHeaderId;
                _repository.Update(fixedAssetVoucher);
                await _repository.SaveChanges();
            }
            return true;
        }

        public async Task<ResponseDto> CreateFixedAssetVoucher(FixedAssetVoucherHeaderDto fixedAssetVoucher, bool hasApprove, bool approved, int? requestId, bool separateYears)
        {
            var documentCode = 0;
            if (hasApprove)
            {
                if (approved)
                {
                    documentCode = await GetNextFixedAssetVoucherCode(fixedAssetVoucher.StoreId ?? 0, separateYears, fixedAssetVoucher.DocumentDate ?? DateTime.UtcNow, fixedAssetVoucher.Prefix, fixedAssetVoucher.Suffix);
                }
            }
            else
            {
                documentCode = fixedAssetVoucher.DocumentCode ?? 0;
            }

            var fixedAssetVoucherId = await GetNextId();
            var newFixedAssetVoucher = new FixedAssetVoucherHeader()
            {
                FixedAssetVoucherHeaderId = fixedAssetVoucherId,
                StoreId = fixedAssetVoucher.StoreId ?? 0,
                DocumentDate = fixedAssetVoucher.DocumentDate ?? DateTime.UtcNow,
                DocumentReference = hasApprove ? $"{DocumentReferenceData.Approval}{requestId}" : $"{DocumentReferenceData.FixedAssetVoucher}{fixedAssetVoucherId}",
                EntryDate = DateHelper.GetDateTimeNow(),
                PeerReference = fixedAssetVoucher.PeerReference,
                Prefix = fixedAssetVoucher.Prefix,
                Suffix = fixedAssetVoucher.Suffix,
                RemarksAr = fixedAssetVoucher.RemarksAr,
                RemarksEn = fixedAssetVoucher.RemarksEn,
                CreatedAt = DateHelper.GetDateTimeNow(),
                UserNameCreated = await _httpContextAccessor!.GetUserName(),
                IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
                Hide = false,
                DocumentCode = documentCode,
                FixedAssetReference = fixedAssetVoucher.FixedAssetReference,
                SellerId = fixedAssetVoucher.SellerId != 0 ? fixedAssetVoucher.SellerId : null,
                JournalHeaderId = fixedAssetVoucher.JournalHeaderId,
                FixedAssetVoucherTypeId = fixedAssetVoucher.FixedAssetVoucherTypeId??0,                
            };

            var fixedAssetVoucherValidator = await new FixedAssetVoucherValidator(_localizer).ValidateAsync(newFixedAssetVoucher);
            var validationResult = fixedAssetVoucherValidator.IsValid;
            if (validationResult)
            {
                await _repository.Insert(newFixedAssetVoucher);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = newFixedAssetVoucher.FixedAssetVoucherHeaderId, Success = true, Message = _localizer["NewFixedAssetVoucherSuccessMessage", $"{fixedAssetVoucher.Prefix}{newFixedAssetVoucher.DocumentCode}{fixedAssetVoucher.Suffix}"] };
            }
            else
            {
                return new ResponseDto() { Id = newFixedAssetVoucher.FixedAssetVoucherHeaderId, Success = false, Message = fixedAssetVoucherValidator.ToString("~") };
            }
        }

        public async Task<ResponseDto> UpdateFixedAssetVoucher(FixedAssetVoucherHeaderDto fixedAssetVoucher)
        {
            var fixedAssetVoucherDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.FixedAssetVoucherHeaderId == fixedAssetVoucher.FixedAssetVoucherHeaderId);
            if (fixedAssetVoucherDb != null)
            {
                fixedAssetVoucherDb.Prefix = fixedAssetVoucher.Prefix;
                fixedAssetVoucherDb.DocumentCode = fixedAssetVoucher.DocumentCode ?? 0;
                fixedAssetVoucherDb.Suffix = fixedAssetVoucher.Suffix;
                fixedAssetVoucherDb.PeerReference = fixedAssetVoucher.PeerReference;
                fixedAssetVoucherDb.RemarksAr = fixedAssetVoucher.RemarksAr;
                fixedAssetVoucherDb.RemarksEn = fixedAssetVoucher.RemarksEn;
                fixedAssetVoucherDb.StoreId = fixedAssetVoucher.StoreId ?? 0;
                fixedAssetVoucherDb.FixedAssetReference = fixedAssetVoucher.FixedAssetReference;
                fixedAssetVoucherDb.SellerId = fixedAssetVoucher.SellerId != 0 ? fixedAssetVoucher.SellerId : null;
                fixedAssetVoucherDb.ModifiedAt = DateHelper.GetDateTimeNow();
                fixedAssetVoucherDb.UserNameModified = await _httpContextAccessor!.GetUserName();
                fixedAssetVoucherDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();

                var fixedAssetVoucherValidator = await new FixedAssetVoucherValidator(_localizer).ValidateAsync(fixedAssetVoucherDb);
                var validationResult = fixedAssetVoucherValidator.IsValid;
                if (validationResult)
                {
                    _repository.Update(fixedAssetVoucherDb);
                    await _repository.SaveChanges();
                    return new ResponseDto() { Id = fixedAssetVoucherDb.FixedAssetVoucherHeaderId, Success = true, Message = _localizer["UpdateFixedAssetVoucherSuccessMessage", $"{fixedAssetVoucherDb.Prefix}{fixedAssetVoucherDb.DocumentCode}{fixedAssetVoucherDb.Suffix}"] };
                }
                else
                {
                    return new ResponseDto() { Id = 0, Success = false, Message = fixedAssetVoucherValidator.ToString("~") };
                }
            }
            return new ResponseDto() { Id = 0, Success = false, Message = _localizer["NoFixedAssetVoucherFound"] };
        }

        public async Task<ResponseDto> IsFixedAssetVoucherCodeExist(int fixedAssetVoucherHeaderId, int fixedAssetVoucherCode, int storeId, string? prefix, string? suffix, bool separateYears, DateTime documentDate)
        {
            var fixedAssetVoucher = await _repository.GetAll().FirstOrDefaultAsync(x => x.StoreId == storeId && x.DocumentCode == fixedAssetVoucherCode && x.Prefix == prefix && x.Suffix == suffix && (!separateYears || x.DocumentDate.Year == documentDate.Year) && x.FixedAssetVoucherHeaderId != fixedAssetVoucherHeaderId);
            if (fixedAssetVoucher != null)
            {
                return new ResponseDto() { Id = fixedAssetVoucher.FixedAssetVoucherHeaderId, Success = true };
            }
            return new ResponseDto() { Id = 0, Success = false };
        }
        public async Task<ResponseDto> DeleteFixedAssetVoucherHeader(int id)
        {
            var fixedAssetVoucherDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.FixedAssetVoucherHeaderId == id);
            if (fixedAssetVoucherDb != null)
            {
                _repository.Delete(fixedAssetVoucherDb);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = id, Success = true, Message = _localizer["DeleteFixedAssetVoucherMessage", $"{fixedAssetVoucherDb.Prefix}{fixedAssetVoucherDb.DocumentCode}{fixedAssetVoucherDb.Suffix}"] };
            }
            return new ResponseDto() { Id = id, Success = false, Message = _localizer["NoFixedAssetVoucherFound"] };
        }

    }
}
