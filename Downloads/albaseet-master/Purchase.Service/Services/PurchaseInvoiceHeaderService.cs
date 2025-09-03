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
using Shared.CoreOne.Models.Domain.Inventory;
using Shared.Helper.Logic;
using Shared.Service.Services.Menus;
using Shared.CoreOne.Contracts.Basics;
using Sales.CoreOne.Models.Domain;
using Microsoft.VisualBasic;

namespace Purchases.Service.Services
{
    public class PurchaseInvoiceHeaderService : BaseService<PurchaseInvoiceHeader>, IPurchaseInvoiceHeaderService
    {
        private readonly IGenericMessageService _genericMessageService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStoreService _storeService;
        private readonly ISupplierService _supplierService;
        private readonly IPurchaseOrderHeaderService _purchaseOrderHeaderService;
        private readonly IStringLocalizer<PurchaseInvoiceHeaderService> _localizer;
        private readonly IMenuEncodingService _menuEncodingService;
        private readonly IApplicationSettingService _applicationSettingService;
		private readonly IMenuService _menuService;
		private readonly IInvoiceTypeService _invoiceTypeService;

		public PurchaseInvoiceHeaderService(IGenericMessageService genericMessageService, IRepository<PurchaseInvoiceHeader> repository, IHttpContextAccessor httpContextAccessor, IStoreService storeService, IStringLocalizer<PurchaseInvoiceHeaderService> localizer, IMenuEncodingService menuEncodingService, ISupplierService supplierService, IPurchaseOrderHeaderService purchaseOrderHeaderService, IApplicationSettingService applicationSettingService, IMenuService menuService, IInvoiceTypeService invoiceTypeService) : base(repository)
        {
            _genericMessageService = genericMessageService;
            _httpContextAccessor = httpContextAccessor;
            _storeService = storeService;
            _supplierService = supplierService;
            _purchaseOrderHeaderService = purchaseOrderHeaderService;
            _localizer = localizer;
            _menuEncodingService = menuEncodingService;
            _applicationSettingService = applicationSettingService;
            _menuService = menuService;
            _invoiceTypeService = invoiceTypeService;
        }

