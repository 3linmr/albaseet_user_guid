using Microsoft.EntityFrameworkCore;
using Purchases.CoreOne.Models.Domain;
using Sales.CoreOne.Contracts;
using Sales.CoreOne.Models.Domain;
using Sales.CoreOne.Models.Dtos.ViewModels;
using Sales.CoreOne.Models.StaticData;
using Shared.CoreOne.Contracts.Journal;
using Shared.CoreOne.Contracts.Menus;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Domain.Menus;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.CoreOne.Models.Dtos.ViewModels.Journal;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Extensions;
using Shared.Helper.Models.Dtos;
using Shared.Service.Logic.Approval;
using System.Linq;
using Shared.CoreOne.Contracts.Settings;

namespace Sales.Service.Services
{
	public class ClientDebitMemoHandlingService : IClientDebitMemoHandlingService
	{
		private readonly IClientMemoEffectsService _clientMemoEffectsService;
		private readonly ISalesValueService _salesValueService;
		private readonly IJournalService _journalService;
		private readonly IMenuNoteService _menuNoteService;
		private readonly ISalesInvoiceHeaderService _salesInvoiceHeaderService;
		private readonly IStockOutHeaderService _stockOutHeaderService;
		private readonly IStockOutReturnHeaderService _stockOutReturnHeaderService;
		private readonly ISalesInvoiceReturnHeaderService _salesInvoiceReturnHeaderService;
		private readonly IClientDebitMemoService _clientDebitMemoService;
		private readonly IClientCreditMemoService _clientCreditMemoService;
		private readonly IProformaInvoiceStatusService _proformaInvoiceStatusService;
		private readonly IProformaInvoiceHeaderService _proformaInvoiceHeaderService;
		private readonly IGenericMessageService _genericMessageService;
		private readonly ISalesMessageService _salesMessageService;
		private readonly IGetSalesInvoiceSettleValueService _getSalesInvoiceSettleValueService;
		private readonly IDocumentExceedValueSettingService _documentExceedValueSettingService;

		public ClientDebitMemoHandlingService(IClientMemoEffectsService clientMemoEffectsService, ISalesValueService salesValueService, IJournalService journalService, IMenuNoteService menuNoteService, ISalesInvoiceHeaderService salesInvoiceHeaderService, IStockOutHeaderService stockOutHeaderService, IStockOutReturnHeaderService stockOutReturnHeaderService, ISalesInvoiceReturnHeaderService salesInvoiceReturnHeaderService, IClientDebitMemoService clientDebitMemoService, IClientCreditMemoService clientCreditMemoService, IProformaInvoiceStatusService proformaInvoiceStatuService, IProformaInvoiceHeaderService proformaInvoiceHeaderService, IGenericMessageService genericMessageService, ISalesMessageService salesMessageService, IGetSalesInvoiceSettleValueService getSalesInvoiceSettleValueService, IDocumentExceedValueSettingService documentExceedValueSettingService)
		{
			_clientMemoEffectsService = clientMemoEffectsService;
			_salesValueService = salesValueService;
			_journalService = journalService;
			_menuNoteService = menuNoteService;
			_salesInvoiceHeaderService = salesInvoiceHeaderService;
			_stockOutHeaderService = stockOutHeaderService;
			_stockOutReturnHeaderService = stockOutReturnHeaderService;
			_salesInvoiceReturnHeaderService = salesInvoiceReturnHeaderService;
			_clientDebitMemoService = clientDebitMemoService;
			_clientCreditMemoService = clientCreditMemoService;
			_proformaInvoiceStatusService = proformaInvoiceStatuService;
			_proformaInvoiceHeaderService = proformaInvoiceHeaderService;
			_genericMessageService = genericMessageService;
			_salesMessageService = salesMessageService;
			_getSalesInvoiceSettleValueService = getSalesInvoiceSettleValueService;
			_documentExceedValueSettingService = documentExceedValueSettingService;
		}

