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
using Inventory.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne.Contracts.Settings;
using Purchases.CoreOne.Models.StaticData;
using Shared.Helper.Logic;
using Shared.CoreOne.Models.Domain.Menus;
using Shared.Service.Services.Menus;

namespace Purchases.Service.Services
{
    public class PurchaseInvoiceReturnHeaderService : BaseService<PurchaseInvoiceReturnHeader>, IPurchaseInvoiceReturnHeaderService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStoreService _storeService;
        private readonly ISupplierService _supplierService;
        private readonly IPurchaseInvoiceHeaderService _purchaseInvoiceHeaderService;
        private readonly IStringLocalizer<PurchaseInvoiceReturnHeaderService> _localizer;
        private readonly IMenuEncodingService _menuEncodingService;
        private readonly IApplicationSettingService _applicationSettingService;
        private readonly IGenericMessageService _genericMessageService;
		private readonly IMenuService _menuService;

		public PurchaseInvoiceReturnHeaderService(IRepository<PurchaseInvoiceReturnHeader> repository, IHttpContextAccessor httpContextAccessor, IStoreService storeService, IStringLocalizer<PurchaseInvoiceReturnHeaderService> localizer, IMenuEncodingService menuEncodingService, ISupplierService supplierService, IPurchaseInvoiceHeaderService purchaseInvoiceHeaderService, IApplicationSettingService applicationSettingService, IGenericMessageService genericMessageService, IMenuService menuService) : base(repository)
        {
            _httpContextAccessor = httpContextAccessor;
            _storeService = storeService;
            _supplierService = supplierService;
            _purchaseInvoiceHeaderService = purchaseInvoiceHeaderService;
            _localizer = localizer;
            _menuEncodingService = menuEncodingService;
            _applicationSettingService = applicationSettingService;
            _genericMessageService = genericMessageService;
            _menuService = menuService;
        }

