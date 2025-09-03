using Shared.CoreOne.Contracts.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Journal;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.CoreOne.Models.Dtos.ViewModels.Journal;
using Shared.Helper.Models.Dtos;
using static Shared.CoreOne.Models.StaticData.StaticData;
using System.Reflection.PortableExecutable;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Contracts.Accounts;
using Shared.Service.Logic.Approval;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.StaticData;
using Shared.CoreOne.Models.Domain.Modules;
using Accounting.CoreOne.Models.StaticData;
using Shared.Helper.Logic;
using Microsoft.AspNetCore.Http;
using Shared.Helper.Identity;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Shared.Helper.Extensions;

namespace Shared.Service.Services.Journal
{
	public class JournalService : IJournalService
	{
		private readonly IJournalHeaderService _journalHeaderService;
		private readonly IJournalDetailService _journalDetailService;
		private readonly ICostCenterJournalDetailService _costCenterJournalDetailService;
		private readonly IMenuNoteService _menuNoteService;
		private readonly IStringLocalizer<JournalService> _localizer;
		private readonly IAccountService _accountService;
		private readonly IStoreService _storeService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private static List<JournalDetail> _journalDetailCreate = new();
		private static List<JournalDetail> _journalDetailUpdate = new();
		private static List<JournalDetail> _journalDetailDelete = new();
		private static List<CostCenterJournalDetail> _costCenterJournalDetailCreate = new();
		private static List<CostCenterJournalDetail> _costCenterJournalDetailUpdate = new();
		private static List<CostCenterJournalDetail> _costCenterJournalDetailDelete = new();
		private readonly IClientService _clientService;
		private readonly ISupplierService _supplierService;
		private readonly IBankService _bankService;
		private readonly ISellerService _sellerService;

		public JournalService(IJournalHeaderService journalHeaderService, IJournalDetailService journalDetailService, ICostCenterJournalDetailService costCenterJournalDetailService, IMenuNoteService menuNoteService, IStringLocalizer<JournalService> localizer, IAccountService accountService,IStoreService storeService, IHttpContextAccessor httpContextAccessor, IClientService clientService , ISupplierService supplierService, IBankService bankService, ISellerService sellerService)
		{
			_journalHeaderService = journalHeaderService;
			_journalDetailService = journalDetailService;
			_costCenterJournalDetailService = costCenterJournalDetailService;
			_menuNoteService = menuNoteService;
			_localizer = localizer;
			_accountService = accountService;
			_storeService = storeService;
			_httpContextAccessor = httpContextAccessor;
			_clientService = clientService;
			_supplierService = supplierService;
			_bankService = bankService;
			_sellerService = sellerService;
		}

		public List<RequestChangesDto> GetJournalEntryRequestChanges(JournalDto oldItem, JournalDto newItem)
		{
			var requestChanges = new List<RequestChangesDto>();
			var items = CompareLogic.GetDifferences(oldItem.JournalHeader, newItem.JournalHeader);
			requestChanges.AddRange(items);

			if (oldItem.JournalDetails != null && oldItem.JournalDetails.Any() && newItem.JournalDetails != null && newItem.JournalDetails.Any())
			{
				var oldCount = oldItem.JournalDetails.Count;
				var newCount = newItem.JournalDetails.Count;
				var index = 0;
				if (oldCount == newCount)
				{
					for (int i = index; i < oldCount; i++)
					{
						for (int j = index; j < newCount; j++)
						{
							var changes = CompareLogic.GetDifferences(oldItem.JournalDetails[i], newItem.JournalDetails[i]);
							requestChanges.AddRange(changes);
							index++;
							break;
						}
					}
				}
			}
			if (oldItem.CostCenterJournalDetails != null && oldItem.CostCenterJournalDetails.Any() && newItem.CostCenterJournalDetails != null && newItem.CostCenterJournalDetails.Any())
			{
				var oldCount = oldItem.CostCenterJournalDetails.Count;
				var newCount = newItem.CostCenterJournalDetails.Count;
				var index = 0;
				if (oldCount == newCount)
				{
					for (int i = index; i < oldCount; i++)
					{
						for (int j = index; j < newCount; j++)
						{
							var changes = CompareLogic.GetDifferences(oldItem.CostCenterJournalDetails[i], newItem.CostCenterJournalDetails[i]);
							requestChanges.AddRange(changes);
							index++;
							break;
						}
					}
				}
			}

			if (oldItem.MenuNotes != null && oldItem.MenuNotes.Any() && newItem.MenuNotes != null && newItem.MenuNotes.Any())
			{
				var oldCount = oldItem.MenuNotes.Count;
				var newCount = newItem.MenuNotes.Count;
				var index = 0;
				if (oldCount <= newCount)
				{
					for (int i = index; i < oldCount; i++)
					{
						for (int j = index; j < newCount; j++)
						{
							var changes = CompareLogic.GetDifferences(oldItem.MenuNotes[i], newItem.MenuNotes[i]);
							requestChanges.AddRange(changes);
							index++;
							break;
						}
					}
				}
			}

			return requestChanges;
		}

