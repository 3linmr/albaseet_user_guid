using Shared.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Purchases.CoreOne.Models.Domain;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Contracts.Journal;
using Microsoft.AspNetCore.Http;
using Shared.Helper.Identity;
using Purchases.CoreOne.Models.Dtos.ViewModels;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Shared.Helper.Models.Dtos;
using Purchases.CoreOne.Contracts;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne.Models.StaticData;
using Shared.CoreOne.Contracts.Menus;
using Microsoft.Extensions.Localization;
using Purchases.Service.Validators;
using FluentValidation;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.Service.Logic.Approval;
using Shared.Helper.Extensions;
using Shared.CoreOne.Models.Dtos.ViewModels.Journal;
using Purchases.CoreOne.Models.StaticData;
using Sales.CoreOne.Contracts;
using Sales.CoreOne.Models.Domain;
using Shared.CoreOne.Contracts.Settings;

namespace Purchases.Service.Services
{
    public class SupplierCreditMemoHandlingService: ISupplierCreditMemoHandlingService
    {
        private readonly IPurchaseValueService _purchaseValueService;
        private readonly ISupplierMemoEffectsService _supplierMemoEffectsService;
        private readonly IJournalService _journalService;
        private readonly IMenuNoteService _menuNoteService;
        private readonly IPurchaseInvoiceHeaderService _purchaseInvoiceHeaderService;
        private readonly IStockInHeaderService _stockInHeaderService;
        private readonly IStockInReturnHeaderService _stockInReturnHeaderService;
        private readonly IPurchaseInvoiceReturnHeaderService _purchaseInvoiceReturnHeaderService;
        private readonly ISupplierCreditMemoService _supplierCreditMemoService;
        private readonly ISupplierDebitMemoService _supplierDebitMemoService;
        private readonly IPurchaseOrderStatusService _purchaseOrderStatusService;
        private readonly IPurchaseOrderHeaderService _purchaseOrderHeaderService;
        private readonly IGenericMessageService _genericMessageService;
        private readonly IPurchaseMessageService _purchaseMessageService;
        private readonly IGetPurchaseInvoiceSettleValueService _getPurchaseInvoiceSettleValueService;
		private readonly IDocumentExceedValueSettingService _documentExceedValueSettingService;

        public SupplierCreditMemoHandlingService(IPurchaseValueService purchaseValueService, ISupplierMemoEffectsService supplierMemoEffectsService, IJournalService journalService, IMenuNoteService menuNoteService, IPurchaseInvoiceHeaderService purchaseInvoiceHeaderService, IStockInHeaderService stockInHeaderService, IStockInReturnHeaderService stockInReturnHeaderService, IPurchaseInvoiceReturnHeaderService purchaseInvoiceReturnHeaderService, ISupplierCreditMemoService supplierCreditMemoService, ISupplierDebitMemoService supplierDebitMemoService, IPurchaseOrderStatusService purchaseOrderStatusService, IPurchaseOrderHeaderService purchaseOrderHeaderService, IGenericMessageService genericMessageService, IPurchaseMessageService purchaseMessageService, IGetPurchaseInvoiceSettleValueService getPurchaseInvoiceSettleValueService, IDocumentExceedValueSettingService documentExceedValueSettingService)
        {
            _purchaseValueService = purchaseValueService;
            _supplierMemoEffectsService = supplierMemoEffectsService;
            _journalService = journalService;
            _menuNoteService = menuNoteService;
            _purchaseInvoiceHeaderService = purchaseInvoiceHeaderService;
            _stockInHeaderService = stockInHeaderService;
            _stockInReturnHeaderService = stockInReturnHeaderService;
            _purchaseInvoiceReturnHeaderService = purchaseInvoiceReturnHeaderService;
            _supplierCreditMemoService = supplierCreditMemoService;
            _supplierDebitMemoService = supplierDebitMemoService;
            _purchaseOrderStatusService = purchaseOrderStatusService;
            _purchaseOrderHeaderService = purchaseOrderHeaderService;
            _genericMessageService = genericMessageService;
            _purchaseMessageService = purchaseMessageService;
            _getPurchaseInvoiceSettleValueService = getPurchaseInvoiceSettleValueService;
			_documentExceedValueSettingService = documentExceedValueSettingService;
        }

