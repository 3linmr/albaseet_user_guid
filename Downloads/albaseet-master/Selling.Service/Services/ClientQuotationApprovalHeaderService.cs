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
	public class ClientQuotationApprovalHeaderService : BaseService<ClientQuotationApprovalHeader>, IClientQuotationApprovalHeaderService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IStoreService _storeService;
        private readonly IClientService _clientService;
		private readonly IStringLocalizer<ClientQuotationApprovalHeaderService> _localizer;
		private readonly IMenuEncodingService _menuEncodingService;
        private readonly IGenericMessageService _genericMessageService;
		private readonly ISellerService _sellerService;
        private readonly IApplicationSettingService _applicationSettingService;

		public ClientQuotationApprovalHeaderService(IRepository<ClientQuotationApprovalHeader> repository, IHttpContextAccessor httpContextAccessor, IStoreService storeService, IStringLocalizer<ClientQuotationApprovalHeaderService> localizer, IMenuEncodingService menuEncodingService, IClientService clientService, IGenericMessageService genericMessageService, ISellerService sellerService, IApplicationSettingService applicationSettingService) : base(repository)
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

		public IQueryable<ClientQuotationApprovalHeaderDto> GetClientQuotationApprovalHeaders()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var data = from clientQuotationApprovalHeader in _repository.GetAll()
				from store in _storeService.GetAll().Where(x => x.StoreId == clientQuotationApprovalHeader.StoreId)
				from client in _clientService.GetAll().Where(x => x.ClientId == clientQuotationApprovalHeader.ClientId)
                from seller in _sellerService.GetAll().Where(x => x.SellerId == clientQuotationApprovalHeader.SellerId).DefaultIfEmpty()
                select new ClientQuotationApprovalHeaderDto()
				{
					ClientQuotationApprovalHeaderId = clientQuotationApprovalHeader.ClientQuotationHeaderApprovalId,
					Prefix = clientQuotationApprovalHeader.Prefix,
					DocumentCode = clientQuotationApprovalHeader.DocumentCode,
					Suffix = clientQuotationApprovalHeader.Suffix,
					DocumentFullCode = $"{clientQuotationApprovalHeader.Prefix}{clientQuotationApprovalHeader.DocumentCode}{clientQuotationApprovalHeader.Suffix}",
					ClientQuotationHeaderId = clientQuotationApprovalHeader.ClientQuotationHeaderId,
                    ClientId = clientQuotationApprovalHeader.ClientId,
                    ClientCode = client.ClientCode,
                    ClientName = language == LanguageCode.Arabic ? client.ClientNameAr : client.ClientNameEn,
					SellerId = seller != null ? seller.SellerId : null,
                    SellerCode = seller != null ? seller.SellerCode : null,
					SellerName = seller != null ? language == LanguageCode.Arabic ? seller.SellerNameAr : seller.SellerNameEn : null,
					DocumentReference = clientQuotationApprovalHeader.DocumentReference,
                    StoreId = clientQuotationApprovalHeader.StoreId,
					StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
                    TaxTypeId = clientQuotationApprovalHeader.TaxTypeId,
					DocumentDate = clientQuotationApprovalHeader.DocumentDate,
					EntryDate = clientQuotationApprovalHeader.EntryDate,
                    Reference = clientQuotationApprovalHeader.Reference,
					TotalValue = clientQuotationApprovalHeader.TotalValue,
					DiscountPercent = clientQuotationApprovalHeader.DiscountPercent,
					DiscountValue = clientQuotationApprovalHeader.DiscountValue,
					TotalItemDiscount = clientQuotationApprovalHeader.TotalItemDiscount,
					GrossValue = clientQuotationApprovalHeader.GrossValue,
					VatValue = clientQuotationApprovalHeader.VatValue,
					SubNetValue = clientQuotationApprovalHeader.SubNetValue,
					OtherTaxValue = clientQuotationApprovalHeader.OtherTaxValue,
                    NetValueBeforeAdditionalDiscount = clientQuotationApprovalHeader.NetValueBeforeAdditionalDiscount,
                    AdditionalDiscountValue = clientQuotationApprovalHeader.AdditionalDiscountValue,
					NetValue = clientQuotationApprovalHeader.NetValue,
                    TotalCostValue = clientQuotationApprovalHeader.TotalCostValue,
					RemarksAr = clientQuotationApprovalHeader.RemarksAr,
					RemarksEn = clientQuotationApprovalHeader.RemarksEn,
					IsClosed = clientQuotationApprovalHeader.IsClosed,
					ArchiveHeaderId = clientQuotationApprovalHeader.ArchiveHeaderId,

                    CreatedAt = clientQuotationApprovalHeader.CreatedAt,
                    UserNameCreated = clientQuotationApprovalHeader.UserNameCreated,
                    IpAddressCreated = clientQuotationApprovalHeader.IpAddressCreated,

                    ModifiedAt = clientQuotationApprovalHeader.ModifiedAt,
                    UserNameModified = clientQuotationApprovalHeader.UserNameModified,
                    IpAddressModified = clientQuotationApprovalHeader.IpAddressModified
                };
			return data;
		}

        public IQueryable<ClientQuotationApprovalHeaderDto> GetUserClientQuotationApprovalHeaders()
        {
            var userStore = _httpContextAccessor.GetCurrentUserStore();
            return GetClientQuotationApprovalHeaders().Where(x => x.StoreId == userStore);
        }

		public IQueryable<ClientQuotationApprovalHeaderDto> GetClientQuotationApprovalHeadersByStoreId(int storeId, int? clientId, int clientQuotationApprovalHeaderId)
		{
            clientId ??= 0;
            if (clientQuotationApprovalHeaderId == 0)
            {
                return GetClientQuotationApprovalHeaders().Where(x => x.StoreId == storeId && (clientId == 0 || x.ClientId == clientId) && x.IsClosed == false && x.TaxTypeId != 0);
            }
            else
            {
                return GetClientQuotationApprovalHeaders().Where(x => x.ClientQuotationApprovalHeaderId == clientQuotationApprovalHeaderId);
            }
        }

        public async Task<ClientQuotationApprovalHeaderDto?> GetClientQuotationApprovalHeaderById(int id)
		{
			return await GetClientQuotationApprovalHeaders().FirstOrDefaultAsync(x => x.ClientQuotationApprovalHeaderId == id);
		}

        public async Task<DocumentCodeDto> GetClientQuotationApprovalCode(int storeId, DateTime documentDate)
        {
            var separateYears = await _applicationSettingService.SeparateYears(storeId);
            return await GetClientQuotationApprovalCodeInternal(storeId, separateYears, documentDate);
        }


        public async Task<DocumentCodeDto> GetClientQuotationApprovalCodeInternal(int storeId, bool separateYears, DateTime documentDate)
        {
            var encoding = await _menuEncodingService.GetMenuEncoding(storeId, MenuCodeData.ClientQuotationApproval);
            var code = await GetNextClientQuotationApprovalCode(storeId, separateYears, documentDate, encoding.Prefix, encoding.Suffix);
            return new DocumentCodeDto() { NextCode = code, Prefix = encoding.Prefix, Suffix = encoding.Suffix };
        }

        public async Task<ResponseDto> UpdateClosed(int? clientQuotationApprovalHeaderId, bool isClosed)
        {
            var header = await _repository.FindBy(x => x.ClientQuotationHeaderApprovalId == clientQuotationApprovalHeaderId).FirstOrDefaultAsync();
            if (header is null) return new ResponseDto { Id = (int)clientQuotationApprovalHeaderId!, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.ClientQuotationApproval, GenericMessageData.NotFound) };

            header.IsClosed = isClosed;
            _repository.Update(header);
            await _repository.SaveChanges();
            return new ResponseDto { Id = (int)clientQuotationApprovalHeaderId!, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.ClientQuotationApproval, isClosed ? GenericMessageData.ClosedSuccessfully : GenericMessageData.OpenedSuccessfully) };
        }
        public async Task<ResponseDto> SaveClientQuotationApprovalHeader(ClientQuotationApprovalHeaderDto clientQuotationApprovalHeader, bool hasApprove, bool approved, int? requestId)
		{
            var separateYears = await _applicationSettingService.SeparateYears(clientQuotationApprovalHeader.StoreId);

            if (hasApprove)
            {
                if (clientQuotationApprovalHeader.ClientQuotationApprovalHeaderId == 0)
                {
                    return await CreateClientQuotationApprovalHeader(clientQuotationApprovalHeader, hasApprove, approved, requestId, separateYears);
                }
                else
                {
                    return await UpdateClientQuotationApprovalHeader(clientQuotationApprovalHeader);
                }
            }
            else
            {
                var clientQuotationApprovalHeaderExist = await IsClientQuotationApprovalCodeExist(clientQuotationApprovalHeader.ClientQuotationApprovalHeaderId, clientQuotationApprovalHeader.DocumentCode, clientQuotationApprovalHeader.StoreId, separateYears, clientQuotationApprovalHeader.DocumentDate, clientQuotationApprovalHeader.Prefix, clientQuotationApprovalHeader.Suffix);
                if (clientQuotationApprovalHeaderExist.Success)
                {
                    var nextClientQuotationApprovalCode = await GetNextClientQuotationApprovalCode(clientQuotationApprovalHeader.StoreId, separateYears, clientQuotationApprovalHeader.DocumentDate, clientQuotationApprovalHeader.Prefix, clientQuotationApprovalHeader.Suffix);
                    return new ResponseDto() { Id = nextClientQuotationApprovalCode, ResponseType = ResponseTypeData.Confirm, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.ClientQuotationApproval, GenericMessageData.CodeAlreadyExist, $"{nextClientQuotationApprovalCode}") };
                }
                else
                {
                    if (clientQuotationApprovalHeader.ClientQuotationApprovalHeaderId == 0)
                    {
                        return await CreateClientQuotationApprovalHeader(clientQuotationApprovalHeader, hasApprove, approved, requestId, separateYears);
                    }
                    else
                    {
                        return await UpdateClientQuotationApprovalHeader(clientQuotationApprovalHeader);
                    }
                }
            }
        }

        public async Task<ResponseDto> CreateClientQuotationApprovalHeader(ClientQuotationApprovalHeaderDto clientQuotationApprovalHeader, bool hasApprove, bool approved, int? requestId, bool separateYears)
        {
            int clientQuotationApprovalCode;
            string? prefix;
            string? suffix;
            var nextClientQuotationApprovalCode = await GetClientQuotationApprovalCodeInternal(clientQuotationApprovalHeader.StoreId, separateYears, clientQuotationApprovalHeader.DocumentDate);
            if (hasApprove && approved)
            {
                clientQuotationApprovalCode = nextClientQuotationApprovalCode.NextCode;
                prefix = nextClientQuotationApprovalCode.Prefix;
                suffix = nextClientQuotationApprovalCode.Suffix;
            }
            else
            {
                clientQuotationApprovalCode = clientQuotationApprovalHeader.DocumentCode != 0 ? clientQuotationApprovalHeader.DocumentCode : nextClientQuotationApprovalCode.NextCode;
                prefix = string.IsNullOrWhiteSpace(clientQuotationApprovalHeader.Prefix) ? nextClientQuotationApprovalCode.Prefix : clientQuotationApprovalHeader.Prefix;
                suffix = string.IsNullOrWhiteSpace(clientQuotationApprovalHeader.Suffix) ? nextClientQuotationApprovalCode.Suffix : clientQuotationApprovalHeader.Suffix;
            }

            var clientQuotationApprovalHeaderId = await GetNextId();
            var newClientQuotationApprovalHeader = new ClientQuotationApprovalHeader()
            {
				ClientQuotationHeaderApprovalId = clientQuotationApprovalHeaderId,
                Prefix = prefix,
                DocumentCode = clientQuotationApprovalCode,
                Suffix = suffix,
                ClientQuotationHeaderId = clientQuotationApprovalHeader.ClientQuotationHeaderId,
                ClientId = clientQuotationApprovalHeader.ClientId,
                SellerId = clientQuotationApprovalHeader.SellerId,
                DocumentReference = hasApprove ? $"{DocumentReferenceData.Approval}{requestId}" : $"{DocumentReferenceData.ClientQuotationApproval}{clientQuotationApprovalHeaderId}",
                StoreId = clientQuotationApprovalHeader.StoreId,
                TaxTypeId = clientQuotationApprovalHeader.TaxTypeId,
                DocumentDate = clientQuotationApprovalHeader.DocumentDate,
                EntryDate = DateHelper.GetDateTimeNow(),
                Reference = clientQuotationApprovalHeader.Reference,
                TotalValue = clientQuotationApprovalHeader.TotalValue,
                DiscountPercent = clientQuotationApprovalHeader.DiscountPercent,
                DiscountValue = clientQuotationApprovalHeader.DiscountValue,
                TotalItemDiscount = clientQuotationApprovalHeader.TotalItemDiscount,
                GrossValue = clientQuotationApprovalHeader.GrossValue,
                VatValue = clientQuotationApprovalHeader.VatValue,
                SubNetValue = clientQuotationApprovalHeader.SubNetValue,
                OtherTaxValue = clientQuotationApprovalHeader.OtherTaxValue,
                NetValueBeforeAdditionalDiscount = clientQuotationApprovalHeader.NetValueBeforeAdditionalDiscount,
                AdditionalDiscountValue = clientQuotationApprovalHeader.AdditionalDiscountValue,
                NetValue = clientQuotationApprovalHeader.NetValue,
                TotalCostValue = clientQuotationApprovalHeader.TotalCostValue,
                RemarksAr = clientQuotationApprovalHeader.RemarksAr,
                RemarksEn = clientQuotationApprovalHeader.RemarksEn,
                IsClosed = false,
                ArchiveHeaderId = clientQuotationApprovalHeader.ArchiveHeaderId,


                CreatedAt = DateHelper.GetDateTimeNow(),
                UserNameCreated = await _httpContextAccessor!.GetUserName(),
                IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
                Hide = false,
            };

            var clientQuotationApprovalHeaderValidator = await new ClientQuotationApprovalHeaderValidator(_localizer).ValidateAsync(newClientQuotationApprovalHeader);
            var validationResult = clientQuotationApprovalHeaderValidator.IsValid;
            if (validationResult)
            {
                await _repository.Insert(newClientQuotationApprovalHeader);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = newClientQuotationApprovalHeader.ClientQuotationHeaderApprovalId, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.ClientQuotationApproval, GenericMessageData.CreatedSuccessWithCode, $"{newClientQuotationApprovalHeader.Prefix}{newClientQuotationApprovalHeader.DocumentCode}{newClientQuotationApprovalHeader.Suffix}") };
            }
            else
            {
                return new ResponseDto() { Id = newClientQuotationApprovalHeader.ClientQuotationHeaderApprovalId, Success = false, Message = clientQuotationApprovalHeaderValidator.ToString("~") };
            }
        }

        public async Task<ResponseDto> UpdateClientQuotationApprovalHeader(ClientQuotationApprovalHeaderDto clientQuotationApprovalHeader)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var clientQuotationApprovalHeaderDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.ClientQuotationHeaderApprovalId == clientQuotationApprovalHeader.ClientQuotationApprovalHeaderId);
            if (clientQuotationApprovalHeaderDb != null)
            {
                if (clientQuotationApprovalHeaderDb.IsClosed)
                {
                    return new ResponseDto() { Id = clientQuotationApprovalHeader.ClientQuotationApprovalHeaderId, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.ClientQuotationApproval, GenericMessageData.CannotUpdateBecauseClosed) };
                }

                clientQuotationApprovalHeaderDb.ClientQuotationHeaderId = clientQuotationApprovalHeader.ClientQuotationHeaderId;
                clientQuotationApprovalHeaderDb.ClientId = clientQuotationApprovalHeader.ClientId;
                clientQuotationApprovalHeaderDb.SellerId = clientQuotationApprovalHeader.SellerId;
                clientQuotationApprovalHeaderDb.StoreId = clientQuotationApprovalHeader.StoreId;
                clientQuotationApprovalHeaderDb.TaxTypeId = clientQuotationApprovalHeader.TaxTypeId;
                clientQuotationApprovalHeaderDb.DocumentDate = clientQuotationApprovalHeader.DocumentDate;
                clientQuotationApprovalHeaderDb.Reference = clientQuotationApprovalHeader.Reference;
                clientQuotationApprovalHeaderDb.TotalValue = clientQuotationApprovalHeader.TotalValue;
                clientQuotationApprovalHeaderDb.DiscountPercent = clientQuotationApprovalHeader.DiscountPercent;
                clientQuotationApprovalHeaderDb.DiscountValue = clientQuotationApprovalHeader.DiscountValue;
                clientQuotationApprovalHeaderDb.TotalItemDiscount = clientQuotationApprovalHeader.TotalItemDiscount;
                clientQuotationApprovalHeaderDb.GrossValue = clientQuotationApprovalHeader.GrossValue;
                clientQuotationApprovalHeaderDb.VatValue = clientQuotationApprovalHeader.VatValue;
                clientQuotationApprovalHeaderDb.SubNetValue = clientQuotationApprovalHeader.SubNetValue;
                clientQuotationApprovalHeaderDb.OtherTaxValue = clientQuotationApprovalHeader.OtherTaxValue;
                clientQuotationApprovalHeaderDb.NetValueBeforeAdditionalDiscount = clientQuotationApprovalHeader.NetValueBeforeAdditionalDiscount;
                clientQuotationApprovalHeaderDb.AdditionalDiscountValue = clientQuotationApprovalHeader.AdditionalDiscountValue;
                clientQuotationApprovalHeaderDb.NetValue = clientQuotationApprovalHeader.NetValue;
                clientQuotationApprovalHeaderDb.TotalCostValue = clientQuotationApprovalHeader.TotalCostValue;
                clientQuotationApprovalHeaderDb.RemarksAr = clientQuotationApprovalHeader.RemarksAr;
                clientQuotationApprovalHeaderDb.RemarksEn = clientQuotationApprovalHeader.RemarksEn;
                clientQuotationApprovalHeaderDb.ArchiveHeaderId = clientQuotationApprovalHeader.ArchiveHeaderId;
                
                clientQuotationApprovalHeaderDb.ModifiedAt = DateHelper.GetDateTimeNow();
                clientQuotationApprovalHeaderDb.UserNameModified = await _httpContextAccessor!.GetUserName();
                clientQuotationApprovalHeaderDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();


                var clientQuotationApprovalHeaderValidator = await new ClientQuotationApprovalHeaderValidator(_localizer).ValidateAsync(clientQuotationApprovalHeaderDb);
                var validationResult = clientQuotationApprovalHeaderValidator.IsValid;
                if (validationResult)
                {
                    _repository.Update(clientQuotationApprovalHeaderDb);
                    await _repository.SaveChanges();
                    return new ResponseDto() { Id = clientQuotationApprovalHeaderDb.ClientQuotationHeaderApprovalId, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.ClientQuotationApproval, GenericMessageData.UpdatedSuccessWithCode, $"{clientQuotationApprovalHeaderDb.Prefix}{clientQuotationApprovalHeaderDb.DocumentCode}{clientQuotationApprovalHeaderDb.Suffix}") };
                }
                else
                {
                    return new ResponseDto() { Id = 0, Success = false, Message = clientQuotationApprovalHeaderValidator.ToString("~") };
                }
            }
            return new ResponseDto() { Id = 0, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.ClientQuotationApproval, GenericMessageData.NotFound) };
        }

        public async Task<ResponseDto> IsClientQuotationApprovalCodeExist(int clientQuotationApprovalHeaderId, int clientQuotationApprovalCode, int storeId, bool separateYears, DateTime documentDate, string? prefix, string? suffix)
		{
			var clientQuotationApprovalHeader = await _repository.GetAll().FirstOrDefaultAsync(x => x.ClientQuotationHeaderApprovalId != clientQuotationApprovalHeaderId && (x.StoreId == storeId && (!separateYears || x.DocumentDate.Year == documentDate.Year) && x.Prefix == prefix && x.DocumentCode == clientQuotationApprovalCode && x.Suffix == suffix));
			if (clientQuotationApprovalHeader is not null)
			{
                return new ResponseDto() { Id = clientQuotationApprovalHeader.ClientQuotationHeaderApprovalId, Success = true };
            }
			return new ResponseDto() {  Id = 0, Success = false };
		}

        public async Task<int> GetNextId()
        {
            int id = 1;
            try { id = await _repository.GetAll().MaxAsync(a => a.ClientQuotationHeaderApprovalId) + 1; } catch { id = 1; }
            return id;
        }

        public async Task<ResponseDto> DeleteClientQuotationApprovalHeader(int id)
		{
            var clientQuotationApprovalHeader = await _repository.GetAll().FirstOrDefaultAsync(x => x.ClientQuotationHeaderApprovalId == id);
            if (clientQuotationApprovalHeader != null)
            {
                _repository.Delete(clientQuotationApprovalHeader);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = id, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.ClientQuotationApproval, GenericMessageData.DeleteSuccessWithCode, $"{clientQuotationApprovalHeader.Prefix}{clientQuotationApprovalHeader.DocumentCode}{clientQuotationApprovalHeader.Suffix}") };
            }
            return new ResponseDto() { Id = id, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.ClientQuotationApproval, GenericMessageData.NotFound) };
        }

        public async Task<int> GetNextClientQuotationApprovalCode(int storeId, bool separateYears, DateTime documentDate, string? prefix, string? suffix)
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
