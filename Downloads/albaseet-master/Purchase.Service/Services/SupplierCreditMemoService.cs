using Shared.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Purchases.CoreOne.Models.Domain;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Contracts.Journal;
using Microsoft.AspNetCore.Http;
using Shared.Helper.Identity;
using Purchases.CoreOne.Models.Dtos.ViewModels;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Shared.Helper.Models.Dtos;
using Purchases.CoreOne.Contracts;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne.Models.StaticData;
using Shared.CoreOne.Contracts.Menus;
using Microsoft.Extensions.Localization;
using Purchases.Service.Validators;
using FluentValidation;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.Service.Logic.Approval;
using Shared.Helper.Extensions;
using Shared.CoreOne.Models.Dtos.ViewModels.Journal;
using Shared.Helper.Logic;
using Shared.CoreOne.Contracts.Settings;

namespace Purchases.Service.Services
{
    public class SupplierCreditMemoService: BaseService<SupplierCreditMemo>, ISupplierCreditMemoService
    {
        private readonly IPurchaseInvoiceHeaderService _purchaseInvoiceHeaderService;
        private readonly ISupplierService _supplierService;
        private readonly IStoreService _storeService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMenuEncodingService _menuEncodingService;
        private readonly IGenericMessageService _genericMessageService;
        private readonly IStringLocalizer<SupplierCreditMemoService> _localizer;
        private readonly IApplicationSettingService _applicationSettingService;

        public SupplierCreditMemoService(IPurchaseInvoiceHeaderService purchaseInvoiceHeaderService, IRepository<SupplierCreditMemo> repository, ISupplierService supplierService, IStoreService storeService, IHttpContextAccessor httpContextAccessor, IMenuEncodingService menuEncodingService, IGenericMessageService genericMessageService, IStringLocalizer<SupplierCreditMemoService> localizer, IApplicationSettingService applicationSettingService) : base(repository) 
        {
            _purchaseInvoiceHeaderService = purchaseInvoiceHeaderService;
            _supplierService = supplierService;
            _storeService = storeService;
            _httpContextAccessor = httpContextAccessor;
            _menuEncodingService = menuEncodingService;
            _genericMessageService = genericMessageService;
            _localizer = localizer;
            _applicationSettingService = applicationSettingService;
        }

        public IQueryable<SupplierCreditMemoDto> GetSupplierCreditMemos()
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var data = from supplierCreditMemo in _repository.GetAll()
                       from store in _storeService.GetAll().Where(x => x.StoreId == supplierCreditMemo.StoreId)
                       from supplier in _supplierService.GetAll().Where(x => x.SupplierId == supplierCreditMemo.SupplierId)
                       from purchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == supplierCreditMemo.PurchaseInvoiceHeaderId)
                       select new SupplierCreditMemoDto
                       {
                           SupplierCreditMemoId = supplierCreditMemo.SupplierCreditMemoId,
                           Prefix = supplierCreditMemo.Prefix,
                           DocumentCode = supplierCreditMemo.DocumentCode,
                           Suffix = supplierCreditMemo.Suffix,
                           DocumentFullCode = $"{supplierCreditMemo.Prefix}{supplierCreditMemo.DocumentCode}{supplierCreditMemo.Suffix}",
                           DocumentDate = supplierCreditMemo.DocumentDate,
                           EntryDate = supplierCreditMemo.EntryDate,
                           DocumentReference = supplierCreditMemo.DocumentReference,
                           PurchaseInvoiceHeaderId = supplierCreditMemo.PurchaseInvoiceHeaderId,
                           SupplierId = supplierCreditMemo.SupplierId,
                           SupplierCode = supplier.SupplierCode,
                           SupplierName = language == LanguageCode.Arabic ? supplier.SupplierNameAr : supplier.SupplierNameEn,
                           StoreId = supplierCreditMemo.StoreId,
                           StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
                           Reference = supplierCreditMemo.Reference,
                           DebitAccountId = supplierCreditMemo.DebitAccountId,
                           CreditAccountId = supplierCreditMemo.CreditAccountId,
                           MemoValue = supplierCreditMemo.MemoValue,
                           JournalHeaderId = supplierCreditMemo.JournalHeaderId,
                           RemarksAr = supplierCreditMemo.RemarksAr,
                           RemarksEn = supplierCreditMemo.RemarksEn,
                           IsClosed = supplierCreditMemo.IsClosed || purchaseInvoiceHeader.IsBlocked,
                           ArchiveHeaderId = supplierCreditMemo.ArchiveHeaderId,

                           CreatedAt = supplierCreditMemo.CreatedAt,
                           UserNameCreated = supplierCreditMemo.UserNameCreated,
                           IpAddressCreated = supplierCreditMemo.IpAddressCreated,

                           ModifiedAt = supplierCreditMemo.ModifiedAt,
                           UserNameModified = supplierCreditMemo.UserNameModified,
                           IpAddressModified = supplierCreditMemo.IpAddressModified,
                       };

