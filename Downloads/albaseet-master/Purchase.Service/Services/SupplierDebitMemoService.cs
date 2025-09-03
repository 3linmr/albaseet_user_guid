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
    public class SupplierDebitMemoService: BaseService<SupplierDebitMemo>, ISupplierDebitMemoService
    {
        private readonly ISupplierService _supplierService;
        private readonly IStoreService _storeService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMenuEncodingService _menuEncodingService;
        private readonly IPurchaseInvoiceHeaderService _purchaseInvoiceHeaderService;
        private readonly IStringLocalizer<SupplierDebitMemoService> _localizer;
        private readonly IGenericMessageService _genericMessageService;
        private readonly IApplicationSettingService _applicationSettingService;

        public SupplierDebitMemoService(IPurchaseInvoiceHeaderService purchaseInvoiceHeaderService, IRepository<SupplierDebitMemo> repository, ISupplierService supplierService, IStoreService storeService, IHttpContextAccessor httpContextAccessor, IMenuEncodingService menuEncodingService, IStringLocalizer<SupplierDebitMemoService> localizer, IGenericMessageService genericMessageService, IApplicationSettingService applicationSettingService) : base(repository) 
        {
            _purchaseInvoiceHeaderService = purchaseInvoiceHeaderService;
            _supplierService = supplierService;
            _storeService = storeService;
            _httpContextAccessor = httpContextAccessor;
            _menuEncodingService = menuEncodingService;
            _localizer = localizer;
            _genericMessageService = genericMessageService;
            _applicationSettingService = applicationSettingService;
        }

        public IQueryable<SupplierDebitMemoDto> GetSupplierDebitMemos()
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var data = from supplierDebitMemo in _repository.GetAll()
                       from store in _storeService.GetAll().Where(x => x.StoreId == supplierDebitMemo.StoreId)
                       from supplier in _supplierService.GetAll().Where(x => x.SupplierId == supplierDebitMemo.SupplierId)
                       from purchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == supplierDebitMemo.PurchaseInvoiceHeaderId)
                       select new SupplierDebitMemoDto
                       {
                           SupplierDebitMemoId = supplierDebitMemo.SupplierDebitMemoId,
                           Prefix = supplierDebitMemo.Prefix,
                           DocumentCode = supplierDebitMemo.DocumentCode,
                           Suffix = supplierDebitMemo.Suffix,
                           DocumentFullCode = $"{supplierDebitMemo.Prefix}{supplierDebitMemo.DocumentCode}{supplierDebitMemo.Suffix}",
                           DocumentDate = supplierDebitMemo.DocumentDate,
                           EntryDate = supplierDebitMemo.EntryDate,
                           DocumentReference = supplierDebitMemo.DocumentReference,
                           PurchaseInvoiceHeaderId = supplierDebitMemo.PurchaseInvoiceHeaderId,
                           SupplierId = supplierDebitMemo.SupplierId,
                           SupplierCode = supplier.SupplierCode,
                           SupplierName = language == LanguageCode.Arabic ? supplier.SupplierNameAr : supplier.SupplierNameEn,
                           StoreId = supplierDebitMemo.StoreId,
                           StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
                           Reference = supplierDebitMemo.Reference,
                           DebitAccountId = supplierDebitMemo.DebitAccountId,
                           CreditAccountId = supplierDebitMemo.CreditAccountId,
                           MemoValue = supplierDebitMemo.MemoValue,
                           JournalHeaderId = supplierDebitMemo.JournalHeaderId,
                           RemarksAr = supplierDebitMemo.RemarksAr,
                           RemarksEn = supplierDebitMemo.RemarksEn,
                           IsClosed = supplierDebitMemo.IsClosed || purchaseInvoiceHeader.IsBlocked,
                           ArchiveHeaderId = supplierDebitMemo.ArchiveHeaderId,

                           CreatedAt = supplierDebitMemo.CreatedAt,
                           UserNameCreated = supplierDebitMemo.UserNameCreated,
                           IpAddressCreated = supplierDebitMemo.IpAddressCreated,

                           ModifiedAt = supplierDebitMemo.ModifiedAt,
                           UserNameModified = supplierDebitMemo.UserNameModified,
                           IpAddressModified = supplierDebitMemo.IpAddressModified,
                       };

            return data;
        }

        public async Task<SupplierDebitMemoDto?> GetSupplierDebitMemoById(int supplierDebitMemoId)
        {
            return await GetSupplierDebitMemos().FirstOrDefaultAsync(x => x.SupplierDebitMemoId == supplierDebitMemoId);
        }

        public IQueryable<SupplierDebitMemoDto> GetUserSupplierDebitMemos()
        {
            var userStore = _httpContextAccessor.GetCurrentUserStore();
            return GetSupplierDebitMemos().Where(x => x.StoreId == userStore);
        }

        public IQueryable<SupplierDebitMemoDto> GetSupplierDebitMemosByStoreId(int storeId, int? supplierId, int supplierDebitMemoId)
        {
            supplierId ??= 0;
            if (supplierDebitMemoId == 0)
            {
                return GetSupplierDebitMemos().Where(x => x.StoreId == storeId && (supplierId == 0 || x.SupplierId == supplierId) && x.IsClosed == false);
            }
            else
            {
                return GetSupplierDebitMemos().Where(x => x.SupplierDebitMemoId == supplierDebitMemoId);
            }
        }

        public async Task<DocumentCodeDto> GetSupplierDebitMemoCode(int storeId, DateTime documentDate)
        {
            var separateYears = await _applicationSettingService.SeparateYears(storeId);
            return await GetSupplierDebitMemoCodeInternal(storeId, separateYears, documentDate);
        }

        public async Task<DocumentCodeDto> GetSupplierDebitMemoCodeInternal(int storeId, bool separateYears, DateTime documentDate)
        {
            var encoding = await _menuEncodingService.GetMenuEncoding(storeId, MenuCodeData.SupplierDebitMemo);
            var code = await GetNextSupplierDebitMemoCode(storeId, separateYears, documentDate, encoding.Prefix, encoding.Suffix);
            return new DocumentCodeDto() { NextCode = code, Prefix = encoding.Prefix, Suffix = encoding.Suffix };
        }

        private async Task<int> GetNextSupplierDebitMemoCode(int storeId, bool separateYears, DateTime documentDate, string? prefix, string? suffix)
        {
            int id = 1;
            try
            {
                id = await _repository.GetAll().AsNoTracking().Where(x => (!separateYears || x.DocumentDate.Year == documentDate.Year) && x.Prefix == prefix && x.Suffix == suffix && x.StoreId == storeId).MaxAsync(a => a.DocumentCode) + 1;
            }
            catch { id = 1; }
            return id;
        }

        public async Task<ResponseDto> SaveSupplierDebitMemo(SupplierDebitMemoDto supplierDebitMemo, bool hasApprove, bool approved, int? requestId)
        {
            var separateYears = await _applicationSettingService.SeparateYears(supplierDebitMemo.StoreId);

            if (hasApprove)
            {
                if (supplierDebitMemo.SupplierDebitMemoId == 0)
                {
                    return await CreateSupplierDebitMemo(supplierDebitMemo, hasApprove, approved, requestId, separateYears);
                }
                else
                {
                    return await UpdateSupplierDebitMemo(supplierDebitMemo);
                }
            }
            else
            {
                var supplierDebitMemoExist = await IsSupplierDebitMemoCodeExist(supplierDebitMemo.SupplierDebitMemoId, supplierDebitMemo.DocumentCode, supplierDebitMemo.StoreId, separateYears, supplierDebitMemo.DocumentDate, supplierDebitMemo.Prefix, supplierDebitMemo.Suffix);
                if (supplierDebitMemoExist)
                {
                    var nextSupplierDebitMemoCode = await GetNextSupplierDebitMemoCode(supplierDebitMemo.StoreId, separateYears, supplierDebitMemo.DocumentDate, supplierDebitMemo.Prefix, supplierDebitMemo.Suffix);
                    return new ResponseDto() { Id = nextSupplierDebitMemoCode, ResponseType = ResponseTypeData.Confirm, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.SupplierDebitMemo, GenericMessageData.CodeAlreadyExist, $"{nextSupplierDebitMemoCode}") };
                }
                else
                {
                    if (supplierDebitMemo.SupplierDebitMemoId == 0)
                    {
                        return await CreateSupplierDebitMemo(supplierDebitMemo, hasApprove, approved, requestId, separateYears);
                    }
                    else
                    {
                        return await UpdateSupplierDebitMemo(supplierDebitMemo);
                    }
                }
            }
        }

        private async Task<ResponseDto> CreateSupplierDebitMemo(SupplierDebitMemoDto supplierDebitMemo, bool hasApprove, bool approved, int? requestId, bool separateYears)
        {
            int supplierDebitMemoCode;
            string? prefix;
            string? suffix;
            var nextSupplierDebitMemoCode = await GetSupplierDebitMemoCodeInternal(supplierDebitMemo.StoreId, separateYears, supplierDebitMemo.DocumentDate);
            if (hasApprove && approved)
            {
                supplierDebitMemoCode = nextSupplierDebitMemoCode.NextCode;
                prefix = nextSupplierDebitMemoCode.Prefix;
                suffix = nextSupplierDebitMemoCode.Suffix;
            }
            else
            {
                supplierDebitMemoCode = supplierDebitMemo.DocumentCode != 0 ? supplierDebitMemo.DocumentCode : nextSupplierDebitMemoCode.NextCode;
                prefix = string.IsNullOrWhiteSpace(supplierDebitMemo.Prefix) ? nextSupplierDebitMemoCode.Prefix : supplierDebitMemo.Prefix;
                suffix = string.IsNullOrWhiteSpace(supplierDebitMemo.Suffix) ? nextSupplierDebitMemoCode.Suffix : supplierDebitMemo.Suffix;
            }

            int newSupplierDebitMemoId = await GetNextId();
            var newSupplierDebitMemo = new SupplierDebitMemo
            {
                SupplierDebitMemoId = newSupplierDebitMemoId,
                Prefix = prefix,
                DocumentCode = supplierDebitMemoCode,
                Suffix = suffix,
                DocumentDate = supplierDebitMemo.DocumentDate,
                EntryDate = DateHelper.GetDateTimeNow(),
                DocumentReference = hasApprove ? $"{DocumentReferenceData.Approval}{requestId}" : $"{DocumentReferenceData.SupplierDebitMemo}{newSupplierDebitMemoId}",
                PurchaseInvoiceHeaderId = supplierDebitMemo.PurchaseInvoiceHeaderId,
                SupplierId = supplierDebitMemo.SupplierId,
                StoreId = supplierDebitMemo.StoreId,
                Reference = supplierDebitMemo.Reference,
                DebitAccountId = supplierDebitMemo.DebitAccountId,
                CreditAccountId = supplierDebitMemo.CreditAccountId,
                MemoValue = supplierDebitMemo.MemoValue,
                JournalHeaderId = supplierDebitMemo.JournalHeaderId,
                RemarksAr = supplierDebitMemo.RemarksAr,
                RemarksEn = supplierDebitMemo.RemarksEn,
                IsClosed = false,
                ArchiveHeaderId = supplierDebitMemo.ArchiveHeaderId,

                CreatedAt = DateHelper.GetDateTimeNow(),
                UserNameCreated = await _httpContextAccessor!.GetUserName(),
                IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
                Hide = false,
            };

            var supplierDebitMemoValidator = await new SupplierDebitMemoValidator(_localizer).ValidateAsync(newSupplierDebitMemo);
            var validationResult = supplierDebitMemoValidator.IsValid;
            if (validationResult)
            {
                await _repository.Insert(newSupplierDebitMemo);
                await _repository.SaveChanges();
                return new ResponseDto { Id = newSupplierDebitMemoId, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.SupplierDebitMemo, GenericMessageData.CreatedSuccessWithCode, $"{newSupplierDebitMemo.Prefix}{newSupplierDebitMemo.DocumentCode}{newSupplierDebitMemo.Suffix}") };
            }
            else
            {
                return new ResponseDto { Id = newSupplierDebitMemoId, Success = false, Message = supplierDebitMemoValidator.ToString("~") };
            }
        }

        private async Task<ResponseDto> UpdateSupplierDebitMemo(SupplierDebitMemoDto supplierDebitMemo)
        {
            var supplierDebitMemoDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.SupplierDebitMemoId == supplierDebitMemo.SupplierDebitMemoId);
            if(supplierDebitMemoDb != null) {
                if (supplierDebitMemoDb.IsClosed)
                {
                    return new ResponseDto { Id = supplierDebitMemo.SupplierDebitMemoId, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.SupplierDebitMemo, GenericMessageData.CannotUpdateBecauseClosed) };
                }

                supplierDebitMemoDb.DocumentDate = supplierDebitMemo.DocumentDate;
                supplierDebitMemoDb.PurchaseInvoiceHeaderId = supplierDebitMemo.PurchaseInvoiceHeaderId;
                supplierDebitMemoDb.SupplierId = supplierDebitMemo.SupplierId;
                supplierDebitMemoDb.StoreId = supplierDebitMemo.StoreId;
                supplierDebitMemoDb.Reference = supplierDebitMemo.Reference;
                supplierDebitMemoDb.DebitAccountId = supplierDebitMemo.DebitAccountId;
                supplierDebitMemoDb.CreditAccountId = supplierDebitMemo.CreditAccountId;
                supplierDebitMemoDb.MemoValue = supplierDebitMemo.MemoValue;
                supplierDebitMemoDb.RemarksAr = supplierDebitMemo.RemarksAr;
                supplierDebitMemoDb.RemarksEn = supplierDebitMemo.RemarksEn;
                supplierDebitMemoDb.ArchiveHeaderId = supplierDebitMemo.ArchiveHeaderId;

                supplierDebitMemoDb.ModifiedAt = DateHelper.GetDateTimeNow();
                supplierDebitMemoDb.UserNameModified = await _httpContextAccessor!.GetUserName();
                supplierDebitMemoDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();

                var supplierDebitMemoValidator = await new SupplierDebitMemoValidator(_localizer).ValidateAsync(supplierDebitMemoDb);
                var validationResult = supplierDebitMemoValidator.IsValid;
                if (validationResult)
                {
                    _repository.Update(supplierDebitMemoDb);
                    await _repository.SaveChanges();
                    return new ResponseDto { Id = supplierDebitMemo.SupplierDebitMemoId, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.SupplierDebitMemo, GenericMessageData.UpdatedSuccessWithCode, $"{supplierDebitMemoDb.Prefix}{supplierDebitMemoDb.DocumentCode}{supplierDebitMemoDb.Suffix}") };
                }
                else
                {
                    return new ResponseDto { Id = supplierDebitMemo.SupplierDebitMemoId, Success = false, Message = supplierDebitMemoValidator.ToString("~") };
                }
            }
            return new ResponseDto { Id = supplierDebitMemo.SupplierDebitMemoId, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.SupplierDebitMemo, GenericMessageData.NotFound) };
        }

        public async Task<int> GetNextId()
        {
            int id = 1;
            try { id = await _repository.GetAll().MaxAsync(a => a.SupplierDebitMemoId) + 1; } catch { id = 1; }
            return id;
        }

        private async Task<bool> IsSupplierDebitMemoCodeExist(int supplierDebitMemoId, int documentCode, int storeId, bool separateYears, DateTime documentDate, string? prefix, string? suffix)
        {
            return await _repository.GetAll().Where(x => x.SupplierDebitMemoId != supplierDebitMemoId && (x.DocumentCode == documentCode && x.StoreId == storeId && (!separateYears || x.DocumentDate.Year == documentDate.Year) && x.Prefix == prefix && x.Suffix == suffix)).AnyAsync();
        }

        public async Task<ResponseDto> DeleteSupplierDebitMemo(int supplierDebitMemoId)
        {
            var supplierDebitMemo = await _repository.GetAll().FirstOrDefaultAsync(x => x.SupplierDebitMemoId == supplierDebitMemoId);
            if (supplierDebitMemo != null)
            {
                _repository.Delete(supplierDebitMemo);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = supplierDebitMemoId, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.SupplierDebitMemo, GenericMessageData.DeleteSuccessWithCode, $"{supplierDebitMemo.Prefix}{supplierDebitMemo.DocumentCode}{supplierDebitMemo.Suffix}") };
            }
            return new ResponseDto() { Id = supplierDebitMemoId, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.SupplierDebitMemo, GenericMessageData.NotFound) };
        }
    }
}