        public List<RequestChangesDto> GetSupplierCreditMemoRequestChanges(SupplierCreditMemoVm oldItem, SupplierCreditMemoVm newItem)
        {
            var requestChanges = new List<RequestChangesDto>();

            var supplierCreditMemoChanges = CompareLogic.GetDifferences(oldItem.SupplierCreditMemo, newItem.SupplierCreditMemo);
            requestChanges.AddRange(supplierCreditMemoChanges);

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

        public async Task<SupplierCreditMemoVm> GetSupplierCreditMemo(int supplierCreditMemoId)
        {
            var supplierCreditMemo = await _supplierCreditMemoService.GetSupplierCreditMemoById(supplierCreditMemoId);
            if(supplierCreditMemo == null)
            {
                return new SupplierCreditMemoVm();
            }

            var menuNotes = await _menuNoteService.GetMenuNotes(MenuCodeData.SupplierCreditMemo, supplierCreditMemoId).ToListAsync();
            var journal = await _journalService.GetJournal(supplierCreditMemo.JournalHeaderId);
            return new SupplierCreditMemoVm {SupplierCreditMemo = supplierCreditMemo, MenuNotes = menuNotes, Journal = journal};
        }

        public async Task<SupplierCreditMemoVm> GetSupplierCreditMemoFromPurchaseInvoice(int purchaseInvoiceHeaderId)
        {
            var purchaseInvoiceHeader = await _purchaseInvoiceHeaderService.GetPurchaseInvoiceHeaderById(purchaseInvoiceHeaderId);
            if(purchaseInvoiceHeader == null)
            {
                return new SupplierCreditMemoVm();
            }

            var returnedNetValue = await GetNetValueOfPurchaseInvoiceReturnAttachedToPurchaseInvoice(purchaseInvoiceHeaderId);

            var supplierCreditMemo = new SupplierCreditMemoDto
            {
                SupplierCreditMemoId = 0,
                DocumentDate = purchaseInvoiceHeader.DocumentDate,
                PurchaseInvoiceHeaderId = purchaseInvoiceHeaderId,
                SupplierId = purchaseInvoiceHeader.SupplierId,
                SupplierCode = purchaseInvoiceHeader.SupplierCode,
                SupplierName = purchaseInvoiceHeader.SupplierName,
                StoreId = purchaseInvoiceHeader.StoreId,
                StoreName = purchaseInvoiceHeader.StoreName,
                Reference = purchaseInvoiceHeader.Reference,
                CreditAccountId = purchaseInvoiceHeader.CreditAccountId,
                DebitAccountId = purchaseInvoiceHeader.DebitAccountId,
                MemoValue = purchaseInvoiceHeader.NetValue - returnedNetValue,
                JournalHeaderId = 0,
                RemarksAr = purchaseInvoiceHeader.RemarksAr,
                RemarksEn = purchaseInvoiceHeader.RemarksEn,
                IsClosed = false,
                ArchiveHeaderId = purchaseInvoiceHeader.ArchiveHeaderId
            };

            return new SupplierCreditMemoVm { SupplierCreditMemo = supplierCreditMemo};
        }

        private async Task<decimal> GetNetValueOfPurchaseInvoiceReturnAttachedToPurchaseInvoice(int purchaseInvoiceHeaderId)
        {
            return await _purchaseInvoiceReturnHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId).Select(x => x.NetValue).FirstOrDefaultAsync();
        }

		private async Task<int> GetPurchaseInvoiceMenuCode(int purchaseInvoiceHeaderId)
		{
			var purchaseInvoiceHeader = await _purchaseInvoiceHeaderService.GetPurchaseInvoiceHeaderById(purchaseInvoiceHeaderId);
			return PurchaseInvoiceMenuCodeHelper.GetMenuCode(purchaseInvoiceHeader!);
		}

