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
using Shared.CoreOne.Contracts.Basics;
using System.Security;
using Shared.CoreOne.Models.Domain.Modules;
using static Shared.Helper.Models.StaticData.LanguageData;
using Inventory.CoreOne.Models.Domain;
using Shared.CoreOne.Models.Domain.Archive;
using Inventory.CoreOne.Models.Dtos.ViewModels;
using Shared.Helper.Logic;
using Shared.CoreOne.Contracts.Settings;

namespace Purchases.Service.Services
{
	public class PurchaseOrderHeaderService : BaseService<PurchaseOrderHeader>, IPurchaseOrderHeaderService
	{
        private readonly IGenericMessageService _genericMessageService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IStoreService _storeService;
        private readonly ISupplierService _supplierService;
        private readonly IShipmentTypeService _shipmentTypeService;
		private readonly IStringLocalizer<PurchaseOrderHeaderService> _localizer;
		private readonly IMenuEncodingService _menuEncodingService;
        private readonly IDocumentStatusService _documentStatusService;
        private readonly IApplicationSettingService _applicationSettingService;

		public PurchaseOrderHeaderService(IGenericMessageService genericMessageService, IRepository<PurchaseOrderHeader> repository, IHttpContextAccessor httpContextAccessor, IStoreService storeService, IStringLocalizer<PurchaseOrderHeaderService> localizer, IMenuEncodingService menuEncodingService, ISupplierService supplierService, IShipmentTypeService shipmentTypeService, IDocumentStatusService documentStatusService, IApplicationSettingService applicationSettingService) : base(repository)
		{
            _genericMessageService = genericMessageService;
			_httpContextAccessor = httpContextAccessor;
			_storeService = storeService;
            _supplierService = supplierService;
            _shipmentTypeService = shipmentTypeService;
			_localizer = localizer;
			_menuEncodingService = menuEncodingService;
            _documentStatusService = documentStatusService;
            _applicationSettingService = applicationSettingService;
		}

