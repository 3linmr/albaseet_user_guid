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
using Shared.Helper.Logic;
using Shared.Service.Services.Modules;
using Shared.CoreOne.Contracts.Settings;

namespace Sales.Service.Services
{
	public class ClientPriceRequestHeaderService : BaseService<ClientPriceRequestHeader>, IClientPriceRequestHeaderService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IStoreService _storeService;
        private readonly IClientService _clientService;
		private readonly IStringLocalizer<ClientPriceRequestHeaderService> _localizer;
		private readonly IMenuEncodingService _menuEncodingService;
        private readonly IGenericMessageService _genericMessageService;
        private readonly ISellerService _sellerService;
        private readonly IApplicationSettingService _applicationSettingService;

		public ClientPriceRequestHeaderService(IRepository<ClientPriceRequestHeader> repository, IHttpContextAccessor httpContextAccessor, IStoreService storeService, IClientService clientService, IStringLocalizer<ClientPriceRequestHeaderService> localizer, IMenuEncodingService menuEncodingService, IGenericMessageService genericMessageService, ISellerService sellerService, IApplicationSettingService applicationSettingService) : base(repository)
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

		public IQueryable<ClientPriceRequestHeaderDto> GetClientPriceRequestHeaders()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var data = from clientPriceRequestHeader in _repository.GetAll()
                       from store in _storeService.GetAll().Where(x => x.StoreId == clientPriceRequestHeader.StoreId)
                       from client in _clientService.GetAll().Where(x => x.ClientId == clientPriceRequestHeader.ClientId)
                       from seller in _sellerService.GetAll().Where(x => x.SellerId ==  clientPriceRequestHeader.SellerId).DefaultIfEmpty()
                       select new ClientPriceRequestHeaderDto()
                       {
                           ClientPriceRequestHeaderId = clientPriceRequestHeader.ClientPriceRequestHeaderId,
                           Prefix = clientPriceRequestHeader.Prefix,
                           DocumentCode = clientPriceRequestHeader.DocumentCode,
                           Suffix = clientPriceRequestHeader.Suffix,
                           DocumentFullCode = $"{clientPriceRequestHeader.Prefix}{clientPriceRequestHeader.DocumentCode}{clientPriceRequestHeader.Suffix}",
                           DocumentReference = clientPriceRequestHeader.DocumentReference,
                           StoreId = clientPriceRequestHeader.StoreId,
                           StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
                           ClientId = clientPriceRequestHeader.ClientId,
                           ClientCode = client.ClientCode,
                           ClientName = language == LanguageCode.Arabic ? client.ClientNameAr : client.ClientNameEn,
						   SellerId = seller != null ? seller.SellerId : null,
                           SellerCode = seller != null ? seller.SellerCode : null,
						   SellerName = seller != null ? language == LanguageCode.Arabic ? seller.SellerNameAr : seller.SellerNameEn : null,
						   DocumentDate = clientPriceRequestHeader.DocumentDate,
                           EntryDate = clientPriceRequestHeader.EntryDate,
                           Reference = clientPriceRequestHeader.Reference,
                           ConsumerValue = clientPriceRequestHeader.ConsumerValue,
                           CostValue = clientPriceRequestHeader.CostValue,
                           RemarksAr = clientPriceRequestHeader.RemarksAr,
                           RemarksEn = clientPriceRequestHeader.RemarksEn,
                           IsClosed = clientPriceRequestHeader.IsClosed,
                           ArchiveHeaderId = clientPriceRequestHeader.ArchiveHeaderId,

                           CreatedAt = clientPriceRequestHeader.CreatedAt,
                           UserNameCreated = clientPriceRequestHeader.UserNameCreated,
                           IpAddressCreated = clientPriceRequestHeader.IpAddressCreated,

                           ModifiedAt = clientPriceRequestHeader.ModifiedAt,
                           UserNameModified = clientPriceRequestHeader.UserNameModified,
                           IpAddressModified = clientPriceRequestHeader.IpAddressModified,
                       };