		public async Task<ResponseDto> SaveSupplierCreditMemoInFull(SupplierCreditMemoVm supplierCreditMemo, bool hasApprove, bool approved, int? requestId)
		{
			var purchaseInvoiceMenuCode = await GetPurchaseInvoiceMenuCode(supplierCreditMemo.SupplierCreditMemo!.PurchaseInvoiceHeaderId);

			var validationResult = await CheckSupplierCreditMemoIsValidForSaving(supplierCreditMemo, purchaseInvoiceMenuCode);
            if(validationResult.Success == false)
            {
                validationResult.Id = supplierCreditMemo.SupplierCreditMemo!.SupplierCreditMemoId;
                return validationResult;
            }

            if(supplierCreditMemo.SupplierCreditMemo != null)
            {
                if (supplierCreditMemo.Journal != null)
                {
                    supplierCreditMemo.Journal.JournalHeader!.MenuCode = MenuCodeData.SupplierCreditMemo;
                    supplierCreditMemo.Journal.JournalHeader!.MenuReferenceId = await _supplierCreditMemoService.GetNextId();
                    var journalResult = await _journalService.SaveJournal(supplierCreditMemo.Journal, hasApprove, approved, requestId);

                    if (journalResult.Success)
                    {
                        supplierCreditMemo.SupplierCreditMemo.JournalHeaderId = journalResult.Id;
                    }
                    else
                    {
                        return journalResult;
                    }
                }

                var result = await _supplierCreditMemoService.SaveSupplierCreditMemo(supplierCreditMemo.SupplierCreditMemo, hasApprove, approved, requestId);
                if (result.Success)
                {
                    if (supplierCreditMemo.MenuNotes != null)
                    {
                        await _menuNoteService.SaveMenuNotes(supplierCreditMemo.MenuNotes, result.Id);
                    }

					await MarkAllDocumentsLinkedToPurchaseInvoiceAsEnded(supplierCreditMemo.SupplierCreditMemo.PurchaseInvoiceHeaderId, supplierCreditMemo.SupplierCreditMemo.SupplierCreditMemoId == 0);
					await UpdatePurchaseOrderStatusOnSave(supplierCreditMemo.SupplierCreditMemo);

					var settlementExccedResult = await _getPurchaseInvoiceSettleValueService.CheckSettlementExceedingAndUpdateSettlementCompletedFlag(supplierCreditMemo.SupplierCreditMemo.PurchaseInvoiceHeaderId, MenuCodeData.SupplierCreditMemo, purchaseInvoiceMenuCode, true);
					if (settlementExccedResult.Success == false) return settlementExccedResult;
				}
				return result;
			}
            return new ResponseDto{ Message = "Header should not be null" };
        }

        private async Task UpdatePurchaseOrderStatusOnSave(SupplierCreditMemoDto supplierCreditMemo)
        {
            if (supplierCreditMemo.SupplierCreditMemoId == 0)
            {
                var purchaseOrderHeaderId = await GetRelatedPurchaseOrder(supplierCreditMemo);
                await _purchaseOrderStatusService.UpdatePurchaseOrderStatus(purchaseOrderHeaderId, DocumentStatusData.SupplierCreditMemoCreated, MenuCodeData.SupplierCreditMemo);
            }
        }