		public IQueryable<PurchaseOrderHeaderDto> GetPurchaseOrderHeaders()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var data = from purchaseOrderHeader in _repository.GetAll()
				from store in _storeService.GetAll().Where(x => x.StoreId == purchaseOrderHeader.StoreId)
				from supplier in _supplierService.GetAll().Where(x => x.SupplierId == purchaseOrderHeader.SupplierId).DefaultIfEmpty()
                from shipmentType in _shipmentTypeService.GetAll().Where(x => x.ShipmentTypeId == purchaseOrderHeader.ShipmentTypeId).DefaultIfEmpty()
                from documentStatus in _documentStatusService.GetAll().Where(x => x.DocumentStatusId == purchaseOrderHeader.DocumentStatusId)
                select new PurchaseOrderHeaderDto()
				{
					PurchaseOrderHeaderId = purchaseOrderHeader.PurchaseOrderHeaderId,
					Prefix = purchaseOrderHeader.Prefix,
					DocumentCode = purchaseOrderHeader.DocumentCode,
					Suffix = purchaseOrderHeader.Suffix,
					DocumentFullCode = $"{purchaseOrderHeader.Prefix}{purchaseOrderHeader.DocumentCode}{purchaseOrderHeader.Suffix}",
					SupplierQuotationHeaderId = purchaseOrderHeader.SupplierQuotationHeaderId,
					DocumentDate = purchaseOrderHeader.DocumentDate,
					EntryDate = purchaseOrderHeader.EntryDate,
                    DocumentReference = purchaseOrderHeader.DocumentReference,
                    SupplierId = purchaseOrderHeader.SupplierId,
                    SupplierCode = supplier.SupplierCode,
                    SupplierName = supplier != null? language == LanguageCode.Arabic ? supplier.SupplierNameAr : supplier.SupplierNameEn : null,
                    StoreId = purchaseOrderHeader.StoreId,
					StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
                    TaxTypeId = purchaseOrderHeader.TaxTypeId,
                    Reference = purchaseOrderHeader.Reference,
                    CreditPayment = purchaseOrderHeader.CreditPayment,
                    PaymentPeriodDays = purchaseOrderHeader.PaymentPeriodDays,
                    DueDate = purchaseOrderHeader.DueDate,
                    ShipmentTypeId = purchaseOrderHeader.ShipmentTypeId,
                    ShipmentTypeName = shipmentType != null ? language == LanguageCode.Arabic ? shipmentType.ShipmentTypeNameAr : shipmentType.ShipmentTypeNameEn : null,
                    DeliveryDate = purchaseOrderHeader.DeliveryDate,
                    TotalValue = purchaseOrderHeader.TotalValue,
					DiscountPercent = purchaseOrderHeader.DiscountPercent,
					DiscountValue = purchaseOrderHeader.DiscountValue,
					TotalItemDiscount = purchaseOrderHeader.TotalItemDiscount,
					GrossValue = purchaseOrderHeader.GrossValue,
					VatValue = purchaseOrderHeader.VatValue,
					SubNetValue = purchaseOrderHeader.SubNetValue,
					OtherTaxValue = purchaseOrderHeader.OtherTaxValue,
                    NetValueBeforeAdditionalDiscount = purchaseOrderHeader.NetValueBeforeAdditionalDiscount,
                    AdditionalDiscountValue = purchaseOrderHeader.AdditionalDiscountValue,
					NetValue = purchaseOrderHeader.NetValue,
                    TotalCostValue = purchaseOrderHeader.TotalCostValue,
					RemarksAr = purchaseOrderHeader.RemarksAr,
					RemarksEn = purchaseOrderHeader.RemarksEn,
					IsClosed = purchaseOrderHeader.IsClosed || purchaseOrderHeader.IsEnded || purchaseOrderHeader.IsBlocked,
                    IsCancelled = purchaseOrderHeader.IsCancelled,
                    IsEnded = purchaseOrderHeader.IsEnded,
                    IsBlocked = purchaseOrderHeader.IsBlocked,
                    DocumentStatusId = purchaseOrderHeader.DocumentStatusId,
                    DocumentStatusName = language == LanguageCode.Arabic ? documentStatus.DocumentStatusNameAr : documentStatus.DocumentStatusNameEn, 
					ArchiveHeaderId = purchaseOrderHeader.ArchiveHeaderId,

                    CreatedAt = purchaseOrderHeader.CreatedAt,
                    UserNameCreated = purchaseOrderHeader.UserNameCreated,
                    IpAddressCreated = purchaseOrderHeader.IpAddressCreated,

                    ModifiedAt = purchaseOrderHeader.ModifiedAt,
                    UserNameModified = purchaseOrderHeader.UserNameModified,
                    IpAddressModified = purchaseOrderHeader.IpAddressModified
                };
			return data;
		}

        public IQueryable<PurchaseOrderHeaderDto> GetUserPurchaseOrderHeaders()
        {
            var userStore = _httpContextAccessor.GetCurrentUserStore();
            return GetPurchaseOrderHeaders().Where(x => x.StoreId == userStore);
        }

