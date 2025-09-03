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

namespace Sales.Service.Services
{
    public class SalesInvoiceReturnHeaderService: BaseService<SalesInvoiceReturnHeader>, ISalesInvoiceReturnHeaderService
    {

        private readonly IShipmentTypeService _shipmentTypeService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStoreService _storeService;
        private readonly IClientService _clientService;
        private readonly IStringLocalizer<SalesInvoiceReturnHeaderService> _localizer;
        private readonly IGenericMessageService _genericMessageService;
        private readonly IMenuEncodingService _menuEncodingService;
        private readonly ISalesInvoiceHeaderService _salesInvoiceHeaderService;
        private readonly IApplicationSettingService _applicationSettingService;
		private readonly IMenuService _menuService;
		private readonly ISellerService _sellerService;

		public SalesInvoiceReturnHeaderService(IRepository<SalesInvoiceReturnHeader> repository, IShipmentTypeService shipmentTypeService, IHttpContextAccessor httpContextAccessor, IStoreService storeService, IStringLocalizer<SalesInvoiceReturnHeaderService> localizer, IGenericMessageService genericMessageService, IMenuEncodingService menuEncodingService, IClientService clientService, ISalesInvoiceHeaderService salesInvoiceHeaderService, IApplicationSettingService applicationSettingService, IMenuService menuService, ISellerService sellerService) : base(repository)
        {
            _shipmentTypeService = shipmentTypeService;
            _httpContextAccessor = httpContextAccessor;
            _storeService = storeService;
            _clientService = clientService;
            _localizer = localizer;
            _genericMessageService = genericMessageService;
            _menuEncodingService = menuEncodingService;
            _salesInvoiceHeaderService = salesInvoiceHeaderService;
            _applicationSettingService = applicationSettingService;
            _menuService = menuService;
			_sellerService = sellerService;
		}

