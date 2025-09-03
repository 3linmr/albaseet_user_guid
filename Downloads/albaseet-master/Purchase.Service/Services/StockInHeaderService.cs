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
using Shared.CoreOne.Models.Domain.Menus;
using Shared.CoreOne.Contracts.Settings;

namespace Purchases.Service.Services
{
	public class StockInHeaderService : BaseService<StockInHeader>, IStockInHeaderService
	{
        private readonly IGenericMessageService _genericMessageService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IStoreService _storeService;
        private readonly ISupplierService _supplierService;
		private readonly IStringLocalizer<StockInHeaderService> _localizer;
		private readonly IMenuEncodingService _menuEncodingService;
        private readonly IPurchaseOrderHeaderService _purchaseOrderHeaderService;
        private readonly IPurchaseInvoiceHeaderService _purchaseInvoiceHeaderService;
        private readonly IMenuService _menuService;
        private readonly IApplicationSettingService _applicationSettingService;

		public StockInHeaderService(IGenericMessageService genericMessageService, IRepository<StockInHeader> repository, IHttpContextAccessor httpContextAccessor, IStoreService storeService, IStringLocalizer<StockInHeaderService> localizer, IMenuEncodingService menuEncodingService, ISupplierService supplierService, IPurchaseOrderHeaderService purchaseOrderHeaderService, IPurchaseInvoiceHeaderService purchaseInvoiceHeaderService, IMenuService menuService, IApplicationSettingService applicationSettingService) : base(repository)
		{
            _genericMessageService = genericMessageService;
			_httpContextAccessor = httpContextAccessor;
			_storeService = storeService;
            _supplierService = supplierService;
			_localizer = localizer;
			_menuEncodingService = menuEncodingService;
            _purchaseOrderHeaderService = purchaseOrderHeaderService;
            _purchaseInvoiceHeaderService = purchaseInvoiceHeaderService;
            _menuService = menuService;
            _applicationSettingService = applicationSettingService;
		}

