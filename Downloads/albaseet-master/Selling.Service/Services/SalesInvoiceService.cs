using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sales.CoreOne.Contracts;
using Sales.CoreOne.Models.Dtos.ViewModels;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.Service.Logic.Approval;
using Shared.CoreOne.Models.StaticData;
using Shared.CoreOne.Contracts.Modules;
using Shared.Helper.Models.Dtos;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Contracts.Journal;
using Purchases.CoreOne.Models.Dtos.ViewModels;
using Purchases.CoreOne.Models.Domain;
using Shared.CoreOne.Contracts.Menus;
using System.Linq.Expressions;
using Sales.CoreOne.Models.Domain;
using Sales.CoreOne.Models.StaticData;
using Shared.CoreOne.Contracts.Inventory;
using Shared.CoreOne.Contracts.Accounts;

namespace Sales.Service.Services
{
    public class SalesInvoiceService: ISalesInvoiceService
    {
        private readonly ISalesInvoiceDetailService _salesInvoiceDetailService;
        private readonly ISalesInvoiceDetailTaxService _salesInvoiceDetailTaxService;
        private readonly ISalesInvoiceHeaderService _salesInvoiceHeaderService;
        private readonly IMenuNoteService _menuNoteService;
        private readonly IGenericMessageService _genericMessageService;
        private readonly ISalesMessageService _salesMessageService;
        private readonly IJournalService _journalService;
        private readonly ISalesInvoiceCollectionService _salesInvoiceCollectionService;
        private readonly IPaymentMethodService _paymentMethodService;
        private readonly IItemCurrentBalanceService _itemCurrentBalanceService;
        private readonly ICostCenterJournalDetailService _costCenterJournalDetailService;
		private readonly IClientBalanceService _clientBalanceService;
		private readonly IClientService _clientService;

        public SalesInvoiceService(ISalesInvoiceDetailService salesInvoiceDetailService, ISalesInvoiceDetailTaxService salesInvoiceDetailTaxService, ISalesInvoiceHeaderService salesInvoiceHeaderService, IMenuNoteService menuNoteService, IGenericMessageService genericMessageService, ISalesMessageService salesMessageService, IJournalService journalService, ISalesInvoiceCollectionService salesInvoiceCollectionService, IPaymentMethodService paymentMethodService, IItemCurrentBalanceService itemCurrentBalanceService, ICostCenterJournalDetailService costCenterJournalDetailService, IClientBalanceService clientBalanceService, IClientService clientService)
        {
            _salesInvoiceDetailService = salesInvoiceDetailService;
            _salesInvoiceDetailTaxService = salesInvoiceDetailTaxService;
            _salesInvoiceHeaderService = salesInvoiceHeaderService;
            _menuNoteService = menuNoteService;
            _salesMessageService = salesMessageService;
            _genericMessageService = genericMessageService;
            _journalService = journalService;
            _salesInvoiceCollectionService = salesInvoiceCollectionService;
            _paymentMethodService = paymentMethodService;
            _itemCurrentBalanceService = itemCurrentBalanceService;
            _costCenterJournalDetailService = costCenterJournalDetailService;
			_clientBalanceService = clientBalanceService;
			_clientService = clientService;
        }

