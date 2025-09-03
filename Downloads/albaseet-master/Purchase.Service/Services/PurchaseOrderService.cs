using Purchases.CoreOne.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accounting.CoreOne.Models.Dtos.ViewModels;
using Purchases.CoreOne.Models.Dtos.ViewModels;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.Helper.Models.Dtos;
using Shared.Service.Logic.Approval;
using Shared.CoreOne.Contracts.Inventory;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Shared.CoreOne.Contracts.Modules;
using Shared.Service.Logic.Calculation;
using Shared.CoreOne.Contracts.Taxes;
using Shared.Service.Services.Taxes;
using Shared.CoreOne.Contracts.Items;
using Shared.CoreOne.Models.StaticData;
using System.ComponentModel.DataAnnotations;
using Shared.Service.Services.Modules;
using System.Reflection.PortableExecutable;
using Purchases.CoreOne.Models.Domain;
using System.Runtime.CompilerServices;
using Shared.Service.Services.Inventory;
using Shared.Service.Services.Items;
using Microsoft.AspNetCore.Http;
using Shared.Helper.Identity;
using Shared.CoreOne.Contracts.CostCenters;
using Shared.Helper.Extensions;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Contracts.Menus;

namespace Purchases.Service.Services
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly IPurchaseOrderHeaderService _purchaseOrderHeaderService;
        private readonly IPurchaseOrderDetailService _purchaseOrderDetailService;
        private readonly ISupplierQuotationHeaderService _supplierQuotationHeaderService;
        private readonly ISupplierQuotationDetailService _supplierQuotationDetailService;
        private readonly ISupplierQuotationDetailTaxService _supplierQuotationDetailTaxService;
        private readonly ITaxPercentService _taxPercentService;
        private readonly IItemService _itemService;
        private readonly IMenuNoteService _menuNoteService;
        private readonly IPurchaseOrderDetailTaxService _purchaseOrderDetailTaxService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IItemPackageService _itemPackageService;
        private readonly IItemPackingService _itemPackingService;
        private readonly IItemCostService _itemCostService;
        private readonly IPurchaseInvoiceService _purchaseInvoiceService;
        private readonly IPurchaseInvoiceDetailService _purchaseInvoiceDetailService;
        private readonly ICostCenterService _costCenterService;
        private readonly IItemBarCodeService _itemBarCodeService;
        private readonly IItemTaxService _itemTaxService;
        private readonly IStockInHeaderService _stockInHeaderService;
        private readonly IStockInReturnHeaderService _stockInReturnHeaderService;
        private readonly IPurchaseInvoiceHeaderService _purchaseInvoiceHeaderService;
        private readonly IPurchaseInvoiceReturnHeaderService _purchaseInvoiceReturnHeaderService;
        private readonly IGenericMessageService _genericMessageService;
        private readonly ITaxService _taxService;
        private readonly IItemNoteValidationService _itemNoteValidationService;


        public PurchaseOrderService(IPurchaseOrderHeaderService purchaseOrderHeaderService, IPurchaseOrderDetailService purchaseOrderDetailService, ITaxPercentService taxPercentService, ISupplierQuotationHeaderService supplierQuotationHeaderService, ISupplierQuotationDetailService supplierQuotationDetailService, ISupplierQuotationDetailTaxService supplierQuotationDetailTaxService, IItemService itemService, IMenuNoteService menuNoteService, IPurchaseOrderDetailTaxService purchaseOrderDetailTaxService, IHttpContextAccessor httpContextAccessor, IItemPackageService itemPackageService, IItemPackingService itemPackingService, IItemCostService itemCostService, IPurchaseInvoiceService purchaseInvoiceService, IPurchaseInvoiceDetailService purchaseInvoiceDetailService, ICostCenterService costCenterService, IItemBarCodeService itemBarCodeService, IItemTaxService itemTaxService, IStockInHeaderService stockInHeaderService, IStockInReturnHeaderService stockInReturnHeaderService, IPurchaseInvoiceHeaderService purchaseInvoiceHeaderService, IPurchaseInvoiceReturnHeaderService purchaseInvoiceReturnHeaderService, IGenericMessageService genericMessageService, ITaxService taxService, IItemNoteValidationService itemNoteValidationService)
        {
            _purchaseOrderHeaderService = purchaseOrderHeaderService;
            _purchaseOrderDetailService = purchaseOrderDetailService;
            _supplierQuotationHeaderService = supplierQuotationHeaderService;
            _supplierQuotationDetailService = supplierQuotationDetailService;
            _supplierQuotationDetailTaxService = supplierQuotationDetailTaxService;
            _taxPercentService = taxPercentService;
            _itemService = itemService;
            _menuNoteService = menuNoteService;
            _purchaseOrderDetailTaxService = purchaseOrderDetailTaxService;
            _httpContextAccessor = httpContextAccessor;
            _itemPackageService = itemPackageService;
            _itemPackingService = itemPackingService;
            _itemCostService = itemCostService;
            _purchaseInvoiceService = purchaseInvoiceService;
            _purchaseInvoiceDetailService = purchaseInvoiceDetailService;
            _costCenterService = costCenterService;
            _itemBarCodeService = itemBarCodeService;
            _itemTaxService = itemTaxService;
            _stockInHeaderService = stockInHeaderService;
            _stockInReturnHeaderService = stockInReturnHeaderService;
            _purchaseInvoiceHeaderService = purchaseInvoiceHeaderService;
            _purchaseInvoiceReturnHeaderService = purchaseInvoiceReturnHeaderService;
            _genericMessageService = genericMessageService;
            _taxService = taxService;
            _itemNoteValidationService = itemNoteValidationService;
        }

        public List<RequestChangesDto> GetPurchaseOrderRequestChanges(PurchaseOrderDto oldItem, PurchaseOrderDto newItem)
        {
            var requestChanges = new List<RequestChangesDto>();
            var items = CompareLogic.GetDifferences(oldItem.PurchaseOrderHeader, newItem.PurchaseOrderHeader);
            requestChanges.AddRange(items);

            if (oldItem.PurchaseOrderDetails.Any() && newItem.PurchaseOrderDetails.Any())
            {
                var oldCount = oldItem.PurchaseOrderDetails.Count;
                var newCount = newItem.PurchaseOrderDetails.Count;
                var index = 0;
                if (oldCount <= newCount)
                {
                    for (int i = index; i < oldCount; i++)
                    {
                        for (int j = index; j < newCount; j++)
                        {
                            var changes = CompareLogic.GetDifferences(oldItem.PurchaseOrderDetails[i], newItem.PurchaseOrderDetails[i]);
                            requestChanges.AddRange(changes);

                            var detailTaxChanges = GetPurchaseOrderDetailTaxesRequestChanges(oldItem.PurchaseOrderDetails[i], newItem.PurchaseOrderDetails[i]);
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

            requestChanges.RemoveAll(x => x.ColumnName == "PurchaseOrderDetailTaxes" || x.ColumnName == "ItemTaxData");

            return requestChanges;
        }

        private static List<RequestChangesDto> GetPurchaseOrderDetailTaxesRequestChanges(PurchaseOrderDetailDto oldItem, PurchaseOrderDetailDto newItem)
        {
            var requestChanges = new List<RequestChangesDto>();

            if (oldItem.PurchaseOrderDetailTaxes.Any() && newItem.PurchaseOrderDetailTaxes.Any())
            {
                var oldCount = oldItem.PurchaseOrderDetailTaxes.Count;
                var newCount = newItem.PurchaseOrderDetailTaxes.Count;
                var index = 0;
                if (oldCount <= newCount)
                {
                    for (int i = index; i < oldCount; i++)
                    {
                        for (int j = index; j < newCount; j++)
                        {
                            var changes = CompareLogic.GetDifferences(oldItem.PurchaseOrderDetailTaxes[i], newItem.PurchaseOrderDetailTaxes[i]);
                            requestChanges.AddRange(changes);

                            index++;
                            break;
                        }
                    }
                }
            }

            return requestChanges;
        }

        public async Task<PurchaseOrderDto> GetPurchaseOrder(int purchaseOrderHeaderId)
        {
            var header = await _purchaseOrderHeaderService.GetPurchaseOrderHeaderById(purchaseOrderHeaderId);
            var details = await _purchaseOrderDetailService.GetPurchaseOrderDetails(purchaseOrderHeaderId);
            var menuNotes = await _menuNoteService.GetMenuNotes(MenuCodeData.PurchaseOrder,purchaseOrderHeaderId).ToListAsync();
            var purchaseOrderDetailTaxes = await _purchaseOrderDetailTaxService.GetPurchaseOrderDetailTaxes(purchaseOrderHeaderId).ToListAsync();

            foreach (var detail in details)
            {
                detail.PurchaseOrderDetailTaxes = purchaseOrderDetailTaxes.Where(x => x.PurchaseOrderDetailId == detail.PurchaseOrderDetailId).ToList();
            }

            return new PurchaseOrderDto() { PurchaseOrderHeader = header, PurchaseOrderDetails = details, MenuNotes = menuNotes};
        }

        public async Task<PurchaseOrderDto> GetPurchaseOrderFromSupplierQuotation(int supplierQuotationHeaderId)
        {
            var supplierQuotationHeader = await _supplierQuotationHeaderService.GetSupplierQuotationHeaderById(supplierQuotationHeaderId);

            if (supplierQuotationHeader == null)
            {
                return new PurchaseOrderDto();
            }

            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var purchaseOrderDetails =
             await (from supplierQuotationDetail in _supplierQuotationDetailService.GetAll().Where(x => x.SupplierQuotationHeaderId == supplierQuotationHeaderId).OrderBy(x => x.SupplierQuotationDetailId)
                    from item in _itemService.GetAll().Where(x => x.ItemId == supplierQuotationDetail.ItemId)
                    from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == supplierQuotationDetail.ItemPackageId)
                    from costCenter in _costCenterService.GetAll().Where(x => x.CostCenterId == supplierQuotationDetail.CostCenterId).DefaultIfEmpty()
                    from itemCost in _itemCostService.GetAll().Where(x => x.ItemId == supplierQuotationDetail.ItemId && x.StoreId == supplierQuotationHeader.StoreId).DefaultIfEmpty()
                    from itemPacking in _itemPackingService.GetAll().Where(x => x.ItemId == supplierQuotationDetail.ItemId && x.FromPackageId == supplierQuotationDetail.ItemPackageId && x.ToPackageId == item.SingularPackageId)
                    from purchaseInvoiceDetail in _purchaseInvoiceDetailService.GetAll().Where(x => x.ItemId == supplierQuotationDetail.ItemId && x.ItemPackageId == supplierQuotationDetail.ItemPackageId).OrderByDescending(x => x.PurchaseInvoiceDetailId).Take(1).DefaultIfEmpty()
                    select new PurchaseOrderDetailDto
                    {
                        PurchaseOrderDetailId = supplierQuotationDetail.SupplierQuotationDetailId, // <-- This is used to get the related detail taxes
                        CostCenterId = supplierQuotationDetail.CostCenterId,
                        CostCenterName = costCenter != null ? language == LanguageCode.Arabic ? costCenter.CostCenterNameAr : costCenter.CostCenterNameEn : null,
                        ItemId = supplierQuotationDetail.ItemId,
                        ItemCode = item.ItemCode,
                        ItemName = language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn,
                        TaxTypeId = item.TaxTypeId,
                        ItemTypeId = item.ItemTypeId,
                        ItemPackageId = supplierQuotationDetail.ItemPackageId,
                        ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
                        IsItemVatInclusive = supplierQuotationDetail.IsItemVatInclusive,
                        BarCode = supplierQuotationDetail.BarCode,
                        Packing = itemPacking.Packing,
                        Quantity = supplierQuotationDetail.Quantity,
                        BonusQuantity = 0,
                        PurchasePrice = supplierQuotationDetail.ReceivedPrice,
                        TotalValue = supplierQuotationDetail.TotalValue,
                        ItemDiscountPercent = supplierQuotationDetail.ItemDiscountPercent,
                        ItemDiscountValue = supplierQuotationDetail.ItemDiscountValue,
                        TotalValueAfterDiscount = supplierQuotationDetail.TotalValueAfterDiscount,
                        HeaderDiscountValue = supplierQuotationDetail.HeaderDiscountValue,
                        GrossValue = supplierQuotationDetail.GrossValue,
                        VatPercent = supplierQuotationDetail.VatPercent,
                        VatValue = supplierQuotationDetail.VatValue,
                        SubNetValue = supplierQuotationDetail.SubNetValue,
                        OtherTaxValue = supplierQuotationDetail.OtherTaxValue,
                        NetValue = supplierQuotationDetail.NetValue,
                        Notes = supplierQuotationDetail.Notes,
                        ItemNote = supplierQuotationDetail.ItemNote,
                        ConsumerPrice = item.ConsumerPrice,
                        CostPrice = itemCost != null ? itemCost.CostPrice : 0,
                        CostPackage = itemCost != null ? itemCost.CostPrice * itemPacking.Packing : 0,
                        CostValue = itemCost != null ? itemCost.CostPrice * itemPacking.Packing * supplierQuotationDetail.Quantity : 0,
                        LastPurchasePrice = purchaseInvoiceDetail != null ? purchaseInvoiceDetail.PurchasePrice : 0,
                        CreatedAt = supplierQuotationDetail.CreatedAt,
                        IpAddressCreated = supplierQuotationDetail.IpAddressCreated,
                        UserNameCreated = supplierQuotationDetail.UserNameCreated,
                    }).ToListAsync();


            var itemIds = purchaseOrderDetails.Select(x => x.ItemId).ToList();
            var packages = await _itemBarCodeService.GetItemsPackages(itemIds);
            var itemTaxData = await _itemTaxService.GetItemTaxDataByItemIds(itemIds);
            var supplierQuotationDetailTaxData = await _supplierQuotationDetailTaxService.GetSupplierQuotationDetailTaxes(supplierQuotationHeaderId).ToListAsync();

            int newId = -1;
            int newSubId = -1;
            foreach (var purchaseOrderDetail in purchaseOrderDetails)
            {

                purchaseOrderDetail.Packages = packages.Where(x => x.ItemId == purchaseOrderDetail.ItemId).Select(s => s.Packages).FirstOrDefault().ToJson();
                purchaseOrderDetail.ItemTaxData = itemTaxData.Where(x => x.ItemId == purchaseOrderDetail.ItemId).ToList();
                purchaseOrderDetail.Taxes = purchaseOrderDetail.ItemTaxData.ToJson();

                purchaseOrderDetail.PurchaseOrderDetailTaxes = (
                        from itemTax in supplierQuotationDetailTaxData.Where(x => x.SupplierQuotationDetailId == purchaseOrderDetail.PurchaseOrderDetailId)
                        select new PurchaseOrderDetailTaxDto
                        {
                            TaxId = itemTax.TaxId,
                            DebitAccountId = itemTax.DebitAccountId,
                            TaxAfterVatInclusive = itemTax.TaxAfterVatInclusive,
                            TaxPercent = itemTax.TaxPercent,
                            TaxValue = itemTax.TaxValue
                        }
                    ).ToList();

                purchaseOrderDetail.PurchaseOrderDetailId = newId;
                purchaseOrderDetail.PurchaseOrderDetailTaxes.ForEach(y =>
                {
                    y.PurchaseOrderDetailId = newId;
                    y.PurchaseOrderDetailTaxId = newSubId--;
                });
                newId--;
            }

            var totalCostValueFromDetail = purchaseOrderDetails.Sum(x => x.CostValue);

            var purchaseOrderHeader = new PurchaseOrderHeaderDto
            {
                PurchaseOrderHeaderId = 0,
                SupplierQuotationHeaderId = supplierQuotationHeader.SupplierQuotationHeaderId,
                DocumentDate = supplierQuotationHeader.DocumentDate,
                SupplierId = supplierQuotationHeader.SupplierId,
                SupplierCode = supplierQuotationHeader.SupplierCode,
                SupplierName = supplierQuotationHeader.SupplierName,
                StoreId = supplierQuotationHeader.StoreId,
                StoreName = supplierQuotationHeader.StoreName,
                TaxTypeId = supplierQuotationHeader.TaxTypeId,
                Reference = supplierQuotationHeader.Reference,
                CreditPayment = false,
                PaymentPeriodDays = null,
                DueDate = null,
                ShipmentTypeId = null,
                DeliveryDate = null,
                TotalValue = supplierQuotationHeader.TotalValue,
                DiscountPercent = supplierQuotationHeader.DiscountPercent,
                DiscountValue = supplierQuotationHeader.DiscountValue,
                TotalItemDiscount = supplierQuotationHeader.TotalItemDiscount,
                GrossValue = supplierQuotationHeader.GrossValue,
                VatValue = supplierQuotationHeader.VatValue,
                SubNetValue = supplierQuotationHeader.SubNetValue,
                OtherTaxValue = supplierQuotationHeader.OtherTaxValue,
				NetValueBeforeAdditionalDiscount = supplierQuotationHeader.NetValueBeforeAdditionalDiscount,
				AdditionalDiscountValue = supplierQuotationHeader.AdditionalDiscountValue,
                NetValue = supplierQuotationHeader.NetValue,
                TotalCostValue = totalCostValueFromDetail,
                RemarksAr = supplierQuotationHeader.RemarksAr,
                RemarksEn = supplierQuotationHeader.RemarksEn,
                IsClosed = false,
                IsCancelled = false,
                IsEnded = false,
                IsBlocked = false,
                ArchiveHeaderId = supplierQuotationHeader.ArchiveHeaderId,
            };

            return new PurchaseOrderDto { PurchaseOrderHeader = purchaseOrderHeader, PurchaseOrderDetails = purchaseOrderDetails };
        }

        public async Task<ResponseDto> UpdateBlocked(int purchaseOrderHeaderId, bool isBlocked)
        {
            await _purchaseOrderHeaderService.UpdateBlocked(purchaseOrderHeaderId, isBlocked);
            await _stockInHeaderService.UpdateAllStockInsBlockedFromPurchaseOrder(purchaseOrderHeaderId, isBlocked);
            await _stockInReturnHeaderService.UpdateAllStockInReturnsBlockedFromPurchaseOrder(purchaseOrderHeaderId, isBlocked);
            await _purchaseInvoiceHeaderService.UpdateAllPurchaseInvoicesBlockedFromPurchaseOrder(purchaseOrderHeaderId, isBlocked);
            await _purchaseInvoiceReturnHeaderService.UpdateAllPurchaseInvoiceReturnsBlockedFromPurchaseOrder(purchaseOrderHeaderId, isBlocked);

            return new ResponseDto { Id = purchaseOrderHeaderId, Success = true, Message = await _genericMessageService.GetMessage(MenuCodeData.PurchaseOrder, isBlocked ? GenericMessageData.StoppedDealingOnSuccessfully : GenericMessageData.OpenedToDealingOnSuccessfully) };
        }

        public async Task<ResponseDto> SavePurchaseOrder(PurchaseOrderDto purchaseOrder, bool hasApprove, bool approved, int? requestId, bool shouldValidate = true, string? documentReference = null, int documentStatusId = DocumentStatusData.PurchaseOrderCreated, bool shouldInitializeFlags = false)
        {
            TrimDetailStrings(purchaseOrder.PurchaseOrderDetails);
            if (purchaseOrder.PurchaseOrderHeader != null)
			{
				if (shouldValidate && purchaseOrder.PurchaseOrderHeader.PurchaseOrderHeaderId == 0)
				{
					var itemNoteValidationResult = await _itemNoteValidationService.CheckItemNoteWithItemType(purchaseOrder.PurchaseOrderDetails, x => x.ItemId, x => x.ItemNote);
					if (itemNoteValidationResult.Success == false) return itemNoteValidationResult;

					await UpdateModelPrices(purchaseOrder);
                }

                var result = await _purchaseOrderHeaderService.SavePurchaseOrderHeader(purchaseOrder.PurchaseOrderHeader, hasApprove, approved, requestId, shouldValidate, documentReference, documentStatusId, shouldInitializeFlags);
                if (result.Success)
                {
                    var modifiedPurchaseOrderDetails = await _purchaseOrderDetailService.SavePurchaseOrderDetails(result.Id, purchaseOrder.PurchaseOrderDetails);
                    await SavePurchaseOrderDetailTaxes(result.Id, modifiedPurchaseOrderDetails);
                    await _purchaseOrderDetailService.DeletePurchaseOrderDetailList(modifiedPurchaseOrderDetails, result.Id);
                    if (purchaseOrder.MenuNotes != null)
                    {
                        await _menuNoteService.SaveMenuNotes(purchaseOrder.MenuNotes, result.Id);
                    }
                }

                return result;
            }
            return new ResponseDto { Message = "Header should not be null"};
		}

		private void TrimDetailStrings(List<PurchaseOrderDetailDto> purchaseOrderDetails)
		{
			foreach (var purchaseOrderDetail in purchaseOrderDetails)
			{
				purchaseOrderDetail.ItemNote = string.IsNullOrWhiteSpace(purchaseOrderDetail.ItemNote) ? null : purchaseOrderDetail.ItemNote.Trim();
			}
		}

		private async Task UpdateModelPrices(PurchaseOrderDto purchaseOrder)
        {
            await UpdateDetailPrices(purchaseOrder.PurchaseOrderDetails, purchaseOrder.PurchaseOrderHeader!.StoreId);
            purchaseOrder.PurchaseOrderHeader!.TotalCostValue = purchaseOrder.PurchaseOrderDetails.Sum(x => x.CostValue);
        }

        private async Task UpdateDetailPrices(List<PurchaseOrderDetailDto> purchaseOrderDetails, int storeId)
        {
            var itemIds = purchaseOrderDetails.Select(x => x.ItemId).ToList();

            var itemCosts = await _itemCostService.GetAll().Where(x => itemIds.Contains(x.ItemId) && x.StoreId == storeId).Select(x => new { x.StoreId, x.ItemId, x.CostPrice }).ToListAsync();
            var consumerPrices = await _itemService.GetAll().Where(x => itemIds.Contains(x.ItemId)).Select(x => new { x.ItemId, x.ConsumerPrice }).ToListAsync();
            var lastPurchasePrices = await _purchaseInvoiceService.GetMultipleLastPurchasePrices(itemIds);

            var packings = await (
                        from item in _itemService.GetAll().Where(x => itemIds.Contains(x.ItemId))
                        from itemPacking in _itemPackingService.GetAll().Where(x => x.ItemId == item.ItemId && x.ToPackageId == item.SingularPackageId)
                        select new
                        {
                            item.ItemId,
                            itemPacking.FromPackageId,
                            itemPacking.Packing
                        }
                    ).ToListAsync();

            foreach (var purchaseOrderDetail in purchaseOrderDetails)
            {
                var packing = packings.Where(x => x.ItemId == purchaseOrderDetail.ItemId && x.FromPackageId == purchaseOrderDetail.ItemPackageId).Select(x => x.Packing).FirstOrDefault(1);

                purchaseOrderDetail.ConsumerPrice = consumerPrices.Where(x => x.ItemId == purchaseOrderDetail.ItemId).Select(x => x.ConsumerPrice).FirstOrDefault(0);
                purchaseOrderDetail.CostPrice = itemCosts.Where(x => x.ItemId == purchaseOrderDetail.ItemId && x.StoreId == storeId).Select(x => x.CostPrice).FirstOrDefault(0);
                purchaseOrderDetail.CostPackage = purchaseOrderDetail.CostPrice * packing;
                purchaseOrderDetail.CostValue = purchaseOrderDetail.CostPackage * (purchaseOrderDetail.Quantity + purchaseOrderDetail.BonusQuantity);
                purchaseOrderDetail.LastPurchasePrice = lastPurchasePrices.Where(x => x.ItemId == purchaseOrderDetail.ItemId && x.ItemPackageId == purchaseOrderDetail.ItemPackageId).Select(x => x.PurchasePrice).FirstOrDefault(0);
            }
        }

        private async Task SavePurchaseOrderDetailTaxes(int purchaseOrderHeaderId, List<PurchaseOrderDetailDto> purchaseOrderDetails)
        {
            List<PurchaseOrderDetailTaxDto> purchaseOrderDetailTaxes = new List<PurchaseOrderDetailTaxDto>();

            foreach (var purchaseOrderDetail in purchaseOrderDetails)
            {
                foreach (var purchaseOrderDetailTax in purchaseOrderDetail.PurchaseOrderDetailTaxes)
                {
                    purchaseOrderDetailTax.PurchaseOrderDetailId = purchaseOrderDetail.PurchaseOrderDetailId;
                    purchaseOrderDetailTaxes.Add(purchaseOrderDetailTax);
                }
            }

            await _purchaseOrderDetailTaxService.SavePurchaseOrderDetailTaxes(purchaseOrderHeaderId, purchaseOrderDetailTaxes);
        }

        public async Task<ResponseDto> DeletePurchaseOrder(int purchaseOrderHeaderId)
        {
            await _menuNoteService.DeleteMenuNotes(MenuCodeData.PurchaseOrder, purchaseOrderHeaderId);
            await _purchaseOrderDetailTaxService.DeletePurchaseOrderDetailTaxes(purchaseOrderHeaderId);
            await _purchaseOrderDetailService.DeletePurchaseOrderDetails(purchaseOrderHeaderId);
            var result = await _purchaseOrderHeaderService.DeletePurchaseOrderHeader(purchaseOrderHeaderId);
            return result;
        }
    }
}
