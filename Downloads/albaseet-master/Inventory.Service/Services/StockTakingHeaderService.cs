using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventory.CoreOne.Contracts;
using Inventory.CoreOne.Models.Domain;
using Inventory.CoreOne.Models.Dtos.ViewModels;
using Inventory.Service.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Menus;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Domain.Journal;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Journal;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Helper.Models.Dtos;
using Shared.Service;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Shared.CoreOne.Contracts.Settings;
using Shared.Helper.Models.UserDetail;

namespace Inventory.Service.Services
{
	public class StockTakingHeaderService : BaseService<StockTakingHeader>, IStockTakingHeaderService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IStoreService _storeService;
		private readonly IStringLocalizer<StockTakingHeaderService> _localizer;
		private readonly IMenuEncodingService _menuEncodingService;
		private readonly IApplicationSettingService _applicationSettingService;

		public StockTakingHeaderService(IRepository<StockTakingHeader> repository,IHttpContextAccessor httpContextAccessor,IStoreService storeService,IStringLocalizer<StockTakingHeaderService> localizer,IMenuEncodingService menuEncodingService, IApplicationSettingService applicationSettingService) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
			_storeService = storeService;
			_localizer = localizer;
			_menuEncodingService = menuEncodingService;
			_applicationSettingService = applicationSettingService;
		}

		public IQueryable<StockTakingHeaderDto> GetStockTakingHeaders()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var data =
				from stockTakingHeader in _repository.GetAll()
				from store in _storeService.GetAll().Where(x=>x.StoreId == stockTakingHeader.StoreId)
				select new StockTakingHeaderDto()
				{
					StockTakingHeaderId = stockTakingHeader.StockTakingHeaderId,
					DocumentReference = stockTakingHeader.DocumentReference,
					StockDate = stockTakingHeader.StockDate,
					StoreId = stockTakingHeader.StoreId,
					Prefix = stockTakingHeader.Prefix,
					Suffix = stockTakingHeader.Suffix,
					StockTakingCode = stockTakingHeader.StockTakingCode,
					RemarksAr = stockTakingHeader.RemarksAr,
					RemarksEn = stockTakingHeader.RemarksEn,
					Reference = stockTakingHeader.Reference,
					StockTakingNameAr = stockTakingHeader.StockTakingNameAr,
					StockTakingNameEn = stockTakingHeader.StockTakingNameEn,
					IsOpenBalance = stockTakingHeader.IsOpenBalance,
					IsDeleted = stockTakingHeader.IsDeleted,
					EntryDate = stockTakingHeader.EntryDate,
					IsClosed = stockTakingHeader.IsClosed,
					IsCarriedOver = stockTakingHeader.IsCarriedOver,
					StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
					StockTakingFullCode = $"{stockTakingHeader.Prefix}{stockTakingHeader.StockTakingCode}{stockTakingHeader.Suffix}",
					TotalConsumerValue = stockTakingHeader.TotalConsumerValue,
					TotalCostValue = stockTakingHeader.TotalCostValue
				};
			return data;
		}

        public IQueryable<StockTakingHeaderDto> GetUserStockTakingHeaders(bool isOpenBalance)
        {
            var userStore = _httpContextAccessor.GetCurrentUserStore();

            return GetStockTakingHeaders().Where(x => x.StoreId == userStore && x.IsOpenBalance == isOpenBalance);
        }

        public IQueryable<StockTakingHeaderDto> GetStockTakingHeaders(bool isOpenBalance)
		{
			return GetStockTakingHeaders().Where(x => x.IsOpenBalance == isOpenBalance);
		}

		public async Task<List<StockTakingDropDownDto>> GetPendingStockTakings(int storeId, bool isOpenBalance)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			return await _repository.GetAll().Where(x=>x.StoreId == storeId && !x.IsCarriedOver && x.IsOpenBalance == isOpenBalance).Select(x=> new StockTakingDropDownDto()
			{
				StockTakingHeaderId = x.StockTakingHeaderId,
				StockTakingName = language == LanguageCode.Arabic ? x.StockTakingNameAr : x.StockTakingNameEn,
			}).ToListAsync();
		}

		public IQueryable<StockTakingHeaderDto> GetStockTakingHeadersByStoreId(int storeId)
		{
			return GetStockTakingHeaders().Where(x => x.StoreId == storeId);
		}

		public async Task<StockTakingHeaderDto?> GetStockTakingHeaderById(int id)
		{
			return await GetStockTakingHeaders().FirstOrDefaultAsync(x => x.StockTakingHeaderId == id);
		}

		public async Task<DocumentCodeDto> GetStockTakingCode(int storeId, DateTime stockDate, bool isOpenBalance)
		{
			var separateYears = await _applicationSettingService.SeparateYears(storeId);
			return await GetStockTakingCodeInternal(storeId, separateYears, stockDate, isOpenBalance);
		}

		public async Task<DocumentCodeDto> GetStockTakingCodeInternal(int storeId, bool separateYears, DateTime stockDate, bool isOpenBalance)
		{
			var encoding = await _menuEncodingService.GetMenuEncoding(storeId, isOpenBalance ? MenuCodeData.StockTakingOpenBalance : MenuCodeData.StockTakingCurrentBalance);
			var code = await GetNextStockTakingCode(storeId, separateYears, stockDate, isOpenBalance, encoding.Prefix, encoding.Suffix);
			return new DocumentCodeDto() { NextCode = code, Prefix = encoding.Prefix, Suffix = encoding.Suffix };
		}

		public async Task<ResponseDto> SaveStockTakingHeader(StockTakingHeaderDto stockTakingHeader, bool hasApprove, bool approved, int? requestId)
		{
			var separateYears = await _applicationSettingService.SeparateYears(stockTakingHeader.StoreId);

			if (hasApprove)
			{
				if (stockTakingHeader.StockTakingHeaderId == 0)
				{
					return await CreateStockTakingHeader(stockTakingHeader, hasApprove, approved, requestId, separateYears);
				}
				else
				{
					return await UpdateStockTakingHeader(stockTakingHeader);
				}
			}
			else
			{
				var stockTakingHeaderExist = await IsStockTakingCodeExist(stockTakingHeader.StockTakingHeaderId, separateYears, stockTakingHeader.StockDate ,stockTakingHeader.StockTakingCode,stockTakingHeader.IsOpenBalance, stockTakingHeader.StoreId, stockTakingHeader.Prefix, stockTakingHeader.Suffix);
				if (stockTakingHeaderExist.Success)
				{
					var nextJournalCode = await GetNextStockTakingCode(stockTakingHeader.StoreId, separateYears, stockTakingHeader.StockDate,stockTakingHeader.IsOpenBalance, stockTakingHeader.Prefix, stockTakingHeader.Suffix);
					return new ResponseDto() { Id = nextJournalCode, ResponseType = ResponseTypeData.Confirm, Success = false, Message = _localizer["StockTakingAlreadyExist", nextJournalCode] };
				}
				else
				{
					if (stockTakingHeader.StockTakingHeaderId == 0)
					{
						return await CreateStockTakingHeader(stockTakingHeader, hasApprove, approved, requestId, separateYears);
					}
					else
					{
						return await UpdateStockTakingHeader(stockTakingHeader);
					}
				}
			}
		}

		public async Task<ResponseDto> CreateStockTakingHeader(StockTakingHeaderDto stockTakingHeader, bool hasApprove, bool approved, int? requestId, bool separateYears)
		{
			int stockTakingCode;
			var nextStockTakingCode = await GetStockTakingCodeInternal(stockTakingHeader.StoreId, separateYears, stockTakingHeader.StockDate, stockTakingHeader.IsOpenBalance);
			if (hasApprove && approved)
			{
				stockTakingCode = nextStockTakingCode.NextCode;
			}
			else
			{
				stockTakingCode = stockTakingHeader.StockTakingCode != 0 ? stockTakingHeader.StockTakingCode : nextStockTakingCode.NextCode;
			}

			var stockTakingHeaderId = await GetNextId();
			var newStockTakingHeader = new StockTakingHeader()
			{
				StockTakingHeaderId = stockTakingHeaderId,
				StoreId = stockTakingHeader.StoreId,
				StockDate = stockTakingHeader.StockDate,
				DocumentReference = hasApprove ? $"{DocumentReferenceData.Approval}{requestId}" : $"{(stockTakingHeader.IsOpenBalance ? DocumentReferenceData.StockTakingOpenBalance : DocumentReferenceData.StockTaking)}{stockTakingHeaderId}",
				Reference = stockTakingHeader.Reference,
				StockTakingNameAr = stockTakingHeader.StockTakingNameAr,
				StockTakingNameEn = stockTakingHeader.StockTakingNameEn,
				EntryDate = DateHelper.GetDateTimeNow(),
				Prefix = nextStockTakingCode.Prefix,
				StockTakingCode = stockTakingCode,
				Suffix = nextStockTakingCode.Suffix,
				TotalConsumerValue = stockTakingHeader.TotalConsumerValue,
				TotalCostValue = stockTakingHeader.TotalCostValue,
				IsClosed = stockTakingHeader.IsClosed,
				RemarksAr = stockTakingHeader.RemarksAr,
				IsCarriedOver = stockTakingHeader.IsCarriedOver,
				IsOpenBalance = stockTakingHeader.IsOpenBalance,
				IsDeleted = stockTakingHeader.IsDeleted,
				RemarksEn = stockTakingHeader.RemarksEn,
				CreatedAt = DateHelper.GetDateTimeNow(),
				UserNameCreated = await _httpContextAccessor!.GetUserName(),
				IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
				Hide = false,
			};

			var stockTakingHeaderValidator = await new StockTakingHeaderValidator(_localizer).ValidateAsync(newStockTakingHeader);
			var validationResult = stockTakingHeaderValidator.IsValid;
			if (validationResult)
			{
				await _repository.Insert(newStockTakingHeader);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = newStockTakingHeader.StockTakingHeaderId, Success = true, Message = _localizer["NewStockTakingSuccessMessage", $"{stockTakingHeader.Prefix}{newStockTakingHeader.StockTakingCode}{stockTakingHeader.Suffix}"] };
			}
			else
			{
				return new ResponseDto() { Id = newStockTakingHeader.StockTakingHeaderId, Success = false, Message = stockTakingHeaderValidator.ToString("~") };
			}
		}

		public async Task<ResponseDto> UpdateStockTakingHeader(StockTakingHeaderDto stockTakingHeader)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var stockTakingHeaderDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.StockTakingHeaderId == stockTakingHeader.StockTakingHeaderId);
			if (stockTakingHeaderDb != null)
			{
				stockTakingHeaderDb.StoreId = stockTakingHeader.StoreId;
				stockTakingHeaderDb.StockDate = stockTakingHeader.StockDate;
				stockTakingHeaderDb.Reference = stockTakingHeader.Reference;
				stockTakingHeaderDb.StockTakingNameAr = stockTakingHeader.StockTakingNameAr;
				stockTakingHeaderDb.StockTakingNameEn = stockTakingHeader.StockTakingNameEn;
				stockTakingHeaderDb.TotalConsumerValue = stockTakingHeader.TotalConsumerValue;
				stockTakingHeaderDb.TotalCostValue = stockTakingHeader.TotalCostValue;
				stockTakingHeaderDb.IsClosed = stockTakingHeader.IsClosed;
				stockTakingHeaderDb.RemarksEn = stockTakingHeader.RemarksEn;
				stockTakingHeaderDb.RemarksAr = stockTakingHeader.RemarksAr;
				stockTakingHeaderDb.ModifiedAt = DateHelper.GetDateTimeNow();
				stockTakingHeaderDb.UserNameModified = await _httpContextAccessor!.GetUserName();
				stockTakingHeaderDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();
				//stockTakingHeaderDb.IsCarriedOver = stockTakingHeader.IsCarriedOver;
				//stockTakingHeaderDb.IsOpenBalance = stockTakingHeader.IsOpenBalance;
				//stockTakingHeaderDb.IsDeleted = stockTakingHeader.IsDeleted;

				var stockTakingHeaderValidator = await new StockTakingHeaderValidator(_localizer).ValidateAsync(stockTakingHeaderDb);
				var validationResult = stockTakingHeaderValidator.IsValid;
				if (validationResult)
				{
					_repository.Update(stockTakingHeaderDb);
					await _repository.SaveChanges();
					return new ResponseDto() { Id = stockTakingHeaderDb.StockTakingHeaderId, Success = true, Message = _localizer["UpdateStockTakingSuccessMessage", $"{stockTakingHeaderDb.Prefix}{stockTakingHeaderDb.StockTakingCode}{stockTakingHeaderDb.Suffix}"] };
				}
				else
				{
					return new ResponseDto() { Id = 0, Success = false, Message = stockTakingHeaderValidator.ToString("~") };
				}
			}
			return new ResponseDto() { Id = 0, Success = false, Message = _localizer["NoStockTakingFound"] };
		}

		public async Task<ResponseDto> IsStockTakingCodeExist(int stockTakingHeaderId, bool separateYears, DateTime stockDate, int stockTakingCode, bool isOpenBalance, int storeId, string? prefix, string? suffix)
		{
			var stockTakingHeader = await _repository.GetAll().FirstOrDefaultAsync(x => (x.StoreId == storeId && (!separateYears || x.StockDate.Year == stockDate.Year) && x.StockTakingCode == stockTakingCode && x.Prefix == prefix && x.Suffix == suffix && x.IsOpenBalance == isOpenBalance) && x.StockTakingHeaderId != stockTakingHeaderId);
			if (stockTakingHeader != null)
			{
				return new ResponseDto() { Id = stockTakingHeader.StockTakingHeaderId, Success = true };
			}
			return new ResponseDto() { Id = 0, Success = false };
		}

		public async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.StockTakingHeaderId) + 1; } catch { id = 1; }
			return id;
		}


		public async Task<bool> UpdateStockTakingToBeCarriedOver(List<int> ids)
		{
			if (ids.Any())
			{
				var data = await _repository.GetAll().Where(x => ids.Contains(x.StockTakingHeaderId)).AsNoTracking().ToListAsync();
				data.ForEach(x => x.IsCarriedOver = true);
				_repository.UpdateRange(data);
				await _repository.SaveChanges();
				return true;
			}
			return false;
		}

		public async Task<bool> UpdateStockTakingToBeUnCarriedOver(List<int> ids)
		{
			if (ids.Any())
			{
				var data = await _repository.GetAll().Where(x => ids.Contains(x.StockTakingHeaderId)).AsNoTracking().ToListAsync();
				data.ForEach(x => x.IsCarriedOver = false);
				_repository.UpdateRange(data);
				await _repository.SaveChanges();
				return true;
			}
			return false;
		}

		public async Task<ResponseDto> DeleteStockTakingHeader(int id)
		{
			var stockTakingHeader = await _repository.GetAll().FirstOrDefaultAsync(x => x.StockTakingHeaderId == id);
			if (stockTakingHeader != null)
			{
				_repository.Delete(stockTakingHeader);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = id, Success = true, Message = _localizer["DeleteStockTakingMessage", $"{stockTakingHeader.Prefix}{stockTakingHeader.StockTakingCode}{stockTakingHeader.Suffix}"] };
			}
			return new ResponseDto() { Id = id, Success = false, Message = _localizer["NoStockTakingFound"] };
		}

		public async Task<int> GetNextStockTakingCode(int storeId, bool separateYears, DateTime stockDate, bool isOpenBalance, string? prefix, string? suffix)
		{
			int id = 1;
			try
			{
				id = await _repository.GetAll().AsNoTracking().Where(x => (!separateYears || x.StockDate.Year == stockDate.Year) && x.Prefix == prefix && x.Suffix == suffix && x.StoreId == storeId && x.IsOpenBalance == isOpenBalance).MaxAsync(a => a.StockTakingCode) + 1;
			}
			catch { id = 1; }
			return id;
		}
	}
}
