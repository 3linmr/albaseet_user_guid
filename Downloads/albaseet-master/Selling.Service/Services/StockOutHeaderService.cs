using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Sales.CoreOne.Models.Domain;
using Sales.CoreOne.Models.Dtos.ViewModels;
using Sales.Service.Validators;
using Shared.CoreOne.Contracts.Menus;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.StaticData;
using Shared.CoreOne;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Shared.Helper.Identity;
using Sales.CoreOne.Contracts;
using Shared.Helper.Logic;
using Shared.Service;
using Purchases.CoreOne.Models.Domain;
using Shared.CoreOne.Contracts.Settings;

namespace Sales.Service.Services
{
    public class StockOutHeaderService: BaseService<StockOutHeader>, IStockOutHeaderService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStoreService _storeService;
        private readonly IClientService _clientService;
        private readonly IStringLocalizer<StockOutHeaderService> _localizer;
        private readonly IMenuEncodingService _menuEncodingService;
        private readonly IProformaInvoiceHeaderService _proformaInvoiceHeaderService;
        private readonly ISalesInvoiceHeaderService _salesInvoiceHeaderService;
        private readonly IGenericMessageService _genericMessageService;
        private readonly IMenuService _menuService;
		private readonly ISellerService _sellerService;
        private readonly IApplicationSettingService _applicationSettingService;

		public StockOutHeaderService(IRepository<StockOutHeader> repository, IHttpContextAccessor httpContextAccessor, IStoreService storeService, IStringLocalizer<StockOutHeaderService> localizer, IMenuEncodingService menuEncodingService, IClientService clientService, IProformaInvoiceHeaderService proformaInvoiceHeaderService, ISalesInvoiceHeaderService salesInvoiceHeaderService, IGenericMessageService genericMessageService, IMenuService menuService, ISellerService sellerService, IApplicationSettingService applicationSettingService) : base(repository)
        {
            _httpContextAccessor = httpContextAccessor;
            _storeService = storeService;
            _clientService = clientService;
            _localizer = localizer;
            _menuEncodingService = menuEncodingService;
            _proformaInvoiceHeaderService = proformaInvoiceHeaderService;
            _salesInvoiceHeaderService = salesInvoiceHeaderService;
            _genericMessageService = genericMessageService;
            _menuService = menuService;
			_sellerService = sellerService;
            _applicationSettingService = applicationSettingService;
		}

