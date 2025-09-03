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
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Helper.Models.Dtos;
using Shared.Service;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Shared.CoreOne.Contracts.Settings;

namespace Inventory.Service.Services
{
	public class StockTakingCarryOverHeaderService : BaseService<StockTakingCarryOverHeader>, IStockTakingCarryOverHeaderService
	{
		private readonly IStoreService _storeService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IMenuEncodingService _menuEncodingService;
		private readonly IStringLocalizer<StockTakingCarryOverHeaderService> _localizer;
		private readonly IApplicationSettingService _applicationSettingService;

		public StockTakingCarryOverHeaderService(IRepository<StockTakingCarryOverHeader> repository,IStoreService storeService,IHttpContextAccessor httpContextAccessor,IMenuEncodingService menuEncodingService,IStringLocalizer<StockTakingCarryOverHeaderService> localizer, IApplicationSettingService applicationSettingService) : base(repository)
		{
			_storeService = storeService;
			_httpContextAccessor = httpContextAccessor;
			_menuEncodingService = menuEncodingService;
			_localizer = localizer;
			_applicationSettingService = applicationSettingService;
		}

		public IQueryable<StockTakingCarryOverHeaderDto> GetStockTakingCarryOverHeaders()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var data =
				from stockTakingCarryOverHeader in _repository.GetAll()
				from store in _storeService.GetAll().Where(x=>x.StoreId == stockTakingCarryOverHeader.StoreId)
				select new StockTakingCarryOverHeaderDto
				{
					StockTakingCarryOverHeaderId = stockTakingCarryOverHeader.StockTakingCarryOverHeaderId,
					StoreId = stockTakingCarryOverHeader.StoreId,
					DocumentDate = stockTakingCarryOverHeader.DocumentDate,
					IsOpenBalance = stockTakingCarryOverHeader.IsOpenBalance,
					Reference = stockTakingCarryOverHeader.Reference,
					IsAllItemsAffected = stockTakingCarryOverHeader.IsAllItemsAffected,
					StockTakingCarryOverNameAr = stockTakingCarryOverHeader.StockTakingCarryOverNameAr,
					StockTakingCarryOverNameEn = stockTakingCarryOverHeader.StockTakingCarryOverNameEn,
					RemarksEn = stockTakingCarryOverHeader.RemarksEn,
					RemarksAr = stockTakingCarryOverHeader.RemarksAr,
					StockTakingList = stockTakingCarryOverHeader.StockTakingList,
					TotalCurrentBalanceConsumerValue = stockTakingCarryOverHeader.TotalCurrentBalanceConsumerValue,
					TotalCurrentBalanceCostValue = stockTakingCarryOverHeader.TotalCurrentBalanceCostValue,
					TotalStockTakingConsumerValue = stockTakingCarryOverHeader.TotalStockTakingConsumerValue,
					TotalStockTakingCostValue = stockTakingCarryOverHeader.TotalStockTakingCostValue,
					StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
					DocumentReference = stockTakingCarryOverHeader.DocumentReference,
					Prefix = stockTakingCarryOverHeader.Prefix,
					Suffix = stockTakingCarryOverHeader.Suffix,
					StockTakingCarryOverFullCode = $"{stockTakingCarryOverHeader.Prefix}{stockTakingCarryOverHeader.StockTakingCarryOverCode}{stockTakingCarryOverHeader.Suffix}",
					StockTakingCarryOverCode = stockTakingCarryOverHeader.StockTakingCarryOverCode,
					EntryDate = stockTakingCarryOverHeader.EntryDate
				};
			return data;
        }

        public IQueryable<StockTakingCarryOverHeaderDto> GetUserStockTakingCarryOverHeaders(bool isOpenBalance)
        {
            var userStore = _httpContextAccessor.GetCurrentUserStore();
            return GetStockTakingCarryOverHeaders().Where(x => x.StoreId == userStore && x.IsOpenBalance == isOpenBalance);
        }

        public IQueryable<StockTakingCarryOverHeaderDto> GetStockTakingCarryOverHeaders(bool isOpenBalance)
		{
			return GetStockTakingCarryOverHeaders().Where(x => x.IsOpenBalance == isOpenBalance);
		}

		public IQueryable<StockTakingCarryOverHeaderDto> GetStockTakingCarryOverHeadersByStoreId(int storeId)
		{
			return GetStockTakingCarryOverHeaders().Where(x => x.StoreId == storeId);
		}