		public IQueryable<PurchaseOrderHeaderDto> GetPurchaseOrderHeadersByStoreId(int storeId, int? supplierId, int purchaseOrderHeaderId)
		{
            supplierId ??= 0;
            if (purchaseOrderHeaderId == 0)
            {
                return GetPurchaseOrderHeaders().Where(x => x.StoreId == storeId && (supplierId == 0 || x.SupplierId == supplierId) && x.IsEnded == false && x.IsBlocked == false && x.TaxTypeId != 0);
            }
            else
            {
                return GetPurchaseOrderHeaders().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeaderId);
            }
        }

        public IQueryable<PurchaseOrderHeaderDto> GetPurchaseOrderHeadersByStoreIdAndMenuCode(int storeId, int? supplierId, int menuCode, int purchaseOrderHeaderId)
        {
            supplierId ??= 0;
            if (purchaseOrderHeaderId != 0)
            {
                return GetPurchaseOrderHeaders().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeaderId);
            }
            else if (menuCode == MenuCodeData.ReceiptStatement)
            {
                return GetPurchaseOrderHeaders().Where(x => x.StoreId == storeId && (supplierId == 0 || x.SupplierId == supplierId) && x.IsBlocked == false && x.IsEnded == false && x.DocumentStatusId != DocumentStatusData.QuantityFullyReceived /*make sure purchase order has quantity left to receive*/ && x.TaxTypeId != 0);
            }
            else if (menuCode == MenuCodeData.PurchaseInvoiceInterim)
            {
                return GetPurchaseOrderHeaders().Where(x => x.StoreId == storeId && (supplierId == 0 || x.SupplierId == supplierId) && x.IsBlocked == false && x.IsEnded == false && x.DocumentStatusId != DocumentStatusData.PurchaseOrderCreated /*make sure purchase order is atleast partially received*/ && x.TaxTypeId != 0);
            }
            else
            {
                return Enumerable.Empty<PurchaseOrderHeaderDto>().AsQueryable();
            }
        }

        public async Task<PurchaseOrderHeaderDto?> GetPurchaseOrderHeaderById(int id)
		{
			return await GetPurchaseOrderHeaders().FirstOrDefaultAsync(x => x.PurchaseOrderHeaderId == id);
		}

        public async Task<bool> UpdateBlocked(int? purchaseOrderHeaderId, bool isBlocked)
        {
            var header = await _repository.FindBy(x => x.PurchaseOrderHeaderId == purchaseOrderHeaderId).FirstOrDefaultAsync();
            if (header is null) return false;

            header.IsBlocked = isBlocked;
            _repository.Update(header);
            await _repository.SaveChanges();
            return true;
        }

        public async Task<bool> UpdateClosed(int? purchaseOrderHeaderId, bool isClosed)
        {
            var header = await _repository.FindBy(x => x.PurchaseOrderHeaderId == purchaseOrderHeaderId).FirstOrDefaultAsync();
            if (header is null) return false;

            header.IsClosed = isClosed;
            _repository.Update(header);
            await _repository.SaveChanges();
            return true;
        }

        public async Task<bool> UpdateEnded(int? purchaseOrderHeaderId, bool isEnded)
        {
            var header = await _repository.FindBy(x => x.PurchaseOrderHeaderId == purchaseOrderHeaderId).FirstOrDefaultAsync();
            if (header is null) return false;

            header.IsEnded = isEnded;
            _repository.Update(header);
            await _repository.SaveChanges();
            return true;
        }

        public async Task<bool> UpdateEndedAndClosed(int? purchaseOrderHeaderId, bool isEnded, bool isClosed)
        {
            var header = await _repository.FindBy(x => x.PurchaseOrderHeaderId == purchaseOrderHeaderId).FirstOrDefaultAsync();
            if (header is null) return false;

            header.IsEnded = isEnded;
            header.IsClosed = isClosed;
            _repository.Update(header);
            await _repository.SaveChanges();
            return true;
        }

        public async Task<bool> UpdateDocumentStatus(int purchaseOrderHeaderId, int documentStatusId)
        {
            var header = await _repository.FindBy(x => x.PurchaseOrderHeaderId == purchaseOrderHeaderId).FirstOrDefaultAsync();
            if (header is null) return false;

            header.DocumentStatusId = documentStatusId;
            _repository.Update(header);
            await _repository.SaveChanges();
            return true;
        }

        public async Task<DocumentCodeDto> GetPurchaseOrderCode(int storeId, DateTime documentDate)
        {
            var separateYears = await _applicationSettingService.SeparateYears(storeId);
            return await GetPurchaseOrderCodeInternal(storeId, separateYears, documentDate);
        }

		public async Task<DocumentCodeDto> GetPurchaseOrderCodeInternal(int storeId, bool separateYears, DateTime documentDate)
		{
			var encoding = await _menuEncodingService.GetMenuEncoding(storeId, MenuCodeData.PurchaseOrder);
			var code = await GetNextPurchaseOrderCode(storeId, separateYears, documentDate, encoding.Prefix, encoding.Suffix);
			return new DocumentCodeDto() { NextCode = code, Prefix = encoding.Prefix, Suffix = encoding.Suffix };
		}

		public async Task<ResponseDto> SavePurchaseOrderHeader(PurchaseOrderHeaderDto purchaseOrderHeader, bool hasApprove, bool approved, int? requestId, bool shouldValidate, string? documentReference, int documentStatusId, bool shouldInitializeFlags)
		{
            var separateYears = await _applicationSettingService.SeparateYears(purchaseOrderHeader.StoreId);

            if (hasApprove)
			{
                if (purchaseOrderHeader.PurchaseOrderHeaderId == 0)
                {
                    return await CreatePurchaseOrderHeader(purchaseOrderHeader, hasApprove, approved, requestId, documentReference, documentStatusId, shouldInitializeFlags, separateYears);
                }
                else
                {
                    return await UpdatePurchaseOrderHeader(purchaseOrderHeader, shouldValidate);
                }
            }
            else
            {
                var purchaseOrderHeaderExist = await IsPurchaseOrderCodeExist(purchaseOrderHeader.PurchaseOrderHeaderId, purchaseOrderHeader.DocumentCode, purchaseOrderHeader.StoreId, separateYears, purchaseOrderHeader.DocumentDate, purchaseOrderHeader.Prefix, purchaseOrderHeader.Suffix);
                if (purchaseOrderHeaderExist.Success)
                {
                    var nextPurchaseOrderCode = await GetNextPurchaseOrderCode(purchaseOrderHeader.StoreId, separateYears, purchaseOrderHeader.DocumentDate, purchaseOrderHeader.Prefix, purchaseOrderHeader.Suffix);
                    return new ResponseDto() { Id = nextPurchaseOrderCode, ResponseType = ResponseTypeData.Confirm, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.PurchaseOrder, GenericMessageData.CodeAlreadyExist, $"{nextPurchaseOrderCode}") };
                }
                else
                {
                    if (purchaseOrderHeader.PurchaseOrderHeaderId == 0)
                    {
                        return await CreatePurchaseOrderHeader(purchaseOrderHeader, hasApprove, approved, requestId, documentReference, documentStatusId, shouldInitializeFlags, separateYears);
                    }
                    else
                    {
                        return await UpdatePurchaseOrderHeader(purchaseOrderHeader, shouldValidate);
                    }
                }
            }
        }

        public async Task<ResponseDto> CreatePurchaseOrderHeader(PurchaseOrderHeaderDto purchaseOrderHeader, bool hasApprove, bool approved, int? requestId, string? documentReference, int documentStatusId, bool shouldInitializeFlags, bool separateYears)
        {
            int purchaseOrderCode;
            string? prefix;
            string? suffix;
            var nextPurchaseOrderCode = await GetPurchaseOrderCodeInternal(purchaseOrderHeader.StoreId, separateYears, purchaseOrderHeader.DocumentDate);
            if (hasApprove && approved)
            {
                purchaseOrderCode = nextPurchaseOrderCode.NextCode;
                prefix = nextPurchaseOrderCode.Prefix;
                suffix = nextPurchaseOrderCode.Suffix;
            }
            else
            {
                purchaseOrderCode = purchaseOrderHeader.DocumentCode != 0 ? purchaseOrderHeader.DocumentCode : nextPurchaseOrderCode.NextCode;
                prefix = string.IsNullOrWhiteSpace(purchaseOrderHeader.Prefix) ? nextPurchaseOrderCode.Prefix : purchaseOrderHeader.Prefix;
                suffix = string.IsNullOrWhiteSpace(purchaseOrderHeader.Suffix) ? nextPurchaseOrderCode.Suffix : purchaseOrderHeader.Suffix;
            }

            var purchaseOrderHeaderId = await GetNextId();
            var newPurchaseOrderHeader = new PurchaseOrderHeader()
            {
                PurchaseOrderHeaderId = purchaseOrderHeaderId,
                Prefix = prefix,
                DocumentCode = purchaseOrderCode,
                Suffix = suffix,
                SupplierQuotationHeaderId = purchaseOrderHeader.SupplierQuotationHeaderId,
                DocumentDate = purchaseOrderHeader.DocumentDate,
                EntryDate = DateHelper.GetDateTimeNow(),
                DocumentReference = documentReference != null ? documentReference : (hasApprove ? $"{DocumentReferenceData.Approval}{requestId}" : $"{DocumentReferenceData.PurchaseOrder}{purchaseOrderHeaderId}"),
                SupplierId = purchaseOrderHeader.SupplierId,
                StoreId = purchaseOrderHeader.StoreId,
                TaxTypeId = purchaseOrderHeader.TaxTypeId,
                Reference = purchaseOrderHeader.Reference,
                CreditPayment = purchaseOrderHeader.CreditPayment,
                PaymentPeriodDays = purchaseOrderHeader.PaymentPeriodDays,
                DueDate = purchaseOrderHeader.DueDate,
                ShipmentTypeId = purchaseOrderHeader.ShipmentTypeId,
                DeliveryDate = purchaseOrderHeader.DeliveryDate,
                TotalValue = purchaseOrderHeader.TotalValue,
                DiscountPercent = purchaseOrderHeader.DiscountPercent,
                DiscountValue = purchaseOrderHeader.DiscountValue,
                TotalItemDiscount = purchaseOrderHeader.TotalItemDiscount,
                GrossValue = purchaseOrderHeader.GrossValue,
                VatValue = purchaseOrderHeader.VatValue,
                SubNetValue = purchaseOrderHeader.SubNetValue,
                OtherTaxValue = purchaseOrderHeader.OtherTaxValue,
                NetValueBeforeAdditionalDiscount = purchaseOrderHeader.NetValueBeforeAdditionalDiscount,
                AdditionalDiscountValue = purchaseOrderHeader.AdditionalDiscountValue,
                NetValue = purchaseOrderHeader.NetValue,
                TotalCostValue = purchaseOrderHeader.TotalCostValue,
                RemarksAr = purchaseOrderHeader.RemarksAr,
                RemarksEn = purchaseOrderHeader.RemarksEn,
                IsClosed = shouldInitializeFlags ? purchaseOrderHeader.IsClosed : false,
                IsCancelled = purchaseOrderHeader.IsCancelled,
                IsEnded = shouldInitializeFlags ? purchaseOrderHeader.IsEnded : false,
                IsBlocked = shouldInitializeFlags ? purchaseOrderHeader.IsBlocked : false,
                DocumentStatusId = documentStatusId,
                ArchiveHeaderId = purchaseOrderHeader.ArchiveHeaderId,


                CreatedAt = DateHelper.GetDateTimeNow(),
                UserNameCreated = await _httpContextAccessor!.GetUserName(),
                IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
                Hide = false,
            };

            var purchaseOrderHeaderValidator = await new PurchaseOrderHeaderValidator(_localizer).ValidateAsync(newPurchaseOrderHeader);
            var validationResult = purchaseOrderHeaderValidator.IsValid;
            if (validationResult)
            {
                await _repository.Insert(newPurchaseOrderHeader);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = newPurchaseOrderHeader.PurchaseOrderHeaderId, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.PurchaseOrder, GenericMessageData.CreatedSuccessWithCode, $"{newPurchaseOrderHeader.Prefix}{newPurchaseOrderHeader.DocumentCode}{newPurchaseOrderHeader.Suffix}") };
            }
            else
            {
                return new ResponseDto() { Id = newPurchaseOrderHeader.PurchaseOrderHeaderId, Success = false, Message = purchaseOrderHeaderValidator.ToString("~") };
            }
        }

        public async Task<ResponseDto> UpdatePurchaseOrderHeader(PurchaseOrderHeaderDto purchaseOrderHeader, bool shouldValidate)
        {
            var purchaseOrderHeaderDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.PurchaseOrderHeaderId == purchaseOrderHeader.PurchaseOrderHeaderId);
            if (purchaseOrderHeaderDb != null)
            {
                if (shouldValidate)
                {
                    if (purchaseOrderHeaderDb.IsClosed)
                    {
                        return new ResponseDto() { Id = purchaseOrderHeader.PurchaseOrderHeaderId, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.PurchaseOrder, GenericMessageData.CannotUpdateBecauseClosed) };
                    }

                    if (purchaseOrderHeaderDb.IsBlocked)
                    {
                        return new ResponseDto() { Id = purchaseOrderHeader.PurchaseOrderHeaderId, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.PurchaseOrder, GenericMessageData.CannotPerformOperationBecauseStoppedDealingOn) };
                    }
                }

                purchaseOrderHeaderDb.SupplierQuotationHeaderId = purchaseOrderHeader.SupplierQuotationHeaderId;
                purchaseOrderHeaderDb.DocumentDate = purchaseOrderHeader.DocumentDate;
                purchaseOrderHeaderDb.SupplierId = purchaseOrderHeader.SupplierId;
                purchaseOrderHeaderDb.StoreId = purchaseOrderHeader.StoreId;
                purchaseOrderHeaderDb.TaxTypeId = purchaseOrderHeader.TaxTypeId;
                purchaseOrderHeaderDb.Reference = purchaseOrderHeader.Reference;
                purchaseOrderHeaderDb.CreditPayment = purchaseOrderHeader.CreditPayment;
                purchaseOrderHeaderDb.PaymentPeriodDays = purchaseOrderHeader.PaymentPeriodDays;
                purchaseOrderHeaderDb.DueDate = purchaseOrderHeader.DueDate;
                purchaseOrderHeaderDb.ShipmentTypeId = purchaseOrderHeader.ShipmentTypeId;
                purchaseOrderHeaderDb.DeliveryDate = purchaseOrderHeader.DeliveryDate;
                purchaseOrderHeaderDb.TotalValue = purchaseOrderHeader.TotalValue;
                purchaseOrderHeaderDb.DiscountPercent = purchaseOrderHeader.DiscountPercent;
                purchaseOrderHeaderDb.DiscountValue = purchaseOrderHeader.DiscountValue;
                purchaseOrderHeaderDb.TotalItemDiscount = purchaseOrderHeader.TotalItemDiscount;
                purchaseOrderHeaderDb.GrossValue = purchaseOrderHeader.GrossValue;
                purchaseOrderHeaderDb.VatValue = purchaseOrderHeader.VatValue;
                purchaseOrderHeaderDb.SubNetValue = purchaseOrderHeader.SubNetValue;
                purchaseOrderHeaderDb.OtherTaxValue = purchaseOrderHeader.OtherTaxValue;
                purchaseOrderHeaderDb.NetValueBeforeAdditionalDiscount = purchaseOrderHeader.NetValueBeforeAdditionalDiscount;
                purchaseOrderHeaderDb.AdditionalDiscountValue = purchaseOrderHeader.AdditionalDiscountValue;
                purchaseOrderHeaderDb.NetValue = purchaseOrderHeader.NetValue;
                purchaseOrderHeaderDb.TotalCostValue = purchaseOrderHeader.TotalCostValue;
                purchaseOrderHeaderDb.RemarksAr = purchaseOrderHeader.RemarksAr;
                purchaseOrderHeaderDb.RemarksEn = purchaseOrderHeader.RemarksEn;
                purchaseOrderHeaderDb.IsCancelled = purchaseOrderHeader.IsCancelled;
                purchaseOrderHeaderDb.ArchiveHeaderId = purchaseOrderHeader.ArchiveHeaderId;
                
                purchaseOrderHeaderDb.ModifiedAt = DateHelper.GetDateTimeNow();
                purchaseOrderHeaderDb.UserNameModified = await _httpContextAccessor!.GetUserName();
                purchaseOrderHeaderDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();


                var purchaseOrderHeaderValidator = await new PurchaseOrderHeaderValidator(_localizer).ValidateAsync(purchaseOrderHeaderDb);
                var validationResult = purchaseOrderHeaderValidator.IsValid;
                if (validationResult)
                {
                    _repository.Update(purchaseOrderHeaderDb);
                    await _repository.SaveChanges();
                    return new ResponseDto() { Id = purchaseOrderHeaderDb.PurchaseOrderHeaderId, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.PurchaseOrder, GenericMessageData.UpdatedSuccessWithCode, $"{purchaseOrderHeaderDb.Prefix}{purchaseOrderHeaderDb.DocumentCode}{purchaseOrderHeaderDb.Suffix}") };
                }
                else
                {
                    return new ResponseDto() { Id = 0, Success = false, Message = purchaseOrderHeaderValidator.ToString("~") };
                }
            }
            return new ResponseDto() { Id = 0, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.PurchaseOrder, GenericMessageData.NotFound) };
        }

        public async Task<ResponseDto> IsPurchaseOrderCodeExist(int purchaseOrderHeaderId, int purchaseOrderCode, int storeId, bool separateYears, DateTime documentDate, string? prefix, string? suffix)
		{
			var purchaseOrderHeader = await _repository.GetAll().FirstOrDefaultAsync(x => x.PurchaseOrderHeaderId != purchaseOrderHeaderId && (x.StoreId == storeId && (!separateYears || x.DocumentDate.Year == documentDate.Year) && x.Prefix == prefix && x.DocumentCode == purchaseOrderCode && x.Suffix == suffix));
			if (purchaseOrderHeader is not null)
			{
                return new ResponseDto() { Id = purchaseOrderHeader.PurchaseOrderHeaderId, Success = true };
            }
			return new ResponseDto() {  Id = 0, Success = false };
		}

        public async Task<int> GetNextId()
        {
            int id = 1;
            try { id = await _repository.GetAll().MaxAsync(a => a.PurchaseOrderHeaderId) + 1; } catch { id = 1; }
            return id;
        }

        public async Task<ResponseDto> DeletePurchaseOrderHeader(int id)
		{
            var purchaseOrderHeader = await _repository.GetAll().FirstOrDefaultAsync(x => x.PurchaseOrderHeaderId == id);

            if (purchaseOrderHeader != null)
            {
                if (purchaseOrderHeader.IsBlocked)
                {
                    return new ResponseDto() { Id = purchaseOrderHeader.PurchaseOrderHeaderId, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.PurchaseOrder, GenericMessageData.CannotPerformOperationBecauseStoppedDealingOn) };
                }

                _repository.Delete(purchaseOrderHeader);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = id, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.PurchaseOrder, GenericMessageData.DeleteSuccessWithCode, $"{purchaseOrderHeader.Prefix}{purchaseOrderHeader.DocumentCode}{purchaseOrderHeader.Suffix}") };
            }
            return new ResponseDto() { Id = id, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.PurchaseOrder, GenericMessageData.NotFound) };
        }

        public async Task<int> GetNextPurchaseOrderCode(int storeId, bool separateYears, DateTime documentDate, string? prefix, string? suffix)
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