		public async Task<ResponseDto> CheckExistingTaxReference(string taxReference, DateTime ticketDate)
		{
			return await CheckExistingTaxReferences([taxReference], ticketDate);
		}

		public async Task<ResponseDto> CheckExistingTaxReferences(List<string> taxReferences, DateTime ticketDate)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var companyId = _httpContextAccessor.GetCurrentUserCompany();

			var journalCompany = await (from journalHeader in _journalHeaderService.GetAll()
										from journalDetail in _journalDetailService.GetAll().Where(x => x.JournalHeaderId == journalHeader.JournalHeaderId)
										from account in _accountService.GetAll().Where(x => x.AccountId == journalDetail.AccountId)
										where account.CompanyId == companyId && journalHeader.TicketDate.Year == ticketDate.Year && taxReferences.Contains(journalDetail.TaxReference!)
										select new JournalCompanyDto
										{
											JournalHeaderId = journalDetail.JournalHeaderId,
											JournalDetailId = journalDetail.JournalDetailId,
											JournalCode = journalHeader.JournalCode,
											CompanyId = account.CompanyId,
											TicketDate = journalHeader.TicketDate,
											TaxReference = journalDetail.TaxReference,
											EntityName = language == LanguageCode.Arabic ? journalDetail.EntityNameAr : journalDetail.EntityNameEn
										}).FirstOrDefaultAsync();

			if (journalCompany != null)
			{
				return new ResponseDto { Success = true, Message = _localizer[journalCompany.EntityName != null ? "ReferenceNumberExistsWithName" : "ReferenceNumberExists", journalCompany.TaxReference!, journalCompany.JournalCode, journalCompany.EntityName ?? "", journalCompany.TicketDate.ToShortDateString()] };
			}
			else
			{
				return new ResponseDto { Success = false };
			}
		}

		public async Task<JournalDto> GetJournal(int journalHeaderId)
		{
			var data = new JournalDto()
			{
				JournalHeader = await _journalHeaderService.GetJournalHeader(journalHeaderId),
				JournalDetails = await _journalDetailService.GetJournalDetailByHeaderId(journalHeaderId),
				CostCenterJournalDetails = await _costCenterJournalDetailService.GetCostCenterJournalDetails(journalHeaderId),
				MenuNotes = await _menuNoteService.GetMenuNotes(MenuCodeData.JournalEntry, journalHeaderId).ToListAsync()
			};
			return data;
		}

		public async Task<ResponseDto> CheckOpenBalanceJournalExists(JournalDto journal)
		{
			var journalHeader = journal.JournalHeader!;
			if (journalHeader.JournalTypeId == JournalTypeData.OpeningBalance)
			{
				var accounts = journal.JournalDetails!.Select(x => x.AccountId);

				var existingOpenJournalDetail = await (from journalHeaderDb in _journalHeaderService.GetAll()
													   from journalDetail in _journalDetailService.GetAll().Where(x => x.JournalHeaderId == journalHeaderDb.JournalHeaderId)
													   where journalHeaderDb.JournalTypeId == JournalTypeData.OpeningBalance
														  && journalHeaderDb.JournalHeaderId != journalHeader.JournalHeaderId
														  && journalHeaderDb.TicketDate.Year == journalHeader.TicketDate.Year
														  && journalHeaderDb.StoreId == journalHeader.StoreId
														  && accounts.Contains(journalDetail.AccountId)
													   select journalDetail).FirstOrDefaultAsync();

				if (existingOpenJournalDetail is not null)
				{
					var language = _httpContextAccessor.GetProgramCurrentLanguage();

					var accountId = existingOpenJournalDetail.AccountId;
					var account = await _accountService.GetAccountByAccountId(accountId);
					var accountName = language == LanguageCode.Arabic ? account.AccountNameAr : account.AccountNameEn;
					return new ResponseDto { Success = false, Message = _localizer["OpeningBalanceExists", accountName!, journalHeader.TicketDate.Year, Math.Max(existingOpenJournalDetail.CreditValue, existingOpenJournalDetail.DebitValue).ToNormalizedString()] };
				}
			}
			return new ResponseDto { Success = true };
		}

