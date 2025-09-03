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
using Shared.Helper.Logic;
using Shared.CoreOne.Contracts.Settings;

namespace Purchases.Service.Services
{
	public class ProductRequestPriceHeaderService : BaseService<ProductRequestPriceHeader>, IProductRequestPriceHeaderService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IStoreService _storeService;
        private readonly ISupplierService _supplierService;
		private readonly IStringLocalizer<ProductRequestPriceHeaderService> _localizer;
        private readonly IGenericMessageService _genericMessageService;
		private readonly IMenuEncodingService _menuEncodingService;
        private readonly IApplicationSettingService _applicationSettingService;

		public ProductRequestPriceHeaderService(IRepository<ProductRequestPriceHeader> repository, IHttpContextAccessor httpContextAccessor, IStoreService storeService, IStringLocalizer<ProductRequestPriceHeaderService> localizer, IGenericMessageService genericMessageService, IMenuEncodingService menuEncodingService, ISupplierService supplierService, IApplicationSettingService applicationSettingService) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
			_storeService = storeService;
            _supplierService = supplierService;
			_localizer = localizer;
            _genericMessageService = genericMessageService;
			_menuEncodingService = menuEncodingService;
            _applicationSettingService = applicationSettingService;
		}

		public IQueryable<ProductRequestPriceHeaderDto> GetProductRequestPriceHeaders()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var data = from productRequestPriceHeader in _repository.GetAll()
				from store in _storeService.GetAll().Where(x => x.StoreId == productRequestPriceHeader.StoreId)
				from supplier in _supplierService.GetAll().Where(x => x.SupplierId == productRequestPriceHeader.SupplierId).DefaultIfEmpty()
                select new ProductRequestPriceHeaderDto()
				{
					ProductRequestPriceHeaderId = productRequestPriceHeader.ProductRequestPriceHeaderId,
					Prefix = productRequestPriceHeader.Prefix,
					DocumentCode = productRequestPriceHeader.DocumentCode,
					Suffix = productRequestPriceHeader.Suffix,
					DocumentFullCode = $"{productRequestPriceHeader.Prefix}{productRequestPriceHeader.DocumentCode}{productRequestPriceHeader.Suffix}",
					ProductRequestHeaderId = productRequestPriceHeader.ProductRequestHeaderId,
                    SupplierId = productRequestPriceHeader.SupplierId,
                    SupplierCode = supplier.SupplierCode,
                    SupplierName = supplier != null? language == LanguageCode.Arabic ? supplier.SupplierNameAr : supplier.SupplierNameEn : null,
                    DocumentReference = productRequestPriceHeader.DocumentReference,
                    StoreId = productRequestPriceHeader.StoreId,
					StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
                    TaxTypeId = productRequestPriceHeader.TaxTypeId,
					DocumentDate = productRequestPriceHeader.DocumentDate,
					EntryDate = productRequestPriceHeader.EntryDate,
                    TotalValue = productRequestPriceHeader.TotalValue,
                    DiscountPercent = productRequestPriceHeader.DiscountPercent,
                    DiscountValue = productRequestPriceHeader.DiscountValue,
                    TotalItemDiscount = productRequestPriceHeader.TotalItemDiscount,
                    GrossValue = productRequestPriceHeader.GrossValue,
                    VatValue = productRequestPriceHeader.VatValue,
                    SubNetValue = productRequestPriceHeader.SubNetValue,
                    OtherTaxValue = productRequestPriceHeader.OtherTaxValue,
                    NetValueBeforeAdditionalDiscount = productRequestPriceHeader.NetValueBeforeAdditionalDiscount,
                    AdditionalDiscountValue = productRequestPriceHeader.AdditionalDiscountValue,
                    NetValue = productRequestPriceHeader.NetValue,
                    TotalCostValue = productRequestPriceHeader.TotalCostValue,
                    Reference = productRequestPriceHeader.Reference,
					RemarksAr = productRequestPriceHeader.RemarksAr,
					RemarksEn = productRequestPriceHeader.RemarksEn,
					IsClosed = productRequestPriceHeader.IsClosed,
					ArchiveHeaderId = productRequestPriceHeader.ArchiveHeaderId,

                    CreatedAt = productRequestPriceHeader.CreatedAt,
                    UserNameCreated = productRequestPriceHeader.UserNameCreated,
                    IpAddressCreated = productRequestPriceHeader.IpAddressCreated,

                    ModifiedAt = productRequestPriceHeader.ModifiedAt,
                    UserNameModified = productRequestPriceHeader.UserNameModified,
                    IpAddressModified = productRequestPriceHeader.IpAddressModified
				};
			return data;
		}

        public IQueryable<ProductRequestPriceHeaderDto> GetUserProductRequestPriceHeaders()
        {
            var userStore = _httpContextAccessor.GetCurrentUserStore();
            return GetProductRequestPriceHeaders().Where(x => x.StoreId == userStore);
        }

		public IQueryable<ProductRequestPriceHeaderDto> GetProductRequestPriceHeadersByStoreId(int storeId, int? supplierId, int productRequestPriceHeaderId)
		{
            supplierId ??= 0;
            if (productRequestPriceHeaderId == 0)
            {
                return GetProductRequestPriceHeaders().Where(x => x.StoreId == storeId && (supplierId == 0 || x.SupplierId == supplierId) && x.IsClosed == false && x.TaxTypeId != 0);
            }
            else
            {
				return GetProductRequestPriceHeaders().Where(x => x.ProductRequestPriceHeaderId == productRequestPriceHeaderId);
			}
		}

        public async Task<ProductRequestPriceHeaderDto?> GetProductRequestPriceHeaderById(int id)
		{
			return await GetProductRequestPriceHeaders().FirstOrDefaultAsync(x => x.ProductRequestPriceHeaderId == id);
		}


        public async Task<DocumentCodeDto> GetProductRequestPriceCode(int storeId, DateTime documentDate)
        {
            var separateYears = await _applicationSettingService.SeparateYears(storeId);
            return await GetProductRequestPriceCodeInternal(storeId, separateYears, documentDate);
        }

		public async Task<DocumentCodeDto> GetProductRequestPriceCodeInternal(int storeId, bool separateYears, DateTime documentDate)
		{
			var encoding = await _menuEncodingService.GetMenuEncoding(storeId, MenuCodeData.ProductRequestPrice);
			var code = await GetNextProductRequestPriceCode(storeId, separateYears, documentDate, encoding.Prefix, encoding.Suffix);
			return new DocumentCodeDto() { NextCode = code, Prefix = encoding.Prefix, Suffix = encoding.Suffix };
		}

		public async Task<ResponseDto> UpdateClosed(int? productRequestPriceHeaderId, bool isClosed)
        {
            var header = await _repository.FindBy(x => x.ProductRequestPriceHeaderId == productRequestPriceHeaderId).FirstOrDefaultAsync();
            if (header is null) return new ResponseDto { Id = (int)productRequestPriceHeaderId!, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.ProductRequestPrice, GenericMessageData.NotFound) };

            header.IsClosed = isClosed;
            _repository.Update(header);
            await _repository.SaveChanges();
            return new ResponseDto { Id = (int)productRequestPriceHeaderId!, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.ProductRequestPrice, isClosed ? GenericMessageData.ClosedSuccessfully : GenericMessageData.OpenedSuccessfully) };
        }

        public async Task<ResponseDto> SaveProductRequestPriceHeader(ProductRequestPriceHeaderDto productRequestPriceHeader, bool hasApprove, bool approved, int? requestId)
		{
            var separateYears = await _applicationSettingService.SeparateYears(productRequestPriceHeader.StoreId);

			if (hasApprove)
            {
                if (productRequestPriceHeader.ProductRequestPriceHeaderId == 0)
                {
                    return await CreateProductRequestPriceHeader(productRequestPriceHeader, hasApprove, approved, requestId, separateYears);
                }
                else
                {
                    return await UpdateProductRequestPriceHeader(productRequestPriceHeader);
                }
            }
            else
            {
                var productRequestPriceHeaderExist = await IsProductRequestPriceCodeExist(productRequestPriceHeader.ProductRequestPriceHeaderId, productRequestPriceHeader.DocumentCode, productRequestPriceHeader.StoreId, separateYears, productRequestPriceHeader.DocumentDate, productRequestPriceHeader.Prefix, productRequestPriceHeader.Suffix);
                if (productRequestPriceHeaderExist.Success)
                {
                    var nextProductRequestPriceCode = await GetNextProductRequestPriceCode(productRequestPriceHeader.StoreId, separateYears, productRequestPriceHeader.DocumentDate, productRequestPriceHeader.Prefix, productRequestPriceHeader.Suffix);
                    return new ResponseDto() { Id = nextProductRequestPriceCode, ResponseType = ResponseTypeData.Confirm, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.ProductRequestPrice, GenericMessageData.CodeAlreadyExist, nextProductRequestPriceCode.ToString()) };
                }
                else
                {
                    if (productRequestPriceHeader.ProductRequestPriceHeaderId == 0)
                    {
                        return await CreateProductRequestPriceHeader(productRequestPriceHeader, hasApprove, approved, requestId, separateYears);
                    }
                    else
                    {
                        return await UpdateProductRequestPriceHeader(productRequestPriceHeader);
                    }
                }
            }
        }

        public async Task<ResponseDto> CreateProductRequestPriceHeader(ProductRequestPriceHeaderDto productRequestPriceHeader, bool hasApprove, bool approved, int? requestId, bool separateYears)
        {
            int productRequestPriceCode;
            string? prefix;
            string? suffix;
            var nextProductRequestPriceCode = await GetProductRequestPriceCodeInternal(productRequestPriceHeader.StoreId, separateYears, productRequestPriceHeader.DocumentDate);
            if (hasApprove && approved)
            {
                productRequestPriceCode = nextProductRequestPriceCode.NextCode;
                prefix = nextProductRequestPriceCode.Prefix;
                suffix = nextProductRequestPriceCode.Suffix;
            }
            else
            {
                productRequestPriceCode = productRequestPriceHeader.DocumentCode != 0 ? productRequestPriceHeader.DocumentCode : nextProductRequestPriceCode.NextCode;
                prefix = string.IsNullOrWhiteSpace(productRequestPriceHeader.Prefix) ? nextProductRequestPriceCode.Prefix : productRequestPriceHeader.Prefix;
                suffix = string.IsNullOrWhiteSpace(productRequestPriceHeader.Suffix) ? nextProductRequestPriceCode.Suffix : productRequestPriceHeader.Suffix;
            }

            var productRequestPriceHeaderId = await GetNextId();
            var newProductRequestPriceHeader = new ProductRequestPriceHeader()
            {
                ProductRequestPriceHeaderId = productRequestPriceHeaderId,
                Prefix = prefix,
                DocumentCode = productRequestPriceCode,
                Suffix = suffix,
                ProductRequestHeaderId = productRequestPriceHeader.ProductRequestHeaderId,
                SupplierId = productRequestPriceHeader.SupplierId,
                DocumentReference = hasApprove ? $"{DocumentReferenceData.Approval}{requestId}" : $"{DocumentReferenceData.ProductRequestPrice}{productRequestPriceHeaderId}",
                StoreId = productRequestPriceHeader.StoreId,
                TaxTypeId = productRequestPriceHeader.TaxTypeId,
                DocumentDate = productRequestPriceHeader.DocumentDate,
                EntryDate = DateHelper.GetDateTimeNow(),
                TotalValue = productRequestPriceHeader.TotalValue,
				DiscountPercent = productRequestPriceHeader.DiscountPercent,
				DiscountValue = productRequestPriceHeader.DiscountValue,
				TotalItemDiscount = productRequestPriceHeader.TotalItemDiscount,
                GrossValue = productRequestPriceHeader.GrossValue,
                VatValue = productRequestPriceHeader.VatValue,
                SubNetValue = productRequestPriceHeader.SubNetValue,
                OtherTaxValue = productRequestPriceHeader.OtherTaxValue,
                NetValueBeforeAdditionalDiscount = productRequestPriceHeader.NetValueBeforeAdditionalDiscount,
                AdditionalDiscountValue = productRequestPriceHeader.AdditionalDiscountValue,
                NetValue = productRequestPriceHeader.NetValue,
                TotalCostValue = productRequestPriceHeader.TotalCostValue,
                Reference = productRequestPriceHeader.Reference,
                RemarksAr = productRequestPriceHeader.RemarksAr,
                RemarksEn = productRequestPriceHeader.RemarksEn,
                IsClosed = false,
                ArchiveHeaderId = productRequestPriceHeader.ArchiveHeaderId,
                Hide = false,
                CreatedAt = DateHelper.GetDateTimeNow(),
                UserNameCreated = await _httpContextAccessor!.GetUserName(),
                IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
			};

            var productRequestPriceHeaderValidator = await new ProductRequestPriceHeaderValidator(_localizer).ValidateAsync(newProductRequestPriceHeader);
            var validationResult = productRequestPriceHeaderValidator.IsValid;
            if (validationResult)
            {
                await _repository.Insert(newProductRequestPriceHeader);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = newProductRequestPriceHeader.ProductRequestPriceHeaderId, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.ProductRequestPrice, GenericMessageData.CreatedSuccessWithCode, $"{newProductRequestPriceHeader.Prefix}{newProductRequestPriceHeader.DocumentCode}{newProductRequestPriceHeader.Suffix}") };
            }
            else
            {
                return new ResponseDto() { Id = newProductRequestPriceHeader.ProductRequestPriceHeaderId, Success = false, Message = productRequestPriceHeaderValidator.ToString("~") };
            }
        }

        public async Task<ResponseDto> UpdateProductRequestPriceHeader(ProductRequestPriceHeaderDto productRequestPriceHeader)
        {
            var productRequestPriceHeaderDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.ProductRequestPriceHeaderId == productRequestPriceHeader.ProductRequestPriceHeaderId);
            if (productRequestPriceHeaderDb != null)
            {
                if (productRequestPriceHeaderDb.IsClosed)
                {
                    return new ResponseDto() { Id = productRequestPriceHeader.ProductRequestPriceHeaderId, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.ProductRequestPrice, GenericMessageData.CannotUpdateBecauseClosed) };
                }

                productRequestPriceHeaderDb.ProductRequestHeaderId = productRequestPriceHeader.ProductRequestHeaderId;
                productRequestPriceHeaderDb.SupplierId = productRequestPriceHeader.SupplierId;
                productRequestPriceHeaderDb.StoreId = productRequestPriceHeader.StoreId;
                productRequestPriceHeaderDb.TaxTypeId = productRequestPriceHeader.TaxTypeId;
                productRequestPriceHeaderDb.DocumentDate = productRequestPriceHeader.DocumentDate;
                productRequestPriceHeaderDb.TotalValue = productRequestPriceHeader.TotalValue;
                productRequestPriceHeaderDb.DiscountPercent = productRequestPriceHeader.DiscountPercent;
                productRequestPriceHeaderDb.DiscountValue = productRequestPriceHeader.DiscountValue;
                productRequestPriceHeaderDb.TotalItemDiscount = productRequestPriceHeader.TotalItemDiscount;
                productRequestPriceHeaderDb.GrossValue = productRequestPriceHeader.GrossValue;
                productRequestPriceHeaderDb.VatValue = productRequestPriceHeader.VatValue;
                productRequestPriceHeaderDb.SubNetValue = productRequestPriceHeader.SubNetValue;
                productRequestPriceHeaderDb.OtherTaxValue = productRequestPriceHeader.OtherTaxValue;
                productRequestPriceHeaderDb.NetValueBeforeAdditionalDiscount = productRequestPriceHeader.NetValueBeforeAdditionalDiscount;
                productRequestPriceHeaderDb.AdditionalDiscountValue = productRequestPriceHeader.AdditionalDiscountValue;
                productRequestPriceHeaderDb.NetValue = productRequestPriceHeader.NetValue;
                productRequestPriceHeaderDb.TotalCostValue = productRequestPriceHeader.TotalCostValue;
                productRequestPriceHeaderDb.Reference = productRequestPriceHeader.Reference;
                productRequestPriceHeaderDb.RemarksAr = productRequestPriceHeader.RemarksAr;
                productRequestPriceHeaderDb.RemarksEn = productRequestPriceHeader.RemarksEn;
                productRequestPriceHeaderDb.ArchiveHeaderId = productRequestPriceHeader.ArchiveHeaderId;
                
                productRequestPriceHeaderDb.ModifiedAt = DateHelper.GetDateTimeNow();
                productRequestPriceHeaderDb.UserNameModified = await _httpContextAccessor!.GetUserName();
                productRequestPriceHeaderDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();


                var productRequestPriceHeaderValidator = await new ProductRequestPriceHeaderValidator(_localizer).ValidateAsync(productRequestPriceHeaderDb);
                var validationResult = productRequestPriceHeaderValidator.IsValid;
                if (validationResult)
                {
                    _repository.Update(productRequestPriceHeaderDb);
                    await _repository.SaveChanges();
                    return new ResponseDto() { Id = productRequestPriceHeaderDb.ProductRequestPriceHeaderId, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.ProductRequestPrice, GenericMessageData.UpdatedSuccessWithCode, $"{productRequestPriceHeaderDb.Prefix}{productRequestPriceHeaderDb.DocumentCode}{productRequestPriceHeaderDb.Suffix}") };
                }
                else
                {
                    return new ResponseDto() { Id = 0, Success = false, Message = productRequestPriceHeaderValidator.ToString("~") };
                }
            }
            return new ResponseDto() { Id = 0, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.ProductRequestPrice, GenericMessageData.NotFound) };
        }

        public async Task<ResponseDto> IsProductRequestPriceCodeExist(int productRequestPriceHeaderId, int productRequestPriceCode, int storeId, bool separateYears, DateTime documentDate, string? prefix, string? suffix)
		{
			var productRequestPriceHeader = await _repository.GetAll().FirstOrDefaultAsync(x => x.ProductRequestPriceHeaderId != productRequestPriceHeaderId && (x.StoreId == storeId && (!separateYears || x.DocumentDate.Year == documentDate.Year) && x.Prefix == prefix && x.DocumentCode == productRequestPriceCode && x.Suffix == suffix));
			if (productRequestPriceHeader is not null)
			{
                return new ResponseDto() { Id = productRequestPriceHeader.ProductRequestPriceHeaderId, Success = true };
            }
			return new ResponseDto() {  Id = 0, Success = false };
		}

        public async Task<int> GetNextId()
        {
            int id = 1;
            try { id = await _repository.GetAll().MaxAsync(a => a.ProductRequestPriceHeaderId) + 1; } catch { id = 1; }
            return id;
        }

        public async Task<ResponseDto> DeleteProductRequestPriceHeader(int id)
		{
            var productRequestPriceHeader = await _repository.GetAll().FirstOrDefaultAsync(x => x.ProductRequestPriceHeaderId == id);
            if (productRequestPriceHeader != null)
            {
                _repository.Delete(productRequestPriceHeader);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = id, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.ProductRequestPrice, GenericMessageData.DeleteSuccessWithCode, $"{productRequestPriceHeader.Prefix}{productRequestPriceHeader.DocumentCode}{productRequestPriceHeader.Suffix}") };
            }
            return new ResponseDto() { Id = id, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.ProductRequestPrice, GenericMessageData.NotFound) };
        }

        public async Task<int> GetNextProductRequestPriceCode(int storeId, bool separateYears, DateTime documentDate, string? prefix, string? suffix)
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
