using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Contracts.Menus;
using Shared.CoreOne.Contracts.Modules;
using Shared.Service;
using Sales.CoreOne.Models.Domain;
using Shared.CoreOne;
using Sales.CoreOne.Contracts;
using Shared.Helper.Models.Dtos;
using Sales.CoreOne.Models.Dtos.ViewModels;
using Shared.Helper.Identity;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Microsoft.EntityFrameworkCore;
using Sales.Service.Validators;
using Shared.CoreOne.Models.StaticData;
using System.Security;
using Shared.CoreOne.Models.Domain.Modules;
using static Shared.Helper.Models.StaticData.LanguageData;
using Inventory.CoreOne.Models.Domain;
using Shared.CoreOne.Models.Domain.Archive;
using Shared.CoreOne.Contracts.Basics;
using Shared.CoreOne.Models.Domain.Basics;
using Shared.Helper.Logic;
using Shared.CoreOne.Contracts.Settings;

namespace Sales.Service.Services
{
	public class ProformaInvoiceHeaderService : BaseService<ProformaInvoiceHeader>, IProformaInvoiceHeaderService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IStoreService _storeService;
        private readonly IClientService _clientService;
		private readonly IStringLocalizer<ProformaInvoiceHeaderService> _localizer;
		private readonly IMenuEncodingService _menuEncodingService;
        private readonly IShipmentTypeService _shipmentTypeService;
        private readonly IDocumentStatusService _documentStatusService;
        private readonly IShippingStatusService _shippingStatusService;
        private readonly IGenericMessageService _genericMessageService;
		private readonly ISellerService _sellerService;
        private readonly IApplicationSettingService _applicationSettingService;