		public async Task<ResponseDto> ValidateJournal(JournalDto journal)
		{
			var isBalanced = IsJournalIsBalanced(journal.JournalDetails);
			if (!isBalanced) 
				return new ResponseDto() { Id = 0, Success = false, Message = _localizer["JournalNotBalanced"] };

			var openingBalanceExists = await CheckOpenBalanceJournalExists(journal);
			if (openingBalanceExists.Success == false) return openingBalanceExists;

			//return await CheckJournalExistingTaxReferences(journal.JournalHeader!.JournalHeaderId, journal.JournalDetails!, journal.JournalHeader!.TicketDate);
			return new ResponseDto { Success = true };
		}

		public void TrimModelStrings(JournalDto journal)
		{
			journal.JournalDetails!.ForEach(x => x.TaxReference = !string.IsNullOrWhiteSpace(x.TaxReference) ? x.TaxReference.Trim() : null);
		}

		public async Task DetermineJournalEntityTypes(List<JournalDetailDto> journalDetails)
		{
			var accountIds = journalDetails.Select(x => x.AccountId).ToList();
			var taxJournals = journalDetails.Where(x => x.IsTax);
			var accountTypes = await _clientService.GetAll().Where(x => accountIds.Contains(x.AccountId ?? 0)).Select(x => new
			{
				AccountId = x.AccountId,
				EntityId = x.ClientId,
				EntityTypeId = EntityTypeData.Client
			}).Concat(_supplierService.GetAll().Where(x => accountIds.Contains(x.AccountId ?? 0)).Select(x => new
			{
				AccountId = x.AccountId,
				EntityId = x.SupplierId,
				EntityTypeId = EntityTypeData.Supplier
			})).Concat(_bankService.GetAll().Where(x => accountIds.Contains(x.AccountId ?? 0)).Select(x => new
			{
				AccountId = x.AccountId,
				EntityId = x.BankId,
				EntityTypeId = EntityTypeData.Bank
			})).GroupBy(x => x.AccountId ?? 0).Select(x => new
			{
				//Key
				AccountId = x.Key,
				//Values
				EntityId = x.First().EntityId,
				EntityTypeId = x.First().EntityTypeId
			}).ToDictionaryAsync(x => x.AccountId, x => new { x.EntityId, x.EntityTypeId }); //TODO: Add Sellers here when link with account implemented

			foreach (var journalDetail in journalDetails)
			{
				var accountType = accountTypes.GetValueOrDefault(journalDetail.AccountId); 
				if (accountType != null)
				{
					journalDetail.EntityId = accountType?.EntityId;
					journalDetail.EntityTypeId = accountType?.EntityTypeId;

					foreach (var taxJournal in taxJournals)
					{
						taxJournal.EntityId = accountType?.EntityId;
						taxJournal.EntityTypeId = accountType?.EntityTypeId;
					}
				}
			}
		}