		public IQueryable<StockInHeaderDto> GetStockInHeaders()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var data = from stockInHeader in _repository.GetAll()
				from store in _storeService.GetAll().Where(x => x.StoreId == stockInHeader.StoreId)
				from supplier in _supplierService.GetAll().Where(x => x.SupplierId == stockInHeader.SupplierId).DefaultIfEmpty()
                from purchaseOrderHeader in _purchaseOrderHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == stockInHeader.PurchaseOrderHeaderId).DefaultIfEmpty()
                from purchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == stockInHeader.PurchaseInvoiceHeaderId).DefaultIfEmpty()
                from menu in _menuService.GetAll().Where(x => x.MenuCode == stockInHeader.MenuCode).DefaultIfEmpty()
                select new StockInHeaderDto()
				{
					StockInHeaderId = stockInHeader.StockInHeaderId,
					Prefix = stockInHeader.Prefix,
					DocumentCode = stockInHeader.DocumentCode,
					Suffix = stockInHeader.Suffix,
					DocumentFullCode = $"{stockInHeader.Prefix}{stockInHeader.DocumentCode}{stockInHeader.Suffix}",
                    DocumentReference = stockInHeader.DocumentReference,
					StockTypeId = stockInHeader.StockTypeId,
                    PurchaseOrderHeaderId = stockInHeader.PurchaseOrderHeaderId,
                    PurchaseOrderFullCode = purchaseOrderHeader != null ? $"{purchaseOrderHeader.Prefix}{purchaseOrderHeader.DocumentCode}{purchaseOrderHeader.Suffix}" : null,
                    PurchaseOrderDocumentReference = purchaseOrderHeader != null ? purchaseOrderHeader.DocumentReference : null,
                    PurchaseInvoiceHeaderId = stockInHeader.PurchaseInvoiceHeaderId,
                    PurchaseInvoiceFullCode = purchaseInvoiceHeader != null ? $"{purchaseInvoiceHeader.Prefix}{purchaseInvoiceHeader.DocumentCode}{purchaseInvoiceHeader.Suffix}" : null,
                    PurchaseInvoiceDocumentReference = purchaseInvoiceHeader != null ? purchaseInvoiceHeader.DocumentReference : null,
                    SupplierId = stockInHeader.SupplierId,
                    SupplierCode = supplier.SupplierCode,
                    SupplierName = supplier != null? language == LanguageCode.Arabic ? supplier.SupplierNameAr : supplier.SupplierNameEn : null,
                    StoreId = stockInHeader.StoreId,
					StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
					DocumentDate = stockInHeader.DocumentDate,
					EntryDate = stockInHeader.EntryDate,
                    Reference = stockInHeader.Reference,
                    TotalValue = stockInHeader.TotalValue,
					DiscountPercent = stockInHeader.DiscountPercent,
					DiscountValue = stockInHeader.DiscountValue,
					TotalItemDiscount = stockInHeader.TotalItemDiscount,
					GrossValue = stockInHeader.GrossValue,
					VatValue = stockInHeader.VatValue,
					SubNetValue = stockInHeader.SubNetValue,
					OtherTaxValue = stockInHeader.OtherTaxValue,
                    NetValueBeforeAdditionalDiscount = stockInHeader.NetValueBeforeAdditionalDiscount,
                    AdditionalDiscountValue = stockInHeader.AdditionalDiscountValue,
					NetValue = stockInHeader.NetValue,
                    TotalCostValue = stockInHeader.TotalCostValue,
					RemarksAr = stockInHeader.RemarksAr,
					RemarksEn = stockInHeader.RemarksEn,
					IsClosed = (purchaseOrderHeader != null && purchaseOrderHeader.IsEnded) || (purchaseInvoiceHeader != null && purchaseInvoiceHeader.IsEnded) || stockInHeader.IsClosed || stockInHeader.IsBlocked, //if parent is ended, isClosed is always true
                    IsEnded = (purchaseOrderHeader != null && purchaseOrderHeader.IsEnded) || (purchaseInvoiceHeader != null && purchaseInvoiceHeader.IsEnded), //is ended of stock in is always the same as the is ended of its parent
					IsBlocked = stockInHeader.IsBlocked,
                    ArchiveHeaderId = stockInHeader.ArchiveHeaderId,
                    MenuCode = stockInHeader.MenuCode,
                    MenuName = menu != null ? language == LanguageCode.Arabic ? menu.MenuNameAr : menu.MenuNameEn : null,

                    CreatedAt = stockInHeader.CreatedAt,
                    UserNameCreated = stockInHeader.UserNameCreated,
                    IpAddressCreated = stockInHeader.IpAddressCreated,

                    ModifiedAt = stockInHeader.ModifiedAt,
                    UserNameModified = stockInHeader.UserNameModified,
                    IpAddressModified = stockInHeader.IpAddressModified
                };
			return data;
		}

        public IQueryable<StockInHeaderDto> GetUserStockInHeaders(int stockTypeId)
        {
            var userStore = _httpContextAccessor.GetCurrentUserStore();
            return GetStockInHeaders().Where(x => x.StoreId == userStore && x.StockTypeId == stockTypeId);
        }

		public async Task<StockInHeaderDto?> GetStockInHeaderById(int id)
		{
			return await GetStockInHeaders().FirstOrDefaultAsync(x => x.StockInHeaderId == id);
		}

        public async Task<DocumentCodeDto> GetStockInCode(int storeId, DateTime documentDate, int stockTypeId)
        {
			var separateYears = await _applicationSettingService.SeparateYears(storeId);
            return await GetStockInCodeInternal(storeId, separateYears, documentDate, stockTypeId);
        }        
        
        public async Task<DocumentCodeDto> GetStockInCodeInternal(int storeId, bool separateYears, DateTime documentDate, int stockTypeId)
        {
            var encoding = await _menuEncodingService.GetMenuEncoding(storeId, StockTypeData.ToMenuCode((byte)stockTypeId));
            var code = await GetNextStockInCode(storeId, separateYears, documentDate, stockTypeId, encoding.Prefix, encoding.Suffix);
            return new DocumentCodeDto() { NextCode = code, Prefix = encoding.Prefix, Suffix = encoding.Suffix };
        }

        public async Task<bool> UpdateClosed(int? stockInHeaderId, bool isClosed)
        {
            var header = await _repository.FindBy(x => x.StockInHeaderId == stockInHeaderId).FirstOrDefaultAsync();
            if (header is null) return false;

            header.IsClosed = isClosed;
            _repository.Update(header);
            await _repository.SaveChanges();
            return true;
        }

        public async Task<bool> UpdateAllStockInsBlockedFromPurchaseOrder(int purchaseOrderHeaderId, bool isBlocked)
        {
            var headerList = new List<StockInHeader>();

            var stockInsDirectlyRelatedToPurchaseOrder = await _repository.FindBy(x => x.PurchaseOrderHeaderId == purchaseOrderHeaderId).ToListAsync();
            headerList.AddRange(stockInsDirectlyRelatedToPurchaseOrder);

            var stocksInsRelatedToPurchaseOrderThroughPurchaseInvoice = await (
                    from stockInHeader in _repository.GetAll()
                    from purchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == stockInHeader.PurchaseInvoiceHeaderId && x.PurchaseOrderHeaderId == purchaseOrderHeaderId)
                    select stockInHeader
                ).ToListAsync();
            
            headerList.AddRange(stocksInsRelatedToPurchaseOrderThroughPurchaseInvoice);

            foreach (var header in headerList) {
                header.IsBlocked = isBlocked;
            }

            _repository.UpdateRange(headerList);
            await _repository.SaveChanges();
            return true;
        }

		public async Task<int> UpdateAllStockInsEndedDirectlyFromPurchaseOrder(int purchaseOrderHeaderId, bool isEnded)
		{
			var headerList = new List<StockInHeader>();

			var stockInsRelatedToPurchaseOrder = await _repository.FindBy(x => x.PurchaseOrderHeaderId == purchaseOrderHeaderId).ToListAsync();
			headerList.AddRange(stockInsRelatedToPurchaseOrder);

			foreach (var header in headerList)
			{
				header.IsEnded = isEnded;
			}

            _repository.UpdateRange(headerList);
			await _repository.SaveChanges();
			return headerList.Count;
		}

		public async Task<int> UpdateAllStockInsEndedDirectlyFromPurchaseInvoice(int purchaseInvoiceHeaderId, bool isEnded)
		{
			var headerList = new List<StockInHeader>();

			var stockInsRelatedToPurchaseInvoice = await _repository.FindBy(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId).ToListAsync();
			headerList.AddRange(stockInsRelatedToPurchaseInvoice);

			foreach (var header in headerList)
			{
				header.IsEnded = isEnded;
			}

            _repository.UpdateRange(headerList);
			await _repository.SaveChanges();
			return headerList.Count;
		}

		public async Task<ResponseDto> SaveStockInHeader(StockInHeaderDto stockInHeader, bool hasApprove, bool approved, int? requestId, string? documentReference, bool shouldInitializeFlags)
		{
			var separateYears = await _applicationSettingService.SeparateYears(stockInHeader.StoreId);

			if (hasApprove)
            {
                if (stockInHeader.StockInHeaderId == 0)
                {
                    return await CreateStockInHeader(stockInHeader, hasApprove, approved, requestId, documentReference, shouldInitializeFlags, separateYears);
                }
                else
                {
                    return await UpdateStockInHeader(stockInHeader);
                }
            }
            else
            {
                var stockInHeaderExist = await IsStockInCodeExist(stockInHeader.StockInHeaderId, stockInHeader.DocumentCode, stockInHeader.StoreId, separateYears, stockInHeader.DocumentDate, stockInHeader.StockTypeId, stockInHeader.Prefix, stockInHeader.Suffix);
                if (stockInHeaderExist.Success)
                {
                    var nextStockInCode = await GetNextStockInCode(stockInHeader.StoreId, separateYears, stockInHeader.DocumentDate, stockInHeader.StockTypeId, stockInHeader.Prefix, stockInHeader.Suffix);
                    return new ResponseDto() { Id = nextStockInCode, ResponseType = ResponseTypeData.Confirm, Success = false, Message = await _genericMessageService.GetMessage(StockTypeData.ToMenuCode(stockInHeader.StockTypeId), GenericMessageData.CodeAlreadyExist, $"{nextStockInCode}") };
                }
                else
                {
                    if (stockInHeader.StockInHeaderId == 0)
                    {
                        return await CreateStockInHeader(stockInHeader, hasApprove, approved, requestId, documentReference, shouldInitializeFlags, separateYears);
                    }
                    else
                    {
                        return await UpdateStockInHeader(stockInHeader);
                    }
                }
            }
        }

        public async Task<ResponseDto> CreateStockInHeader(StockInHeaderDto stockInHeader, bool hasApprove, bool approved, int? requestId, string? documentReference, bool shouldInitializeFlags, bool separateYears)
        {
            int stockInCode;
            string? prefix;
            string? suffix;
            var nextStockInCode = await GetStockInCodeInternal(stockInHeader.StoreId, separateYears, stockInHeader.DocumentDate, stockInHeader.StockTypeId);
            if (hasApprove && approved)
            {
                stockInCode = nextStockInCode.NextCode;
                prefix = nextStockInCode.Prefix;
                suffix = nextStockInCode.Suffix;
            }
            else
            {
                stockInCode = stockInHeader.DocumentCode != 0 ? stockInHeader.DocumentCode : nextStockInCode.NextCode;
                prefix = string.IsNullOrWhiteSpace(stockInHeader.Prefix) ? nextStockInCode.Prefix : stockInHeader.Prefix;
                suffix = string.IsNullOrWhiteSpace(stockInHeader.Suffix) ? nextStockInCode.Suffix : stockInHeader.Suffix;
            }

            var stockInHeaderId = await GetNextId();
            var newStockInHeader = new StockInHeader()
            {
                StockInHeaderId = stockInHeaderId,
                Prefix = prefix,
                DocumentCode = stockInCode,
                Suffix = suffix,
                DocumentReference = documentReference != null ? documentReference : (hasApprove ? $"{DocumentReferenceData.Approval}{requestId}" : $"{StockTypeData.ToDocumentReference(stockInHeader.StockTypeId)}{stockInHeaderId}"),
                StockTypeId = stockInHeader.StockTypeId,
                PurchaseOrderHeaderId = stockInHeader.PurchaseOrderHeaderId,
                PurchaseInvoiceHeaderId = stockInHeader.PurchaseInvoiceHeaderId,
                SupplierId = stockInHeader.SupplierId,
                StoreId = stockInHeader.StoreId,
                DocumentDate = stockInHeader.DocumentDate,
                EntryDate = DateHelper.GetDateTimeNow(),
                Reference = stockInHeader.Reference,
                TotalValue = stockInHeader.TotalValue,
                DiscountPercent = stockInHeader.DiscountPercent,
                DiscountValue = stockInHeader.DiscountValue,
                TotalItemDiscount = stockInHeader.TotalItemDiscount,
                GrossValue = stockInHeader.GrossValue,
                VatValue = stockInHeader.VatValue,
                SubNetValue = stockInHeader.SubNetValue,
                OtherTaxValue = stockInHeader.OtherTaxValue,
                NetValueBeforeAdditionalDiscount = stockInHeader.NetValueBeforeAdditionalDiscount,
                AdditionalDiscountValue = stockInHeader.AdditionalDiscountValue,
                NetValue = stockInHeader.NetValue,
                TotalCostValue = stockInHeader.TotalCostValue,
                RemarksAr = stockInHeader.RemarksAr,
                RemarksEn = stockInHeader.RemarksEn,
                IsClosed = shouldInitializeFlags ? stockInHeader.IsClosed : false,
                IsEnded = shouldInitializeFlags ? stockInHeader.IsEnded : false,
                IsBlocked = shouldInitializeFlags ? stockInHeader.IsBlocked : false,
                ArchiveHeaderId = stockInHeader.ArchiveHeaderId,
                MenuCode = StockTypeData.ToMenuCode(stockInHeader.StockTypeId),


                CreatedAt = DateHelper.GetDateTimeNow(),
                UserNameCreated = await _httpContextAccessor!.GetUserName(),
                IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
                Hide = false,
            };

            var stockInHeaderValidator = await new StockInHeaderValidator(_localizer).ValidateAsync(newStockInHeader);
            var validationResult = stockInHeaderValidator.IsValid;
            if (validationResult)
            {
                await _repository.Insert(newStockInHeader);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = newStockInHeader.StockInHeaderId, Success = true, Message = await _genericMessageService.GetMessage(StockTypeData.ToMenuCode(newStockInHeader.StockTypeId), GenericMessageData.CreatedSuccessWithCode, $"{newStockInHeader.Prefix}{newStockInHeader.DocumentCode}{newStockInHeader.Suffix}") };
            }
            else
            {
                return new ResponseDto() { Id = newStockInHeader.StockInHeaderId, Success = false, Message = stockInHeaderValidator.ToString("~") };
            }
        }

        public async Task<ResponseDto> UpdateStockInHeader(StockInHeaderDto stockInHeader)
        {
            var stockInHeaderDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.StockInHeaderId == stockInHeader.StockInHeaderId);
            if (stockInHeaderDb != null)
            {
                stockInHeaderDb.StockTypeId = stockInHeaderDb.StockTypeId;
                stockInHeaderDb.PurchaseOrderHeaderId = stockInHeader.PurchaseOrderHeaderId;
                stockInHeaderDb.PurchaseInvoiceHeaderId = stockInHeader.PurchaseInvoiceHeaderId;
                stockInHeaderDb.SupplierId = stockInHeader.SupplierId;
                stockInHeaderDb.StoreId = stockInHeader.StoreId;
                stockInHeaderDb.DocumentDate = stockInHeader.DocumentDate;
                stockInHeaderDb.Reference = stockInHeader.Reference;
                stockInHeaderDb.TotalValue = stockInHeader.TotalValue;
                stockInHeaderDb.DiscountPercent = stockInHeader.DiscountPercent;
                stockInHeaderDb.DiscountValue = stockInHeader.DiscountValue;
                stockInHeaderDb.TotalItemDiscount = stockInHeader.TotalItemDiscount;
                stockInHeaderDb.GrossValue = stockInHeader.GrossValue;
                stockInHeaderDb.VatValue = stockInHeader.VatValue;
                stockInHeaderDb.SubNetValue = stockInHeader.SubNetValue;
                stockInHeaderDb.OtherTaxValue = stockInHeader.OtherTaxValue;
                stockInHeaderDb.NetValueBeforeAdditionalDiscount = stockInHeader.NetValueBeforeAdditionalDiscount;
                stockInHeaderDb.AdditionalDiscountValue = stockInHeader.AdditionalDiscountValue;
                stockInHeaderDb.NetValue = stockInHeader.NetValue;
                stockInHeaderDb.TotalCostValue = stockInHeader.TotalCostValue;
                stockInHeaderDb.RemarksAr = stockInHeader.RemarksAr;
                stockInHeaderDb.RemarksEn = stockInHeader.RemarksEn;
                stockInHeaderDb.ArchiveHeaderId = stockInHeader.ArchiveHeaderId;
                
                stockInHeaderDb.ModifiedAt = DateHelper.GetDateTimeNow();
                stockInHeaderDb.UserNameModified = await _httpContextAccessor!.GetUserName();
                stockInHeaderDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();

				var stockInHeaderValidator = await new StockInHeaderValidator(_localizer).ValidateAsync(stockInHeaderDb);
                var validationResult = stockInHeaderValidator.IsValid;
                if (validationResult)
                {
                    _repository.Update(stockInHeaderDb);
                    await _repository.SaveChanges();
                    return new ResponseDto() { Id = stockInHeaderDb.StockInHeaderId, Success = true, Message = await _genericMessageService.GetMessage(StockTypeData.ToMenuCode(stockInHeaderDb.StockTypeId), GenericMessageData.UpdatedSuccessWithCode, $"{stockInHeaderDb.Prefix}{stockInHeaderDb.DocumentCode}{stockInHeaderDb.Suffix}") };
                }
                else
                {
                    return new ResponseDto() { Id = 0, Success = false, Message = stockInHeaderValidator.ToString("~") };
                }
            }
            return new ResponseDto() { Id = 0, Success = false, Message = await _genericMessageService.GetMessage(StockTypeData.ToMenuCode(stockInHeader.StockTypeId), GenericMessageData.NotFound) };
        }

        public async Task<ResponseDto> IsStockInCodeExist(int stockInHeaderId, int stockInCode, int storeId, bool separateYears, DateTime documentDate, int stockTypeId, string? prefix, string? suffix)
		{
			var stockInHeader = await _repository.GetAll().FirstOrDefaultAsync(x => x.StockInHeaderId != stockInHeaderId && (x.StoreId == storeId && (!separateYears || x.DocumentDate.Year == documentDate.Year) && x.StockTypeId == stockTypeId && x.Prefix == prefix && x.DocumentCode == stockInCode && x.Suffix == suffix));
			if (stockInHeader is not null)
			{
                return new ResponseDto() { Id = stockInHeader.StockInHeaderId, Success = true };
            }
			return new ResponseDto() {  Id = 0, Success = false };
		}

        public async Task<int> GetNextId()
        {
            int id = 1;
            try { id = await _repository.GetAll().MaxAsync(a => a.StockInHeaderId) + 1; } catch { id = 1; }
            return id;
        }

        public async Task<ResponseDto> DeleteStockInHeader(int id, int menuCode)
		{
            var stockInHeader = await _repository.GetAll().FirstOrDefaultAsync(x => x.StockInHeaderId == id);
            if (stockInHeader != null)
            {
                _repository.Delete(stockInHeader);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = id, Success = true, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.DeleteSuccessWithCode, $"{stockInHeader.Prefix}{stockInHeader.DocumentCode}{stockInHeader.Suffix}") };
            }
            return new ResponseDto() { Id = id, Success = false, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.NotFound) };
        }

        public async Task<int> GetNextStockInCode(int storeId, bool separateYears, DateTime documentDate, int stockTypeId, string? prefix, string? suffix)
        {
            int id = 1;
            try
            {
                id = await _repository.GetAll().AsNoTracking().Where(x => (!separateYears || x.DocumentDate.Year == documentDate.Year) && x.StockTypeId == stockTypeId && x.Prefix == prefix && x.Suffix == suffix && x.StoreId == storeId).MaxAsync(a => a.DocumentCode) + 1;
            }
            catch { id = 1; }
            return id;
        }
    }
}
