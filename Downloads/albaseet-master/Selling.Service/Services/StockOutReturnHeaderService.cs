using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Sales.CoreOne.Contracts;
using Sales.CoreOne.Models.Domain;
using Sales.CoreOne.Models.Dtos.ViewModels;
using Sales.Service.Validators;
using Shared.CoreOne.Contracts.Menus;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.StaticData;
using Shared.CoreOne;
using Shared.Helper.Models.Dtos;
using Shared.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.CoreOne.Contracts.Settings;

namespace Sales.Service.Services
{
    public class StockOutReturnHeaderService: BaseService<StockOutReturnHeader>, IStockOutReturnHeaderService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStoreService _storeService;
        private readonly IClientService _clientService;
        private readonly IStringLocalizer<StockOutReturnHeaderService> _localizer;
        private readonly IMenuEncodingService _menuEncodingService;
        private readonly IStockOutHeaderService _stockOutHeaderService;
        private readonly ISalesInvoiceHeaderService _salesInvoiceHeaderService;
        private readonly IProformaInvoiceHeaderService _proformaInvoiceHeaderService;
        private readonly IGenericMessageService _genericMessageService;
		private readonly IMenuService _menuService;
        private readonly ISellerService _sellerService;
        private readonly IApplicationSettingService _applicationSettingService;

		public StockOutReturnHeaderService(IRepository<StockOutReturnHeader> repository, IHttpContextAccessor httpContextAccessor, IStoreService storeService, IStringLocalizer<StockOutReturnHeaderService> localizer, IMenuEncodingService menuEncodingService, IClientService clientService, IStockOutHeaderService stockOutHeaderService, ISalesInvoiceHeaderService salesInvoiceHeaderService, IProformaInvoiceHeaderService proformaInvoiceHeaderService, IGenericMessageService genericMessageService, IMenuService menuService, ISellerService sellerService, IApplicationSettingService applicationSettingService) : base(repository)
        {
            _httpContextAccessor = httpContextAccessor;
            _storeService = storeService;
            _clientService = clientService;
            _localizer = localizer;
            _menuEncodingService = menuEncodingService;
            _stockOutHeaderService = stockOutHeaderService;
            _salesInvoiceHeaderService = salesInvoiceHeaderService;
            _proformaInvoiceHeaderService = proformaInvoiceHeaderService;
            _genericMessageService = genericMessageService;
            _menuService = menuService;
			_sellerService = sellerService;
            _applicationSettingService = applicationSettingService;
		}