		public List<RequestChangesDto> GetClientDebitMemoRequestChanges(ClientDebitMemoVm oldItem, ClientDebitMemoVm newItem)
		{
			var requestChanges = new List<RequestChangesDto>();

			var clientDebitMemoChanges = CompareLogic.GetDifferences(oldItem.ClientDebitMemo, newItem.ClientDebitMemo);
			requestChanges.AddRange(clientDebitMemoChanges);

			if (oldItem.Journal != null && newItem.Journal != null)
			{
				var journalChanges = _journalService.GetJournalEntryRequestChanges(oldItem.Journal, newItem.Journal);
				requestChanges.AddRange(journalChanges);
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

		public async Task<ClientDebitMemoVm> GetClientDebitMemo(int clientDebitMemoId)
		{
			var clientDebitMemo = await _clientDebitMemoService.GetClientDebitMemoById(clientDebitMemoId);
			if (clientDebitMemo == null)
			{
				return new ClientDebitMemoVm();
			}

			var menuNotes = await _menuNoteService.GetMenuNotes(MenuCodeData.ClientDebitMemo, clientDebitMemoId).ToListAsync();
			var journal = await _journalService.GetJournal(clientDebitMemo.JournalHeaderId);
			return new ClientDebitMemoVm { ClientDebitMemo = clientDebitMemo, MenuNotes = menuNotes, Journal = journal };
		}

		public async Task<ClientDebitMemoVm> GetClientDebitMemoFromSalesInvoice(int salesInvoiceHeaderId)
		{
			var salesInvoiceHeader = await _salesInvoiceHeaderService.GetSalesInvoiceHeaderById(salesInvoiceHeaderId);
			if (salesInvoiceHeader == null)
			{
				return new ClientDebitMemoVm();
			}

			var returnedNetValue = await GetNetValueOfSalesInvoiceReturnAttachedToSalesInvoice(salesInvoiceHeaderId);

			var clientDebitMemo = new ClientDebitMemoDto
			{
				ClientDebitMemoId = 0,
				DocumentDate = salesInvoiceHeader.DocumentDate,
				SalesInvoiceHeaderId = salesInvoiceHeaderId,
				ClientId = salesInvoiceHeader.ClientId,
				ClientCode = salesInvoiceHeader.ClientCode,
				ClientName = salesInvoiceHeader.ClientName,
				SellerId = salesInvoiceHeader.SellerId,
				SellerCode = salesInvoiceHeader.SellerCode,
				SellerName = salesInvoiceHeader.SellerName,
				StoreId = salesInvoiceHeader.StoreId,
				StoreName = salesInvoiceHeader.StoreName,
				Reference = salesInvoiceHeader.Reference,
				DebitAccountId = salesInvoiceHeader.DebitAccountId ?? 0,
				CreditAccountId = salesInvoiceHeader.CreditAccountId,
				MemoValue = salesInvoiceHeader.NetValue - returnedNetValue,
				JournalHeaderId = 0,
				RemarksAr = salesInvoiceHeader.RemarksAr,
				RemarksEn = salesInvoiceHeader.RemarksEn,
				IsClosed = false,
				ArchiveHeaderId = salesInvoiceHeader.ArchiveHeaderId
			};

			return new ClientDebitMemoVm { ClientDebitMemo = clientDebitMemo };
		}

		private async Task<decimal> GetNetValueOfSalesInvoiceReturnAttachedToSalesInvoice(int salesInvoiceHeaderId)
		{
			return await _salesInvoiceReturnHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId).Select(x => x.NetValue).FirstOrDefaultAsync();
		}

		private async Task<int> GetSalesInvoiceMenuCode(int salesInvoiceHeaderId)
		{
			var salesInvoiceHeader = await _salesInvoiceHeaderService.GetSalesInvoiceHeaderById(salesInvoiceHeaderId);
			return SalesInvoiceMenuCodeHelper.GetMenuCode(salesInvoiceHeader!);
		}

		public async Task<ResponseDto> SaveClientDebitMemoInFull(ClientDebitMemoVm clientDebitMemo, bool hasApprove, bool approved, int? requestId)
		{
			var salesInvoiceMenuCode = await GetSalesInvoiceMenuCode(clientDebitMemo.ClientDebitMemo!.SalesInvoiceHeaderId);

			var validationResult = await CheckClientDebitMemoIsValidForSaving(clientDebitMemo, salesInvoiceMenuCode);
			if (validationResult.Success == false)
			{
				validationResult.Id = clientDebitMemo.ClientDebitMemo!.ClientDebitMemoId;
				return validationResult;
			}

			if (clientDebitMemo.ClientDebitMemo != null)
			{
				if (clientDebitMemo.Journal != null)
				{
					clientDebitMemo.Journal.JournalHeader!.MenuCode = MenuCodeData.ClientDebitMemo;
					clientDebitMemo.Journal.JournalHeader!.MenuReferenceId = await _clientDebitMemoService.GetNextId();
					var journalResult = await _journalService.SaveJournal(clientDebitMemo.Journal, hasApprove, approved, requestId);

					if (journalResult.Success)
					{
						clientDebitMemo.ClientDebitMemo.JournalHeaderId = journalResult.Id;
					}
					else
					{
						return journalResult;
					}
				}

				var result = await _clientDebitMemoService.SaveClientDebitMemo(clientDebitMemo.ClientDebitMemo, hasApprove, approved, requestId);
				if (result.Success)
				{
					if (clientDebitMemo.MenuNotes != null)
					{
						await _menuNoteService.SaveMenuNotes(clientDebitMemo.MenuNotes, result.Id);
					}

					await CloseSalesInvoiceAndRelatedDocuments(clientDebitMemo.ClientDebitMemo.SalesInvoiceHeaderId, clientDebitMemo.ClientDebitMemo.ClientDebitMemoId == 0);
					await UpdateProformaInvoiceStatusOnSave(clientDebitMemo.ClientDebitMemo);

					var settlementExccedResult = await _getSalesInvoiceSettleValueService.CheckSettlementExceedingAndUpdateSettlementCompletedFlag(clientDebitMemo.ClientDebitMemo.SalesInvoiceHeaderId, MenuCodeData.ClientDebitMemo, salesInvoiceMenuCode, true);
					if (settlementExccedResult.Success == false) return settlementExccedResult;
				}
				return result;
			}
			return new ResponseDto { Message = "Header should not be null" };
		}

		private async Task UpdateProformaInvoiceStatusOnSave(ClientDebitMemoDto clientDebitMemo)
		{
			if (clientDebitMemo.ClientDebitMemoId == 0)
			{
				var proformaInvoiceHeaderId = await GetRelatedProformaInvoice(clientDebitMemo);
				await _proformaInvoiceStatusService.UpdateProformaInvoiceStatus(proformaInvoiceHeaderId, DocumentStatusData.ClientDebitMemoCreated);
			}
		}

		private async Task<int> GetRelatedProformaInvoice(ClientDebitMemoDto clientDebitMemo)
		{
			return await (from salesInvoiceHeader in _salesInvoiceHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == clientDebitMemo.SalesInvoiceHeaderId)
						  from proformaInvoiceHeader in _proformaInvoiceHeaderService.GetAll().Where(x => x.ProformaInvoiceHeaderId == salesInvoiceHeader.ProformaInvoiceHeaderId)
						  select salesInvoiceHeader.ProformaInvoiceHeaderId).FirstOrDefaultAsync();
		}

