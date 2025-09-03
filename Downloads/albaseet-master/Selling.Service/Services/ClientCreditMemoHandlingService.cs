using Microsoft.EntityFrameworkCore;
using Sales.CoreOne.Contracts;
using Sales.CoreOne.Models.Domain;
using Sales.CoreOne.Models.Dtos.ViewModels;
using Sales.CoreOne.Models.StaticData;
using Shared.CoreOne.Contracts.Journal;
using Shared.CoreOne.Contracts.Menus;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.CoreOne.Models.Dtos.ViewModels.Journal;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Extensions;
using Shared.Helper.Models.Dtos;
using Shared.Service.Logic.Approval;
using Shared.CoreOne.Contracts.Settings;

namespace Sales.Service.Services
{
    public class ClientCreditMemoHandlingService: IClientCreditMemoHandlingService
    {
        private readonly ISalesValueService _salesValueService;
        private readonly IClientMemoEffectsService _clientMemoEffectsService;
        private readonly IJournalService _journalService;
        private readonly IMenuNoteService _menuNoteService;
        private readonly ISalesInvoiceHeaderService _salesInvoiceHeaderService;
        private readonly IStockOutHeaderService _stockOutHeaderService;
        private readonly IStockOutReturnHeaderService _stockOutReturnHeaderService;
        private readonly ISalesInvoiceReturnHeaderService _salesInvoiceReturnHeaderService;
        private readonly IClientCreditMemoService _clientCreditMemoService;
        private readonly IClientDebitMemoService _clientDebitMemoService;
        private readonly IProformaInvoiceStatusService _proformaInvoiceStatusService;
        private readonly IProformaInvoiceHeaderService _proformaInvoiceHeaderService;
        private readonly IGenericMessageService _genericMessageService;
        private readonly ISalesMessageService _salesMessageService;
        private readonly IGetSalesInvoiceSettleValueService _getSalesInvoiceSettleValueService;
		private readonly IDocumentExceedValueSettingService _documentExceedValueSettingService;

        public ClientCreditMemoHandlingService(ISalesValueService salesValueService, IClientMemoEffectsService clientMemoEffectsService, IJournalService journalService, IMenuNoteService menuNoteService, ISalesInvoiceHeaderService salesInvoiceHeaderService, IStockOutHeaderService stockOutHeaderService, IStockOutReturnHeaderService stockOutReturnHeaderService, ISalesInvoiceReturnHeaderService salesInvoiceReturnHeaderService, IClientCreditMemoService clientCreditMemoService, IClientDebitMemoService clientDebitMemoService, IProformaInvoiceStatusService proformaInvoiceStatusService, IProformaInvoiceHeaderService proformaInvoiceHeaderService, IGenericMessageService genericMessageService, ISalesMessageService salesMessageService, IGetSalesInvoiceSettleValueService getSalesInvoiceSettleValueService, IDocumentExceedValueSettingService documentExceedValueSettingService)
        {
            _salesValueService = salesValueService;
            _clientMemoEffectsService = clientMemoEffectsService;
            _journalService = journalService;
            _menuNoteService = menuNoteService;
            _salesInvoiceHeaderService = salesInvoiceHeaderService;
            _stockOutHeaderService = stockOutHeaderService;
            _stockOutReturnHeaderService = stockOutReturnHeaderService;
            _salesInvoiceReturnHeaderService = salesInvoiceReturnHeaderService;
            _clientCreditMemoService = clientCreditMemoService;
            _clientDebitMemoService = clientDebitMemoService;
            _proformaInvoiceStatusService = proformaInvoiceStatusService;
            _proformaInvoiceHeaderService = proformaInvoiceHeaderService;
            _genericMessageService = genericMessageService;
            _salesMessageService = salesMessageService;
            _getSalesInvoiceSettleValueService = getSalesInvoiceSettleValueService;
			_documentExceedValueSettingService = documentExceedValueSettingService;
        }