        private IQueryable<PurchaseInvoiceReturnHeaderDto> GetPurchaseInvoiceReturnHeaders()
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();
            var data = from purchaseInvoiceReturnHeader in _repository.GetAll()
                       from store in _storeService.GetAll().Where(x => x.StoreId == purchaseInvoiceReturnHeader.StoreId)
                       from supplier in _supplierService.GetAll().Where(x => x.SupplierId == purchaseInvoiceReturnHeader.SupplierId).DefaultIfEmpty()
                       from purchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceReturnHeader.PurchaseInvoiceHeaderId).DefaultIfEmpty()
					   from menu in _menuService.GetAll().Where(x => x.MenuCode == purchaseInvoiceReturnHeader.MenuCode).DefaultIfEmpty()
					   select new PurchaseInvoiceReturnHeaderDto()
                       {
                           PurchaseInvoiceReturnHeaderId = purchaseInvoiceReturnHeader.PurchaseInvoiceReturnHeaderId,
                           Prefix = purchaseInvoiceReturnHeader.Prefix,
                           DocumentCode = purchaseInvoiceReturnHeader.DocumentCode,
                           Suffix = purchaseInvoiceReturnHeader.Suffix,
                           DocumentFullCode = $"{purchaseInvoiceReturnHeader.Prefix}{purchaseInvoiceReturnHeader.DocumentCode}{purchaseInvoiceReturnHeader.Suffix}",
                           PurchaseInvoiceHeaderId = purchaseInvoiceReturnHeader.PurchaseInvoiceHeaderId,
                           PurchaseInvoiceFullCode = purchaseInvoiceHeader != null ? $"{purchaseInvoiceHeader.Prefix}{purchaseInvoiceHeader.DocumentCode}{purchaseInvoiceHeader.Suffix}" : null,
                           PurchaseInvoiceDocumentReference = purchaseInvoiceHeader != null ? purchaseInvoiceHeader.DocumentReference : null,
                           DocumentReference = purchaseInvoiceReturnHeader.DocumentReference,
                           SupplierId = purchaseInvoiceReturnHeader.SupplierId,
                           SupplierCode = supplier.SupplierCode,
                           SupplierName = supplier != null ? language == LanguageCode.Arabic ? supplier.SupplierNameAr : supplier.SupplierNameEn : null,
                           StoreId = purchaseInvoiceReturnHeader.StoreId,
                           StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
                           DocumentDate = purchaseInvoiceReturnHeader.DocumentDate,
                           EntryDate = purchaseInvoiceReturnHeader.EntryDate,
                           Reference = purchaseInvoiceReturnHeader.Reference,
                           IsDirectInvoice = purchaseInvoiceReturnHeader.IsDirectInvoice,
                           CreditPayment = purchaseInvoiceReturnHeader.CreditPayment,
                           TaxTypeId = purchaseInvoiceReturnHeader.TaxTypeId,
                           TotalValue = purchaseInvoiceReturnHeader.TotalValue,
                           DiscountPercent = purchaseInvoiceReturnHeader.DiscountPercent,
                           DiscountValue = purchaseInvoiceReturnHeader.DiscountValue,
                           TotalItemDiscount = purchaseInvoiceReturnHeader.TotalItemDiscount,
                           GrossValue = purchaseInvoiceReturnHeader.GrossValue,
                           VatValue = purchaseInvoiceReturnHeader.VatValue,
                           SubNetValue = purchaseInvoiceReturnHeader.SubNetValue,
                           OtherTaxValue = purchaseInvoiceReturnHeader.OtherTaxValue,
                           NetValueBeforeAdditionalDiscount = purchaseInvoiceReturnHeader.NetValueBeforeAdditionalDiscount,
                           AdditionalDiscountValue = purchaseInvoiceReturnHeader.AdditionalDiscountValue,
                           NetValue = purchaseInvoiceReturnHeader.NetValue,
                           TotalCostValue = purchaseInvoiceReturnHeader.TotalCostValue,
                           DebitAccountId = purchaseInvoiceReturnHeader.DebitAccountId,
                           CreditAccountId = purchaseInvoiceReturnHeader.CreditAccountId,
                           JournalHeaderId = purchaseInvoiceReturnHeader.JournalHeaderId,
                           RemarksAr = purchaseInvoiceReturnHeader.RemarksAr,
                           RemarksEn = purchaseInvoiceReturnHeader.RemarksEn,
                           IsOnTheWay = purchaseInvoiceReturnHeader.IsOnTheWay,
                           IsClosed = purchaseInvoiceReturnHeader.IsClosed || purchaseInvoiceReturnHeader.IsEnded || purchaseInvoiceReturnHeader.IsBlocked,
                           IsEnded = purchaseInvoiceReturnHeader.IsEnded,
                           IsBlocked = purchaseInvoiceReturnHeader.IsBlocked,
                           ArchiveHeaderId = purchaseInvoiceReturnHeader.ArchiveHeaderId,
						   MenuCode = purchaseInvoiceReturnHeader.MenuCode,
						   MenuName = menu != null ? language == LanguageCode.Arabic ? menu.MenuNameAr : menu.MenuNameEn : null,

						   CreatedAt = purchaseInvoiceReturnHeader.CreatedAt,
						   UserNameCreated = purchaseInvoiceReturnHeader.UserNameCreated,
                           IpAddressCreated = purchaseInvoiceReturnHeader.IpAddressCreated,

                           ModifiedAt = purchaseInvoiceReturnHeader.ModifiedAt,
                           UserNameModified = purchaseInvoiceReturnHeader.UserNameModified,
                           IpAddressModified = purchaseInvoiceReturnHeader.IpAddressModified
                       };
            return data;
        }

        public IQueryable<PurchaseInvoiceReturnHeaderDto> GetUserPurchaseInvoiceReturnHeaders(int menuCode)
        {
            var userStore = _httpContextAccessor.GetCurrentUserStore();
            return menuCode switch
            {
                MenuCodeData.PurchaseInvoiceReturn => GetUserPurchaseInvoiceReturnHeadersInternal(false, false, null),
                MenuCodeData.PurchaseInvoiceReturnOnTheWay => GetUserPurchaseInvoiceReturnHeadersInternal(true, false, null),
                MenuCodeData.CashPurchaseInvoiceReturn => GetUserPurchaseInvoiceReturnHeadersInternal(false, true, false),
                MenuCodeData.CreditPurchaseInvoiceReturn => GetUserPurchaseInvoiceReturnHeadersInternal(false, true, true),
                _ => GetPurchaseInvoiceReturnHeaders().Where(x => x.StoreId == userStore)
            };
        }

        private IQueryable<PurchaseInvoiceReturnHeaderDto> GetUserPurchaseInvoiceReturnHeadersInternal(bool isOnTheWay, bool isDirectInvoice, bool? isCreditPayment)
        {
            var userStore = _httpContextAccessor.GetCurrentUserStore();
            return GetPurchaseInvoiceReturnHeaders()
                .Where(x => x.StoreId == userStore && x.IsOnTheWay == isOnTheWay && x.IsDirectInvoice == isDirectInvoice && (isCreditPayment == null || x.CreditPayment == isCreditPayment));
        }

        public IQueryable<PurchaseInvoiceReturnHeaderDto> GetPurchaseInvoiceReturnHeadersByStoreId(int storeId, int? supplierId, int purchaseInvoiceReturnHeaderId)
        {
            supplierId ??= 0;
            if (purchaseInvoiceReturnHeaderId == 0)
            {
                return GetPurchaseInvoiceReturnHeaders().Where(x => x.StoreId == storeId && (supplierId == 0 || x.SupplierId == supplierId) && x.IsEnded == false && x.IsBlocked == false);
            }
            else
            {
                return GetPurchaseInvoiceReturnHeaders().Where(x => x.PurchaseInvoiceReturnHeaderId == purchaseInvoiceReturnHeaderId);
            }
        }

        public async Task<PurchaseInvoiceReturnHeaderDto?> GetPurchaseInvoiceReturnHeaderById(int id)
        {
            return await GetPurchaseInvoiceReturnHeaders().FirstOrDefaultAsync(x => x.PurchaseInvoiceReturnHeaderId == id);
        }

        public async Task<bool> UpdateClosed(int? purchaseInvoiceReturnHeaderId, bool isClosed)
        {
            var header = await _repository.FindBy(x => x.PurchaseInvoiceReturnHeaderId == purchaseInvoiceReturnHeaderId).FirstOrDefaultAsync();
            if (header is null) return false;

            header.IsClosed = isClosed;
            _repository.Update(header);
            await _repository.SaveChanges();
            return true;
        }

        public async Task<bool> UpdateEnded(int? purchaseInvoiceReturnHeaderId, bool isEnded)
        {
            var header = await _repository.FindBy(x => x.PurchaseInvoiceReturnHeaderId == purchaseInvoiceReturnHeaderId).FirstOrDefaultAsync();
            if (header is null) return false;

            header.IsEnded = isEnded;
            _repository.Update(header);
            await _repository.SaveChanges();
            return true;
        }

        public async Task<bool> UpdatePurchaseInvoiceReturnOnTheWayEndedLinkedToPurchaseInvoice(int? purchaseInvoiceHeaderId, bool isEnded)
        {
            var header = await _repository.FindBy(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId && x.IsOnTheWay).FirstOrDefaultAsync();
            if (header is null) return false;

            header.IsEnded = isEnded;
            _repository.Update(header);
            await _repository.SaveChanges();
            return true;
        }

        public async Task<bool> UpdatePurchaseInvoiceReturnNotOnTheWayEndedLinkedToPurchaseInvoice(int? purchaseInvoiceHeaderId, bool isEnded)
        {
            var header = await _repository.FindBy(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId && !x.IsOnTheWay).FirstOrDefaultAsync();
            if (header is null) return false;

            header.IsEnded = isEnded;
            _repository.Update(header);
            await _repository.SaveChanges();
            return true;
        }

        public async Task<DocumentCodeDto> GetPurchaseInvoiceReturnCode(int storeId, DateTime documentDate, bool isOnTheWay, bool isDirectInvoice, bool creditPayment)
        {
            bool separatingCashFromCredit = await _applicationSettingService.SeparateCashFromCreditPurchaseInvoiceReturn(storeId);
            bool separateYears = await _applicationSettingService.SeparateYears(storeId);

            return await GetPurchaseInvoiceReturnCodeInternal(storeId, separateYears, documentDate, isOnTheWay, isDirectInvoice, separatingCashFromCredit,creditPayment);
        }

        //function used to avoid querying settings two times when create invoice
        private async Task<DocumentCodeDto> GetPurchaseInvoiceReturnCodeInternal(int storeId, bool separateYears, DateTime documentDate, bool isOnTheWay, bool isDirectInvoice, bool separatingCashFromCredit, bool creditPayment)
        {
            var encoding = await _menuEncodingService.GetMenuEncoding(storeId, PurchaseInvoiceReturnMenuCodeHelper.GetMenuCode(isOnTheWay, isDirectInvoice, creditPayment));
            var code = await GetNextPurchaseInvoiceReturnCode(storeId, separateYears, documentDate, encoding.Prefix, encoding.Suffix, isOnTheWay, isDirectInvoice, separatingCashFromCredit, creditPayment);
            return new DocumentCodeDto() { NextCode = code, Prefix = encoding.Prefix, Suffix = encoding.Suffix };
        }

        public async Task<bool> UpdateAllPurchaseInvoiceReturnsBlockedFromPurchaseOrder(int purchaseOrderHeaderId, bool isBlocked)
        {
            var headerList = await (
                    from purchaseInvoiceReturn in _repository.GetAll()
                    from purchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceReturn.PurchaseInvoiceHeaderId && x.PurchaseOrderHeaderId == purchaseOrderHeaderId)
                    select purchaseInvoiceReturn
                ).ToListAsync();

            foreach (var header in headerList)
            {
                header.IsBlocked = isBlocked;
            }

            _repository.UpdateRange(headerList);
            await _repository.SaveChanges();
            return true;
        }

        public async Task<ResponseDto> SavePurchaseInvoiceReturnHeader(PurchaseInvoiceReturnHeaderDto purchaseInvoiceReturnHeader, bool hasApprove, bool approved, int? requestId, string? documentReference)
        {
            bool separatingCashFromCredit = await _applicationSettingService.SeparateCashFromCreditPurchaseInvoiceReturn(purchaseInvoiceReturnHeader.StoreId);
            bool separateYears = await _applicationSettingService.SeparateYears(purchaseInvoiceReturnHeader.StoreId);

			if (hasApprove)
            {
                if (purchaseInvoiceReturnHeader.PurchaseInvoiceReturnHeaderId == 0)
                {
                    return await CreatePurchaseInvoiceReturnHeader(purchaseInvoiceReturnHeader, hasApprove, approved, requestId, separatingCashFromCredit, separateYears, documentReference);
                }
                else
                {
                    return await UpdatePurchaseInvoiceReturnHeader(purchaseInvoiceReturnHeader);
                }
            }
            else
            {
                var purchaseInvoiceReturnHeaderExist = await IsPurchaseInvoiceReturnCodeExist(purchaseInvoiceReturnHeader.PurchaseInvoiceReturnHeaderId, purchaseInvoiceReturnHeader.DocumentCode, purchaseInvoiceReturnHeader.StoreId, separateYears, purchaseInvoiceReturnHeader.DocumentDate, purchaseInvoiceReturnHeader.Prefix, purchaseInvoiceReturnHeader.Suffix, purchaseInvoiceReturnHeader.IsOnTheWay, purchaseInvoiceReturnHeader.IsDirectInvoice, separatingCashFromCredit, purchaseInvoiceReturnHeader.CreditPayment);
                if (purchaseInvoiceReturnHeaderExist.Success)
                {
                    var nextPurchaseInvoiceReturnCode = await GetNextPurchaseInvoiceReturnCode(purchaseInvoiceReturnHeader.StoreId, separateYears, purchaseInvoiceReturnHeader.DocumentDate, purchaseInvoiceReturnHeader.Prefix, purchaseInvoiceReturnHeader.Suffix, purchaseInvoiceReturnHeader.IsOnTheWay, purchaseInvoiceReturnHeader.IsDirectInvoice, separatingCashFromCredit, purchaseInvoiceReturnHeader.CreditPayment);
                    return new ResponseDto() { Id = nextPurchaseInvoiceReturnCode, ResponseType = ResponseTypeData.Confirm, Success = false, Message = await _genericMessageService.GetMessage(PurchaseInvoiceReturnMenuCodeHelper.GetMenuCode(purchaseInvoiceReturnHeader), GenericMessageData.CodeAlreadyExist, $"{nextPurchaseInvoiceReturnCode}") };
                }
                else
                {
                    if (purchaseInvoiceReturnHeader.PurchaseInvoiceReturnHeaderId == 0)
                    {
                        return await CreatePurchaseInvoiceReturnHeader(purchaseInvoiceReturnHeader, hasApprove, approved, requestId, separatingCashFromCredit, separateYears, documentReference);
                    }
                    else
                    {
                        return await UpdatePurchaseInvoiceReturnHeader(purchaseInvoiceReturnHeader);
                    }
                }
            }
        }

        public async Task<ResponseDto> CreatePurchaseInvoiceReturnHeader(PurchaseInvoiceReturnHeaderDto purchaseInvoiceReturnHeader, bool hasApprove, bool approved, int? requestId, bool separatingCashFromCredit, bool separateYears, string? documentReference)
        {
            int purchaseInvoiceReturnCode;
            string? prefix;
            string? suffix;
            var nextPurchaseInvoiceReturnCode = await GetPurchaseInvoiceReturnCodeInternal(purchaseInvoiceReturnHeader.StoreId, separateYears, purchaseInvoiceReturnHeader.DocumentDate, purchaseInvoiceReturnHeader.IsOnTheWay, purchaseInvoiceReturnHeader.IsDirectInvoice, separatingCashFromCredit, purchaseInvoiceReturnHeader.CreditPayment);
            if (hasApprove && approved)
            {
                purchaseInvoiceReturnCode = nextPurchaseInvoiceReturnCode.NextCode;
                prefix = nextPurchaseInvoiceReturnCode.Prefix;
                suffix = nextPurchaseInvoiceReturnCode.Suffix;
            }
            else
            {
                purchaseInvoiceReturnCode = purchaseInvoiceReturnHeader.DocumentCode != 0 ? purchaseInvoiceReturnHeader.DocumentCode : nextPurchaseInvoiceReturnCode.NextCode;
                prefix = string.IsNullOrWhiteSpace(purchaseInvoiceReturnHeader.Prefix) ? nextPurchaseInvoiceReturnCode.Prefix : purchaseInvoiceReturnHeader.Prefix;
                suffix = string.IsNullOrWhiteSpace(purchaseInvoiceReturnHeader.Suffix) ? nextPurchaseInvoiceReturnCode.Suffix : purchaseInvoiceReturnHeader.Suffix;
            }

            var purchaseInvoiceReturnHeaderId = await GetNextId();
            var newPurchaseInvoiceReturnHeader = new PurchaseInvoiceReturnHeader()
            {
                PurchaseInvoiceReturnHeaderId = purchaseInvoiceReturnHeaderId,
                Prefix = prefix,
                DocumentCode = purchaseInvoiceReturnCode,
                Suffix = suffix,
                PurchaseInvoiceHeaderId = purchaseInvoiceReturnHeader.PurchaseInvoiceHeaderId,
                DocumentReference = documentReference != null ? documentReference : (hasApprove ? $"{DocumentReferenceData.Approval}{requestId}" : $"{PurchaseInvoiceReturnMenuCodeHelper.GetDocumentReference(purchaseInvoiceReturnHeader)}{purchaseInvoiceReturnHeaderId}"),
                SupplierId = purchaseInvoiceReturnHeader.SupplierId,
                StoreId = purchaseInvoiceReturnHeader.StoreId,
                DocumentDate = purchaseInvoiceReturnHeader.DocumentDate,
                EntryDate = DateHelper.GetDateTimeNow(),
                Reference = purchaseInvoiceReturnHeader.Reference,
                IsDirectInvoice = purchaseInvoiceReturnHeader.IsDirectInvoice,
                CreditPayment = purchaseInvoiceReturnHeader.CreditPayment,
                TaxTypeId = purchaseInvoiceReturnHeader.TaxTypeId,
                TotalValue = purchaseInvoiceReturnHeader.TotalValue,
                DiscountPercent = purchaseInvoiceReturnHeader.DiscountPercent,
                DiscountValue = purchaseInvoiceReturnHeader.DiscountValue,
                TotalItemDiscount = purchaseInvoiceReturnHeader.TotalItemDiscount,
                GrossValue = purchaseInvoiceReturnHeader.GrossValue,
                VatValue = purchaseInvoiceReturnHeader.VatValue,
                SubNetValue = purchaseInvoiceReturnHeader.SubNetValue,
                OtherTaxValue = purchaseInvoiceReturnHeader.OtherTaxValue,
                NetValueBeforeAdditionalDiscount = purchaseInvoiceReturnHeader.NetValueBeforeAdditionalDiscount,
                AdditionalDiscountValue = purchaseInvoiceReturnHeader.AdditionalDiscountValue,
                NetValue = purchaseInvoiceReturnHeader.NetValue,
                TotalCostValue = purchaseInvoiceReturnHeader.TotalCostValue,
                DebitAccountId = purchaseInvoiceReturnHeader.DebitAccountId,
                CreditAccountId = purchaseInvoiceReturnHeader.CreditAccountId,
                JournalHeaderId = purchaseInvoiceReturnHeader.JournalHeaderId,
                RemarksAr = purchaseInvoiceReturnHeader.RemarksAr,
                RemarksEn = purchaseInvoiceReturnHeader.RemarksEn,
                IsOnTheWay = purchaseInvoiceReturnHeader.IsOnTheWay,
                IsClosed = false,
                IsEnded = false,
                IsBlocked = false,
                ArchiveHeaderId = purchaseInvoiceReturnHeader.ArchiveHeaderId,
                MenuCode = (short)PurchaseInvoiceReturnMenuCodeHelper.GetMenuCode(purchaseInvoiceReturnHeader),


                CreatedAt = DateHelper.GetDateTimeNow(),
                UserNameCreated = await _httpContextAccessor!.GetUserName(),
                IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
                Hide = false,
            };

            var purchaseInvoiceReturnHeaderValidator = await new PurchaseInvoiceReturnHeaderValidator(_localizer).ValidateAsync(newPurchaseInvoiceReturnHeader);
            var validationResult = purchaseInvoiceReturnHeaderValidator.IsValid;
            if (validationResult)
            {
                await _repository.Insert(newPurchaseInvoiceReturnHeader);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = newPurchaseInvoiceReturnHeader.PurchaseInvoiceReturnHeaderId, Success = true, Message = await _genericMessageService.GetMessage(PurchaseInvoiceReturnMenuCodeHelper.GetMenuCode(purchaseInvoiceReturnHeader), GenericMessageData.CreatedSuccessWithCode, $"{newPurchaseInvoiceReturnHeader.Prefix}{newPurchaseInvoiceReturnHeader.DocumentCode}{newPurchaseInvoiceReturnHeader.Suffix}") };
            }
            else
            {
                return new ResponseDto() { Id = newPurchaseInvoiceReturnHeader.PurchaseInvoiceReturnHeaderId, Success = false, Message = purchaseInvoiceReturnHeaderValidator.ToString("~") };
            }
        }

        public async Task<ResponseDto> UpdatePurchaseInvoiceReturnHeader(PurchaseInvoiceReturnHeaderDto purchaseInvoiceReturnHeader)
        {
            var purchaseInvoiceReturnHeaderDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.PurchaseInvoiceReturnHeaderId == purchaseInvoiceReturnHeader.PurchaseInvoiceReturnHeaderId);
            if (purchaseInvoiceReturnHeaderDb != null)
            {
                purchaseInvoiceReturnHeaderDb.PurchaseInvoiceHeaderId = purchaseInvoiceReturnHeader.PurchaseInvoiceHeaderId;
                purchaseInvoiceReturnHeaderDb.SupplierId = purchaseInvoiceReturnHeader.SupplierId;
                purchaseInvoiceReturnHeaderDb.StoreId = purchaseInvoiceReturnHeader.StoreId;
                purchaseInvoiceReturnHeaderDb.DocumentDate = purchaseInvoiceReturnHeader.DocumentDate;
                purchaseInvoiceReturnHeaderDb.Reference = purchaseInvoiceReturnHeader.Reference;
                purchaseInvoiceReturnHeaderDb.IsDirectInvoice = purchaseInvoiceReturnHeader.IsDirectInvoice;
                purchaseInvoiceReturnHeaderDb.CreditPayment = purchaseInvoiceReturnHeader.CreditPayment;
                purchaseInvoiceReturnHeaderDb.TaxTypeId = purchaseInvoiceReturnHeader.TaxTypeId;
                purchaseInvoiceReturnHeaderDb.TotalValue = purchaseInvoiceReturnHeader.TotalValue;
                purchaseInvoiceReturnHeaderDb.DiscountPercent = purchaseInvoiceReturnHeader.DiscountPercent;
                purchaseInvoiceReturnHeaderDb.DiscountValue = purchaseInvoiceReturnHeader.DiscountValue;
                purchaseInvoiceReturnHeaderDb.TotalItemDiscount = purchaseInvoiceReturnHeader.TotalItemDiscount;
                purchaseInvoiceReturnHeaderDb.GrossValue = purchaseInvoiceReturnHeader.GrossValue;
                purchaseInvoiceReturnHeaderDb.VatValue = purchaseInvoiceReturnHeader.VatValue;
                purchaseInvoiceReturnHeaderDb.SubNetValue = purchaseInvoiceReturnHeader.SubNetValue;
                purchaseInvoiceReturnHeaderDb.OtherTaxValue = purchaseInvoiceReturnHeader.OtherTaxValue;
                purchaseInvoiceReturnHeaderDb.NetValueBeforeAdditionalDiscount = purchaseInvoiceReturnHeader.NetValueBeforeAdditionalDiscount;
                purchaseInvoiceReturnHeaderDb.AdditionalDiscountValue = purchaseInvoiceReturnHeader.AdditionalDiscountValue;
                purchaseInvoiceReturnHeaderDb.NetValue = purchaseInvoiceReturnHeader.NetValue;
                purchaseInvoiceReturnHeaderDb.TotalCostValue = purchaseInvoiceReturnHeader.TotalCostValue;
                purchaseInvoiceReturnHeaderDb.DebitAccountId = purchaseInvoiceReturnHeader.DebitAccountId;
                purchaseInvoiceReturnHeaderDb.CreditAccountId = purchaseInvoiceReturnHeader.CreditAccountId;
                purchaseInvoiceReturnHeaderDb.RemarksAr = purchaseInvoiceReturnHeader.RemarksAr;
                purchaseInvoiceReturnHeaderDb.RemarksEn = purchaseInvoiceReturnHeader.RemarksEn;
                purchaseInvoiceReturnHeaderDb.ArchiveHeaderId = purchaseInvoiceReturnHeader.ArchiveHeaderId;

                purchaseInvoiceReturnHeaderDb.ModifiedAt = DateHelper.GetDateTimeNow();
                purchaseInvoiceReturnHeaderDb.UserNameModified = await _httpContextAccessor!.GetUserName();
                purchaseInvoiceReturnHeaderDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();


                var purchaseInvoiceReturnHeaderValidator = await new PurchaseInvoiceReturnHeaderValidator(_localizer).ValidateAsync(purchaseInvoiceReturnHeaderDb);
                var validationResult = purchaseInvoiceReturnHeaderValidator.IsValid;
                if (validationResult)
                {
                    _repository.Update(purchaseInvoiceReturnHeaderDb);
                    await _repository.SaveChanges();
                    return new ResponseDto() { Id = purchaseInvoiceReturnHeaderDb.PurchaseInvoiceReturnHeaderId, Success = true, Message = await _genericMessageService.GetMessage(PurchaseInvoiceReturnMenuCodeHelper.GetMenuCode(purchaseInvoiceReturnHeader), GenericMessageData.UpdatedSuccessWithCode, $"{purchaseInvoiceReturnHeaderDb.Prefix}{purchaseInvoiceReturnHeaderDb.DocumentCode}{purchaseInvoiceReturnHeaderDb.Suffix}") };
                }
                else
                {
                    return new ResponseDto() { Id = 0, Success = false, Message = purchaseInvoiceReturnHeaderValidator.ToString("~") };
                }
            }
            return new ResponseDto() { Id = 0, Success = false, Message = await _genericMessageService.GetMessage(PurchaseInvoiceReturnMenuCodeHelper.GetMenuCode(purchaseInvoiceReturnHeader), GenericMessageData.NotFound) };
        }

        public async Task<ResponseDto> IsPurchaseInvoiceReturnCodeExist(int purchaseInvoiceReturnHeaderId, int purchaseInvoiceReturnCode, int storeId, bool separateYears, DateTime documentDate, string? prefix, string? suffix, bool isOnTheWay, bool isDirectInvoice, bool separateCashFromCredit, bool creditPayment)
        {
            PurchaseInvoiceReturnHeader? purchaseInvoiceReturnHeader;

            purchaseInvoiceReturnHeader = await _repository.GetAll()
                .Where(x => x.PurchaseInvoiceReturnHeaderId != purchaseInvoiceReturnHeaderId &&
                            (x.StoreId == storeId &&
							(!separateYears || x.DocumentDate.Year == documentDate.Year) &&
                            x.Prefix == prefix &&
                            x.DocumentCode == purchaseInvoiceReturnCode &&
                            x.Suffix == suffix &&
                            x.IsOnTheWay == isOnTheWay &&
                            x.IsDirectInvoice == isDirectInvoice &&
                            (!separateCashFromCredit || x.CreditPayment == creditPayment))).FirstOrDefaultAsync();

            if (purchaseInvoiceReturnHeader is not null)
            {
                return new ResponseDto() { Id = purchaseInvoiceReturnHeader.PurchaseInvoiceReturnHeaderId, Success = true };
            }
            return new ResponseDto() { Id = 0, Success = false };
        }

        public async Task<int> GetNextId()
        {
            int id = 1;
            try { id = await _repository.GetAll().MaxAsync(a => a.PurchaseInvoiceReturnHeaderId) + 1; } catch { id = 1; }
            return id;
        }

        public async Task<ResponseDto> DeletePurchaseInvoiceReturnHeader(int id, int menuCode)
        {
            var purchaseInvoiceReturnHeader = await _repository.GetAll().FirstOrDefaultAsync(x => x.PurchaseInvoiceReturnHeaderId == id);
            if (purchaseInvoiceReturnHeader != null)
            {
                _repository.Delete(purchaseInvoiceReturnHeader);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = id, Success = true, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.DeleteSuccessWithCode, $"{purchaseInvoiceReturnHeader.Prefix}{purchaseInvoiceReturnHeader.DocumentCode}{purchaseInvoiceReturnHeader.Suffix}") };
            }
            return new ResponseDto() { Id = id, Success = false, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.NotFound) };
        }

        public async Task<int> GetNextPurchaseInvoiceReturnCode(int storeId, bool separateYears, DateTime documentDate, string? prefix, string? suffix, bool isOnTheWay, bool isDirectInvoice, bool separateCashFromCredit, bool creditPayment)
        {
            int id = 1;
            try
            {
                id = await _repository.GetAll().AsNoTracking()
                    .Where(
                        x => (!separateYears || x.DocumentDate.Year == documentDate.Year) && 
                        x.Prefix == prefix && x.Suffix == suffix && 
                        x.StoreId == storeId && 
                        x.IsOnTheWay == isOnTheWay &&
                        x.IsDirectInvoice == isDirectInvoice && 
                        (!separateCashFromCredit || x.CreditPayment == creditPayment)).MaxAsync(a => a.DocumentCode) + 1;
            }
            catch { id = 1; }
            return id;
        }
    }
}
