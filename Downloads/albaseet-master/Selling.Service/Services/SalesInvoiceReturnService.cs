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
using Sales.CoreOne.Models.Domain;
using Shared.Service.Services.Modules;
using Shared.CoreOne.Contracts.Menus;
using Sales.CoreOne.Models.StaticData;
using Shared.CoreOne.Contracts.Items;
using Shared.Helper.Extensions;

namespace Sales.Service.Services
{
    public class SalesInvoiceReturnService: ISalesInvoiceReturnService
    {
        private readonly ISalesInvoiceReturnPaymentService _salesInvoiceReturnPaymentService;
        private readonly ISalesInvoiceReturnDetailService _salesInvoiceReturnDetailService;
        private readonly ISalesInvoiceReturnDetailTaxService _salesInvoiceReturnDetailTaxService;
        private readonly ISalesInvoiceReturnHeaderService _salesInvoiceReturnHeaderService;
        private readonly IMenuNoteService _menuNoteService;
        private readonly IGenericMessageService _genericMessageService;
        private readonly IJournalService _journalService;
        private readonly IPaymentMethodService _paymentMethodService;
        private readonly ISalesInvoiceDetailService _salesInvoiceDetailService;
        private readonly IItemBarCodeService _itemBarCodeService;
        private readonly IItemTaxService _itemTaxService;
        private readonly IStockOutQuantityService _stockOutQuantityService;
        private readonly IInvoiceStockOutReturnService _invoiceStockOutReturnService;
        private readonly ICostCenterJournalDetailService _costCenterJournalDetailService;

        public SalesInvoiceReturnService(ISalesInvoiceReturnPaymentService salesInvoiceReturnPaymentService, ISalesInvoiceReturnDetailService salesInvoiceReturnDetailService, ISalesInvoiceReturnDetailTaxService salesInvoiceReturnDetailTaxService, ISalesInvoiceReturnHeaderService salesInvoiceReturnHeaderService, IMenuNoteService menuNoteService, IGenericMessageService genericMessageService, IJournalService journalService, IPaymentMethodService paymentMethodService, ISalesInvoiceDetailService salesInvoiceDetailService, IItemBarCodeService itemBarcodeService, IItemTaxService itemTaxService, IStockOutQuantityService stockOutQuantityService, IInvoiceStockOutReturnService invoiceStockOutReturnService, ICostCenterJournalDetailService costCenterJournalDetailService)
        {
            _salesInvoiceReturnPaymentService = salesInvoiceReturnPaymentService;
            _salesInvoiceReturnDetailService = salesInvoiceReturnDetailService;
            _salesInvoiceReturnDetailTaxService = salesInvoiceReturnDetailTaxService;
            _salesInvoiceReturnHeaderService = salesInvoiceReturnHeaderService;
            _menuNoteService = menuNoteService;
            _genericMessageService = genericMessageService;
            _journalService = journalService;
            _paymentMethodService = paymentMethodService;
            _salesInvoiceDetailService = salesInvoiceDetailService;
            _itemBarCodeService = itemBarcodeService;
            _itemTaxService = itemTaxService;
            _stockOutQuantityService = stockOutQuantityService;
            _invoiceStockOutReturnService = invoiceStockOutReturnService;
            _costCenterJournalDetailService = costCenterJournalDetailService;
        }

