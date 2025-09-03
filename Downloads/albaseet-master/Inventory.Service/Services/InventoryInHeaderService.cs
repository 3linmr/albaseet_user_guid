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
using Shared.CoreOne.Models.Domain.Modules;

namespace Inventory.Service.Services
{
	public class InventoryInHeaderService : BaseService<InventoryInHeader>, IInventoryInHeaderService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IStoreService _storeService;
		private readonly IStringLocalizer<InventoryInHeaderService> _localizer;
        private readonly IGenericMessageService _genericMessageService;
		private readonly IMenuEncodingService _menuEncodingService;
        private readonly IApplicationSettingService _applicationSettingService;

		public InventoryInHeaderService(IRepository<InventoryInHeader> repository, IHttpContextAccessor httpContextAccessor, IStoreService storeService, IGenericMessageService genericMessageService, IStringLocalizer<InventoryInHeaderService> localizer, IMenuEncodingService menuEncodingService, IApplicationSettingService applicationSettingService) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
			_storeService = storeService;
            _genericMessageService = genericMessageService;
			_localizer = localizer;
			_menuEncodingService = menuEncodingService;
            _applicationSettingService = applicationSettingService;
		}

		public IQueryable<InventoryInHeaderDto> GetInventoryInHeaders()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var data = from inventoryInHeader in _repository.GetAll()
				from store in _storeService.GetAll().Where(x => x.StoreId == inventoryInHeader.StoreId)
				select new InventoryInHeaderDto()
				{
					InventoryInHeaderId = inventoryInHeader.InventoryInHeaderId,
					Prefix = inventoryInHeader.Prefix,
					InventoryInCode = inventoryInHeader.InventoryInCode,
					Suffix = inventoryInHeader.Suffix,
					InventoryInFullCode = $"{inventoryInHeader.Prefix}{inventoryInHeader.InventoryInCode}{inventoryInHeader.Suffix}",
					DocumentReference = inventoryInHeader.DocumentReference,
					StoreId = inventoryInHeader.StoreId,
					StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
					DocumentDate = inventoryInHeader.DocumentDate,
					EntryDate = inventoryInHeader.EntryDate,
					Reference = inventoryInHeader.Reference,
					TotalConsumerValue = inventoryInHeader.TotalConsumerValue,
					TotalCostValue = inventoryInHeader.TotalCostValue,
					RemarksAr = inventoryInHeader.RemarksAr,
					RemarksEn = inventoryInHeader.RemarksEn,
					IsClosed = inventoryInHeader.IsClosed,
					ArchiveHeaderId = inventoryInHeader.ArchiveHeaderId,

                    CreatedAt = inventoryInHeader.CreatedAt,
                    UserNameCreated = inventoryInHeader.UserNameCreated,
                    IpAddressCreated = inventoryInHeader.IpAddressCreated,

                    ModifiedAt = inventoryInHeader.ModifiedAt,
                    UserNameModified = inventoryInHeader.UserNameModified,
                    IpAddressModified = inventoryInHeader.IpAddressModified,
				};
			return data;
		}

        public IQueryable<InventoryInHeaderDto> GetUserInventoryInHeaders()
        {
            var userStore = _httpContextAccessor.GetCurrentUserStore();

            return GetInventoryInHeaders().Where(x => x.StoreId == userStore);
        }

        public IQueryable<InventoryInHeaderDto> GetInventoryInHeadersByStoreId(int storeId)
		{
			return GetInventoryInHeaders().Where(x  => x.StoreId == storeId);
		}

		public async Task<InventoryInHeaderDto?> GetInventoryInHeaderById(int id)
		{
			return await GetInventoryInHeaders().FirstOrDefaultAsync(x => x.InventoryInHeaderId == id);
		}

        public async Task<DocumentCodeDto> GetInventoryInCode(int storeId, DateTime documentDate)
        {
            var separateYears = await _applicationSettingService.SeparateYears(storeId);
            return await GetInventoryInCodeInternal(storeId, separateYears, documentDate);
        }

		public async Task<DocumentCodeDto> GetInventoryInCodeInternal(int storeId, bool separateYears, DateTime documentDate)
		{
			var encoding = await _menuEncodingService.GetMenuEncoding(storeId, MenuCodeData.InventoryIn);
			var code = await GetNextInventoryInCode(storeId, separateYears, documentDate, encoding.Prefix, encoding.Suffix);
			return new DocumentCodeDto() { NextCode = code, Prefix = encoding.Prefix, Suffix = encoding.Suffix };
		}

		public async Task<ResponseDto> SaveInventoryInHeader(InventoryInHeaderDto inventoryInHeader, bool hasApprove, bool approved, int? requestId)
		{
			var separateYears = await _applicationSettingService.SeparateYears(inventoryInHeader.StoreId);

			if (hasApprove)
            {
                if (inventoryInHeader.InventoryInHeaderId == 0)
                {
                    return await CreateInventoryInHeader(inventoryInHeader, hasApprove, approved, requestId, separateYears);
                }
                else
                {
                    return await UpdateInventoryInHeader(inventoryInHeader);
                }
            }
            else
            {
                var inventoryInHeaderExist = await IsInventoryInCodeExist(inventoryInHeader.InventoryInHeaderId, inventoryInHeader.InventoryInCode, inventoryInHeader.StoreId, separateYears, inventoryInHeader.DocumentDate, inventoryInHeader.Prefix, inventoryInHeader.Suffix);
                if (inventoryInHeaderExist.Success)
                {
                    var nextInventoryInCode = await GetNextInventoryInCode(inventoryInHeader.StoreId, separateYears, inventoryInHeader.DocumentDate, inventoryInHeader.Prefix, inventoryInHeader.Suffix);
                    return new ResponseDto() { Id = nextInventoryInCode, ResponseType = ResponseTypeData.Confirm, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.InventoryIn, GenericMessageData.CodeAlreadyExist, $"{nextInventoryInCode}") };
                }
                else
                {
                    if (inventoryInHeader.InventoryInHeaderId == 0)
                    {
                        return await CreateInventoryInHeader(inventoryInHeader, hasApprove, approved, requestId, separateYears);
                    }
                    else
                    {
                        return await UpdateInventoryInHeader(inventoryInHeader);
                    }
                }
            }
        }

        public async Task<ResponseDto> CreateInventoryInHeader(InventoryInHeaderDto inventoryInHeader, bool hasApprove, bool approved, int? requestId, bool separateYears)
        {
            int inventoryInCode;
            string? prefix;
            string? suffix ;
            var nextInventoryInCode = await GetInventoryInCodeInternal(inventoryInHeader.StoreId, separateYears, inventoryInHeader.DocumentDate);
            if (hasApprove && approved)
            {
                    inventoryInCode = nextInventoryInCode.NextCode;
                    prefix = nextInventoryInCode.Prefix;
                    suffix = nextInventoryInCode.Suffix;
            }
            else
            {
                inventoryInCode = inventoryInHeader.InventoryInCode != 0 ? inventoryInHeader.InventoryInCode : nextInventoryInCode.NextCode;
                prefix = string.IsNullOrWhiteSpace(inventoryInHeader.Prefix) ? nextInventoryInCode.Prefix: inventoryInHeader.Prefix;
                suffix = string.IsNullOrWhiteSpace(inventoryInHeader.Suffix) ? nextInventoryInCode.Suffix : inventoryInHeader.Suffix;
            }

            var inventoryInHeaderId = await GetNextId();
            var newInventoryInHeader = new InventoryInHeader()
            {
                InventoryInHeaderId = inventoryInHeaderId,
                Prefix = prefix,
                InventoryInCode = inventoryInCode,
                Suffix = suffix,
                DocumentReference = hasApprove ? $"{DocumentReferenceData.Approval}{requestId}" : $"{DocumentReferenceData.InventoryIn}{inventoryInHeaderId}",
                StoreId = inventoryInHeader.StoreId,
                DocumentDate = inventoryInHeader.DocumentDate,
                EntryDate = DateHelper.GetDateTimeNow(),
                Reference = inventoryInHeader.Reference,
                TotalConsumerValue = inventoryInHeader.TotalConsumerValue,
                TotalCostValue = inventoryInHeader.TotalCostValue,
                RemarksAr = inventoryInHeader.RemarksAr,
                RemarksEn = inventoryInHeader.RemarksEn,
                IsClosed = false,
                ArchiveHeaderId = inventoryInHeader.ArchiveHeaderId,

                CreatedAt = DateHelper.GetDateTimeNow(),
                UserNameCreated = await _httpContextAccessor!.GetUserName(),
                IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
                Hide = false,
            };

            var inventoryInHeaderValidator = await new InventoryInHeaderValidator(_localizer).ValidateAsync(newInventoryInHeader);
            var validationResult = inventoryInHeaderValidator.IsValid;
            if (validationResult)
            {
                await _repository.Insert(newInventoryInHeader);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = newInventoryInHeader.InventoryInHeaderId, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.InventoryIn, GenericMessageData.CreatedSuccessWithCode, $"{newInventoryInHeader.Prefix}{newInventoryInHeader.InventoryInCode}{newInventoryInHeader.Suffix}") };
            }
            else
            {
                return new ResponseDto() { Id = newInventoryInHeader.InventoryInHeaderId, Success = false, Message = inventoryInHeaderValidator.ToString("~") };
            }
        }

        public async Task<ResponseDto> UpdateInventoryInHeader(InventoryInHeaderDto inventoryInHeader)
        {
            var inventoryInHeaderDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.InventoryInHeaderId == inventoryInHeader.InventoryInHeaderId);
            if (inventoryInHeaderDb != null)
            {

                if (inventoryInHeaderDb.IsClosed)
                {
                    return new ResponseDto() { Id = inventoryInHeader.InventoryInHeaderId, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.InventoryIn, GenericMessageData.CannotUpdateBecauseClosed) };
                }

                inventoryInHeaderDb.StoreId = inventoryInHeader.StoreId;
                inventoryInHeaderDb.DocumentDate = inventoryInHeader.DocumentDate;
                inventoryInHeaderDb.Reference = inventoryInHeader.Reference;
                inventoryInHeaderDb.TotalConsumerValue = inventoryInHeader.TotalConsumerValue;
                inventoryInHeaderDb.TotalCostValue = inventoryInHeader.TotalCostValue;
                inventoryInHeaderDb.RemarksAr = inventoryInHeader.RemarksAr;
                inventoryInHeaderDb.RemarksEn = inventoryInHeader.RemarksEn;
                inventoryInHeaderDb.ArchiveHeaderId = inventoryInHeader.ArchiveHeaderId;

                inventoryInHeaderDb.ModifiedAt = DateHelper.GetDateTimeNow();
                inventoryInHeaderDb.UserNameModified = await _httpContextAccessor!.GetUserName();
                inventoryInHeaderDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();


                var inventoryInHeaderValidator = await new InventoryInHeaderValidator(_localizer).ValidateAsync(inventoryInHeaderDb);
                var validationResult = inventoryInHeaderValidator.IsValid;
                if (validationResult)
                {
                    _repository.Update(inventoryInHeaderDb);
                    await _repository.SaveChanges();
                    return new ResponseDto() { Id = inventoryInHeaderDb.InventoryInHeaderId, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.InventoryIn, GenericMessageData.UpdatedSuccessWithCode, $"{inventoryInHeaderDb.Prefix}{inventoryInHeaderDb.InventoryInCode}{inventoryInHeaderDb.Suffix}") };
                }
                else
                {
                    return new ResponseDto() { Id = 0, Success = false, Message = inventoryInHeaderValidator.ToString("~") };
                }
            }
            return new ResponseDto() { Id = 0, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.InventoryIn, GenericMessageData.NotFound) };
        }

        public async Task<ResponseDto> IsInventoryInCodeExist(int inventoryInHeaderId, int inventoryInCode, int storeId, bool separateYears, DateTime documentDate, string? prefix, string? suffix)
		{
            var inventoryInHeader = await _repository.GetAll().FirstOrDefaultAsync(x => x.InventoryInHeaderId != inventoryInHeaderId && (x.StoreId == storeId && (!separateYears || x.DocumentDate.Year == documentDate.Year) && x.Prefix == prefix && x.InventoryInCode == inventoryInCode && x.Suffix == suffix));
            if (inventoryInHeader is not null)
			{
                return new ResponseDto() { Id = inventoryInHeader.InventoryInHeaderId, Success = true };
            }
			return new ResponseDto() {  Id = 0, Success = false };
		}

        public async Task<int> GetNextId()
        {
            int id = 1;
            try { id = await _repository.GetAll().MaxAsync(a => a.InventoryInHeaderId) + 1; } catch { id = 1; }
            return id;
        }

        public async Task<ResponseDto> DeleteInventoryInHeader(int id)
		{
            var inventoryInHeader = await _repository.GetAll().FirstOrDefaultAsync(x => x.InventoryInHeaderId == id);
            if (inventoryInHeader != null)
            {
                _repository.Delete(inventoryInHeader);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = id, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.InventoryIn, GenericMessageData.DeleteSuccessWithCode, $"{inventoryInHeader.Prefix}{inventoryInHeader.InventoryInCode}{inventoryInHeader.Suffix}") };
            }
            return new ResponseDto() { Id = id, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.InventoryIn, GenericMessageData.NotFound) };
        }

        public async Task<int> GetNextInventoryInCode(int storeId, bool separateYears, DateTime documentDate, string? prefix, string? suffix)
        {
            int id = 1;
            try
            {
                id = await _repository.GetAll().AsNoTracking().Where(x => (!separateYears || x.DocumentDate.Year == documentDate.Year) && x.Prefix == prefix && x.Suffix == suffix && x.StoreId == storeId).MaxAsync(a => a.InventoryInCode) + 1;
            }
            catch { id = 1; }
            return id;
        }
    }
}
