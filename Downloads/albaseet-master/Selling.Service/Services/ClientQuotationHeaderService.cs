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
using Shared.Helper.Logic;
using Shared.CoreOne.Contracts.Settings;

namespace Sales.Service.Services
{
	public class ClientQuotationHeaderService : BaseService<ClientQuotationHeader>, IClientQuotationHeaderService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IStoreService _storeService;
        private readonly IClientService _clientService;
		private readonly IStringLocalizer<ClientQuotationHeaderService> _localizer;
		private readonly IMenuEncodingService _menuEncodingService;
        private readonly IGenericMessageService _genericMessageService;
		private readonly ISellerService _sellerService;
        private readonly IApplicationSettingService _applicationSettingService;

		public ClientQuotationHeaderService(IRepository<ClientQuotationHeader> repository, IHttpContextAccessor httpContextAccessor, IStoreService storeService, IStringLocalizer<ClientQuotationHeaderService> localizer, IMenuEncodingService menuEncodingService, IClientService clientService, IGenericMessageService genericMessageService, ISellerService sellerService, IApplicationSettingService applicationSettingService) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
			_storeService = storeService;
            _clientService = clientService;
			_localizer = localizer;
			_menuEncodingService = menuEncodingService;
            _genericMessageService = genericMessageService;
			_sellerService = sellerService;
            _applicationSettingService = applicationSettingService;
		}

		public IQueryable<ClientQuotationHeaderDto> GetClientQuotationHeaders()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var data = from clientQuotationHeader in _repository.GetAll()
				from store in _storeService.GetAll().Where(x => x.StoreId == clientQuotationHeader.StoreId)
				from client in _clientService.GetAll().Where(x => x.ClientId == clientQuotationHeader.ClientId)
                from seller in _sellerService.GetAll().Where(x => x.SellerId ==  clientQuotationHeader.SellerId).DefaultIfEmpty()
                select new ClientQuotationHeaderDto()
				{
					ClientQuotationHeaderId = clientQuotationHeader.ClientQuotationHeaderId,
					Prefix = clientQuotationHeader.Prefix,
					DocumentCode = clientQuotationHeader.DocumentCode,
					Suffix = clientQuotationHeader.Suffix,
					DocumentFullCode = $"{clientQuotationHeader.Prefix}{clientQuotationHeader.DocumentCode}{clientQuotationHeader.Suffix}",
					ClientPriceRequestHeaderId = clientQuotationHeader.ClientPriceRequestHeaderId,
                    ClientId = clientQuotationHeader.ClientId,
                    ClientCode = client.ClientCode,
                    ClientName = language == LanguageCode.Arabic ? client.ClientNameAr : client.ClientNameEn,
					SellerId = seller != null ? seller.SellerId : null,
                    SellerCode = seller != null ? seller.SellerCode : null,
					SellerName = seller != null ? language == LanguageCode.Arabic ? seller.SellerNameAr : seller.SellerNameEn : null,
					DocumentReference = clientQuotationHeader.DocumentReference,
                    StoreId = clientQuotationHeader.StoreId,
					StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
                    TaxTypeId = clientQuotationHeader.TaxTypeId,
					DocumentDate = clientQuotationHeader.DocumentDate,
					EntryDate = clientQuotationHeader.EntryDate,
                    Reference = clientQuotationHeader.Reference,
					TotalValue = clientQuotationHeader.TotalValue,
					DiscountPercent = clientQuotationHeader.DiscountPercent,
					DiscountValue = clientQuotationHeader.DiscountValue,
					TotalItemDiscount = clientQuotationHeader.TotalItemDiscount,
					GrossValue = clientQuotationHeader.GrossValue,
					VatValue = clientQuotationHeader.VatValue,
					SubNetValue = clientQuotationHeader.SubNetValue,
					OtherTaxValue = clientQuotationHeader.OtherTaxValue,
                    NetValueBeforeAdditionalDiscount = clientQuotationHeader.NetValueBeforeAdditionalDiscount,
                    AdditionalDiscountValue = clientQuotationHeader.AdditionalDiscountValue,
					NetValue = clientQuotationHeader.NetValue,
                    TotalCostValue = clientQuotationHeader.TotalCostValue,
                    ValidInDays = clientQuotationHeader.ValidInDays,
                    ValidUntilDate = clientQuotationHeader.ValidUntilDate,
					RemarksAr = clientQuotationHeader.RemarksAr,
					RemarksEn = clientQuotationHeader.RemarksEn,
					IsClosed = clientQuotationHeader.IsClosed,
					ArchiveHeaderId = clientQuotationHeader.ArchiveHeaderId,

                    CreatedAt = clientQuotationHeader.CreatedAt,
                    UserNameCreated = clientQuotationHeader.UserNameCreated,
                    IpAddressCreated = clientQuotationHeader.IpAddressCreated,

                    ModifiedAt = clientQuotationHeader.ModifiedAt,
                    UserNameModified = clientQuotationHeader.UserNameModified,
                    IpAddressModified = clientQuotationHeader.IpAddressModified
                };
			return data;
		}

        public IQueryable<ClientQuotationHeaderDto> GetUserClientQuotationHeaders()
        {
            var userStore = _httpContextAccessor.GetCurrentUserStore();
            return GetClientQuotationHeaders().Where(x => x.StoreId == userStore);
        }

		public IQueryable<ClientQuotationHeaderDto> GetClientQuotationHeadersByStoreId(int storeId, int? clientId, int clientQuotationHeaderId)
		{
            clientId ??= 0;
            if (clientQuotationHeaderId == 0)
            {
                return GetClientQuotationHeaders().Where(x => x.StoreId == storeId && (clientId == 0 || x.ClientId == clientId) && x.IsClosed == false && x.ValidUntilDate.Date >= DateHelper.GetDateTimeNow().Date && x.TaxTypeId != 0);
            }
            else
            {
                return GetClientQuotationHeaders().Where(x => x.ClientQuotationHeaderId ==  clientQuotationHeaderId);
            }
        }

        public async Task<ClientQuotationHeaderDto?> GetClientQuotationHeaderById(int id)
		{
			return await GetClientQuotationHeaders().FirstOrDefaultAsync(x => x.ClientQuotationHeaderId == id);
		}

        public async Task<DocumentCodeDto> GetClientQuotationCode(int storeId, DateTime documentDate)
        {
            var separateYears = await _applicationSettingService.SeparateYears(storeId);
            return await GetClientQuotationCodeInternal(storeId, separateYears, documentDate);
        }

        public async Task<DocumentCodeDto> GetClientQuotationCodeInternal(int storeId, bool separateYears, DateTime documentDate)
        {
            var encoding = await _menuEncodingService.GetMenuEncoding(storeId, MenuCodeData.ClientQuotation);
            var code = await GetNextClientQuotationCode(storeId, separateYears, documentDate, encoding.Prefix, encoding.Suffix);
            return new DocumentCodeDto() { NextCode = code, Prefix = encoding.Prefix, Suffix = encoding.Suffix };
        }

        public async Task<ResponseDto> UpdateClosed(int? clientQuotationHeaderId, bool isClosed)
        {
            var header = await _repository.FindBy(x => x.ClientQuotationHeaderId == clientQuotationHeaderId).FirstOrDefaultAsync();
            if (header is null) return new ResponseDto { Id = (int)clientQuotationHeaderId!, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.ClientQuotation, GenericMessageData.NotFound) };

            header.IsClosed = isClosed;
            _repository.Update(header);
            await _repository.SaveChanges();
            return new ResponseDto { Id = (int)clientQuotationHeaderId!, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.ClientQuotation, isClosed ? GenericMessageData.ClosedSuccessfully : GenericMessageData.OpenedSuccessfully) };
        }
        public async Task<ResponseDto> SaveClientQuotationHeader(ClientQuotationHeaderDto clientQuotationHeader, bool hasApprove, bool approved, int? requestId)
		{
            var separateYears = await _applicationSettingService.SeparateYears(clientQuotationHeader.StoreId);

            if (hasApprove)
            {
                if (clientQuotationHeader.ClientQuotationHeaderId == 0)
                {
                    return await CreateClientQuotationHeader(clientQuotationHeader, hasApprove, approved, requestId, separateYears);
                }
                else
                {
                    return await UpdateClientQuotationHeader(clientQuotationHeader);
                }
            }
            else
            {
                var clientQuotationHeaderExist = await IsClientQuotationCodeExist(clientQuotationHeader.ClientQuotationHeaderId, clientQuotationHeader.DocumentCode, clientQuotationHeader.StoreId, separateYears, clientQuotationHeader.DocumentDate, clientQuotationHeader.Prefix, clientQuotationHeader.Suffix);
                if (clientQuotationHeaderExist.Success)
                {
                    var nextClientQuotationCode = await GetNextClientQuotationCode(clientQuotationHeader.StoreId, separateYears, clientQuotationHeader.DocumentDate, clientQuotationHeader.Prefix, clientQuotationHeader.Suffix);
                    return new ResponseDto() { Id = nextClientQuotationCode, ResponseType = ResponseTypeData.Confirm, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.ClientQuotation, GenericMessageData.CodeAlreadyExist, $"{nextClientQuotationCode}") };
                }
                else
                {
                    if (clientQuotationHeader.ClientQuotationHeaderId == 0)
                    {
                        return await CreateClientQuotationHeader(clientQuotationHeader, hasApprove, approved, requestId, separateYears);
                    }
                    else
                    {
                        return await UpdateClientQuotationHeader(clientQuotationHeader);
                    }
                }
            }
        }

        public async Task<ResponseDto> CreateClientQuotationHeader(ClientQuotationHeaderDto clientQuotationHeader, bool hasApprove, bool approved, int? requestId, bool separateYears)
        {
            int clientQuotationCode;
            string? prefix;
            string? suffix;
            var nextClientQuotationCode = await GetClientQuotationCodeInternal(clientQuotationHeader.StoreId, separateYears, clientQuotationHeader.DocumentDate);
            if (hasApprove && approved)
            {
                clientQuotationCode = nextClientQuotationCode.NextCode;
                prefix = nextClientQuotationCode.Prefix;
                suffix = nextClientQuotationCode.Suffix;
            }
            else
            {
                clientQuotationCode = clientQuotationHeader.DocumentCode != 0 ? clientQuotationHeader.DocumentCode : nextClientQuotationCode.NextCode;
                prefix = string.IsNullOrWhiteSpace(clientQuotationHeader.Prefix) ? nextClientQuotationCode.Prefix : clientQuotationHeader.Prefix;
                suffix = string.IsNullOrWhiteSpace(clientQuotationHeader.Suffix) ? nextClientQuotationCode.Suffix : clientQuotationHeader.Suffix;
            }

            var clientQuotationHeaderId = await GetNextId();
            var newClientQuotationHeader = new ClientQuotationHeader()
            {
                ClientQuotationHeaderId = clientQuotationHeaderId,
                Prefix = prefix,
                DocumentCode = clientQuotationCode,
                Suffix = suffix,
                ClientPriceRequestHeaderId = clientQuotationHeader.ClientPriceRequestHeaderId,
                ClientId = clientQuotationHeader.ClientId,
                SellerId = clientQuotationHeader.SellerId,
                DocumentReference = hasApprove ? $"{DocumentReferenceData.Approval}{requestId}" : $"{DocumentReferenceData.ClientQuotation}{clientQuotationHeaderId}",
                StoreId = clientQuotationHeader.StoreId,
                TaxTypeId = clientQuotationHeader.TaxTypeId,
                DocumentDate = clientQuotationHeader.DocumentDate,
                EntryDate = DateHelper.GetDateTimeNow(),
                Reference = clientQuotationHeader.Reference,
                TotalValue = clientQuotationHeader.TotalValue,
                DiscountPercent = clientQuotationHeader.DiscountPercent,
                DiscountValue = clientQuotationHeader.DiscountValue,
                TotalItemDiscount = clientQuotationHeader.TotalItemDiscount,
                GrossValue = clientQuotationHeader.GrossValue,
                VatValue = clientQuotationHeader.VatValue,
                SubNetValue = clientQuotationHeader.SubNetValue,
                OtherTaxValue = clientQuotationHeader.OtherTaxValue,
                NetValueBeforeAdditionalDiscount = clientQuotationHeader.NetValueBeforeAdditionalDiscount,
                AdditionalDiscountValue = clientQuotationHeader.AdditionalDiscountValue,
                NetValue = clientQuotationHeader.NetValue,
                TotalCostValue = clientQuotationHeader.TotalCostValue,
                ValidInDays = clientQuotationHeader.ValidInDays,
                ValidUntilDate = clientQuotationHeader.ValidUntilDate,
                RemarksAr = clientQuotationHeader.RemarksAr,
                RemarksEn = clientQuotationHeader.RemarksEn,
                IsClosed = false,
                ArchiveHeaderId = clientQuotationHeader.ArchiveHeaderId,


                CreatedAt = DateHelper.GetDateTimeNow(),
                UserNameCreated = await _httpContextAccessor!.GetUserName(),
                IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
                Hide = false,
            };

            var clientQuotationHeaderValidator = await new ClientQuotationHeaderValidator(_localizer).ValidateAsync(newClientQuotationHeader);
            var validationResult = clientQuotationHeaderValidator.IsValid;
            if (validationResult)
            {
                await _repository.Insert(newClientQuotationHeader);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = newClientQuotationHeader.ClientQuotationHeaderId, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.ClientQuotation, GenericMessageData.CreatedSuccessWithCode, $"{newClientQuotationHeader.Prefix}{newClientQuotationHeader.DocumentCode}{newClientQuotationHeader.Suffix}") };
            }
            else
            {
                return new ResponseDto() { Id = newClientQuotationHeader.ClientQuotationHeaderId, Success = false, Message = clientQuotationHeaderValidator.ToString("~") };
            }
        }

        public async Task<ResponseDto> UpdateClientQuotationHeader(ClientQuotationHeaderDto clientQuotationHeader)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var clientQuotationHeaderDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.ClientQuotationHeaderId == clientQuotationHeader.ClientQuotationHeaderId);
            if (clientQuotationHeaderDb != null)
            {
                if (clientQuotationHeaderDb.IsClosed)
                {
                    return new ResponseDto() { Id = clientQuotationHeader.ClientQuotationHeaderId, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.ClientQuotation, GenericMessageData.CannotUpdateBecauseClosed) };
                }

                clientQuotationHeaderDb.ClientPriceRequestHeaderId = clientQuotationHeader.ClientPriceRequestHeaderId;
                clientQuotationHeaderDb.ClientId = clientQuotationHeader.ClientId;
                clientQuotationHeaderDb.SellerId = clientQuotationHeader.SellerId;
                clientQuotationHeaderDb.StoreId = clientQuotationHeader.StoreId;
                clientQuotationHeaderDb.TaxTypeId = clientQuotationHeader.TaxTypeId;
                clientQuotationHeaderDb.DocumentDate = clientQuotationHeader.DocumentDate;
                clientQuotationHeaderDb.Reference = clientQuotationHeader.Reference;
                clientQuotationHeaderDb.TotalValue = clientQuotationHeader.TotalValue;
                clientQuotationHeaderDb.DiscountPercent = clientQuotationHeader.DiscountPercent;
                clientQuotationHeaderDb.DiscountValue = clientQuotationHeader.DiscountValue;
                clientQuotationHeaderDb.TotalItemDiscount = clientQuotationHeader.TotalItemDiscount;
                clientQuotationHeaderDb.GrossValue = clientQuotationHeader.GrossValue;
                clientQuotationHeaderDb.VatValue = clientQuotationHeader.VatValue;
                clientQuotationHeaderDb.SubNetValue = clientQuotationHeader.SubNetValue;
                clientQuotationHeaderDb.OtherTaxValue = clientQuotationHeader.OtherTaxValue;
                clientQuotationHeaderDb.NetValueBeforeAdditionalDiscount = clientQuotationHeader.NetValueBeforeAdditionalDiscount;
                clientQuotationHeaderDb.AdditionalDiscountValue = clientQuotationHeader.AdditionalDiscountValue;
                clientQuotationHeaderDb.NetValue = clientQuotationHeader.NetValue;
                clientQuotationHeaderDb.TotalCostValue = clientQuotationHeader.TotalCostValue;
                clientQuotationHeaderDb.ValidInDays = clientQuotationHeader.ValidInDays;
                clientQuotationHeaderDb.ValidUntilDate = clientQuotationHeader.ValidUntilDate;
                clientQuotationHeaderDb.RemarksAr = clientQuotationHeader.RemarksAr;
                clientQuotationHeaderDb.RemarksEn = clientQuotationHeader.RemarksEn;
                clientQuotationHeaderDb.ArchiveHeaderId = clientQuotationHeader.ArchiveHeaderId;
                
                clientQuotationHeaderDb.ModifiedAt = DateHelper.GetDateTimeNow();
                clientQuotationHeaderDb.UserNameModified = await _httpContextAccessor!.GetUserName();
                clientQuotationHeaderDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();


                var clientQuotationHeaderValidator = await new ClientQuotationHeaderValidator(_localizer).ValidateAsync(clientQuotationHeaderDb);
                var validationResult = clientQuotationHeaderValidator.IsValid;
                if (validationResult)
                {
                    _repository.Update(clientQuotationHeaderDb);
                    await _repository.SaveChanges();
                    return new ResponseDto() { Id = clientQuotationHeaderDb.ClientQuotationHeaderId, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.ClientQuotation, GenericMessageData.UpdatedSuccessWithCode, $"{clientQuotationHeaderDb.Prefix}{clientQuotationHeaderDb.DocumentCode}{clientQuotationHeaderDb.Suffix}") };
                }
                else
                {
                    return new ResponseDto() { Id = 0, Success = false, Message = clientQuotationHeaderValidator.ToString("~") };
                }
            }
            return new ResponseDto() { Id = 0, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.ClientQuotation, GenericMessageData.NotFound) };
        }

        public async Task<ResponseDto> IsClientQuotationCodeExist(int clientQuotationHeaderId, int clientQuotationCode, int storeId, bool separateYears, DateTime documentDate, string? prefix, string? suffix)
		{
			var clientQuotationHeader = await _repository.GetAll().FirstOrDefaultAsync(x => x.ClientQuotationHeaderId != clientQuotationHeaderId && (x.StoreId == storeId && (!separateYears || x.DocumentDate.Year == documentDate.Year) && x.Prefix == prefix && x.DocumentCode == clientQuotationCode && x.Suffix == suffix));
			if (clientQuotationHeader is not null)
			{
                return new ResponseDto() { Id = clientQuotationHeader.ClientQuotationHeaderId, Success = true };
            }
			return new ResponseDto() {  Id = 0, Success = false };
		}

        public async Task<int> GetNextId()
        {
            int id = 1;
            try { id = await _repository.GetAll().MaxAsync(a => a.ClientQuotationHeaderId) + 1; } catch { id = 1; }
            return id;
        }

        public async Task<ResponseDto> DeleteClientQuotationHeader(int id)
		{
            var clientQuotationHeader = await _repository.GetAll().FirstOrDefaultAsync(x => x.ClientQuotationHeaderId == id);
            if (clientQuotationHeader != null)
            {
                _repository.Delete(clientQuotationHeader);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = id, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.ClientQuotation, GenericMessageData.DeleteSuccessWithCode, $"{clientQuotationHeader.Prefix}{clientQuotationHeader.DocumentCode}{clientQuotationHeader.Suffix}") };
            }
            return new ResponseDto() { Id = id, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.ClientQuotation, GenericMessageData.NotFound) };
        }

        public async Task<int> GetNextClientQuotationCode(int storeId, bool separateYears, DateTime documentDate, string? prefix, string? suffix)
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
