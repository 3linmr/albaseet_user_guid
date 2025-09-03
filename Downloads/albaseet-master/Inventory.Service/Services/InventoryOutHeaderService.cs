using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Contracts.Menus;
using Shared.CoreOne.Contracts.Modules;
using Shared.Service;
using Inventory.CoreOne.Models.Domain;
using Shared.CoreOne;
using Inventory.CoreOne.Contracts;
using Shared.Helper.Models.Dtos;
using Inventory.CoreOne.Models.Dtos.ViewModels;
using Shared.Helper.Identity;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Microsoft.EntityFrameworkCore;
using Inventory.Service.Validators;
using Shared.CoreOne.Models.StaticData;
using Purchases.CoreOne.Models.Domain;
using Shared.Helper.Logic;
using Shared.CoreOne.Contracts.Settings;

namespace Inventory.Service.Services
{
	public class InventoryOutHeaderService : BaseService<InventoryOutHeader>, IInventoryOutHeaderService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IStoreService _storeService;
        private readonly IGenericMessageService _genericMessageService;
		private readonly IStringLocalizer<InventoryOutHeaderService> _localizer;
		private readonly IMenuEncodingService _menuEncodingService;
        private readonly IApplicationSettingService _applicationSettingService;

		public InventoryOutHeaderService(IRepository<InventoryOutHeader> repository, IHttpContextAccessor httpContextAccessor, IStoreService storeService, IGenericMessageService genericMessageService, IStringLocalizer<InventoryOutHeaderService> localizer, IMenuEncodingService menuEncodingService, IApplicationSettingService applicationSettingService) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
			_storeService = storeService;
            _genericMessageService = genericMessageService;
			_localizer = localizer;
			_menuEncodingService = menuEncodingService;
            _applicationSettingService = applicationSettingService;
		}

		public IQueryable<InventoryOutHeaderDto> GetInventoryOutHeaders()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var data = from inventoryOutHeader in _repository.GetAll()
				from store in _storeService.GetAll().Where(x => x.StoreId == inventoryOutHeader.StoreId)
				select new InventoryOutHeaderDto()
				{
					InventoryOutHeaderId = inventoryOutHeader.InventoryOutHeaderId,
					Prefix = inventoryOutHeader.Prefix,
					InventoryOutCode = inventoryOutHeader.InventoryOutCode,
					Suffix = inventoryOutHeader.Suffix,
					InventoryOutFullCode = $"{inventoryOutHeader.Prefix}{inventoryOutHeader.InventoryOutCode}{inventoryOutHeader.Suffix}",
					DocumentReference = inventoryOutHeader.DocumentReference,
					StoreId = inventoryOutHeader.StoreId,
					StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
					DocumentDate = inventoryOutHeader.DocumentDate,
					EntryDate = inventoryOutHeader.EntryDate,
					Reference = inventoryOutHeader.Reference,
					TotalConsumerValue = inventoryOutHeader.TotalConsumerValue,
					TotalCostValue = inventoryOutHeader.TotalCostValue,
					RemarksAr = inventoryOutHeader.RemarksAr,
					RemarksEn = inventoryOutHeader.RemarksEn,
					IsClosed = inventoryOutHeader.IsClosed,
					ArchiveHeaderId = inventoryOutHeader.ArchiveHeaderId,

                    CreatedAt = inventoryOutHeader.CreatedAt,
                    UserNameCreated = inventoryOutHeader.UserNameCreated,
                    IpAddressCreated = inventoryOutHeader.IpAddressCreated,

                    ModifiedAt = inventoryOutHeader.ModifiedAt,
                    UserNameModified = inventoryOutHeader.UserNameModified,
                    IpAddressModified = inventoryOutHeader.IpAddressModified,
                };
			return data;
		}

        public IQueryable<InventoryOutHeaderDto> GetUserInventoryOutHeaders()
        {
            var userStore = _httpContextAccessor.GetCurrentUserStore();
            return GetInventoryOutHeaders().Where(x => x.StoreId == userStore);
        }

        public IQueryable<InventoryOutHeaderDto> GetInventoryOutHeadersByStoreId(int storeId)
		{
			return GetInventoryOutHeaders().Where(x  => x.StoreId == storeId);
		}

		public async Task<InventoryOutHeaderDto?> GetInventoryOutHeaderById(int id)
		{
			return await GetInventoryOutHeaders().FirstOrDefaultAsync(x => x.InventoryOutHeaderId == id);
		}

        public async Task<DocumentCodeDto> GetInventoryOutCode(int storeId, DateTime documentDate)
        {
            var separateYears = await _applicationSettingService.SeparateYears(storeId);
            return await GetInventoryOutCodeInternal(storeId, separateYears, documentDate);
        }

		public async Task<DocumentCodeDto> GetInventoryOutCodeInternal(int storeId, bool separateYears, DateTime documentDate)
		{
			var encoding = await _menuEncodingService.GetMenuEncoding(storeId, MenuCodeData.InventoryOut);
			var code = await GetNextInventoryOutCode(storeId, separateYears, documentDate, encoding.Prefix, encoding.Suffix);
			return new DocumentCodeDto() { NextCode = code, Prefix = encoding.Prefix, Suffix = encoding.Suffix };
		}

		public async Task<ResponseDto> SaveInventoryOutHeader(InventoryOutHeaderDto inventoryOutHeader, bool hasApprove, bool approved, int? requestId)
		{
            var separateYears = await _applicationSettingService.SeparateYears(inventoryOutHeader.StoreId);

            if (hasApprove)
			{
                if (inventoryOutHeader.InventoryOutHeaderId == 0)
                {
                    return await CreateInventoryOutHeader(inventoryOutHeader, hasApprove, approved, requestId, separateYears);
                }
                else
                {
                    return await UpdateInventoryOutHeader(inventoryOutHeader);
                }
            }
            else
            {
                var inventoryOutHeaderExist = await IsInventoryOutCodeExist(inventoryOutHeader.InventoryOutHeaderId, inventoryOutHeader.InventoryOutCode, inventoryOutHeader.StoreId, separateYears, inventoryOutHeader.DocumentDate, inventoryOutHeader.Prefix, inventoryOutHeader.Suffix);
                if (inventoryOutHeaderExist.Success)
                {
                    var nextInventoryOutCode = await GetNextInventoryOutCode(inventoryOutHeader.StoreId, separateYears, inventoryOutHeader.DocumentDate, inventoryOutHeader.Prefix, inventoryOutHeader.Suffix);
                    return new ResponseDto() { Id = nextInventoryOutCode, ResponseType = ResponseTypeData.Confirm, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.InventoryOut, GenericMessageData.CodeAlreadyExist, $"{nextInventoryOutCode}") };
                }
                else
                {
                    if (inventoryOutHeader.InventoryOutHeaderId == 0)
                    {
                        return await CreateInventoryOutHeader(inventoryOutHeader, hasApprove, approved, requestId, separateYears);
                    }
                    else
                    {
                        return await UpdateInventoryOutHeader(inventoryOutHeader);
                    }
                }
            }
        }

        public async Task<ResponseDto> CreateInventoryOutHeader(InventoryOutHeaderDto inventoryOutHeader, bool hasApprove, bool approved, int? requestId, bool separateYears)
        {
            int inventoryOutCode;
            string? prefix;
            string? suffix;
            var nextInventoryOutCode = await GetInventoryOutCodeInternal(inventoryOutHeader.StoreId, separateYears, inventoryOutHeader.DocumentDate);
            if (hasApprove && approved)
            {
                inventoryOutCode = nextInventoryOutCode.NextCode;
                prefix = nextInventoryOutCode.Prefix;
                suffix = nextInventoryOutCode.Suffix;
            }
            else
            {
                inventoryOutCode = inventoryOutHeader.InventoryOutCode != 0 ? inventoryOutHeader.InventoryOutCode : nextInventoryOutCode.NextCode;
                prefix = string.IsNullOrWhiteSpace(inventoryOutHeader.Prefix) ? nextInventoryOutCode.Prefix : inventoryOutHeader.Prefix;
                suffix = string.IsNullOrWhiteSpace(inventoryOutHeader.Suffix) ? nextInventoryOutCode.Suffix : inventoryOutHeader.Suffix;
            }

            var inventoryOutHeaderId = await GetNextId();
            var newInventoryOutHeader = new InventoryOutHeader()
            {
                InventoryOutHeaderId = inventoryOutHeaderId,
                Prefix = prefix,
                InventoryOutCode = inventoryOutCode,
                Suffix = suffix,
                DocumentReference = hasApprove ? $"{DocumentReferenceData.Approval}{requestId}" : $"{DocumentReferenceData.InventoryOut}{inventoryOutHeaderId}",
                StoreId = inventoryOutHeader.StoreId,
                DocumentDate = inventoryOutHeader.DocumentDate,
                EntryDate = DateHelper.GetDateTimeNow(),
                Reference = inventoryOutHeader.Reference,
                TotalConsumerValue = inventoryOutHeader.TotalConsumerValue,
                TotalCostValue = inventoryOutHeader.TotalCostValue,
                RemarksAr = inventoryOutHeader.RemarksAr,
                RemarksEn = inventoryOutHeader.RemarksEn,
                IsClosed = false,
                ArchiveHeaderId = inventoryOutHeader.ArchiveHeaderId,

                CreatedAt = DateHelper.GetDateTimeNow(),
                UserNameCreated = await _httpContextAccessor!.GetUserName(),
                IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
                Hide = false,
            };

            var inventoryOutHeaderValidator = await new InventoryOutHeaderValidator(_localizer).ValidateAsync(newInventoryOutHeader);
            var validationResult = inventoryOutHeaderValidator.IsValid;
            if (validationResult)
            {
                await _repository.Insert(newInventoryOutHeader);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = newInventoryOutHeader.InventoryOutHeaderId, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.InventoryOut, GenericMessageData.CreatedSuccessWithCode, $"{newInventoryOutHeader.Prefix}{newInventoryOutHeader.InventoryOutCode}{newInventoryOutHeader.Suffix}") };
            }
            else
            {
                return new ResponseDto() { Id = newInventoryOutHeader.InventoryOutHeaderId, Success = false, Message = inventoryOutHeaderValidator.ToString("~") };
            }
        }

        public async Task<ResponseDto> UpdateInventoryOutHeader(InventoryOutHeaderDto inventoryOutHeader)
        {
            var inventoryOutHeaderDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.InventoryOutHeaderId == inventoryOutHeader.InventoryOutHeaderId);
            if (inventoryOutHeaderDb != null)
            {
                if (inventoryOutHeaderDb.IsClosed)
                {
                    return new ResponseDto() { Id = inventoryOutHeader.InventoryOutHeaderId, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.InventoryOut, GenericMessageData.CannotUpdateBecauseClosed) };
                }

                inventoryOutHeaderDb.StoreId = inventoryOutHeader.StoreId;
                inventoryOutHeaderDb.DocumentDate = inventoryOutHeader.DocumentDate;
                inventoryOutHeaderDb.Reference = inventoryOutHeader.Reference;
                inventoryOutHeaderDb.TotalConsumerValue = inventoryOutHeader.TotalConsumerValue;
                inventoryOutHeaderDb.TotalCostValue = inventoryOutHeader.TotalCostValue;
                inventoryOutHeaderDb.RemarksAr = inventoryOutHeader.RemarksAr;
                inventoryOutHeaderDb.RemarksEn = inventoryOutHeader.RemarksEn;
                inventoryOutHeaderDb.ArchiveHeaderId = inventoryOutHeader.ArchiveHeaderId;

                inventoryOutHeaderDb.ModifiedAt = DateHelper.GetDateTimeNow();
                inventoryOutHeaderDb.UserNameModified = await _httpContextAccessor!.GetUserName();
                inventoryOutHeaderDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();


                var inventoryOutHeaderValidator = await new InventoryOutHeaderValidator(_localizer).ValidateAsync(inventoryOutHeaderDb);
                var validationResult = inventoryOutHeaderValidator.IsValid;
                if (validationResult)
                {
                    _repository.Update(inventoryOutHeaderDb);
                    await _repository.SaveChanges();
                    return new ResponseDto() { Id = inventoryOutHeaderDb.InventoryOutHeaderId, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.InventoryOut, GenericMessageData.UpdatedSuccessWithCode, $"{inventoryOutHeaderDb.Prefix}{inventoryOutHeaderDb.InventoryOutCode}{inventoryOutHeaderDb.Suffix}") };
                }
                else
                {
                    return new ResponseDto() { Id = 0, Success = false, Message = inventoryOutHeaderValidator.ToString("~") };
                }
            }
            return new ResponseDto() { Id = 0, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.InventoryOut, GenericMessageData.NotFound) };
        }

        public async Task<ResponseDto> IsInventoryOutCodeExist(int inventoryOutHeaderId, int inventoryOutCode, int storeId, bool separateYears, DateTime documentDate, string? prefix, string? suffix)
		{
			var inventoryOutHeader = await _repository.GetAll().FirstOrDefaultAsync(x => x.InventoryOutHeaderId != inventoryOutHeaderId && (x.StoreId == storeId && (!separateYears || x.DocumentDate.Year == documentDate.Year) && x.Prefix == prefix && x.InventoryOutCode == inventoryOutCode && x.Suffix == suffix));
			if (inventoryOutHeader is not null)
			{
                return new ResponseDto() { Id = inventoryOutHeader.InventoryOutHeaderId, Success = true };
            }
			return new ResponseDto() {  Id = 0, Success = false };
		}

        public async Task<int> GetNextId()
        {
            int id = 1;
            try { id = await _repository.GetAll().MaxAsync(a => a.InventoryOutHeaderId) + 1; } catch { id = 1; }
            return id;
        }

        public async Task<ResponseDto> DeleteInventoryOutHeader(int id)
		{
            var inventoryOutHeader = await _repository.GetAll().FirstOrDefaultAsync(x => x.InventoryOutHeaderId == id);
            if (inventoryOutHeader != null)
            {
                _repository.Delete(inventoryOutHeader);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = id, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.InventoryOut, GenericMessageData.DeleteSuccessWithCode, $"{inventoryOutHeader.Prefix}{inventoryOutHeader.InventoryOutCode}{inventoryOutHeader.Suffix}") };
            }
            return new ResponseDto() { Id = id, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.InventoryOut, GenericMessageData.NotFound) };
        }

        public async Task<int> GetNextInventoryOutCode(int storeId, bool separateYears, DateTime documentDate, string? prefix, string? suffix)
        {
            int id = 1;
            try
            {
                id = await _repository.GetAll().AsNoTracking().Where(x => (!separateYears || x.DocumentDate.Year == documentDate.Year) && x.Prefix == prefix && x.Suffix == suffix && x.StoreId == storeId).MaxAsync(a => a.InventoryOutCode) + 1;
            }
            catch { id = 1; }
            return id;
        }
    }
}
