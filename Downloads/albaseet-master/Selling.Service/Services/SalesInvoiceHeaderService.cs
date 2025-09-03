using Sales.CoreOne.Contracts;
using Shared.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sales.CoreOne.Models.Domain;
using Shared.CoreOne;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Sales.CoreOne.Models.Dtos.ViewModels;
using Sales.Service.Validators;
using Shared.CoreOne.Contracts.Basics;
using Shared.CoreOne.Contracts.Menus;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Models.Dtos;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Shared.Helper.Identity;
using Shared.CoreOne.Contracts.Settings;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.Helper.Logic;
using Sales.CoreOne.Models.StaticData;
using Purchases.CoreOne.Models.Domain;
using Shared.CoreOne.Models.Domain.Archive;

namespace Sales.Service.Services
{
    public class SalesInvoiceHeaderService: BaseService<SalesInvoiceHeader>, ISalesInvoiceHeaderService
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStoreService _storeService;
        private readonly IClientService _clientService;
        private readonly IStringLocalizer<SalesInvoiceHeaderService> _localizer;
        private readonly IMenuEncodingService _menuEncodingService;
        private readonly IShipmentTypeService _shipmentTypeService;
        private readonly IProformaInvoiceHeaderService _proformaInvoiceHeaderService;
        private readonly IApplicationSettingService _applicationSettingService;
        private readonly IGenericMessageService _genericMessageService;
		private readonly IMenuService _menuService;
        private readonly IInvoiceTypeService _invoiceTypeService;
		private readonly ISellerService _sellerService;

		public SalesInvoiceHeaderService(IRepository<SalesInvoiceHeader> repository, IHttpContextAccessor httpContextAccessor, IStoreService storeService, IStringLocalizer<SalesInvoiceHeaderService> localizer, IMenuEncodingService menuEncodingService, IClientService clientService, IShipmentTypeService shipmentTypeService, IProformaInvoiceHeaderService proformaInvoiceHeaderService, IApplicationSettingService applicationSettingService, IGenericMessageService genericMessageService, IMenuService menuService, IInvoiceTypeService invoiceTypeService, ISellerService sellerService) : base(repository)
        {
            _httpContextAccessor = httpContextAccessor;
            _storeService = storeService;
            _clientService = clientService;
            _localizer = localizer;
            _menuEncodingService = menuEncodingService;
            _shipmentTypeService = shipmentTypeService;
            _proformaInvoiceHeaderService = proformaInvoiceHeaderService;
            _applicationSettingService = applicationSettingService;
            _genericMessageService = genericMessageService;
            _menuService = menuService;
            _invoiceTypeService = invoiceTypeService;
			_sellerService = sellerService;
		}