        public List<RequestChangesDto> GetClientCreditMemoRequestChanges(ClientCreditMemoVm oldItem, ClientCreditMemoVm newItem)
        {
            var requestChanges = new List<RequestChangesDto>();

            var clientCreditMemoChanges = CompareLogic.GetDifferences(oldItem.ClientCreditMemo, newItem.ClientCreditMemo);
            requestChanges.AddRange(clientCreditMemoChanges);

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

        public async Task<ClientCreditMemoVm> GetClientCreditMemo(int clientCreditMemoId)
        {
            var clientCreditMemo = await _clientCreditMemoService.GetClientCreditMemoById(clientCreditMemoId);
            if(clientCreditMemo == null)
            {
                return new ClientCreditMemoVm();
            }

            var menuNotes = await _menuNoteService.GetMenuNotes(MenuCodeData.ClientCreditMemo, clientCreditMemoId).ToListAsync();
            var journal = await _journalService.GetJournal(clientCreditMemo.JournalHeaderId);
            return new ClientCreditMemoVm {ClientCreditMemo = clientCreditMemo, MenuNotes = menuNotes, Journal = journal};
        }

        public async Task<ClientCreditMemoVm> GetClientCreditMemoFromSalesInvoice(int salesInvoiceHeaderId)
        {
			var salesInvoiceHeader = await _salesInvoiceHeaderService.GetSalesInvoiceHeaderById(salesInvoiceHeaderId);
			if (salesInvoiceHeader == null)
			{
				return new ClientCreditMemoVm();
			}

			var overallInvoiceValue = await _getSalesInvoiceSettleValueService.GetSalesInvoiceValueMinusSettledValue(salesInvoiceHeaderId);

			var clientCreditMemo = new ClientCreditMemoDto
			{
				ClientCreditMemoId = 0,
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
				CreditAccountId = salesInvoiceHeader.DebitAccountId ?? 0,
				DebitAccountId = salesInvoiceHeader.CreditAccountId,
				MemoValue = overallInvoiceValue,
				JournalHeaderId = 0,
				RemarksAr = salesInvoiceHeader.RemarksAr,
				RemarksEn = salesInvoiceHeader.RemarksEn,
				IsClosed = false,
				ArchiveHeaderId = salesInvoiceHeader.ArchiveHeaderId
			};

			return new ClientCreditMemoVm { ClientCreditMemo = clientCreditMemo };
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

		public async Task<ResponseDto> SaveClientCreditMemoInFull(ClientCreditMemoVm clientCreditMemo, bool hasApprove, bool approved, int? requestId)
        {
			var salesInvoiceMenuCode = await GetSalesInvoiceMenuCode(clientCreditMemo.ClientCreditMemo!.SalesInvoiceHeaderId);

			var validationResult = await CheckClientCreditMemoIsValidForSaving(clientCreditMemo, salesInvoiceMenuCode);
            if(validationResult.Success == false)
            {
                validationResult.Id = clientCreditMemo.ClientCreditMemo!.ClientCreditMemoId;
                return validationResult;
            }

            if(clientCreditMemo.ClientCreditMemo != null)
            {
                if (clientCreditMemo.Journal != null)
                {
                    clientCreditMemo.Journal.JournalHeader!.MenuCode = MenuCodeData.ClientCreditMemo;
                    clientCreditMemo.Journal.JournalHeader!.MenuReferenceId = await _clientCreditMemoService.GetNextId();
                    var journalResult = await _journalService.SaveJournal(clientCreditMemo.Journal, hasApprove, approved, requestId);

                    if (journalResult.Success)
                    {
                        clientCreditMemo.ClientCreditMemo.JournalHeaderId = journalResult.Id;
                    }
                    else
                    {
                        return journalResult;
                    }
                }

                var result = await _clientCreditMemoService.SaveClientCreditMemo(clientCreditMemo.ClientCreditMemo, hasApprove, approved, requestId);
                if (result.Success)
                {
                    if (clientCreditMemo.MenuNotes != null)
                    {
                        await _menuNoteService.SaveMenuNotes(clientCreditMemo.MenuNotes, result.Id);
                    }

					await CloseSalesInvoiceAndRelatedDocuments(clientCreditMemo.ClientCreditMemo.SalesInvoiceHeaderId, clientCreditMemo.ClientCreditMemo.ClientCreditMemoId == 0);
					await UpdateProformaInvoiceStatusOnSave(clientCreditMemo.ClientCreditMemo);

					var settlementExccedResult = await _getSalesInvoiceSettleValueService.CheckSettlementExceedingAndUpdateSettlementCompletedFlag(clientCreditMemo.ClientCreditMemo.SalesInvoiceHeaderId, MenuCodeData.ClientCreditMemo, salesInvoiceMenuCode, true);
					if (settlementExccedResult.Success == false) return settlementExccedResult;
				}
                return result;
            }
            return new ResponseDto{ Message = "Header should not be null" };
        }

        private async Task UpdateProformaInvoiceStatusOnSave(ClientCreditMemoDto clientCreditMemo)
        {
            if (clientCreditMemo.ClientCreditMemoId == 0)
            {
                var proformaInvoiceHeaderId = await GetRelatedProformaInvoice(clientCreditMemo);
                await _proformaInvoiceStatusService.UpdateProformaInvoiceStatus(proformaInvoiceHeaderId, DocumentStatusData.ClientCreditMemoCreated);
            }
        }

        private async Task<int> GetRelatedProformaInvoice(ClientCreditMemoDto clientCreditMemo)
        {
            return await (from salesInvoiceHeader in _salesInvoiceHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == clientCreditMemo.SalesInvoiceHeaderId)
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

		private async Task<ResponseDto> CheckClientCreditMemoIsValidForSaving(ClientCreditMemoVm clientCreditMemo, int salesInvoiceMenuCode)
        {
            ResponseDto result;

            result = await CheckClientCreditMemoJournalMismatch(clientCreditMemo.ClientCreditMemo!, clientCreditMemo.Journal?.JournalHeader);
            if (result.Success == false) return result;

            result = await CheckSalesInvoiceBlockedAndIsCreditPayment(clientCreditMemo.ClientCreditMemo!.SalesInvoiceHeaderId);
            if (result.Success == false) return result;

            result = await CheckClientCreditMemoExceedingInvoice(clientCreditMemo.ClientCreditMemo!, salesInvoiceMenuCode);
            if (result.Success == false) return result;

            return new ResponseDto { Success = true };
        }

        private async Task<ResponseDto> CheckClientCreditMemoJournalMismatch(ClientCreditMemoDto clientCreditMemo, JournalHeaderDto? journalHeader)
        {
			var exceedValueSetting = await _documentExceedValueSettingService.GetSettingByMenuCode(MenuCodeData.ClientCreditMemo, clientCreditMemo.StoreId);
			if (exceedValueSetting) return new ResponseDto { Success = true };

            var journalValue = journalHeader?.TotalCreditValue ?? 0;
            var memoValue = clientCreditMemo.MemoValue;

            if(memoValue != journalValue)
            {
                return new ResponseDto { Success = false, Message = await _salesMessageService.GetMessage(MenuCodeData.ClientCreditMemo, SalesMessageData.ValueNotMatchingJournalCredit) };
            }

            return new ResponseDto { Success = true };
        }

        private async Task<ResponseDto> CheckClientCreditMemoExceedingInvoice(ClientCreditMemoDto clientCreditMemo, int salesInvoiceMenuCode)
        {
            var currentInvoiceTotal = clientCreditMemo.ClientCreditMemoId == 0 ?
                await _salesValueService.GetOverallValueOfSalesInvoice(clientCreditMemo.SalesInvoiceHeaderId) :
                await _salesValueService.GetOverallValueOfSalesInvoiceExceptClientCredit(clientCreditMemo.SalesInvoiceHeaderId, clientCreditMemo.ClientCreditMemoId);

            if (currentInvoiceTotal - clientCreditMemo.MemoValue < 0)
            {
                return new ResponseDto { Success = false, Message = await _salesMessageService.GetMessage(MenuCodeData.ClientCreditMemo, salesInvoiceMenuCode, SalesMessageData.ValueExceeding, clientCreditMemo.MemoValue.ToNormalizedString(), currentInvoiceTotal.ToNormalizedString()) };
            }

            return new ResponseDto { Success = true };
        }

		private async Task<ResponseDto> CheckSalesInvoiceBlockedAndIsCreditPayment(int salesInvoiceHeaderId)
		{
			var salesInvoiceHeader = await _salesInvoiceHeaderService.GetSalesInvoiceHeaderById(salesInvoiceHeaderId);

			if (salesInvoiceHeader!.IsBlocked)
			{
				return new ResponseDto { Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.ClientCreditMemo, GenericMessageData.CannotPerformOperationBecauseStoppedDealingOn) };
			}

			if (!salesInvoiceHeader!.CreditPayment)
			{
				return new ResponseDto { Success = false, Message = await _salesMessageService.GetMessage(MenuCodeData.ClientCreditMemo, SalesMessageData.CanOnlyCreateOnCreditInvoice) };
			}

			return new ResponseDto { Success = true };
		}

		public async Task<ResponseDto> CheckClientCreditMemoIsValidForDeleting(ClientCreditMemoDto clientCreditMemo)
        {
            ResponseDto result = await CheckSalesInvoiceBlockedAndIsCreditPayment(clientCreditMemo.SalesInvoiceHeaderId);
            if (result.Success == false) return result;

            return new ResponseDto { Success = true };
        }

        public async Task<ResponseDto> DeleteClientCreditMemoInFull(int clientCreditMemoId)
        {
            var clientCreditMemo = await _clientCreditMemoService.GetClientCreditMemoById(clientCreditMemoId);
            if(clientCreditMemo == null)
            {
                return new ResponseDto { Id = clientCreditMemoId, Message = await _genericMessageService.GetMessage(MenuCodeData.ClientCreditMemo, GenericMessageData.NotFound) };
            }

            ResponseDto validationResult = await CheckClientCreditMemoIsValidForDeleting(clientCreditMemo);
            if (validationResult.Success == false)
            {
                return validationResult;
            }

            await _menuNoteService.DeleteMenuNotes(MenuCodeData.ClientCreditMemo, clientCreditMemoId);

            var clientCreditMemoResult = await _clientCreditMemoService.DeleteClientCreditMemo(clientCreditMemoId);
            if(clientCreditMemoResult.Success == false)
            {
                return clientCreditMemoResult;
            }

            var journalResult = await _journalService.DeleteJournal(clientCreditMemo.JournalHeaderId);
            if (journalResult.Success == false)
            {
                return journalResult;
            }

			await ReopenSalesInvoiceAndRelatedDocuments(clientCreditMemo.SalesInvoiceHeaderId);
			await UpdateProformaInvoiceStatusOnDelete(clientCreditMemo);

            var salesInvoiceMenuCode = await GetSalesInvoiceMenuCode(clientCreditMemo.SalesInvoiceHeaderId);
			var settlementExccedResult = await _getSalesInvoiceSettleValueService.CheckSettlementExceedingAndUpdateSettlementCompletedFlag(clientCreditMemo.SalesInvoiceHeaderId, MenuCodeData.ClientCreditMemo, salesInvoiceMenuCode, false);
			if (settlementExccedResult.Success == false) return settlementExccedResult;

			return clientCreditMemoResult;
        }

        private async Task UpdateProformaInvoiceStatusOnDelete(ClientCreditMemoDto clientCreditMemo)
        {
            var proformaInvoiceHeaderId = await GetRelatedProformaInvoice(clientCreditMemo);
            await _proformaInvoiceStatusService.UpdateProformaInvoiceStatus(proformaInvoiceHeaderId, -1);
        }
    }
}
