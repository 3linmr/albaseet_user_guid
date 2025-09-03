using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Sales.CoreOne.Contracts;
using Sales.CoreOne.Models.Domain;
using Sales.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne.Contracts.Menus;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.StaticData;
using Shared.CoreOne;
using Shared.Helper.Logic;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Shared.Service;
using Shared.Helper.Identity;
using Sales.Service.Validators;
using Shared.CoreOne.Contracts.Settings;

namespace Sales.Service.Services
{
	public class ClientDebitMemoService: BaseService<ClientDebitMemo>, IClientDebitMemoService
	{
		private readonly IClientService _clientService;
		private readonly IStoreService _storeService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IMenuEncodingService _menuEncodingService;
		private readonly ISalesInvoiceHeaderService _purchaseInvoiceHeaderService;
		private readonly IStringLocalizer<ClientDebitMemoService> _localizer;
		private readonly IGenericMessageService _genericMessageService;
		private readonly ISellerService _sellerService;
		private readonly IApplicationSettingService _applicationSettingService;

		public ClientDebitMemoService(ISalesInvoiceHeaderService purchaseInvoiceHeaderService, IRepository<ClientDebitMemo> repository, IClientService clientService, IStoreService storeService, IHttpContextAccessor httpContextAccessor, IMenuEncodingService menuEncodingService, IStringLocalizer<ClientDebitMemoService> localizer, IGenericMessageService genericMessageService, ISellerService sellerService, IApplicationSettingService applicationSettingService) : base(repository)
		{
			_purchaseInvoiceHeaderService = purchaseInvoiceHeaderService;
			_clientService = clientService;
			_storeService = storeService;
			_httpContextAccessor = httpContextAccessor;
			_menuEncodingService = menuEncodingService;
			_localizer = localizer;
			_genericMessageService = genericMessageService;
			_sellerService = sellerService;
			_applicationSettingService = applicationSettingService;
		}

		public IQueryable<ClientDebitMemoDto> GetClientDebitMemos()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var data = from clientDebitMemo in _repository.GetAll()
					   from store in _storeService.GetAll().Where(x => x.StoreId == clientDebitMemo.StoreId)
					   from client in _clientService.GetAll().Where(x => x.ClientId == clientDebitMemo.ClientId)
					   from purchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == clientDebitMemo.SalesInvoiceHeaderId)
					   from seller in _sellerService.GetAll().Where(x => x.SellerId == clientDebitMemo.SellerId).DefaultIfEmpty()
					   select new ClientDebitMemoDto
					   {
						   ClientDebitMemoId = clientDebitMemo.ClientDebitMemoId,
						   Prefix = clientDebitMemo.Prefix,
						   DocumentCode = clientDebitMemo.DocumentCode,
						   Suffix = clientDebitMemo.Suffix,
						   DocumentFullCode = $"{clientDebitMemo.Prefix}{clientDebitMemo.DocumentCode}{clientDebitMemo.Suffix}",
						   DocumentDate = clientDebitMemo.DocumentDate,
						   EntryDate = clientDebitMemo.EntryDate,
						   DocumentReference = clientDebitMemo.DocumentReference,
						   SalesInvoiceHeaderId = clientDebitMemo.SalesInvoiceHeaderId,
						   ClientId = clientDebitMemo.ClientId,
						   ClientCode = client.ClientCode,
						   ClientName = language == LanguageCode.Arabic ? client.ClientNameAr : client.ClientNameEn,
						   SellerId = clientDebitMemo.SellerId,
						   SellerCode = seller != null ? seller.SellerCode : null,
						   SellerName = seller != null ? language == LanguageCode.Arabic ? seller.SellerNameAr : seller.SellerNameEn : null,
						   StoreId = clientDebitMemo.StoreId,
						   StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
						   Reference = clientDebitMemo.Reference,
						   DebitAccountId = clientDebitMemo.DebitAccountId,
						   CreditAccountId = clientDebitMemo.CreditAccountId,
						   MemoValue = clientDebitMemo.MemoValue,
						   JournalHeaderId = clientDebitMemo.JournalHeaderId,
						   RemarksAr = clientDebitMemo.RemarksAr,
						   RemarksEn = clientDebitMemo.RemarksEn,
						   IsClosed = clientDebitMemo.IsClosed || purchaseInvoiceHeader.IsBlocked,
						   ArchiveHeaderId = clientDebitMemo.ArchiveHeaderId,

						   CreatedAt = clientDebitMemo.CreatedAt,
						   UserNameCreated = clientDebitMemo.UserNameCreated,
						   IpAddressCreated = clientDebitMemo.IpAddressCreated,

						   ModifiedAt = clientDebitMemo.ModifiedAt,
						   UserNameModified = clientDebitMemo.UserNameModified,
						   IpAddressModified = clientDebitMemo.IpAddressModified,
					   };