        public IQueryable<SalesInvoiceHeaderDto> GetSalesInvoiceHeaders()
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();
            var data = from salesInvoiceHeader in _repository.GetAll()
                       from store in _storeService.GetAll().Where(x => x.StoreId == salesInvoiceHeader.StoreId)
                       from client in _clientService.GetAll().Where(x => x.ClientId == salesInvoiceHeader.ClientId)
                       from proformaInvoiceHeader in _proformaInvoiceHeaderService.GetAll().Where(x => x.ProformaInvoiceHeaderId == salesInvoiceHeader.ProformaInvoiceHeaderId)
                       from shipmentType in _shipmentTypeService.GetAll().Where(x => x.ShipmentTypeId == salesInvoiceHeader.ShipmentTypeId).DefaultIfEmpty()
                       from menu in _menuService.GetAll().Where(x => x.MenuCode == salesInvoiceHeader.MenuCode).DefaultIfEmpty()
                       from invoiceType in _invoiceTypeService.GetAll().Where(x => x.InvoiceTypeId == salesInvoiceHeader.InvoiceTypeId)
                       from seller in _sellerService.GetAll().Where(x => x.SellerId == salesInvoiceHeader.SellerId).DefaultIfEmpty()
					   select new SalesInvoiceHeaderDto()
                       {
                           SalesInvoiceHeaderId = salesInvoiceHeader.SalesInvoiceHeaderId,
                           Prefix = salesInvoiceHeader.Prefix,
                           DocumentCode = salesInvoiceHeader.DocumentCode,
                           Suffix = salesInvoiceHeader.Suffix,
                           DocumentFullCode = $"{salesInvoiceHeader.Prefix}{salesInvoiceHeader.DocumentCode}{salesInvoiceHeader.Suffix}",
                           ProformaInvoiceHeaderId = salesInvoiceHeader.ProformaInvoiceHeaderId,
                           ProformaInvoiceFullCode = $"{proformaInvoiceHeader.Prefix}{proformaInvoiceHeader.DocumentCode}{proformaInvoiceHeader.Suffix}",
                           ProformaInvoiceDocumentReference = proformaInvoiceHeader.DocumentReference,
                           ClientQuotationApprovalHeaderId = proformaInvoiceHeader.ClientQuotationApprovalHeaderId,
                           DocumentReference = salesInvoiceHeader.DocumentReference,
                           ClientId = salesInvoiceHeader.ClientId,
                           ClientCode = client.ClientCode,
                           ClientName = language == LanguageCode.Arabic ? client.ClientNameAr : client.ClientNameEn,
                           SellerId = seller != null ? seller.SellerId : null,
                           SellerCode = seller != null ? seller.SellerCode : null,
                           SellerName = seller != null ? language == LanguageCode.Arabic ? seller.SellerNameAr : seller.SellerNameEn : null,
                           StoreId = salesInvoiceHeader.StoreId,
                           StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
                           DocumentDate = salesInvoiceHeader.DocumentDate,
                           EntryDate = salesInvoiceHeader.EntryDate,
                           Reference = salesInvoiceHeader.Reference,
                           IsDirectInvoice = salesInvoiceHeader.IsDirectInvoice,
                           CreditPayment = salesInvoiceHeader.CreditPayment,
                           TaxTypeId = salesInvoiceHeader.TaxTypeId,
                           ShippingDate = salesInvoiceHeader.ShippingDate,
                           DeliveryDate = salesInvoiceHeader.DeliveryDate,
                           DueDate = salesInvoiceHeader.DueDate,
                           ShipmentTypeId = salesInvoiceHeader.ShipmentTypeId,
                           ShipmentTypeName = shipmentType != null ? language == LanguageCode.Arabic ? shipmentType.ShipmentTypeNameAr : shipmentType.ShipmentTypeNameEn : null,
                           CashClientName = salesInvoiceHeader.ClientName,
                           ClientPhone = salesInvoiceHeader.ClientPhone,
                           ClientAddress = salesInvoiceHeader.ClientAddress,
                           ClientTaxCode = salesInvoiceHeader.ClientTaxCode,
                           DriverName = salesInvoiceHeader.DriverName,
                           DriverPhone = salesInvoiceHeader.DriverPhone,
                           ClientResponsibleName = salesInvoiceHeader.ClientResponsibleName,
                           ClientResponsiblePhone = salesInvoiceHeader.ClientResponsiblePhone,
                           ShipTo = salesInvoiceHeader.ShipTo,
                           BillTo = salesInvoiceHeader.BillTo,
                           ShippingRemarks = salesInvoiceHeader.ShippingRemarks,
                           TotalValue = salesInvoiceHeader.TotalValue,
                           DiscountPercent = salesInvoiceHeader.DiscountPercent,
                           DiscountValue = salesInvoiceHeader.DiscountValue,
                           TotalItemDiscount = salesInvoiceHeader.TotalItemDiscount,
                           GrossValue = salesInvoiceHeader.GrossValue,
                           VatValue = salesInvoiceHeader.VatValue,
                           SubNetValue = salesInvoiceHeader.SubNetValue,
                           OtherTaxValue = salesInvoiceHeader.OtherTaxValue,
                           NetValueBeforeAdditionalDiscount = salesInvoiceHeader.NetValueBeforeAdditionalDiscount,
                           AdditionalDiscountValue = salesInvoiceHeader.AdditionalDiscountValue,
                           NetValue = salesInvoiceHeader.NetValue,
                           TotalCostValue = salesInvoiceHeader.TotalCostValue,
                           DebitAccountId = salesInvoiceHeader.DebitAccountId,
                           CreditAccountId = salesInvoiceHeader.CreditAccountId,
                           JournalHeaderId = salesInvoiceHeader.JournalHeaderId,
                           RemarksAr = salesInvoiceHeader.RemarksAr,
                           RemarksEn = salesInvoiceHeader.RemarksEn,
                           CanReturnInDays = salesInvoiceHeader.CanReturnInDays,
                           CanReturnUntilDate = salesInvoiceHeader.CanReturnUntilDate,
                           IsOnTheWay = salesInvoiceHeader.IsOnTheWay,
                           IsClosed = salesInvoiceHeader.IsClosed || salesInvoiceHeader.IsEnded || salesInvoiceHeader.IsBlocked || salesInvoiceHeader.HasSettlement,
                           IsEnded = salesInvoiceHeader.IsEnded,
                           IsBlocked = salesInvoiceHeader.IsBlocked,
                           HasSettlement = salesInvoiceHeader.HasSettlement,
                           IsSettlementCompleted = salesInvoiceHeader.IsSettlementCompleted,
						   MenuCode = salesInvoiceHeader.MenuCode,
						   MenuName = menu != null ? language == LanguageCode.Arabic ? menu.MenuNameAr : menu.MenuNameEn : null,
						   InvoiceTypeId = salesInvoiceHeader.InvoiceTypeId,
						   InvoiceTypeName = language == LanguageCode.Arabic ? invoiceType.InvoiceTypeNameAr : invoiceType.InvoiceTypeNameEn,
						   ClientBalance = salesInvoiceHeader.ClientBalance,
						   CreditLimitDays = salesInvoiceHeader.CreditLimitDays,
						   CreditLimitValues = salesInvoiceHeader.CreditLimitValues,
						   DebitLimitDays = salesInvoiceHeader.DebitLimitDays,
                           ArchiveHeaderId = salesInvoiceHeader.ArchiveHeaderId,

						   CreatedAt = salesInvoiceHeader.CreatedAt,
                           UserNameCreated = salesInvoiceHeader.UserNameCreated,
                           IpAddressCreated = salesInvoiceHeader.IpAddressCreated,

                           ModifiedAt = salesInvoiceHeader.ModifiedAt,
                           UserNameModified = salesInvoiceHeader.UserNameModified,
                           IpAddressModified = salesInvoiceHeader.IpAddressModified
                       };
            return data;
        }

        public IQueryable<SalesInvoiceHeaderDto> GetUserSalesInvoiceHeaders(int menuCode)
        {
            return menuCode switch
            {
                MenuCodeData.CashSalesInvoice => GetUserSalesInvoiceHeadersInternal(false, true, false, false),
                MenuCodeData.CreditSalesInvoice => GetUserSalesInvoiceHeadersInternal(false, true, true, false),
                MenuCodeData.SalesInvoiceInterim => GetUserSalesInvoiceHeadersInternal(false, false, true, false),
                MenuCodeData.CashReservationInvoice => GetUserSalesInvoiceHeadersInternal(true, true, false, false),
                MenuCodeData.CreditReservationInvoice => GetUserSalesInvoiceHeadersInternal(true, true, true, false),
                _ => GetUserSalesInvoiceHeadersInternal(false, false, false, true)
            };
        }

		private IQueryable<SalesInvoiceHeaderDto> GetUserSalesInvoiceHeadersInternal(bool isOnTheWay, bool isDirectInvoice, bool isCreditPayment, bool getAll)
		{
			var userStore = _httpContextAccessor.GetCurrentUserStore();
			return GetSalesInvoiceHeaders().Where(x => x.StoreId == userStore && (getAll || x.IsDirectInvoice == isDirectInvoice && x.IsOnTheWay == isOnTheWay && x.CreditPayment == isCreditPayment));
		}

		public IQueryable<SalesInvoiceHeaderDto> GetSalesInvoiceHeadersByStoreId(int storeId, int? clientId, int salesInvoiceHeaderId, bool? isOnTheWay = null)
        {
            clientId ??= 0;
            if (salesInvoiceHeaderId == 0)
            {
                return GetSalesInvoiceHeaders().Where(x => x.StoreId == storeId && (clientId == 0 || x.ClientId == clientId) && x.IsEnded == false && x.IsBlocked == false && (isOnTheWay == null || x.IsOnTheWay == isOnTheWay));
            }
            else
            {
                return GetSalesInvoiceHeaders().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId);
            }
        }

        public async Task<SalesInvoiceHeaderDto?> GetSalesInvoiceHeaderById(int id)
        {
            return await GetSalesInvoiceHeaders().FirstOrDefaultAsync(x => x.SalesInvoiceHeaderId == id);
        }

        public async Task<DocumentCodeDto> GetSalesInvoiceCode(int storeId, DateTime documentDate, bool isOnTheWay, bool isDirectInvoice, bool creditPayment, int? sellerId)
        {
            var separateCashFromCredit = await _applicationSettingService.SeparateCashFromCreditSalesInvoice(storeId);
            var separatingTheSequenceOfInvoicesForEachSeller = await _applicationSettingService.SeparateSellers(storeId);
            var separateYears = await _applicationSettingService.SeparateYears(storeId);
            return await GetSalesInvoiceCodeInternal(storeId, separateYears, documentDate, isOnTheWay, isDirectInvoice, separateCashFromCredit, creditPayment, separatingTheSequenceOfInvoicesForEachSeller, sellerId);
        }

        //function used to avoid querying settings two times when create invoice
        private async Task<DocumentCodeDto> GetSalesInvoiceCodeInternal(int storeId, bool separateYears, DateTime documentDate, bool isOnTheWay, bool isDirectInvoice, bool separateCashFromCredit, bool creditPayment, bool separatingTheSequenceOfInvoicesForEachSeller, int? sellerId)
        {
            var encoding = await _menuEncodingService.GetMenuEncoding(storeId, SalesInvoiceMenuCodeHelper.GetMenuCode(isOnTheWay, isDirectInvoice, creditPayment));
            var code = await GetNextSalesInvoiceCode(storeId, separateYears, documentDate, encoding.Prefix, encoding.Suffix, isOnTheWay, isDirectInvoice, separateCashFromCredit, creditPayment, separatingTheSequenceOfInvoicesForEachSeller, sellerId);
            return new DocumentCodeDto() { NextCode = code, Prefix = encoding.Prefix, Suffix = encoding.Suffix };
        }

        public async Task<bool> UpdateBlocked(int? salesInvoiceHeaderId, bool isBlocked)
        {
            var header = await _repository.FindBy(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId).FirstOrDefaultAsync();
            if (header is null) return false;

            header.IsBlocked = isBlocked;
            _repository.Update(header);
            await _repository.SaveChanges();
            return true;
        }

        public async Task UpdateHasSettlementFlag(List<int> salesInvoiceHeaderIds, bool hasSettlement)
        {
			var headerList = await _repository.GetAll().Where(x => salesInvoiceHeaderIds.Contains(x.SalesInvoiceHeaderId)).ToListAsync();

			foreach (var header in headerList)
			{
				header.HasSettlement = hasSettlement;
			}

            _repository.UpdateRange(headerList);
			await _repository.SaveChanges();
		}

		public async Task UpdateIsSettlementCompletedFlags(List<int> salesInvoiceHeaderIds, bool isSettlementCompleted)
		{
			var headerList = await _repository.GetAll().Where(x => salesInvoiceHeaderIds.Contains(x.SalesInvoiceHeaderId)).ToListAsync();

			foreach (var header in headerList)
			{
				header.IsSettlementCompleted = isSettlementCompleted;
			}

			_repository.UpdateRange(headerList);
			await _repository.SaveChanges();
		}

		public async Task<bool> UpdateClosed(int? salesInvoiceHeaderId, bool isClosed)
        {
            var header = await _repository.FindBy(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId).FirstOrDefaultAsync();
            if (header is null) return false;

            header.IsClosed = isClosed;
            _repository.Update(header);
            await _repository.SaveChanges();
            return true;
        }

        public async Task<bool> UpdateEnded(int? salesInvoiceHeaderId, bool isEnded)
        {
            var header = await _repository.FindBy(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId).FirstOrDefaultAsync();
            if (header is null) return false;

            header.IsEnded = isEnded;
            _repository.Update(header);
            await _repository.SaveChanges();
            return true;
        }

        public async Task<bool> UpdateEndedAndClosed(int? salesInvoiceHeaderId, bool isEnded, bool isClosed)
        {
            var header = await _repository.FindBy(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId).FirstOrDefaultAsync();
            if (header is null) return false;

            header.IsEnded = isEnded;
            header.IsClosed = isClosed;
            _repository.Update(header);
            await _repository.SaveChanges();
            return true;
        }

		public async Task UpdateAllSalesInvoicesBlockedFromProformaInvoice(int proformaInvoiceHeaderId, bool isBlocked)
		{
			var headerList = await _repository.GetAll().Where(x => x.ProformaInvoiceHeaderId == proformaInvoiceHeaderId).ToListAsync();

			foreach (var header in headerList)
			{
				header.IsBlocked = isBlocked;
			}

            _repository.UpdateRange(headerList);
			await _repository.SaveChanges();
		}

		public async Task<ResponseDto> SaveSalesInvoiceHeader(SalesInvoiceHeaderDto salesInvoiceHeader, bool hasApprove, bool approved, int? requestId, string? documentReference)
        {
            var separateCashFromCredit = await _applicationSettingService.SeparateCashFromCreditSalesInvoice(salesInvoiceHeader.StoreId);
            var separatingTheSequenceOfInvoicesForEachSeller = await _applicationSettingService.SeparateSellers(salesInvoiceHeader.StoreId);
            var separateYears = await _applicationSettingService.SeparateYears(salesInvoiceHeader.StoreId);

			if (hasApprove)
            {
                if (salesInvoiceHeader.SalesInvoiceHeaderId == 0)
                {
                    return await CreateSalesInvoiceHeader(salesInvoiceHeader, hasApprove, approved, requestId, separateCashFromCredit, separatingTheSequenceOfInvoicesForEachSeller, separateYears, documentReference);
                }
                else
                {
                    return await UpdateSalesInvoiceHeader(salesInvoiceHeader);
                }
            }
            else
            {
                var salesInvoiceHeaderExist = await IsSalesInvoiceCodeExist(salesInvoiceHeader.SalesInvoiceHeaderId, salesInvoiceHeader.DocumentCode, salesInvoiceHeader.StoreId, separateYears, salesInvoiceHeader.DocumentDate, salesInvoiceHeader.Prefix, salesInvoiceHeader.Suffix, salesInvoiceHeader.IsOnTheWay, salesInvoiceHeader.IsDirectInvoice, separateCashFromCredit, salesInvoiceHeader.CreditPayment, separatingTheSequenceOfInvoicesForEachSeller, salesInvoiceHeader.SellerId);
                if (salesInvoiceHeaderExist.Success)
                {
                    var nextSalesInvoiceCode = await GetNextSalesInvoiceCode(salesInvoiceHeader.StoreId, separateYears, salesInvoiceHeader.DocumentDate, salesInvoiceHeader.Prefix, salesInvoiceHeader.Suffix, salesInvoiceHeader.IsOnTheWay, salesInvoiceHeader.IsDirectInvoice, separateCashFromCredit, salesInvoiceHeader.CreditPayment, separatingTheSequenceOfInvoicesForEachSeller, salesInvoiceHeader.SellerId);
                    return new ResponseDto() { Id = nextSalesInvoiceCode, ResponseType = ResponseTypeData.Confirm, Success = false, Message = await _genericMessageService.GetMessage(SalesInvoiceMenuCodeHelper.GetMenuCode(salesInvoiceHeader), GenericMessageData.CodeAlreadyExist, $"{nextSalesInvoiceCode}") };
                }
                else
                {
                    if (salesInvoiceHeader.SalesInvoiceHeaderId == 0)
                    {
                        return await CreateSalesInvoiceHeader(salesInvoiceHeader, hasApprove, approved, requestId, separateCashFromCredit, separatingTheSequenceOfInvoicesForEachSeller, separateYears ,documentReference);
                    }
                    else
                    {
                        return await UpdateSalesInvoiceHeader(salesInvoiceHeader);
                    }
                }
            }
        }

        public async Task<ResponseDto> CreateSalesInvoiceHeader(SalesInvoiceHeaderDto salesInvoiceHeader, bool hasApprove, bool approved, int? requestId, bool separateCashFromCredit, bool separatingTheSequenceOfInvoicesForEachSeller, bool separateYears, string? documentReference)
        {
            int salesInvoiceCode;
            string? prefix;
            string? suffix;
            var nextSalesInvoiceCode = await GetSalesInvoiceCodeInternal(salesInvoiceHeader.StoreId, separateYears, salesInvoiceHeader.DocumentDate, salesInvoiceHeader.IsOnTheWay, salesInvoiceHeader.IsDirectInvoice, separateCashFromCredit, salesInvoiceHeader.CreditPayment, separatingTheSequenceOfInvoicesForEachSeller, salesInvoiceHeader.SellerId);
            if (hasApprove && approved)
            {
                salesInvoiceCode = nextSalesInvoiceCode.NextCode;
                prefix = nextSalesInvoiceCode.Prefix;
                suffix = nextSalesInvoiceCode.Suffix;
            }
            else
            {
                salesInvoiceCode = salesInvoiceHeader.DocumentCode != 0 ? salesInvoiceHeader.DocumentCode : nextSalesInvoiceCode.NextCode;
                prefix = string.IsNullOrWhiteSpace(salesInvoiceHeader.Prefix) ? nextSalesInvoiceCode.Prefix : salesInvoiceHeader.Prefix;
                suffix = string.IsNullOrWhiteSpace(salesInvoiceHeader.Suffix) ? nextSalesInvoiceCode.Suffix : salesInvoiceHeader.Suffix;
            }

            var salesInvoiceHeaderId = await GetNextId();
            var newSalesInvoiceHeader = new SalesInvoiceHeader()
            {
                SalesInvoiceHeaderId = salesInvoiceHeaderId,
                Prefix = prefix,
                DocumentCode = salesInvoiceCode,
                Suffix = suffix,
                ProformaInvoiceHeaderId = salesInvoiceHeader.ProformaInvoiceHeaderId,
                DocumentReference = documentReference != null ? documentReference : ( hasApprove ? $"{DocumentReferenceData.Approval}{requestId}" : $"{SalesInvoiceMenuCodeHelper.GetDocumentReference(salesInvoiceHeader)}{salesInvoiceHeaderId}" ),
                ClientId = salesInvoiceHeader.ClientId,
                SellerId = salesInvoiceHeader.SellerId,
                StoreId = salesInvoiceHeader.StoreId,
                DocumentDate = salesInvoiceHeader.DocumentDate,
                EntryDate = DateHelper.GetDateTimeNow(),
                Reference = salesInvoiceHeader.Reference,
                IsDirectInvoice = salesInvoiceHeader.IsDirectInvoice,
                CreditPayment = salesInvoiceHeader.CreditPayment,
                TaxTypeId = salesInvoiceHeader.TaxTypeId,
                ShippingDate = salesInvoiceHeader.ShippingDate,
                DeliveryDate = salesInvoiceHeader.DeliveryDate,
                DueDate = salesInvoiceHeader.DueDate,
                ShipmentTypeId = salesInvoiceHeader.ShipmentTypeId,
                ClientName = salesInvoiceHeader.CashClientName,
                ClientPhone = salesInvoiceHeader.ClientPhone,
                ClientAddress = salesInvoiceHeader.ClientAddress,
                ClientTaxCode = salesInvoiceHeader.ClientTaxCode,
                DriverName = salesInvoiceHeader.DriverName,
                DriverPhone = salesInvoiceHeader.DriverPhone,
                ClientResponsibleName = salesInvoiceHeader.ClientResponsibleName,
                ClientResponsiblePhone = salesInvoiceHeader.ClientResponsiblePhone,
                ShipTo = salesInvoiceHeader.ShipTo,
                BillTo = salesInvoiceHeader.BillTo,
                ShippingRemarks = salesInvoiceHeader.ShippingRemarks,
                TotalValue = salesInvoiceHeader.TotalValue,
                DiscountPercent = salesInvoiceHeader.DiscountPercent,
                DiscountValue = salesInvoiceHeader.DiscountValue,
                TotalItemDiscount = salesInvoiceHeader.TotalItemDiscount,
                GrossValue = salesInvoiceHeader.GrossValue,
                VatValue = salesInvoiceHeader.VatValue,
                SubNetValue = salesInvoiceHeader.SubNetValue,
                OtherTaxValue = salesInvoiceHeader.OtherTaxValue,
                NetValueBeforeAdditionalDiscount = salesInvoiceHeader.NetValueBeforeAdditionalDiscount,
                AdditionalDiscountValue = salesInvoiceHeader.AdditionalDiscountValue,
                NetValue = salesInvoiceHeader.NetValue,
                TotalCostValue = salesInvoiceHeader.TotalCostValue,
                DebitAccountId = salesInvoiceHeader.DebitAccountId,
                CreditAccountId = salesInvoiceHeader.CreditAccountId,
                JournalHeaderId = salesInvoiceHeader.JournalHeaderId,
                RemarksAr = salesInvoiceHeader.RemarksAr,
                RemarksEn = salesInvoiceHeader.RemarksEn,
                CanReturnInDays = salesInvoiceHeader.CanReturnInDays,
                CanReturnUntilDate = salesInvoiceHeader.CanReturnUntilDate,
                IsOnTheWay = salesInvoiceHeader.IsOnTheWay,
                IsClosed = false,
                IsEnded = false,
                IsBlocked = false,
                HasSettlement = false,
                IsSettlementCompleted = false,
                MenuCode = (short)SalesInvoiceMenuCodeHelper.GetMenuCode(salesInvoiceHeader),
                InvoiceTypeId = salesInvoiceHeader.InvoiceTypeId,
				ClientBalance = salesInvoiceHeader.ClientBalance,
				CreditLimitDays = salesInvoiceHeader.CreditLimitDays,
				CreditLimitValues = salesInvoiceHeader.CreditLimitValues,
				DebitLimitDays = salesInvoiceHeader.DebitLimitDays,
				ArchiveHeaderId = salesInvoiceHeader.ArchiveHeaderId,


				CreatedAt = DateHelper.GetDateTimeNow(),
                UserNameCreated = await _httpContextAccessor!.GetUserName(),
                IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
                Hide = false,
            };

            var salesInvoiceHeaderValidator = await new SalesInvoiceHeaderValidator(_localizer).ValidateAsync(newSalesInvoiceHeader);
            var validationResult = salesInvoiceHeaderValidator.IsValid;
            if (validationResult)
            {
                await _repository.Insert(newSalesInvoiceHeader);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = newSalesInvoiceHeader.SalesInvoiceHeaderId, Success = true, Message = await _genericMessageService.GetMessage(SalesInvoiceMenuCodeHelper.GetMenuCode(salesInvoiceHeader), GenericMessageData.CreatedSuccessWithCode, $"{newSalesInvoiceHeader.Prefix}{newSalesInvoiceHeader.DocumentCode}{newSalesInvoiceHeader.Suffix}") };
            }
            else
            {
                return new ResponseDto() { Id = newSalesInvoiceHeader.SalesInvoiceHeaderId, Success = false, Message = salesInvoiceHeaderValidator.ToString("~") };
            }
        }

        public async Task<ResponseDto> UpdateSalesInvoiceHeader(SalesInvoiceHeaderDto salesInvoiceHeader)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var salesInvoiceHeaderDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.SalesInvoiceHeaderId == salesInvoiceHeader.SalesInvoiceHeaderId);
            if (salesInvoiceHeaderDb != null)
            {
                salesInvoiceHeaderDb.ProformaInvoiceHeaderId = salesInvoiceHeader.ProformaInvoiceHeaderId;
                salesInvoiceHeaderDb.ClientId = salesInvoiceHeader.ClientId;
                salesInvoiceHeaderDb.SellerId = salesInvoiceHeader.SellerId;
                salesInvoiceHeaderDb.StoreId = salesInvoiceHeader.StoreId;
                salesInvoiceHeaderDb.DocumentDate = salesInvoiceHeader.DocumentDate;
                salesInvoiceHeaderDb.Reference = salesInvoiceHeader.Reference;
                salesInvoiceHeaderDb.CreditPayment = salesInvoiceHeader.CreditPayment;
                salesInvoiceHeaderDb.TaxTypeId = salesInvoiceHeader.TaxTypeId;
                salesInvoiceHeaderDb.ShippingDate = salesInvoiceHeader.ShippingDate;
                salesInvoiceHeaderDb.DeliveryDate = salesInvoiceHeader.DeliveryDate;
                salesInvoiceHeaderDb.DueDate = salesInvoiceHeader.DueDate;
                salesInvoiceHeaderDb.ShipmentTypeId = salesInvoiceHeader.ShipmentTypeId;
                salesInvoiceHeaderDb.ClientName = salesInvoiceHeader.CashClientName;
                salesInvoiceHeaderDb.ClientPhone = salesInvoiceHeader.ClientPhone;
                salesInvoiceHeaderDb.ClientAddress = salesInvoiceHeader.ClientAddress;
                salesInvoiceHeaderDb.ClientTaxCode = salesInvoiceHeader.ClientTaxCode;
                salesInvoiceHeaderDb.DriverName = salesInvoiceHeader.DriverName;
                salesInvoiceHeaderDb.DriverPhone = salesInvoiceHeader.DriverPhone;
                salesInvoiceHeaderDb.ClientResponsibleName = salesInvoiceHeader.ClientResponsibleName;
                salesInvoiceHeaderDb.ClientResponsiblePhone = salesInvoiceHeader.ClientResponsiblePhone;
                salesInvoiceHeaderDb.ShipTo = salesInvoiceHeader.ShipTo;
                salesInvoiceHeaderDb.BillTo = salesInvoiceHeader.BillTo;
                salesInvoiceHeaderDb.ShippingRemarks = salesInvoiceHeader.ShippingRemarks;
                salesInvoiceHeaderDb.TotalValue = salesInvoiceHeader.TotalValue;
                salesInvoiceHeaderDb.DiscountPercent = salesInvoiceHeader.DiscountPercent;
                salesInvoiceHeaderDb.DiscountValue = salesInvoiceHeader.DiscountValue;
                salesInvoiceHeaderDb.TotalItemDiscount = salesInvoiceHeader.TotalItemDiscount;
                salesInvoiceHeaderDb.GrossValue = salesInvoiceHeader.GrossValue;
                salesInvoiceHeaderDb.VatValue = salesInvoiceHeader.VatValue;
                salesInvoiceHeaderDb.SubNetValue = salesInvoiceHeader.SubNetValue;
                salesInvoiceHeaderDb.OtherTaxValue = salesInvoiceHeader.OtherTaxValue;
                salesInvoiceHeaderDb.NetValueBeforeAdditionalDiscount = salesInvoiceHeader.NetValueBeforeAdditionalDiscount;
                salesInvoiceHeaderDb.AdditionalDiscountValue = salesInvoiceHeader.AdditionalDiscountValue;
                salesInvoiceHeaderDb.NetValue = salesInvoiceHeader.NetValue;
                salesInvoiceHeaderDb.TotalCostValue = salesInvoiceHeader.TotalCostValue;
                salesInvoiceHeaderDb.DebitAccountId = salesInvoiceHeader.DebitAccountId;
                salesInvoiceHeaderDb.CreditAccountId = salesInvoiceHeader.CreditAccountId;
                salesInvoiceHeaderDb.RemarksAr = salesInvoiceHeader.RemarksAr;
                salesInvoiceHeaderDb.RemarksEn = salesInvoiceHeader.RemarksEn;
                salesInvoiceHeaderDb.CanReturnInDays = salesInvoiceHeader.CanReturnInDays;
                salesInvoiceHeaderDb.CanReturnUntilDate = salesInvoiceHeader.CanReturnUntilDate;
                salesInvoiceHeaderDb.InvoiceTypeId = salesInvoiceHeader.InvoiceTypeId;
                salesInvoiceHeaderDb.ClientBalance = salesInvoiceHeader.ClientBalance;
                salesInvoiceHeaderDb.CreditLimitDays = salesInvoiceHeader.CreditLimitDays;
                salesInvoiceHeaderDb.CreditLimitValues = salesInvoiceHeader.CreditLimitValues;
                salesInvoiceHeaderDb.DebitLimitDays = salesInvoiceHeader.DebitLimitDays;
                salesInvoiceHeaderDb.ArchiveHeaderId = salesInvoiceHeader.ArchiveHeaderId;

                salesInvoiceHeaderDb.ModifiedAt = DateHelper.GetDateTimeNow();
                salesInvoiceHeaderDb.UserNameModified = await _httpContextAccessor!.GetUserName();
                salesInvoiceHeaderDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();


                var salesInvoiceHeaderValidator = await new SalesInvoiceHeaderValidator(_localizer).ValidateAsync(salesInvoiceHeaderDb);
                var validationResult = salesInvoiceHeaderValidator.IsValid;
                if (validationResult)
                {
                    _repository.Update(salesInvoiceHeaderDb);
                    await _repository.SaveChanges();
                    return new ResponseDto() { Id = salesInvoiceHeaderDb.SalesInvoiceHeaderId, Success = true, Message = await _genericMessageService.GetMessage(SalesInvoiceMenuCodeHelper.GetMenuCode(salesInvoiceHeader), GenericMessageData.UpdatedSuccessWithCode, $"{salesInvoiceHeaderDb.Prefix}{salesInvoiceHeaderDb.DocumentCode}{salesInvoiceHeaderDb.Suffix}") };
                }
                else
                {
                    return new ResponseDto() { Id = 0, Success = false, Message = salesInvoiceHeaderValidator.ToString("~") };
                }
            }
            return new ResponseDto() { Id = 0, Success = false, Message = await _genericMessageService.GetMessage(SalesInvoiceMenuCodeHelper.GetMenuCode(salesInvoiceHeader), GenericMessageData.NotFound) };
        }

        public async Task<ResponseDto> IsSalesInvoiceCodeExist(int salesInvoiceHeaderId, int salesInvoiceCode, int storeId, bool separateYears, DateTime documentDate, string? prefix, string? suffix, bool isOnTheWay, bool isDirectInvoice, bool separateCashFromCredit, bool creditPayment, bool separatingTheSequenceOfInvoicesForEachSeller, int? sellerId)
        {
            var salesInvoiceHeader = await _repository.GetAll().FirstOrDefaultAsync(
                x => x.SalesInvoiceHeaderId != salesInvoiceHeaderId && 
                    (
                        x.StoreId == storeId && 
                        (!separateYears || x.DocumentDate.Year == documentDate.Year) && 
                        x.Prefix == prefix && 
                        x.DocumentCode == salesInvoiceCode && 
                        x.Suffix == suffix && 
                        x.IsOnTheWay == isOnTheWay && 
                        x.IsDirectInvoice == isDirectInvoice && 
                        (!separateCashFromCredit || x.CreditPayment == creditPayment) &&
                        (!separatingTheSequenceOfInvoicesForEachSeller || x.SellerId == sellerId)
                    )
            );
            if (salesInvoiceHeader is not null)
            {
                return new ResponseDto() { Id = salesInvoiceHeader.SalesInvoiceHeaderId, Success = true };
            }
            return new ResponseDto() { Id = 0, Success = false };
        }

        public async Task<int> GetNextId()
        {
            int id = 1;
            try { id = await _repository.GetAll().MaxAsync(a => a.SalesInvoiceHeaderId) + 1; } catch { id = 1; }
            return id;
        }

        public async Task<ResponseDto> DeleteSalesInvoiceHeader(int id, int menuCode)
        {
            var salesInvoiceHeader = await _repository.GetAll().FirstOrDefaultAsync(x => x.SalesInvoiceHeaderId == id);
            if (salesInvoiceHeader != null)
            {
                if (salesInvoiceHeader.IsBlocked)
                {
                    return new ResponseDto() { Id = salesInvoiceHeader.SalesInvoiceHeaderId, Success = false, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.CannotPerformOperationBecauseStoppedDealingOn) };
                }

                _repository.Delete(salesInvoiceHeader);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = id, Success = true, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.DeleteSuccessWithCode, $"{salesInvoiceHeader.Prefix}{salesInvoiceHeader.DocumentCode}{salesInvoiceHeader.Suffix}") };
            }
            return new ResponseDto() { Id = id, Success = false, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.NotFound) };
        }

        public async Task<int> GetNextSalesInvoiceCode(int storeId, bool separateYears, DateTime documentDate, string? prefix, string? suffix, bool isOnTheWay, bool isDirectInvoice, bool separateCashFromCredit, bool creditPayment, bool separatingTheSequenceOfInvoicesForEachSeller, int? sellerId)
        {
            int id = 1;
            try
            {
				id = await _repository.GetAll().AsNoTracking().Where(x =>
						(!separateYears || x.DocumentDate.Year == documentDate.Year) &&
						x.Prefix == prefix &&
						x.Suffix == suffix &&
						x.StoreId == storeId &&
						x.IsOnTheWay == isOnTheWay &&
						x.IsDirectInvoice == isDirectInvoice &&
						(!separateCashFromCredit || x.CreditPayment == creditPayment) &&
						(!separatingTheSequenceOfInvoicesForEachSeller || x.SellerId == sellerId)
					).MaxAsync(a => a.DocumentCode) + 1;
			}
            catch { id = 1; }
            return id;
        }
    }
}
