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
using Shared.CoreOne.Contracts.Settings;

namespace Purchases.Service.Services
{
    public class SupplierDebitMemoHandlingService: ISupplierDebitMemoHandlingService
    {
        private readonly ISupplierMemoEffectsService _supplierMemoEffectsService;
        private readonly IPurchaseValueService _purchaseValueService;
        private readonly IJournalService _journalService;
        private readonly IMenuNoteService _menuNoteService;
        private readonly IPurchaseInvoiceHeaderService _purchaseInvoiceHeaderService;
        private readonly IStockInHeaderService _stockInHeaderService;
        private readonly IStockInReturnHeaderService _stockInReturnHeaderService;
        private readonly IPurchaseInvoiceReturnHeaderService _purchaseInvoiceReturnHeaderService;
        private readonly ISupplierDebitMemoService _supplierDebitMemoService;
        private readonly ISupplierCreditMemoService _supplierCreditMemoService;
        private readonly IPurchaseOrderStatusService _purchaseOrderStatusService;
        private readonly IPurchaseOrderHeaderService _purchaseOrderHeaderService;
        private readonly IGenericMessageService _genericMessageService;
        private readonly IPurchaseMessageService _purchaseMessageService;
        private readonly IGetPurchaseInvoiceSettleValueService _getPurchaseInvoiceSettleValueService;
		private readonly IDocumentExceedValueSettingService _documentExceedValueSettingService;

        public SupplierDebitMemoHandlingService(ISupplierMemoEffectsService supplierMemoEffectsService, IPurchaseValueService purchaseValueService, IJournalService journalService, IMenuNoteService menuNoteService, IPurchaseInvoiceHeaderService purchaseInvoiceHeaderService, IStockInHeaderService stockInHeaderService, IStockInReturnHeaderService stockInReturnHeaderService, IPurchaseInvoiceReturnHeaderService purchaseInvoiceReturnHeaderService, ISupplierDebitMemoService supplierDebitMemoService, ISupplierCreditMemoService supplierCreditMemoService, IPurchaseOrderStatusService purchaseOrderStatuService, IPurchaseOrderHeaderService purchaseOrderHeaderService, IGenericMessageService genericMessageService, IPurchaseMessageService purchaseMessageService, IGetPurchaseInvoiceSettleValueService getPurchaseInvoiceSettleValueService, IDocumentExceedValueSettingService documentExceedValueSettingService)
        { 
            _supplierMemoEffectsService = supplierMemoEffectsService;
            _purchaseValueService = purchaseValueService;
            _journalService = journalService;
            _menuNoteService = menuNoteService;
            _purchaseInvoiceHeaderService = purchaseInvoiceHeaderService;
            _stockInHeaderService = stockInHeaderService;
            _stockInReturnHeaderService = stockInReturnHeaderService;
            _purchaseInvoiceReturnHeaderService = purchaseInvoiceReturnHeaderService;
            _supplierDebitMemoService = supplierDebitMemoService;
            _supplierCreditMemoService = supplierCreditMemoService;
            _purchaseOrderStatusService = purchaseOrderStatuService;
            _purchaseOrderHeaderService = purchaseOrderHeaderService;
            _genericMessageService = genericMessageService;
            _purchaseMessageService = purchaseMessageService;
            _getPurchaseInvoiceSettleValueService = getPurchaseInvoiceSettleValueService;
			_documentExceedValueSettingService = documentExceedValueSettingService;
        }

