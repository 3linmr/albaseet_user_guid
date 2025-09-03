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
using Shared.Helper.Logic;
using Microsoft.AspNetCore.Http.HttpResults;
using Shared.CoreOne.Contracts.Settings;

namespace Purchases.Service.Services
{
	public class ProductRequestHeaderService : BaseService<ProductRequestHeader>, IProductRequestHeaderService
	{
        private readonly IStringLocalizer<ProductRequestHeaderService> _localization;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IStoreService _storeService;
		private readonly IGenericMessageService _genericMessageService;
		private readonly IMenuEncodingService _menuEncodingService;
        private readonly IApplicationSettingService _applicationSettingService;

		public ProductRequestHeaderService(IStringLocalizer<ProductRequestHeaderService> localization, IRepository<ProductRequestHeader> repository, IHttpContextAccessor httpContextAccessor, IStoreService storeService, IGenericMessageService genericMessageService, IMenuEncodingService menuEncodingService, IApplicationSettingService applicationSettingService) : base(repository)
		{
            _localization = localization;
			_httpContextAccessor = httpContextAccessor;
			_storeService = storeService;
			_genericMessageService = genericMessageService;
			_menuEncodingService = menuEncodingService;
            _applicationSettingService = applicationSettingService;
		}

		public IQueryable<ProductRequestHeaderDto> GetProductRequestHeaders()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var data = from productRequestHeader in _repository.GetAll()
				from store in _storeService.GetAll().Where(x => x.StoreId == productRequestHeader.StoreId)
				select new ProductRequestHeaderDto()
				{
					ProductRequestHeaderId = productRequestHeader.ProductRequestHeaderId,
					Prefix = productRequestHeader.Prefix,
					DocumentCode = productRequestHeader.DocumentCode,
					Suffix = productRequestHeader.Suffix,
					DocumentFullCode = $"{productRequestHeader.Prefix}{productRequestHeader.DocumentCode}{productRequestHeader.Suffix}",
					DocumentReference = productRequestHeader.DocumentReference,
                    StoreId = productRequestHeader.StoreId,
					StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
					DocumentDate = productRequestHeader.DocumentDate,
					EntryDate = productRequestHeader.EntryDate,
                    Reference = productRequestHeader.Reference,
					ConsumerValue = productRequestHeader.ConsumerValue,
					CostValue = productRequestHeader.CostValue,
					RemarksAr = productRequestHeader.RemarksAr,
					RemarksEn = productRequestHeader.RemarksEn,
					IsClosed = productRequestHeader.IsClosed,
					ArchiveHeaderId = productRequestHeader.ArchiveHeaderId,

                    CreatedAt = productRequestHeader.CreatedAt,
                    UserNameCreated = productRequestHeader.UserNameCreated,
                    IpAddressCreated = productRequestHeader.IpAddressCreated,

                    ModifiedAt = productRequestHeader.ModifiedAt,
                    UserNameModified = productRequestHeader.UserNameModified,
                    IpAddressModified = productRequestHeader.IpAddressModified,
				};
			return data;
		}

        public IQueryable<ProductRequestHeaderDto> GetUserProductRequestHeaders()
        {
            var userStore = _httpContextAccessor.GetCurrentUserStore();
            return GetProductRequestHeaders().Where(x => x.StoreId == userStore);
        }

        public IQueryable<ProductRequestHeaderDto> GetProductRequestHeadersByStoreId(int storeId, int productRequestHeaderId)
		{
            if (productRequestHeaderId == 0)
            {
                return GetProductRequestHeaders().Where(x => x.StoreId == storeId && x.IsClosed == false);
            }
            else
            {
				return GetProductRequestHeaders().Where(x => x.ProductRequestHeaderId == productRequestHeaderId);
			}
		}

		public async Task<ProductRequestHeaderDto?> GetProductRequestHeaderById(int id)
		{
			return await GetProductRequestHeaders().FirstOrDefaultAsync(x => x.ProductRequestHeaderId == id);
		}

		public async Task<DocumentCodeDto> GetProductRequestCode(int storeId, DateTime documentDate)
		{
            var separateYears = await _applicationSettingService.SeparateYears(storeId);
            return await GetProductRequestCodeInternal(storeId, separateYears, documentDate);
		}

		public async Task<DocumentCodeDto> GetProductRequestCodeInternal(int storeId, bool separateYears, DateTime documentDate)
        {
            var encoding = await _menuEncodingService.GetMenuEncoding(storeId, MenuCodeData.ProductRequest);
            var code = await GetNextProductRequestCode(storeId, separateYears, documentDate, encoding.Prefix, encoding.Suffix);
            return new DocumentCodeDto() { NextCode = code, Prefix = encoding.Prefix, Suffix = encoding.Suffix };
        }