		public async Task<ResponseDto> SaveJournal(JournalDto journal, bool hasApprove, bool approved, int? requestId)
		{
			TrimModelStrings(journal);
			EmptyList();
			var result = new ResponseDto();
			if (journal.JournalHeader != null)
			{
				var journalModel = BuildJournal(journal);
				if (journalModel.JournalHeader != null)
				{
					var validationResult = await ValidateJournal(journal);
					if (validationResult.Success)
					{
						result = await _journalHeaderService.SaveJournalHeader(journalModel.JournalHeader, hasApprove, approved, requestId);
						if (result.Success)
						{
							await DetermineJournalEntityTypes(journal.JournalDetails!);
							await HandleJournalDetails(journal, journalModel, result.Id);

							await DeleteFractionEntryIfExist(result.Id, journal.JournalHeader.StoreId);

							await _costCenterJournalDetailService.DeleteCostCenterJournalDetail(_costCenterJournalDetailDelete);
							await _journalDetailService.DeleteJournalDetail(_journalDetailDelete);

							await _journalDetailService.CreateJournalDetail(_journalDetailCreate);
							await _costCenterJournalDetailService.CreateCostCenterJournalDetail(_costCenterJournalDetailCreate);

							await _journalDetailService.UpdateJournalDetail(_journalDetailUpdate);
							await _costCenterJournalDetailService.UpdateCostCenterJournalDetail(_costCenterJournalDetailUpdate);

							await AddFractionalEntryIfExist(result.Id, journal.JournalHeader.StoreId);

							if (journal.MenuNotes != null)
							{
								await _menuNoteService.SaveMenuNotes(journal.MenuNotes, result.Id);
							}
						}
					}
					else
					{
						return validationResult;
					}
				}
			}
			return result;
		}

		public async Task<bool> DeleteFractionEntryIfExist(int journalHeaderId, int storeId)
		{
			var details = await _journalDetailService.GetAll().AsNoTracking().Where(x => x.JournalHeaderId == journalHeaderId).AsNoTracking().ToListAsync();
			var companyId = await _storeService.GetCompanyIdByStoreId(storeId);
			var fractionalAccount = await _accountService.GetFractionalApproximationDifferenceAccount(companyId);
			var journalDetail = details.FirstOrDefault(x => x.AccountId == fractionalAccount.AccountId);
			if (journalDetail != null)
			{
				_journalDetailDelete.Add(journalDetail);
				_journalDetailUpdate = _journalDetailUpdate.Where(x=>x.JournalDetailId != journalDetail.JournalDetailId).ToList();
			}
			return true;
		}

		public async Task<bool> AddFractionalEntryIfExist(int journalHeaderId, int storeId)
		{
			var details = await _journalDetailService.GetAll().AsNoTracking().Where(x => x.JournalHeaderId == journalHeaderId).AsNoTracking().ToListAsync();
			var companyId = await _storeService.GetCompanyIdByStoreId(storeId);
			var fractionalAccount = await _accountService.GetFractionalApproximationDifferenceAccount(companyId);

			var debit = details.Sum(x => x.DebitValue);
			var credit = details.Sum(x => x.CreditValue);
			var nextId = _journalDetailService.GetAll().AsNoTracking().Max(x => x.JournalDetailId) +1;
			var newEntry = new JournalDetail()
			{
				AccountId = fractionalAccount.AccountId,
				CurrencyId = fractionalAccount.CurrencyId,
				CurrencyRate = 1,
				DebitValue = debit > credit ? 0 : credit - debit,
				CreditValue = credit > debit ? 0 : debit - credit,
				DebitValueAccount = debit > credit ? 0 : credit - debit,
				CreditValueAccount = credit > debit ? 0 : debit - credit,
				IsTax = false,
				Serial = details.Max(x => x.Serial) + 1,
				RemarksAr = JournalEntryData.FractionalEntryRemarksAr,
				RemarksEn = JournalEntryData.FractionalEntryRemarksEn,
				JournalDetailId = nextId,
				JournalHeaderId = journalHeaderId,
			};
			if (((NumberHelper.RoundNumber(newEntry.DebitValue,4)) > 0) || ((NumberHelper.RoundNumber(newEntry.CreditValue,4)) > 0))
			{
				await _journalDetailService.Insert(newEntry);
			}
			return true;
		}

		//public async Task<bool> CheckJournal(int companyId)
		//{
		//	var fractionalAccount = await _accountService.GetFractionalApproximationDifferenceAccount(companyId);

		//	var allJournals = _journalDetailCreate.Union(_journalDetailUpdate).ToList();
		//	var journals = _journalDetailCreate.Union(_journalDetailUpdate).ToList().Where(x=>x.AccountId != fractionalAccount.AccountId).ToList();
		//	var debit = journals.Sum(x => x.DebitValue);
		//	var credit = journals.Sum(x => x.CreditValue);

		//	UpdateFractionalDifferenceEntry()
		//}

		//public async Task<bool> UpdateFractionalDifferenceEntry(List<JournalDetail> journals,int companyId,decimal debit,decimal credit,int journalHeaderId)
		//{
		//	var fractionalAccount = await _accountService.GetFractionalApproximationDifferenceAccount(companyId);
		//	var isExist = journals.Any(x => x.AccountId == fractionalAccount.AccountId);