        public List<RequestChangesDto> GetSupplierDebitMemoRequestChanges(SupplierDebitMemoVm oldItem, SupplierDebitMemoVm newItem)
        {
            var requestChanges = new List<RequestChangesDto>();

            var supplierDebitMemoChanges = CompareLogic.GetDifferences(oldItem.SupplierDebitMemo, newItem.SupplierDebitMemo);
            requestChanges.AddRange(supplierDebitMemoChanges);

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

        public async Task<SupplierDebitMemoVm> GetSupplierDebitMemo(int supplierDebitMemoId)
        {
            var supplierDebitMemo = await _supplierDebitMemoService.GetSupplierDebitMemoById(supplierDebitMemoId);
            if(supplierDebitMemo == null)
            {
                return new SupplierDebitMemoVm();
            }

            var menuNotes = await _menuNoteService.GetMenuNotes(MenuCodeData.SupplierDebitMemo, supplierDebitMemoId).ToListAsync();
            var journal = await _journalService.GetJournal(supplierDebitMemo.JournalHeaderId);
            return new SupplierDebitMemoVm {SupplierDebitMemo = supplierDebitMemo, MenuNotes = menuNotes, Journal = journal};
        }

        public async Task<SupplierDebitMemoVm> GetSupplierDebitMemoFromPurchaseInvoice(int purchaseInvoiceHeaderId)
        {
            var purchaseInvoiceHeader = await _purchaseInvoiceHeaderService.GetPurchaseInvoiceHeaderById(purchaseInvoiceHeaderId);
            if(purchaseInvoiceHeader == null)
            {
                return new SupplierDebitMemoVm();
            }

            var overallInvoiceValue = await _getPurchaseInvoiceSettleValueService.GetPurchaseInvoiceValueMinusSettledValue(purchaseInvoiceHeaderId);

			var supplierDebitMemo = new SupplierDebitMemoDto
            {
                SupplierDebitMemoId = 0,
                DocumentDate = purchaseInvoiceHeader.DocumentDate,
                PurchaseInvoiceHeaderId = purchaseInvoiceHeaderId,
                SupplierId = purchaseInvoiceHeader.SupplierId,
                SupplierCode = purchaseInvoiceHeader.SupplierCode,
                SupplierName = purchaseInvoiceHeader.SupplierName,
                StoreId = purchaseInvoiceHeader.StoreId,
                StoreName = purchaseInvoiceHeader.StoreName,
                Reference = purchaseInvoiceHeader.Reference,
                DebitAccountId = purchaseInvoiceHeader.CreditAccountId,
                CreditAccountId = purchaseInvoiceHeader.DebitAccountId,
                MemoValue = overallInvoiceValue,
                JournalHeaderId = 0,
                RemarksAr = purchaseInvoiceHeader.RemarksAr,
                RemarksEn = purchaseInvoiceHeader.RemarksEn,
                IsClosed = false,
                ArchiveHeaderId = purchaseInvoiceHeader.ArchiveHeaderId
            };

            return new SupplierDebitMemoVm { SupplierDebitMemo = supplierDebitMemo};
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

        public async Task<ResponseDto> SaveSupplierDebitMemoInFull(SupplierDebitMemoVm supplierDebitMemo, bool hasApprove, bool approved, int? requestId)
        {
            var purchaseInvoiceMenuCode = await GetPurchaseInvoiceMenuCode(supplierDebitMemo.SupplierDebitMemo!.PurchaseInvoiceHeaderId);

			var validationResult = await CheckSupplierDebitMemoIsValidForSaving(supplierDebitMemo, purchaseInvoiceMenuCode);
            if(validationResult.Success == false)
            {
                validationResult.Id = supplierDebitMemo.SupplierDebitMemo!.SupplierDebitMemoId;
                return validationResult;
            }

            if(supplierDebitMemo.SupplierDebitMemo != null)
            {
                if (supplierDebitMemo.Journal != null)
                {
                    supplierDebitMemo.Journal.JournalHeader!.MenuCode = MenuCodeData.SupplierDebitMemo;
                    supplierDebitMemo.Journal.JournalHeader!.MenuReferenceId = await _supplierDebitMemoService.GetNextId();
                    var journalResult = await _journalService.SaveJournal(supplierDebitMemo.Journal, hasApprove, approved, requestId);

                    if (journalResult.Success)
                    {
                        supplierDebitMemo.SupplierDebitMemo.JournalHeaderId = journalResult.Id;
                    }
                    else
                    {
                        return journalResult;
                    }
                }

                var result = await _supplierDebitMemoService.SaveSupplierDebitMemo(supplierDebitMemo.SupplierDebitMemo, hasApprove, approved, requestId);
                if (result.Success)
                {
                    if (supplierDebitMemo.MenuNotes != null)
                    {
                        await _menuNoteService.SaveMenuNotes(supplierDebitMemo.MenuNotes, result.Id);
                    }

                    await MarkAllDocumentsLinkedToPurchaseInvoiceAsEnded(supplierDebitMemo.SupplierDebitMemo.PurchaseInvoiceHeaderId, supplierDebitMemo.SupplierDebitMemo.SupplierDebitMemoId == 0);
                    await UpdatePurchaseOrderStatusOnSave(supplierDebitMemo.SupplierDebitMemo);

					var settlementExccedResult = await _getPurchaseInvoiceSettleValueService.CheckSettlementExceedingAndUpdateSettlementCompletedFlag(supplierDebitMemo.SupplierDebitMemo.PurchaseInvoiceHeaderId, MenuCodeData.SupplierDebitMemo, purchaseInvoiceMenuCode, true);
					if (settlementExccedResult.Success == false) return settlementExccedResult;
				}
                return result;
            }
            return new ResponseDto{ Message = "Header should not be null" };
        }

        private async Task UpdatePurchaseOrderStatusOnSave(SupplierDebitMemoDto supplierDebitMemo)
        {
            if (supplierDebitMemo.SupplierDebitMemoId == 0)
            {
                var purchaseOrderHeaderId = await GetRelatedPurchaseOrder(supplierDebitMemo);
                await _purchaseOrderStatusService.UpdatePurchaseOrderStatus(purchaseOrderHeaderId, DocumentStatusData.SupplierDebitMemoCreated, MenuCodeData.SupplierDebitMemo);
            }
        }

        private async Task<int> GetRelatedPurchaseOrder(SupplierDebitMemoDto supplierDebitMemo)
        {
            return await (from purchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == supplierDebitMemo.PurchaseInvoiceHeaderId)
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


		private async Task<ResponseDto> CheckSupplierDebitMemoIsValidForSaving(SupplierDebitMemoVm supplierDebitMemo, int purchaseInvoiceMenuCode)
        {
            ResponseDto result;

            result = await CheckSupplierDebitMemoJournalMismatch(supplierDebitMemo.SupplierDebitMemo!, supplierDebitMemo.Journal?.JournalHeader);
            if (result.Success == false) return result;

            result = await CheckPurchaseInvoiceBlockedAndIsCreditPayment(supplierDebitMemo.SupplierDebitMemo!.PurchaseInvoiceHeaderId);
            if (result.Success == false) return result;

            result = await CheckSupplierDebitMemoExceedingInvoice(supplierDebitMemo.SupplierDebitMemo!, purchaseInvoiceMenuCode);
            if (result.Success == false) return result;

            return new ResponseDto { Success = true };
        }

        private async Task<ResponseDto> CheckSupplierDebitMemoJournalMismatch(SupplierDebitMemoDto supplierDebitMemo, JournalHeaderDto? journalHeader)
        {
			var exceedValueSetting = await _documentExceedValueSettingService.GetSettingByMenuCode(MenuCodeData.SupplierDebitMemo, supplierDebitMemo.StoreId);
			if (exceedValueSetting) return new ResponseDto { Success = true };

            var journalValue = journalHeader?.TotalDebitValue ?? 0;
            var memoValue = supplierDebitMemo.MemoValue;

            if(memoValue != journalValue)
            {
                return new ResponseDto { Success = false, Message = await _purchaseMessageService.GetMessage(MenuCodeData.SupplierDebitMemo, PurchaseMessageData.ValueNotMatchingJournalDebit) };
            }

            return new ResponseDto { Success = true };
        }

        private async Task<ResponseDto> CheckSupplierDebitMemoExceedingInvoice(SupplierDebitMemoDto supplierDebitMemo, int purchaseInvoiceMenuCode)
        {
            var currentInvoiceTotal = supplierDebitMemo.SupplierDebitMemoId == 0 ?
                await _purchaseValueService.GetOverallValueOfPurchaseInvoice(supplierDebitMemo.PurchaseInvoiceHeaderId) :
                await _purchaseValueService.GetOverallValueOfPurchaseInvoiceExceptSupplierDebit(supplierDebitMemo.PurchaseInvoiceHeaderId, supplierDebitMemo.SupplierDebitMemoId);

            if (currentInvoiceTotal - supplierDebitMemo.MemoValue < 0)
            {
				return new ResponseDto { Success = false, Message = await _purchaseMessageService.GetMessage(MenuCodeData.SupplierDebitMemo, purchaseInvoiceMenuCode, PurchaseMessageData.ValueExceeding, supplierDebitMemo.MemoValue.ToNormalizedString(), currentInvoiceTotal.ToNormalizedString()) };
			}

			return new ResponseDto { Success = true };
        }

        private async Task<ResponseDto> CheckPurchaseInvoiceBlockedAndIsCreditPayment(int purchaseInvoiceHeaderId)
        {
			var purchaseInvoiceHeader = await _purchaseInvoiceHeaderService.GetPurchaseInvoiceHeaderById(purchaseInvoiceHeaderId);

			if (purchaseInvoiceHeader!.IsBlocked)
			{
				return new ResponseDto { Success = false, Message = await _genericMessageService.GetMessage(MenuCodeData.SupplierDebitMemo, GenericMessageData.CannotPerformOperationBecauseStoppedDealingOn) };
			}

			if (!purchaseInvoiceHeader!.CreditPayment)
			{
				return new ResponseDto { Success = false, Message = await _purchaseMessageService.GetMessage(MenuCodeData.SupplierDebitMemo, PurchaseMessageData.CanOnlyCreateOnCreditInvoice) };
			}

			return new ResponseDto { Success = true };
		}

        public async Task<ResponseDto> CheckSupplierDebitMemoIsValidForDeleting(SupplierDebitMemoDto supplierDebitMemo)
        {
            ResponseDto result = await CheckPurchaseInvoiceBlockedAndIsCreditPayment(supplierDebitMemo.PurchaseInvoiceHeaderId);
            if (result.Success == false) return result;

            return new ResponseDto { Success = true };
        }

        public async Task<ResponseDto> DeleteSupplierDebitMemoInFull(int supplierDebitMemoId)
        {
            var supplierDebitMemo = await _supplierDebitMemoService.GetSupplierDebitMemoById(supplierDebitMemoId);
            if(supplierDebitMemo == null)
            {
                return new ResponseDto { Id = supplierDebitMemoId, Message = await _genericMessageService.GetMessage(MenuCodeData.SupplierDebitMemo, GenericMessageData.NotFound) };
            }

            ResponseDto validationResult = await CheckSupplierDebitMemoIsValidForDeleting(supplierDebitMemo);
            if (validationResult.Success == false)
            {
                return validationResult;
            }

            await _menuNoteService.DeleteMenuNotes(MenuCodeData.SupplierDebitMemo, supplierDebitMemoId);

            var supplierDebitMemoResult = await _supplierDebitMemoService.DeleteSupplierDebitMemo(supplierDebitMemoId);
            if(supplierDebitMemoResult.Success == false)
            {
                return supplierDebitMemoResult;
            }

            var journalResult = await _journalService.DeleteJournal(supplierDebitMemo.JournalHeaderId);
            if (journalResult.Success == false)
            {
                return journalResult;
            }

            await _supplierMemoEffectsService.ReopenAllDocumentsRelatedToPurchaseInvoice(supplierDebitMemo.PurchaseInvoiceHeaderId);
            await UpdatePurchaseOrderStatusOnDelete(supplierDebitMemo);

            var purchaseInvoiceMenuCode = await GetPurchaseInvoiceMenuCode(supplierDebitMemo.PurchaseInvoiceHeaderId);
			var settlementExccedResult = await _getPurchaseInvoiceSettleValueService.CheckSettlementExceedingAndUpdateSettlementCompletedFlag(supplierDebitMemo.PurchaseInvoiceHeaderId, MenuCodeData.SupplierDebitMemo, purchaseInvoiceMenuCode, false);
			if (settlementExccedResult.Success == false) return settlementExccedResult;

			return supplierDebitMemoResult;
        }

        private async Task UpdatePurchaseOrderStatusOnDelete(SupplierDebitMemoDto supplierDebitMemo)
        {
            var purchaseOrderHeaderId = await GetRelatedPurchaseOrder(supplierDebitMemo);
            await _purchaseOrderStatusService.UpdatePurchaseOrderStatus(purchaseOrderHeaderId, -1, MenuCodeData.SupplierDebitMemo);
        }
    }
}