        public IQueryable<SalesInvoiceReturnHeaderDto> GetSalesInvoiceReturnHeaders()
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();
            var data = from salesInvoiceReturnHeader in _repository.GetAll()
                       from store in _storeService.GetAll().Where(x => x.StoreId == salesInvoiceReturnHeader.StoreId)
                       from client in _clientService.GetAll().Where(x => x.ClientId == salesInvoiceReturnHeader.ClientId)
                       from salesInvoiceHeader in _salesInvoiceHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceReturnHeader.SalesInvoiceHeaderId)
                       from shipmentType in _shipmentTypeService.GetAll().Where(x => x.ShipmentTypeId == salesInvoiceReturnHeader.ShipmentTypeId).DefaultIfEmpty()
					   from menu in _menuService.GetAll().Where(x => x.MenuCode == salesInvoiceReturnHeader.MenuCode).DefaultIfEmpty()
                       from seller in _sellerService.GetAll().Where(x => x.SellerId == salesInvoiceReturnHeader.SellerId).DefaultIfEmpty()
					   select new SalesInvoiceReturnHeaderDto()
                       {
                           SalesInvoiceReturnHeaderId = salesInvoiceReturnHeader.SalesInvoiceReturnHeaderId,
                           Prefix = salesInvoiceReturnHeader.Prefix,
                           DocumentCode = salesInvoiceReturnHeader.DocumentCode,
                           Suffix = salesInvoiceReturnHeader.Suffix,
                           DocumentFullCode = $"{salesInvoiceReturnHeader.Prefix}{salesInvoiceReturnHeader.DocumentCode}{salesInvoiceReturnHeader.Suffix}",
                           SalesInvoiceHeaderId = salesInvoiceReturnHeader.SalesInvoiceHeaderId,
                           SalesInvoiceFullCode = $"{salesInvoiceHeader.Prefix}{salesInvoiceHeader.DocumentCode}{salesInvoiceHeader.Suffix}",
                           SalesInvoiceDocumentReference = salesInvoiceHeader.DocumentReference,
                           DocumentReference = salesInvoiceReturnHeader.DocumentReference,
                           ClientId = salesInvoiceReturnHeader.ClientId,
                           ClientCode = client.ClientCode,
                           ClientName = language == LanguageCode.Arabic ? client.ClientNameAr : client.ClientNameEn,
						   SellerId = seller != null ? seller.SellerId : null,
                           SellerCode = seller != null ? seller.SellerCode : null,
						   SellerName = seller != null ? language == LanguageCode.Arabic ? seller.SellerNameAr : seller.SellerNameEn : null,
						   StoreId = salesInvoiceReturnHeader.StoreId,
                           StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
                           DocumentDate = salesInvoiceReturnHeader.DocumentDate,
                           EntryDate = salesInvoiceReturnHeader.EntryDate,
                           Reference = salesInvoiceReturnHeader.Reference,
                           IsDirectInvoice = salesInvoiceReturnHeader.IsDirectInvoice,
                           CreditPayment = salesInvoiceReturnHeader.CreditPayment,
                           TaxTypeId = salesInvoiceReturnHeader.TaxTypeId,
                           ShippingDate = salesInvoiceReturnHeader.ShippingDate,
                           DeliveryDate = salesInvoiceReturnHeader.DeliveryDate,
                           DueDate = salesInvoiceReturnHeader.DueDate,
                           ShipmentTypeId = salesInvoiceReturnHeader.ShipmentTypeId,
                           ShipmentTypeName = shipmentType != null ? language == LanguageCode.Arabic ? shipmentType.ShipmentTypeNameAr : shipmentType.ShipmentTypeNameEn : null,
                           CashClientName = salesInvoiceReturnHeader.ClientName,
                           ClientPhone = salesInvoiceReturnHeader.ClientPhone,
                           ClientAddress = salesInvoiceReturnHeader.ClientAddress,
                           ClientTaxCode = salesInvoiceReturnHeader.ClientTaxCode,
                           DriverName = salesInvoiceReturnHeader.DriverName,
                           DriverPhone = salesInvoiceReturnHeader.DriverPhone,
                           ClientResponsibleName = salesInvoiceReturnHeader.ClientResponsibleName,
                           ClientResponsiblePhone = salesInvoiceReturnHeader.ClientResponsiblePhone,
                           ShipTo = salesInvoiceReturnHeader.ShipTo,
                           BillTo = salesInvoiceReturnHeader.BillTo,
                           ShippingRemarks = salesInvoiceReturnHeader.ShippingRemarks,
                           TotalValue = salesInvoiceReturnHeader.TotalValue,
                           DiscountPercent = salesInvoiceReturnHeader.DiscountPercent,
                           DiscountValue = salesInvoiceReturnHeader.DiscountValue,
                           TotalItemDiscount = salesInvoiceReturnHeader.TotalItemDiscount,
                           GrossValue = salesInvoiceReturnHeader.GrossValue,
                           VatValue = salesInvoiceReturnHeader.VatValue,
                           SubNetValue = salesInvoiceReturnHeader.SubNetValue,
                           OtherTaxValue = salesInvoiceReturnHeader.OtherTaxValue,
                           NetValueBeforeAdditionalDiscount = salesInvoiceReturnHeader.NetValueBeforeAdditionalDiscount,
                           AdditionalDiscountValue = salesInvoiceReturnHeader.AdditionalDiscountValue,
                           NetValue = salesInvoiceReturnHeader.NetValue,
                           TotalCostValue = salesInvoiceReturnHeader.TotalCostValue,
                           DebitAccountId = salesInvoiceReturnHeader.DebitAccountId,
                           CreditAccountId = salesInvoiceReturnHeader.CreditAccountId,
                           JournalHeaderId = salesInvoiceReturnHeader.JournalHeaderId,
                           RemarksAr = salesInvoiceReturnHeader.RemarksAr,
                           RemarksEn = salesInvoiceReturnHeader.RemarksEn,
                           IsOnTheWay = salesInvoiceReturnHeader.IsOnTheWay,
                           IsClosed = salesInvoiceReturnHeader.IsClosed || salesInvoiceReturnHeader.IsEnded || salesInvoiceReturnHeader.IsBlocked,
                           IsEnded = salesInvoiceReturnHeader.IsEnded,
                           IsBlocked = salesInvoiceReturnHeader.IsBlocked,
                           ArchiveHeaderId = salesInvoiceReturnHeader.ArchiveHeaderId,
						   MenuCode = salesInvoiceReturnHeader.MenuCode,
						   MenuName = menu != null ? language == LanguageCode.Arabic ? menu.MenuNameAr : menu.MenuNameEn : null,

						   CreatedAt = salesInvoiceReturnHeader.CreatedAt,
                           UserNameCreated = salesInvoiceReturnHeader.UserNameCreated,
                           IpAddressCreated = salesInvoiceReturnHeader.IpAddressCreated,

                           ModifiedAt = salesInvoiceReturnHeader.ModifiedAt,
                           UserNameModified = salesInvoiceReturnHeader.UserNameModified,
                           IpAddressModified = salesInvoiceReturnHeader.IpAddressModified
                       };
            return data;
        }

        public IQueryable<SalesInvoiceReturnHeaderDto> GetUserSalesInvoiceReturnHeaders(int menuCode)
        {
            return menuCode switch
            {
                MenuCodeData.SalesInvoiceReturn => GetUserSalesInvoiceReturnHeadersInternal(false, false, null, false),
                MenuCodeData.ReservationInvoiceCloseOut => GetUserSalesInvoiceReturnHeadersInternal(false, true, null, false),
                MenuCodeData.CreditSalesInvoiceReturn => GetUserSalesInvoiceReturnHeadersInternal(true, false, true, false),
                MenuCodeData.CashSalesInvoiceReturn => GetUserSalesInvoiceReturnHeadersInternal(true, false, false, false),
                _ => GetUserSalesInvoiceReturnHeadersInternal(false, false, false, true)
            };
        }

		private IQueryable<SalesInvoiceReturnHeaderDto> GetUserSalesInvoiceReturnHeadersInternal(bool isDirectInvoice, bool isOnTheWay, bool? isCreditPayment, bool getAll)
		{
			var userStore = _httpContextAccessor.GetCurrentUserStore();
			return GetSalesInvoiceReturnHeaders().Where(x => x.StoreId == userStore && (getAll || (x.IsDirectInvoice == isDirectInvoice && x.IsOnTheWay == isOnTheWay && (isCreditPayment == null || x.CreditPayment == isCreditPayment))));
		}

		public IQueryable<SalesInvoiceReturnHeaderDto> GetSalesInvoiceReturnHeadersByStoreId(int storeId, int? clientId, int salesInvoiceReturnHeaderId)
        {
            clientId ??= 0;
            if (salesInvoiceReturnHeaderId == 0)
            {
                return GetSalesInvoiceReturnHeaders().Where(x => x.StoreId == storeId && (clientId == 0 || x.ClientId == clientId) && x.IsEnded == false && x.IsBlocked == false);
            }
            else
            {
                return GetSalesInvoiceReturnHeaders().Where(x => x.SalesInvoiceReturnHeaderId == salesInvoiceReturnHeaderId);
            }
        }

        public async Task<SalesInvoiceReturnHeaderDto?> GetSalesInvoiceReturnHeaderById(int id)
        {
            return await GetSalesInvoiceReturnHeaders().FirstOrDefaultAsync(x => x.SalesInvoiceReturnHeaderId == id);
        }

        public async Task<DocumentCodeDto> GetSalesInvoiceReturnCode(int storeId, DateTime documentDate, bool isOnTheWay,bool isDirectInvoice, bool creditPayment)
        {
            var separateCashFromCredit = await _applicationSettingService.SeparateCashFromCreditSalesInvoiceReturn(storeId);
            var separateYears = await _applicationSettingService.SeparateYears(storeId);
            return await GetSalesInvoiceReturnCodeInternal(storeId, separateYears, documentDate, isOnTheWay, isDirectInvoice, creditPayment, separateCashFromCredit);
        }

        public async Task<DocumentCodeDto> GetSalesInvoiceReturnCodeInternal(int storeId, bool separateYears, DateTime documentDate, bool isOnTheWay, bool isDirectInvoice, bool creditPayment, bool separateCashFromCredit)
        {
            var encoding = await _menuEncodingService.GetMenuEncoding(storeId, SalesInvoiceReturnMenuCodeHelper.GetMenuCode(isOnTheWay, isDirectInvoice, creditPayment));
            var code = await GetNextSalesInvoiceReturnCode(storeId, separateYears, documentDate, encoding.Prefix, encoding.Suffix, isOnTheWay, isDirectInvoice, separateCashFromCredit, creditPayment);
            return new DocumentCodeDto() { NextCode = code, Prefix = encoding.Prefix, Suffix = encoding.Suffix };
        }

		public async Task<bool> UpdateAllSalesInvoiceReturnsBlockedFromProformaInvoice(int proformaInvoiceHeaderId, bool isBlocked)
		{
			var headerList = await (
					from salesInvoiceReturn in _repository.GetAll()
					from salesInvoiceHeader in _salesInvoiceHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceReturn.SalesInvoiceHeaderId && x.ProformaInvoiceHeaderId == proformaInvoiceHeaderId)
					select salesInvoiceReturn
				).ToListAsync();

			foreach (var header in headerList)
			{
				header.IsBlocked = isBlocked;
			}

            _repository.UpdateRange(headerList);
			await _repository.SaveChanges();
			return true;
		}

        public async Task<bool> UpdateClosed(int? salesInvoiceReturnHeaderId, bool isClosed)
        {
            var header = await _repository.FindBy(x => x.SalesInvoiceReturnHeaderId == salesInvoiceReturnHeaderId).FirstOrDefaultAsync();
            if (header is null) return false;

            header.IsClosed = isClosed;

            _repository.Update(header);
            await _repository.SaveChanges();
            return true;
        }

        public async Task<bool> UpdateEnded(int? salesInvoiceReturnHeaderId, bool isEnded)
        {
            var header = await _repository.FindBy(x => x.SalesInvoiceReturnHeaderId == salesInvoiceReturnHeaderId).FirstOrDefaultAsync();
            if (header is null) return false;

            header.IsEnded = isEnded;

            _repository.Update(header);
            await _repository.SaveChanges();
            return true;
		}

		public async Task<int> UpdateAllSalesInvoiceReturnEndedLinkedToSalesInvoice(int? salesInvoiceHeaderId, bool isEnded)
		{
			var headers = await _repository.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId && !x.IsOnTheWay).ToListAsync();

            foreach (var header in headers)
            {
                header.IsEnded = isEnded;
            }

            _repository.UpdateRange(headers);
			await _repository.SaveChanges();
			return headers.Count;
		}

		public async Task<bool> UpdateReservationInvoiceCloseOutEndedLinkedToSalesInvoice(int? salesInvoiceHeaderId, bool isEnded)
		{
			var header = await _repository.FindBy(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId && x.IsOnTheWay).FirstOrDefaultAsync();
			if (header is null) return false;

			header.IsEnded = isEnded;

            _repository.Update(header);
			await _repository.SaveChanges();
			return true;
		}

		public async Task<bool> UpdateEndedAndClosed(int? salesInvoiceReturnHeaderId, bool isEnded, bool isClosed)
        {
            var header = await _repository.FindBy(x => x.SalesInvoiceReturnHeaderId == salesInvoiceReturnHeaderId).FirstOrDefaultAsync();
            if (header is null) return false;

            header.IsEnded = isEnded;
            header.IsClosed = isClosed;

            _repository.Update(header);
            await _repository.SaveChanges();
            return true;
        }

        public async Task<ResponseDto> SaveSalesInvoiceReturnHeader(SalesInvoiceReturnHeaderDto salesInvoiceReturnHeader, bool hasApprove, bool approved, int? requestId, string? documentReference)
        {
            var separateCashFromCredit = await _applicationSettingService.SeparateCashFromCreditSalesInvoiceReturn(salesInvoiceReturnHeader.StoreId);
            var separateYears = await _applicationSettingService.SeparateYears(salesInvoiceReturnHeader.StoreId);

            if (hasApprove)
            {
                if (salesInvoiceReturnHeader.SalesInvoiceReturnHeaderId == 0)
                {
                    return await CreateSalesInvoiceReturnHeader(salesInvoiceReturnHeader, hasApprove, approved, requestId, separateCashFromCredit, separateYears, documentReference);
                }
                else
                {
                    return await UpdateSalesInvoiceReturnHeader(salesInvoiceReturnHeader);
                }
            }
            else
            {
                var salesInvoiceReturnHeaderExist = await IsSalesInvoiceReturnCodeExist(salesInvoiceReturnHeader.SalesInvoiceReturnHeaderId, salesInvoiceReturnHeader.DocumentCode, salesInvoiceReturnHeader.StoreId, separateYears, salesInvoiceReturnHeader.DocumentDate, salesInvoiceReturnHeader.Prefix, salesInvoiceReturnHeader.Suffix, salesInvoiceReturnHeader.IsOnTheWay, salesInvoiceReturnHeader.IsDirectInvoice, separateCashFromCredit, salesInvoiceReturnHeader.CreditPayment);
                if (salesInvoiceReturnHeaderExist.Success)
                {
                    var nextSalesInvoiceReturnCode = await GetNextSalesInvoiceReturnCode(salesInvoiceReturnHeader.StoreId, separateYears, salesInvoiceReturnHeader.DocumentDate, salesInvoiceReturnHeader.Prefix, salesInvoiceReturnHeader.Suffix, salesInvoiceReturnHeader.IsOnTheWay, salesInvoiceReturnHeader.IsDirectInvoice, separateCashFromCredit, salesInvoiceReturnHeader.CreditPayment);
                    return new ResponseDto() { Id = nextSalesInvoiceReturnCode, ResponseType = ResponseTypeData.Confirm, Success = false, Message = await _genericMessageService.GetMessage(SalesInvoiceReturnMenuCodeHelper.GetMenuCode(salesInvoiceReturnHeader), GenericMessageData.CodeAlreadyExist, $"{ nextSalesInvoiceReturnCode }") };
                }
                else
                {
                    if (salesInvoiceReturnHeader.SalesInvoiceReturnHeaderId == 0)
                    {
                        return await CreateSalesInvoiceReturnHeader(salesInvoiceReturnHeader, hasApprove, approved, requestId, separateCashFromCredit, separateYears, documentReference);
                    }
                    else
                    {
                        return await UpdateSalesInvoiceReturnHeader(salesInvoiceReturnHeader);
                    }
                }
            }
        }

        public async Task<ResponseDto> CreateSalesInvoiceReturnHeader(SalesInvoiceReturnHeaderDto salesInvoiceReturnHeader, bool hasApprove, bool approved, int? requestId, bool separateCashFromCredit, bool separateYears, string? documentReference)
        {
            int salesInvoiceReturnCode;
            string? prefix;
            string? suffix;
            var nextSalesInvoiceReturnCode = await GetSalesInvoiceReturnCodeInternal(salesInvoiceReturnHeader.StoreId, separateYears, salesInvoiceReturnHeader.DocumentDate, salesInvoiceReturnHeader.IsOnTheWay, salesInvoiceReturnHeader.IsDirectInvoice, salesInvoiceReturnHeader.CreditPayment, separateCashFromCredit);
            if (hasApprove && approved)
            {
                salesInvoiceReturnCode = nextSalesInvoiceReturnCode.NextCode;
                prefix = nextSalesInvoiceReturnCode.Prefix;
                suffix = nextSalesInvoiceReturnCode.Suffix;
            }
            else
            {
                salesInvoiceReturnCode = salesInvoiceReturnHeader.DocumentCode != 0 ? salesInvoiceReturnHeader.DocumentCode : nextSalesInvoiceReturnCode.NextCode;
                prefix = string.IsNullOrWhiteSpace(salesInvoiceReturnHeader.Prefix) ? nextSalesInvoiceReturnCode.Prefix : salesInvoiceReturnHeader.Prefix;
                suffix = string.IsNullOrWhiteSpace(salesInvoiceReturnHeader.Suffix) ? nextSalesInvoiceReturnCode.Suffix : salesInvoiceReturnHeader.Suffix;
            }

            var salesInvoiceReturnHeaderId = await GetNextId();
            var newSalesInvoiceReturnHeader = new SalesInvoiceReturnHeader()
            {
                SalesInvoiceReturnHeaderId = salesInvoiceReturnHeaderId,
                Prefix = prefix,
                DocumentCode = salesInvoiceReturnCode,
                Suffix = suffix,
                SalesInvoiceHeaderId = salesInvoiceReturnHeader.SalesInvoiceHeaderId,
                DocumentReference = documentReference != null ? documentReference : (hasApprove ? $"{DocumentReferenceData.Approval}{requestId}" : $"{DocumentReferenceData.SalesInvoiceReturn}{salesInvoiceReturnHeaderId}"),
                ClientId = salesInvoiceReturnHeader.ClientId,
                SellerId = salesInvoiceReturnHeader.SellerId,
                StoreId = salesInvoiceReturnHeader.StoreId,
                DocumentDate = salesInvoiceReturnHeader.DocumentDate,
                EntryDate = DateHelper.GetDateTimeNow(),
                Reference = salesInvoiceReturnHeader.Reference,
                IsDirectInvoice = salesInvoiceReturnHeader.IsDirectInvoice,
                CreditPayment = salesInvoiceReturnHeader.CreditPayment,
                TaxTypeId = salesInvoiceReturnHeader.TaxTypeId,
                ShippingDate = salesInvoiceReturnHeader.ShippingDate,
                DeliveryDate = salesInvoiceReturnHeader.DeliveryDate,
                DueDate = salesInvoiceReturnHeader.DueDate,
                ShipmentTypeId = salesInvoiceReturnHeader.ShipmentTypeId,
                ClientName = salesInvoiceReturnHeader.CashClientName,
                ClientPhone = salesInvoiceReturnHeader.ClientPhone,
                ClientAddress = salesInvoiceReturnHeader.ClientAddress,
                ClientTaxCode = salesInvoiceReturnHeader.ClientTaxCode,
                DriverName = salesInvoiceReturnHeader.DriverName,
                DriverPhone = salesInvoiceReturnHeader.DriverPhone,
                ClientResponsibleName = salesInvoiceReturnHeader.ClientResponsibleName,
                ClientResponsiblePhone = salesInvoiceReturnHeader.ClientResponsiblePhone,
                ShipTo = salesInvoiceReturnHeader.ShipTo,
                BillTo = salesInvoiceReturnHeader.BillTo,
                ShippingRemarks = salesInvoiceReturnHeader.ShippingRemarks,
                TotalValue = salesInvoiceReturnHeader.TotalValue,
                DiscountPercent = salesInvoiceReturnHeader.DiscountPercent,
                DiscountValue = salesInvoiceReturnHeader.DiscountValue,
                TotalItemDiscount = salesInvoiceReturnHeader.TotalItemDiscount,
                GrossValue = salesInvoiceReturnHeader.GrossValue,
                VatValue = salesInvoiceReturnHeader.VatValue,
                SubNetValue = salesInvoiceReturnHeader.SubNetValue,
                OtherTaxValue = salesInvoiceReturnHeader.OtherTaxValue,
                NetValueBeforeAdditionalDiscount = salesInvoiceReturnHeader.NetValueBeforeAdditionalDiscount,
                AdditionalDiscountValue = salesInvoiceReturnHeader.AdditionalDiscountValue,
                NetValue = salesInvoiceReturnHeader.NetValue,
                TotalCostValue = salesInvoiceReturnHeader.TotalCostValue,
                DebitAccountId = salesInvoiceReturnHeader.DebitAccountId,
                CreditAccountId = salesInvoiceReturnHeader.CreditAccountId,
                JournalHeaderId = salesInvoiceReturnHeader.JournalHeaderId,
                RemarksAr = salesInvoiceReturnHeader.RemarksAr,
                RemarksEn = salesInvoiceReturnHeader.RemarksEn,
                IsOnTheWay = salesInvoiceReturnHeader.IsOnTheWay,
                IsClosed = false,
                IsEnded = false,
                IsBlocked = false,
                ArchiveHeaderId = salesInvoiceReturnHeader.ArchiveHeaderId,
                MenuCode = (short)SalesInvoiceReturnMenuCodeHelper.GetMenuCode(salesInvoiceReturnHeader),

				CreatedAt = DateHelper.GetDateTimeNow(),
                UserNameCreated = await _httpContextAccessor!.GetUserName(),
                IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
                Hide = false,
            };

            var salesInvoiceReturnHeaderValidator = await new SalesInvoiceReturnHeaderValidator(_localizer).ValidateAsync(newSalesInvoiceReturnHeader);
            var validationResult = salesInvoiceReturnHeaderValidator.IsValid;
            if (validationResult)
            {
                await _repository.Insert(newSalesInvoiceReturnHeader);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = newSalesInvoiceReturnHeader.SalesInvoiceReturnHeaderId, Success = true, Message = await _genericMessageService.GetMessage(SalesInvoiceReturnMenuCodeHelper.GetMenuCode(salesInvoiceReturnHeader), GenericMessageData.CreatedSuccessWithCode, $"{newSalesInvoiceReturnHeader.Prefix}{newSalesInvoiceReturnHeader.DocumentCode}{newSalesInvoiceReturnHeader.Suffix}") };
            }
            else
            {
                return new ResponseDto() { Id = newSalesInvoiceReturnHeader.SalesInvoiceReturnHeaderId, Success = false, Message = salesInvoiceReturnHeaderValidator.ToString("~") };
            }
        }

        public async Task<ResponseDto> UpdateSalesInvoiceReturnHeader(SalesInvoiceReturnHeaderDto salesInvoiceReturnHeader)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var salesInvoiceReturnHeaderDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.SalesInvoiceReturnHeaderId == salesInvoiceReturnHeader.SalesInvoiceReturnHeaderId);
            if (salesInvoiceReturnHeaderDb != null)
            {
                salesInvoiceReturnHeaderDb.SalesInvoiceHeaderId = salesInvoiceReturnHeader.SalesInvoiceHeaderId;
                salesInvoiceReturnHeaderDb.ClientId = salesInvoiceReturnHeader.ClientId;
                salesInvoiceReturnHeaderDb.SellerId = salesInvoiceReturnHeader.SellerId;
                salesInvoiceReturnHeaderDb.StoreId = salesInvoiceReturnHeader.StoreId;
                salesInvoiceReturnHeaderDb.DocumentDate = salesInvoiceReturnHeader.DocumentDate;
                salesInvoiceReturnHeaderDb.Reference = salesInvoiceReturnHeader.Reference;
                salesInvoiceReturnHeaderDb.CreditPayment = salesInvoiceReturnHeader.CreditPayment;
                salesInvoiceReturnHeaderDb.TaxTypeId = salesInvoiceReturnHeader.TaxTypeId;
                salesInvoiceReturnHeaderDb.ShippingDate = salesInvoiceReturnHeader.ShippingDate;
                salesInvoiceReturnHeaderDb.DeliveryDate = salesInvoiceReturnHeader.DeliveryDate;
                salesInvoiceReturnHeaderDb.DueDate = salesInvoiceReturnHeader.DueDate;
                salesInvoiceReturnHeaderDb.ShipmentTypeId = salesInvoiceReturnHeader.ShipmentTypeId;
                salesInvoiceReturnHeaderDb.ClientName = salesInvoiceReturnHeader.CashClientName;
                salesInvoiceReturnHeaderDb.ClientPhone = salesInvoiceReturnHeader.ClientPhone;
                salesInvoiceReturnHeaderDb.ClientAddress = salesInvoiceReturnHeader.ClientAddress;
                salesInvoiceReturnHeaderDb.ClientTaxCode = salesInvoiceReturnHeader.ClientTaxCode;
                salesInvoiceReturnHeaderDb.DriverName = salesInvoiceReturnHeader.DriverName;
                salesInvoiceReturnHeaderDb.DriverPhone = salesInvoiceReturnHeader.DriverPhone;
                salesInvoiceReturnHeaderDb.ClientResponsibleName = salesInvoiceReturnHeader.ClientResponsibleName;
                salesInvoiceReturnHeaderDb.ClientResponsiblePhone = salesInvoiceReturnHeader.ClientResponsiblePhone;
                salesInvoiceReturnHeaderDb.ShipTo = salesInvoiceReturnHeader.ShipTo;
                salesInvoiceReturnHeaderDb.BillTo = salesInvoiceReturnHeader.BillTo;
                salesInvoiceReturnHeaderDb.ShippingRemarks = salesInvoiceReturnHeader.ShippingRemarks;
                salesInvoiceReturnHeaderDb.TotalValue = salesInvoiceReturnHeader.TotalValue;
                salesInvoiceReturnHeaderDb.DiscountPercent = salesInvoiceReturnHeader.DiscountPercent;
                salesInvoiceReturnHeaderDb.DiscountValue = salesInvoiceReturnHeader.DiscountValue;
                salesInvoiceReturnHeaderDb.TotalItemDiscount = salesInvoiceReturnHeader.TotalItemDiscount;
                salesInvoiceReturnHeaderDb.GrossValue = salesInvoiceReturnHeader.GrossValue;
                salesInvoiceReturnHeaderDb.VatValue = salesInvoiceReturnHeader.VatValue;
                salesInvoiceReturnHeaderDb.SubNetValue = salesInvoiceReturnHeader.SubNetValue;
                salesInvoiceReturnHeaderDb.OtherTaxValue = salesInvoiceReturnHeader.OtherTaxValue;
                salesInvoiceReturnHeaderDb.NetValueBeforeAdditionalDiscount = salesInvoiceReturnHeader.NetValueBeforeAdditionalDiscount;
                salesInvoiceReturnHeaderDb.AdditionalDiscountValue = salesInvoiceReturnHeader.AdditionalDiscountValue;
                salesInvoiceReturnHeaderDb.NetValue = salesInvoiceReturnHeader.NetValue;
                salesInvoiceReturnHeaderDb.TotalCostValue = salesInvoiceReturnHeader.TotalCostValue;
                salesInvoiceReturnHeaderDb.DebitAccountId = salesInvoiceReturnHeader.DebitAccountId;
                salesInvoiceReturnHeaderDb.CreditAccountId = salesInvoiceReturnHeader.CreditAccountId;
                salesInvoiceReturnHeaderDb.RemarksAr = salesInvoiceReturnHeader.RemarksAr;
                salesInvoiceReturnHeaderDb.RemarksEn = salesInvoiceReturnHeader.RemarksEn;
                salesInvoiceReturnHeaderDb.ArchiveHeaderId = salesInvoiceReturnHeader.ArchiveHeaderId;

                salesInvoiceReturnHeaderDb.ModifiedAt = DateHelper.GetDateTimeNow();
                salesInvoiceReturnHeaderDb.UserNameModified = await _httpContextAccessor!.GetUserName();
                salesInvoiceReturnHeaderDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();


                var salesInvoiceReturnHeaderValidator = await new SalesInvoiceReturnHeaderValidator(_localizer).ValidateAsync(salesInvoiceReturnHeaderDb);
                var validationResult = salesInvoiceReturnHeaderValidator.IsValid;
                if (validationResult)
                {
                    _repository.Update(salesInvoiceReturnHeaderDb);
                    await _repository.SaveChanges();
                    return new ResponseDto() { Id = salesInvoiceReturnHeaderDb.SalesInvoiceReturnHeaderId, Success = true, Message = await _genericMessageService.GetMessage(SalesInvoiceReturnMenuCodeHelper.GetMenuCode(salesInvoiceReturnHeader), GenericMessageData.UpdatedSuccessWithCode, $"{salesInvoiceReturnHeaderDb.Prefix}{salesInvoiceReturnHeaderDb.DocumentCode}{salesInvoiceReturnHeaderDb.Suffix}") };
                }
                else
                {
                    return new ResponseDto() { Id = 0, Success = false, Message = salesInvoiceReturnHeaderValidator.ToString("~") };
                }
            }
            return new ResponseDto() { Id = 0, Success = false, Message = await _genericMessageService.GetMessage(SalesInvoiceReturnMenuCodeHelper.GetMenuCode(salesInvoiceReturnHeader), GenericMessageData.NotFound) };
        }

        public async Task<ResponseDto> IsSalesInvoiceReturnCodeExist(int salesInvoiceReturnHeaderId, int salesInvoiceReturnCode, int storeId, bool separateYears, DateTime documentDate, string? prefix, string? suffix, bool isOnTheWay, bool isDirectInvoice, bool separateCashFromCredit, bool creditPayment)
        {
            var salesInvoiceReturnHeader = await _repository.GetAll().FirstOrDefaultAsync(x => x.SalesInvoiceReturnHeaderId != salesInvoiceReturnHeaderId && (x.StoreId == storeId && (!separateYears || x.DocumentDate.Year == documentDate.Year) && x.Prefix == prefix && x.DocumentCode == salesInvoiceReturnCode && x.Suffix == suffix && x.IsOnTheWay == isOnTheWay && x.IsDirectInvoice == isDirectInvoice && (!separateCashFromCredit || x.CreditPayment == creditPayment)));
            if (salesInvoiceReturnHeader is not null)
            {
                return new ResponseDto() { Id = salesInvoiceReturnHeader.SalesInvoiceReturnHeaderId, Success = true };
            }
            return new ResponseDto() { Id = 0, Success = false };
        }

        public async Task<int> GetNextId()
        {
            int id = 1;
            try { id = await _repository.GetAll().MaxAsync(a => a.SalesInvoiceReturnHeaderId) + 1; } catch { id = 1; }
            return id;
        }

        public async Task<ResponseDto> DeleteSalesInvoiceReturnHeader(int id, int menuCode)
        {
            var salesInvoiceReturnHeader = await _repository.GetAll().FirstOrDefaultAsync(x => x.SalesInvoiceReturnHeaderId == id);
            if (salesInvoiceReturnHeader != null)
            {
                if (salesInvoiceReturnHeader.IsBlocked)
                {
                    return new ResponseDto() { Id = salesInvoiceReturnHeader.SalesInvoiceReturnHeaderId, Success = false, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.CannotPerformOperationBecauseStoppedDealingOn) };
                }

                _repository.Delete(salesInvoiceReturnHeader);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = id, Success = true, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.DeleteSuccessWithCode, $"{salesInvoiceReturnHeader.Prefix}{salesInvoiceReturnHeader.DocumentCode}{salesInvoiceReturnHeader.Suffix}") };
            }
            return new ResponseDto() { Id = id, Success = false, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.NotFound) };
        }

        public async Task<int> GetNextSalesInvoiceReturnCode(int storeId, bool separateYears, DateTime documentDate, string? prefix, string? suffix, bool isOnTheWay, bool isDirectInvoice, bool separateCashFromCredit, bool creditPayment)
        {
            int id = 1;
            try
            {
                id = await _repository.GetAll().AsNoTracking().Where(x => (!separateYears || x.DocumentDate.Year == documentDate.Year) && x.Prefix == prefix && x.Suffix == suffix && x.StoreId == storeId && x.IsOnTheWay == isOnTheWay && x.IsDirectInvoice == isDirectInvoice && (!separateCashFromCredit || x.CreditPayment == creditPayment)).MaxAsync(a => a.DocumentCode) + 1;
            }
            catch { id = 1; }
            return id;
        }
    }
}