        public List<RequestChangesDto> GetSalesInvoiceReturnRequestChanges(SalesInvoiceReturnDto oldItem, SalesInvoiceReturnDto newItem)
        {
            var requestChanges = new List<RequestChangesDto>();
            var items = CompareLogic.GetDifferences(oldItem.SalesInvoiceReturnHeader, newItem.SalesInvoiceReturnHeader);
            requestChanges.AddRange(items);

            if (oldItem.SalesInvoiceReturnDetails.Any() && newItem.SalesInvoiceReturnDetails.Any())
            {
                var oldCount = oldItem.SalesInvoiceReturnDetails.Count;
                var newCount = newItem.SalesInvoiceReturnDetails.Count;
                var index = 0;
                if (oldCount <= newCount)
                {
                    for (int i = index; i < oldCount; i++)
                    {
                        for (int j = index; j < newCount; j++)
                        {
                            var changes = CompareLogic.GetDifferences(oldItem.SalesInvoiceReturnDetails[i], newItem.SalesInvoiceReturnDetails[i]);
                            requestChanges.AddRange(changes);

                            var detailTaxChanges = GetSalesInvoiceReturnDetailTaxesRequestChanges(oldItem.SalesInvoiceReturnDetails[i], newItem.SalesInvoiceReturnDetails[i]);
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

			if (oldItem.SalesInvoiceReturnPayments != null && oldItem.SalesInvoiceReturnPayments.Any() && newItem.SalesInvoiceReturnPayments != null && newItem.SalesInvoiceReturnPayments.Any())
			{
				var oldCount = oldItem.SalesInvoiceReturnPayments.Count;
				var newCount = newItem.SalesInvoiceReturnPayments.Count;
				var index = 0;
				if (oldCount <= newCount)
				{
					for (int i = index; i < oldCount; i++)
					{
						for (int j = index; j < newCount; j++)
						{
							var changes = CompareLogic.GetDifferences(oldItem.SalesInvoiceReturnPayments[i], newItem.SalesInvoiceReturnPayments[i]);
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

            requestChanges.RemoveAll(x => x.ColumnName == "SalesInvoiceReturnDetailTaxes" || x.ColumnName == "ItemTaxData");

            return requestChanges;
        }

        private static List<RequestChangesDto> GetSalesInvoiceReturnDetailTaxesRequestChanges(SalesInvoiceReturnDetailDto oldItem, SalesInvoiceReturnDetailDto newItem)
        {
            var requestChanges = new List<RequestChangesDto>();

            if (oldItem.SalesInvoiceReturnDetailTaxes.Any() && newItem.SalesInvoiceReturnDetailTaxes.Any())
            {
                var oldCount = oldItem.SalesInvoiceReturnDetailTaxes.Count;
                var newCount = newItem.SalesInvoiceReturnDetailTaxes.Count;
                var index = 0;
                if (oldCount <= newCount)
                {
                    for (int i = index; i < oldCount; i++)
                    {
                        for (int j = index; j < newCount; j++)
                        {
                            var changes = CompareLogic.GetDifferences(oldItem.SalesInvoiceReturnDetailTaxes[i], newItem.SalesInvoiceReturnDetailTaxes[i]);
                            requestChanges.AddRange(changes);

                            index++;
                            break;
                        }
                    }
                }
            }

            return requestChanges;
        }

        public async Task<SalesInvoiceReturnDto> GetSalesInvoiceReturn(int salesInvoiceReturnHeaderId)
        {
            var header = await _salesInvoiceReturnHeaderService.GetSalesInvoiceReturnHeaderById(salesInvoiceReturnHeaderId);
            if (header == null) return new SalesInvoiceReturnDto();

            var details = await GetSalesInvoiceReturnDetailsCalculated(salesInvoiceReturnHeaderId, header);
            var menuNotes = await _menuNoteService.GetMenuNotes(SalesInvoiceReturnMenuCodeHelper.GetMenuCode(header), salesInvoiceReturnHeaderId).ToListAsync();
            var salesInvoiceReturnDetailTaxes = await _salesInvoiceReturnDetailTaxService.GetSalesInvoiceReturnDetailTaxes(salesInvoiceReturnHeaderId).ToListAsync();
            var journal = await _journalService.GetJournal(header.JournalHeaderId);
			var payments = header.CreditPayment ?
	            new List<SalesInvoiceReturnPaymentDto>() :
	            await _salesInvoiceReturnPaymentService.GetSalesInvoiceReturnPayments(salesInvoiceReturnHeaderId, header.StoreId);

			foreach (var detail in details)
            {
                detail.SalesInvoiceReturnDetailTaxes = salesInvoiceReturnDetailTaxes.Where(x => x.SalesInvoiceReturnDetailId == detail.SalesInvoiceReturnDetailId).ToList();
            }

            return new SalesInvoiceReturnDto() { SalesInvoiceReturnHeader = header, SalesInvoiceReturnDetails = details, Journal = journal, MenuNotes = menuNotes, SalesInvoiceReturnPayments = payments};
		}

		public async Task<List<SalesInvoiceReturnDetailDto>> GetSalesInvoiceReturnDetailsCalculated(int salesInvoiceReturnHeaderId, SalesInvoiceReturnHeaderDto? salesInvoiceReturnHeader = null, List<SalesInvoiceReturnDetailDto>? salesInvoiceReturnDetails = null)
		{
			salesInvoiceReturnHeader ??= await _salesInvoiceReturnHeaderService.GetSalesInvoiceReturnHeaderById(salesInvoiceReturnHeaderId);

            var salesInvoiceHeaderId = salesInvoiceReturnHeader!.SalesInvoiceHeaderId;
            var isDirectInvoice = salesInvoiceReturnHeader!.IsDirectInvoice;
			salesInvoiceReturnDetails ??= await _salesInvoiceReturnDetailService.GetSalesInvoiceReturnDetailsAsQueryable(salesInvoiceReturnHeaderId).ToListAsync();
			
            var itemIds = salesInvoiceReturnDetails.Select(x => x.ItemId).ToList();
			var salesInvoiceDetailsGrouped = await _salesInvoiceDetailService.GetSalesInvoiceDetailsGrouped(salesInvoiceHeaderId);

            var relatedStockOutReturnHeaderId = isDirectInvoice ? await GetStockOutReturnHeaderIdLinkedToSalesInvoiceReturn(salesInvoiceReturnHeaderId) : 0;
            var stocksReturned = isDirectInvoice ? await _stockOutQuantityService.GetStocksDisbursedReturnedFromSalesInvoiceExceptStockOutReturnHeaderId(salesInvoiceHeaderId, relatedStockOutReturnHeaderId).Where(x => itemIds.Contains(x.ItemId)).ToListAsync() : [];

			salesInvoiceReturnDetails = (
				from salesInvoiceReturnDetail in salesInvoiceReturnDetails
				from salesInvoiceDetailGroup in salesInvoiceDetailsGrouped.Where(x => x.ItemId == salesInvoiceReturnDetail.ItemId && x.ItemPackageId == salesInvoiceReturnDetail.ItemPackageId && x.CostCenterId == salesInvoiceReturnDetail.CostCenterId && x.BarCode == salesInvoiceReturnDetail.BarCode && x.SellingPrice == salesInvoiceReturnDetail.SellingPrice).DefaultIfEmpty()
				from stockReturned in stocksReturned.Where(x => x.ItemId == salesInvoiceReturnDetail.ItemId && x.ItemPackageId == salesInvoiceReturnDetail.ItemPackageId && x.CostCenterId == salesInvoiceReturnDetail.CostCenterId && x.BarCode == salesInvoiceReturnDetail.BarCode && x.SellingPrice == salesInvoiceReturnDetail.SellingPrice).DefaultIfEmpty()
				select new SalesInvoiceReturnDetailDto
				{
					SalesInvoiceReturnDetailId = salesInvoiceReturnDetail.SalesInvoiceReturnDetailId,
					SalesInvoiceReturnHeaderId = salesInvoiceReturnDetail.SalesInvoiceReturnHeaderId,
					CostCenterId = salesInvoiceReturnDetail.CostCenterId,
					CostCenterName = salesInvoiceReturnDetail.CostCenterName,
					ItemId = salesInvoiceReturnDetail.ItemId,
					ItemCode = salesInvoiceReturnDetail.ItemCode,
					ItemName = salesInvoiceReturnDetail.ItemName,
					TaxTypeId = salesInvoiceReturnDetail.TaxTypeId,
                    ItemTypeId = salesInvoiceReturnDetail.ItemTypeId,
					ItemPackageId = salesInvoiceReturnDetail.ItemPackageId,
					ItemPackageName = salesInvoiceReturnDetail.ItemPackageName,
					IsItemVatInclusive = salesInvoiceReturnDetail.IsItemVatInclusive,
					BarCode = salesInvoiceReturnDetail.BarCode,
					Packing = salesInvoiceReturnDetail.Packing,
					ExpireDate = salesInvoiceReturnDetail.ExpireDate,
					BatchNumber = salesInvoiceReturnDetail.BatchNumber,
					Quantity = salesInvoiceReturnDetail.Quantity,
					BonusQuantity = salesInvoiceReturnDetail.BonusQuantity,
					SalesInvoiceQuantity = salesInvoiceDetailGroup != null ? salesInvoiceDetailGroup.Quantity : 0,
					SalesInvoiceBonusQuantity = salesInvoiceDetailGroup != null ? salesInvoiceDetailGroup.BonusQuantity : 0,
					AvailableQuantity = isDirectInvoice ? ((salesInvoiceDetailGroup != null ? salesInvoiceDetailGroup.Quantity : 0) - (stockReturned != null ? stockReturned.QuantityReturned : 0)) : 0,
					AvailableBonusQuantity = isDirectInvoice ? ((salesInvoiceDetailGroup != null ? salesInvoiceDetailGroup.BonusQuantity : 0) - (stockReturned != null ? stockReturned.BonusQuantityReturned : 0)) : 0,
					SellingPrice = salesInvoiceReturnDetail.SellingPrice,
					TotalValue = salesInvoiceReturnDetail.TotalValue,
					ItemDiscountPercent = salesInvoiceReturnDetail.ItemDiscountPercent,
					ItemDiscountValue = salesInvoiceReturnDetail.ItemDiscountValue,
					TotalValueAfterDiscount = salesInvoiceReturnDetail.TotalValueAfterDiscount,
					HeaderDiscountValue = salesInvoiceReturnDetail.HeaderDiscountValue,
					GrossValue = salesInvoiceReturnDetail.GrossValue,
					VatPercent = salesInvoiceReturnDetail.VatPercent,
					VatValue = salesInvoiceReturnDetail.VatValue,
					SubNetValue = salesInvoiceReturnDetail.SubNetValue,
					OtherTaxValue = salesInvoiceReturnDetail.OtherTaxValue,
					NetValue = salesInvoiceReturnDetail.NetValue,
					Notes = salesInvoiceReturnDetail.Notes,
                    ItemNote = salesInvoiceReturnDetail.ItemNote,
					ConsumerPrice = salesInvoiceReturnDetail.ConsumerPrice,
					CostPrice = salesInvoiceReturnDetail.CostPrice,
					CostPackage = salesInvoiceReturnDetail.CostPackage,
					LastSalesPrice = salesInvoiceReturnDetail.LastSalesPrice,

					SalesInvoiceReturnDetailTaxes = salesInvoiceReturnDetail.SalesInvoiceReturnDetailTaxes,

					CreatedAt = salesInvoiceReturnDetail.CreatedAt,
					IpAddressCreated = salesInvoiceReturnDetail.IpAddressCreated,
					UserNameCreated = salesInvoiceReturnDetail.UserNameCreated,
				}).ToList();


			var packages = await _itemBarCodeService.GetItemsPackages(itemIds);
			var itemTaxData = await _itemTaxService.GetItemTaxDataByItemIds(itemIds);

			foreach (var salesInvoiceReturnDetail in salesInvoiceReturnDetails)
			{
				salesInvoiceReturnDetail.Packages = packages.Where(x => x.ItemId == salesInvoiceReturnDetail.ItemId).Select(s => s.Packages).FirstOrDefault().ToJson();
				salesInvoiceReturnDetail.ItemTaxData = itemTaxData.Where(x => x.ItemId == salesInvoiceReturnDetail.ItemId).ToList();
				salesInvoiceReturnDetail.Taxes = salesInvoiceReturnDetail.ItemTaxData.ToJson();
			}

			return salesInvoiceReturnDetails;
		}

		private async Task<int> GetStockOutReturnHeaderIdLinkedToSalesInvoiceReturn(int salesInvoiceReturnHeaderId)
		{
			if (salesInvoiceReturnHeaderId == 0)
			{
				return 0;
			}
			else
			{
				return await _invoiceStockOutReturnService.GetStockOutReturnsLinkedToSalesInvoiceReturn(salesInvoiceReturnHeaderId).FirstAsync();
			}
		}


		public async Task<List<SalesInvoiceReturnPaymentDto>> AddNonincludedPaymentMethods(List<SalesInvoiceReturnPaymentDto> invoicePayments, int storeId)
		{
			var includedPaymentMethods = invoicePayments.Select(x => x.PaymentMethodId).ToList();

			var toAdd = (await _paymentMethodService.GetVoucherPaymentMethods(storeId, true, true)).Where(x => !includedPaymentMethods.Contains(x.PaymentMethodId)).Select(x => new SalesInvoiceReturnPaymentDto
			{
				SalesInvoiceReturnPaymentId = 0,
				SalesInvoiceReturnHeaderId = 0,
				PaymentMethodId = x.PaymentMethodId,
				PaymentMethodName = x.PaymentMethodName,
				AccountId = x.AccountId,
				CurrencyId = x.CurrencyId,
				CurrencyName = x.CurrencyName,
				CurrencyRate = x.CurrencyRate,
				PaidValue = 0,
				PaidValueAccount = 0,
				RemarksAr = null,
				RemarksEn = null
			});

			invoicePayments.AddRange(toAdd);

			var newId = -1;
			invoicePayments.ForEach(x => x.SalesInvoiceReturnPaymentId = x.SalesInvoiceReturnPaymentId <= 0 ? newId-- : x.SalesInvoiceReturnPaymentId);

			return invoicePayments;
		}

		public async Task<ResponseDto> SaveSalesInvoiceReturn(SalesInvoiceReturnDto salesInvoiceReturn, bool hasApprove, bool approved, int? requestId, string? documentReference)
        {
            if (salesInvoiceReturn.SalesInvoiceReturnHeader != null)
            {
                if (!salesInvoiceReturn.SalesInvoiceReturnDetails.Any()) return new ResponseDto { Message = await _genericMessageService.GetMessage(MenuCodeData.SalesInvoiceReturn, GenericMessageData.DetailIsEmpty) };

                var menuCode = (short)SalesInvoiceReturnMenuCodeHelper.GetMenuCode(salesInvoiceReturn.SalesInvoiceReturnHeader);
                if (salesInvoiceReturn.Journal != null)
                {
                    salesInvoiceReturn.Journal.JournalHeader!.MenuCode = menuCode;
                    salesInvoiceReturn.Journal.JournalHeader!.MenuReferenceId = await _salesInvoiceReturnHeaderService.GetNextId();
                    var journalResult = await _journalService.SaveJournal(salesInvoiceReturn.Journal, hasApprove, approved, requestId);

                    if (journalResult.Success)
                    {
                        salesInvoiceReturn.SalesInvoiceReturnHeader.JournalHeaderId = journalResult.Id;
                    }
                    else
                    {
                        return journalResult;
                    }
                }

                var result = await _salesInvoiceReturnHeaderService.SaveSalesInvoiceReturnHeader(salesInvoiceReturn.SalesInvoiceReturnHeader, hasApprove, approved, requestId, documentReference);
                if (result.Success)
                {
                    var modifiedSalesInvoiceReturnDetails = await _salesInvoiceReturnDetailService.SaveSalesInvoiceReturnDetails(result.Id, salesInvoiceReturn.SalesInvoiceReturnDetails);
                    await SaveSalesInvoiceReturnDetailTaxes(result.Id, modifiedSalesInvoiceReturnDetails);
					await UpdateCostCenterJournalDetails(result.Id, modifiedSalesInvoiceReturnDetails, menuCode);
                    await _salesInvoiceReturnDetailService.DeleteSalesInvoiceReturnDetailList(modifiedSalesInvoiceReturnDetails, result.Id);
                    await _salesInvoiceReturnPaymentService.SaveSalesInvoiceReturnPayments(result.Id, salesInvoiceReturn.SalesInvoiceReturnPayments);

                    if (salesInvoiceReturn.MenuNotes != null)
                    {
                        await _menuNoteService.SaveMenuNotes(salesInvoiceReturn.MenuNotes, result.Id);
                    }
                }

                return result;
            }
            return new ResponseDto { Message = "Header should not be null" };
        }

		private async Task UpdateCostCenterJournalDetails(int salesInvoiceReturnHeaderId, List<SalesInvoiceReturnDetailDto> salesInvoiceReturnDetails, short menuCode)
		{
			await _costCenterJournalDetailService.UpdateCostCenterJournalDetailsBasedOnInvoiceDetails(
				invoiceHeaderId: salesInvoiceReturnHeaderId, 
				invoiceDetails: salesInvoiceReturnDetails,
				detailIdSelector: x => x.SalesInvoiceReturnDetailId,
				itemIdSelector: x => x.ItemId,
				costCenterIdSelector: x => x.CostCenterId,
				creditValueSelector: x => 0,
				debitValueSelector: x => x.NetValue,
				remarksSelector: x => x.Notes,
				menuCode: menuCode
			);
		}

        private async Task SaveSalesInvoiceReturnDetailTaxes(int salesInvoiceReturnHeaderId, List<SalesInvoiceReturnDetailDto> salesInvoiceReturnDetails)
        {
            List<SalesInvoiceReturnDetailTaxDto> salesInvoiceReturnDetailTaxes = new List<SalesInvoiceReturnDetailTaxDto>();

            foreach (var salesInvoiceReturnDetail in salesInvoiceReturnDetails)
            {
                foreach (var salesInvoiceReturnDetailTax in salesInvoiceReturnDetail.SalesInvoiceReturnDetailTaxes)
                {
                    salesInvoiceReturnDetailTax.SalesInvoiceReturnDetailId = salesInvoiceReturnDetail.SalesInvoiceReturnDetailId;
                    salesInvoiceReturnDetailTaxes.Add(salesInvoiceReturnDetailTax);
                }
            }

            await _salesInvoiceReturnDetailTaxService.SaveSalesInvoiceReturnDetailTaxes(salesInvoiceReturnHeaderId, salesInvoiceReturnDetailTaxes);
        }

        public async Task<ResponseDto> DeleteSalesInvoiceReturn(int salesInvoiceReturnHeaderId, int menuCode)
        {
            var salesInvoiceReturnHeader = await _salesInvoiceReturnHeaderService.GetSalesInvoiceReturnHeaderById(salesInvoiceReturnHeaderId);

			await _costCenterJournalDetailService.DeleteCostCenterJournalDetails(salesInvoiceReturnHeaderId, (short)menuCode);
            await _menuNoteService.DeleteMenuNotes(menuCode, salesInvoiceReturnHeaderId);
            await _salesInvoiceReturnDetailTaxService.DeleteSalesInvoiceReturnDetailTaxes(salesInvoiceReturnHeaderId);
            await _salesInvoiceReturnDetailService.DeleteSalesInvoiceReturnDetails(salesInvoiceReturnHeaderId);
            await _salesInvoiceReturnPaymentService.DeleteSalesInvoiceReturnPayments(salesInvoiceReturnHeaderId);

            var salesInvoiceReturnResult = await _salesInvoiceReturnHeaderService.DeleteSalesInvoiceReturnHeader(salesInvoiceReturnHeaderId, menuCode);
            if( salesInvoiceReturnResult.Success == false)
            {
                return salesInvoiceReturnResult;
            }

            var journalResult = await _journalService.DeleteJournal(salesInvoiceReturnHeader!.JournalHeaderId);
            if (journalResult.Success == false)
            {
                return journalResult;
            }

            return salesInvoiceReturnResult;
        }
    }
}