		public async Task<StockTakingCarryOverHeaderDto?> GetStockTakingCarryOverHeaderById(int id)
		{
			return await GetStockTakingCarryOverHeaders().FirstOrDefaultAsync(x => x.StockTakingCarryOverHeaderId == id);
		}

		public async Task<DocumentCodeDto> GetStockTakingCarryOverCode(int storeId, DateTime documentDate, bool isOpenBalance)
		{
			var separateYears = await _applicationSettingService.SeparateYears(storeId);
			return await GetStockTakingCarryOverCodeInternal(storeId, separateYears, documentDate, isOpenBalance);
		}

		public async Task<DocumentCodeDto> GetStockTakingCarryOverCodeInternal(int storeId, bool separateYears, DateTime documentDate, bool isOpenBalance)
		{
			var encoding = await _menuEncodingService.GetMenuEncoding(storeId, isOpenBalance ? MenuCodeData.StockTakingOpenBalance : MenuCodeData.StockTakingCurrentBalance);
			var code = await GetNextStockTakingCode(storeId, separateYears, documentDate, isOpenBalance, encoding.Prefix, encoding.Suffix);
			return new DocumentCodeDto() { NextCode = code, Prefix = encoding.Prefix, Suffix = encoding.Suffix };
		}

		public async Task<int> GetNextStockTakingCode(int storeId, bool separateYears, DateTime documentDate, bool isOpenBalance, string? prefix, string? suffix)
		{
			int id = 1;
			try
			{
				id = await _repository.GetAll().AsNoTracking().Where(x => (!separateYears || x.DocumentDate.Year == documentDate.Year) && x.Prefix == prefix && x.Suffix == suffix && x.StoreId == storeId && x.IsOpenBalance == isOpenBalance).MaxAsync(a => a.StockTakingCarryOverCode) + 1;
			}
			catch { id = 1; }
			return id;
		}

		public async Task<ResponseDto> SaveStockTakingCarryOverHeader(StockTakingCarryOverHeaderDto stockTakingHeader, bool hasApprove, bool approved, int? requestId)
		{
			var separateYears = await _applicationSettingService.SeparateYears(stockTakingHeader.StoreId);

			if (hasApprove)
			{
				if (stockTakingHeader.StockTakingCarryOverHeaderId == 0)
				{
					return await CreateStockTakingHeader(stockTakingHeader, hasApprove, approved, requestId, separateYears);
				}
			}
			else
			{
				var stockTakingHeaderExist = await IsStockTakingCodeExist(stockTakingHeader.StockTakingCarryOverHeaderId, separateYears, stockTakingHeader.DocumentDate,stockTakingHeader.StockTakingCarryOverCode, stockTakingHeader.IsOpenBalance, stockTakingHeader.StoreId, stockTakingHeader.Prefix, stockTakingHeader.Suffix);
				if (stockTakingHeaderExist.Success)
				{
					var nextJournalCode = await GetNextStockTakingCode(stockTakingHeader.StoreId, separateYears, stockTakingHeader.DocumentDate, stockTakingHeader.IsOpenBalance, stockTakingHeader.Prefix, stockTakingHeader.Suffix);
					return new ResponseDto() { Id = nextJournalCode, ResponseType = ResponseTypeData.Confirm, Success = false, Message = _localizer["StockTakingAlreadyExist", nextJournalCode] };
				}
				else
				{
					if (stockTakingHeader.StockTakingCarryOverHeaderId == 0)
					{
						return await CreateStockTakingHeader(stockTakingHeader, hasApprove, approved, requestId, separateYears);
					}
				}
			}
			return new ResponseDto();
		}

