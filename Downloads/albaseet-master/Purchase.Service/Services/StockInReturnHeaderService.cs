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
using System.Collections.Generic;
using Shared.Helper.Logic;
using Shared.CoreOne.Contracts.Settings;

namespace Purchases.Service.Services
{
	public class StockInReturnHeaderService : BaseService<StockInReturnHeader>, IStockInReturnHeaderService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IStoreService _storeService;
        private readonly ISupplierService _supplierService;
		private readonly IStringLocalizer<StockInReturnHeaderService> _localizer;
		private readonly IMenuEncodingService _menuEncodingService;
        private readonly IStockInHeaderService _stockInHeaderService;
        private readonly IPurchaseInvoiceHeaderService _purchaseInvoiceHeaderService;
        private readonly IPurchaseOrderHeaderService _purchaseOrderHeaderService;
        private readonly IGenericMessageService _genericMessageService;
		private readonly IMenuService _menuService;
        private readonly IApplicationSettingService _applicationSettingService;

		public StockInReturnHeaderService(IRepository<StockInReturnHeader> repository, IHttpContextAccessor httpContextAccessor, IStoreService storeService, IStringLocalizer<StockInReturnHeaderService> localizer, IMenuEncodingService menuEncodingService, ISupplierService supplierService, IStockInHeaderService stockInHeaderService, IPurchaseInvoiceHeaderService purchaseInvoiceHeaderService, IPurchaseOrderHeaderService purchaseOrderHeaderService, IGenericMessageService genericMessageService, IMenuService menuService, IApplicationSettingService applicationSettingService) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
			_storeService = storeService;
            _supplierService = supplierService;
			_localizer = localizer;
			_menuEncodingService = menuEncodingService;
            _stockInHeaderService = stockInHeaderService;
            _purchaseInvoiceHeaderService = purchaseInvoiceHeaderService;
            _purchaseOrderHeaderService = purchaseOrderHeaderService;
            _genericMessageService = genericMessageService;
            _menuService = menuService;
            _applicationSettingService = applicationSettingService;
		}

		public IQueryable<StockInReturnHeaderDto> GetStockInReturnHeaders()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var data = from stockInReturnHeader in _repository.GetAll()
				from store in _storeService.GetAll().Where(x => x.StoreId == stockInReturnHeader.StoreId)
				from supplier in _supplierService.GetAll().Where(x => x.SupplierId == stockInReturnHeader.SupplierId).DefaultIfEmpty()
                from purchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == stockInReturnHeader.PurchaseInvoiceHeaderId).DefaultIfEmpty()
                from stockInHeader in _stockInHeaderService.GetAll().Where(x => x.StockInHeaderId == stockInReturnHeader.StockInHeaderId).DefaultIfEmpty()
                from stockInPurchaseOrderHeader in _purchaseOrderHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == stockInHeader.PurchaseOrderHeaderId).DefaultIfEmpty()
                from stockInPurchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == stockInHeader.PurchaseInvoiceHeaderId).DefaultIfEmpty()
                from menu in _menuService.GetAll().Where(x => x.MenuCode == stockInReturnHeader.MenuCode).DefaultIfEmpty()
                select new StockInReturnHeaderDto()
				{
					StockInReturnHeaderId = stockInReturnHeader.StockInReturnHeaderId,
					Prefix = stockInReturnHeader.Prefix,
					DocumentCode = stockInReturnHeader.DocumentCode,
					Suffix = stockInReturnHeader.Suffix,
					DocumentFullCode = $"{stockInReturnHeader.Prefix}{stockInReturnHeader.DocumentCode}{stockInReturnHeader.Suffix}",
                    DocumentReference = stockInReturnHeader.DocumentReference,
					StockTypeId = stockInReturnHeader.StockTypeId,
                    StockInHeaderId = stockInReturnHeader.StockInHeaderId,
                    StockInFullCode = stockInHeader != null ? $"{stockInHeader.Prefix}{stockInHeader.DocumentCode}{stockInHeader.Suffix}" : null,
                    StockInDocumentReference = stockInHeader != null ? stockInHeader.DocumentReference : null,
                    StockInPurchaseOrderHeaderId = stockInHeader != null ? stockInHeader.PurchaseOrderHeaderId : null,
                    StockInPurchaseOrderFullCode = stockInPurchaseOrderHeader != null ? $"{stockInPurchaseOrderHeader.Prefix}{stockInPurchaseOrderHeader.DocumentCode}{stockInPurchaseOrderHeader.Suffix}" : null,
                    StockInPurchaseOrderDocumentReference = stockInPurchaseOrderHeader != null ? stockInPurchaseOrderHeader.DocumentReference : null,
                    StockInPurchaseInvoiceHeaderId = stockInHeader != null ? stockInHeader.PurchaseInvoiceHeaderId : null,
                    StockInPurchaseInvoiceFullCode = stockInPurchaseInvoiceHeader != null ? $"{stockInPurchaseInvoiceHeader.Prefix}{stockInPurchaseInvoiceHeader.DocumentCode}{stockInPurchaseInvoiceHeader.Suffix}" : null,
                    StockInPurchaseInvoiceDocumentReference = stockInPurchaseInvoiceHeader != null ? stockInPurchaseInvoiceHeader.DocumentReference : null,
                    PurchaseInvoiceHeaderId = stockInReturnHeader.PurchaseInvoiceHeaderId,
                    PurchaseInvoiceFullCode = purchaseInvoiceHeader != null ? $"{purchaseInvoiceHeader.Prefix}{purchaseInvoiceHeader.DocumentCode}{purchaseInvoiceHeader.Suffix}" : null,
                    PurchaseInvoiceDocumentReference = purchaseInvoiceHeader != null ? purchaseInvoiceHeader.DocumentReference : null,
                    SupplierId = stockInReturnHeader.SupplierId,
                    SupplierCode = supplier.SupplierCode,
                    SupplierName = supplier != null ? language == LanguageCode.Arabic ? supplier.SupplierNameAr : supplier.SupplierNameEn : null,
                    StoreId = stockInReturnHeader.StoreId,
                    StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
                    DocumentDate = stockInReturnHeader.DocumentDate,
					EntryDate = stockInReturnHeader.EntryDate,
                    Reference = stockInReturnHeader.Reference,
                    TotalValue = stockInReturnHeader.TotalValue,
					DiscountPercent = stockInReturnHeader.DiscountPercent,
					DiscountValue = stockInReturnHeader.DiscountValue,
					TotalItemDiscount = stockInReturnHeader.TotalItemDiscount,
					GrossValue = stockInReturnHeader.GrossValue,
					VatValue = stockInReturnHeader.VatValue,
					SubNetValue = stockInReturnHeader.SubNetValue,
					OtherTaxValue = stockInReturnHeader.OtherTaxValue,
                    NetValueBeforeAdditionalDiscount = stockInReturnHeader.NetValueBeforeAdditionalDiscount,
                    AdditionalDiscountValue = stockInReturnHeader.AdditionalDiscountValue,
					NetValue = stockInReturnHeader.NetValue,
                    TotalCostValue = stockInReturnHeader.TotalCostValue,
					RemarksAr = stockInReturnHeader.RemarksAr,
					RemarksEn = stockInReturnHeader.RemarksEn,
					IsClosed = stockInReturnHeader.IsClosed || stockInReturnHeader.IsEnded || stockInReturnHeader.IsBlocked,
                    IsEnded = stockInReturnHeader.IsEnded,
                    IsBlocked = stockInReturnHeader.IsBlocked,
                    ArchiveHeaderId = stockInReturnHeader.ArchiveHeaderId,
					MenuCode = stockInReturnHeader.MenuCode,
					MenuName = menu != null ? language == LanguageCode.Arabic ? menu.MenuNameAr : menu.MenuNameEn : null,

					CreatedAt = stockInReturnHeader.CreatedAt,
                    UserNameCreated = stockInReturnHeader.UserNameCreated,
                    IpAddressCreated = stockInReturnHeader.IpAddressCreated,

                    ModifiedAt = stockInReturnHeader.ModifiedAt,
                    UserNameModified = stockInReturnHeader.UserNameModified,
                    IpAddressModified = stockInReturnHeader.IpAddressModified
                };
			return data;
		}

        public IQueryable<StockInReturnHeaderDto> GetUserStockInReturnHeaders(int stockTypeId)
        {
            var userStore = _httpContextAccessor.GetCurrentUserStore();
            return GetStockInReturnHeaders().Where(x => x.StoreId == userStore && x.StockTypeId == stockTypeId);
        }

		public IQueryable<StockInReturnHeaderDto> GetStockInReturnHeadersByStoreId(int storeId, int? supplierId, int stockTypeId, int stockInReturnHeaderId)
		{
            supplierId ??= 0;
            if (stockInReturnHeaderId == 0)
            {
                return GetStockInReturnHeaders().Where(x => x.StoreId == storeId && (supplierId == 0 || x.SupplierId == supplierId) && x.StockTypeId == stockTypeId && x.IsEnded == false && x.IsBlocked == false);
            }
            else
            {
                return GetStockInReturnHeaders().Where(x => x.StockInReturnHeaderId == stockInReturnHeaderId);
            }
        }

		public async Task<StockInReturnHeaderDto?> GetStockInReturnHeaderById(int id)
		{
			return await GetStockInReturnHeaders().FirstOrDefaultAsync(x => x.StockInReturnHeaderId == id);
		}


        public async Task<DocumentCodeDto> GetStockInReturnCode(int storeId, DateTime documentDate, int stockTypeId)
        {
            var separateYears = await _applicationSettingService.SeparateYears(storeId);
            return await GetStockInReturnCodeInternal(storeId, separateYears, documentDate, stockTypeId);
        }

        public async Task<DocumentCodeDto> GetStockInReturnCodeInternal(int storeId, bool separateYears, DateTime documentDate, int stockTypeId)
        {
            var encoding = await _menuEncodingService.GetMenuEncoding(storeId, StockTypeData.ToMenuCode((byte)stockTypeId));
            var code = await GetNextStockInReturnCode(storeId, separateYears, documentDate, stockTypeId, encoding.Prefix, encoding.Suffix);
            return new DocumentCodeDto() { NextCode = code, Prefix = encoding.Prefix, Suffix = encoding.Suffix };
        }

        public async Task<bool> UpdateAllStockInReturnsBlockedFromPurchaseOrder(int purchaseOrderHeaderId, bool isBlocked)
        {
            var headerList = new List<StockInReturnHeader>();

            //SR->SI->PO
            var stockInReturnsDirectlyThroughStockIn = await (
                    from stockInReturnHeader in _repository.GetAll()
                    from stockInHeader in _stockInHeaderService.GetAll().Where(x => x.StockInHeaderId == stockInReturnHeader.StockInHeaderId && x.PurchaseOrderHeaderId == purchaseOrderHeaderId)
                    select stockInReturnHeader
                ).ToListAsync();

            headerList.AddRange(stockInReturnsDirectlyThroughStockIn);

            //SR->SI->PI->PO
            var stockInReturnThroughStockInThroughPurchaseInvoice = await (
                from stockInReturnHeader in _repository.GetAll()
                from stockInHeader in _stockInHeaderService.GetAll().Where(x => x.StockInHeaderId == stockInReturnHeader.StockInHeaderId)
                from purchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == stockInHeader.PurchaseInvoiceHeaderId && x.PurchaseOrderHeaderId == purchaseOrderHeaderId)
                select stockInReturnHeader
            ).ToListAsync();

            headerList.AddRange(stockInReturnThroughStockInThroughPurchaseInvoice);

            //SR->PI->PO
            var stockInReturnDirectlyThroughPurchaseInvoice = await (
                from stockInReturnHeader in _repository.GetAll()
                from purchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == stockInReturnHeader.PurchaseInvoiceHeaderId && x.PurchaseOrderHeaderId == purchaseOrderHeaderId)
                select stockInReturnHeader
            ).ToListAsync();

            headerList.AddRange(stockInReturnDirectlyThroughPurchaseInvoice);


            foreach (var header in headerList)
            {
                header.IsBlocked = isBlocked;
            }

            _repository.UpdateRange(headerList);
            await _repository.SaveChanges();
            return true;
        }

        public async Task<int> UpdateAllStockInReturnsEndedDirectlyFromPurchaseOrder(int purchaseOrderHeaderId, bool isEnded)
        {
			var headerList = new List<StockInReturnHeader>();

			//SR->SI->PO
			var stockInReturnsDirectlyThroughStockIn = await (
					from stockInReturnHeader in _repository.GetAll()
					from stockInHeader in _stockInHeaderService.GetAll().Where(x => x.StockInHeaderId == stockInReturnHeader.StockInHeaderId && x.PurchaseOrderHeaderId == purchaseOrderHeaderId)
					select stockInReturnHeader
				).ToListAsync();

			headerList.AddRange(stockInReturnsDirectlyThroughStockIn);

			foreach (var header in headerList)
			{
				header.IsEnded = isEnded;
			}

            _repository.UpdateRange(headerList);
			await _repository.SaveChanges();
			return headerList.Count;
        }

		public async Task<int> UpdateAllStockInReturnsOnTheWayEndedFromPurchaseInvoice(int purchaseInvoiceHeaderId, bool isEnded)
		{
			var headerList = new List<StockInReturnHeader>();

			//SR->SI->PO
			var stockInReturnsDirectlyThroughStockIn = await (
					from stockInReturnHeader in _repository.GetAll()
					from stockInHeader in _stockInHeaderService.GetAll().Where(x => x.StockInHeaderId == stockInReturnHeader.StockInHeaderId && x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId)
					select stockInReturnHeader
				).ToListAsync();

			headerList.AddRange(stockInReturnsDirectlyThroughStockIn);

			foreach (var header in headerList)
			{
				header.IsEnded = isEnded;
			}

            _repository.UpdateRange(headerList);
			await _repository.SaveChanges();
			return headerList.Count;
		}

		public async Task<int> UpdateAllStockInReturnsEndedDirectlyFromPurchaseInvoice(int purchaseInvoiceHeaderId, bool isEnded)
		{
			var headerList = new List<StockInReturnHeader>();

			var stockInReturnsRelatedToPurchaseInvoice = await _repository.FindBy(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId).ToListAsync();
			headerList.AddRange(stockInReturnsRelatedToPurchaseInvoice);

			foreach (var header in headerList)
			{
				header.IsEnded = isEnded;
			}

            _repository.UpdateRange(headerList);
			await _repository.SaveChanges();
			return headerList.Count;
		}

		public async Task<ResponseDto> SaveStockInReturnHeader(StockInReturnHeaderDto stockInReturnHeader, bool hasApprove, bool approved, int? requestId, string? documentReference, bool shouldInitializeFlags)
		{
            var separateYears = await _applicationSettingService.SeparateYears(stockInReturnHeader.StoreId);

            if (hasApprove)
            {
                if (stockInReturnHeader.StockInReturnHeaderId == 0)
                {
                    return await CreateStockInReturnHeader(stockInReturnHeader, hasApprove, approved, requestId, documentReference, shouldInitializeFlags, separateYears);
                }
                else
                {
                    return await UpdateStockInReturnHeader(stockInReturnHeader);
                }
            }
            else
            {
                var stockInReturnHeaderExist = await IsStockInReturnCodeExist(stockInReturnHeader.StockInReturnHeaderId, stockInReturnHeader.DocumentCode, stockInReturnHeader.StoreId, separateYears, stockInReturnHeader.DocumentDate, stockInReturnHeader.StockTypeId, stockInReturnHeader.Prefix, stockInReturnHeader.Suffix);
                if (stockInReturnHeaderExist.Success)
                {
                    var nextStockInReturnCode = await GetNextStockInReturnCode(stockInReturnHeader.StoreId, separateYears, stockInReturnHeader.DocumentDate, stockInReturnHeader.StockTypeId, stockInReturnHeader.Prefix, stockInReturnHeader.Suffix);
                    return new ResponseDto() { Id = nextStockInReturnCode, ResponseType = ResponseTypeData.Confirm, Success = false, Message = await _genericMessageService.GetMessage(StockTypeData.ToMenuCode(stockInReturnHeader.StockTypeId), GenericMessageData.CodeAlreadyExist, $"{nextStockInReturnCode}") };
                }
                else
                {
                    if (stockInReturnHeader.StockInReturnHeaderId == 0)
                    {
                        return await CreateStockInReturnHeader(stockInReturnHeader, hasApprove, approved, requestId, documentReference, shouldInitializeFlags, separateYears);
                    }
                    else
                    {
                        return await UpdateStockInReturnHeader(stockInReturnHeader);
                    }
                }
            }
        }

        public async Task<ResponseDto> CreateStockInReturnHeader(StockInReturnHeaderDto stockInReturnHeader, bool hasApprove, bool approved, int? requestId, string? documentReference, bool shouldInitializeFlags, bool separateYears)
        {
            int stockInReturnCode;
            string? prefix;
            string? suffix;
            var nextStockInReturnCode = await GetStockInReturnCodeInternal(stockInReturnHeader.StoreId, separateYears, stockInReturnHeader.DocumentDate, stockInReturnHeader.StockTypeId);
            if (hasApprove && approved)
            {
                stockInReturnCode = nextStockInReturnCode.NextCode;
                prefix = nextStockInReturnCode.Prefix;
                suffix = nextStockInReturnCode.Suffix;
            }
            else
            {
                stockInReturnCode = stockInReturnHeader.DocumentCode != 0 ? stockInReturnHeader.DocumentCode : nextStockInReturnCode.NextCode;
                prefix = string.IsNullOrWhiteSpace(stockInReturnHeader.Prefix) ? nextStockInReturnCode.Prefix : stockInReturnHeader.Prefix;
                suffix = string.IsNullOrWhiteSpace(stockInReturnHeader.Suffix) ? nextStockInReturnCode.Suffix : stockInReturnHeader.Suffix;
            }

            var stockInReturnHeaderId = await GetNextId();
            var newStockInReturnHeader = new StockInReturnHeader()
            {
                StockInReturnHeaderId = stockInReturnHeaderId,
                Prefix = prefix,
                DocumentCode = stockInReturnCode,
                Suffix = suffix,
                DocumentReference = documentReference != null ? documentReference : (hasApprove ? $"{DocumentReferenceData.Approval}{requestId}" : $"{StockTypeData.ToDocumentReference(stockInReturnHeader.StockTypeId)}{stockInReturnHeaderId}"),
                StockTypeId = stockInReturnHeader.StockTypeId,
                StockInHeaderId = stockInReturnHeader.StockInHeaderId,
                PurchaseInvoiceHeaderId = stockInReturnHeader.PurchaseInvoiceHeaderId,
                SupplierId = stockInReturnHeader.SupplierId,
                StoreId = stockInReturnHeader.StoreId,
                DocumentDate = stockInReturnHeader.DocumentDate,
                EntryDate = DateHelper.GetDateTimeNow(),
                Reference = stockInReturnHeader.Reference,
                TotalValue = stockInReturnHeader.TotalValue,
                DiscountPercent = stockInReturnHeader.DiscountPercent,
                DiscountValue = stockInReturnHeader.DiscountValue,
                TotalItemDiscount = stockInReturnHeader.TotalItemDiscount,
                GrossValue = stockInReturnHeader.GrossValue,
                VatValue = stockInReturnHeader.VatValue,
                SubNetValue = stockInReturnHeader.SubNetValue,
                OtherTaxValue = stockInReturnHeader.OtherTaxValue,
                NetValueBeforeAdditionalDiscount = stockInReturnHeader.NetValueBeforeAdditionalDiscount,
                AdditionalDiscountValue = stockInReturnHeader.AdditionalDiscountValue,
                NetValue = stockInReturnHeader.NetValue,
                TotalCostValue = stockInReturnHeader.TotalCostValue,
                RemarksAr = stockInReturnHeader.RemarksAr,
                RemarksEn = stockInReturnHeader.RemarksEn,
                IsClosed = false,
                IsBlocked = false,
                IsEnded = false,
                ArchiveHeaderId = stockInReturnHeader.ArchiveHeaderId,
                MenuCode = StockTypeData.ToMenuCode(stockInReturnHeader.StockTypeId),


                CreatedAt = DateHelper.GetDateTimeNow(),
                UserNameCreated = await _httpContextAccessor!.GetUserName(),
                IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
                Hide = false,
            };

            var stockInReturnHeaderValidator = await new StockInReturnHeaderValidator(_localizer).ValidateAsync(newStockInReturnHeader);
            var validationResult = stockInReturnHeaderValidator.IsValid;
            if (validationResult)
            {
                await _repository.Insert(newStockInReturnHeader);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = newStockInReturnHeader.StockInReturnHeaderId, Success = true, Message = await _genericMessageService.GetMessage(StockTypeData.ToMenuCode(stockInReturnHeader.StockTypeId), GenericMessageData.CreatedSuccessWithCode, $"{newStockInReturnHeader.Prefix}{newStockInReturnHeader.DocumentCode}{newStockInReturnHeader.Suffix}") };
            }
            else
            {
                return new ResponseDto() { Id = newStockInReturnHeader.StockInReturnHeaderId, Success = false, Message = stockInReturnHeaderValidator.ToString("~") };
            }
        }

        public async Task<ResponseDto> UpdateStockInReturnHeader(StockInReturnHeaderDto stockInReturnHeader)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var stockInReturnHeaderDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.StockInReturnHeaderId == stockInReturnHeader.StockInReturnHeaderId);
            if (stockInReturnHeaderDb != null)
            {
                stockInReturnHeaderDb.StockTypeId = stockInReturnHeader.StockTypeId;
                stockInReturnHeaderDb.StockInHeaderId = stockInReturnHeader.StockInHeaderId;
                stockInReturnHeaderDb.PurchaseInvoiceHeaderId = stockInReturnHeader.PurchaseInvoiceHeaderId;
                stockInReturnHeaderDb.SupplierId = stockInReturnHeader.SupplierId;
                stockInReturnHeaderDb.StoreId = stockInReturnHeader.StoreId;
                stockInReturnHeaderDb.DocumentDate = stockInReturnHeader.DocumentDate;
                stockInReturnHeaderDb.Reference = stockInReturnHeader.Reference;
                stockInReturnHeaderDb.TotalValue = stockInReturnHeader.TotalValue;
                stockInReturnHeaderDb.DiscountPercent = stockInReturnHeader.DiscountPercent;
                stockInReturnHeaderDb.DiscountValue = stockInReturnHeader.DiscountValue;
                stockInReturnHeaderDb.TotalItemDiscount = stockInReturnHeader.TotalItemDiscount;
                stockInReturnHeaderDb.GrossValue = stockInReturnHeader.GrossValue;
                stockInReturnHeaderDb.VatValue = stockInReturnHeader.VatValue;
                stockInReturnHeaderDb.SubNetValue = stockInReturnHeader.SubNetValue;
                stockInReturnHeaderDb.OtherTaxValue = stockInReturnHeader.OtherTaxValue;
                stockInReturnHeaderDb.NetValueBeforeAdditionalDiscount = stockInReturnHeader.NetValueBeforeAdditionalDiscount;
                stockInReturnHeaderDb.AdditionalDiscountValue = stockInReturnHeader.AdditionalDiscountValue;
                stockInReturnHeaderDb.NetValue = stockInReturnHeader.NetValue;
                stockInReturnHeaderDb.TotalCostValue = stockInReturnHeader.TotalCostValue;
                stockInReturnHeaderDb.RemarksAr = stockInReturnHeader.RemarksAr;
                stockInReturnHeaderDb.RemarksEn = stockInReturnHeader.RemarksEn;
                stockInReturnHeaderDb.ArchiveHeaderId = stockInReturnHeader.ArchiveHeaderId;
                
                stockInReturnHeaderDb.ModifiedAt = DateHelper.GetDateTimeNow();
                stockInReturnHeaderDb.UserNameModified = await _httpContextAccessor!.GetUserName();
                stockInReturnHeaderDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();


                var stockInReturnHeaderValidator = await new StockInReturnHeaderValidator(_localizer).ValidateAsync(stockInReturnHeaderDb);
                var validationResult = stockInReturnHeaderValidator.IsValid;
                if (validationResult)
                {
                    _repository.Update(stockInReturnHeaderDb);
                    await _repository.SaveChanges();
                    return new ResponseDto() { Id = stockInReturnHeaderDb.StockInReturnHeaderId, Success = true, Message = await _genericMessageService.GetMessage(StockTypeData.ToMenuCode(stockInReturnHeaderDb.StockTypeId), GenericMessageData.UpdatedSuccessWithCode, $"{stockInReturnHeaderDb.Prefix}{stockInReturnHeaderDb.DocumentCode}{stockInReturnHeaderDb.Suffix}") };
                }
                else
                {
                    return new ResponseDto() { Id = 0, Success = false, Message = stockInReturnHeaderValidator.ToString("~") };
                }
            }
            return new ResponseDto() { Id = 0, Success = false, Message = await _genericMessageService.GetMessage(StockTypeData.ToMenuCode(stockInReturnHeader.StockTypeId), GenericMessageData.NotFound) };
        }

        public async Task<ResponseDto> IsStockInReturnCodeExist(int stockInReturnHeaderId, int stockInReturnCode, int storeId, bool separateYears, DateTime documentDate, int stockTypeId, string? prefix, string? suffix)
		{
			var stockInReturnHeader = await _repository.GetAll().FirstOrDefaultAsync(x => x.StockInReturnHeaderId != stockInReturnHeaderId && (x.StoreId == storeId && (!separateYears || x.DocumentDate.Year == documentDate.Year) && x.StockTypeId == stockTypeId && x.Prefix == prefix && x.DocumentCode == stockInReturnCode && x.Suffix == suffix));
			if (stockInReturnHeader is not null)
			{
                return new ResponseDto() { Id = stockInReturnHeader.StockInReturnHeaderId, Success = true };
            }
			return new ResponseDto() {  Id = 0, Success = false };
		}

        public async Task<int> GetNextId()
        {
            int id = 1;
            try { id = await _repository.GetAll().MaxAsync(a => a.StockInReturnHeaderId) + 1; } catch { id = 1; }
            return id;
        }

        public async Task<ResponseDto> DeleteStockInReturnHeader(int id, int menuCode)
		{
            var stockInReturnHeader = await _repository.GetAll().FirstOrDefaultAsync(x => x.StockInReturnHeaderId == id);
            if (stockInReturnHeader != null)
            {
                _repository.Delete(stockInReturnHeader);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = id, Success = true, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.DeleteSuccessWithCode, $"{stockInReturnHeader.Prefix}{stockInReturnHeader.DocumentCode}{stockInReturnHeader.Suffix}") };
            }
            return new ResponseDto() { Id = id, Success = false, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.NotFound) };
        }

        public async Task<int> GetNextStockInReturnCode(int storeId, bool separateYears, DateTime documentDate, int stockTypeId, string? prefix, string? suffix)
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
