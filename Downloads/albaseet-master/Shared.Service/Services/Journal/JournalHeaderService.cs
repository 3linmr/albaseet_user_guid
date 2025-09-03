using Shared.CoreOne.Contracts.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Domain.Journal;
using Shared.CoreOne.Models.Dtos.ViewModels.Journal;
using Shared.Helper.Identity;
using Shared.Helper.Models.Dtos;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Shared.Service.Validators;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Contracts.Menus;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Logic;
using Shared.Helper.Models.StaticData;
using Shared.CoreOne.Contracts.Settings;

namespace Shared.Service.Services.Journal
{
	public class JournalHeaderService : BaseService<JournalHeader>, IJournalHeaderService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IJournalTypeService _journalTypeService;
		private readonly IStoreService _storeService;
		private readonly IStringLocalizer<JournalHeaderService> _localizer;
		private readonly IMenuEncodingService _menuEncodingService;
		private readonly IBranchService _branchService;
		private readonly ICompanyService _companyService;
		private readonly IApplicationSettingService _applicationSettingService;

		public JournalHeaderService(IRepository<JournalHeader> repository, IHttpContextAccessor httpContextAccessor, IJournalTypeService journalTypeService, IStoreService storeService, IStringLocalizer<JournalHeaderService> localizer, IMenuEncodingService menuEncodingService, IBranchService branchService, ICompanyService companyService, IApplicationSettingService applicationSettingService) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
			_journalTypeService = journalTypeService;
			_storeService = storeService;
			_localizer = localizer;
			_menuEncodingService = menuEncodingService;
			_branchService = branchService;
			_companyService = companyService;
			_applicationSettingService = applicationSettingService;
		}

		public IQueryable<JournalHeaderDto> GetJournalHeaders()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var data =
				from journalHeader in _repository.GetAll()
				from journalType in _journalTypeService.GetAll().Where(x => x.JournalTypeId == journalHeader.JournalTypeId)
				from store in _storeService.GetAll().Where(x => x.StoreId == journalHeader.StoreId)
				from branch in _branchService.GetAll().Where(x => x.BranchId == store.BranchId)
				from company in _companyService.GetAll().Where(x => x.CompanyId == branch.CompanyId)
				select new JournalHeaderDto()
				{
					JournalHeaderId = journalHeader.JournalHeaderId,
					JournalId = journalHeader.JournalId,
					TicketId = journalHeader.TicketId,
					Prefix = journalHeader.Prefix,
					JournalCode = journalHeader.JournalCode,
					Suffix = journalHeader.Suffix,
					JournalCodeFull = $"{journalHeader.Prefix}{journalHeader.JournalCode}{journalHeader.Suffix}",
					JournalTypeId = journalHeader.JournalTypeId,
					StoreId = journalHeader.StoreId,
					StoreCurrencyId = company.CurrencyId,
					TicketDate = journalHeader.TicketDate,
					EntryDate = journalHeader.EntryDate,
					PeerReference = journalHeader.PeerReference,
					RemarksAr = journalHeader.RemarksAr,
					RemarksEn = journalHeader.RemarksEn,
					IsCancelled = journalHeader.IsCancelled,
					IsClosed = journalHeader.IsClosed,
					IsSystematic = journalHeader.IsSystematic,
					ArchiveHeaderId = journalHeader.ArchiveHeaderId,
					CreatedAt = journalHeader.CreatedAt,
					ModifiedAt = journalHeader.ModifiedAt,
					UserNameCreated = journalHeader.UserNameCreated,
					IpAddressCreated = journalHeader.IpAddressCreated,
					UserNameModified = journalHeader.UserNameModified,
					JournalTypeName = language == LanguageCode.Arabic ? journalType.JournalTypeNameAr : journalType.JournalTypeNameEn,
					StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
					MenuCode = journalHeader.MenuCode,
					MenuReferenceId = journalHeader.MenuReferenceId,
					DocumentReference = journalHeader.DocumentReference,
					TotalDebitValue = journalHeader.TotalDebitValue,
					TotalCreditValue = journalHeader.TotalCreditValue,
					TotalDebitValueAccount = journalHeader.TotalDebitValueAccount,
					TotalCreditValueAccount = journalHeader.TotalCreditValueAccount,
                };
			return data;
		}

		public IQueryable<JournalHeaderDto> GetUserJournalHeaders()
		{
			var userStore = _httpContextAccessor.GetCurrentUserStore();
			return GetJournalHeaders().Where(x => x.StoreId == userStore);
		}

		public async Task<JournalHeaderDto> GetJournalHeader(int journalHeaderId)
		{
			return await GetJournalHeaders().FirstOrDefaultAsync(x => x.JournalHeaderId == journalHeaderId) ?? new JournalHeaderDto();
		}

		public async Task<DocumentCodeDto> GetJournalCode(int storeId, DateTime ticketDate)
		{
            var separateYears = await _applicationSettingService.SeparateYears(storeId);
			return await GetJournalCodeInternal(storeId, separateYears, ticketDate);
		}

		public async Task<DocumentCodeDto> GetJournalCodeInternal(int storeId, bool separateYears, DateTime ticketDate)
		{
			var encoding = await _menuEncodingService.GetMenuEncoding(storeId, MenuCodeData.JournalEntry);
			var code = await GetNextJournalCode(storeId, separateYears, ticketDate, encoding.Prefix, encoding.Suffix);
			return new DocumentCodeDto() { NextCode = code, Prefix = encoding.Prefix, Suffix = encoding.Suffix };
		}

		public async Task<ResponseDto> SaveJournalHeader(JournalHeaderDto journalHeader, bool hasApprove, bool approved, int? requestId)
		{
            var separateYears = await _applicationSettingService.SeparateYears(journalHeader.StoreId);

			if (hasApprove || (journalHeader.JournalTypeId != JournalTypeData.JournalEntry && journalHeader.JournalTypeId != JournalTypeData.OpeningBalance))
			{
				if (journalHeader.JournalHeaderId == 0)
				{
					return await CreateJournalHeader(journalHeader, hasApprove, approved, requestId, separateYears);
				}
				else
				{
					return await UpdateJournalHeader(journalHeader);
				}
			}
			else
			{
				var journalHeaderExist = await IsJournalCodeExist(journalHeader.JournalHeaderId, journalHeader.JournalCode, journalHeader.StoreId, separateYears, journalHeader.TicketDate, journalHeader.Prefix, journalHeader.Suffix);
				if (journalHeaderExist.Success)
				{
					var nextJournalCode = await GetNextJournalCode(journalHeader.StoreId, separateYears, journalHeader.TicketDate, journalHeader.Prefix, journalHeader.Suffix);
					return new ResponseDto() { Id = nextJournalCode, ResponseType = ResponseTypeData.Confirm, Success = false, Message = _localizer["JournalHeaderAlreadyExist", nextJournalCode] };
				}
				else
				{
					if (journalHeader.JournalHeaderId == 0)
					{
						return await CreateJournalHeader(journalHeader, hasApprove, approved, requestId, separateYears);
					}
					else
					{
						return await UpdateJournalHeader(journalHeader);
					}
				}
			}

		}


		public async Task<ResponseDto> CreateJournalHeader(JournalHeaderDto journalHeader, bool hasApprove, bool approved, int? requestId, bool separateYears)
		{
			int journalCode;
			var journalCodeData = await GetJournalCodeInternal(journalHeader.StoreId, separateYears, journalHeader.TicketDate);
			if ((hasApprove && approved) || (journalHeader.JournalTypeId != JournalTypeData.JournalEntry && journalHeader.JournalTypeId != JournalTypeData.OpeningBalance))
			{
				journalCode = journalCodeData.NextCode;
			}
			else
			{
				journalCode = journalHeader.JournalCode != 0 ? journalHeader.JournalCode : journalCodeData.NextCode;
			}

			var journalHeaderId = await GetNextId();
			var newJournalHeader = new JournalHeader()
			{
				JournalHeaderId = journalHeaderId,
				JournalId = await GetNextJournalId(journalHeader.StoreId, journalHeader.TicketDate),
				TicketId = await GetNextTicketId(journalHeader.StoreId, journalHeader.TicketDate, journalHeader.JournalTypeId),
				StoreId = journalHeader.StoreId,
				TicketDate = journalHeader.TicketDate,
				DocumentReference = hasApprove ? $"{DocumentReferenceData.Approval}{requestId}" : $"{DocumentReferenceData.JournalEntry}{journalHeaderId}",
				EntryDate = DateHelper.GetDateTimeNow(),
				JournalTypeId = journalHeader.JournalTypeId,
				PeerReference = journalHeader.PeerReference,
				Prefix = journalCodeData.Prefix,
				JournalCode = journalCode,
				Suffix = journalCodeData.Suffix,
				MenuCode = journalHeader.MenuCode,
				MenuReferenceId = journalHeader.MenuReferenceId,
				IsClosed = journalHeader.IsClosed,
				IsCancelled = journalHeader.IsCancelled,
				IsSystematic = journalHeader.IsSystematic,
                TotalDebitValue = journalHeader.TotalDebitValue,
                TotalCreditValue = journalHeader.TotalCreditValue,
                TotalDebitValueAccount = journalHeader.TotalDebitValueAccount,
                TotalCreditValueAccount = journalHeader.TotalCreditValueAccount,
				RemarksAr = journalHeader.RemarksAr,
				RemarksEn = journalHeader.RemarksEn,
				CreatedAt = DateHelper.GetDateTimeNow(),
				UserNameCreated = await _httpContextAccessor!.GetUserName(),
				IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
				Hide = false,
			};

			var journalHeaderValidator = await new JournalHeaderValidator(_localizer).ValidateAsync(newJournalHeader);
			var validationResult = journalHeaderValidator.IsValid;
			if (validationResult)
			{
				await _repository.Insert(newJournalHeader);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = newJournalHeader.JournalHeaderId, Success = true, Message = _localizer["NewJournalHeaderSuccessMessage", $"{journalHeader.Prefix}{newJournalHeader.JournalCode}{journalHeader.Suffix}"] };
			}
			else
			{
				return new ResponseDto() { Id = newJournalHeader.JournalHeaderId, Success = false, Message = journalHeaderValidator.ToString("~") };
			}
		}

		public async Task<ResponseDto> UpdateJournalHeader(JournalHeaderDto journalHeader)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var journalHeaderDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.JournalHeaderId == journalHeader.JournalHeaderId);
			if (journalHeaderDb != null)
			{
				journalHeaderDb.IsClosed = journalHeader.IsClosed;
				journalHeaderDb.IsCancelled = journalHeader.IsCancelled;
				journalHeaderDb.IsSystematic = journalHeader.IsSystematic;
				//journalHeaderDb.Prefix = journalHeader.Prefix;
				//journalHeaderDb.JournalCode = journalHeader.JournalCode;
				//journalHeaderDb.Suffix = journalHeader.Suffix;
				journalHeaderDb.JournalTypeId = journalHeader.JournalTypeId;
				journalHeaderDb.PeerReference = journalHeader.PeerReference;
                journalHeaderDb.TotalDebitValue = journalHeader.TotalDebitValue;
                journalHeaderDb.TotalCreditValue = journalHeader.TotalCreditValue;
                journalHeaderDb.TotalDebitValueAccount = journalHeader.TotalDebitValueAccount;
                journalHeaderDb.TotalCreditValueAccount = journalHeader.TotalCreditValueAccount;
				journalHeaderDb.RemarksAr = journalHeader.RemarksAr;
				journalHeaderDb.RemarksEn = journalHeader.RemarksEn;
				journalHeaderDb.StoreId = journalHeader.StoreId;
				journalHeaderDb.TicketDate = journalHeader.TicketDate;
				journalHeaderDb.ModifiedAt = DateHelper.GetDateTimeNow();
				journalHeaderDb.UserNameModified = await _httpContextAccessor!.GetUserName();
				journalHeaderDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();

				var journalHeaderValidator = await new JournalHeaderValidator(_localizer).ValidateAsync(journalHeaderDb);
				var validationResult = journalHeaderValidator.IsValid;
				if (validationResult)
				{
					_repository.Update(journalHeaderDb);
					await _repository.SaveChanges();
					return new ResponseDto() { Id = journalHeaderDb.JournalHeaderId, Success = true, Message = _localizer["UpdateJournalHeaderSuccessMessage", $"{journalHeaderDb.Prefix}{journalHeaderDb.JournalCode}{journalHeaderDb.Suffix}"] };
				}
				else
				{
					return new ResponseDto() { Id = 0, Success = false, Message = journalHeaderValidator.ToString("~") };
				}
			}
			return new ResponseDto() { Id = 0, Success = false, Message = _localizer["NoJournalHeaderFound"] };
		}

		public async Task<ResponseDto> IsJournalCodeExist(int journalHeaderId, int journalCode, int storeId, bool separateYears, DateTime ticketDate, string? prefix, string? suffix)
		{
			var journalHeader = await _repository.GetAll().FirstOrDefaultAsync(x => (x.StoreId == storeId && x.JournalCode == journalCode && (!separateYears || x.TicketDate.Year == ticketDate.Year) && x.Prefix == prefix && x.Suffix == suffix) && x.JournalHeaderId != journalHeaderId);
			if (journalHeader != null)
			{
				return new ResponseDto() { Id = journalHeader.JournalHeaderId, Success = true };
			}
			return new ResponseDto() { Id = 0, Success = false };
		}

		public async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.JournalHeaderId) + 1; } catch { id = 1; }
			return id;
		}

		public async Task<int> GetNextJournalId(int storeId, DateTime ticketDate)
		{
			int id = 1;
			try
			{
				id = await _repository.GetAll().Where(x => x.TicketDate.Year == ticketDate.Year && x.StoreId == storeId).MaxAsync(a => a.JournalId) + 1;

			}
			catch { id = 1; }
			return id;
		}
		public async Task<int> GetNextTicketId(int storeId, DateTime ticketDate, int journalTypeId)
		{
			int id = 1;
			try
			{
				id = await _repository.GetAll().Where(x => x.TicketDate.Year == ticketDate.Year && x.JournalTypeId == journalTypeId && x.StoreId == storeId).MaxAsync(a => a.TicketId) + 1;

			}
			catch { id = 1; }
			return id;
		}
		public async Task<int> GetNextJournalCode(int storeId, bool separateYears, DateTime ticketDate, string? prefix, string? suffix)
		{
			int id = 1;
			try
			{
				id = await _repository.GetAll().AsNoTracking().Where(x => (!separateYears || x.TicketDate.Year == ticketDate.Year) && x.Prefix == prefix && x.Suffix == suffix && x.StoreId == storeId).MaxAsync(a => a.JournalCode) + 1;
			}
			catch { id = 1; }
			return id;
		}

		public async Task<ResponseDto> DeleteJournalHeader(int id)
		{
			var journalHeaderDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.JournalHeaderId == id);
			if (journalHeaderDb != null)
			{
				_repository.Delete(journalHeaderDb);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = id, Success = true, Message = _localizer["DeleteJournalHeaderMessage", $"{journalHeaderDb.Prefix}{journalHeaderDb.JournalCode}{journalHeaderDb.Suffix}"] };
			}
			return new ResponseDto() { Id = id, Success = false, Message = _localizer["NoJournalHeaderFound"] };
		}
	}
}
