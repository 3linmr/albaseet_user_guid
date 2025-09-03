using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Contracts.Menus;
using Shared.CoreOne.Contracts.Modules;
using Shared.Service;
using Inventory.CoreOne.Models.Domain;
using Inventory.CoreOne.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne;
using Inventory.CoreOne.Contracts;
using Shared.Helper.Models.Dtos;
using Inventory.CoreOne.Models.Dtos.ViewModels;
using Shared.Helper.Identity;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using Inventory.Service.Validators;
using Shared.CoreOne.Models.StaticData;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Shared.CoreOne.Models.Domain.Archive;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.Helper.Logic;
using Shared.CoreOne.Contracts.Settings;

namespace Inventory.Service.Services
{
	public class InternalTransferReceiveHeaderService : BaseService<InternalTransferReceiveHeader>, IInternalTransferReceiveHeaderService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IStoreService _storeService;
		private readonly IStringLocalizer<InternalTransferReceiveHeaderService> _localizer;
        private readonly IGenericMessageService _genericMessageService;
		private readonly IMenuEncodingService _menuEncodingService;
        private readonly IInternalTransferHeaderService _internalTransferHeaderService;
        private readonly IApplicationSettingService _applicationSettingService;

		public InternalTransferReceiveHeaderService(IRepository<InternalTransferReceiveHeader> repository, IHttpContextAccessor httpContextAccessor, IStoreService storeService, IStringLocalizer<InternalTransferReceiveHeaderService> localizer, IGenericMessageService genericMessageService, IMenuEncodingService menuEncodingService, IInternalTransferHeaderService internalTransferHeaderService, IApplicationSettingService applicationSettingService) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
			_storeService = storeService;
			_localizer = localizer;
            _genericMessageService = genericMessageService;
			_menuEncodingService = menuEncodingService;
            _internalTransferHeaderService = internalTransferHeaderService;
            _applicationSettingService = applicationSettingService;
		}

		public IQueryable<InternalTransferReceiveHeaderDto> GetInternalTransferReceiveHeaders()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var data = from internalTransferReceiveHeader in _repository.GetAll()
				from fromStore in _storeService.GetAll().Where(x => x.StoreId == internalTransferReceiveHeader.FromStoreId)
                from toStore in _storeService.GetAll().Where(x => x.StoreId == internalTransferReceiveHeader.ToStoreId)
                from internalTransfer in _internalTransferHeaderService.GetAll().Where(x => x.InternalTransferId == internalTransferReceiveHeader.InternalTransferHeaderId).AsNoTracking().Select(x => new {x.IsReturned, x.ReturnReason})
				select new InternalTransferReceiveHeaderDto()
				{
					InternalTransferReceiveHeaderId = internalTransferReceiveHeader.InternalTransferReceiveHeaderId,
					Prefix = internalTransferReceiveHeader.Prefix,
					InternalTransferReceiveCode = internalTransferReceiveHeader.InternalTransferReceiveCode,
					Suffix = internalTransferReceiveHeader.Suffix,
					InternalTransferReceiveFullCode = $"{internalTransferReceiveHeader.Prefix}{internalTransferReceiveHeader.InternalTransferReceiveCode}{internalTransferReceiveHeader.Suffix}",
					DocumentReference = internalTransferReceiveHeader.DocumentReference,
					InternalTransferHeaderId = internalTransferReceiveHeader.InternalTransferHeaderId,
					DocumentDate = internalTransferReceiveHeader.DocumentDate,
					EntryDate = internalTransferReceiveHeader.EntryDate,
                    FromStoreId = internalTransferReceiveHeader.FromStoreId,
					FromStoreName = language == LanguageCode.Arabic ? fromStore.StoreNameAr : fromStore.StoreNameEn,
					ToStoreId = internalTransferReceiveHeader.ToStoreId,
                    ToStoreName = language == LanguageCode.Arabic ? toStore.StoreNameAr : toStore.StoreNameEn,
                    Reference = internalTransferReceiveHeader.Reference,
					TotalConsumerValue = internalTransferReceiveHeader.TotalConsumerValue,
					TotalCostValue = internalTransferReceiveHeader.TotalCostValue,
					RemarksAr = internalTransferReceiveHeader.RemarksAr,
					RemarksEn = internalTransferReceiveHeader.RemarksEn,
                    IsReturned = internalTransfer.IsReturned,
                    ReturnReason = internalTransfer.ReturnReason,
					ArchiveHeaderId = internalTransferReceiveHeader.ArchiveHeaderId,
					IsClosed = internalTransferReceiveHeader.IsClosed,
					MenuCode = internalTransferReceiveHeader.MenuCode,
					ReferenceId = internalTransferReceiveHeader.ReferenceId,

                    CreatedAt = internalTransferReceiveHeader.CreatedAt,
                    UserNameCreated = internalTransferReceiveHeader.UserNameCreated,
                    IpAddressCreated = internalTransferReceiveHeader.IpAddressCreated
				};
			return data;
        }

        public IQueryable<InternalTransferReceiveHeaderDto> GetUserInternalTransferReceiveHeaders()
        {
            var userStore = _httpContextAccessor.GetCurrentUserStore();

            return GetInternalTransferReceiveHeaders().Where(x => x.ToStoreId == userStore);
        }

        public IQueryable<InternalTransferReceiveHeaderDto> GetInternalTransferReceiveHeadersByStoreId(int storeId)
		{
            return GetInternalTransferReceiveHeaders().Where(x => x.ToStoreId == storeId);
        }

		public async Task<InternalTransferReceiveHeaderDto?> GetInternalTransferReceiveHeaderById(int id)
		{
			return await GetInternalTransferReceiveHeaders().FirstOrDefaultAsync(x => x.InternalTransferReceiveHeaderId == id);
		}

        public async Task<DocumentCodeDto> GetInternalTransferReceiveCode(int storeId, DateTime documentDate)
        {
            var separateYears = await _applicationSettingService.SeparateYears(storeId);
            return await GetInternalTransferReceiveCodeInternal(storeId, separateYears, documentDate);
        }

		public async Task<DocumentCodeDto> GetInternalTransferReceiveCodeInternal(int storeId, bool separateYears, DateTime documentDate)
		{
			var encoding = await _menuEncodingService.GetMenuEncoding(storeId, MenuCodeData.InternalReceiveItems);
			var code = await GetNextInternalTransferReceiveCode(storeId, separateYears, documentDate, encoding.Prefix, encoding.Suffix);
			return new DocumentCodeDto() { NextCode = code, Prefix = encoding.Prefix, Suffix = encoding.Suffix };
		}

		public async Task<ResponseDto> SaveInternalTransferReceiveHeader(InternalTransferReceiveHeaderDto internalTransferReceiveHeader, bool hasApprove, bool approved, int? requestId, string? documentReference, bool shouldInitializeFlags)
		{
            var separateYears = await _applicationSettingService.SeparateYears(internalTransferReceiveHeader.ToStoreId);

            if (hasApprove)
			{
                if (internalTransferReceiveHeader.InternalTransferReceiveHeaderId == 0)
                {
                    return await CreateInternalTransferReceiveHeader(internalTransferReceiveHeader, hasApprove, approved, requestId, documentReference, shouldInitializeFlags, separateYears);
                }
                else
                {
                    return await UpdateInternalTransferReceiveHeader(internalTransferReceiveHeader);
                }
            }
            else
            {
                var internalTransferReceiveHeaderExist = await IsInternalTransferReceiveCodeExist(internalTransferReceiveHeader.InternalTransferReceiveHeaderId, internalTransferReceiveHeader.InternalTransferReceiveCode, internalTransferReceiveHeader.ToStoreId, separateYears, internalTransferReceiveHeader.DocumentDate, internalTransferReceiveHeader.Prefix, internalTransferReceiveHeader.Suffix);
                if (internalTransferReceiveHeaderExist.Success)
                {
                    var nextInternalTransferReceiveCode = await GetNextInternalTransferReceiveCode(internalTransferReceiveHeader.ToStoreId, separateYears, internalTransferReceiveHeader.DocumentDate, internalTransferReceiveHeader.Prefix, internalTransferReceiveHeader.Suffix);
                    return new ResponseDto() { Id = nextInternalTransferReceiveCode, ResponseType = ResponseTypeData.Confirm, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.InternalReceiveItems, GenericMessageData.CodeAlreadyExist, $"{nextInternalTransferReceiveCode}") };
                }
                else
                {
                    if (internalTransferReceiveHeader.InternalTransferReceiveHeaderId == 0)
                    {
                        return await CreateInternalTransferReceiveHeader(internalTransferReceiveHeader, hasApprove, approved, requestId, documentReference, shouldInitializeFlags, separateYears);
                    }
                    else
                    {
                        return await UpdateInternalTransferReceiveHeader(internalTransferReceiveHeader);
                    }
                }
            }
        }

        public async Task<ResponseDto> CreateInternalTransferReceiveHeader(InternalTransferReceiveHeaderDto internalTransferReceiveHeader, bool hasApprove, bool approved, int? requestId, string? documentReference, bool shouldInitializeFlags, bool separateYears)
        {
            int internalTransferReceiveCode;
            string? prefix;
            string? suffix;
            var nextInternalTransferReceiveCode = await GetInternalTransferReceiveCodeInternal(internalTransferReceiveHeader.ToStoreId, separateYears, internalTransferReceiveHeader.DocumentDate);
            if (hasApprove && approved)
            {
                internalTransferReceiveCode = nextInternalTransferReceiveCode.NextCode;
                prefix = nextInternalTransferReceiveCode.Prefix;
                suffix = nextInternalTransferReceiveCode.Suffix;
            }
            else
            {
                internalTransferReceiveCode = internalTransferReceiveHeader.InternalTransferReceiveCode != 0 ? internalTransferReceiveHeader.InternalTransferReceiveCode : nextInternalTransferReceiveCode.NextCode;
                prefix = string.IsNullOrWhiteSpace(internalTransferReceiveHeader.Prefix) ? nextInternalTransferReceiveCode.Prefix : internalTransferReceiveHeader.Prefix;
                suffix = string.IsNullOrWhiteSpace(internalTransferReceiveHeader.Suffix) ? nextInternalTransferReceiveCode.Suffix : internalTransferReceiveHeader.Suffix;
            }

            var internalTransferReceiveHeaderId = await GetNextId();
            var newInternalTransferReceiveHeader = new InternalTransferReceiveHeader()
            {
                InternalTransferReceiveHeaderId = internalTransferReceiveHeaderId,
                Prefix = prefix,
                InternalTransferReceiveCode = internalTransferReceiveCode,
                Suffix = suffix,
                DocumentReference = documentReference != null ? documentReference : (hasApprove ? $"{DocumentReferenceData.Approval}{requestId}" : $"{DocumentReferenceData.InternalTransferReceive}{internalTransferReceiveHeaderId}"),
                InternalTransferHeaderId = internalTransferReceiveHeader.InternalTransferHeaderId,
                DocumentDate = internalTransferReceiveHeader.DocumentDate,
                EntryDate = DateHelper.GetDateTimeNow(),
                FromStoreId = internalTransferReceiveHeader.FromStoreId,
                ToStoreId = internalTransferReceiveHeader.ToStoreId,
                Reference = internalTransferReceiveHeader.Reference,
                TotalConsumerValue = internalTransferReceiveHeader.TotalConsumerValue,
                TotalCostValue = internalTransferReceiveHeader.TotalCostValue,
                RemarksAr = internalTransferReceiveHeader.RemarksAr,
                RemarksEn = internalTransferReceiveHeader.RemarksEn,
                ArchiveHeaderId = internalTransferReceiveHeader.ArchiveHeaderId,
                IsClosed = shouldInitializeFlags ? internalTransferReceiveHeader.IsClosed : false,
                MenuCode = internalTransferReceiveHeader.MenuCode,
                ReferenceId = internalTransferReceiveHeader.ReferenceId,


				CreatedAt = DateHelper.GetDateTimeNow(),
                UserNameCreated = await _httpContextAccessor!.GetUserName(),
                IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
                Hide = false,
            };

            var internalTransferReceiveHeaderValidator = await new InternalTransferReceiveHeaderValidator(_localizer).ValidateAsync(newInternalTransferReceiveHeader);
            var validationResult = internalTransferReceiveHeaderValidator.IsValid;
            if (validationResult)
            {
                await _repository.Insert(newInternalTransferReceiveHeader);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = newInternalTransferReceiveHeader.InternalTransferReceiveHeaderId, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.InternalReceiveItems, GenericMessageData.CreatedSuccessWithCode, $"{newInternalTransferReceiveHeader.Prefix}{newInternalTransferReceiveHeader.InternalTransferReceiveCode}{newInternalTransferReceiveHeader.Suffix}") };
            }
            else
            {
                return new ResponseDto() { Id = newInternalTransferReceiveHeader.InternalTransferReceiveHeaderId, Success = false, Message = internalTransferReceiveHeaderValidator.ToString("~") };
            }
        }

        public async Task<ResponseDto> UpdateInternalTransferReceiveHeader(InternalTransferReceiveHeaderDto internalTransferReceiveHeader)
        {
            var internalTransferReceiveHeaderDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.InternalTransferReceiveHeaderId == internalTransferReceiveHeader.InternalTransferReceiveHeaderId);
            if (internalTransferReceiveHeaderDb != null)
            {
                internalTransferReceiveHeaderDb.InternalTransferHeaderId = internalTransferReceiveHeader.InternalTransferHeaderId;
                internalTransferReceiveHeaderDb.DocumentDate = internalTransferReceiveHeader.DocumentDate;
                internalTransferReceiveHeaderDb.FromStoreId = internalTransferReceiveHeader.FromStoreId;
                internalTransferReceiveHeaderDb.ToStoreId = internalTransferReceiveHeader.ToStoreId;
                internalTransferReceiveHeaderDb.Reference = internalTransferReceiveHeader.Reference;
                internalTransferReceiveHeaderDb.TotalConsumerValue = internalTransferReceiveHeader.TotalConsumerValue;
                internalTransferReceiveHeaderDb.TotalCostValue = internalTransferReceiveHeader.TotalCostValue;
                internalTransferReceiveHeaderDb.RemarksAr = internalTransferReceiveHeader.RemarksAr;
                internalTransferReceiveHeaderDb.RemarksEn = internalTransferReceiveHeader.RemarksEn;
                internalTransferReceiveHeaderDb.ArchiveHeaderId = internalTransferReceiveHeader.ArchiveHeaderId;
                //internalTransferReceiveHeaderDb.MenuCode = internalTransferReceiveHeader.MenuCode;
                //internalTransferReceiveHeaderDb.ReferenceId = internalTransferReceiveHeader.ReferenceId;


				internalTransferReceiveHeaderDb.ModifiedAt = DateHelper.GetDateTimeNow();
                internalTransferReceiveHeaderDb.UserNameModified = await _httpContextAccessor!.GetUserName();
                internalTransferReceiveHeaderDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();


                var internalTransferReceiveHeaderValidator = await new InternalTransferReceiveHeaderValidator(_localizer).ValidateAsync(internalTransferReceiveHeaderDb);
                var validationResult = internalTransferReceiveHeaderValidator.IsValid;
                if (validationResult)
                {
                    _repository.Update(internalTransferReceiveHeaderDb);
                    await _repository.SaveChanges();
                    return new ResponseDto() { Id = internalTransferReceiveHeaderDb.InternalTransferReceiveHeaderId, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.InternalReceiveItems, GenericMessageData.UpdatedSuccessWithCode, $"{internalTransferReceiveHeaderDb.Prefix}{internalTransferReceiveHeaderDb.InternalTransferReceiveCode}{internalTransferReceiveHeaderDb.Suffix}") };
                }
                else
                {
                    return new ResponseDto() { Id = 0, Success = false, Message = internalTransferReceiveHeaderValidator.ToString("~") };
                }
            }
            return new ResponseDto() { Id = 0, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.InternalReceiveItems, GenericMessageData.NotFound) };
        }

        public async Task<ResponseDto> IsInternalTransferReceiveCodeExist(int internalTransferReceiveHeaderId, int internalTransferReceiveCode, int storeId, bool separateYears, DateTime documentDate, string? prefix, string? suffix)
		{
			var internalTransferReceiveHeader = await _repository.GetAll().FirstOrDefaultAsync(x => x.InternalTransferReceiveHeaderId != internalTransferReceiveHeaderId && (x.ToStoreId == storeId && (!separateYears || x.DocumentDate.Year == documentDate.Year) && x.Prefix == prefix && x.InternalTransferReceiveCode == internalTransferReceiveCode && x.Suffix == suffix));
			if (internalTransferReceiveHeader is not null)
			{
                return new ResponseDto() { Id = internalTransferReceiveHeader.InternalTransferReceiveHeaderId, Success = true };
            }
			return new ResponseDto() {  Id = 0, Success = false };
		}

        public async Task<int> GetNextId()
        {
            int id = 1;
            try { id = await _repository.GetAll().MaxAsync(a => a.InternalTransferReceiveHeaderId) + 1; } catch { id = 1; }
            return id;
        }

        public async Task<ResponseDto> DeleteInternalTransferReceiveHeader(int id)
		{
            var internalTransferReceiveHeader = await _repository.GetAll().FirstOrDefaultAsync(x => x.InternalTransferReceiveHeaderId == id);
            if (internalTransferReceiveHeader != null)
            {
                _repository.Delete(internalTransferReceiveHeader);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = id, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.InternalReceiveItems, GenericMessageData.DeleteSuccessWithCode, $"{internalTransferReceiveHeader.Prefix}{internalTransferReceiveHeader.InternalTransferReceiveCode}{internalTransferReceiveHeader.Suffix}") };
            }
            return new ResponseDto() { Id = id, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.InternalReceiveItems, GenericMessageData.NotFound) };
        }

        public async Task<int> GetNextInternalTransferReceiveCode(int storeId, bool separateYears, DateTime documentDate, string? prefix, string? suffix)
        {
            int id = 1;
            try
            {
                id = await _repository.GetAll().AsNoTracking().Where(x => (!separateYears || x.DocumentDate.Year == documentDate.Year) && x.Prefix == prefix && x.Suffix == suffix && x.ToStoreId == storeId).MaxAsync(a => a.InternalTransferReceiveCode) + 1;
            }
            catch { id = 1; }
            return id;
        }
    }
}