			return data;
		}

		public async Task<ClientDebitMemoDto?> GetClientDebitMemoById(int clientDebitMemoId)
		{
			return await GetClientDebitMemos().FirstOrDefaultAsync(x => x.ClientDebitMemoId == clientDebitMemoId);
		}

		public IQueryable<ClientDebitMemoDto> GetUserClientDebitMemos()
		{
			var userStore = _httpContextAccessor.GetCurrentUserStore();
			return GetClientDebitMemos().Where(x => x.StoreId == userStore);
		}

		public IQueryable<ClientDebitMemoDto> GetClientDebitMemosByStoreId(int storeId, int clientId)
		{
			return GetClientDebitMemos().Where(x => x.StoreId == storeId && x.ClientId == clientId && x.IsClosed == false);
		}

		public async Task<DocumentCodeDto> GetClientDebitMemoCode(int storeId, DateTime documentDate)
		{
            var separateYears = await _applicationSettingService.SeparateYears(storeId);
			return await GetClientDebitMemoCodeInternal(storeId, separateYears, documentDate);
		}

		public async Task<DocumentCodeDto> GetClientDebitMemoCodeInternal(int storeId, bool separateYears, DateTime documentDate)
		{
			var encoding = await _menuEncodingService.GetMenuEncoding(storeId, MenuCodeData.ClientDebitMemo);
			var code = await GetNextClientDebitMemoCode(storeId, separateYears, documentDate, encoding.Prefix, encoding.Suffix);
			return new DocumentCodeDto() { NextCode = code, Prefix = encoding.Prefix, Suffix = encoding.Suffix };
		}

		private async Task<int> GetNextClientDebitMemoCode(int storeId, bool separateYears, DateTime documentDate, string? prefix, string? suffix)
		{
			int id = 1;
			try
			{
				id = await _repository.GetAll().AsNoTracking().Where(x => (!separateYears || x.DocumentDate.Year == documentDate.Year) && x.Prefix == prefix && x.Suffix == suffix && x.StoreId == storeId).MaxAsync(a => a.DocumentCode) + 1;
			}
			catch { id = 1; }
			return id;
		}

		public async Task<ResponseDto> SaveClientDebitMemo(ClientDebitMemoDto clientDebitMemo, bool hasApprove, bool approved, int? requestId)
		{
            var separateYears = await _applicationSettingService.SeparateYears(clientDebitMemo.StoreId);

			if (hasApprove)
			{
				if (clientDebitMemo.ClientDebitMemoId == 0)
				{
					return await CreateClientDebitMemo(clientDebitMemo, hasApprove, approved, requestId, separateYears);
				}
				else
				{
					return await UpdateClientDebitMemo(clientDebitMemo);
				}
			}
			else
			{
				var clientDebitMemoExist = await IsClientDebitMemoCodeExist(clientDebitMemo.ClientDebitMemoId, clientDebitMemo.DocumentCode, clientDebitMemo.StoreId, separateYears, clientDebitMemo.DocumentDate, clientDebitMemo.Prefix, clientDebitMemo.Suffix);
				if (clientDebitMemoExist)
				{
					var nextClientDebitMemoCode = await GetNextClientDebitMemoCode(clientDebitMemo.StoreId, separateYears, clientDebitMemo.DocumentDate, clientDebitMemo.Prefix, clientDebitMemo.Suffix);
					return new ResponseDto() { Id = nextClientDebitMemoCode, ResponseType = ResponseTypeData.Confirm, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.ClientDebitMemo, GenericMessageData.CodeAlreadyExist, $"{nextClientDebitMemoCode}") };
				}
				else
				{
					if (clientDebitMemo.ClientDebitMemoId == 0)
					{
						return await CreateClientDebitMemo(clientDebitMemo, hasApprove, approved, requestId, separateYears);
					}
					else
					{
						return await UpdateClientDebitMemo(clientDebitMemo);
					}
				}
			}
		}

		private async Task<ResponseDto> CreateClientDebitMemo(ClientDebitMemoDto clientDebitMemo, bool hasApprove, bool approved, int? requestId, bool separateYears)
		{
			int clientDebitMemoCode;
			string? prefix;
			string? suffix;
			var nextClientDebitMemoCode = await GetClientDebitMemoCodeInternal(clientDebitMemo.StoreId, separateYears, clientDebitMemo.DocumentDate);
			if (hasApprove && approved)
			{
				clientDebitMemoCode = nextClientDebitMemoCode.NextCode;
				prefix = nextClientDebitMemoCode.Prefix;
				suffix = nextClientDebitMemoCode.Suffix;
			}
			else
			{
				clientDebitMemoCode = clientDebitMemo.DocumentCode != 0 ? clientDebitMemo.DocumentCode : nextClientDebitMemoCode.NextCode;
				prefix = string.IsNullOrWhiteSpace(clientDebitMemo.Prefix) ? nextClientDebitMemoCode.Prefix : clientDebitMemo.Prefix;
				suffix = string.IsNullOrWhiteSpace(clientDebitMemo.Suffix) ? nextClientDebitMemoCode.Suffix : clientDebitMemo.Suffix;
			}

			int newClientDebitMemoId = await GetNextId();
			var newClientDebitMemo = new ClientDebitMemo
			{
				ClientDebitMemoId = newClientDebitMemoId,
				Prefix = prefix,
				DocumentCode = clientDebitMemoCode,
				Suffix = suffix,
				DocumentDate = clientDebitMemo.DocumentDate,
				EntryDate = DateHelper.GetDateTimeNow(),
				DocumentReference = hasApprove ? $"{DocumentReferenceData.Approval}{requestId}" : $"{DocumentReferenceData.ClientDebitMemo}{newClientDebitMemoId}",
				SalesInvoiceHeaderId = clientDebitMemo.SalesInvoiceHeaderId,
				ClientId = clientDebitMemo.ClientId,
				SellerId = clientDebitMemo.SellerId,
				StoreId = clientDebitMemo.StoreId,
				Reference = clientDebitMemo.Reference,
				DebitAccountId = clientDebitMemo.DebitAccountId,
				CreditAccountId = clientDebitMemo.CreditAccountId,
				MemoValue = clientDebitMemo.MemoValue,
				JournalHeaderId = clientDebitMemo.JournalHeaderId,
				RemarksAr = clientDebitMemo.RemarksAr,
				RemarksEn = clientDebitMemo.RemarksEn,
				IsClosed = false,
				ArchiveHeaderId = clientDebitMemo.ArchiveHeaderId,

				CreatedAt = DateHelper.GetDateTimeNow(),
				UserNameCreated = await _httpContextAccessor!.GetUserName(),
				IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
				Hide = false,
			};

			var clientDebitMemoValidator = await new ClientDebitMemoValidator(_localizer).ValidateAsync(newClientDebitMemo);
			var validationResult = clientDebitMemoValidator.IsValid;
			if (validationResult)
			{
				await _repository.Insert(newClientDebitMemo);
				await _repository.SaveChanges();
				return new ResponseDto { Id = newClientDebitMemoId, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.ClientDebitMemo, GenericMessageData.CreatedSuccessWithCode, $"{newClientDebitMemo.Prefix}{newClientDebitMemo.DocumentCode}{newClientDebitMemo.Suffix}") };
			}
			else
			{
				return new ResponseDto { Id = newClientDebitMemoId, Success = false, Message = clientDebitMemoValidator.ToString("~") };
			}
		}

		private async Task<ResponseDto> UpdateClientDebitMemo(ClientDebitMemoDto clientDebitMemo)
		{
			var clientDebitMemoDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.ClientDebitMemoId == clientDebitMemo.ClientDebitMemoId);
			if (clientDebitMemoDb != null)
			{
				if (clientDebitMemoDb.IsClosed)
				{
					return new ResponseDto { Id = clientDebitMemo.ClientDebitMemoId, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.ClientDebitMemo, GenericMessageData.CannotUpdateBecauseClosed) };
				}

				clientDebitMemoDb.DocumentDate = clientDebitMemo.DocumentDate;
				clientDebitMemoDb.SalesInvoiceHeaderId = clientDebitMemo.SalesInvoiceHeaderId;
				clientDebitMemoDb.ClientId = clientDebitMemo.ClientId;
				clientDebitMemoDb.SellerId = clientDebitMemo.SellerId;
				clientDebitMemoDb.StoreId = clientDebitMemo.StoreId;
				clientDebitMemoDb.Reference = clientDebitMemo.Reference;
				clientDebitMemoDb.DebitAccountId = clientDebitMemo.DebitAccountId;
				clientDebitMemoDb.CreditAccountId = clientDebitMemo.CreditAccountId;
				clientDebitMemoDb.MemoValue = clientDebitMemo.MemoValue;
				clientDebitMemoDb.RemarksAr = clientDebitMemo.RemarksAr;
				clientDebitMemoDb.RemarksEn = clientDebitMemo.RemarksEn;
				clientDebitMemoDb.ArchiveHeaderId = clientDebitMemo.ArchiveHeaderId;

				clientDebitMemoDb.ModifiedAt = DateHelper.GetDateTimeNow();
				clientDebitMemoDb.UserNameModified = await _httpContextAccessor!.GetUserName();
				clientDebitMemoDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();

				var clientDebitMemoValidator = await new ClientDebitMemoValidator(_localizer).ValidateAsync(clientDebitMemoDb);
				var validationResult = clientDebitMemoValidator.IsValid;
				if (validationResult)
				{
					_repository.Update(clientDebitMemoDb);
					await _repository.SaveChanges();
					return new ResponseDto { Id = clientDebitMemo.ClientDebitMemoId, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.ClientDebitMemo, GenericMessageData.UpdatedSuccessWithCode, $"{clientDebitMemoDb.Prefix}{clientDebitMemoDb.DocumentCode}{clientDebitMemoDb.Suffix}") };
				}
				else
				{
					return new ResponseDto { Id = clientDebitMemo.ClientDebitMemoId, Success = false, Message = clientDebitMemoValidator.ToString("~") };
				}
			}
			return new ResponseDto { Id = clientDebitMemo.ClientDebitMemoId, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.ClientDebitMemo, GenericMessageData.NotFound) };
		}

		public async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.ClientDebitMemoId) + 1; } catch { id = 1; }
			return id;
		}

		private async Task<bool> IsClientDebitMemoCodeExist(int clientDebitMemoId, int documentCode, int storeId, bool separateYears, DateTime documentDate, string? prefix, string? suffix)
		{
			return await _repository.GetAll().Where(x => x.ClientDebitMemoId != clientDebitMemoId && (x.DocumentCode == documentCode && x.StoreId == storeId && (!separateYears || x.DocumentDate.Year == documentDate.Year) && x.Prefix == prefix && x.Suffix == suffix)).AnyAsync();
		}

		public async Task<ResponseDto> DeleteClientDebitMemo(int clientDebitMemoId)
		{
			var clientDebitMemo = await _repository.GetAll().FirstOrDefaultAsync(x => x.ClientDebitMemoId == clientDebitMemoId);
			if (clientDebitMemo != null)
			{
				_repository.Delete(clientDebitMemo);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = clientDebitMemoId, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.ClientDebitMemo, GenericMessageData.DeleteSuccessWithCode, $"{clientDebitMemo.Prefix}{clientDebitMemo.DocumentCode}{clientDebitMemo.Suffix}") };
			}
			return new ResponseDto() { Id = clientDebitMemoId, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.ClientDebitMemo, GenericMessageData.NotFound) };
		}
	}
}