		//	if (isExist)
		//	{
		//		var journal = journals.FirstOrDefault(x => x.AccountId == fractionalAccount.AccountId);
		//		if (journal != null)
		//		{
		//			if (debit > credit)
		//			{
		//				journal.DebitValue = 0;
		//				journal.CreditValue = (debit - credit);
		//			}
		//			else
		//			{
		//				journal.CreditValue = 0;
		//				journal.DebitValue = (credit - debit);
		//			}
		//		}
		//	}
		//	else
		//	{
		//		var newEntry = new JournalDetail()
		//		{
		//			AccountId = fractionalAccount.AccountId,
		//			CurrencyId = fractionalAccount.CurrencyId,
		//			CurrencyRate = 1,
		//			DebitValue = debit > credit ? 0 : credit - debit,
		//			CreditValue = credit > debit ? 0 : debit - credit,
		//			DebitValueAccount = debit > credit ? 0 : credit - debit,
		//			CreditValueAccount = credit > debit ? 0 : debit - credit,
		//			IsTax = false,
		//			Serial = journals.Max(x => x.Serial) + 1,
		//			RemarksAr = "",
		//			RemarksEn = "",
		//			JournalDetailId = journals.Max(x => x.JournalDetailId) + 1,
		//			JournalHeaderId = journalHeaderId,
		//		};
		//		_journalDetailCreate.Add(newEntry);
		//	}

		//	return true;
		//}

		public static void EmptyList()
		{
			_journalDetailCreate = new List<JournalDetail>();
			_journalDetailUpdate = new List<JournalDetail>();
			_journalDetailDelete = new List<JournalDetail>();
			_costCenterJournalDetailCreate = new List<CostCenterJournalDetail>();
			_costCenterJournalDetailUpdate = new List<CostCenterJournalDetail>();
			_costCenterJournalDetailDelete = new List<CostCenterJournalDetail>();
		}
		public async Task<bool> HandleJournalDetails(JournalDto journal, JournalVm journalBuilt, int journalHeaderId)
		{
			if (journal.JournalHeader != null)
			{
				await HandleDeletedCostCenterJournalDetails(journal.JournalHeader.JournalHeaderId, journal.CostCenterJournalDetails);
				await HandleDeletedJournalDetails(journal.JournalHeader.JournalHeaderId, journal.JournalDetails);

				var nextJournalDetail = await _journalDetailService.GetNextId();
				var serial = 1;
				if (journalBuilt.JournalDetails != null)
				{
					foreach (var detail in journalBuilt.JournalDetails)
					{
						var journalDetailId = await HandleJournalDetail(detail.JournalDetail, journalHeaderId, nextJournalDetail, serial, journal.JournalDetails, detail.CostCenterJournalDetails!.Any(),journal.JournalHeader?.RemarksAr,journal.JournalHeader?.RemarksEn);
						await HandleCostCenterJournalDetail(journalDetailId, detail.CostCenterJournalDetails);
						nextJournalDetail++;
						serial++;
					}
				}
				return true;
			}
			return false;
		}

		public static bool IsJournalIsBalanced(List<JournalDetailDto>? journalDetail)
		{
			if (journalDetail != null)
			{
				var debit = journalDetail.Sum(x => x.DebitValue);
				var credit = journalDetail.Sum(x => x.CreditValue);
				return Math.Abs(debit - credit) < 1;
			}
			return false;
		}

		public static JournalVm BuildJournal(JournalDto journal)
		{
			var detailList = new List<JournalDetailVm>();

			if (journal.JournalDetails != null)
			{
				foreach (var journalDetail in journal.JournalDetails)
				{
					var detail = new JournalDetailVm()
					{
						JournalDetail = journalDetail,
						CostCenterJournalDetails = journal.CostCenterJournalDetails != null ? journal.CostCenterJournalDetails.Where(x => x.JournalDetailId == journalDetail.JournalDetailId).ToList() : new List<CostCenterJournalDetailDto>()
					};
					detailList.Add(detail);
				}
			}
			var journalModel = new JournalVm()
			{
				JournalHeader = journal.JournalHeader,
				JournalDetails = detailList
			};
			return journalModel;
		}