		private async Task CloseSalesInvoiceAndRelatedDocuments(int salesInvoiceHeaderId, bool isCreate)
		{
			if (isCreate)
			{
				await _clientMemoEffectsService.MarkAllDocumentsLinkedToSalesInvoiceAsEnded(salesInvoiceHeaderId);
			}
		}

		private async Task ReopenSalesInvoiceAndRelatedDocuments(int salesInvoiceHeaderId)
		{
			await _clientMemoEffectsService.ReopenAllDocumentsRelatedToSalesInvoice(salesInvoiceHeaderId);
		}


		private async Task<ResponseDto> CheckClientDebitMemoIsValidForSaving(ClientDebitMemoVm clientDebitMemo, int salesInvoiceMenuCode)
		{
			ResponseDto result;

			result = await CheckClientDebitMemoJournalMismatch(clientDebitMemo.ClientDebitMemo!, clientDebitMemo.Journal?.JournalHeader);
			if (result.Success == false) return result;

			result = await CheckSalesInvoiceBlockedAndIsCreditPayment(clientDebitMemo.ClientDebitMemo!.SalesInvoiceHeaderId);
			if (result.Success == false) return result;

			result = await CheckClientDebitMemoMakeInvoiceNegative(clientDebitMemo.ClientDebitMemo!, salesInvoiceMenuCode);
			if (result.Success == false) return result;

			return new ResponseDto { Success = true };
		}

		private async Task<ResponseDto> CheckClientDebitMemoJournalMismatch(ClientDebitMemoDto clientDebitMemo, JournalHeaderDto? journalHeader)
		{
			var exceedValueSetting = await _documentExceedValueSettingService.GetSettingByMenuCode(MenuCodeData.ClientDebitMemo, clientDebitMemo.StoreId);
			if (exceedValueSetting) return new ResponseDto { Success = true };

			var journalValue = journalHeader?.TotalDebitValue ?? 0;
			var memoValue = clientDebitMemo.MemoValue;

			if (memoValue != journalValue)
			{
				return new ResponseDto { Success = false, Message = await _salesMessageService.GetMessage(MenuCodeData.ClientDebitMemo, SalesMessageData.ValueNotMatchingJournalDebit) };
			}

			return new ResponseDto { Success = true };
		}

		private async Task<ResponseDto> CheckClientDebitMemoMakeInvoiceNegative(ClientDebitMemoDto clientDebitMemo, int salesInvoiceMenuCode)
		{
			if (clientDebitMemo.ClientDebitMemoId != 0) // we only need to do this validation when decreasing the value of an existing client debit memo
			{
				var currentInvoiceTotal = await _salesValueService.GetOverallValueOfSalesInvoiceExceptClientDebit(clientDebitMemo.SalesInvoiceHeaderId, clientDebitMemo.ClientDebitMemoId);

				var minimumDebitValue = -currentInvoiceTotal;
				if (clientDebitMemo.MemoValue < minimumDebitValue)
				{
					return new ResponseDto { Success = false, Message = await _salesMessageService.GetMessage(MenuCodeData.ClientDebitMemo, salesInvoiceMenuCode, SalesMessageData.CannotDecreaseValueOfMemo) };
				}
			}

			return new ResponseDto { Success = true };
		}