        public IQueryable<PurchaseInvoiceHeaderDto> GetPurchaseInvoiceHeaders()
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();
            var data = from purchaseInvoiceHeader in _repository.GetAll()
                       from store in _storeService.GetAll().Where(x => x.StoreId == purchaseInvoiceHeader.StoreId)
                       from supplier in _supplierService.GetAll().Where(x => x.SupplierId == purchaseInvoiceHeader.SupplierId).DefaultIfEmpty()
                       from purchaseOrderHeader in _purchaseOrderHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseInvoiceHeader.PurchaseOrderHeaderId).DefaultIfEmpty()
					   from menu in _menuService.GetAll().Where(x => x.MenuCode == purchaseInvoiceHeader.MenuCode).DefaultIfEmpty()
					   from invoiceType in _invoiceTypeService.GetAll().Where(x => x.InvoiceTypeId == purchaseInvoiceHeader.InvoiceTypeId)
					   select new PurchaseInvoiceHeaderDto()
                       {
                           PurchaseInvoiceHeaderId = purchaseInvoiceHeader.PurchaseInvoiceHeaderId,
                           Prefix = purchaseInvoiceHeader.Prefix,
                           DocumentCode = purchaseInvoiceHeader.DocumentCode,
                           Suffix = purchaseInvoiceHeader.Suffix,
                           DocumentFullCode = $"{purchaseInvoiceHeader.Prefix}{purchaseInvoiceHeader.DocumentCode}{purchaseInvoiceHeader.Suffix}",
                           PurchaseOrderHeaderId = purchaseInvoiceHeader.PurchaseOrderHeaderId,
                           PurchaseOrderFullCode = purchaseOrderHeader != null ? $"{purchaseOrderHeader.Prefix}{purchaseOrderHeader.DocumentCode}{purchaseOrderHeader.Suffix}" : null,
                           PurchaseOrderDocumentReference = purchaseOrderHeader != null ? purchaseOrderHeader.DocumentReference : null,
                           SupplierQuotationHeaderId = purchaseOrderHeader.SupplierQuotationHeaderId,
                           DocumentReference = purchaseInvoiceHeader.DocumentReference,
                           SupplierId = purchaseInvoiceHeader.SupplierId,
                           SupplierCode = supplier.SupplierCode,
                           SupplierName = supplier != null ? language == LanguageCode.Arabic ? supplier.SupplierNameAr : supplier.SupplierNameEn : null,
                           StoreId = purchaseInvoiceHeader.StoreId,
                           StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
                           DocumentDate = purchaseInvoiceHeader.DocumentDate,
                           EntryDate = purchaseInvoiceHeader.EntryDate,
                           Reference = purchaseInvoiceHeader.Reference,
                           IsDirectInvoice = purchaseInvoiceHeader.IsDirectInvoice,
                           CreditPayment = purchaseInvoiceHeader.CreditPayment,
                           TaxTypeId = purchaseInvoiceHeader.TaxTypeId,
                           TotalValue = purchaseInvoiceHeader.TotalValue,
                           DiscountPercent = purchaseInvoiceHeader.DiscountPercent,
                           DiscountValue = purchaseInvoiceHeader.DiscountValue,
                           TotalItemDiscount = purchaseInvoiceHeader.TotalItemDiscount,
                           GrossValue = purchaseInvoiceHeader.GrossValue,
                           VatValue = purchaseInvoiceHeader.VatValue,
                           SubNetValue = purchaseInvoiceHeader.SubNetValue,
                           OtherTaxValue = purchaseInvoiceHeader.OtherTaxValue,
                           NetValueBeforeAdditionalDiscount = purchaseInvoiceHeader.NetValueBeforeAdditionalDiscount,
                           AdditionalDiscountValue = purchaseInvoiceHeader.AdditionalDiscountValue,
                           NetValue = purchaseInvoiceHeader.NetValue,
                           TotalInvoiceExpense = purchaseInvoiceHeader.TotalInvoiceExpense,
                           TotalCostValue = purchaseInvoiceHeader.TotalCostValue,
                           DebitAccountId = purchaseInvoiceHeader.DebitAccountId,
                           CreditAccountId = purchaseInvoiceHeader.CreditAccountId,
                           RemarksAr = purchaseInvoiceHeader.RemarksAr,
                           RemarksEn = purchaseInvoiceHeader.RemarksEn,
                           IsOnTheWay = purchaseInvoiceHeader.IsOnTheWay,
                           IsClosed = purchaseInvoiceHeader.IsClosed || purchaseInvoiceHeader.IsEnded || purchaseInvoiceHeader.IsBlocked || purchaseInvoiceHeader.HasSettlement,
                           IsEnded = purchaseInvoiceHeader.IsEnded,
                           IsBlocked = purchaseInvoiceHeader.IsBlocked,
                           HasSettlement = purchaseInvoiceHeader.HasSettlement,
                           IsSettlementCompleted = purchaseInvoiceHeader.IsSettlementCompleted,
                           JournalHeaderId = purchaseInvoiceHeader.JournalHeaderId,
						   MenuCode = purchaseInvoiceHeader.MenuCode,
						   MenuName = menu != null ? language == LanguageCode.Arabic ? menu.MenuNameAr : menu.MenuNameEn : null,
                           InvoiceTypeId = purchaseInvoiceHeader.InvoiceTypeId,
                           InvoiceTypeName = language == LanguageCode.Arabic ? invoiceType.InvoiceTypeNameAr : invoiceType.InvoiceTypeNameEn,
						   SupplierBalance = purchaseInvoiceHeader.SupplierBalance,
						   CreditLimitDays = purchaseInvoiceHeader.CreditLimitDays,
						   CreditLimitValues = purchaseInvoiceHeader.CreditLimitValues,
						   DebitLimitDays = purchaseInvoiceHeader.DebitLimitDays,
						   DueDate = purchaseInvoiceHeader.DueDate,
						   ArchiveHeaderId = purchaseInvoiceHeader.ArchiveHeaderId,

						   CreatedAt = purchaseInvoiceHeader.CreatedAt,
                           UserNameCreated = purchaseInvoiceHeader.UserNameCreated,
                           IpAddressCreated = purchaseInvoiceHeader.IpAddressCreated,

                           ModifiedAt = purchaseInvoiceHeader.ModifiedAt,
                           UserNameModified = purchaseInvoiceHeader.UserNameModified,
                           IpAddressModified = purchaseInvoiceHeader.IpAddressModified
                       };
            return data;
        }

        public IQueryable<PurchaseInvoiceHeaderDto> GetUserPurchaseInvoiceHeaders(int menuCode)
        {
            var userStore = _httpContextAccessor.GetCurrentUserStore();
            return menuCode switch {
                MenuCodeData.CashPurchaseInvoice => GetPurchaseInvoiceHeaders().Where(x => x.StoreId == userStore && (x.IsDirectInvoice && !x.IsOnTheWay && !x.CreditPayment)),
                MenuCodeData.CreditPurchaseInvoice => GetPurchaseInvoiceHeaders().Where(x => x.StoreId == userStore && (x.IsDirectInvoice && !x.IsOnTheWay && x.CreditPayment)),
                MenuCodeData.PurchaseInvoiceInterim => GetPurchaseInvoiceHeaders().Where(x => x.StoreId == userStore && (!x.IsDirectInvoice && !x.IsOnTheWay)),
                MenuCodeData.PurchaseInvoiceOnTheWayCash => GetPurchaseInvoiceHeaders().Where(x => x.StoreId == userStore && (x.IsDirectInvoice && x.IsOnTheWay && !x.CreditPayment)),
                MenuCodeData.PurchaseInvoiceOnTheWayCredit => GetPurchaseInvoiceHeaders().Where(x => x.StoreId == userStore && (x.IsDirectInvoice && x.IsOnTheWay && x.CreditPayment)),
                _ => GetPurchaseInvoiceHeaders().Where(x => x.StoreId == userStore)
			};
        }

        public IQueryable<PurchaseInvoiceHeaderDto> GetPurchaseInvoiceHeadersByStoreId(int storeId, int? supplierId, int purchaseInvoiceHeaderId, bool? isOnTheWay = null)
        {
            supplierId ??= 0;

            if (purchaseInvoiceHeaderId != 0)
            {
                return GetPurchaseInvoiceHeaders().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId);
            }
            else if (isOnTheWay != null)
            {
                return GetPurchaseInvoiceHeaders().Where(x => x.StoreId == storeId && (supplierId == 0 || x.SupplierId == supplierId) && x.IsEnded == false && x.IsBlocked == false && x.IsOnTheWay == isOnTheWay);
            }
            else
            {
                return GetPurchaseInvoiceHeaders().Where(x => x.StoreId == storeId && (supplierId == 0 || x.SupplierId == supplierId) && x.IsEnded == false && !x.IsBlocked);
            }
        }

        public async Task<PurchaseInvoiceHeaderDto?> GetPurchaseInvoiceHeaderById(int id)
        {
            return await GetPurchaseInvoiceHeaders().FirstOrDefaultAsync(x => x.PurchaseInvoiceHeaderId == id);
        }

        public async Task<bool> GetIsOnTheWay(int purchaseInvoiceHeaderId)
        {
            return await _repository.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId).Select(x => x.IsOnTheWay).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateAllPurchaseInvoicesBlockedFromPurchaseOrder(int purchaseOrderHeaderId, bool isBlocked)
        {
            var headerList = await _repository.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeaderId).ToListAsync();

            foreach (var header in headerList)
            {
                header.IsBlocked = isBlocked;
            }

            _repository.UpdateRange(headerList);
            await _repository.SaveChanges();
            return true;
        }

		public async Task UpdateHasSettlementFlag(List<int> purchaseInvoiceHeaderIds, bool hasSettlement)
		{
			var headerList = await _repository.GetAll().Where(x => purchaseInvoiceHeaderIds.Contains(x.PurchaseInvoiceHeaderId)).ToListAsync();

			foreach (var header in headerList)
			{
				header.HasSettlement = hasSettlement;
			}

			_repository.UpdateRange(headerList);
			await _repository.SaveChanges();
		}

		public async Task UpdateIsSettlementCompletedFlags(List<int> purchaseInvoiceHeaderIds, bool isSettlementCompleted)
		{
			var headerList = await _repository.GetAll().Where(x => purchaseInvoiceHeaderIds.Contains(x.PurchaseInvoiceHeaderId)).ToListAsync();

			foreach (var header in headerList)
			{
				header.IsSettlementCompleted = isSettlementCompleted;
			}

			_repository.UpdateRange(headerList);
			await _repository.SaveChanges();
		}

		public async Task<bool> UpdateClosed(int? purchaseInvoiceHeaderId, bool isClosed)
        {
            var header = await _repository.FindBy(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId).FirstOrDefaultAsync();
            if (header is null) return false;

            header.IsClosed = isClosed;
            _repository.Update(header);
            await _repository.SaveChanges();
            return true;
        }

        public async Task<bool> UpdateEnded(int? purchaseInvoiceHeaderId, bool isEnded)
        {
            var header = await _repository.FindBy(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId).FirstOrDefaultAsync();
            if (header is null) return false;

            header.IsEnded = isEnded;
            _repository.Update(header);
            await _repository.SaveChanges();
            return true;
        }

        public async Task<bool> UpdateEndedAndClosed(int? purchaseInvoiceHeaderId, bool isEnded, bool isClosed)
        {
            var header = await _repository.FindBy(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId).FirstOrDefaultAsync();
            if (header is null) return false;

            header.IsEnded = isEnded;
            header.IsClosed = isClosed;

            _repository.Update(header);
            await _repository.SaveChanges();
            return true;
        }

        public async Task<DocumentCodeDto> GetPurchaseInvoiceCode(int storeId, DateTime documentDate, bool isOnTheWay, bool isDirectInvoice, bool creditPayment)
        {
            bool separatingCashFromCredit = await _applicationSettingService.SeparateCashFromCreditPurchaseInvoice(storeId);
            bool separateYears = await _applicationSettingService.SeparateYears(storeId);
            return await GetPurchaseInvoiceCodeInternal(storeId, separateYears, documentDate, isOnTheWay, isDirectInvoice, separatingCashFromCredit, creditPayment);
        }

        //function used to avoid querying settings two times when create invoice
        private async Task<DocumentCodeDto> GetPurchaseInvoiceCodeInternal(int storeId, bool separateYears, DateTime documentDate, bool isOnTheWay, bool isDirectInvoice, bool separatingCashFromCredit, bool creditPayment)
        {
            var encoding = await _menuEncodingService.GetMenuEncoding(storeId, PurchaseInvoiceMenuCodeHelper.GetMenuCode(isOnTheWay, isDirectInvoice, creditPayment));
            var code = await GetNextPurchaseInvoiceCode(storeId, separateYears, documentDate, encoding.Prefix, encoding.Suffix, isOnTheWay, isDirectInvoice, separatingCashFromCredit, creditPayment);
            return new DocumentCodeDto() { NextCode = code, Prefix = encoding.Prefix, Suffix = encoding.Suffix };
        }

        public async Task<ResponseDto> SavePurchaseInvoiceHeader(PurchaseInvoiceHeaderDto purchaseInvoiceHeader, bool hasApprove, bool approved, int? requestId, string? documentReference)
        {
            bool separatingCashFromCredit = await _applicationSettingService.SeparateCashFromCreditPurchaseInvoice(purchaseInvoiceHeader.StoreId);
            bool separateYears = await _applicationSettingService.SeparateYears(purchaseInvoiceHeader.StoreId);

			if (hasApprove)
            {
                if (purchaseInvoiceHeader.PurchaseInvoiceHeaderId == 0)
                {
                    return await CreatePurchaseInvoiceHeader(purchaseInvoiceHeader, hasApprove, approved, requestId, separatingCashFromCredit, separateYears, documentReference);
                }
                else
                {
                    return await UpdatePurchaseInvoiceHeader(purchaseInvoiceHeader);
                }
            }
            else
            {
                var purchaseInvoiceHeaderExist = await IsPurchaseInvoiceCodeExist(purchaseInvoiceHeader.PurchaseInvoiceHeaderId, purchaseInvoiceHeader.DocumentCode, purchaseInvoiceHeader.StoreId, separateYears, purchaseInvoiceHeader.DocumentDate, purchaseInvoiceHeader.Prefix, purchaseInvoiceHeader.Suffix, purchaseInvoiceHeader.IsOnTheWay, purchaseInvoiceHeader.IsDirectInvoice, separatingCashFromCredit, purchaseInvoiceHeader.CreditPayment);
                if (purchaseInvoiceHeaderExist.Success)
                {
                    var nextPurchaseInvoiceCode = await GetNextPurchaseInvoiceCode(purchaseInvoiceHeader.StoreId, separateYears, purchaseInvoiceHeader.DocumentDate, purchaseInvoiceHeader.Prefix, purchaseInvoiceHeader.Suffix, purchaseInvoiceHeader.IsOnTheWay, purchaseInvoiceHeader.IsDirectInvoice, separatingCashFromCredit, purchaseInvoiceHeader.CreditPayment);
                    return new ResponseDto() { Id = nextPurchaseInvoiceCode, ResponseType = ResponseTypeData.Confirm, Success = false, Message = await _genericMessageService.GetMessage(PurchaseInvoiceMenuCodeHelper.GetMenuCode(purchaseInvoiceHeader), GenericMessageData.CodeAlreadyExist, $"{nextPurchaseInvoiceCode}") };
                }
                else
                {
                    if (purchaseInvoiceHeader.PurchaseInvoiceHeaderId == 0)
                    {
                        return await CreatePurchaseInvoiceHeader(purchaseInvoiceHeader, hasApprove, approved, requestId, separatingCashFromCredit, separateYears, documentReference);
                    }
                    else
                    {
                        return await UpdatePurchaseInvoiceHeader(purchaseInvoiceHeader);
                    }
                }
            }
        }

        public async Task<ResponseDto> CreatePurchaseInvoiceHeader(PurchaseInvoiceHeaderDto purchaseInvoiceHeader, bool hasApprove, bool approved, int? requestId, bool separatingCashFromCredit, bool separateYears, string? documentReference)
        {
            int purchaseInvoiceCode;
            string? prefix;
            string? suffix;
            var nextPurchaseInvoiceCode = await GetPurchaseInvoiceCodeInternal(purchaseInvoiceHeader.StoreId, separateYears, purchaseInvoiceHeader.DocumentDate, purchaseInvoiceHeader.IsOnTheWay, purchaseInvoiceHeader.IsDirectInvoice, separatingCashFromCredit, purchaseInvoiceHeader.CreditPayment);
            if (hasApprove && approved)
            {
                purchaseInvoiceCode = nextPurchaseInvoiceCode.NextCode;
                prefix = nextPurchaseInvoiceCode.Prefix;
                suffix = nextPurchaseInvoiceCode.Suffix;
            }
            else
            {
                purchaseInvoiceCode = purchaseInvoiceHeader.DocumentCode != 0 ? purchaseInvoiceHeader.DocumentCode : nextPurchaseInvoiceCode.NextCode;
                prefix = string.IsNullOrWhiteSpace(purchaseInvoiceHeader.Prefix) ? nextPurchaseInvoiceCode.Prefix : purchaseInvoiceHeader.Prefix;
                suffix = string.IsNullOrWhiteSpace(purchaseInvoiceHeader.Suffix) ? nextPurchaseInvoiceCode.Suffix : purchaseInvoiceHeader.Suffix;
            }

            var purchaseInvoiceHeaderId = await GetNextId();
            var newPurchaseInvoiceHeader = new PurchaseInvoiceHeader()
            {
                PurchaseInvoiceHeaderId = purchaseInvoiceHeaderId,
                Prefix = prefix,
                DocumentCode = purchaseInvoiceCode,
                Suffix = suffix,
                PurchaseOrderHeaderId = purchaseInvoiceHeader.PurchaseOrderHeaderId,
                DocumentReference = documentReference != null ? documentReference : (hasApprove ? $"{DocumentReferenceData.Approval}{requestId}" : $"{PurchaseInvoiceMenuCodeHelper.GetDocumentReference(purchaseInvoiceHeader)}{purchaseInvoiceHeaderId}"),
                SupplierId = purchaseInvoiceHeader.SupplierId,
                StoreId = purchaseInvoiceHeader.StoreId,
                DocumentDate = purchaseInvoiceHeader.DocumentDate,
                EntryDate = DateHelper.GetDateTimeNow(),
                Reference = purchaseInvoiceHeader.Reference,
                IsDirectInvoice = purchaseInvoiceHeader.IsDirectInvoice,
                CreditPayment = purchaseInvoiceHeader.CreditPayment,
                TaxTypeId = purchaseInvoiceHeader.TaxTypeId,
                TotalValue = purchaseInvoiceHeader.TotalValue,
                DiscountPercent = purchaseInvoiceHeader.DiscountPercent,
                DiscountValue = purchaseInvoiceHeader.DiscountValue,
                TotalItemDiscount = purchaseInvoiceHeader.TotalItemDiscount,
                GrossValue = purchaseInvoiceHeader.GrossValue,
                VatValue = purchaseInvoiceHeader.VatValue,
                SubNetValue = purchaseInvoiceHeader.SubNetValue,
                OtherTaxValue = purchaseInvoiceHeader.OtherTaxValue,
                NetValueBeforeAdditionalDiscount = purchaseInvoiceHeader.NetValueBeforeAdditionalDiscount,
                AdditionalDiscountValue = purchaseInvoiceHeader.AdditionalDiscountValue,
                NetValue = purchaseInvoiceHeader.NetValue,
                TotalInvoiceExpense = purchaseInvoiceHeader.TotalInvoiceExpense,
                TotalCostValue = purchaseInvoiceHeader.TotalCostValue,
                DebitAccountId = purchaseInvoiceHeader.DebitAccountId,
                CreditAccountId = purchaseInvoiceHeader.CreditAccountId,
                JournalHeaderId = purchaseInvoiceHeader.JournalHeaderId,
                RemarksAr = purchaseInvoiceHeader.RemarksAr,
                RemarksEn = purchaseInvoiceHeader.RemarksEn,
                IsOnTheWay = purchaseInvoiceHeader.IsOnTheWay,
                IsClosed = false,
                IsEnded = false,
                IsBlocked = false,
                HasSettlement = false,
                IsSettlementCompleted = false,
                MenuCode = (short)PurchaseInvoiceMenuCodeHelper.GetMenuCode(purchaseInvoiceHeader),
                InvoiceTypeId = purchaseInvoiceHeader.InvoiceTypeId,
				SupplierBalance = purchaseInvoiceHeader.SupplierBalance,
				CreditLimitDays = purchaseInvoiceHeader.CreditLimitDays,
				CreditLimitValues = purchaseInvoiceHeader.CreditLimitValues,
				DebitLimitDays = purchaseInvoiceHeader.DebitLimitDays,
				DueDate = purchaseInvoiceHeader.DueDate,
                ArchiveHeaderId = purchaseInvoiceHeader.ArchiveHeaderId,


				CreatedAt = DateHelper.GetDateTimeNow(),
                UserNameCreated = await _httpContextAccessor!.GetUserName(),
                IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
                Hide = false,
            };

            var purchaseInvoiceHeaderValidator = await new PurchaseInvoiceHeaderValidator(_localizer).ValidateAsync(newPurchaseInvoiceHeader);
            var validationResult = purchaseInvoiceHeaderValidator.IsValid;
            if (validationResult)
            {
                await _repository.Insert(newPurchaseInvoiceHeader);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = newPurchaseInvoiceHeader.PurchaseInvoiceHeaderId, Success = true, Message = await _genericMessageService.GetMessage(PurchaseInvoiceMenuCodeHelper.GetMenuCode(purchaseInvoiceHeader), GenericMessageData.CreatedSuccessWithCode, $"{newPurchaseInvoiceHeader.Prefix}{newPurchaseInvoiceHeader.DocumentCode}{newPurchaseInvoiceHeader.Suffix}") };
            }
            else
            {
                return new ResponseDto() { Id = newPurchaseInvoiceHeader.PurchaseInvoiceHeaderId, Success = false, Message = purchaseInvoiceHeaderValidator.ToString("~") };
            }
        }

        public async Task<ResponseDto> UpdatePurchaseInvoiceHeader(PurchaseInvoiceHeaderDto purchaseInvoiceHeader)
        {
            var purchaseInvoiceHeaderDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeader.PurchaseInvoiceHeaderId);
            if (purchaseInvoiceHeaderDb != null)
            {
                purchaseInvoiceHeaderDb.PurchaseOrderHeaderId = purchaseInvoiceHeader.PurchaseOrderHeaderId;
                purchaseInvoiceHeaderDb.SupplierId = purchaseInvoiceHeader.SupplierId;
                purchaseInvoiceHeaderDb.StoreId = purchaseInvoiceHeader.StoreId;
                purchaseInvoiceHeaderDb.DocumentDate = purchaseInvoiceHeader.DocumentDate;
                purchaseInvoiceHeaderDb.Reference = purchaseInvoiceHeader.Reference;
                purchaseInvoiceHeaderDb.IsDirectInvoice = purchaseInvoiceHeader.IsDirectInvoice;
                purchaseInvoiceHeaderDb.CreditPayment = purchaseInvoiceHeader.CreditPayment;
                purchaseInvoiceHeaderDb.TaxTypeId = purchaseInvoiceHeader.TaxTypeId;
                purchaseInvoiceHeaderDb.TotalValue = purchaseInvoiceHeader.TotalValue;
                purchaseInvoiceHeaderDb.DiscountPercent = purchaseInvoiceHeader.DiscountPercent;
                purchaseInvoiceHeaderDb.DiscountValue = purchaseInvoiceHeader.DiscountValue;
                purchaseInvoiceHeaderDb.TotalItemDiscount = purchaseInvoiceHeader.TotalItemDiscount;
                purchaseInvoiceHeaderDb.GrossValue = purchaseInvoiceHeader.GrossValue;
                purchaseInvoiceHeaderDb.VatValue = purchaseInvoiceHeader.VatValue;
                purchaseInvoiceHeaderDb.SubNetValue = purchaseInvoiceHeader.SubNetValue;
                purchaseInvoiceHeaderDb.OtherTaxValue = purchaseInvoiceHeader.OtherTaxValue;
                purchaseInvoiceHeaderDb.NetValueBeforeAdditionalDiscount = purchaseInvoiceHeader.NetValueBeforeAdditionalDiscount;
                purchaseInvoiceHeaderDb.AdditionalDiscountValue = purchaseInvoiceHeader.AdditionalDiscountValue;
                purchaseInvoiceHeaderDb.NetValue = purchaseInvoiceHeader.NetValue;
                purchaseInvoiceHeaderDb.TotalInvoiceExpense = purchaseInvoiceHeader.TotalInvoiceExpense;
                purchaseInvoiceHeaderDb.TotalCostValue = purchaseInvoiceHeader.TotalCostValue;
                purchaseInvoiceHeaderDb.DebitAccountId = purchaseInvoiceHeader.DebitAccountId;
                purchaseInvoiceHeaderDb.CreditAccountId = purchaseInvoiceHeader.CreditAccountId;
                purchaseInvoiceHeaderDb.RemarksAr = purchaseInvoiceHeader.RemarksAr;
                purchaseInvoiceHeaderDb.RemarksEn = purchaseInvoiceHeader.RemarksEn;
                purchaseInvoiceHeaderDb.InvoiceTypeId = purchaseInvoiceHeader.InvoiceTypeId;
                purchaseInvoiceHeaderDb.SupplierBalance = purchaseInvoiceHeader.SupplierBalance;
				purchaseInvoiceHeaderDb.CreditLimitDays = purchaseInvoiceHeader.CreditLimitDays;
				purchaseInvoiceHeaderDb.CreditLimitValues = purchaseInvoiceHeader.CreditLimitValues;
				purchaseInvoiceHeaderDb.DebitLimitDays = purchaseInvoiceHeader.DebitLimitDays;
				purchaseInvoiceHeaderDb.DueDate = purchaseInvoiceHeader.DueDate;
                purchaseInvoiceHeaderDb.ArchiveHeaderId = purchaseInvoiceHeader.ArchiveHeaderId;

                purchaseInvoiceHeaderDb.ModifiedAt = DateHelper.GetDateTimeNow();
                purchaseInvoiceHeaderDb.UserNameModified = await _httpContextAccessor!.GetUserName();
                purchaseInvoiceHeaderDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();


                var purchaseInvoiceHeaderValidator = await new PurchaseInvoiceHeaderValidator(_localizer).ValidateAsync(purchaseInvoiceHeaderDb);
                var validationResult = purchaseInvoiceHeaderValidator.IsValid;
                if (validationResult)
                {
                    _repository.Update(purchaseInvoiceHeaderDb);
                    await _repository.SaveChanges();
                    return new ResponseDto() { Id = purchaseInvoiceHeaderDb.PurchaseInvoiceHeaderId, Success = true, Message = await _genericMessageService.GetMessage(PurchaseInvoiceMenuCodeHelper.GetMenuCode(purchaseInvoiceHeader), GenericMessageData.UpdatedSuccessWithCode, $"{purchaseInvoiceHeaderDb.Prefix}{purchaseInvoiceHeaderDb.DocumentCode}{purchaseInvoiceHeaderDb.Suffix}") };
                }
                else
                {
                    return new ResponseDto() { Id = 0, Success = false, Message = purchaseInvoiceHeaderValidator.ToString("~") };
                }
            }
            return new ResponseDto() { Id = 0, Success = false, Message = await _genericMessageService.GetMessage(PurchaseInvoiceMenuCodeHelper.GetMenuCode(purchaseInvoiceHeader), GenericMessageData.NotFound) };
        }

        public async Task<ResponseDto> IsPurchaseInvoiceCodeExist(int purchaseInvoiceHeaderId, int purchaseInvoiceCode, int storeId, bool separateYears, DateTime documentDate, string? prefix, string? suffix, bool isOnTheWay, bool isDirectInvoice, bool separatingCashFromCredit, bool creditPayment)
        {
            PurchaseInvoiceHeader? purchaseInvoiceHeader;
            if (separatingCashFromCredit)
            {
                purchaseInvoiceHeader = await _repository.GetAll().FirstOrDefaultAsync(x => x.PurchaseInvoiceHeaderId != purchaseInvoiceHeaderId && (x.StoreId == storeId && (!separateYears || x.DocumentDate.Year == documentDate.Year) && x.Prefix == prefix && x.DocumentCode == purchaseInvoiceCode && x.Suffix == suffix && x.IsOnTheWay == isOnTheWay && x.IsDirectInvoice == isDirectInvoice && x.CreditPayment == creditPayment));
            }
            else
            {
                purchaseInvoiceHeader = await _repository.GetAll().FirstOrDefaultAsync(x => x.PurchaseInvoiceHeaderId != purchaseInvoiceHeaderId && (x.StoreId == storeId && (!separateYears || x.DocumentDate.Year == documentDate.Year) && x.Prefix == prefix && x.DocumentCode == purchaseInvoiceCode && x.Suffix == suffix && x.IsOnTheWay == isOnTheWay && x.IsDirectInvoice == isDirectInvoice));
            }

            if (purchaseInvoiceHeader is not null)
            {
                return new ResponseDto() { Id = purchaseInvoiceHeader.PurchaseInvoiceHeaderId, Success = true };
            }
            return new ResponseDto() { Id = 0, Success = false };
        }

        public async Task<int> GetNextId()
        {
            int id = 1;
            try { id = await _repository.GetAll().MaxAsync(a => a.PurchaseInvoiceHeaderId) + 1; } catch { id = 1; }
            return id;
        }

        public async Task<ResponseDto> DeletePurchaseInvoiceHeader(int id, int menuCode)
        {
            var purchaseInvoiceHeader = await _repository.GetAll().FirstOrDefaultAsync(x => x.PurchaseInvoiceHeaderId == id);
            if (purchaseInvoiceHeader != null)
            {
                _repository.Delete(purchaseInvoiceHeader);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = id, Success = true, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.DeleteSuccessWithCode, $"{purchaseInvoiceHeader.Prefix}{purchaseInvoiceHeader.DocumentCode}{purchaseInvoiceHeader.Suffix}") };
            }
            return new ResponseDto() { Id = id, Success = false, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.NotFound) };
        }

        public async Task<int> GetNextPurchaseInvoiceCode(int storeId, bool separateYears, DateTime documentDate, string? prefix, string? suffix, bool isOnTheWay, bool isDirectInvoice, bool separatingCashFromCredit, bool creditPayment)
        {
            int id = 1;
            try
            {
                if (separatingCashFromCredit)
                {
                    id = await _repository.GetAll().AsNoTracking().Where(x => (!separateYears || x.DocumentDate.Year == documentDate.Year) && x.Prefix == prefix && x.Suffix == suffix && x.StoreId == storeId && x.IsOnTheWay == isOnTheWay && x.IsDirectInvoice == isDirectInvoice && x.CreditPayment == creditPayment).MaxAsync(a => a.DocumentCode) + 1;
                }
                else
                {
                    id = await _repository.GetAll().AsNoTracking().Where(x => (!separateYears || x.DocumentDate.Year == documentDate.Year) && x.Prefix == prefix && x.Suffix == suffix && x.StoreId == storeId && x.IsOnTheWay == isOnTheWay && x.IsDirectInvoice == isDirectInvoice).MaxAsync(a => a.DocumentCode) + 1;
                }
            }
            catch { id = 1; }
            return id;
        }
    }
}
