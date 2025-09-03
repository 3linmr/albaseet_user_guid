using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Contracts.Menus;
using Shared.CoreOne.Contracts.Modules;
using Shared.Service;
using Purchases.CoreOne.Models.Domain;
using Shared.CoreOne;
using Purchases.CoreOne.Contracts;
using Shared.Helper.Models.Dtos;
using Purchases.CoreOne.Models.Dtos.ViewModels;
using Shared.Helper.Identity;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Microsoft.EntityFrameworkCore;
using Purchases.Service.Validators;
using Shared.CoreOne.Models.StaticData;
using System.Security;
using Shared.CoreOne.Models.Domain.Modules;
using static Shared.Helper.Models.StaticData.LanguageData;
using Inventory.CoreOne.Models.Domain;
using Shared.CoreOne.Models.Domain.Archive;
using Shared.Helper.Logic;
using Shared.CoreOne.Contracts.Settings;

namespace Purchases.Service.Services
{
	public class SupplierQuotationHeaderService : BaseService<SupplierQuotationHeader>, ISupplierQuotationHeaderService
	{
        private readonly IGenericMessageService _genericMessageService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IStoreService _storeService;
        private readonly ISupplierService _supplierService;
		private readonly IStringLocalizer<SupplierQuotationHeaderService> _localizer;
		private readonly IMenuEncodingService _menuEncodingService;
        private readonly IApplicationSettingService _applicationSettingService;

		public SupplierQuotationHeaderService(IGenericMessageService genericMessageService, IRepository<SupplierQuotationHeader> repository, IHttpContextAccessor httpContextAccessor, IStoreService storeService, IStringLocalizer<SupplierQuotationHeaderService> localizer, IMenuEncodingService menuEncodingService, ISupplierService supplierService, IApplicationSettingService applicationSettingService) : base(repository)
		{
            _genericMessageService = genericMessageService;
			_httpContextAccessor = httpContextAccessor;
			_storeService = storeService;
            _supplierService = supplierService;
			_localizer = localizer;
			_menuEncodingService = menuEncodingService;
            _applicationSettingService = applicationSettingService;
		}

		public IQueryable<SupplierQuotationHeaderDto> GetSupplierQuotationHeaders()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var data = from supplierQuotationHeader in _repository.GetAll()
				from store in _storeService.GetAll().Where(x => x.StoreId == supplierQuotationHeader.StoreId)
				from supplier in _supplierService.GetAll().Where(x => x.SupplierId == supplierQuotationHeader.SupplierId).DefaultIfEmpty()
                select new SupplierQuotationHeaderDto()
				{
					SupplierQuotationHeaderId = supplierQuotationHeader.SupplierQuotationHeaderId,
					Prefix = supplierQuotationHeader.Prefix,
					DocumentCode = supplierQuotationHeader.DocumentCode,
					Suffix = supplierQuotationHeader.Suffix,
					DocumentFullCode = $"{supplierQuotationHeader.Prefix}{supplierQuotationHeader.DocumentCode}{supplierQuotationHeader.Suffix}",
					ProductRequestPriceHeaderId = supplierQuotationHeader.ProductRequestPriceHeaderId,
                    SupplierId = supplierQuotationHeader.SupplierId,
                    SupplierCode = supplier.SupplierCode,
                    SupplierName = supplier != null? language == LanguageCode.Arabic ? supplier.SupplierNameAr : supplier.SupplierNameEn : null,
                    DocumentReference = supplierQuotationHeader.DocumentReference,
                    StoreId = supplierQuotationHeader.StoreId,
					StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
                    TaxTypeId = supplierQuotationHeader.TaxTypeId,
					DocumentDate = supplierQuotationHeader.DocumentDate,
					EntryDate = supplierQuotationHeader.EntryDate,
                    Reference = supplierQuotationHeader.Reference,
					TotalValue = supplierQuotationHeader.TotalValue,
					DiscountPercent = supplierQuotationHeader.DiscountPercent,
					DiscountValue = supplierQuotationHeader.DiscountValue,
					TotalItemDiscount = supplierQuotationHeader.TotalItemDiscount,
					GrossValue = supplierQuotationHeader.GrossValue,
					VatValue = supplierQuotationHeader.VatValue,
					SubNetValue = supplierQuotationHeader.SubNetValue,
					OtherTaxValue = supplierQuotationHeader.OtherTaxValue,
                    NetValueBeforeAdditionalDiscount = supplierQuotationHeader.NetValueBeforeAdditionalDiscount,
                    AdditionalDiscountValue = supplierQuotationHeader.AdditionalDiscountValue,
					NetValue = supplierQuotationHeader.NetValue,
                    TotalCostValue = supplierQuotationHeader.TotalCostValue,
					RemarksAr = supplierQuotationHeader.RemarksAr,
					RemarksEn = supplierQuotationHeader.RemarksEn,
					IsClosed = supplierQuotationHeader.IsClosed,
					ArchiveHeaderId = supplierQuotationHeader.ArchiveHeaderId,

                    CreatedAt = supplierQuotationHeader.CreatedAt,
                    UserNameCreated = supplierQuotationHeader.UserNameCreated,
                    IpAddressCreated = supplierQuotationHeader.IpAddressCreated,

                    ModifiedAt = supplierQuotationHeader.ModifiedAt,
                    UserNameModified = supplierQuotationHeader.UserNameModified,
                    IpAddressModified = supplierQuotationHeader.IpAddressModified
                };
			return data;
		}