        private async Task<int> GetRelatedPurchaseOrder(SupplierCreditMemoDto supplierCreditMemo)
        {
            return await (from purchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == supplierCreditMemo.PurchaseInvoiceHeaderId)
                          from purchaseOrderHeader in _purchaseOrderHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseInvoiceHeader.PurchaseOrderHeaderId)
                          select purchaseInvoiceHeader.PurchaseOrderHeaderId).FirstOrDefaultAsync();
        }

		private async Task MarkAllDocumentsLinkedToPurchaseInvoiceAsEnded(int purchaseInvoiceHeaderId, bool isCreate)
		{
			if (isCreate)
			{
				await _supplierMemoEffectsService.MarkAllDocumentsLinkedToPurchaseInvoiceAsEnded(purchaseInvoiceHeaderId);
			}
		}

		private async Task<ResponseDto> CheckSupplierCreditMemoIsValidForSaving(SupplierCreditMemoVm supplierCreditMemo, int purchaseInvoiceMenuCode)
        {
            ResponseDto result;

            result = await CheckSupplierCreditMemoJournalMismatch(supplierCreditMemo.SupplierCreditMemo!, supplierCreditMemo.Journal?.JournalHeader);
            if (result.Success == false) return result;

            result = await CheckPurchaseInvoiceBlockedAndIsCreditPayment(supplierCreditMemo.SupplierCreditMemo!.PurchaseInvoiceHeaderId);
            if (result.Success == false) return result;

            result = await CheckSupplierCreditMemoMakeInvoiceNegative(supplierCreditMemo.SupplierCreditMemo!, purchaseInvoiceMenuCode);
            if (result.Success == false) return result;

            return new ResponseDto { Success = true };
        }

        private async Task<ResponseDto> CheckSupplierCreditMemoJournalMismatch(SupplierCreditMemoDto supplierCreditMemo, JournalHeaderDto? journalHeader)
        {
			var exceedValueSetting = await _documentExceedValueSettingService.GetSettingByMenuCode(MenuCodeData.SupplierCreditMemo, supplierCreditMemo.StoreId);
			if (exceedValueSetting) return new ResponseDto { Success = true };

            var journalValue = journalHeader?.TotalCreditValue ?? 0;
            var memoValue = supplierCreditMemo.MemoValue;

            if(memoValue != journalValue)
            {
                return new ResponseDto { Success = false, Message = await _purchaseMessageService.GetMessage(MenuCodeData.SupplierCreditMemo, PurchaseMessageData.ValueNotMatchingJournalCredit) };
            }

            return new ResponseDto { Success = true };
        }

        private async Task<ResponseDto> CheckSupplierCreditMemoMakeInvoiceNegative(SupplierCreditMemoDto supplierCreditMemo, int purchaseInvoiceMenuCode)
        {
            if (supplierCreditMemo.SupplierCreditMemoId != 0) // we only need to do this validation when decreasing the value of an existing supplier credit memo
            {
                var currentInvoiceTotal = await _purchaseValueService.GetOverallValueOfPurchaseInvoiceExceptSupplierCredit(supplierCreditMemo.PurchaseInvoiceHeaderId, supplierCreditMemo.SupplierCreditMemoId);

                var minimumCreditValue = -currentInvoiceTotal;
                if (supplierCreditMemo.MemoValue < minimumCreditValue)
                {
                    return new ResponseDto { Success = false, Message = await _purchaseMessageService.GetMessage(MenuCodeData.SupplierCreditMemo, purchaseInvoiceMenuCode, PurchaseMessageData.CannotDecreaseValueOfMemo) };
                }
            }

            return new ResponseDto { Success = true };
        }

        private async Task<ResponseDto> CheckPurchaseInvoiceBlockedAndIsCreditPayment(int purchaseInvoiceHeaderId)
        {
            var purchaseInvoiceHeader = await _purchaseInvoiceHeaderService.GetPurchaseInvoiceHeaderById(purchaseInvoiceHeaderId);

            if (purchaseInvoiceHeader!.IsBlocked)
            {
                return new ResponseDto { Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.SupplierCreditMemo, GenericMessageData.CannotPerformOperationBecauseStoppedDealingOn) };
            }

			if (!purchaseInvoiceHeader!.CreditPayment)
			{
				return new ResponseDto { Success = false, Message = await _purchaseMessageService.GetMessage(MenuCodeData.SupplierCreditMemo, PurchaseMessageData.CanOnlyCreateOnCreditInvoice) };
			}

			return new ResponseDto { Success = true };
        }

        public async Task<ResponseDto> CheckSupplierCreditMemoIsValidForDeleting(SupplierCreditMemoDto supplierCreditMemo, int purchaseInvoiceMenuCode)
        {
            ResponseDto result = await CheckPurchaseInvoiceBlockedAndIsCreditPayment(supplierCreditMemo.PurchaseInvoiceHeaderId);
            if (result.Success == false) return result;

			result = await CheckNegativePurchaseInvoice(supplierCreditMemo.PurchaseInvoiceHeaderId, supplierCreditMemo.SupplierCreditMemoId, purchaseInvoiceMenuCode);
			if (result.Success == false) return result;

			return new ResponseDto { Success = true };
        }

        private async Task<ResponseDto> CheckNegativePurchaseInvoice(int purchaseInvoiceHeaderId, int supplierCreditMemoId, int purchaseInvoiceMenuCode)
        {
            var invoiceValue = await _purchaseValueService.GetOverallValueOfPurchaseInvoiceExceptSupplierCredit(purchaseInvoiceHeaderId, supplierCreditMemoId);
            if (invoiceValue < 0) 
            { 
                return new ResponseDto { Success = false, Message = await _purchaseMessageService.GetMessage(MenuCodeData.SupplierCreditMemo, purchaseInvoiceMenuCode, PurchaseMessageData.DeleteCauseInvoiceValueNegative) };
            }
            return new ResponseDto { Success = true };
        }

        public async Task<ResponseDto> DeleteSupplierCreditMemoInFull(int supplierCreditMemoId)
        {
            var supplierCreditMemo = await _supplierCreditMemoService.GetSupplierCreditMemoById(supplierCreditMemoId);
            if(supplierCreditMemo == null)
            {
                return new ResponseDto { Id = supplierCreditMemoId, Message = await _genericMessageService.GetMessage(MenuCodeData.SupplierCreditMemo, GenericMessageData.NotFound) };
            }

			var purchaseInvoiceMenuCode = await GetPurchaseInvoiceMenuCode(supplierCreditMemo.PurchaseInvoiceHeaderId);
			ResponseDto validationResult = await CheckSupplierCreditMemoIsValidForDeleting(supplierCreditMemo, purchaseInvoiceMenuCode);
            if (validationResult.Success == false)
            {
                return validationResult;
            }

            await _menuNoteService.DeleteMenuNotes(MenuCodeData.SupplierCreditMemo, supplierCreditMemoId);

            var supplierCreditMemoResult = await _supplierCreditMemoService.DeleteSupplierCreditMemo(supplierCreditMemoId);
            if(supplierCreditMemoResult.Success == false)
            {
                return supplierCreditMemoResult;
            }

            var journalResult = await _journalService.DeleteJournal(supplierCreditMemo.JournalHeaderId);
            if (journalResult.Success == false)
            {
                return journalResult;
            }

			await _supplierMemoEffectsService.ReopenAllDocumentsRelatedToPurchaseInvoice(supplierCreditMemo.PurchaseInvoiceHeaderId);
			await UpdatePurchaseOrderStatusOnDelete(supplierCreditMemo);

			var settlementExccedResult = await _getPurchaseInvoiceSettleValueService.CheckSettlementExceedingAndUpdateSettlementCompletedFlag(supplierCreditMemo.PurchaseInvoiceHeaderId, MenuCodeData.SupplierCreditMemo, purchaseInvoiceMenuCode, false);
			if (settlementExccedResult.Success == false) return settlementExccedResult;

			return supplierCreditMemoResult;
        }

        private async Task UpdatePurchaseOrderStatusOnDelete(SupplierCreditMemoDto supplierCreditMemo)
        {
            var purchaseOrderHeaderId = await GetRelatedPurchaseOrder(supplierCreditMemo);
            await _purchaseOrderStatusService.UpdatePurchaseOrderStatus(purchaseOrderHeaderId, -1, MenuCodeData.SupplierCreditMemo);
        }
    }
}
