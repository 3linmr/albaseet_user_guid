using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Sales.CoreOne.Contracts;
using Sales.CoreOne.Models.Domain;
using Sales.CoreOne.Models.Dtos.ViewModels;
using Sales.Service.Validators;
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
using Shared.CoreOne.Contracts.Settings;

namespace Sales.Service.Services
{
	public class ClientCreditMemoService: BaseService<ClientCreditMemo>, IClientCreditMemoService
	{
		private readonly IClientService _clientService;
		private readonly IStoreService _storeService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IMenuEncodingService _menuEncodingService;
		private readonly ISalesInvoiceHeaderService _purchaseInvoiceHeaderService;
		private readonly IStringLocalizer<ClientCreditMemoService> _localizer;
		private readonly IGenericMessageService _genericMessageService;
		private readonly ISellerService _sellerService;
		private readonly IApplicationSettingService _applicationSettingService;

		public ClientCreditMemoService(ISalesInvoiceHeaderService purchaseInvoiceHeaderService, IRepository<ClientCreditMemo> repository, IClientService clientService, IStoreService storeService, IHttpContextAccessor httpContextAccessor, IMenuEncodingService menuEncodingService, IStringLocalizer<ClientCreditMemoService> localizer, IGenericMessageService genericMessageService, ISellerService sellerService, IApplicationSettingService applicationSettingService) : base(repository)
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

		public IQueryable<ClientCreditMemoDto> GetClientCreditMemos()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var data = from clientDebitMemo in _repository.GetAll()
					   from store in _storeService.GetAll().Where(x => x.StoreId == clientDebitMemo.StoreId)
					   from client in _clientService.GetAll().Where(x => x.ClientId == clientDebitMemo.ClientId)
					   from purchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == clientDebitMemo.SalesInvoiceHeaderId)
					   from seller in _sellerService.GetAll().Where(x => x.SellerId == clientDebitMemo.SellerId).DefaultIfEmpty()
					   select new ClientCreditMemoDto
					   {
						   ClientCreditMemoId = clientDebitMemo.ClientCreditMemoId,
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
						   SellerName = language == LanguageCode.Arabic ? seller.SellerNameAr : seller.SellerNameEn,
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

		public async Task<ClientCreditMemoDto?> GetClientCreditMemoById(int clientDebitMemoId)
		{
			return await GetClientCreditMemos().FirstOrDefaultAsync(x => x.ClientCreditMemoId == clientDebitMemoId);
		}

		public IQueryable<ClientCreditMemoDto> GetUserClientCreditMemos()
		{
			var userStore = _httpContextAccessor.GetCurrentUserStore();
			return GetClientCreditMemos().Where(x => x.StoreId == userStore);
		}

		public IQueryable<ClientCreditMemoDto> GetClientCreditMemosByStoreId(int storeId, int clientId)
		{
			return GetClientCreditMemos().Where(x => x.StoreId == storeId && x.ClientId == clientId && x.IsClosed == false);
		}

		public async Task<DocumentCodeDto> GetClientCreditMemoCode(int storeId, DateTime documentDate)
		{
            var separateYears = await _applicationSettingService.SeparateYears(storeId);
			return await GetClientCreditMemoCodeInternal(storeId, separateYears, documentDate);
		}

		public async Task<DocumentCodeDto> GetClientCreditMemoCodeInternal(int storeId, bool separateYears, DateTime documentDate)
		{
			var encoding = await _menuEncodingService.GetMenuEncoding(storeId, MenuCodeData.ClientCreditMemo);
			var code = await GetNextClientCreditMemoCode(storeId, separateYears, documentDate, encoding.Prefix, encoding.Suffix);
			return new DocumentCodeDto() { NextCode = code, Prefix = encoding.Prefix, Suffix = encoding.Suffix };
		}

		private async Task<int> GetNextClientCreditMemoCode(int storeId, bool separateYears, DateTime documentDate, string? prefix, string? suffix)
		{
			int id = 1;
			try
			{
				id = await _repository.GetAll().AsNoTracking().Where(x => (!separateYears || x.DocumentDate.Year == documentDate.Year) && x.Prefix == prefix && x.Suffix == suffix && x.StoreId == storeId).MaxAsync(a => a.DocumentCode) + 1;
			}
			catch { id = 1; }
			return id;
		}

		public async Task<ResponseDto> SaveClientCreditMemo(ClientCreditMemoDto clientDebitMemo, bool hasApprove, bool approved, int? requestId)
		{
            var separateYears = await _applicationSettingService.SeparateYears(clientDebitMemo.StoreId);

			if (hasApprove)
			{
				if (clientDebitMemo.ClientCreditMemoId == 0)
				{
					return await CreateClientCreditMemo(clientDebitMemo, hasApprove, approved, requestId, separateYears);
				}
				else
				{
					return await UpdateClientCreditMemo(clientDebitMemo);
				}
			}
			else
			{
				var clientDebitMemoExist = await IsClientCreditMemoCodeExist(clientDebitMemo.ClientCreditMemoId, clientDebitMemo.DocumentCode, clientDebitMemo.StoreId, separateYears, clientDebitMemo.DocumentDate, clientDebitMemo.Prefix, clientDebitMemo.Suffix);
				if (clientDebitMemoExist)
				{
					var nextClientCreditMemoCode = await GetNextClientCreditMemoCode(clientDebitMemo.StoreId, separateYears, clientDebitMemo.DocumentDate, clientDebitMemo.Prefix, clientDebitMemo.Suffix);
					return new ResponseDto() { Id = nextClientCreditMemoCode, ResponseType = ResponseTypeData.Confirm, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.ClientCreditMemo, GenericMessageData.CodeAlreadyExist, $"{nextClientCreditMemoCode}") };
				}
				else
				{
					if (clientDebitMemo.ClientCreditMemoId == 0)
					{
						return await CreateClientCreditMemo(clientDebitMemo, hasApprove, approved, requestId, separateYears);
					}
					else
					{
						return await UpdateClientCreditMemo(clientDebitMemo);
					}
				}
			}
		}