		public ProformaInvoiceHeaderService(IRepository<ProformaInvoiceHeader> repository, IHttpContextAccessor httpContextAccessor, IStoreService storeService, IStringLocalizer<ProformaInvoiceHeaderService> localizer, IMenuEncodingService menuEncodingService, IClientService clientService, IShipmentTypeService shipmentTypeService, IDocumentStatusService documentStatusService, IShippingStatusService shippingStatusService, IGenericMessageService genericMessageService, ISellerService sellerService, IApplicationSettingService applicationSettingService) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
			_storeService = storeService;
            _clientService = clientService;
			_localizer = localizer;
			_menuEncodingService = menuEncodingService;
            _shipmentTypeService = shipmentTypeService;
            _documentStatusService = documentStatusService;
            _shippingStatusService = shippingStatusService;
            _genericMessageService = genericMessageService;
			_sellerService = sellerService;
            _applicationSettingService = applicationSettingService;
		}

		public IQueryable<ProformaInvoiceHeaderDto> GetProformaInvoiceHeaders()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var data = from proformaInvoiceHeader in _repository.GetAll()
				from store in _storeService.GetAll().Where(x => x.StoreId == proformaInvoiceHeader.StoreId)
				from client in _clientService.GetAll().Where(x => x.ClientId == proformaInvoiceHeader.ClientId)
                from documentStatus in _documentStatusService.GetAll().Where(x => x.DocumentStatusId == proformaInvoiceHeader.DocumentStatusId)
                from shipmentType in _shipmentTypeService.GetAll().Where(x => x.ShipmentTypeId == proformaInvoiceHeader.ShipmentTypeId).DefaultIfEmpty()
                from shippingStatus in _shippingStatusService.GetAll().Where(x => x.ShippingStatusId == proformaInvoiceHeader.ShippingStatusId).DefaultIfEmpty()
                from seller in _sellerService.GetAll().Where(x => x.SellerId ==  proformaInvoiceHeader.SellerId).DefaultIfEmpty()
                select new ProformaInvoiceHeaderDto()
				{
					ProformaInvoiceHeaderId = proformaInvoiceHeader.ProformaInvoiceHeaderId,
					Prefix = proformaInvoiceHeader.Prefix,
					DocumentCode = proformaInvoiceHeader.DocumentCode,
					Suffix = proformaInvoiceHeader.Suffix,
					DocumentFullCode = $"{proformaInvoiceHeader.Prefix}{proformaInvoiceHeader.DocumentCode}{proformaInvoiceHeader.Suffix}",
					ClientQuotationApprovalHeaderId = proformaInvoiceHeader.ClientQuotationApprovalHeaderId,
					DocumentDate = proformaInvoiceHeader.DocumentDate,
					EntryDate = proformaInvoiceHeader.EntryDate,
                    DocumentReference = proformaInvoiceHeader.DocumentReference,
                    ClientId = proformaInvoiceHeader.ClientId,
                    ClientCode = client.ClientCode,
                    ClientName = language == LanguageCode.Arabic ? client.ClientNameAr : client.ClientNameEn,
					SellerId = seller != null ? seller.SellerId : null,
                    SellerCode = seller != null ? seller.SellerCode : null,
					SellerName = seller != null ? language == LanguageCode.Arabic ? seller.SellerNameAr : seller.SellerNameEn : null,
					StoreId = proformaInvoiceHeader.StoreId,
					StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
                    Reference = proformaInvoiceHeader.Reference,
                    CreditPayment = proformaInvoiceHeader.CreditPayment,
                    TaxTypeId = proformaInvoiceHeader.TaxTypeId,
                    ShippingDate = proformaInvoiceHeader.ShippingDate,
                    DeliveryDate = proformaInvoiceHeader.DeliveryDate,
                    DueDate = proformaInvoiceHeader.DueDate,
                    ShipmentTypeId = proformaInvoiceHeader.ShipmentTypeId,
                    ShipmentTypeName = shipmentType != null ? language == LanguageCode.Arabic ? shipmentType.ShipmentTypeNameAr : shipmentType.ShipmentTypeNameEn : null,
                    CashClientName = proformaInvoiceHeader.ClientName,
                    ClientPhone = proformaInvoiceHeader.ClientPhone,
                    ClientAddress = proformaInvoiceHeader.ClientAddress,
                    ClientTaxCode = proformaInvoiceHeader.ClientTaxCode,
                    DriverName = proformaInvoiceHeader.DriverName,
                    DriverPhone = proformaInvoiceHeader.DriverPhone,
                    ClientResponsibleName = proformaInvoiceHeader.ClientResponsibleName,
                    ClientResponsiblePhone = proformaInvoiceHeader.ClientResponsiblePhone,
                    ShipTo = proformaInvoiceHeader.ShipTo,
                    BillTo = proformaInvoiceHeader.BillTo,
                    ShippingRemarks = proformaInvoiceHeader.ShippingRemarks,
                    TotalValue = proformaInvoiceHeader.TotalValue,
					DiscountPercent = proformaInvoiceHeader.DiscountPercent,
					DiscountValue = proformaInvoiceHeader.DiscountValue,
					TotalItemDiscount = proformaInvoiceHeader.TotalItemDiscount,
					GrossValue = proformaInvoiceHeader.GrossValue,
					VatValue = proformaInvoiceHeader.VatValue,
					SubNetValue = proformaInvoiceHeader.SubNetValue,
					OtherTaxValue = proformaInvoiceHeader.OtherTaxValue,
                    NetValueBeforeAdditionalDiscount = proformaInvoiceHeader.NetValueBeforeAdditionalDiscount,
                    AdditionalDiscountValue = proformaInvoiceHeader.AdditionalDiscountValue,
					NetValue = proformaInvoiceHeader.NetValue,
                    TotalCostValue = proformaInvoiceHeader.TotalCostValue,
					RemarksAr = proformaInvoiceHeader.RemarksAr,
					RemarksEn = proformaInvoiceHeader.RemarksEn,
					IsClosed = proformaInvoiceHeader.IsClosed || proformaInvoiceHeader.IsEnded || proformaInvoiceHeader.IsBlocked,
                    IsCancelled = proformaInvoiceHeader.IsCancelled,
                    IsEnded = proformaInvoiceHeader.IsEnded,
                    IsBlocked = proformaInvoiceHeader.IsBlocked,
                    DocumentStatusId = proformaInvoiceHeader.DocumentStatusId,
                    DocumentStatusName = language == LanguageCode.Arabic ? documentStatus.DocumentStatusNameAr : documentStatus.DocumentStatusNameEn,
                    ShippingStatusId = proformaInvoiceHeader.ShippingStatusId,
                    ShippingStatusName = shippingStatus != null ? (language == LanguageCode.Arabic ? shippingStatus.ShippingStatusNameAr : shippingStatus.ShippingStatusNameEn) : null,
                    ArchiveHeaderId = proformaInvoiceHeader.ArchiveHeaderId,

                    CreatedAt = proformaInvoiceHeader.CreatedAt,
                    UserNameCreated = proformaInvoiceHeader.UserNameCreated,
                    IpAddressCreated = proformaInvoiceHeader.IpAddressCreated,

                    ModifiedAt = proformaInvoiceHeader.ModifiedAt,
                    UserNameModified = proformaInvoiceHeader.UserNameModified,
                    IpAddressModified = proformaInvoiceHeader.IpAddressModified
                };
			return data;
		}

        public IQueryable<ProformaInvoiceHeaderDto> GetUserProformaInvoiceHeaders()
        {
            var userStore = _httpContextAccessor.GetCurrentUserStore();
            return GetProformaInvoiceHeaders().Where(x => x.StoreId == userStore);
        }

		public IQueryable<ProformaInvoiceHeaderDto> GetProformaInvoiceHeadersByStoreId(int storeId, int? clientId, int proformaInvoiceHeaderId)
		{
            clientId ??= 0;
            if (proformaInvoiceHeaderId == 0)
            {
                return GetProformaInvoiceHeaders().Where(x => x.StoreId == storeId && (clientId == 0 || x.ClientId == clientId) && x.IsEnded == false && x.IsBlocked == false);
            }
            else
            {
                return GetProformaInvoiceHeaders().Where(x => x.ProformaInvoiceHeaderId == proformaInvoiceHeaderId);
            }
        }

        public async Task<ProformaInvoiceHeaderDto?> GetProformaInvoiceHeaderById(int id)
		{
			return await GetProformaInvoiceHeaders().FirstOrDefaultAsync(x => x.ProformaInvoiceHeaderId == id);
		}


        public async Task<DocumentCodeDto> GetProformaInvoiceCode(int storeId, DateTime documentDate)
        {
            var separateYears = await _applicationSettingService.SeparateYears(storeId);
            return await GetProformaInvoiceCodeInternal(storeId, separateYears, documentDate);
        }

        public async Task<DocumentCodeDto> GetProformaInvoiceCodeInternal(int storeId, bool separateYears, DateTime documentDate)
        {
            var encoding = await _menuEncodingService.GetMenuEncoding(storeId, MenuCodeData.ProformaInvoice);
            var code = await GetNextProformaInvoiceCode(storeId, separateYears, documentDate, encoding.Prefix, encoding.Suffix);
            return new DocumentCodeDto() { NextCode = code, Prefix = encoding.Prefix, Suffix = encoding.Suffix };
        }

        public async Task<bool> UpdateBlocked(int? proformaInvoiceHeaderId, bool isBlocked)
        {
            var header = await _repository.FindBy(x => x.ProformaInvoiceHeaderId == proformaInvoiceHeaderId).FirstOrDefaultAsync();
            if (header is null) return false;

            header.IsBlocked = isBlocked;
            _repository.Update(header);
            await _repository.SaveChanges();
            return true;
        }

        public async Task<bool> UpdateClosed(int? proformaInvoiceHeaderId, bool isClosed)
        {
            var header = await _repository.FindBy(x => x.ProformaInvoiceHeaderId == proformaInvoiceHeaderId).FirstOrDefaultAsync();
            if (header is null) return false;

            header.IsClosed = isClosed;
            _repository.Update(header);
            await _repository.SaveChanges();
            return true;
        }

        public async Task<bool> UpdateEnded(int? proformaInvoiceHeaderId, bool isEnded)
        {
            var header = await _repository.FindBy(x => x.ProformaInvoiceHeaderId == proformaInvoiceHeaderId).FirstOrDefaultAsync();
            if (header is null) return false;

            header.IsEnded = isEnded;
            _repository.Update(header);
            await _repository.SaveChanges();
            return true;
        }

        public async Task<bool> UpdateEndedAndClosed(int? proformaInvoiceHeaderId, bool isEnded, bool isClosed)
        {
            var header = await _repository.FindBy(x => x.ProformaInvoiceHeaderId == proformaInvoiceHeaderId).FirstOrDefaultAsync();
            if (header is null) return false;

            header.IsEnded = isEnded;
            header.IsClosed = isClosed;
            _repository.Update(header);
            await _repository.SaveChanges();
            return true;
        }

        public async Task<bool> UpdateDocumentStatus(int proformaInvoiceHeaderId, int documentStatusId)
        {
            var header = await _repository.FindBy(x => x.ProformaInvoiceHeaderId == proformaInvoiceHeaderId).FirstOrDefaultAsync();
            if (header is null) return false;

            header.DocumentStatusId = documentStatusId;
            _repository.Update(header);
            await _repository.SaveChanges();
            return true;
        }

        public async Task<ResponseDto> UpdateShippingStatus(int proformaInvoiceHeaderId, int shippingStatusId)
        {
            var header = await _repository.FindBy(x => x.ProformaInvoiceHeaderId == proformaInvoiceHeaderId).FirstOrDefaultAsync();
            if (header is null) return new ResponseDto { Id = proformaInvoiceHeaderId, Message = await _genericMessageService.GetMessage(MenuCodeData.ProformaInvoice, GenericMessageData.NotFound) };

            header.ShippingStatusId = shippingStatusId;
            _repository.Update(header);
            await _repository.SaveChanges();

            return new ResponseDto { Id = proformaInvoiceHeaderId, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.ProformaInvoice, GenericMessageData.UpdatedSuccessWithCode) };
        }

        public async Task<ResponseDto> SaveProformaInvoiceHeader(ProformaInvoiceHeaderDto proformaInvoiceHeader, bool hasApprove, bool approved, int? requestId, bool shouldValidate, string? documentReference, int documentStatusId, bool shouldInitializeFlags)
		{
            var separateYears = await _applicationSettingService.SeparateYears(proformaInvoiceHeader.StoreId);

            if (hasApprove)
            {
                if (proformaInvoiceHeader.ProformaInvoiceHeaderId == 0)
                {
                    return await CreateProformaInvoiceHeader(proformaInvoiceHeader, hasApprove, approved, requestId, documentReference, documentStatusId, shouldInitializeFlags, separateYears);
                }
                else
                {
                    return await UpdateProformaInvoiceHeader(proformaInvoiceHeader, shouldValidate);
                }
            }
            else
            {
                var proformaInvoiceHeaderExist = await IsProformaInvoiceCodeExist(proformaInvoiceHeader.ProformaInvoiceHeaderId, proformaInvoiceHeader.DocumentCode, proformaInvoiceHeader.StoreId, separateYears, proformaInvoiceHeader.DocumentDate, proformaInvoiceHeader.Prefix, proformaInvoiceHeader.Suffix);
                if (proformaInvoiceHeaderExist.Success)
                {
                    var nextProformaInvoiceCode = await GetNextProformaInvoiceCode(proformaInvoiceHeader.StoreId, separateYears, proformaInvoiceHeader.DocumentDate, proformaInvoiceHeader.Prefix, proformaInvoiceHeader.Suffix);
                    return new ResponseDto() { Id = nextProformaInvoiceCode, ResponseType = ResponseTypeData.Confirm, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.ProformaInvoice, GenericMessageData.CodeAlreadyExist, $"{nextProformaInvoiceCode}") };
                }
                else
                {
                    if (proformaInvoiceHeader.ProformaInvoiceHeaderId == 0)
                    {
                        return await CreateProformaInvoiceHeader(proformaInvoiceHeader, hasApprove, approved, requestId, documentReference, documentStatusId, shouldInitializeFlags, separateYears);
                    }
                    else
                    {
                        return await UpdateProformaInvoiceHeader(proformaInvoiceHeader, shouldValidate);
                    }
                }
            }
        }

        public async Task<ResponseDto> CreateProformaInvoiceHeader(ProformaInvoiceHeaderDto proformaInvoiceHeader, bool hasApprove, bool approved, int? requestId, string? documentReference, int documentStatusId, bool shouldInitializeFlags, bool separateYears)
        {
            int proformaInvoiceCode;
            string? prefix;
            string? suffix;
            var nextProformaInvoiceCode = await GetProformaInvoiceCodeInternal(proformaInvoiceHeader.StoreId, separateYears, proformaInvoiceHeader.DocumentDate);
            if (hasApprove && approved)
            {
                proformaInvoiceCode = nextProformaInvoiceCode.NextCode;
                prefix = nextProformaInvoiceCode.Prefix;
                suffix = nextProformaInvoiceCode.Suffix;
            }
            else
            {
                proformaInvoiceCode = proformaInvoiceHeader.DocumentCode != 0 ? proformaInvoiceHeader.DocumentCode : nextProformaInvoiceCode.NextCode;
                prefix = string.IsNullOrWhiteSpace(proformaInvoiceHeader.Prefix) ? nextProformaInvoiceCode.Prefix : proformaInvoiceHeader.Prefix;
                suffix = string.IsNullOrWhiteSpace(proformaInvoiceHeader.Suffix) ? nextProformaInvoiceCode.Suffix : proformaInvoiceHeader.Suffix;
            }

            var proformaInvoiceHeaderId = await GetNextId();
            var newProformaInvoiceHeader = new ProformaInvoiceHeader()
            {
                ProformaInvoiceHeaderId = proformaInvoiceHeaderId,
                Prefix = prefix,
                DocumentCode = proformaInvoiceCode,
                Suffix = suffix,
                ClientQuotationApprovalHeaderId = proformaInvoiceHeader.ClientQuotationApprovalHeaderId,
                DocumentDate = proformaInvoiceHeader.DocumentDate,
                EntryDate = DateHelper.GetDateTimeNow(),
                DocumentReference = documentReference != null ? documentReference : (hasApprove ? $"{DocumentReferenceData.Approval}{requestId}" : $"{DocumentReferenceData.ProformaInvoice}{proformaInvoiceHeaderId}"),
                ClientId = proformaInvoiceHeader.ClientId,
                SellerId = proformaInvoiceHeader.SellerId,
                StoreId = proformaInvoiceHeader.StoreId,
                Reference = proformaInvoiceHeader.Reference,
                CreditPayment = proformaInvoiceHeader.CreditPayment,
                TaxTypeId = proformaInvoiceHeader.TaxTypeId,
                ShippingDate = proformaInvoiceHeader.ShippingDate,
                DeliveryDate = proformaInvoiceHeader.DeliveryDate,
                DueDate = proformaInvoiceHeader.DueDate,
                ShipmentTypeId = proformaInvoiceHeader.ShipmentTypeId,
                ClientName = proformaInvoiceHeader.CashClientName,
                ClientPhone = proformaInvoiceHeader.ClientPhone,
                ClientAddress = proformaInvoiceHeader.ClientAddress,
                ClientTaxCode = proformaInvoiceHeader.ClientTaxCode,
                DriverName = proformaInvoiceHeader.DriverName,
                DriverPhone = proformaInvoiceHeader.DriverPhone,
                ClientResponsibleName = proformaInvoiceHeader.ClientResponsibleName,
                ClientResponsiblePhone = proformaInvoiceHeader.ClientResponsiblePhone,
                ShipTo = proformaInvoiceHeader.ShipTo,
                BillTo = proformaInvoiceHeader.BillTo,
                ShippingRemarks = proformaInvoiceHeader.ShippingRemarks,
                TotalValue = proformaInvoiceHeader.TotalValue,
                DiscountPercent = proformaInvoiceHeader.DiscountPercent,
                DiscountValue = proformaInvoiceHeader.DiscountValue,
                TotalItemDiscount = proformaInvoiceHeader.TotalItemDiscount,
                GrossValue = proformaInvoiceHeader.GrossValue,
                VatValue = proformaInvoiceHeader.VatValue,
                SubNetValue = proformaInvoiceHeader.SubNetValue,
                OtherTaxValue = proformaInvoiceHeader.OtherTaxValue,
                NetValueBeforeAdditionalDiscount = proformaInvoiceHeader.NetValueBeforeAdditionalDiscount,
                AdditionalDiscountValue = proformaInvoiceHeader.AdditionalDiscountValue,
                NetValue = proformaInvoiceHeader.NetValue,
                TotalCostValue = proformaInvoiceHeader.TotalCostValue,
                RemarksAr = proformaInvoiceHeader.RemarksAr,
                RemarksEn = proformaInvoiceHeader.RemarksEn,
                IsClosed = shouldInitializeFlags ? proformaInvoiceHeader.IsClosed : false,
                IsCancelled = proformaInvoiceHeader.IsCancelled,
                IsEnded = shouldInitializeFlags ? proformaInvoiceHeader.IsEnded : false,
                IsBlocked = shouldInitializeFlags ? proformaInvoiceHeader.IsBlocked : false,
                DocumentStatusId = documentStatusId,
                ShippingStatusId = proformaInvoiceHeader.ShippingStatusId,
                ArchiveHeaderId = proformaInvoiceHeader.ArchiveHeaderId,


                CreatedAt = DateHelper.GetDateTimeNow(),
                UserNameCreated = await _httpContextAccessor!.GetUserName(),
                IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
                Hide = false,
            };

            var proformaInvoiceHeaderValidator = await new ProformaInvoiceHeaderValidator(_localizer).ValidateAsync(newProformaInvoiceHeader);
            var validationResult = proformaInvoiceHeaderValidator.IsValid;
            if (validationResult)
            {
                await _repository.Insert(newProformaInvoiceHeader);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = newProformaInvoiceHeader.ProformaInvoiceHeaderId, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.ProformaInvoice, GenericMessageData.CreatedSuccessWithCode, $"{newProformaInvoiceHeader.Prefix}{newProformaInvoiceHeader.DocumentCode}{newProformaInvoiceHeader.Suffix}") };
            }
            else
            {
                return new ResponseDto() { Id = newProformaInvoiceHeader.ProformaInvoiceHeaderId, Success = false, Message = proformaInvoiceHeaderValidator.ToString("~") };
            }
        }

        public async Task<ResponseDto> UpdateProformaInvoiceHeader(ProformaInvoiceHeaderDto proformaInvoiceHeader, bool shouldValidate)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var proformaInvoiceHeaderDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.ProformaInvoiceHeaderId == proformaInvoiceHeader.ProformaInvoiceHeaderId);
            if (proformaInvoiceHeaderDb != null)
            {
                if (shouldValidate)
                {
                    if (proformaInvoiceHeaderDb.IsClosed)
                    {
                        return new ResponseDto() { Id = proformaInvoiceHeader.ProformaInvoiceHeaderId, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.ProformaInvoice, GenericMessageData.CannotUpdateBecauseClosed) };
                    }

                    if (proformaInvoiceHeaderDb.IsBlocked)
                    {
                        return new ResponseDto() { Id = proformaInvoiceHeader.ProformaInvoiceHeaderId, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.ProformaInvoice, GenericMessageData.CannotPerformOperationBecauseStoppedDealingOn) };
                    }
                }

                proformaInvoiceHeaderDb.ClientQuotationApprovalHeaderId = proformaInvoiceHeader.ClientQuotationApprovalHeaderId;
                proformaInvoiceHeaderDb.DocumentDate = proformaInvoiceHeader.DocumentDate;
                proformaInvoiceHeaderDb.ClientId = proformaInvoiceHeader.ClientId;
                proformaInvoiceHeaderDb.SellerId = proformaInvoiceHeader.SellerId;
                proformaInvoiceHeaderDb.StoreId = proformaInvoiceHeader.StoreId;
                proformaInvoiceHeaderDb.Reference = proformaInvoiceHeader.Reference;
                proformaInvoiceHeaderDb.CreditPayment = proformaInvoiceHeader.CreditPayment;
                proformaInvoiceHeaderDb.TaxTypeId = proformaInvoiceHeader.TaxTypeId;
                proformaInvoiceHeaderDb.ShippingDate = proformaInvoiceHeader.ShippingDate;
                proformaInvoiceHeaderDb.DeliveryDate = proformaInvoiceHeader.DeliveryDate;
                proformaInvoiceHeaderDb.DueDate = proformaInvoiceHeader.DueDate;
                proformaInvoiceHeaderDb.ShipmentTypeId = proformaInvoiceHeader.ShipmentTypeId;
                proformaInvoiceHeaderDb.ClientName = proformaInvoiceHeader.CashClientName;
                proformaInvoiceHeaderDb.ClientPhone = proformaInvoiceHeader.ClientPhone;
                proformaInvoiceHeaderDb.ClientAddress = proformaInvoiceHeader.ClientAddress;
                proformaInvoiceHeaderDb.ClientTaxCode = proformaInvoiceHeader.ClientTaxCode;
                proformaInvoiceHeaderDb.DriverName = proformaInvoiceHeader.DriverName;
                proformaInvoiceHeaderDb.DriverPhone = proformaInvoiceHeader.DriverPhone;
                proformaInvoiceHeaderDb.ClientResponsibleName = proformaInvoiceHeader.ClientResponsibleName;
                proformaInvoiceHeaderDb.ClientResponsiblePhone = proformaInvoiceHeader.ClientResponsiblePhone;
                proformaInvoiceHeaderDb.ShipTo = proformaInvoiceHeader.ShipTo;
                proformaInvoiceHeaderDb.BillTo = proformaInvoiceHeader.BillTo;
                proformaInvoiceHeaderDb.ShippingRemarks = proformaInvoiceHeader.ShippingRemarks;
                proformaInvoiceHeaderDb.TotalValue = proformaInvoiceHeader.TotalValue;
                proformaInvoiceHeaderDb.DiscountPercent = proformaInvoiceHeader.DiscountPercent;
                proformaInvoiceHeaderDb.DiscountValue = proformaInvoiceHeader.DiscountValue;
                proformaInvoiceHeaderDb.TotalItemDiscount = proformaInvoiceHeader.TotalItemDiscount;
                proformaInvoiceHeaderDb.GrossValue = proformaInvoiceHeader.GrossValue;
                proformaInvoiceHeaderDb.VatValue = proformaInvoiceHeader.VatValue;
                proformaInvoiceHeaderDb.SubNetValue = proformaInvoiceHeader.SubNetValue;
                proformaInvoiceHeaderDb.OtherTaxValue = proformaInvoiceHeader.OtherTaxValue;
                proformaInvoiceHeaderDb.NetValueBeforeAdditionalDiscount = proformaInvoiceHeader.NetValueBeforeAdditionalDiscount;
                proformaInvoiceHeaderDb.AdditionalDiscountValue = proformaInvoiceHeader.AdditionalDiscountValue;
                proformaInvoiceHeaderDb.NetValue = proformaInvoiceHeader.NetValue;
                proformaInvoiceHeaderDb.TotalCostValue = proformaInvoiceHeader.TotalCostValue;
                proformaInvoiceHeaderDb.RemarksAr = proformaInvoiceHeader.RemarksAr;
                proformaInvoiceHeaderDb.RemarksEn = proformaInvoiceHeader.RemarksEn;
                proformaInvoiceHeaderDb.IsCancelled = proformaInvoiceHeader.IsCancelled;
                proformaInvoiceHeaderDb.ArchiveHeaderId = proformaInvoiceHeader.ArchiveHeaderId;
                proformaInvoiceHeaderDb.ShippingStatusId = proformaInvoiceHeader.ShippingStatusId;
                
                proformaInvoiceHeaderDb.ModifiedAt = DateHelper.GetDateTimeNow();
                proformaInvoiceHeaderDb.UserNameModified = await _httpContextAccessor!.GetUserName();
                proformaInvoiceHeaderDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();


                var proformaInvoiceHeaderValidator = await new ProformaInvoiceHeaderValidator(_localizer).ValidateAsync(proformaInvoiceHeaderDb);
                var validationResult = proformaInvoiceHeaderValidator.IsValid;
                if (validationResult)
                {
                    _repository.Update(proformaInvoiceHeaderDb);
                    await _repository.SaveChanges();
                    return new ResponseDto() { Id = proformaInvoiceHeaderDb.ProformaInvoiceHeaderId, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.ProformaInvoice, GenericMessageData.UpdatedSuccessWithCode, $"{proformaInvoiceHeaderDb.Prefix}{proformaInvoiceHeaderDb.DocumentCode}{proformaInvoiceHeaderDb.Suffix}") };
                }
                else
                {
                    return new ResponseDto() { Id = 0, Success = false, Message = proformaInvoiceHeaderValidator.ToString("~") };
                }
            }
            return new ResponseDto() { Id = 0, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.ProformaInvoice, GenericMessageData.NotFound) };
        }

        public async Task<ResponseDto> IsProformaInvoiceCodeExist(int proformaInvoiceHeaderId, int proformaInvoiceCode, int storeId, bool separateYears, DateTime documentDate, string? prefix, string? suffix)
		{
			var proformaInvoiceHeader = await _repository.GetAll().FirstOrDefaultAsync(x => x.ProformaInvoiceHeaderId != proformaInvoiceHeaderId && (x.StoreId == storeId && (!separateYears || x.DocumentDate.Year == documentDate.Year) && x.Prefix == prefix && x.DocumentCode == proformaInvoiceCode && x.Suffix == suffix));
			if (proformaInvoiceHeader is not null)
			{
                return new ResponseDto() { Id = proformaInvoiceHeader.ProformaInvoiceHeaderId, Success = true };
            }
			return new ResponseDto() {  Id = 0, Success = false };
		}

        public async Task<int> GetNextId()
        {
            int id = 1;
            try { id = await _repository.GetAll().MaxAsync(a => a.ProformaInvoiceHeaderId) + 1; } catch { id = 1; }
            return id;
        }

        public async Task<ResponseDto> DeleteProformaInvoiceHeader(int id)
		{
            var proformaInvoiceHeader = await _repository.GetAll().FirstOrDefaultAsync(x => x.ProformaInvoiceHeaderId == id);
            if (proformaInvoiceHeader != null)
            {
                _repository.Delete(proformaInvoiceHeader);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = id, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.ProformaInvoice, GenericMessageData.DeleteSuccessWithCode, $"{proformaInvoiceHeader.Prefix}{proformaInvoiceHeader.DocumentCode}{proformaInvoiceHeader.Suffix}") };
            }
            return new ResponseDto() { Id = id, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.ProformaInvoice, GenericMessageData.NotFound) };
        }

        public async Task<int> GetNextProformaInvoiceCode(int storeId, bool separateYears, DateTime documentDate, string? prefix, string? suffix)
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