        public async Task<ResponseDto> UpdateClosed(int? productRequestHeaderId, bool isClosed)
        {
            var header = await _repository.FindBy(x => x.ProductRequestHeaderId == productRequestHeaderId).FirstOrDefaultAsync();
            if (header is null) return new ResponseDto { Id = (int)productRequestHeaderId!, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.ProductRequest, GenericMessageData.NotFound) };

            header.IsClosed = isClosed;
            _repository.Update(header);
            await _repository.SaveChanges();

            return new ResponseDto { Id = (int)productRequestHeaderId!, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.ProductRequest, isClosed ? GenericMessageData.ClosedSuccessfully : GenericMessageData.OpenedSuccessfully) };
        }

        public async Task<ResponseDto> SaveProductRequestHeader(ProductRequestHeaderDto productRequestHeader, bool hasApprove, bool approved, int? requestId)
		{
            var separateYears = await _applicationSettingService.SeparateYears(productRequestHeader.StoreId);

            if (hasApprove)
            {
                if (productRequestHeader.ProductRequestHeaderId == 0)
                {
                    return await CreateProductRequestHeader(productRequestHeader, hasApprove, approved, requestId, separateYears);
                }
                else
                {
                    return await UpdateProductRequestHeader(productRequestHeader);
                }
            }
            else
            {
                var productRequestHeaderExist = await IsProductRequestCodeExist(productRequestHeader.ProductRequestHeaderId, productRequestHeader.DocumentCode, productRequestHeader.StoreId, separateYears, productRequestHeader.DocumentDate, productRequestHeader.Prefix, productRequestHeader.Suffix);
                if (productRequestHeaderExist.Success)
                {
                    var nextProductRequestCode = await GetNextProductRequestCode(productRequestHeader.StoreId, separateYears, productRequestHeader.DocumentDate, productRequestHeader.Prefix, productRequestHeader.Suffix);
                    return new ResponseDto() { Id = nextProductRequestCode, ResponseType = ResponseTypeData.Confirm, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.ProductRequest, GenericMessageData.CodeAlreadyExist, $"{nextProductRequestCode}") };
                }
                else
                {
                    if (productRequestHeader.ProductRequestHeaderId == 0)
                    {
                        return await CreateProductRequestHeader(productRequestHeader, hasApprove, approved, requestId, separateYears);
                    }
                    else
                    {
                        return await UpdateProductRequestHeader(productRequestHeader);
                    }
                }
            }
        }

        public async Task<ResponseDto> CreateProductRequestHeader(ProductRequestHeaderDto productRequestHeader, bool hasApprove, bool approved, int? requestId, bool separateYears)
        {
            int productRequestCode;
            string? prefix;
            string? suffix;
            var nextProductRequestCode = await GetProductRequestCodeInternal(productRequestHeader.StoreId, separateYears, productRequestHeader.DocumentDate);
            if (hasApprove && approved)
            {
                productRequestCode = nextProductRequestCode.NextCode;
                prefix = nextProductRequestCode.Prefix;
                suffix = nextProductRequestCode.Suffix;
            }
            else
            {
                productRequestCode = productRequestHeader.DocumentCode != 0 ? productRequestHeader.DocumentCode : nextProductRequestCode.NextCode;
                prefix = string.IsNullOrWhiteSpace(productRequestHeader.Prefix) ? nextProductRequestCode.Prefix : productRequestHeader.Prefix;
                suffix = string.IsNullOrWhiteSpace(productRequestHeader.Suffix) ? nextProductRequestCode.Suffix : productRequestHeader.Suffix;
            }

            var productRequestHeaderId = await GetNextId();
            var newProductRequestHeader = new ProductRequestHeader()
            {
                ProductRequestHeaderId = productRequestHeaderId,
                Prefix = prefix,
                DocumentCode = productRequestCode,
                Suffix = suffix,
                DocumentReference = hasApprove ? $"{DocumentReferenceData.Approval}{requestId}" : $"{DocumentReferenceData.ProductRequest}{productRequestHeaderId}",
                StoreId = productRequestHeader.StoreId,
                DocumentDate = productRequestHeader.DocumentDate,
                EntryDate = DateHelper.GetDateTimeNow(),
                Reference = productRequestHeader.Reference,
                ConsumerValue = productRequestHeader.ConsumerValue,
                CostValue = productRequestHeader.CostValue,
                RemarksAr = productRequestHeader.RemarksAr,
                RemarksEn = productRequestHeader.RemarksEn,
                IsClosed = false,
                ArchiveHeaderId = productRequestHeader.ArchiveHeaderId,
                
                CreatedAt = DateHelper.GetDateTimeNow(),
                UserNameCreated = await _httpContextAccessor!.GetUserName(),
                IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
                Hide = false,
            };

            var productRequestHeaderValidator = await new ProductRequestHeaderValidator(_localization).ValidateAsync(newProductRequestHeader);
            var validationResult = productRequestHeaderValidator.IsValid;
            if (validationResult)
            {
                await _repository.Insert(newProductRequestHeader);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = newProductRequestHeader.ProductRequestHeaderId, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.ProductRequest, GenericMessageData.CreatedSuccessWithCode, $"{newProductRequestHeader.Prefix}{newProductRequestHeader.DocumentCode}{newProductRequestHeader.Suffix}") };
            }
            else
            {
                return new ResponseDto() { Id = newProductRequestHeader.ProductRequestHeaderId, Success = false, Message = productRequestHeaderValidator.ToString("~") };
            }
        }

        public async Task<ResponseDto> UpdateProductRequestHeader(ProductRequestHeaderDto productRequestHeader)
        {
            var productRequestHeaderDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.ProductRequestHeaderId == productRequestHeader.ProductRequestHeaderId);
            if (productRequestHeaderDb != null)
            {
                if (productRequestHeaderDb.IsClosed)
                {
                    return new ResponseDto() { Id = productRequestHeader.ProductRequestHeaderId, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.ProductRequest, GenericMessageData.CannotUpdateBecauseClosed) };
                }

                productRequestHeaderDb.StoreId = productRequestHeader.StoreId;
                productRequestHeaderDb.DocumentDate = productRequestHeader.DocumentDate;
                productRequestHeaderDb.Reference = productRequestHeader.Reference;
                productRequestHeaderDb.ConsumerValue = productRequestHeader.ConsumerValue;
                productRequestHeaderDb.CostValue = productRequestHeader.CostValue;
                productRequestHeaderDb.RemarksAr = productRequestHeader.RemarksAr;
                productRequestHeaderDb.RemarksEn = productRequestHeader.RemarksEn;
                productRequestHeaderDb.ArchiveHeaderId = productRequestHeader.ArchiveHeaderId;
                
                productRequestHeaderDb.ModifiedAt = DateHelper.GetDateTimeNow();
                productRequestHeaderDb.UserNameModified = await _httpContextAccessor!.GetUserName();
                productRequestHeaderDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();


                var productRequestHeaderValidator = await new ProductRequestHeaderValidator(_localization).ValidateAsync(productRequestHeaderDb);
                var validationResult = productRequestHeaderValidator.IsValid;
                if (validationResult)
                {
                    _repository.Update(productRequestHeaderDb);
                    await _repository.SaveChanges();
                    return new ResponseDto() { Id = productRequestHeaderDb.ProductRequestHeaderId, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.ProductRequest, GenericMessageData.UpdatedSuccessWithCode, $"{productRequestHeaderDb.Prefix}{productRequestHeaderDb.DocumentCode}{productRequestHeaderDb.Suffix}") };
                }
                else
                {
                    return new ResponseDto() { Id = 0, Success = false, Message = productRequestHeaderValidator.ToString("~") };
                }
            }
            return new ResponseDto() { Id = 0, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.ProductRequest, GenericMessageData.NotFound) };
        }

        public async Task<ResponseDto> IsProductRequestCodeExist(int productRequestHeaderId, int productRequestCode, int storeId, bool separateYears, DateTime documentDate, string? prefix, string? suffix)
		{
			var productRequestHeader = await _repository.GetAll().FirstOrDefaultAsync(x => x.ProductRequestHeaderId != productRequestHeaderId && (x.StoreId == storeId && (!separateYears || x.DocumentDate.Year == documentDate.Year) && x.Prefix == prefix && x.DocumentCode == productRequestCode && x.Suffix == suffix));
			if (productRequestHeader is not null)
			{
                return new ResponseDto() { Id = productRequestHeader.ProductRequestHeaderId, Success = true };
            }
			return new ResponseDto() {  Id = 0, Success = false };
		}

        public async Task<int> GetNextId()
        {
            int id = 1;
            try { id = await _repository.GetAll().MaxAsync(a => a.ProductRequestHeaderId) + 1; } catch { id = 1; }
            return id;
        }

        public async Task<ResponseDto> DeleteProductRequestHeader(int id)
		{
            var productRequestHeader = await _repository.GetAll().FirstOrDefaultAsync(x => x.ProductRequestHeaderId == id);
            if (productRequestHeader != null)
            {
                _repository.Delete(productRequestHeader);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = id, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.ProductRequest, GenericMessageData.DeleteSuccessWithCode, $"{productRequestHeader.Prefix}{productRequestHeader.DocumentCode}{productRequestHeader.Suffix}") };
            }
            return new ResponseDto() { Id = id, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.ProductRequest, GenericMessageData.NotFound) };
        }

        public async Task<int> GetNextProductRequestCode(int storeId, bool separateYears, DateTime documentDate, string? prefix, string? suffix)
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