        public IQueryable<StockOutHeaderDto> GetStockOutHeaders()
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();
            var data = from stockOutHeader in _repository.GetAll()
                       from store in _storeService.GetAll().Where(x => x.StoreId == stockOutHeader.StoreId)
                       from client in _clientService.GetAll().Where(x => x.ClientId == stockOutHeader.ClientId)
                       from proformaInvoiceHeader in _proformaInvoiceHeaderService.GetAll().Where(x => x.ProformaInvoiceHeaderId == stockOutHeader.ProformaInvoiceHeaderId).DefaultIfEmpty()
                       from salesInvoiceHeader in _salesInvoiceHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == stockOutHeader.SalesInvoiceHeaderId).DefaultIfEmpty()
                       from menu in _menuService.GetAll().Where(x => x.MenuCode == stockOutHeader.MenuCode).DefaultIfEmpty()
                       from seller in _sellerService.GetAll().Where(x => x.SellerId == stockOutHeader.SellerId).DefaultIfEmpty()
                       select new StockOutHeaderDto()
                       {
                           StockOutHeaderId = stockOutHeader.StockOutHeaderId,
                           Prefix = stockOutHeader.Prefix,
                           DocumentCode = stockOutHeader.DocumentCode,
                           Suffix = stockOutHeader.Suffix,
                           DocumentFullCode = $"{stockOutHeader.Prefix}{stockOutHeader.DocumentCode}{stockOutHeader.Suffix}",
                           DocumentReference = stockOutHeader.DocumentReference,
                           StockTypeId = stockOutHeader.StockTypeId,
                           ProformaInvoiceHeaderId = stockOutHeader.ProformaInvoiceHeaderId,
                           ProformaInvoiceFullCode = proformaInvoiceHeader != null ? $"{proformaInvoiceHeader.Prefix}{proformaInvoiceHeader.DocumentCode}{proformaInvoiceHeader.Suffix}" : null,
                           ProformaInvoiceDocumentReference = proformaInvoiceHeader != null ? proformaInvoiceHeader.DocumentReference : null,
                           SalesInvoiceHeaderId = stockOutHeader.SalesInvoiceHeaderId,
                           SalesInvoiceFullCode = salesInvoiceHeader != null ? $"{salesInvoiceHeader.Prefix}{salesInvoiceHeader.DocumentCode}{salesInvoiceHeader.Suffix}" : null,
                           SalesInvoiceDocumentReference = salesInvoiceHeader != null ? salesInvoiceHeader.DocumentReference : null,
                           ClientId = stockOutHeader.ClientId,
                           ClientCode = client.ClientCode,
                           ClientName = language == LanguageCode.Arabic ? client.ClientNameAr : client.ClientNameEn,
						   SellerId = seller != null ? seller.SellerId : null,
                           SellerCode = seller != null ? seller.SellerCode : null,
						   SellerName = seller != null ? language == LanguageCode.Arabic ? seller.SellerNameAr : seller.SellerNameEn : null,
						   StoreId = stockOutHeader.StoreId,
                           StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
                           DocumentDate = stockOutHeader.DocumentDate,
                           EntryDate = stockOutHeader.EntryDate,
                           Reference = stockOutHeader.Reference,
                           TotalValue = stockOutHeader.TotalValue,
                           DiscountPercent = stockOutHeader.DiscountPercent,
                           DiscountValue = stockOutHeader.DiscountValue,
                           TotalItemDiscount = stockOutHeader.TotalItemDiscount,
                           GrossValue = stockOutHeader.GrossValue,
                           VatValue = stockOutHeader.VatValue,
                           SubNetValue = stockOutHeader.SubNetValue,
                           OtherTaxValue = stockOutHeader.OtherTaxValue,
                           NetValueBeforeAdditionalDiscount = stockOutHeader.NetValueBeforeAdditionalDiscount,
                           AdditionalDiscountValue = stockOutHeader.AdditionalDiscountValue,
                           NetValue = stockOutHeader.NetValue,
                           TotalCostValue = stockOutHeader.TotalCostValue,
                           RemarksAr = stockOutHeader.RemarksAr,
                           RemarksEn = stockOutHeader.RemarksEn,
                           IsClosed = stockOutHeader.IsClosed || stockOutHeader.IsEnded || stockOutHeader.IsBlocked,
                           IsEnded = stockOutHeader.IsEnded,
                           IsBlocked = stockOutHeader.IsBlocked,
                           ArchiveHeaderId = stockOutHeader.ArchiveHeaderId,
                           MenuCode = stockOutHeader.MenuCode,
                           MenuName = menu != null ? language == LanguageCode.Arabic ? menu.MenuNameAr : menu.MenuNameEn : null,

                           CreatedAt = stockOutHeader.CreatedAt,
                           UserNameCreated = stockOutHeader.UserNameCreated,
                           IpAddressCreated = stockOutHeader.IpAddressCreated,

                           ModifiedAt = stockOutHeader.ModifiedAt,
                           UserNameModified = stockOutHeader.UserNameModified,
                           IpAddressModified = stockOutHeader.IpAddressModified
                       };
            return data;
        }

        public IQueryable<StockOutHeaderDto> GetUserStockOutHeaders(int stockTypeId)
        {
            var userStore = _httpContextAccessor.GetCurrentUserStore();
            return GetStockOutHeaders().Where(x => x.StockTypeId == stockTypeId && x.StoreId == userStore);
        }

        public async Task<StockOutHeaderDto?> GetStockOutHeaderById(int id)
        {
            return await GetStockOutHeaders().FirstOrDefaultAsync(x => x.StockOutHeaderId == id);
        }

		public async Task<bool> UpdateAllStockOutsBlockedFromProformaInvoice(int proformaInvoiceHeaderId, bool isBlocked)
		{
			var headerList = new List<StockOutHeader>();

			var stockOutsDirectlyRelatedToProformaInvoice = await _repository.FindBy(x => x.ProformaInvoiceHeaderId == proformaInvoiceHeaderId).ToListAsync();
			headerList.AddRange(stockOutsDirectlyRelatedToProformaInvoice);

			var stocksInsRelatedToProformaInvoiceThroughSalesInvoice = await (
					from stockOutHeader in _repository.GetAll()
					from salesInvoiceHeader in _salesInvoiceHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == stockOutHeader.SalesInvoiceHeaderId && x.ProformaInvoiceHeaderId == proformaInvoiceHeaderId)
					select stockOutHeader
				).ToListAsync();

			headerList.AddRange(stocksInsRelatedToProformaInvoiceThroughSalesInvoice);

			foreach (var header in headerList)
			{
				header.IsBlocked = isBlocked;
			}

            _repository.UpdateRange(headerList);
			await _repository.SaveChanges();
			return true;
		}

		public async Task<int> UpdateAllStockOutsEndedDirectlyFromSalesInvoice(int salesInvoiceHeaderId, bool isEnded)
		{
			var headerList = new List<StockOutHeader>();

			var stockOutsRelatedToSalesInvoice = await _repository.FindBy(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId).ToListAsync();
			headerList.AddRange(stockOutsRelatedToSalesInvoice);

			foreach (var header in headerList)
			{
				header.IsEnded = isEnded;
			}

            _repository.UpdateRange(headerList);
			await _repository.SaveChanges();
			return headerList.Count;
		}

		public async Task<bool> UpdateStockOutsEnded(List<int> stockOutHeaderIds, bool isEnded)
		{
			var stockOutHeaders = await _repository.FindBy(x => stockOutHeaderIds.Contains(x.StockOutHeaderId)).ToListAsync();

			foreach (var header in stockOutHeaders)
			{
				header.IsEnded = isEnded;
			}

            _repository.UpdateRange(stockOutHeaders);
			await _repository.SaveChanges();
			return true;
		}

		public async Task<DocumentCodeDto> GetStockOutCode(int storeId, DateTime documentDate, int stockTypeId)
        {
            var separateYears = await _applicationSettingService.SeparateYears(storeId);
            return await GetStockOutCodeInternal(storeId, separateYears, documentDate, stockTypeId);
        }

		public async Task<DocumentCodeDto> GetStockOutCodeInternal(int storeId, bool separateYears, DateTime documentDate, int stockTypeId)
        {
            var encoding = await _menuEncodingService.GetMenuEncoding(storeId, StockTypeData.ToMenuCode((byte)stockTypeId));
            var code = await GetNextStockOutCode(storeId, separateYears, documentDate, stockTypeId, encoding.Prefix, encoding.Suffix);
            return new DocumentCodeDto() { NextCode = code, Prefix = encoding.Prefix, Suffix = encoding.Suffix };
        }

        public async Task<bool> UpdateClosed(int? stockOutHeaderId, bool isClosed)
        {
            var header = await _repository.FindBy(x => x.StockOutHeaderId == stockOutHeaderId).FirstOrDefaultAsync();
            if (header is null) return false;

            header.IsClosed = isClosed;
            _repository.Update(header);
            await _repository.SaveChanges();
            return true;
        }

        public async Task<ResponseDto> SaveStockOutHeader(StockOutHeaderDto stockOutHeader, bool hasApprove, bool approved, int? requestId, string? documentReference, bool shouldInitializeFlags)
        {
            var separateYears = await _applicationSettingService.SeparateYears(stockOutHeader.StoreId);

            if (hasApprove)
            {
                if (stockOutHeader.StockOutHeaderId == 0)
                {
                    return await CreateStockOutHeader(stockOutHeader, hasApprove, approved, requestId, documentReference, shouldInitializeFlags, separateYears);
                }
                else
                {
                    return await UpdateStockOutHeader(stockOutHeader);
                }
            }
            else
            {
                var stockOutHeaderExist = await IsStockOutCodeExist(stockOutHeader.StockOutHeaderId, stockOutHeader.DocumentCode, stockOutHeader.StoreId, separateYears, stockOutHeader.DocumentDate, stockOutHeader.StockTypeId, stockOutHeader.Prefix, stockOutHeader.Suffix);
                if (stockOutHeaderExist.Success)
                {
                    var nextStockOutCode = await GetNextStockOutCode(stockOutHeader.StoreId, separateYears, stockOutHeader.DocumentDate, stockOutHeader.StockTypeId, stockOutHeader.Prefix, stockOutHeader.Suffix);
                    return new ResponseDto() { Id = nextStockOutCode, ResponseType = ResponseTypeData.Confirm, Success = false, Message = await _genericMessageService.GetMessage(StockTypeData.ToMenuCode(stockOutHeader.StockTypeId), GenericMessageData.CodeAlreadyExist, $"{nextStockOutCode}") };
                }
                else
                {
                    if (stockOutHeader.StockOutHeaderId == 0)
                    {
                        return await CreateStockOutHeader(stockOutHeader, hasApprove, approved, requestId, documentReference, shouldInitializeFlags, separateYears);
                    }
                    else
                    {
                        return await UpdateStockOutHeader(stockOutHeader);
                    }
                }
            }
        }

        public async Task<ResponseDto> CreateStockOutHeader(StockOutHeaderDto stockOutHeader, bool hasApprove, bool approved, int? requestId, string? documentReference, bool shouldInitializeFlags, bool separateYears)
        {
            int stockOutCode;
            string? prefix;
            string? suffix;
            var nextStockOutCode = await GetStockOutCodeInternal(stockOutHeader.StoreId, separateYears, stockOutHeader.DocumentDate, stockOutHeader.StockTypeId);
            if (hasApprove && approved)
            {
                stockOutCode = nextStockOutCode.NextCode;
                prefix = nextStockOutCode.Prefix;
                suffix = nextStockOutCode.Suffix;
            }
            else
            {
                stockOutCode = stockOutHeader.DocumentCode != 0 ? stockOutHeader.DocumentCode : nextStockOutCode.NextCode;
                prefix = string.IsNullOrWhiteSpace(stockOutHeader.Prefix) ? nextStockOutCode.Prefix : stockOutHeader.Prefix;
                suffix = string.IsNullOrWhiteSpace(stockOutHeader.Suffix) ? nextStockOutCode.Suffix : stockOutHeader.Suffix;
            }

            var stockOutHeaderId = await GetNextId();
            var newStockOutHeader = new StockOutHeader()
            {
                StockOutHeaderId = stockOutHeaderId,
                Prefix = prefix,
                DocumentCode = stockOutCode,
                Suffix = suffix,
                DocumentReference = documentReference != null ? documentReference : ( hasApprove ? $"{DocumentReferenceData.Approval}{requestId}" : $"{StockTypeData.ToDocumentReference(stockOutHeader.StockTypeId)}{stockOutHeaderId}" ),
                StockTypeId = stockOutHeader.StockTypeId,
                ProformaInvoiceHeaderId = stockOutHeader.ProformaInvoiceHeaderId,
                SalesInvoiceHeaderId = stockOutHeader.SalesInvoiceHeaderId,
                ClientId = stockOutHeader.ClientId,
                SellerId = stockOutHeader.SellerId,
                StoreId = stockOutHeader.StoreId,
                DocumentDate = stockOutHeader.DocumentDate,
                EntryDate = DateHelper.GetDateTimeNow(),
                Reference = stockOutHeader.Reference,
                TotalValue = stockOutHeader.TotalValue,
                DiscountPercent = stockOutHeader.DiscountPercent,
                DiscountValue = stockOutHeader.DiscountValue,
                TotalItemDiscount = stockOutHeader.TotalItemDiscount,
                GrossValue = stockOutHeader.GrossValue,
                VatValue = stockOutHeader.VatValue,
                SubNetValue = stockOutHeader.SubNetValue,
                OtherTaxValue = stockOutHeader.OtherTaxValue,
                NetValueBeforeAdditionalDiscount = stockOutHeader.NetValueBeforeAdditionalDiscount,
                AdditionalDiscountValue = stockOutHeader.AdditionalDiscountValue,
                NetValue = stockOutHeader.NetValue,
                TotalCostValue = stockOutHeader.TotalCostValue,
                RemarksAr = stockOutHeader.RemarksAr,
                RemarksEn = stockOutHeader.RemarksEn,
                IsClosed = shouldInitializeFlags ? stockOutHeader.IsClosed : false,
                IsEnded = shouldInitializeFlags ? stockOutHeader.IsEnded : false,
                IsBlocked = shouldInitializeFlags ? stockOutHeader.IsBlocked : false,
                ArchiveHeaderId = stockOutHeader.ArchiveHeaderId,
                MenuCode = StockTypeData.ToMenuCode(stockOutHeader.StockTypeId),


                CreatedAt = DateHelper.GetDateTimeNow(),
                UserNameCreated = await _httpContextAccessor!.GetUserName(),
                IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
                Hide = false,
            };

            var stockOutHeaderValidator = await new StockOutHeaderValidator(_localizer).ValidateAsync(newStockOutHeader);
            var validationResult = stockOutHeaderValidator.IsValid;
            if (validationResult)
            {
                await _repository.Insert(newStockOutHeader);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = newStockOutHeader.StockOutHeaderId, Success = true, Message = await _genericMessageService.GetMessage(StockTypeData.ToMenuCode(stockOutHeader.StockTypeId), GenericMessageData.CreatedSuccessWithCode, $"{newStockOutHeader.Prefix}{newStockOutHeader.DocumentCode}{newStockOutHeader.Suffix}") };
            }
            else
            {
                return new ResponseDto() { Id = newStockOutHeader.StockOutHeaderId, Success = false, Message = stockOutHeaderValidator.ToString("~") };
            }
        }

        public async Task<ResponseDto> UpdateStockOutHeader(StockOutHeaderDto stockOutHeader)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var stockOutHeaderDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.StockOutHeaderId == stockOutHeader.StockOutHeaderId);
            if (stockOutHeaderDb != null)
            {
                stockOutHeaderDb.ProformaInvoiceHeaderId = stockOutHeader.ProformaInvoiceHeaderId;
                stockOutHeaderDb.SalesInvoiceHeaderId = stockOutHeader.SalesInvoiceHeaderId;
                stockOutHeaderDb.ClientId = stockOutHeader.ClientId;
                stockOutHeaderDb.SellerId = stockOutHeader.SellerId;
                stockOutHeaderDb.StoreId = stockOutHeader.StoreId;
                stockOutHeaderDb.DocumentDate = stockOutHeader.DocumentDate;
                stockOutHeaderDb.Reference = stockOutHeader.Reference;
                stockOutHeaderDb.TotalValue = stockOutHeader.TotalValue;
                stockOutHeaderDb.DiscountPercent = stockOutHeader.DiscountPercent;
                stockOutHeaderDb.DiscountValue = stockOutHeader.DiscountValue;
                stockOutHeaderDb.TotalItemDiscount = stockOutHeader.TotalItemDiscount;
                stockOutHeaderDb.GrossValue = stockOutHeader.GrossValue;
                stockOutHeaderDb.VatValue = stockOutHeader.VatValue;
                stockOutHeaderDb.SubNetValue = stockOutHeader.SubNetValue;
                stockOutHeaderDb.OtherTaxValue = stockOutHeader.OtherTaxValue;
                stockOutHeaderDb.NetValueBeforeAdditionalDiscount = stockOutHeader.NetValueBeforeAdditionalDiscount;
                stockOutHeaderDb.AdditionalDiscountValue = stockOutHeader.AdditionalDiscountValue;
                stockOutHeaderDb.NetValue = stockOutHeader.NetValue;
                stockOutHeaderDb.TotalCostValue = stockOutHeader.TotalCostValue;
                stockOutHeaderDb.RemarksAr = stockOutHeader.RemarksAr;
                stockOutHeaderDb.RemarksEn = stockOutHeader.RemarksEn;
                stockOutHeaderDb.ArchiveHeaderId = stockOutHeader.ArchiveHeaderId;

                stockOutHeaderDb.ModifiedAt = DateHelper.GetDateTimeNow();
                stockOutHeaderDb.UserNameModified = await _httpContextAccessor!.GetUserName();
                stockOutHeaderDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();


                var stockOutHeaderValidator = await new StockOutHeaderValidator(_localizer).ValidateAsync(stockOutHeaderDb);
                var validationResult = stockOutHeaderValidator.IsValid;
                if (validationResult)
                {
                    _repository.Update(stockOutHeaderDb);
                    await _repository.SaveChanges();
                    return new ResponseDto() { Id = stockOutHeaderDb.StockOutHeaderId, Success = true, Message = await _genericMessageService.GetMessage(StockTypeData.ToMenuCode(stockOutHeaderDb.StockTypeId), GenericMessageData.UpdatedSuccessWithCode, $"{stockOutHeaderDb.Prefix}{stockOutHeaderDb.DocumentCode}{stockOutHeaderDb.Suffix}") };
                }
                else
                {
                    return new ResponseDto() { Id = 0, Success = false, Message = stockOutHeaderValidator.ToString("~") };
                }
            }
            return new ResponseDto() { Id = 0, Success = false, Message = await _genericMessageService.GetMessage(StockTypeData.ToMenuCode(stockOutHeader.StockTypeId), GenericMessageData.NotFound) };
        }

        public async Task<ResponseDto> IsStockOutCodeExist(int stockOutHeaderId, int stockOutCode, int storeId, bool separateYears, DateTime documentDate, int stockTypeId, string? prefix, string? suffix)
        {
            var stockOutHeader = await _repository.GetAll().FirstOrDefaultAsync(x => x.StockOutHeaderId != stockOutHeaderId && (x.StoreId == storeId && (!separateYears || x.DocumentDate.Year == documentDate.Year) && x.StockTypeId == stockTypeId && x.Prefix == prefix && x.DocumentCode == stockOutCode && x.Suffix == suffix));
            if (stockOutHeader is not null)
            {
                return new ResponseDto() { Id = stockOutHeader.StockOutHeaderId, Success = true };
            }
            return new ResponseDto() { Id = 0, Success = false };
        }

        public async Task<int> GetNextId()
        {
            int id = 1;
            try { id = await _repository.GetAll().MaxAsync(a => a.StockOutHeaderId) + 1; } catch { id = 1; }
            return id;
        }

        public async Task<ResponseDto> DeleteStockOutHeader(int id, int menuCode)
        {
            var stockOutHeader = await _repository.GetAll().FirstOrDefaultAsync(x => x.StockOutHeaderId == id);
            if (stockOutHeader != null)
            {
                _repository.Delete(stockOutHeader);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = id, Success = true, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.DeleteSuccessWithCode, $"{stockOutHeader.Prefix}{stockOutHeader.DocumentCode}{stockOutHeader.Suffix}") };
            }
            return new ResponseDto() { Id = id, Success = false, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.NotFound) };
        }

        public async Task<int> GetNextStockOutCode(int storeId, bool separateYears, DateTime documentDate, int stockTypeId, string? prefix, string? suffix)
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