		private async Task<ResponseDto> CheckSalesInvoiceBlockedAndIsCreditPayment(int salesInvoiceHeaderId)
		{
			var salesInvoiceHeader = await _salesInvoiceHeaderService.GetSalesInvoiceHeaderById(salesInvoiceHeaderId);

			if (salesInvoiceHeader!.IsBlocked)
			{
				return new ResponseDto { Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.ClientDebitMemo, GenericMessageData.CannotPerformOperationBecauseStoppedDealingOn) };
			}

			if (!salesInvoiceHeader!.CreditPayment)
			{
				return new ResponseDto { Success = false, Message = await _salesMessageService.GetMessage(MenuCodeData.ClientDebitMemo, SalesMessageData.CanOnlyCreateOnCreditInvoice) };
			}

			return new ResponseDto { Success = true };
		}

		public async Task<ResponseDto> CheckClientDebitMemoIsValidForDeleting(ClientDebitMemoDto clientDebitMemo, int salesInvoiceMenuCode)
		{
			ResponseDto result = await CheckSalesInvoiceBlockedAndIsCreditPayment(clientDebitMemo.SalesInvoiceHeaderId);
			if (result.Success == false) return result;

			result = await CheckNegativeSalesInvoice(clientDebitMemo.SalesInvoiceHeaderId, clientDebitMemo.ClientDebitMemoId, salesInvoiceMenuCode);
			if (result.Success == false) return result;

			return new ResponseDto { Success = true };
		}

		private async Task<ResponseDto> CheckNegativeSalesInvoice(int salesInvoiceHeaderId, int clientDebitMemoId, int salesInvoiceMenuCode)
		{
			var invoiceValue = await _salesValueService.GetOverallValueOfSalesInvoiceExceptClientDebit(salesInvoiceHeaderId, clientDebitMemoId);
			if (invoiceValue < 0)
			{
				return new ResponseDto { Success = false, Message = await _salesMessageService.GetMessage(MenuCodeData.ClientDebitMemo, salesInvoiceMenuCode, SalesMessageData.DeleteCauseInvoiceValueNegative) };
			}
			return new ResponseDto { Success = true };
		}

		public async Task<ResponseDto> DeleteClientDebitMemoInFull(int clientDebitMemoId)
		{
			var clientDebitMemo = await _clientDebitMemoService.GetClientDebitMemoById(clientDebitMemoId);
			if (clientDebitMemo == null)
			{
				return new ResponseDto { Id = clientDebitMemoId, Message = await _genericMessageService.GetMessage(MenuCodeData.ClientDebitMemo, GenericMessageData.NotFound) };
			}

			var salesInvoiceMenuCode = await GetSalesInvoiceMenuCode(clientDebitMemo.SalesInvoiceHeaderId);
			ResponseDto validationResult = await CheckClientDebitMemoIsValidForDeleting(clientDebitMemo, salesInvoiceMenuCode);
			if (validationResult.Success == false)
			{
				return validationResult;
			}

			await _menuNoteService.DeleteMenuNotes(MenuCodeData.ClientDebitMemo, clientDebitMemoId);

			var clientDebitMemoResult = await _clientDebitMemoService.DeleteClientDebitMemo(clientDebitMemoId);
			if (clientDebitMemoResult.Success == false)
			{
				return clientDebitMemoResult;
			}

			var journalResult = await _journalService.DeleteJournal(clientDebitMemo.JournalHeaderId);
			if (journalResult.Success == false)
			{
				return journalResult;
			}

			await ReopenSalesInvoiceAndRelatedDocuments(clientDebitMemo.SalesInvoiceHeaderId);
			await UpdateProformaInvoiceStatusOnDelete(clientDebitMemo);

			var settlementExccedResult = await _getSalesInvoiceSettleValueService.CheckSettlementExceedingAndUpdateSettlementCompletedFlag(clientDebitMemo.SalesInvoiceHeaderId, MenuCodeData.ClientDebitMemo, salesInvoiceMenuCode, false);
			if (settlementExccedResult.Success == false) return settlementExccedResult;

			return clientDebitMemoResult;
		}

		private async Task UpdateProformaInvoiceStatusOnDelete(ClientDebitMemoDto clientDebitMemo)
		{
			var proformaInvoiceHeaderId = await GetRelatedProformaInvoice(clientDebitMemo);
			await _proformaInvoiceStatusService.UpdateProformaInvoiceStatus(proformaInvoiceHeaderId, -1);
		}
	}
}