        public IQueryable<SupplierQuotationHeaderDto> GetUserSupplierQuotationHeaders()
        {
            var userStore = _httpContextAccessor.GetCurrentUserStore();
            return GetSupplierQuotationHeaders().Where(x => x.StoreId == userStore);
        }

		public IQueryable<SupplierQuotationHeaderDto> GetSupplierQuotationHeadersByStoreId(int storeId, int? supplierId, int supplierQuotationHeaderId)
		{
            supplierId ??= 0;
            if (supplierQuotationHeaderId == 0)
            {
                return GetSupplierQuotationHeaders().Where(x => x.StoreId == storeId && (supplierId == 0 || x.SupplierId == supplierId) && x.IsClosed == false && x.TaxTypeId != 0);
            }
            else
            {
                return GetSupplierQuotationHeaders().Where(x => x.SupplierQuotationHeaderId == supplierQuotationHeaderId);
            }
        }

        public async Task<SupplierQuotationHeaderDto?> GetSupplierQuotationHeaderById(int id)
		{
			return await GetSupplierQuotationHeaders().FirstOrDefaultAsync(x => x.SupplierQuotationHeaderId == id);
		}


        public async Task<DocumentCodeDto> GetSupplierQuotationCode(int storeId, DateTime documentDate)
        {
            var separateYears = await _applicationSettingService.SeparateYears(storeId);
            return await GetSupplierQuotationCodeInternal(storeId, separateYears, documentDate);
        }

        public async Task<DocumentCodeDto> GetSupplierQuotationCodeInternal(int storeId, bool separateYears, DateTime documentDate)
        {
            var encoding = await _menuEncodingService.GetMenuEncoding(storeId, MenuCodeData.SupplierQuotation);
            var code = await GetNextSupplierQuotationCode(storeId, separateYears, documentDate, encoding.Prefix, encoding.Suffix);
            return new DocumentCodeDto() { NextCode = code, Prefix = encoding.Prefix, Suffix = encoding.Suffix };
        }

        public async Task<ResponseDto> UpdateClosed(int? supplierQuotationHeaderId, bool isClosed)
        {
            var header = await _repository.FindBy(x => x.SupplierQuotationHeaderId == supplierQuotationHeaderId).FirstOrDefaultAsync();
            if (header is null) return new ResponseDto { Id = (int)supplierQuotationHeaderId!, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.SupplierQuotation, GenericMessageData.NotFound) };