        public List<RequestChangesDto> GetSalesInvoiceRequestChanges(SalesInvoiceDto oldItem, SalesInvoiceDto newItem)
        {
            var requestChanges = new List<RequestChangesDto>();
            var items = CompareLogic.GetDifferences(oldItem.SalesInvoiceHeader, newItem.SalesInvoiceHeader);
            requestChanges.AddRange(items);

            if (oldItem.SalesInvoiceDetails.Any() && newItem.SalesInvoiceDetails.Any())
            {
                var oldCount = oldItem.SalesInvoiceDetails.Count;
                var newCount = newItem.SalesInvoiceDetails.Count;
                var index = 0;
                if (oldCount <= newCount)
                {
                    for (int i = index; i < oldCount; i++)
                    {
                        for (int j = index; j < newCount; j++)
                        {
                            var changes = CompareLogic.GetDifferences(oldItem.SalesInvoiceDetails[i], newItem.SalesInvoiceDetails[i]);
                            requestChanges.AddRange(changes);

                            var detailTaxChanges = GetSalesInvoiceDetailTaxesRequestChanges(oldItem.SalesInvoiceDetails[i], newItem.SalesInvoiceDetails[i]);
                            requestChanges.AddRange(detailTaxChanges);
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

			if (oldItem.SalesInvoiceCollections != null && oldItem.SalesInvoiceCollections.Any() && newItem.SalesInvoiceCollections != null && newItem.SalesInvoiceCollections.Any())
			{
				var oldCount = oldItem.SalesInvoiceCollections.Count;
				var newCount = newItem.SalesInvoiceCollections.Count;
				var index = 0;
				if (oldCount <= newCount)
				{
					for (int i = index; i < oldCount; i++)
					{
						for (int j = index; j < newCount; j++)
						{
							var changes = CompareLogic.GetDifferences(oldItem.SalesInvoiceCollections[i], newItem.SalesInvoiceCollections[i]);
							requestChanges.AddRange(changes);
							index++;
							break;
						}
					}
				}
			}

			if (oldItem.Journal != null && newItem.Journal != null)
            {
                var journalChanges = _journalService.GetJournalEntryRequestChanges(oldItem.Journal, newItem.Journal);
                requestChanges.AddRange(journalChanges);
            }

            requestChanges.RemoveAll(x => x.ColumnName == "SalesInvoiceDetailTaxes" || x.ColumnName == "ItemTaxData");

            return requestChanges;
        }

        private static List<RequestChangesDto> GetSalesInvoiceDetailTaxesRequestChanges(SalesInvoiceDetailDto oldItem, SalesInvoiceDetailDto newItem)
        {
            var requestChanges = new List<RequestChangesDto>();

            if (oldItem.SalesInvoiceDetailTaxes.Any() && newItem.SalesInvoiceDetailTaxes.Any())
            {
                var oldCount = oldItem.SalesInvoiceDetailTaxes.Count;
                var newCount = newItem.SalesInvoiceDetailTaxes.Count;
                var index = 0;
                if (oldCount <= newCount)
                {
                    for (int i = index; i < oldCount; i++)
                    {
                        for (int j = index; j < newCount; j++)
                        {
                            var changes = CompareLogic.GetDifferences(oldItem.SalesInvoiceDetailTaxes[i], newItem.SalesInvoiceDetailTaxes[i]);
                            requestChanges.AddRange(changes);

                            index++;
                            break;
                        }
                    }
                }
            }

            return requestChanges;
        }

		public async Task ModifySalesInvoiceDetailsWithStoreIdAndAvaialbleBalance(int salesInvoiceHeaderId, int storeId, List<SalesInvoiceDetailDto> details, bool isRequestData, bool isCreate)
		{
			var itemIds = details.Select(x => x.ItemId).ToList();

			var availableBalances = await _itemCurrentBalanceService.GetItemCurrentBalanceInfo(storeId, false, false).Where(x => x.StoreId == storeId && itemIds.Contains(x.ItemId)).ToListAsync();
            var currentSalesInvoiceDetails = isCreate ? [] : (isRequestData ? await _salesInvoiceDetailService.GetSalesInvoiceDetailsAsQueryable(salesInvoiceHeaderId).ToListAsync() : details);

            var filteredAvailableBalances = availableBalances.Select(x => new { Key = (x.ItemId, x.ItemPackageId, x.ExpireDate, x.BatchNumber), Quantity = x.AvailableBalance });
            var filteredCurrentDetails = currentSalesInvoiceDetails.Select(x => new { Key = (x.ItemId, x.ItemPackageId, x.ExpireDate, x.BatchNumber), Quantity = x.Quantity + x.BonusQuantity });

            var finalAvailableBalances = filteredAvailableBalances.Concat(filteredCurrentDetails).GroupBy(x => x.Key).ToDictionary(x => x.Key, x => x.Sum(y => y.Quantity));

			foreach (var detail in details)
			{
				detail.StoreId = storeId;
				detail.AvailableBalance = finalAvailableBalances.GetValueOrDefault((detail.ItemId, detail.ItemPackageId, detail.ExpireDate, detail.BatchNumber), 0);
			}
		}

        public async Task ModifySalesInvoiceCreditLimits(SalesInvoiceHeaderDto salesInvoiceHeader)
        {
            var client = await _clientService.GetClientById(salesInvoiceHeader.ClientId);
            var clientBalance = await _clientBalanceService.GetClientBalanceByAccountId(client!.AccountId);

			salesInvoiceHeader.CreditLimitValues = client!.CreditLimitValues;
            salesInvoiceHeader.CreditLimitDays = client!.CreditLimitDays;
            salesInvoiceHeader.ClientBalance = clientBalance;
            salesInvoiceHeader.DebitLimitDays = client!.DebitLimitDays;
        }

		public async Task<SalesInvoiceDto> GetSalesInvoice(int salesInvoiceHeaderId)
        {
            var header = await _salesInvoiceHeaderService.GetSalesInvoiceHeaderById(salesInvoiceHeaderId);
            if (header == null) return new SalesInvoiceDto();

            var details = await _salesInvoiceDetailService.GetSalesInvoiceDetails(salesInvoiceHeaderId);
            var menuNotes = await _menuNoteService.GetMenuNotes(SalesInvoiceMenuCodeHelper.GetMenuCode(header), salesInvoiceHeaderId).ToListAsync();
            var salesInvoiceDetailTaxes = await _salesInvoiceDetailTaxService.GetSalesInvoiceDetailTaxes(salesInvoiceHeaderId).ToListAsync();
            var journal = await _journalService.GetJournal(header.JournalHeaderId);
            var collections = header.CreditPayment ?
                new List<SalesInvoiceCollectionDto>() :
                await _salesInvoiceCollectionService.GetSalesInvoiceCollections(salesInvoiceHeaderId, header.StoreId);

            await ModifySalesInvoiceDetailsWithStoreIdAndAvaialbleBalance(header.SalesInvoiceHeaderId, header.StoreId, details, false, false);
            await ModifySalesInvoiceCreditLimits(header);
			foreach (var detail in details)
            {
                detail.SalesInvoiceDetailTaxes = salesInvoiceDetailTaxes.Where(x => x.SalesInvoiceDetailId == detail.SalesInvoiceDetailId).ToList();
            }

            return new SalesInvoiceDto() { SalesInvoiceHeader = header, SalesInvoiceDetails = details, SalesInvoiceCollections = collections, Journal = journal, MenuNotes = menuNotes};
        }

        public async Task<List<SalesInvoiceCollectionDto>> AddNonincludedPaymentMethods(List<SalesInvoiceCollectionDto> invoiceCollections, int storeId)
        {
            var includedPaymentMethods = invoiceCollections.Select(x => x.PaymentMethodId).ToList();

            var toAdd = (await _paymentMethodService.GetVoucherPaymentMethods(storeId, false, false)).Where(x => !includedPaymentMethods.Contains(x.PaymentMethodId)).Select(x => new SalesInvoiceCollectionDto
            {
                SalesInvoiceCollectionId = 0,
                SalesInvoiceHeaderId = 0,
                PaymentMethodId = x.PaymentMethodId,
                PaymentMethodName = x.PaymentMethodName,
                AccountId = x.AccountId,
                CurrencyId = x.CurrencyId,
                CurrencyName = x.CurrencyName,
                CurrencyRate = x.CurrencyRate,
                CollectedValue = 0,
                CollectedValueAccount = 0,
                RemarksAr = null,
                RemarksEn = null
            });

            invoiceCollections.AddRange(toAdd);

            var newId = -1;
            invoiceCollections.ForEach(x => x.SalesInvoiceCollectionId = x.SalesInvoiceCollectionId <= 0 ? newId-- : x.SalesInvoiceCollectionId);

            return invoiceCollections;
        }

        public async Task<ResponseDto> SaveSalesInvoice(SalesInvoiceDto salesInvoice, bool hasApprove, bool approved, int? requestId, string? documentReference = null)
        {
            if (salesInvoice.SalesInvoiceHeader != null)
            {
                //the menuCode is not used in these messages
                if (!salesInvoice.SalesInvoiceDetails.Any()) return new ResponseDto { Message = await _genericMessageService.GetMessage(MenuCodeData.SalesInvoiceInterim, GenericMessageData.DetailIsEmpty)};

                var menuCode = (short)SalesInvoiceMenuCodeHelper.GetMenuCode(salesInvoice.SalesInvoiceHeader);
                if (salesInvoice.Journal != null)
                {
                    salesInvoice.Journal.JournalHeader!.MenuCode = menuCode;
                    salesInvoice.Journal.JournalHeader!.MenuReferenceId = await _salesInvoiceHeaderService.GetNextId();
                    var journalResult = await _journalService.SaveJournal(salesInvoice.Journal, hasApprove, approved, requestId);

                    if (journalResult.Success)
                    {
                        salesInvoice.SalesInvoiceHeader.JournalHeaderId = journalResult.Id;
                    }
                    else
                    {
                        return journalResult;
                    }
                }

                var result = await _salesInvoiceHeaderService.SaveSalesInvoiceHeader(salesInvoice.SalesInvoiceHeader, hasApprove, approved, requestId, documentReference);
                if (result.Success)
                {
                    var modifiedSalesInvoiceDetails = await _salesInvoiceDetailService.SaveSalesInvoiceDetails(result.Id, salesInvoice.SalesInvoiceDetails);
                    await SaveSalesInvoiceDetailTaxes(result.Id, modifiedSalesInvoiceDetails);
					await UpdateCostCenterJournalDetails(result.Id, modifiedSalesInvoiceDetails, menuCode);
                    await _salesInvoiceDetailService.DeleteSalesInvoiceDetailList(modifiedSalesInvoiceDetails, result.Id);
                    await _salesInvoiceCollectionService.SaveSalesInvoiceCollections(result.Id, salesInvoice.SalesInvoiceCollections);

                    if (salesInvoice.MenuNotes != null)
                    {
                        await _menuNoteService.SaveMenuNotes(salesInvoice.MenuNotes, result.Id);
                    }
                }

                return result;
            }
            return new ResponseDto { Message = "Header should not be null" };
        }

        private async Task SaveSalesInvoiceDetailTaxes(int salesInvoiceHeaderId, List<SalesInvoiceDetailDto> salesInvoiceDetails)
        {
            List<SalesInvoiceDetailTaxDto> salesInvoiceDetailTaxes = new List<SalesInvoiceDetailTaxDto>();

            foreach (var salesInvoiceDetail in salesInvoiceDetails)
            {
                foreach (var salesInvoiceDetailTax in salesInvoiceDetail.SalesInvoiceDetailTaxes)
                {
                    salesInvoiceDetailTax.SalesInvoiceDetailId = salesInvoiceDetail.SalesInvoiceDetailId;
                    salesInvoiceDetailTaxes.Add(salesInvoiceDetailTax);
                }
            }

            await _salesInvoiceDetailTaxService.SaveSalesInvoiceDetailTaxes(salesInvoiceHeaderId, salesInvoiceDetailTaxes);
        }

		private async Task UpdateCostCenterJournalDetails(int salesInvoiceHeaderId, List<SalesInvoiceDetailDto> salesInvoiceDetails, short menuCode)
		{
			await _costCenterJournalDetailService.UpdateCostCenterJournalDetailsBasedOnInvoiceDetails(
				invoiceHeaderId: salesInvoiceHeaderId, 
				invoiceDetails: salesInvoiceDetails,
				detailIdSelector: x => x.SalesInvoiceDetailId,
				itemIdSelector: x => x.ItemId,
				costCenterIdSelector: x => x.CostCenterId,
				creditValueSelector: x => x.NetValue,
				debitValueSelector: x => 0,
				remarksSelector: x => x.Notes,
				menuCode: menuCode
			);
		}

        public async Task<ResponseDto> DeleteSalesInvoice(int salesInvoiceHeaderId, int menuCode)
        {
            var salesInvoiceHeader = await _salesInvoiceHeaderService.GetSalesInvoiceHeaderById(salesInvoiceHeaderId);

			await _costCenterJournalDetailService.DeleteCostCenterJournalDetails(salesInvoiceHeaderId, (short)menuCode);
            await _menuNoteService.DeleteMenuNotes(menuCode, salesInvoiceHeaderId);
            await _salesInvoiceDetailTaxService.DeleteSalesInvoiceDetailTaxes(salesInvoiceHeaderId);
            await _salesInvoiceDetailService.DeleteSalesInvoiceDetails(salesInvoiceHeaderId);
            await _salesInvoiceCollectionService.DeleteSalesInvoiceCollections(salesInvoiceHeaderId);

            var salesInvoiceResult = await _salesInvoiceHeaderService.DeleteSalesInvoiceHeader(salesInvoiceHeaderId, menuCode);
            if( salesInvoiceResult.Success == false)
            {
                return salesInvoiceResult;
            }

            var journalResult = await _journalService.DeleteJournal(salesInvoiceHeader!.JournalHeaderId);
            if (journalResult.Success == false)
            {
                return journalResult;
            }

            return salesInvoiceResult;
        }

        public async Task<decimal> GetLastSalesPrice(int itemId, int itemPackageId)
        {
            return await _salesInvoiceDetailService.GetAll().Where(x => x.ItemId == itemId && x.ItemPackageId == itemPackageId).OrderByDescending(x => x.SalesInvoiceDetailId).Select(x => x.SellingPrice).FirstOrDefaultAsync();
        }

        public async Task<List<LastSalesPriceDto>> GetMultipleLastSalesPrices(List<int> itemIds)
        {
            var invoices = await _salesInvoiceDetailService.GetAll().GroupBy(x => new { x.ItemId, x.ItemPackageId }).Select(x => new LastSalesPriceDto
            {
                ItemId = x.Key.ItemId,
                ItemPackageId = x.Key.ItemPackageId,
                SellingPrice = x.OrderByDescending(y => y.SalesInvoiceHeaderId).Select(x => x.SellingPrice).FirstOrDefault()
            }).Where(x => itemIds.Contains(x.ItemId)).ToListAsync();

            return invoices;
        }
    }
}