		public async Task<int> HandleJournalDetail(JournalDetailDto? journalDetail, int headerId, int nextJournalDetail, int serial, List<JournalDetailDto>? journalDetails, bool hasCostCenter, string? remarksAr, string? remarksEn)
		{
			if (journalDetails != null)
			{
				if (journalDetail != null)
				{
					var taxParentId = GetRealTaxParentId(journalDetail.TaxParentId, journalDetails);
					var result = await _journalDetailService.BuildJournalDetail(headerId, nextJournalDetail, serial, taxParentId, journalDetail, hasCostCenter);

					var journalDetailId = result.JournalDetail!.JournalDetailId;
					if (result.Mode == RequestMode.Create)
					{
						_journalDetailCreate.Add(result.JournalDetail!);
					}
					else
					{
						_journalDetailUpdate.Add(result.JournalDetail!);
					}
					return journalDetailId;
				}
			}
			return 0;
		}

		public async Task<bool> HandleCostCenterJournalDetail(int journalDetailId, List<CostCenterJournalDetailDto>? costCenterJournalDetails)
		{
			if (costCenterJournalDetails != null)
			{
				if (costCenterJournalDetails.Any())
				{
					var nextCostCenterJournalDetailId = await GetNextCostCenterJournalDetailId();
					var costCenters = await _costCenterJournalDetailService.BuildCostCenterJournalDetail(journalDetailId, nextCostCenterJournalDetailId, costCenterJournalDetails);
					if (costCenters != null)
					{
						if (costCenters.JournalDetailCreate != null)
						{
							_costCenterJournalDetailCreate.AddRange(costCenters.JournalDetailCreate);
						}

						if (costCenters.JournalDetailUpdate != null)
						{
							_costCenterJournalDetailUpdate.AddRange(costCenters.JournalDetailUpdate);
						}
					}
				}
			}
			return true;
		}

		public async Task<bool> HandleDeletedJournalDetails(int journalHeaderId, List<JournalDetailDto>? journalDetail)
		{
			if (journalHeaderId > 0)
			{
				var data = await _journalDetailService.HandleDeletedJournalDetails(journalHeaderId, journalDetail);
				_journalDetailDelete.AddRange(data);
				return true;
			}
			return false;
		}
		public async Task<bool> HandleDeletedCostCenterJournalDetails(int journalHeaderId, List<CostCenterJournalDetailDto>? costCenterJournalDetail)
		{
			if (journalHeaderId > 0)
			{
				var data = await _costCenterJournalDetailService.HandleDeletedCostCenterJournalDetails(journalHeaderId, costCenterJournalDetail);
				_costCenterJournalDetailDelete.AddRange(data);
				return true;
			}
			return false;
		}

		public static int? GetRealTaxParentId(int? taxParentId, List<JournalDetailDto>? viewModel)
		{
			if (taxParentId > 0)
			{
				var originalItem = viewModel?.FirstOrDefault(x => x.JournalDetailId == taxParentId);
				return originalItem != null ? _journalDetailUpdate.Where(x => x.Serial == originalItem.Serial).Select(x => x.JournalDetailId).FirstOrDefault() : null;
			}
			else
			{
				var originalItem = viewModel?.FirstOrDefault(x => x.JournalDetailId == taxParentId);
				return originalItem != null ? _journalDetailCreate.Where(x => x.Serial == originalItem.Serial).Select(x => x.JournalDetailId).FirstOrDefault() : null;
			}
		}
		public async Task<int> GetNextCostCenterJournalDetailId()
		{
			var model = _costCenterJournalDetailCreate.Union(_costCenterJournalDetailUpdate).ToList();
			return model.Count > 0 ? model.Max(x => x.CostCenterJournalDetailId) + 1 : await _costCenterJournalDetailService.GetNextId();
		}

		public async Task<ResponseDto> DeleteJournal(int journalHeaderId)
		{
			await _menuNoteService.DeleteMenuNotes(MenuCodeData.JournalEntry, journalHeaderId);
			await _costCenterJournalDetailService.DeleteCostCenterJournalDetail(journalHeaderId);
			await _journalDetailService.DeleteJournalDetail(journalHeaderId);
			return await _journalHeaderService.DeleteJournalHeader(journalHeaderId);
		}
	}
}