            header.IsClosed = isClosed;
            _repository.Update(header);
            await _repository.SaveChanges();
            return new ResponseDto { Id = (int)supplierQuotationHeaderId!, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.SupplierQuotation, isClosed ? GenericMessageData.ClosedSuccessfully : GenericMessageData.OpenedSuccessfully) };
        }
        public async Task<ResponseDto> SaveSupplierQuotationHeader(SupplierQuotationHeaderDto supplierQuotationHeader, bool hasApprove, bool approved, int? requestId)
		{
            var separateYears = await _applicationSettingService.SeparateYears(supplierQuotationHeader.StoreId);

            if (hasApprove)
            {
                if (supplierQuotationHeader.SupplierQuotationHeaderId == 0)
                {
                    return await CreateSupplierQuotationHeader(supplierQuotationHeader, hasApprove, approved, requestId, separateYears);
                }
                else
                {
                    return await UpdateSupplierQuotationHeader(supplierQuotationHeader);
                }
            }
            else
            {
                var supplierQuotationHeaderExist = await IsSupplierQuotationCodeExist(supplierQuotationHeader.SupplierQuotationHeaderId, supplierQuotationHeader.DocumentCode, supplierQuotationHeader.StoreId, separateYears, supplierQuotationHeader.DocumentDate, supplierQuotationHeader.Prefix, supplierQuotationHeader.Suffix);
                if (supplierQuotationHeaderExist.Success)
                {
                    var nextSupplierQuotationCode = await GetNextSupplierQuotationCode(supplierQuotationHeader.StoreId, separateYears, supplierQuotationHeader.DocumentDate, supplierQuotationHeader.Prefix, supplierQuotationHeader.Suffix);
                    return new ResponseDto() { Id = nextSupplierQuotationCode, ResponseType = ResponseTypeData.Confirm, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.SupplierQuotation, GenericMessageData.CodeAlreadyExist, nextSupplierQuotationCode.ToString()) };
                }
                else
                {
                    if (supplierQuotationHeader.SupplierQuotationHeaderId == 0)
                    {
                        return await CreateSupplierQuotationHeader(supplierQuotationHeader, hasApprove, approved, requestId, separateYears);
                    }
                    else
                    {
                        return await UpdateSupplierQuotationHeader(supplierQuotationHeader);
                    }
                }
            }
        }

        public async Task<ResponseDto> CreateSupplierQuotationHeader(SupplierQuotationHeaderDto supplierQuotationHeader, bool hasApprove, bool approved, int? requestId, bool separateYears)
        {
            int supplierQuotationCode;
            string? prefix;
            string? suffix;
            var nextSupplierQuotationCode = await GetSupplierQuotationCodeInternal(supplierQuotationHeader.StoreId, separateYears, supplierQuotationHeader.DocumentDate);
            if (hasApprove && approved)
            {
                supplierQuotationCode = nextSupplierQuotationCode.NextCode;
                prefix = nextSupplierQuotationCode.Prefix;
                suffix = nextSupplierQuotationCode.Suffix;
            }
            else
            {
                supplierQuotationCode = supplierQuotationHeader.DocumentCode != 0 ? supplierQuotationHeader.DocumentCode : nextSupplierQuotationCode.NextCode;
                prefix = string.IsNullOrWhiteSpace(supplierQuotationHeader.Prefix) ? nextSupplierQuotationCode.Prefix : supplierQuotationHeader.Prefix;
                suffix = string.IsNullOrWhiteSpace(supplierQuotationHeader.Suffix) ? nextSupplierQuotationCode.Suffix : supplierQuotationHeader.Suffix;
            }

            var supplierQuotationHeaderId = await GetNextId();
            var newSupplierQuotationHeader = new SupplierQuotationHeader()
            {
                SupplierQuotationHeaderId = supplierQuotationHeaderId,
                Prefix = prefix,
                DocumentCode = supplierQuotationCode,
                Suffix = suffix,
                ProductRequestPriceHeaderId = supplierQuotationHeader.ProductRequestPriceHeaderId,
                SupplierId = supplierQuotationHeader.SupplierId,
                DocumentReference = hasApprove ? $"{DocumentReferenceData.Approval}{requestId}" : $"{DocumentReferenceData.SupplierQuotation}{supplierQuotationHeaderId}",
                StoreId = supplierQuotationHeader.StoreId,
                TaxTypeId = supplierQuotationHeader.TaxTypeId,
                DocumentDate = supplierQuotationHeader.DocumentDate,
                EntryDate = DateHelper.GetDateTimeNow(),
                Reference = supplierQuotationHeader.Reference,
                TotalValue = supplierQuotationHeader.TotalValue,
                DiscountPercent = supplierQuotationHeader.DiscountPercent,
                DiscountValue = supplierQuotationHeader.DiscountValue,
                TotalItemDiscount = supplierQuotationHeader.TotalItemDiscount,
                GrossValue = supplierQuotationHeader.GrossValue,
                VatValue = supplierQuotationHeader.VatValue,
                SubNetValue = supplierQuotationHeader.SubNetValue,
                OtherTaxValue = supplierQuotationHeader.OtherTaxValue,
                AdditionalDiscountValue = supplierQuotationHeader.AdditionalDiscountValue,
                NetValueBeforeAdditionalDiscount = supplierQuotationHeader.NetValueBeforeAdditionalDiscount,
                NetValue = supplierQuotationHeader.NetValue,
                TotalCostValue = supplierQuotationHeader.TotalCostValue,
                RemarksAr = supplierQuotationHeader.RemarksAr,
                RemarksEn = supplierQuotationHeader.RemarksEn,
                IsClosed = false,
                ArchiveHeaderId = supplierQuotationHeader.ArchiveHeaderId,


                CreatedAt = DateHelper.GetDateTimeNow(),
                UserNameCreated = await _httpContextAccessor!.GetUserName(),
                IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
                Hide = false,
            };

            var supplierQuotationHeaderValidator = await new SupplierQuotationHeaderValidator(_localizer).ValidateAsync(newSupplierQuotationHeader);
            var validationResult = supplierQuotationHeaderValidator.IsValid;
            if (validationResult)
            {
                await _repository.Insert(newSupplierQuotationHeader);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = newSupplierQuotationHeader.SupplierQuotationHeaderId, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.SupplierQuotation, GenericMessageData.CreatedSuccessWithCode, $"{newSupplierQuotationHeader.Prefix}{newSupplierQuotationHeader.DocumentCode}{newSupplierQuotationHeader.Suffix}") };
            }
            else
            {
                return new ResponseDto() { Id = newSupplierQuotationHeader.SupplierQuotationHeaderId, Success = false, Message = supplierQuotationHeaderValidator.ToString("~") };
            }
        }

        public async Task<ResponseDto> UpdateSupplierQuotationHeader(SupplierQuotationHeaderDto supplierQuotationHeader)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var supplierQuotationHeaderDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.SupplierQuotationHeaderId == supplierQuotationHeader.SupplierQuotationHeaderId);
            if (supplierQuotationHeaderDb != null)
            {
                if (supplierQuotationHeaderDb.IsClosed)
                {
                    return new ResponseDto() { Id = supplierQuotationHeader.SupplierQuotationHeaderId, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.SupplierQuotation, GenericMessageData.CannotUpdateBecauseClosed) };
                }

                supplierQuotationHeaderDb.ProductRequestPriceHeaderId = supplierQuotationHeader.ProductRequestPriceHeaderId;
                supplierQuotationHeaderDb.SupplierId = supplierQuotationHeader.SupplierId;
                supplierQuotationHeaderDb.StoreId = supplierQuotationHeader.StoreId;
                supplierQuotationHeaderDb.TaxTypeId = supplierQuotationHeader.TaxTypeId;
                supplierQuotationHeaderDb.DocumentDate = supplierQuotationHeader.DocumentDate;
                supplierQuotationHeaderDb.Reference = supplierQuotationHeader.Reference;
                supplierQuotationHeaderDb.TotalValue = supplierQuotationHeader.TotalValue;
                supplierQuotationHeaderDb.DiscountPercent = supplierQuotationHeader.DiscountPercent;
                supplierQuotationHeaderDb.DiscountValue = supplierQuotationHeader.DiscountValue;
                supplierQuotationHeaderDb.TotalItemDiscount = supplierQuotationHeader.TotalItemDiscount;
                supplierQuotationHeaderDb.GrossValue = supplierQuotationHeader.GrossValue;
                supplierQuotationHeaderDb.VatValue = supplierQuotationHeader.VatValue;
                supplierQuotationHeaderDb.SubNetValue = supplierQuotationHeader.SubNetValue;
                supplierQuotationHeaderDb.OtherTaxValue = supplierQuotationHeader.OtherTaxValue;
                supplierQuotationHeaderDb.NetValueBeforeAdditionalDiscount = supplierQuotationHeader.NetValueBeforeAdditionalDiscount;
                supplierQuotationHeaderDb.AdditionalDiscountValue = supplierQuotationHeader.AdditionalDiscountValue;
                supplierQuotationHeaderDb.NetValue = supplierQuotationHeader.NetValue;
                supplierQuotationHeaderDb.TotalCostValue = supplierQuotationHeader.TotalCostValue;
                supplierQuotationHeaderDb.RemarksAr = supplierQuotationHeader.RemarksAr;
                supplierQuotationHeaderDb.RemarksEn = supplierQuotationHeader.RemarksEn;
                supplierQuotationHeaderDb.ArchiveHeaderId = supplierQuotationHeader.ArchiveHeaderId;
                
                supplierQuotationHeaderDb.ModifiedAt = DateHelper.GetDateTimeNow();
                supplierQuotationHeaderDb.UserNameModified = await _httpContextAccessor!.GetUserName();
                supplierQuotationHeaderDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();


                var supplierQuotationHeaderValidator = await new SupplierQuotationHeaderValidator(_localizer).ValidateAsync(supplierQuotationHeaderDb);
                var validationResult = supplierQuotationHeaderValidator.IsValid;
                if (validationResult)
                {
                    _repository.Update(supplierQuotationHeaderDb);
                    await _repository.SaveChanges();
                    return new ResponseDto() { Id = supplierQuotationHeaderDb.SupplierQuotationHeaderId, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.SupplierQuotation, GenericMessageData.UpdatedSuccessWithCode, $"{supplierQuotationHeaderDb.Prefix}{supplierQuotationHeaderDb.DocumentCode}{supplierQuotationHeaderDb.Suffix}") };
                }
                else
                {
                    return new ResponseDto() { Id = 0, Success = false, Message = supplierQuotationHeaderValidator.ToString("~") };
                }
            }
            return new ResponseDto() { Id = 0, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.SupplierQuotation, GenericMessageData.NotFound) };
        }

        public async Task<ResponseDto> IsSupplierQuotationCodeExist(int supplierQuotationHeaderId, int supplierQuotationCode, int storeId, bool separateYears, DateTime documentDate, string? prefix, string? suffix)
		{
			var supplierQuotationHeader = await _repository.GetAll().FirstOrDefaultAsync(x => x.SupplierQuotationHeaderId != supplierQuotationHeaderId && (x.StoreId == storeId && (!separateYears || x.DocumentDate.Year == documentDate.Year) && x.Prefix == prefix && x.DocumentCode == supplierQuotationCode && x.Suffix == suffix));
			if (supplierQuotationHeader is not null)
			{
                return new ResponseDto() { Id = supplierQuotationHeader.SupplierQuotationHeaderId, Success = true };
            }
			return new ResponseDto() {  Id = 0, Success = false };
		}

        public async Task<int> GetNextId()
        {
            int id = 1;
            try { id = await _repository.GetAll().MaxAsync(a => a.SupplierQuotationHeaderId) + 1; } catch { id = 1; }
            return id;
        }

        public async Task<ResponseDto> DeleteSupplierQuotationHeader(int id)
		{
            var supplierQuotationHeader = await _repository.GetAll().FirstOrDefaultAsync(x => x.SupplierQuotationHeaderId == id);
            if (supplierQuotationHeader != null)
            {
                _repository.Delete(supplierQuotationHeader);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = id, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.SupplierQuotation, GenericMessageData.DeleteSuccessWithCode, $"{supplierQuotationHeader.Prefix}{supplierQuotationHeader.DocumentCode}{supplierQuotationHeader.Suffix}") };
            }
            return new ResponseDto() { Id = id, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.SupplierQuotation, GenericMessageData.NotFound) };
        }

        public async Task<int> GetNextSupplierQuotationCode(int storeId, bool separateYears, DateTime documentDate, string? prefix, string? suffix)
        {
            int id = 1;
            try
            {
                id = await _repository.GetAll().AsNoTracking().Where(x => (!separateYears || x.DocumentDate.Year == documentDate.Year) && x.Prefix == prefix && x.Suffix == suffix && x.StoreId == storeId).MaxAsync(a => a.DocumentCode) + 1;
            }
            catch { id = 1; }
            return id;
        }
    }
}