		private async Task<ResponseDto> CreateClientCreditMemo(ClientCreditMemoDto clientDebitMemo, bool hasApprove, bool approved, int? requestId, bool separateYears)
		{
			int clientDebitMemoCode;
			string? prefix;
			string? suffix;
			var nextClientCreditMemoCode = await GetClientCreditMemoCodeInternal(clientDebitMemo.StoreId, separateYears, clientDebitMemo.DocumentDate);
			if (hasApprove && approved)
			{
				clientDebitMemoCode = nextClientCreditMemoCode.NextCode;
				prefix = nextClientCreditMemoCode.Prefix;
				suffix = nextClientCreditMemoCode.Suffix;
			}
			else
			{
				clientDebitMemoCode = clientDebitMemo.DocumentCode != 0 ? clientDebitMemo.DocumentCode : nextClientCreditMemoCode.NextCode;
				prefix = string.IsNullOrWhiteSpace(clientDebitMemo.Prefix) ? nextClientCreditMemoCode.Prefix : clientDebitMemo.Prefix;
				suffix = string.IsNullOrWhiteSpace(clientDebitMemo.Suffix) ? nextClientCreditMemoCode.Suffix : clientDebitMemo.Suffix;
			}

			int newClientCreditMemoId = await GetNextId();
			var newClientCreditMemo = new ClientCreditMemo
			{
				ClientCreditMemoId = newClientCreditMemoId,
				Prefix = prefix,
				DocumentCode = clientDebitMemoCode,
				Suffix = suffix,
				DocumentDate = clientDebitMemo.DocumentDate,
				EntryDate = DateHelper.GetDateTimeNow(),
				DocumentReference = hasApprove ? $"{DocumentReferenceData.Approval}{requestId}" : $"{DocumentReferenceData.ClientCreditMemo}{newClientCreditMemoId}",
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

			var clientDebitMemoValidator = await new ClientCreditMemoValidator(_localizer).ValidateAsync(newClientCreditMemo);
			var validationResult = clientDebitMemoValidator.IsValid;
			if (validationResult)
			{
				await _repository.Insert(newClientCreditMemo);
				await _repository.SaveChanges();
				return new ResponseDto { Id = newClientCreditMemoId, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.ClientCreditMemo, GenericMessageData.CreatedSuccessWithCode, $"{newClientCreditMemo.Prefix}{newClientCreditMemo.DocumentCode}{newClientCreditMemo.Suffix}") };
			}
			else
			{
				return new ResponseDto { Id = newClientCreditMemoId, Success = false, Message = clientDebitMemoValidator.ToString("~") };
			}
		}

		private async Task<ResponseDto> UpdateClientCreditMemo(ClientCreditMemoDto clientDebitMemo)
		{
			var clientDebitMemoDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.ClientCreditMemoId == clientDebitMemo.ClientCreditMemoId);
			if (clientDebitMemoDb != null)
			{
				if (clientDebitMemoDb.IsClosed)
				{
					return new ResponseDto { Id = clientDebitMemo.ClientCreditMemoId, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.ClientCreditMemo, GenericMessageData.CannotUpdateBecauseClosed) };
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

				var clientDebitMemoValidator = await new ClientCreditMemoValidator(_localizer).ValidateAsync(clientDebitMemoDb);
				var validationResult = clientDebitMemoValidator.IsValid;
				if (validationResult)
				{
					_repository.Update(clientDebitMemoDb);
					await _repository.SaveChanges();
					return new ResponseDto { Id = clientDebitMemo.ClientCreditMemoId, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.ClientCreditMemo, GenericMessageData.UpdatedSuccessWithCode, $"{clientDebitMemoDb.Prefix}{clientDebitMemoDb.DocumentCode}{clientDebitMemoDb.Suffix}") };
				}
				else
				{
					return new ResponseDto { Id = clientDebitMemo.ClientCreditMemoId, Success = false, Message = clientDebitMemoValidator.ToString("~") };
				}
			}
			return new ResponseDto { Id = clientDebitMemo.ClientCreditMemoId, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.ClientCreditMemo, GenericMessageData.NotFound) };
		}

		public async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.ClientCreditMemoId) + 1; } catch { id = 1; }
			return id;
		}

		private async Task<bool> IsClientCreditMemoCodeExist(int clientDebitMemoId, int documentCode, int storeId, bool separateYears, DateTime documentDate, string? prefix, string? suffix)
		{
			return await _repository.GetAll().Where(x => x.ClientCreditMemoId != clientDebitMemoId && (x.DocumentCode == documentCode && x.StoreId == storeId && (!separateYears || x.DocumentDate.Year == documentDate.Year) && x.Prefix == prefix && x.Suffix == suffix)).AnyAsync();
		}

		public async Task<ResponseDto> DeleteClientCreditMemo(int clientDebitMemoId)
		{
			var clientDebitMemo = await _repository.GetAll().FirstOrDefaultAsync(x => x.ClientCreditMemoId == clientDebitMemoId);
			if (clientDebitMemo != null)
			{
				_repository.Delete(clientDebitMemo);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = clientDebitMemoId, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.ClientCreditMemo, GenericMessageData.DeleteSuccessWithCode, $"{clientDebitMemo.Prefix}{clientDebitMemo.DocumentCode}{clientDebitMemo.Suffix}") };
			}
			return new ResponseDto() { Id = clientDebitMemoId, Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.ClientCreditMemo, GenericMessageData.NotFound) };
		}
	}
}