            return data;
        }

        public IQueryable<ClientPriceRequestHeaderDto> GetUserClientPriceRequestHeaders()
        {
            var userStore = _httpContextAccessor.GetCurrentUserStore();
            return GetClientPriceRequestHeaders().Where(x => x.StoreId == userStore);
        }

        public IQueryable<ClientPriceRequestHeaderDto> GetClientPriceRequestHeadersByStoreId(int storeId, int? clientId, int clientPriceRequestHeaderId)
		{
            clientId ??= 0;
            if (clientPriceRequestHeaderId == 0)
            {
                return GetClientPriceRequestHeaders().Where(x => x.StoreId == storeId && (clientId == 0 || x.ClientId == clientId) && x.IsClosed == false);
            }
            else
            {
                return GetClientPriceRequestHeaders().Where(x => x.ClientPriceRequestHeaderId == clientPriceRequestHeaderId);
            }
        }

		public async Task<ClientPriceRequestHeaderDto?> GetClientPriceRequestHeaderById(int id)
		{
			return await GetClientPriceRequestHeaders().FirstOrDefaultAsync(x => x.ClientPriceRequestHeaderId == id);
		}

        public async Task<DocumentCodeDto> GetClientPriceRequestCode(int storeId, DateTime documentDate)
        {
            var separateYears = await _applicationSettingService.SeparateYears(storeId);
            return await GetClientPriceRequestCodeInternal(storeId, separateYears, documentDate);
        }

        public async Task<DocumentCodeDto> GetClientPriceRequestCodeInternal(int storeId, bool separateYears, DateTime documentDate)
        {
            var encoding = await _menuEncodingService.GetMenuEncoding(storeId, MenuCodeData.ClientPriceRequest);
            var code = await GetNextClientPriceRequestCode(storeId, separateYears, documentDate, encoding.Prefix, encoding.Suffix);
            return new DocumentCodeDto() { NextCode = code, Prefix = encoding.Prefix, Suffix = encoding.Suffix };
        }

        public async Task<ResponseDto> UpdateClosed(int? clientPriceRequestHeaderId, bool isClosed)
        {
            var header = await _repository.FindBy(x => x.ClientPriceRequestHeaderId == clientPriceRequestHeaderId).FirstOrDefaultAsync();
            if (header is null) return new ResponseDto { Id = (int)clientPriceRequestHeaderId!, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.ClientPriceRequest, GenericMessageData.NotFound) };

            header.IsClosed = isClosed;
            _repository.Update(header);
            await _repository.SaveChanges();

            return new ResponseDto { Id = (int)clientPriceRequestHeaderId!, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.ClientPriceRequest, isClosed? GenericMessageData.ClosedSuccessfully : GenericMessageData.OpenedSuccessfully) };
        }

        public async Task<ResponseDto> SaveClientPriceRequestHeader(ClientPriceRequestHeaderDto clientPriceRequestHeader, bool hasApprove, bool approved, int? requestId)
		{
            var separateYears = await _applicationSettingService.SeparateYears(clientPriceRequestHeader.StoreId);

            if (hasApprove)
            {
                if (clientPriceRequestHeader.ClientPriceRequestHeaderId == 0)
                {
                    return await CreateClientPriceRequestHeader(clientPriceRequestHeader, hasApprove, approved, requestId, separateYears);
                }
                else
                {
                    return await UpdateClientPriceRequestHeader(clientPriceRequestHeader);
                }
            }
            else
            {
                var clientPriceRequestHeaderExist = await IsClientPriceRequestCodeExist(clientPriceRequestHeader.ClientPriceRequestHeaderId, clientPriceRequestHeader.DocumentCode, clientPriceRequestHeader.StoreId, separateYears, clientPriceRequestHeader.DocumentDate, clientPriceRequestHeader.Prefix, clientPriceRequestHeader.Suffix);
                if (clientPriceRequestHeaderExist.Success)
                {
                    var nextClientPriceRequestCode = await GetNextClientPriceRequestCode(clientPriceRequestHeader.StoreId, separateYears, clientPriceRequestHeader.DocumentDate, clientPriceRequestHeader.Prefix, clientPriceRequestHeader.Suffix);
                    return new ResponseDto() { Id = nextClientPriceRequestCode, ResponseType = ResponseTypeData.Confirm, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.ClientPriceRequest, GenericMessageData.CodeAlreadyExist, $"{nextClientPriceRequestCode}") };
                }
                else
                {
                    if (clientPriceRequestHeader.ClientPriceRequestHeaderId == 0)
                    {
                        return await CreateClientPriceRequestHeader(clientPriceRequestHeader, hasApprove, approved, requestId, separateYears);
                    }
                    else
                    {
                        return await UpdateClientPriceRequestHeader(clientPriceRequestHeader);
                    }
                }
            }
        }

        public async Task<ResponseDto> CreateClientPriceRequestHeader(ClientPriceRequestHeaderDto clientPriceRequestHeader, bool hasApprove, bool approved, int? requestId, bool separateYears)
        {
            int clientPriceRequestCode;
            string? prefix;
            string? suffix;
            var nextClientPriceRequestCode = await GetClientPriceRequestCodeInternal(clientPriceRequestHeader.StoreId, separateYears, clientPriceRequestHeader.DocumentDate);
            if (hasApprove && approved)
            {
                clientPriceRequestCode = nextClientPriceRequestCode.NextCode;
                prefix = nextClientPriceRequestCode.Prefix;
                suffix = nextClientPriceRequestCode.Suffix;
            }
            else
            {
                clientPriceRequestCode = clientPriceRequestHeader.DocumentCode != 0 ? clientPriceRequestHeader.DocumentCode : nextClientPriceRequestCode.NextCode;
                prefix = string.IsNullOrWhiteSpace(clientPriceRequestHeader.Prefix) ? nextClientPriceRequestCode.Prefix : clientPriceRequestHeader.Prefix;
                suffix = string.IsNullOrWhiteSpace(clientPriceRequestHeader.Suffix) ? nextClientPriceRequestCode.Suffix : clientPriceRequestHeader.Suffix;
            }

            var clientPriceRequestHeaderId = await GetNextId();
            var newClientPriceRequestHeader = new ClientPriceRequestHeader()
            {
                ClientPriceRequestHeaderId = clientPriceRequestHeaderId,
                Prefix = prefix,
                DocumentCode = clientPriceRequestCode,
                Suffix = suffix,
                DocumentReference = hasApprove ? $"{DocumentReferenceData.Approval}{requestId}" : $"{DocumentReferenceData.ClientPriceRequest}{clientPriceRequestHeaderId}",
                StoreId = clientPriceRequestHeader.StoreId,
                ClientId = clientPriceRequestHeader.ClientId,
                SellerId = clientPriceRequestHeader.SellerId,
                DocumentDate = clientPriceRequestHeader.DocumentDate,
                EntryDate = DateHelper.GetDateTimeNow(),
                Reference = clientPriceRequestHeader.Reference,
                ConsumerValue = clientPriceRequestHeader.ConsumerValue,
                CostValue = clientPriceRequestHeader.CostValue,
                RemarksAr = clientPriceRequestHeader.RemarksAr,
                RemarksEn = clientPriceRequestHeader.RemarksEn,
                IsClosed = false,
                ArchiveHeaderId = clientPriceRequestHeader.ArchiveHeaderId,
                
                CreatedAt = DateHelper.GetDateTimeNow(),
                UserNameCreated = await _httpContextAccessor!.GetUserName(),
                IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
                Hide = false,
            };

            var clientPriceRequestHeaderValidator = await new ClientPriceRequestHeaderValidator(_localizer).ValidateAsync(newClientPriceRequestHeader);
            var validationResult = clientPriceRequestHeaderValidator.IsValid;
            if (validationResult)
            {
                await _repository.Insert(newClientPriceRequestHeader);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = newClientPriceRequestHeader.ClientPriceRequestHeaderId, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.ClientPriceRequest, GenericMessageData.CreatedSuccessWithCode, $"{newClientPriceRequestHeader.Prefix}{newClientPriceRequestHeader.DocumentCode}{newClientPriceRequestHeader.Suffix}") };
            }
            else
            {
                return new ResponseDto() { Id = newClientPriceRequestHeader.ClientPriceRequestHeaderId, Success = false, Message = clientPriceRequestHeaderValidator.ToString("~") };
            }
        }

        public async Task<ResponseDto> UpdateClientPriceRequestHeader(ClientPriceRequestHeaderDto clientPriceRequestHeader)
        {
            var clientPriceRequestHeaderDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.ClientPriceRequestHeaderId == clientPriceRequestHeader.ClientPriceRequestHeaderId);
            if (clientPriceRequestHeaderDb != null)
            {
                if (clientPriceRequestHeaderDb.IsClosed)
                {
                    return new ResponseDto() { Id = clientPriceRequestHeader.ClientPriceRequestHeaderId, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.ClientPriceRequest, GenericMessageData.CannotUpdateBecauseClosed) };
                }

                clientPriceRequestHeaderDb.StoreId = clientPriceRequestHeader.StoreId;
                clientPriceRequestHeaderDb.ClientId = clientPriceRequestHeader.ClientId;
                clientPriceRequestHeaderDb.SellerId = clientPriceRequestHeader.SellerId;
                clientPriceRequestHeaderDb.DocumentDate = clientPriceRequestHeader.DocumentDate;
                clientPriceRequestHeaderDb.Reference = clientPriceRequestHeader.Reference;
                clientPriceRequestHeaderDb.ConsumerValue = clientPriceRequestHeader.ConsumerValue;
                clientPriceRequestHeaderDb.CostValue = clientPriceRequestHeader.CostValue;
                clientPriceRequestHeaderDb.RemarksAr = clientPriceRequestHeader.RemarksAr;
                clientPriceRequestHeaderDb.RemarksEn = clientPriceRequestHeader.RemarksEn;
                clientPriceRequestHeaderDb.ArchiveHeaderId = clientPriceRequestHeader.ArchiveHeaderId;
                
                clientPriceRequestHeaderDb.ModifiedAt = DateHelper.GetDateTimeNow();
                clientPriceRequestHeaderDb.UserNameModified = await _httpContextAccessor!.GetUserName();
                clientPriceRequestHeaderDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();


                var clientPriceRequestHeaderValidator = await new ClientPriceRequestHeaderValidator(_localizer).ValidateAsync(clientPriceRequestHeaderDb);
                var validationResult = clientPriceRequestHeaderValidator.IsValid;
                if (validationResult)
                {
                    _repository.Update(clientPriceRequestHeaderDb);
                    await _repository.SaveChanges();
                    return new ResponseDto() { Id = clientPriceRequestHeaderDb.ClientPriceRequestHeaderId, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.ClientPriceRequest, GenericMessageData.UpdatedSuccessWithCode, $"{clientPriceRequestHeaderDb.Prefix}{clientPriceRequestHeaderDb.DocumentCode}{clientPriceRequestHeaderDb.Suffix}") };
                }
                else
                {
                    return new ResponseDto() { Id = 0, Success = false, Message = clientPriceRequestHeaderValidator.ToString("~") };
                }
            }
            return new ResponseDto() { Id = 0, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.ClientPriceRequest, GenericMessageData.NotFound) };
        }

        public async Task<ResponseDto> IsClientPriceRequestCodeExist(int clientPriceRequestHeaderId, int clientPriceRequestCode, int storeId, bool separateYears, DateTime documentDate, string? prefix, string? suffix)
		{
			var clientPriceRequestHeader = await _repository.GetAll().FirstOrDefaultAsync(x => x.ClientPriceRequestHeaderId != clientPriceRequestHeaderId && (x.StoreId == storeId && (!separateYears || x.DocumentDate.Year == documentDate.Year) && x.Prefix == prefix && x.DocumentCode == clientPriceRequestCode && x.Suffix == suffix));
			if (clientPriceRequestHeader is not null)
			{
                return new ResponseDto() { Id = clientPriceRequestHeader.ClientPriceRequestHeaderId, Success = true };
            }
			return new ResponseDto() {  Id = 0, Success = false };
		}

        public async Task<int> GetNextId()
        {
            int id = 1;
            try { id = await _repository.GetAll().MaxAsync(a => a.ClientPriceRequestHeaderId) + 1; } catch { id = 1; }
            return id;
        }

        public async Task<ResponseDto> DeleteClientPriceRequestHeader(int id)
		{
            var clientPriceRequestHeader = await _repository.GetAll().FirstOrDefaultAsync(x => x.ClientPriceRequestHeaderId == id);
            if (clientPriceRequestHeader != null)
            {
                _repository.Delete(clientPriceRequestHeader);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = id, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.ClientPriceRequest, GenericMessageData.DeleteSuccessWithCode, $"{clientPriceRequestHeader.Prefix}{clientPriceRequestHeader.DocumentCode}{clientPriceRequestHeader.Suffix}") };
            }
            return new ResponseDto() { Id = id, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.ClientPriceRequest, GenericMessageData.NotFound) };
        }

        public async Task<int> GetNextClientPriceRequestCode(int storeId, bool separateYears, DateTime documentDate, string? prefix, string? suffix)
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