            return data;
        }

        public async Task<SupplierCreditMemoDto?> GetSupplierCreditMemoById(int supplierCreditMemoId)
        {
            return await GetSupplierCreditMemos().FirstOrDefaultAsync(x => x.SupplierCreditMemoId == supplierCreditMemoId);
        }

        public IQueryable<SupplierCreditMemoDto> GetUserSupplierCreditMemos()
        {
            var userStore = _httpContextAccessor.GetCurrentUserStore();
            return GetSupplierCreditMemos().Where(x => x.StoreId == userStore);
        }

        public IQueryable<SupplierCreditMemoDto> GetSupplierCreditMemosByStoreId(int storeId, int? supplierId, int supplierCreditMemoId)
        {
            supplierId ??= 0;
            if (supplierCreditMemoId == 0)
            {
                return GetSupplierCreditMemos().Where(x => x.StoreId == storeId && (supplierId == 0 || x.SupplierId == supplierId) && x.IsClosed == false);
            }
            else
            {
                return GetSupplierCreditMemos().Where(x => x.SupplierCreditMemoId == supplierCreditMemoId);
            }
        }

        public async Task<DocumentCodeDto> GetSupplierCreditMemoCode(int storeId, DateTime documentDate)
        {
            var separateYears = await _applicationSettingService.SeparateYears(storeId);
            return await GetSupplierCreditMemoCodeInternal(storeId, separateYears, documentDate);
        }

        public async Task<DocumentCodeDto> GetSupplierCreditMemoCodeInternal(int storeId, bool separateYears, DateTime documentDate)
        {
            var encoding = await _menuEncodingService.GetMenuEncoding(storeId, MenuCodeData.SupplierCreditMemo);
            var code = await GetNextSupplierCreditMemoCode(storeId, separateYears, documentDate, encoding.Prefix, encoding.Suffix);
            return new DocumentCodeDto() { NextCode = code, Prefix = encoding.Prefix, Suffix = encoding.Suffix };
        }

        private async Task<int> GetNextSupplierCreditMemoCode(int storeId, bool separateYears, DateTime documentDate, string? prefix, string? suffix)
        {
            int id = 1;
            try
            {
                id = await _repository.GetAll().AsNoTracking().Where(x => (!separateYears || x.DocumentDate.Year == documentDate.Year) && x.Prefix == prefix && x.Suffix == suffix && x.StoreId == storeId).MaxAsync(a => a.DocumentCode) + 1;
            }
            catch { id = 1; }
            return id;
        }

        public async Task<ResponseDto> SaveSupplierCreditMemo(SupplierCreditMemoDto supplierCreditMemo, bool hasApprove, bool approved, int? requestId)
        {
            var separateYears = await _applicationSettingService.SeparateYears(supplierCreditMemo.StoreId);

            if (hasApprove)
            {
                if (supplierCreditMemo.SupplierCreditMemoId == 0)
                {
                    return await CreateSupplierCreditMemo(supplierCreditMemo, hasApprove, approved, requestId, separateYears);
                }
                else
                {
                    return await UpdateSupplierCreditMemo(supplierCreditMemo);
                }
            }
            else
            {
                var supplierCreditMemoExist = await IsSupplierCreditMemoCodeExist(supplierCreditMemo.SupplierCreditMemoId, supplierCreditMemo.DocumentCode, supplierCreditMemo.StoreId, separateYears, supplierCreditMemo.DocumentDate, supplierCreditMemo.Prefix, supplierCreditMemo.Suffix);
                if (supplierCreditMemoExist)
                {
                    var nextSupplierCreditMemoCode = await GetNextSupplierCreditMemoCode(supplierCreditMemo.StoreId, separateYears, supplierCreditMemo.DocumentDate, supplierCreditMemo.Prefix, supplierCreditMemo.Suffix);
                    return new ResponseDto() { Id = nextSupplierCreditMemoCode, ResponseType = ResponseTypeData.Confirm, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.SupplierCreditMemo, GenericMessageData.CodeAlreadyExist, $"{nextSupplierCreditMemoCode}") };
                }
                else
                {
                    if (supplierCreditMemo.SupplierCreditMemoId == 0)
                    {
                        return await CreateSupplierCreditMemo(supplierCreditMemo, hasApprove, approved, requestId, separateYears);
                    }
                    else
                    {
                        return await UpdateSupplierCreditMemo(supplierCreditMemo);
                    }
                }
            }
        }

        private async Task<ResponseDto> CreateSupplierCreditMemo(SupplierCreditMemoDto supplierCreditMemo, bool hasApprove, bool approved, int? requestId, bool separateYears)
        {
            int supplierCreditMemoCode;
            string? prefix;
            string? suffix;
            var nextSupplierCreditMemoCode = await GetSupplierCreditMemoCodeInternal(supplierCreditMemo.StoreId, separateYears, supplierCreditMemo.DocumentDate);
            if (hasApprove && approved)
            {
                supplierCreditMemoCode = nextSupplierCreditMemoCode.NextCode;
                prefix = nextSupplierCreditMemoCode.Prefix;
                suffix = nextSupplierCreditMemoCode.Suffix;
            }
            else
            {
                supplierCreditMemoCode = supplierCreditMemo.DocumentCode != 0 ? supplierCreditMemo.DocumentCode : nextSupplierCreditMemoCode.NextCode;
                prefix = string.IsNullOrWhiteSpace(supplierCreditMemo.Prefix) ? nextSupplierCreditMemoCode.Prefix : supplierCreditMemo.Prefix;
                suffix = string.IsNullOrWhiteSpace(supplierCreditMemo.Suffix) ? nextSupplierCreditMemoCode.Suffix : supplierCreditMemo.Suffix;
            }

            int newSupplierCreditMemoId = await GetNextId();
            var newSupplierCreditMemo = new SupplierCreditMemo
            {
                SupplierCreditMemoId = newSupplierCreditMemoId,
                Prefix = prefix,
                DocumentCode = supplierCreditMemoCode,
                Suffix = suffix,
                DocumentDate = supplierCreditMemo.DocumentDate,
                EntryDate = DateHelper.GetDateTimeNow(),
                DocumentReference = hasApprove ? $"{DocumentReferenceData.Approval}{requestId}" : $"{DocumentReferenceData.SupplierCreditMemo}{newSupplierCreditMemoId}",
                PurchaseInvoiceHeaderId = supplierCreditMemo.PurchaseInvoiceHeaderId,
                SupplierId = supplierCreditMemo.SupplierId,
                StoreId = supplierCreditMemo.StoreId,
                Reference = supplierCreditMemo.Reference,
                DebitAccountId = supplierCreditMemo.DebitAccountId,
                CreditAccountId = supplierCreditMemo.CreditAccountId,
                MemoValue = supplierCreditMemo.MemoValue,
                JournalHeaderId = supplierCreditMemo.JournalHeaderId,
                RemarksAr = supplierCreditMemo.RemarksAr,
                RemarksEn = supplierCreditMemo.RemarksEn,
                IsClosed = false,
                ArchiveHeaderId = supplierCreditMemo.ArchiveHeaderId,

                CreatedAt = DateHelper.GetDateTimeNow(),
                UserNameCreated = await _httpContextAccessor!.GetUserName(),
                IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
                Hide = false,
            };

            var supplierCreditMemoValidator = await new SupplierCreditMemoValidator(_localizer).ValidateAsync(newSupplierCreditMemo);
            var validationResult = supplierCreditMemoValidator.IsValid;
            if (validationResult)
            {
                await _repository.Insert(newSupplierCreditMemo);
                await _repository.SaveChanges();
                return new ResponseDto { Id = newSupplierCreditMemoId, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.SupplierCreditMemo, GenericMessageData.CreatedSuccessWithCode, $"{newSupplierCreditMemo.Prefix}{newSupplierCreditMemo.DocumentCode}{newSupplierCreditMemo.Suffix}") };
            }
            else
            {
                return new ResponseDto { Id = newSupplierCreditMemoId, Success = false, Message = supplierCreditMemoValidator.ToString("~") };
            }
        }

        private async Task<ResponseDto> UpdateSupplierCreditMemo(SupplierCreditMemoDto supplierCreditMemo)
        {
            var supplierCreditMemoDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.SupplierCreditMemoId == supplierCreditMemo.SupplierCreditMemoId);
            if(supplierCreditMemoDb != null) {
                if (supplierCreditMemoDb.IsClosed)
                {
                    return new ResponseDto { Id = supplierCreditMemo.SupplierCreditMemoId, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.SupplierCreditMemo, GenericMessageData.CannotUpdateBecauseClosed) };
                }

                supplierCreditMemoDb.DocumentDate = supplierCreditMemo.DocumentDate;
                supplierCreditMemoDb.PurchaseInvoiceHeaderId = supplierCreditMemo.PurchaseInvoiceHeaderId;
                supplierCreditMemoDb.SupplierId = supplierCreditMemo.SupplierId;
                supplierCreditMemoDb.StoreId = supplierCreditMemo.StoreId;
                supplierCreditMemoDb.Reference = supplierCreditMemo.Reference;
                supplierCreditMemoDb.DebitAccountId = supplierCreditMemo.DebitAccountId;
                supplierCreditMemoDb.CreditAccountId = supplierCreditMemo.CreditAccountId;
                supplierCreditMemoDb.MemoValue = supplierCreditMemo.MemoValue;
                supplierCreditMemoDb.RemarksAr = supplierCreditMemo.RemarksAr;
                supplierCreditMemoDb.RemarksEn = supplierCreditMemo.RemarksEn;
                supplierCreditMemoDb.ArchiveHeaderId = supplierCreditMemo.ArchiveHeaderId;

                supplierCreditMemoDb.ModifiedAt = DateHelper.GetDateTimeNow();
                supplierCreditMemoDb.UserNameModified = await _httpContextAccessor!.GetUserName();
                supplierCreditMemoDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();

                var supplierCreditMemoValidator = await new SupplierCreditMemoValidator(_localizer).ValidateAsync(supplierCreditMemoDb);
                var validationResult = supplierCreditMemoValidator.IsValid;
                if (validationResult)
                {
                    _repository.Update(supplierCreditMemoDb);
                    await _repository.SaveChanges();
                    return new ResponseDto { Id = supplierCreditMemo.SupplierCreditMemoId, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.SupplierCreditMemo, GenericMessageData.UpdatedSuccessWithCode, $"{supplierCreditMemoDb.Prefix}{supplierCreditMemoDb.DocumentCode}{supplierCreditMemoDb.Suffix}") };
                }
                else
                {
                    return new ResponseDto { Id = supplierCreditMemo.SupplierCreditMemoId, Success = false, Message = supplierCreditMemoValidator.ToString("~") };
                }
            }
            return new ResponseDto { Id = supplierCreditMemo.SupplierCreditMemoId, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.SupplierCreditMemo, GenericMessageData.NotFound) };
        }

        public async Task<int> GetNextId()
        {
            int id = 1;
            try { id = await _repository.GetAll().MaxAsync(a => a.SupplierCreditMemoId) + 1; } catch { id = 1; }
            return id;
        }

        private async Task<bool> IsSupplierCreditMemoCodeExist(int supplierCreditMemoId, int documentCode, int storeId, bool separateYears, DateTime documentDate, string? prefix, string? suffix)
        {
            return await _repository.GetAll().Where(x => x.SupplierCreditMemoId != supplierCreditMemoId && (x.DocumentCode == documentCode && x.StoreId == storeId && (!separateYears || x.DocumentDate.Year == documentDate.Year) && x.Prefix == prefix && x.Suffix == suffix)).AnyAsync();
        }

        public async Task<ResponseDto> DeleteSupplierCreditMemo(int supplierCreditMemoId)
        {
            var supplierCreditMemo = await _repository.GetAll().FirstOrDefaultAsync(x => x.SupplierCreditMemoId == supplierCreditMemoId);
            if (supplierCreditMemo != null)
            {
                _repository.Delete(supplierCreditMemo);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = supplierCreditMemoId, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.SupplierCreditMemo, GenericMessageData.DeleteSuccessWithCode, $"{supplierCreditMemo.Prefix}{supplierCreditMemo.DocumentCode}{supplierCreditMemo.Suffix}") };
            }
            return new ResponseDto() { Id = supplierCreditMemoId, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.SupplierCreditMemo, GenericMessageData.NotFound) };
        }
    }
}