        public IQueryable<StockOutReturnHeaderDto> GetStockOutReturnHeaders()
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();
            var data = from stockOutReturnHeader in _repository.GetAll()
                       from store in _storeService.GetAll().Where(x => x.StoreId == stockOutReturnHeader.StoreId)
                       from client in _clientService.GetAll().Where(x => x.ClientId == stockOutReturnHeader.ClientId)
                       from stockOutHeader in _stockOutHeaderService.GetAll().Where(x => x.StockOutHeaderId == stockOutReturnHeader.StockOutHeaderId).DefaultIfEmpty()
                       from stockOutProformaInvoiceHeader in _proformaInvoiceHeaderService.GetAll().Where(x => x.ProformaInvoiceHeaderId == stockOutHeader.ProformaInvoiceHeaderId).DefaultIfEmpty()
                       from stockOutSalesInvoiceHeader in _salesInvoiceHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == stockOutHeader.SalesInvoiceHeaderId).DefaultIfEmpty()
                       from salesInvoiceHeader in _salesInvoiceHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == stockOutReturnHeader.SalesInvoiceHeaderId).DefaultIfEmpty()
					   from menu in _menuService.GetAll().Where(x => x.MenuCode == stockOutReturnHeader.MenuCode).DefaultIfEmpty()
                       from seller in _sellerService.GetAll().Where(x => x.SellerId == stockOutReturnHeader.SellerId).DefaultIfEmpty()
					   select new StockOutReturnHeaderDto()
                       {
                           StockOutReturnHeaderId = stockOutReturnHeader.StockOutReturnHeaderId,
                           Prefix = stockOutReturnHeader.Prefix,
                           DocumentCode = stockOutReturnHeader.DocumentCode,
                           Suffix = stockOutReturnHeader.Suffix,
                           DocumentFullCode = $"{stockOutReturnHeader.Prefix}{stockOutReturnHeader.DocumentCode}{stockOutReturnHeader.Suffix}",
                           DocumentReference = stockOutReturnHeader.DocumentReference,
                           StockTypeId = stockOutReturnHeader.StockTypeId,
                           StockOutHeaderId = stockOutReturnHeader.StockOutHeaderId,
                           StockOutFullCode = stockOutHeader != null ? $"{stockOutHeader.Prefix}{stockOutHeader.DocumentCode}{stockOutHeader.Suffix}" : null,
                           StockOutDocumentReference = stockOutHeader != null ? stockOutHeader.DocumentReference : null,
                           StockOutProformaInvoiceHeaderId = stockOutHeader != null ? stockOutHeader.ProformaInvoiceHeaderId : null,
                           StockOutProformaInvoiceFullCode = stockOutProformaInvoiceHeader != null ? $"{stockOutProformaInvoiceHeader.Prefix}{stockOutProformaInvoiceHeader.DocumentCode}{stockOutProformaInvoiceHeader.Suffix}" : null,
                           StockOutProformaInvoiceDocumentReference = stockOutProformaInvoiceHeader != null ? stockOutProformaInvoiceHeader.DocumentReference : null,
                           StockOutSalesInvoiceHeaderId = stockOutHeader != null ? stockOutHeader.SalesInvoiceHeaderId : null,
                           StockOutSalesInvoiceFullCode = stockOutSalesInvoiceHeader != null ? $"{stockOutSalesInvoiceHeader.Prefix}{stockOutSalesInvoiceHeader.DocumentCode}{stockOutSalesInvoiceHeader.Suffix}" : null,
                           StockOutSalesInvoiceDocumentReference = stockOutSalesInvoiceHeader != null ? stockOutSalesInvoiceHeader.DocumentReference : null,
                           SalesInvoiceHeaderId = stockOutReturnHeader.SalesInvoiceHeaderId,
                           SalesInvoiceFullCode = salesInvoiceHeader != null ? $"{salesInvoiceHeader.Prefix}{salesInvoiceHeader.DocumentCode}{salesInvoiceHeader.Suffix}" : null,
                           SalesInvoiceDocumentReference = salesInvoiceHeader != null ? salesInvoiceHeader.DocumentReference : null,
                           ClientId = stockOutReturnHeader.ClientId,
                           ClientCode = client.ClientCode,
                           ClientName = language == LanguageCode.Arabic ? client.ClientNameAr : client.ClientNameEn,
						   SellerId = seller != null ? seller.SellerId : null,
                           SellerCode = seller != null ? seller.SellerCode : null,
						   SellerName = seller != null ? language == LanguageCode.Arabic ? seller.SellerNameAr : seller.SellerNameEn : null,
						   StoreId = stockOutReturnHeader.StoreId,
                           StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
                           DocumentDate = stockOutReturnHeader.DocumentDate,
                           EntryDate = stockOutReturnHeader.EntryDate,
                           Reference = stockOutReturnHeader.Reference,
                           TotalValue = stockOutReturnHeader.TotalValue,
                           DiscountPercent = stockOutReturnHeader.DiscountPercent,
                           DiscountValue = stockOutReturnHeader.DiscountValue,
                           TotalItemDiscount = stockOutReturnHeader.TotalItemDiscount,
                           GrossValue = stockOutReturnHeader.GrossValue,
                           VatValue = stockOutReturnHeader.VatValue,
                           SubNetValue = stockOutReturnHeader.SubNetValue,
                           OtherTaxValue = stockOutReturnHeader.OtherTaxValue,
                           NetValueBeforeAdditionalDiscount = stockOutReturnHeader.NetValueBeforeAdditionalDiscount,
                           AdditionalDiscountValue = stockOutReturnHeader.AdditionalDiscountValue,
                           NetValue = stockOutReturnHeader.NetValue,
                           TotalCostValue = stockOutReturnHeader.TotalCostValue,
                           RemarksAr = stockOutReturnHeader.RemarksAr,
                           RemarksEn = stockOutReturnHeader.RemarksEn,
                           IsClosed = stockOutReturnHeader.IsClosed || stockOutReturnHeader.IsEnded || stockOutReturnHeader.IsBlocked,
                           IsEnded = stockOutReturnHeader.IsEnded,
                           IsBlocked = stockOutReturnHeader.IsBlocked,
                           ArchiveHeaderId = stockOutReturnHeader.ArchiveHeaderId,
						   MenuCode = stockOutReturnHeader.MenuCode,
						   MenuName = menu != null ? language == LanguageCode.Arabic ? menu.MenuNameAr : menu.MenuNameEn : null,

						   CreatedAt = stockOutReturnHeader.CreatedAt,
                           UserNameCreated = stockOutReturnHeader.UserNameCreated,
                           IpAddressCreated = stockOutReturnHeader.IpAddressCreated,

                           ModifiedAt = stockOutReturnHeader.ModifiedAt,
                           UserNameModified = stockOutReturnHeader.UserNameModified,
                           IpAddressModified = stockOutReturnHeader.IpAddressModified
                       };
            return data;
        }

        public IQueryable<StockOutReturnHeaderDto> GetUserStockOutReturnHeaders(int stockTypeId)
        {
            var userStore = _httpContextAccessor.GetCurrentUserStore();
            return GetStockOutReturnHeaders().Where(x => x.StockTypeId == stockTypeId && x.StoreId == userStore);
        }

        public IQueryable<StockOutReturnHeaderDto> GetStockOutReturnHeadersByStoreId(int storeId, int? clientId, int stockTypeId, int stockOutReturnHeaderId)
        {
            clientId ??= 0;
			if (stockOutReturnHeaderId == 0)
			{
				return GetStockOutReturnHeaders().Where(x => x.StoreId == storeId && (clientId == 0 || x.ClientId == clientId) && x.StockTypeId == stockTypeId && x.IsEnded == false && x.IsBlocked == false);
			}
			else
			{
				return GetStockOutReturnHeaders().Where(x => x.StockOutReturnHeaderId == stockOutReturnHeaderId);
			}
		}

        public async Task<StockOutReturnHeaderDto?> GetStockOutReturnHeaderById(int id)
        {
            return await GetStockOutReturnHeaders().FirstOrDefaultAsync(x => x.StockOutReturnHeaderId == id);
        }

        public async Task<DocumentCodeDto> GetStockOutReturnCode(int storeId, DateTime documentDate, int stockTypeId)
        {
            var separateYears = await _applicationSettingService.SeparateYears(storeId);
            return await GetStockOutReturnCodeInternal(storeId, separateYears, documentDate, stockTypeId);
        }

        public async Task<DocumentCodeDto> GetStockOutReturnCodeInternal(int storeId, bool separateYears, DateTime documentDate, int stockTypeId)
        {
            var encoding = await _menuEncodingService.GetMenuEncoding(storeId, StockTypeData.ToMenuCode((byte)stockTypeId));
            var code = await GetNextStockOutReturnCode(storeId, separateYears, documentDate, stockTypeId, encoding.Prefix, encoding.Suffix);
            return new DocumentCodeDto() { NextCode = code, Prefix = encoding.Prefix, Suffix = encoding.Suffix };
        }

		public async Task<bool> UpdateAllStockOutReturnsBlockedFromProformaInvoice(int proformaInvoiceHeaderId, bool isBlocked)
		{
			var headerList = new List<StockOutReturnHeader>();

			//SR->SO->PFI
			var stockOutReturnsDirectlyThroughStockOut = await (
					from stockOutReturnHeader in _repository.GetAll()
					from stockOutHeader in _stockOutHeaderService.GetAll().Where(x => x.StockOutHeaderId == stockOutReturnHeader.StockOutHeaderId && x.ProformaInvoiceHeaderId == proformaInvoiceHeaderId)
					select stockOutReturnHeader
				).ToListAsync();

			headerList.AddRange(stockOutReturnsDirectlyThroughStockOut);

			//SR->SO->SI->PFI
			var stockOutReturnThroughStockOutThroughSalesInvoice = await (
				from stockOutReturnHeader in _repository.GetAll()
				from stockOutHeader in _stockOutHeaderService.GetAll().Where(x => x.StockOutHeaderId == stockOutReturnHeader.StockOutHeaderId)
				from salesInvoiceHeader in _salesInvoiceHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == stockOutHeader.SalesInvoiceHeaderId && x.ProformaInvoiceHeaderId == proformaInvoiceHeaderId)
				select stockOutReturnHeader
			).ToListAsync();

			headerList.AddRange(stockOutReturnThroughStockOutThroughSalesInvoice);

			//SR->SI->PFI
			var stockOutReturnDirectlyThroughSalesInvoice = await (
				from stockOutReturnHeader in _repository.GetAll()
				from salesInvoiceHeader in _salesInvoiceHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == stockOutReturnHeader.SalesInvoiceHeaderId && x.ProformaInvoiceHeaderId == proformaInvoiceHeaderId)
				select stockOutReturnHeader
			).ToListAsync();

			headerList.AddRange(stockOutReturnDirectlyThroughSalesInvoice);

			foreach (var header in headerList)
			{
				header.IsBlocked = isBlocked;
			}

            _repository.UpdateRange(headerList);
			await _repository.SaveChanges();
			return true;
		}

		public async Task<int> UpdateAllStockOutReturnReservationFromSalesInvoice(int salesInvoiceHeaderId, bool isEnded)
		{
			var headerList = new List<StockOutReturnHeader>();

			//SR->SI->PO
			var stockOutReturnsDirectlyThroughStockOut = await (
					from stockOutReturnHeader in _repository.GetAll()
					from stockOutHeader in _stockOutHeaderService.GetAll().Where(x => x.StockOutHeaderId == stockOutReturnHeader.StockOutHeaderId && x.SalesInvoiceHeaderId == salesInvoiceHeaderId)
					select stockOutReturnHeader
				).ToListAsync();

			headerList.AddRange(stockOutReturnsDirectlyThroughStockOut);

			foreach (var header in headerList)
			{
				header.IsEnded = isEnded;
			}

            _repository.UpdateRange(headerList);
			await _repository.SaveChanges();
			return headerList.Count;
		}

		public async Task<int> UpdateAllStockOutReturnsEndedDirectlyFromSalesInvoice(int salesInvoiceHeaderId, bool isEnded)
		{
			var headerList = new List<StockOutReturnHeader>();

			var stockOutReturnsRelatedToSalesInvoice = await _repository.FindBy(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId).ToListAsync();
			headerList.AddRange(stockOutReturnsRelatedToSalesInvoice);

			foreach (var header in headerList)
			{
				header.IsEnded = isEnded;
			}

            _repository.UpdateRange(headerList);
			await _repository.SaveChanges();
			return headerList.Count;
		}

		public async Task<bool> UpdateStockOutReturnsEnded(List<int> stockOutReturnHeaderIds, bool isEnded)
		{
			var stockOutReturns = _repository.GetAll().Where(x => stockOutReturnHeaderIds.Contains(x.StockOutReturnHeaderId));

			foreach (var header in stockOutReturns)
			{
				header.IsEnded = isEnded;
			}

            _repository.UpdateRange(stockOutReturns);
			await _repository.SaveChanges();
			return true;
		}

		public async Task<ResponseDto> SaveStockOutReturnHeader(StockOutReturnHeaderDto stockOutReturnHeader, bool hasApprove, bool approved, int? requestId, string? documentReference, bool shouldInitializeFlags)
        {
            var separateYears = await _applicationSettingService.SeparateYears(stockOutReturnHeader.StoreId);

            if (hasApprove)
            {
                if (stockOutReturnHeader.StockOutReturnHeaderId == 0)
                {
                    return await CreateStockOutReturnHeader(stockOutReturnHeader, hasApprove, approved, requestId, documentReference, shouldInitializeFlags, separateYears);
                }
                else
                {
                    return await UpdateStockOutReturnHeader(stockOutReturnHeader);
                }
            }
            else
            {
                var stockOutReturnHeaderExist = await IsStockOutReturnCodeExist(stockOutReturnHeader.StockOutReturnHeaderId, stockOutReturnHeader.DocumentCode, stockOutReturnHeader.StoreId, separateYears, stockOutReturnHeader.DocumentDate, stockOutReturnHeader.StockTypeId, stockOutReturnHeader.Prefix, stockOutReturnHeader.Suffix);
                if (stockOutReturnHeaderExist.Success)
                {
                    var nextStockOutReturnCode = await GetNextStockOutReturnCode(stockOutReturnHeader.StoreId, separateYears, stockOutReturnHeader.DocumentDate, stockOutReturnHeader.StockTypeId, stockOutReturnHeader.Prefix, stockOutReturnHeader.Suffix);
                    return new ResponseDto() { Id = nextStockOutReturnCode, ResponseType = ResponseTypeData.Confirm, Success = false, Message = await _genericMessageService.GetMessage(StockTypeData.ToMenuCode(stockOutReturnHeader.StockTypeId), GenericMessageData.CodeAlreadyExist, $"{nextStockOutReturnCode}") };
                }
                else
                {
                    if (stockOutReturnHeader.StockOutReturnHeaderId == 0)
                    {
                        return await CreateStockOutReturnHeader(stockOutReturnHeader, hasApprove, approved, requestId, documentReference, shouldInitializeFlags, separateYears);
                    }
                    else
                    {
                        return await UpdateStockOutReturnHeader(stockOutReturnHeader);
                    }
                }
            }
        }

        public async Task<ResponseDto> CreateStockOutReturnHeader(StockOutReturnHeaderDto stockOutReturnHeader, bool hasApprove, bool approved, int? requestId, string? documentReference, bool shouldInitializeFlags, bool separateYears)
        {
            int stockOutReturnCode;
            string? prefix;
            string? suffix;
            var nextStockOutReturnCode = await GetStockOutReturnCodeInternal(stockOutReturnHeader.StoreId, separateYears, stockOutReturnHeader.DocumentDate, stockOutReturnHeader.StockTypeId);
            if (hasApprove && approved)
            {
                stockOutReturnCode = nextStockOutReturnCode.NextCode;
                prefix = nextStockOutReturnCode.Prefix;
                suffix = nextStockOutReturnCode.Suffix;
            }
            else
            {
                stockOutReturnCode = stockOutReturnHeader.DocumentCode != 0 ? stockOutReturnHeader.DocumentCode : nextStockOutReturnCode.NextCode;
                prefix = string.IsNullOrWhiteSpace(stockOutReturnHeader.Prefix) ? nextStockOutReturnCode.Prefix : stockOutReturnHeader.Prefix;
                suffix = string.IsNullOrWhiteSpace(stockOutReturnHeader.Suffix) ? nextStockOutReturnCode.Suffix : stockOutReturnHeader.Suffix;
            }

            var stockOutReturnHeaderId = await GetNextId();
            var newStockOutReturnHeader = new StockOutReturnHeader()
            {
                StockOutReturnHeaderId = stockOutReturnHeaderId,
                Prefix = prefix,
                DocumentCode = stockOutReturnCode,
                Suffix = suffix,
                DocumentReference = documentReference != null ? documentReference : (hasApprove ? $"{DocumentReferenceData.Approval}{requestId}" : $"{StockTypeData.ToDocumentReference(stockOutReturnHeader.StockTypeId)}{stockOutReturnHeaderId}"),
                StockTypeId = stockOutReturnHeader.StockTypeId,
                StockOutHeaderId = stockOutReturnHeader.StockOutHeaderId,
                SalesInvoiceHeaderId = stockOutReturnHeader.SalesInvoiceHeaderId,
                ClientId = stockOutReturnHeader.ClientId,
                SellerId = stockOutReturnHeader.SellerId,
                StoreId = stockOutReturnHeader.StoreId,
                DocumentDate = stockOutReturnHeader.DocumentDate,
                EntryDate = DateHelper.GetDateTimeNow(),
                Reference = stockOutReturnHeader.Reference,
                TotalValue = stockOutReturnHeader.TotalValue,
                DiscountPercent = stockOutReturnHeader.DiscountPercent,
                DiscountValue = stockOutReturnHeader.DiscountValue,
                TotalItemDiscount = stockOutReturnHeader.TotalItemDiscount,
                GrossValue = stockOutReturnHeader.GrossValue,
                VatValue = stockOutReturnHeader.VatValue,
                SubNetValue = stockOutReturnHeader.SubNetValue,
                OtherTaxValue = stockOutReturnHeader.OtherTaxValue,
                NetValueBeforeAdditionalDiscount = stockOutReturnHeader.NetValueBeforeAdditionalDiscount,
                AdditionalDiscountValue = stockOutReturnHeader.AdditionalDiscountValue,
                NetValue = stockOutReturnHeader.NetValue,
                TotalCostValue = stockOutReturnHeader.TotalCostValue,
                RemarksAr = stockOutReturnHeader.RemarksAr,
                RemarksEn = stockOutReturnHeader.RemarksEn,
                IsClosed = shouldInitializeFlags ? stockOutReturnHeader.IsClosed : false,
                IsEnded = shouldInitializeFlags ? stockOutReturnHeader.IsEnded : false,
                IsBlocked = shouldInitializeFlags ? stockOutReturnHeader.IsBlocked : false,
                ArchiveHeaderId = stockOutReturnHeader.ArchiveHeaderId,
                MenuCode = StockTypeData.ToMenuCode(stockOutReturnHeader.StockTypeId),

                CreatedAt = DateHelper.GetDateTimeNow(),
                UserNameCreated = await _httpContextAccessor!.GetUserName(),
                IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
                Hide = false,
            };

            var stockOutReturnHeaderValidator = await new StockOutReturnHeaderValidator(_localizer).ValidateAsync(newStockOutReturnHeader);
            var validationResult = stockOutReturnHeaderValidator.IsValid;
            if (validationResult)
            {
                await _repository.Insert(newStockOutReturnHeader);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = newStockOutReturnHeader.StockOutReturnHeaderId, Success = true, Message = await _genericMessageService.GetMessage(StockTypeData.ToMenuCode(stockOutReturnHeader.StockTypeId), GenericMessageData.CreatedSuccessWithCode, $"{newStockOutReturnHeader.Prefix}{newStockOutReturnHeader.DocumentCode}{newStockOutReturnHeader.Suffix}") };
            }
            else
            {
                return new ResponseDto() { Id = newStockOutReturnHeader.StockOutReturnHeaderId, Success = false, Message = stockOutReturnHeaderValidator.ToString("~") };
            }
        }

        public async Task<ResponseDto> UpdateStockOutReturnHeader(StockOutReturnHeaderDto stockOutReturnHeader)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var stockOutReturnHeaderDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.StockOutReturnHeaderId == stockOutReturnHeader.StockOutReturnHeaderId);
            if (stockOutReturnHeaderDb != null)
            {
                stockOutReturnHeaderDb.StockOutHeaderId = stockOutReturnHeader.StockOutHeaderId;
                stockOutReturnHeaderDb.SalesInvoiceHeaderId = stockOutReturnHeader.SalesInvoiceHeaderId;
                stockOutReturnHeaderDb.ClientId = stockOutReturnHeader.ClientId;
                stockOutReturnHeaderDb.SellerId = stockOutReturnHeader.SellerId;
                stockOutReturnHeaderDb.StoreId = stockOutReturnHeader.StoreId;
                stockOutReturnHeaderDb.DocumentDate = stockOutReturnHeader.DocumentDate;
                stockOutReturnHeaderDb.Reference = stockOutReturnHeader.Reference;
                stockOutReturnHeaderDb.TotalValue = stockOutReturnHeader.TotalValue;
                stockOutReturnHeaderDb.DiscountPercent = stockOutReturnHeader.DiscountPercent;
                stockOutReturnHeaderDb.DiscountValue = stockOutReturnHeader.DiscountValue;
                stockOutReturnHeaderDb.TotalItemDiscount = stockOutReturnHeader.TotalItemDiscount;
                stockOutReturnHeaderDb.GrossValue = stockOutReturnHeader.GrossValue;
                stockOutReturnHeaderDb.VatValue = stockOutReturnHeader.VatValue;
                stockOutReturnHeaderDb.SubNetValue = stockOutReturnHeader.SubNetValue;
                stockOutReturnHeaderDb.OtherTaxValue = stockOutReturnHeader.OtherTaxValue;
                stockOutReturnHeaderDb.NetValueBeforeAdditionalDiscount = stockOutReturnHeader.NetValueBeforeAdditionalDiscount;
                stockOutReturnHeaderDb.AdditionalDiscountValue = stockOutReturnHeader.AdditionalDiscountValue;
                stockOutReturnHeaderDb.NetValue = stockOutReturnHeader.NetValue;
                stockOutReturnHeaderDb.TotalCostValue = stockOutReturnHeader.TotalCostValue;
                stockOutReturnHeaderDb.RemarksAr = stockOutReturnHeader.RemarksAr;
                stockOutReturnHeaderDb.RemarksEn = stockOutReturnHeader.RemarksEn;
                stockOutReturnHeaderDb.ArchiveHeaderId = stockOutReturnHeader.ArchiveHeaderId;

                stockOutReturnHeaderDb.ModifiedAt = DateHelper.GetDateTimeNow();
                stockOutReturnHeaderDb.UserNameModified = await _httpContextAccessor!.GetUserName();
                stockOutReturnHeaderDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();


                var stockOutReturnHeaderValidator = await new StockOutReturnHeaderValidator(_localizer).ValidateAsync(stockOutReturnHeaderDb);
                var validationResult = stockOutReturnHeaderValidator.IsValid;
                if (validationResult)
                {
                    _repository.Update(stockOutReturnHeaderDb);
                    await _repository.SaveChanges();
                    return new ResponseDto() { Id = stockOutReturnHeaderDb.StockOutReturnHeaderId, Success = true, Message = await _genericMessageService.GetMessage(StockTypeData.ToMenuCode(stockOutReturnHeaderDb.StockTypeId), GenericMessageData.UpdatedSuccessWithCode, $"{stockOutReturnHeaderDb.Prefix}{stockOutReturnHeaderDb.DocumentCode}{stockOutReturnHeaderDb.Suffix}") };
                }
                else
                {
                    return new ResponseDto() { Id = 0, Success = false, Message = stockOutReturnHeaderValidator.ToString("~") };
                }
            }
            return new ResponseDto() { Id = 0, Success = false, Message = await _genericMessageService.GetMessage(StockTypeData.ToMenuCode(stockOutReturnHeader.StockTypeId), GenericMessageData.NotFound) };
        }

        public async Task<ResponseDto> IsStockOutReturnCodeExist(int stockOutReturnHeaderId, int stockOutReturnCode, int storeId, bool separateYears, DateTime documentDate, int stockTypeId, string? prefix, string? suffix)
        {
            var stockOutReturnHeader = await _repository.GetAll().FirstOrDefaultAsync(x => x.StockOutReturnHeaderId != stockOutReturnHeaderId && (x.StoreId == storeId && (!separateYears || x.DocumentDate.Year == documentDate.Year) && x.StockTypeId == stockTypeId && x.Prefix == prefix && x.DocumentCode == stockOutReturnCode && x.Suffix == suffix));
            if (stockOutReturnHeader is not null)
            {
                return new ResponseDto() { Id = stockOutReturnHeader.StockOutReturnHeaderId, Success = true };
            }
            return new ResponseDto() { Id = 0, Success = false };
        }

        public async Task<int> GetNextId()
        {
            int id = 1;
            try { id = await _repository.GetAll().MaxAsync(a => a.StockOutReturnHeaderId) + 1; } catch { id = 1; }
            return id;
        }

        public async Task<ResponseDto> DeleteStockOutReturnHeader(int id, int menuCode)
        {
            var stockOutReturnHeader = await _repository.GetAll().FirstOrDefaultAsync(x => x.StockOutReturnHeaderId == id);
            if (stockOutReturnHeader != null)
            {
                _repository.Delete(stockOutReturnHeader);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = id, Success = true, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.DeleteSuccessWithCode, $"{stockOutReturnHeader.Prefix}{stockOutReturnHeader.DocumentCode}{stockOutReturnHeader.Suffix}") };
            }
            return new ResponseDto() { Id = id, Success = false, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.NotFound) };
        }

        public async Task<int> GetNextStockOutReturnCode(int storeId, bool separateYears, DateTime documentDate, int stockTypeId, string? prefix, string? suffix)
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
