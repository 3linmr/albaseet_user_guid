using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compound.CoreOne.Contracts.Reports.Inventory;
using Compound.CoreOne.Contracts.Reports.Shared;
using Compound.CoreOne.Models.Dtos.Reports.Inventory;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Purchases.CoreOne.Contracts;
using Shared.CoreOne.Contracts.Accounts;
using Shared.CoreOne.Contracts.Items;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Contracts.Taxes;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Compound.Service.Services.Reports.Inventory
{
	public class ItemsReorderReportService : IItemsReorderReportService
	{
		private readonly IStockTakingsReportService _stockTakingsReportService;
		private readonly IItemService _itemService;
		private readonly IItemTypeService _itemTypeService;
		private readonly IStoreService _storeService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IPurchaseInvoiceDetailService _purchaseInvoiceDetailService;
		private readonly IGeneralInventoryDocumentService _generalInventoryDocumentService;
		private readonly IItemPackageService _itemPackageService;
		private readonly IBranchService _branchService;
		private readonly IItemCategoryService _itemCategoryService;
		private readonly IItemSubCategoryService _itemSubCategoryService;
		private readonly IItemSubSectionService _itemSubSectionService;
		private readonly IItemSectionService _itemSectionService;
		private readonly IMainItemService _mainItemService;
		private readonly IVendorService _vendorService;
		private readonly IItemTaxService _itemTaxService;
		private readonly IAccountService _accountService;
		private readonly ITaxTypeService _taxTypeService;
		private readonly ITaxPercentService _taxPercentService;
		private readonly IItemReportDataService _itemReportDataService;

		public ItemsReorderReportService(IStockTakingsReportService stockTakingsReportService, IItemService itemService, IItemTypeService itemTypeService, IStoreService storeService, IHttpContextAccessor httpContextAccessor, IPurchaseInvoiceDetailService purchaseInvoiceDetailService, IGeneralInventoryDocumentService generalInventoryDocumentService, IItemPackageService itemPackageService, IBranchService branchService, IItemCategoryService itemCategoryService, IItemSubCategoryService itemSubCategoryService, IItemSubSectionService itemSubSectionService, IItemSectionService itemSectionService, IMainItemService mainItemService, IVendorService vendorService, IItemTaxService itemTaxService, IAccountService accountService, ITaxTypeService taxTypeService, ITaxPercentService taxPercentService, IItemReportDataService itemReportDataService)
		{
			_stockTakingsReportService = stockTakingsReportService;
			_itemService = itemService;
			_itemTypeService = itemTypeService;
			_storeService = storeService;
			_httpContextAccessor = httpContextAccessor;
			_purchaseInvoiceDetailService = purchaseInvoiceDetailService;
			_generalInventoryDocumentService = generalInventoryDocumentService;
			_itemPackageService = itemPackageService;
			_branchService = branchService;
			_itemCategoryService = itemCategoryService;
			_itemSubCategoryService = itemSubCategoryService;
			_itemSubSectionService = itemSubSectionService;
			_itemSectionService = itemSectionService;
			_mainItemService = mainItemService;
			_vendorService = vendorService;
			_itemTaxService = itemTaxService;
			_accountService = accountService;
			_taxTypeService = taxTypeService;
			_taxPercentService = taxPercentService;
			_itemReportDataService = itemReportDataService;
		}

		public async Task<IQueryable<ItemsReorderReportDto>> GetItemsReorderReport(List<int> storeIds)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var companyIds = await _storeService.GetAllStores().Where(x => storeIds.Contains(x.StoreId)).Select(x => x.CompanyId).ToListAsync();
			var today = DateHelper.GetDateTimeNow().Date;

			var perItemOtherTaxes = await _itemReportDataService.GetItemOtherTaxes(companyIds);
			var perItemAttributes = await _itemReportDataService.GetItemAttributes(companyIds);
			var perItemMenuNotes = await _itemReportDataService.GetItemMenuNotes(companyIds);

			//for each item choose a package.
			//If the item has purchase invoices, choose the biggest packageId used in these invoices.
			//If the item does not have any purchase invoice choose the singular item package
			var chosenPackages = from purchaseInvoiceDetail in _purchaseInvoiceDetailService.GetAll()
								 group new { purchaseInvoiceDetail.ItemPackageId } by new { purchaseInvoiceDetail.ItemId } into g
								 select new
								 {
									 ItemId = g.Key.ItemId,
									 SelectedItemPackageId = g.Max(y => y.ItemPackageId)
								 };

			var availableBalances = from generalInventoryDocument in _generalInventoryDocumentService.GetGeneralInventoryDocuments()
									group generalInventoryDocument by new { generalInventoryDocument.StoreId, generalInventoryDocument.ItemId, generalInventoryDocument.ItemPackageId } into g
									select new
									{
										StoreId = g.Key.StoreId,
										ItemId = g.Key.ItemId,
										ItemPackageId = g.Key.ItemPackageId,
										AvailableBalance = g.Sum(x => x.InQuantity - x.OutQuantity + x.PendingInQuantity - x.PendingOutQuantity + x.OpenQuantity)
									};

			return from item in _itemService.GetAll().Where(x => companyIds.Contains(x.CompanyId))
				   from itemType in _itemTypeService.GetAll().Where(x => x.ItemTypeId == item.ItemTypeId)
				   from itemCategory in _itemCategoryService.GetAll().Where(x => x.ItemCategoryId == item.ItemCategoryId).DefaultIfEmpty()
				   from itemSubCategory in _itemSubCategoryService.GetAll().Where(x => x.ItemSubCategoryId == item.ItemSubCategoryId).DefaultIfEmpty()
				   from itemSection in _itemSectionService.GetAll().Where(x => x.ItemSectionId == item.ItemSectionId).DefaultIfEmpty()
				   from itemSubSection in _itemSubSectionService.GetAll().Where(x => x.ItemSubSectionId == item.ItemSubSectionId).DefaultIfEmpty()
				   from mainItem in _mainItemService.GetAll().Where(x => x.MainItemId == item.MainItemId).DefaultIfEmpty()

				   from vendor in _vendorService.GetAll().Where(x => x.VendorId == item.VendorId).DefaultIfEmpty()

				   from salesAccount in _accountService.GetAll().Where(x => x.AccountId == item.SalesAccountId).DefaultIfEmpty()
				   from purchaseAccount in _accountService.GetAll().Where(x => x.AccountId == item.PurchaseAccountId).DefaultIfEmpty()

				   from taxType in _taxTypeService.GetAll().Where(x => x.TaxTypeId == item.TaxTypeId)
				   from vatPercent in _taxPercentService.GetAllCompanyVatPercents(today).Where(x => x.CompanyId == item.CompanyId).DefaultIfEmpty()

				   from store in _storeService.GetAll().Where(x => storeIds.Contains(x.StoreId))
				   from branch in _branchService.GetAll().Where(x => x.BranchId == store.BranchId && x.CompanyId == item.CompanyId) //make sure the store and item are in the same company

				   from chosenPackage in chosenPackages.Where(x => x.ItemId == item.ItemId).DefaultIfEmpty()
				   from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == ((int?)chosenPackage.SelectedItemPackageId ?? item.SingularPackageId))
				   from availableBalance in availableBalances.Where(x => x.ItemId == item.ItemId && x.ItemPackageId == itemPackage.ItemPackageId && x.StoreId == store.StoreId).DefaultIfEmpty()
				   where item.ReorderPointQuantity > 0 && item.ReorderPointQuantity > ((int?)availableBalance.AvailableBalance ?? 0)
				   select new ItemsReorderReportDto
				   {
					   ItemId = item.ItemId,
					   ItemCode = item.ItemCode,
					   ItemName = language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn,
					   ItemNameAr = item.ItemNameAr,
					   ItemNameEn = item.ItemNameEn,
					   ItemTypeId = itemType.ItemTypeId,
					   ItemTypeName = language == LanguageCode.Arabic ? itemType.ItemTypeNameAr : itemType.ItemTypeNameEn,

					   ItemPackageId = itemPackage.ItemPackageId, 
					   ItemPackageCode = itemPackage.ItemPackageCode,
					   ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn, 

					   StoreId = store.StoreId,
					   StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,

					   AvailableBalance = (int?)availableBalance.AvailableBalance ?? 0,
					   ReorderPointQuantity = item.ReorderPointQuantity,

					   VendorId = vendor.VendorId,
					   VendorCode = vendor.VendorCode,
					   VendorName = language == LanguageCode.Arabic ? vendor.VendorNameAr : vendor.VendorNameEn,

					   TaxTypeName = language == LanguageCode.Arabic ? taxType.TaxTypeNameAr : taxType.TaxTypeNameEn,
					   TaxValue = item.TaxTypeId == TaxTypeData.Taxable ? (decimal?)vatPercent.VatPercent ?? 0 : 0,

					   PurchasingPrice = item.PurchasingPrice,
					   ConsumerPrice = item.ConsumerPrice,
					   InternalPrice = item.InternalPrice,
					   MaxDiscountPercent = item.MaxDiscountPercent,
					   SalesAccountId = item.SalesAccountId,
					   SalesAccountName = language == LanguageCode.Arabic ? salesAccount.AccountNameAr : salesAccount.AccountNameEn,
					   PurchaseAccountId = item.PurchaseAccountId,
					   PurchaseAccountName = language == LanguageCode.Arabic ? purchaseAccount.AccountNameAr : purchaseAccount.AccountNameEn,
					   MinBuyQuantity = item.MinBuyQuantity,
					   MinSellQuantity = item.MinSellQuantity,
					   MaxBuyQuantity = item.MaxBuyQuantity,
					   MaxSellQuantity = item.MaxSellQuantity,
					   CoverageQuantity = item.CoverageQuantity,
					   IsActive = item.IsActive,
					   InActiveReasons = item.InActiveReasons,
					   NoReplenishment = item.NoReplenishment,
					   IsUnderSelling = item.IsUnderSelling,
					   IsNoStock = item.IsNoStock,
					   IsUntradeable = item.IsUntradeable,
					   IsDeficit = item.IsDeficit,
					   IsPos = item.IsPos,
					   IsOnline = item.IsOnline,
					   IsPoints = item.IsPoints,
					   IsPromoted = item.IsPromoted,
					   IsExpired = item.IsExpired,
					   IsBatched = item.IsBatched,
					   ItemLocation = item.ItemLocation,

					   ItemCategoryId = item.ItemCategoryId,
					   ItemCategoryName = itemCategory != null ? language == LanguageCode.Arabic ? itemCategory.CategoryNameAr : itemCategory.CategoryNameEn : null,
					   ItemSubCategoryId = item.ItemSubCategoryId,
					   ItemSubCategoryName = itemSubCategory != null ? language == LanguageCode.Arabic ? itemSubCategory.SubCategoryNameAr : itemSubCategory.SubCategoryNameEn : null,
					   ItemSectionId = item.ItemSectionId,
					   ItemSectionName = itemSection != null ? language == LanguageCode.Arabic ? itemSection.SectionNameAr : itemSection.SectionNameEn : null,
					   ItemSubSectionId = item.ItemSubSectionId,
					   ItemSubSectionName = itemSubSection != null ? language == LanguageCode.Arabic ? itemSubSection.SubSectionNameAr : itemSubSection.SubSectionNameEn : null,
					   MainItemId = item.MainItemId,
					   MainItemName = mainItem != null ? language == LanguageCode.Arabic ? mainItem.MainItemNameAr : mainItem.MainItemNameEn : null,

					   OtherTaxes = perItemOtherTaxes.GetValueOrDefault(item.ItemId, string.Empty),
					   ItemAttributes = perItemAttributes.GetValueOrDefault(item.ItemId, string.Empty),
					   ItemMenuNotes = perItemMenuNotes.GetValueOrDefault(item.ItemId, string.Empty),

					   CreatedAt = item.CreatedAt,
					   UserNameCreated = item.UserNameCreated,
					   ModifiedAt = item.ModifiedAt,
					   UserNameModified = item.UserNameModified,
				   };
		}
	}
}
