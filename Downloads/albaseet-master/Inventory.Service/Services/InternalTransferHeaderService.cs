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
using Shared.Helper.Logic;
using Shared.CoreOne.Contracts.Settings;

namespace Inventory.Service.Services
{
	public class InternalTransferHeaderService : BaseService<InternalTransferHeader>, IInternalTransferHeaderService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IStoreService _storeService;
		private readonly IGenericMessageService _genericMessageService;
        private readonly IStringLocalizer<InternalTransferHeaderService> _localizer;
		private readonly IMenuEncodingService _menuEncodingService;
        private readonly IApplicationSettingService _applicationSettingService;

		public InternalTransferHeaderService(IRepository<InternalTransferHeader> repository, IHttpContextAccessor httpContextAccessor, IStoreService storeService, IStringLocalizer<InternalTransferHeaderService> localizer, IGenericMessageService genericMessageService, IMenuEncodingService menuEncodingService, IApplicationSettingService applicationSettingService) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
			_storeService = storeService;
            _localizer = localizer;
			_genericMessageService = genericMessageService;
			_menuEncodingService = menuEncodingService;
            _applicationSettingService = applicationSettingService;
		}

		public IQueryable<InternalTransferHeaderDto> GetInternalTransferHeaders()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var data = from internalTransferHeader in _repository.GetAll()
				from fromStore in _storeService.GetAll().Where(x => x.StoreId == internalTransferHeader.FromStoreId)
                from toStore in _storeService.GetAll().Where(x => x.StoreId == internalTransferHeader.ToStoreId)
				select new InternalTransferHeaderDto()
				{
					InternalTransferHeaderId = internalTransferHeader.InternalTransferId,
					Prefix = internalTransferHeader.Prefix,
					InternalTransferCode = internalTransferHeader.InternalTransferCode,
					Suffix = internalTransferHeader.Suffix,
					InternalTransferFullCode = $"{internalTransferHeader.Prefix}{internalTransferHeader.InternalTransferCode}{internalTransferHeader.Suffix}",
					DocumentReference = internalTransferHeader.DocumentReference,
					DocumentDate = internalTransferHeader.DocumentDate,
					EntryDate = internalTransferHeader.EntryDate,
                    FromStoreId = internalTransferHeader.FromStoreId,
					FromStoreName = language == LanguageCode.Arabic ? fromStore.StoreNameAr : fromStore.StoreNameEn,
					ToStoreId = internalTransferHeader.ToStoreId,
                    ToStoreName = language == LanguageCode.Arabic ? toStore.StoreNameAr : toStore.StoreNameEn,
                    Reference = internalTransferHeader.Reference,
					TotalConsumerValue = internalTransferHeader.TotalConsumerValue,
					TotalCostValue = internalTransferHeader.TotalCostValue,
					RemarksAr = internalTransferHeader.RemarksAr,
					RemarksEn = internalTransferHeader.RemarksEn,
                    IsReturned = internalTransferHeader.IsReturned,
                    ReturnReason = internalTransferHeader.ReturnReason,
					IsClosed = internalTransferHeader.IsClosed,
					ArchiveHeaderId = internalTransferHeader.ArchiveHeaderId,
					MenuCode = internalTransferHeader.MenuCode,
					ReferenceId = internalTransferHeader.ReferenceId,

                    CreatedAt = internalTransferHeader.CreatedAt,
                    UserNameCreated = internalTransferHeader.UserNameCreated,
                    IpAddressCreated = internalTransferHeader.IpAddressCreated,

                    ModifiedAt = internalTransferHeader.ModifiedAt,
                    UserNameModified = internalTransferHeader.UserNameModified,
                    IpAddressModified = internalTransferHeader.IpAddressModified,
                };
			return data;
		}

        public IQueryable<InternalTransferHeaderDto> GetUserInternalTransferHeaders()
        {
            var userStore = _httpContextAccessor.GetCurrentUserStore();

            return GetInternalTransferHeaders().Where(x => x.FromStoreId == userStore);
		}

		private IQueryable<InternalTransferHeaderDto> GetUserInternalTransferHeadersForReceive()
		{
			var userStore = _httpContextAccessor.GetCurrentUserStore();

			return GetInternalTransferHeaders().Where(x => x.ToStoreId == userStore);
		}

		public IQueryable<InternalTransferHeaderDto> GetPendingInternalTransfers()
		{
			return GetUserInternalTransferHeadersForReceive().Where(x => !x.IsClosed);
		}

		public IQueryable<InternalTransferHeaderDto> GetClosedInternalTransfers()
		{
			return GetUserInternalTransferHeadersForReceive().Where(x => x.IsClosed);
		}

		public IQueryable<InternalTransferHeaderDto> GetInternalTransferHeadersByStoreId(int storeId)
		{
            return GetInternalTransferHeaders().Where(x => x.FromStoreId == storeId);
        }

		public async Task<InternalTransferHeaderDto?> GetInternalTransferHeaderById(int id)
		{
			return await GetInternalTransferHeaders().FirstOrDefaultAsync(x => x.InternalTransferHeaderId == id);
		}

        public async Task<DocumentCodeDto> GetInternalTransferCode(int storeId, DateTime documentDate)
        {
            var separateYears = await _applicationSettingService.SeparateYears(storeId);
            return await GetInternalTransferCodeInternal(storeId, separateYears, documentDate);
        }

		public async Task<DocumentCodeDto> GetInternalTransferCodeInternal(int storeId, bool separateYears, DateTime documentDate)
		{
			var encoding = await _menuEncodingService.GetMenuEncoding(storeId, MenuCodeData.InternalTransferItems);
			var code = await GetNextInternalTransferCode(storeId, separateYears, documentDate, encoding.Prefix, encoding.Suffix);
			return new DocumentCodeDto() { NextCode = code, Prefix = encoding.Prefix, Suffix = encoding.Suffix };
		}

		public async Task<ResponseDto> SaveInternalTransferHeader(InternalTransferHeaderDto internalTransferHeader, bool hasApprove, bool approved, int? requestId, string? documentReference, bool shouldValidate, bool shouldInitializeFlags)
		{
            var separateYears = await _applicationSettingService.SeparateYears(internalTransferHeader.FromStoreId);

            if (hasApprove)
            {
                if (internalTransferHeader.InternalTransferHeaderId == 0)
                {
                    return await CreateInternalTransferHeader(internalTransferHeader, hasApprove, approved, requestId, documentReference, shouldInitializeFlags, separateYears);
                }
                else
                {
                    return await UpdateInternalTransferHeader(internalTransferHeader, shouldValidate);
                }
            }
            else
            {
                var internalTransferHeaderExist = await IsInternalTransferCodeExist(internalTransferHeader.InternalTransferHeaderId, internalTransferHeader.InternalTransferCode, internalTransferHeader.FromStoreId, separateYears, internalTransferHeader.DocumentDate, internalTransferHeader.Prefix, internalTransferHeader.Suffix);
                if (internalTransferHeaderExist.Success)
                {
                    var nextInternalTransferCode = await GetNextInternalTransferCode(internalTransferHeader.FromStoreId, separateYears, internalTransferHeader.DocumentDate, internalTransferHeader.Prefix, internalTransferHeader.Suffix);
                    return new ResponseDto() { Id = nextInternalTransferCode, ResponseType = ResponseTypeData.Confirm, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.InternalTransferItems, GenericMessageData.CodeAlreadyExist, $"{nextInternalTransferCode}")};
                }
                else
                {
                    if (internalTransferHeader.InternalTransferHeaderId == 0)
                    {
                        return await CreateInternalTransferHeader(internalTransferHeader, hasApprove, approved, requestId, documentReference, shouldInitializeFlags, separateYears);
                    }
                    else
                    {
                        return await UpdateInternalTransferHeader(internalTransferHeader, shouldValidate);
                    }
                }
            }
        }

        public async Task<bool> UpdateReturned(int internalTransferId, bool isReturned, string? returnedReason)
        {
            var header = await _repository.FindBy(x => x.InternalTransferId == internalTransferId).FirstOrDefaultAsync();
            if (header is null) return false;
            
            header.IsReturned = isReturned;
            header.ReturnReason = isReturned? returnedReason : null;
            header.IsClosed = true;
            _repository.Update(header);
            await _repository.SaveChanges();
            return true;
        }

        public async Task<bool> UpdateClosed(int internalTransferId)
        {
            var header = await _repository.FindBy(x => x.InternalTransferId == internalTransferId).FirstOrDefaultAsync();
            if (header is null) return false;

            header.IsClosed = true;
            _repository.Update(header);
            await _repository.SaveChanges();
            return true;
        }

        public async Task<ResponseDto> CreateInternalTransferHeader(InternalTransferHeaderDto internalTransferHeader, bool hasApprove, bool approved, int? requestId, string? documentReference, bool shouldInitializeFlags, bool separateYears)
        {
            int internalTransferCode;
            string? prefix;
            string? suffix;
            var nextInternalTransferCode = await GetInternalTransferCodeInternal(internalTransferHeader.FromStoreId, separateYears, internalTransferHeader.DocumentDate);
            if (hasApprove && approved)
            {
                internalTransferCode = nextInternalTransferCode.NextCode;
                prefix = nextInternalTransferCode.Prefix;
                suffix = nextInternalTransferCode.Suffix;
            }
            else
            {
                internalTransferCode = internalTransferHeader.InternalTransferCode != 0 ? internalTransferHeader.InternalTransferCode : nextInternalTransferCode.NextCode;
                prefix = string.IsNullOrWhiteSpace(internalTransferHeader.Prefix) ? nextInternalTransferCode.Prefix : internalTransferHeader.Prefix;
                suffix = string.IsNullOrWhiteSpace(internalTransferHeader.Suffix) ? nextInternalTransferCode.Suffix : internalTransferHeader.Suffix;
            }

            var internalTransferHeaderId = await GetNextId();
            var newInternalTransferHeader = new InternalTransferHeader()
            {
                InternalTransferId = internalTransferHeaderId,
                Prefix = prefix,
                InternalTransferCode = internalTransferCode,
                Suffix = suffix,
                DocumentReference = documentReference != null ? documentReference : (hasApprove ? $"{DocumentReferenceData.Approval}{requestId}" : $"{DocumentReferenceData.InternalTransfer}{internalTransferHeaderId}"),
                DocumentDate = internalTransferHeader.DocumentDate,
                EntryDate = DateHelper.GetDateTimeNow(),
                FromStoreId = internalTransferHeader.FromStoreId,
                ToStoreId = internalTransferHeader.ToStoreId,
                Reference = internalTransferHeader.Reference,
                TotalConsumerValue = internalTransferHeader.TotalConsumerValue,
                TotalCostValue = internalTransferHeader.TotalCostValue,
                RemarksAr = internalTransferHeader.RemarksAr,
                RemarksEn = internalTransferHeader.RemarksEn,
                IsReturned = internalTransferHeader.IsReturned,
                ReturnReason = internalTransferHeader.ReturnReason,
                IsClosed = shouldInitializeFlags ? internalTransferHeader.IsClosed : false,
                MenuCode = internalTransferHeader.MenuCode,
                ReferenceId = internalTransferHeader.ReferenceId,
				ArchiveHeaderId = internalTransferHeader.ArchiveHeaderId,
                
                CreatedAt = DateHelper.GetDateTimeNow(),
                UserNameCreated = await _httpContextAccessor!.GetUserName(),
                IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
                Hide = false,
            };

            var internalTransferHeaderValidator = await new InternalTransferHeaderValidator(_localizer).ValidateAsync(newInternalTransferHeader);
            var validationResult = internalTransferHeaderValidator.IsValid;
            if (validationResult)
            {
                await _repository.Insert(newInternalTransferHeader);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = newInternalTransferHeader.InternalTransferId, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.InternalTransferItems, GenericMessageData.CreatedSuccessWithCode, $"{newInternalTransferHeader.Prefix}{newInternalTransferHeader.InternalTransferCode}{newInternalTransferHeader.Suffix}") };
            }
            else
            {
                return new ResponseDto() { Id = newInternalTransferHeader.InternalTransferId, Success = false, Message = internalTransferHeaderValidator.ToString("~") };
            }
        }

        public async Task<ResponseDto> UpdateInternalTransferHeader(InternalTransferHeaderDto internalTransferHeader, bool shouldValidate)
        {
            var internalTransferHeaderDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.InternalTransferId == internalTransferHeader.InternalTransferHeaderId);
            if (internalTransferHeaderDb != null)
            {
                if (shouldValidate && internalTransferHeaderDb.IsClosed)
                {
                    return new ResponseDto() { Id = internalTransferHeader.InternalTransferHeaderId, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.InternalTransferItems, GenericMessageData.CannotUpdateBecauseClosed) };
                }

                internalTransferHeaderDb.DocumentDate = internalTransferHeader.DocumentDate;
                internalTransferHeaderDb.FromStoreId = internalTransferHeader.FromStoreId;
                internalTransferHeaderDb.ToStoreId = internalTransferHeader.ToStoreId;
                internalTransferHeaderDb.Reference = internalTransferHeader.Reference;
                internalTransferHeaderDb.TotalConsumerValue = internalTransferHeader.TotalConsumerValue;
                internalTransferHeaderDb.TotalCostValue = internalTransferHeader.TotalCostValue;
                internalTransferHeaderDb.RemarksAr = internalTransferHeader.RemarksAr;
                internalTransferHeaderDb.RemarksEn = internalTransferHeader.RemarksEn;
                internalTransferHeaderDb.IsReturned = internalTransferHeader.IsReturned;
                internalTransferHeaderDb.ReturnReason = internalTransferHeader.ReturnReason;
                internalTransferHeaderDb.MenuCode = internalTransferHeader.MenuCode;
                internalTransferHeaderDb.ReferenceId = internalTransferHeader.ReferenceId;
                internalTransferHeaderDb.ArchiveHeaderId = internalTransferHeader.ArchiveHeaderId;
                
                internalTransferHeaderDb.ModifiedAt = DateHelper.GetDateTimeNow();
                internalTransferHeaderDb.UserNameModified = await _httpContextAccessor!.GetUserName();
                internalTransferHeaderDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();


                var internalTransferHeaderValidator = await new InternalTransferHeaderValidator(_localizer).ValidateAsync(internalTransferHeaderDb);
                var validationResult = internalTransferHeaderValidator.IsValid;
                if (validationResult)
                {
                    _repository.Update(internalTransferHeaderDb);
                    await _repository.SaveChanges();
                    return new ResponseDto() { Id = internalTransferHeaderDb.InternalTransferId, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.InternalTransferItems, GenericMessageData.UpdatedSuccessWithCode, $"{internalTransferHeaderDb.Prefix}{internalTransferHeaderDb.InternalTransferCode}{internalTransferHeaderDb.Suffix}") };
                }
                else
                {
                    return new ResponseDto() { Id = 0, Success = false, Message = internalTransferHeaderValidator.ToString("~") };
                }
            }
            return new ResponseDto() { Id = 0, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.InternalTransferItems, GenericMessageData.NotFound) };
        }

        public async Task<ResponseDto> IsInternalTransferCodeExist(int internalTransferHeaderId, int internalTransferCode, int storeId, bool separateYears, DateTime documentDate, string? prefix, string? suffix)
		{
			var internalTransferHeader = await _repository.GetAll().FirstOrDefaultAsync(x => x.InternalTransferId != internalTransferHeaderId && (x.FromStoreId == storeId && (!separateYears || x.DocumentDate.Year == documentDate.Year) && x.Prefix == prefix && x.InternalTransferCode == internalTransferCode && x.Suffix == suffix));
			if (internalTransferHeader is not null)
			{
                return new ResponseDto() { Id = internalTransferHeader.InternalTransferId, Success = true };
            }
			return new ResponseDto() {  Id = 0, Success = false };
		}

        public async Task<int> GetNextId()
        {
            int id = 1;
            try { id = await _repository.GetAll().MaxAsync(a => a.InternalTransferId) + 1; } catch { id = 1; }
            return id;
        }

        public async Task<ResponseDto> DeleteInternalTransferHeader(int id)
		{
            var internalTransferHeader = await _repository.GetAll().FirstOrDefaultAsync(x => x.InternalTransferId == id);
            if (internalTransferHeader != null)
            {
                _repository.Delete(internalTransferHeader);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = id, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.InternalTransferItems, GenericMessageData.DeleteSuccessWithCode, $"{internalTransferHeader.Prefix}{internalTransferHeader.InternalTransferCode}{internalTransferHeader.Suffix}") };
            }
            return new ResponseDto() { Id = id, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.InternalTransferItems, GenericMessageData.NotFound) };
        }

        public async Task<int> GetNextInternalTransferCode(int storeId, bool separateYears, DateTime documentDate, string? prefix, string? suffix)
        {
            int id = 1;
            try
            {
                id = await _repository.GetAll().AsNoTracking().Where(x => (!separateYears || x.DocumentDate.Year == documentDate.Year) && x.Prefix == prefix && x.Suffix == suffix && x.FromStoreId == storeId).MaxAsync(a => a.InternalTransferCode) + 1;
            }
            catch { id = 1; }
            return id;
        }
    }
}