		public async Task<ResponseDto> CreateStockTakingHeader(StockTakingCarryOverHeaderDto stockTakingHeader, bool hasApprove, bool approved, int? requestId, bool separateYears)
		{
			int stockTakingCode;
			var nextStockTakingCode = await GetStockTakingCarryOverCodeInternal(stockTakingHeader.StoreId, separateYears, stockTakingHeader.DocumentDate, stockTakingHeader.IsOpenBalance);
			if (hasApprove && approved)
			{
				stockTakingCode = nextStockTakingCode.NextCode;
			}
			else
			{
				stockTakingCode = stockTakingHeader.StockTakingCarryOverCode != 0 ? stockTakingHeader.StockTakingCarryOverCode : nextStockTakingCode.NextCode;
			}

			var stockTakingHeaderId = await GetNextId();
			var newStockTakingHeader = new StockTakingCarryOverHeader()
			{
				StockTakingCarryOverHeaderId = stockTakingHeaderId,
				StoreId = stockTakingHeader.StoreId,
				DocumentDate = stockTakingHeader.DocumentDate,
				DocumentReference = hasApprove ? $"{DocumentReferenceData.Approval}{requestId}" : $"{(stockTakingHeader.IsOpenBalance ? DocumentReferenceData.CarryOverOpenBalance : DocumentReferenceData.CarryOver)}{stockTakingHeaderId}",
				Reference = stockTakingHeader.Reference,
				StockTakingCarryOverNameAr = stockTakingHeader.StockTakingCarryOverNameAr,
				StockTakingCarryOverNameEn = stockTakingHeader.StockTakingCarryOverNameEn,
				EntryDate = DateHelper.GetDateTimeNow(),
				Prefix = nextStockTakingCode.Prefix,
				StockTakingCarryOverCode = stockTakingCode,
				Suffix = nextStockTakingCode.Suffix,
				TotalCurrentBalanceConsumerValue = stockTakingHeader.TotalCurrentBalanceConsumerValue,
				TotalStockTakingConsumerValue = stockTakingHeader.TotalStockTakingConsumerValue,
				TotalCurrentBalanceCostValue = stockTakingHeader.TotalCurrentBalanceCostValue,
				TotalStockTakingCostValue = stockTakingHeader.TotalStockTakingCostValue,
				IsOpenBalance = stockTakingHeader.IsOpenBalance,
				RemarksAr = stockTakingHeader.RemarksAr,
				RemarksEn = stockTakingHeader.RemarksEn,
				IsAllItemsAffected = stockTakingHeader.IsAllItemsAffected,
				StockTakingList = stockTakingHeader.StockTakingList,
				CreatedAt = DateHelper.GetDateTimeNow(),
				UserNameCreated = await _httpContextAccessor!.GetUserName(),
				IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
				Hide = false,
			};

			var stockTakingHeaderValidator = await new StockTakingCarryOverHeaderValidator(_localizer).ValidateAsync(newStockTakingHeader);
			var validationResult = stockTakingHeaderValidator.IsValid;
			if (validationResult)
			{
				await _repository.Insert(newStockTakingHeader);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = newStockTakingHeader.StockTakingCarryOverHeaderId, Success = true, Message = _localizer["NewStockTakingSuccessMessage", $"{stockTakingHeader.Prefix}{newStockTakingHeader.StockTakingCarryOverCode}{stockTakingHeader.Suffix}"] };
			}
			else
			{
				return new ResponseDto() { Id = newStockTakingHeader.StockTakingCarryOverHeaderId, Success = false, Message = stockTakingHeaderValidator.ToString("~") };
			}
		}

		public async Task<ResponseDto> IsStockTakingCodeExist(int stockTakingHeaderId, bool separateYears, DateTime documentDate, int stockTakingCode, bool isOpenBalance, int storeId, string? prefix, string? suffix)
		{
			var stockTakingHeader = await _repository.GetAll().FirstOrDefaultAsync(x => (x.StoreId == storeId && (!separateYears || x.DocumentDate.Year == documentDate.Year) && x.StockTakingCarryOverCode == stockTakingCode && x.Prefix == prefix && x.Suffix == suffix && x.IsOpenBalance == isOpenBalance) && x.StockTakingCarryOverHeaderId != stockTakingHeaderId);
			if (stockTakingHeader != null)
			{
				return new ResponseDto() { Id = stockTakingHeader.StockTakingCarryOverHeaderId, Success = true };
			}
			return new ResponseDto() { Id = 0, Success = false };
		}

		public async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.StockTakingCarryOverHeaderId) + 1; } catch { id = 1; }
			return id;
		}

		public async Task<ResponseDto> DeleteStockTakingCarryOverHeader(int id)
		{
			var stockTakingHeader = await _repository.GetAll().AsNoTracking().FirstOrDefaultAsync(x => x.StockTakingCarryOverHeaderId == id);
			if (stockTakingHeader != null)
			{
				_repository.Delete(stockTakingHeader);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = id, Success = true, Message = _localizer["DeleteStockTakingMessage", $"{stockTakingHeader.Prefix}{stockTakingHeader.StockTakingCarryOverCode}{stockTakingHeader.Suffix}"] };

			}
			return new ResponseDto() { Id = id, Success = false, Message = _localizer["NoStockTakingFound"] };
		}
	}
}
