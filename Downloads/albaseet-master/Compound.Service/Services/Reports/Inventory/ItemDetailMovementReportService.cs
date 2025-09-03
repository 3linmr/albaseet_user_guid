using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compound.CoreOne.Contracts.Reports.Inventory;
using Compound.CoreOne.Contracts.Reports.Shared;
using Shared.CoreOne.Contracts.Menus;
using Shared.Helper.Identity;
using Microsoft.AspNetCore.Http;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Shared.CoreOne.Contracts.Items;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.StaticData;
using Compound.CoreOne.Models.Dtos.Reports.Shared;
using Compound.CoreOne.Models.Dtos.Reports.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Contracts.CostCenters;
using Shared.CoreOne.Contracts.Accounts;
using Shared.CoreOne.Contracts.Taxes;
using Shared.Helper.Logic;

namespace Compound.Service.Services.Reports.Inventory
{
	public class ItemDetailMovementReportService : IItemDetailMovementReportService
	{
		private readonly IGeneralInventoryDocumentService _generalInventoryDocumentService;
		private readonly IMenuService _menuService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IStoreService _storeService;
		private readonly IItemPackageService _itemPackageService;
		private readonly IItemService _itemService;
		private readonly IItemPackingService _itemPackingService;
		private readonly IStringLocalizer<ItemDetailMovementReportService> _localizer;
		private readonly IItemTypeService _itemTypeService;
		private readonly IItemBarCodeService _itemBarCodeService;
		private readonly IBranchService _branchService;
		private readonly IClientService _clientService;
		private readonly ISellerService _sellerService;
		private readonly ICostCenterService _costCenterService;
		private readonly IItemCategoryService _itemCategoryService;
		private readonly IItemSubCategoryService _itemSubCategoryService;
		private readonly IItemSectionService _itemSectionService;
		private readonly IItemSubSectionService _itemSubSectionService;
		private readonly IMainItemService _mainItemService;
		private readonly IAccountService _accountService;
		private readonly ITaxTypeService _taxTypeService;
		private readonly ITaxPercentService _taxPercentService;
		private readonly IVendorService _vendorService;
		private readonly IItemReportDataService _itemReportDataService;

		public ItemDetailMovementReportService(IGeneralInventoryDocumentService generalInventoryDocumentService, IMenuService menuService, IHttpContextAccessor httpContextAccessor, IStoreService storeService, IItemPackageService itemPackageService, IItemService itemService, IItemPackingService itemPackingService, IStringLocalizer<ItemDetailMovementReportService> localizer, IItemTypeService itemTypeService, IItemBarCodeService itemBarCodeService, IBranchService branchService, IClientService clientService, ISellerService sellerService, ICostCenterService costCenterService, IItemCategoryService itemCategoryService, IItemSubCategoryService itemSubCategoryService, IItemSectionService itemSectionService, IItemSubSectionService itemSubSectionService, IMainItemService mainItemService, IAccountService accountService, ITaxTypeService taxTypeService, ITaxPercentService taxPercentService, IVendorService vendorService, IItemReportDataService itemReportDataService)
		{
			_generalInventoryDocumentService = generalInventoryDocumentService;
			_menuService = menuService;
			_httpContextAccessor = httpContextAccessor;
			_storeService = storeService;
			_itemPackageService = itemPackageService;
			_itemService = itemService;
			_itemPackingService = itemPackingService;
			_localizer = localizer;
			_itemTypeService = itemTypeService;
			_itemBarCodeService = itemBarCodeService;
			_branchService = branchService;
			_clientService = clientService;
			_sellerService = sellerService;
			_costCenterService = costCenterService;
			_itemCategoryService = itemCategoryService;
			_itemSubCategoryService = itemSubCategoryService;
			_itemSectionService = itemSectionService;
			_itemSubSectionService = itemSubSectionService;
			_mainItemService = mainItemService;
			_accountService = accountService;
			_taxTypeService = taxTypeService;
			_taxPercentService = taxPercentService;
			_vendorService = vendorService;
			_itemReportDataService = itemReportDataService;
		}

		public async Task<List<ItemDetailMovementReportDto>> GetItemDetailMovementReport(int itemId, List<int> storeIds, DateTime? fromDate, DateTime? toDate, bool isGrouped)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var today = DateHelper.GetDateTimeNow();

			var generalInventories = isGrouped ? 
				GetGeneralInventoryDocumentsWithMenuDataGroupedOnlyItemAndPackage().Where(x => x.ItemId == itemId && storeIds.Contains(x.StoreId)) :
				GetGeneralInventoryDocumentsWithMenuDataGrouped().Where(x => x.ItemId == itemId && storeIds.Contains(x.StoreId));

			var generalInventoriesWithoutGroup = GetGeneralInventoryDocuments(isGrouped);

			//packages for the selected itemId that don't have any movements
			//in the storeIds should still show with opening and closing balances
			//in the report but with quantities zero

			var otherTaxes = await _itemReportDataService.GetItemOtherTaxesByItemId(itemId);
			var attributes = await _itemReportDataService.GetItemAttributesByItemId(itemId);
			var menuNotes = await _itemReportDataService.GetItemMenuNotesByItemId(itemId);

			var openingBalances = await (from item in _itemService.GetAll().Where(x => x.ItemId == itemId && x.ItemTypeId != ItemTypeData.Service)
										 from itemType in _itemTypeService.GetAll().Where(x => x.ItemTypeId == item.ItemTypeId)
							             from itemCategory in _itemCategoryService.GetAll().Where(x => x.ItemCategoryId == item.ItemCategoryId).DefaultIfEmpty()
							             from itemSubCategory in _itemSubCategoryService.GetAll().Where(x => x.ItemSubCategoryId == item.ItemSubCategoryId).DefaultIfEmpty()
							             from itemSection in _itemSectionService.GetAll().Where(x => x.ItemSectionId == item.ItemSectionId).DefaultIfEmpty()
							             from itemSubSection in _itemSubSectionService.GetAll().Where(x => x.ItemSubSectionId == item.ItemSubSectionId).DefaultIfEmpty()
							             from mainItem in _mainItemService.GetAll().Where(x => x.MainItemId == item.MainItemId).DefaultIfEmpty()

						                 from vendor in _vendorService.GetAll().Where(x => x.VendorId == item.VendorId).DefaultIfEmpty()

						                 from taxType in _taxTypeService.GetAll().Where(x => x.TaxTypeId == item.TaxTypeId)
						                 from vatPercent in _taxPercentService.GetAllCompanyVatPercents(today).Where(x => x.CompanyId == item.CompanyId).DefaultIfEmpty()

						                 from salesAccount in _accountService.GetAll().Where(x => x.AccountId == item.SalesAccountId).DefaultIfEmpty()
						                 from purchaseAccount in _accountService.GetAll().Where(x => x.AccountId == item.PurchaseAccountId).DefaultIfEmpty()

				                         from store in _storeService.GetAll().Where(x => storeIds.Contains(x.StoreId))
				                         from branch in _branchService.GetAll().Where(x => x.BranchId == store.BranchId && x.CompanyId == item.CompanyId) //store and item must be from same company

				                         from itemBarCode in _itemBarCodeService.GetAll().Where(x => x.ItemId == item.ItemId)
				                         from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == itemBarCode.FromPackageId)

										 from generalInventory in generalInventoriesWithoutGroup.Where(x => x.ItemId == item.ItemId && x.StoreId == store.StoreId && x.ItemPackageId == itemPackage.ItemPackageId).DefaultIfEmpty()
										 group new { generalInventory } by new { store.StoreId, store.StoreNameAr, store.StoreNameEn, generalInventory.ItemNote, item.ItemId, item.ItemCode, item.ItemNameAr, item.ItemNameEn, itemType.ItemTypeId, itemType.ItemTypeNameAr, itemType.ItemTypeNameEn, itemPackage.ItemPackageId, itemPackage.ItemPackageCode, itemPackage.PackageNameAr, itemPackage.PackageNameEn, generalInventory.ExpireDate, generalInventory.BatchNumber,
							                 ReorderPointQuantity = item.ReorderPointQuantity,

 							                 TaxTypeName = language == LanguageCode.Arabic ? taxType.TaxTypeNameAr : taxType.TaxTypeNameEn,
							                 TaxValue = item.TaxTypeId == TaxTypeData.Taxable ? (decimal?)vatPercent.VatPercent ?? 0 : 0,

							                 VendorId = vendor.VendorId,
							                 VendorCode = vendor.VendorCode,
							                 VendorName = language == LanguageCode.Arabic ? vendor.VendorNameAr : vendor.VendorNameEn,

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
										 } into g
										 select new ItemDetailMovementReportDto
										 {
										 	 HeaderId = null,
										 	 MenuCode = null,
										 	 MenuName = _localizer["OpeningBalance"],
										 	 DocumentFullCode = null,
										 	 StoreId = g.Key.StoreId,
										 	 StoreName = language == LanguageCode.Arabic ? g.Key.StoreNameAr : g.Key.StoreNameEn,
										 	 ItemId = g.Key.ItemId,
											 ItemCode = g.Key.ItemCode,
										 	 ItemName = g.Key.ItemTypeId == ItemTypeData.Note ? g.Key.ItemNote : (language == LanguageCode.Arabic ? g.Key.ItemNameAr : g.Key.ItemNameEn),
											 ItemTypeId = g.Key.ItemTypeId,
											 ItemTypeName = language == LanguageCode.Arabic ? g.Key.ItemTypeNameAr : g.Key.ItemTypeNameEn,
										 	 ItemPackageId = g.Key.ItemPackageId,
											 ItemPackageCode = g.Key.ItemPackageCode,
										 	 ItemPackageName = language == LanguageCode.Arabic ? g.Key.PackageNameAr : g.Key.PackageNameEn,
										 	 ExpireDate = g.Key.ExpireDate,
										 	 BatchNumber = g.Key.BatchNumber,
										 	 DocumentDate = null,
										 	 EntryDate = null,
										 	 BarCode = null,
										 	 CostValue = null,
										 	 Price = null,
										 	 InQuantity = null,
										 	 OutQuantity = null,
										 	 PendingOutQuantity = null,
										 	 ReservedQuantity = null,
										 	 CurrentBalance = g.Sum(x => fromDate != null && x.generalInventory.DocumentDate < fromDate ?
											 	x.generalInventory.InQuantity + x.generalInventory.OpenQuantity - x.generalInventory.OutQuantity :
											 	0),
										 	 AvailableBalance = g.Sum(x => fromDate != null && x.generalInventory.DocumentDate < fromDate ?
											 	x.generalInventory.InQuantity + x.generalInventory.OpenQuantity - x.generalInventory.OutQuantity - x.generalInventory.PendingOutQuantity + x.generalInventory.PendingInQuantity :
											 	0),

											 ReorderPointQuantity = g.Key.ReorderPointQuantity,

											 TaxTypeName = g.Key.TaxTypeName,
											 TaxValue = g.Key.TaxValue,

											 VendorId = g.Key.VendorId,
											 VendorCode = g.Key.VendorCode,
											 VendorName = g.Key.VendorName,

											 PurchasingPrice = g.Key.PurchasingPrice,
											 ConsumerPrice = g.Key.ConsumerPrice,
											 InternalPrice = g.Key.InternalPrice,
											 MaxDiscountPercent = g.Key.MaxDiscountPercent,
											 SalesAccountId = g.Key.SalesAccountId,
											 SalesAccountName = g.Key.SalesAccountName,
											 PurchaseAccountId = g.Key.PurchaseAccountId,
											 PurchaseAccountName = g.Key.PurchaseAccountName,
											 MinBuyQuantity = g.Key.MinBuyQuantity,
											 MinSellQuantity = g.Key.MinSellQuantity,
											 MaxBuyQuantity = g.Key.MaxBuyQuantity,
											 MaxSellQuantity = g.Key.MaxSellQuantity,
											 CoverageQuantity = g.Key.CoverageQuantity,
											 IsActive = g.Key.IsActive,
											 InActiveReasons = g.Key.InActiveReasons,
											 NoReplenishment = g.Key.NoReplenishment,
											 IsUnderSelling = g.Key.IsUnderSelling,
											 IsNoStock = g.Key.IsNoStock,
											 IsUntradeable = g.Key.IsUntradeable,
											 IsDeficit = g.Key.IsDeficit,
											 IsPos = g.Key.IsPos,
											 IsOnline = g.Key.IsOnline,
											 IsPoints = g.Key.IsPoints,
											 IsPromoted = g.Key.IsPromoted,
											 IsExpired = g.Key.IsExpired,
											 IsBatched = g.Key.IsBatched,
											 ItemLocation = g.Key.ItemLocation,

											 ItemCategoryId = g.Key.ItemCategoryId,
											 ItemCategoryName = g.Key.ItemCategoryName,
											 ItemSubCategoryId = g.Key.ItemSubCategoryId,
											 ItemSubCategoryName = g.Key.ItemSubCategoryName,
											 ItemSectionId = g.Key.ItemSectionId,
											 ItemSectionName = g.Key.ItemSectionName,
											 ItemSubSectionId = g.Key.ItemSubSectionId,
											 ItemSubSectionName = g.Key.ItemSubSectionName,
											 MainItemId = g.Key.MainItemId,
											 MainItemName = g.Key.MainItemName,

											 OtherTaxes = otherTaxes,
											 ItemAttributes = attributes,
											 ItemMenuNotes = menuNotes,
										 }).ToDictionaryAsync(x => new { x.StoreId, x.ItemId, x.ItemName, x.ItemPackageId, x.ExpireDate, x.BatchNumber }); //must include itemName in group to handle itemNotes

			var reportMiddle = await (from generalInventory in generalInventories.Where
								         (x =>
								         	(fromDate == null || x.DocumentDate >= fromDate) &&
								         	(toDate == null || x.DocumentDate <= toDate)
								         )
									  from menu in _menuService.GetAll().Where(x => x.MenuCode == generalInventory.MenuCode).DefaultIfEmpty()
									  from store in _storeService.GetAll().Where(x => x.StoreId == generalInventory.StoreId)
									  from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == generalInventory.ItemPackageId)

									  from item in _itemService.GetAll().Where(x => x.ItemId == generalInventory.ItemId)
									  from itemType in _itemTypeService.GetAll().Where(x => x.ItemTypeId == item.ItemTypeId)
							          from itemCategory in _itemCategoryService.GetAll().Where(x => x.ItemCategoryId == item.ItemCategoryId).DefaultIfEmpty()
							          from itemSubCategory in _itemSubCategoryService.GetAll().Where(x => x.ItemSubCategoryId == item.ItemSubCategoryId).DefaultIfEmpty()
							          from itemSection in _itemSectionService.GetAll().Where(x => x.ItemSectionId == item.ItemSectionId).DefaultIfEmpty()
							          from itemSubSection in _itemSubSectionService.GetAll().Where(x => x.ItemSubSectionId == item.ItemSubSectionId).DefaultIfEmpty()
							          from mainItem in _mainItemService.GetAll().Where(x => x.MainItemId == item.MainItemId).DefaultIfEmpty()

						              from vendor in _vendorService.GetAll().Where(x => x.VendorId == item.VendorId).DefaultIfEmpty()

						              from taxType in _taxTypeService.GetAll().Where(x => x.TaxTypeId == item.TaxTypeId)
						              from vatPercent in _taxPercentService.GetAllCompanyVatPercents(today).Where(x => x.CompanyId == item.CompanyId).DefaultIfEmpty()

						              from salesAccount in _accountService.GetAll().Where(x => x.AccountId == item.SalesAccountId).DefaultIfEmpty()
						              from purchaseAccount in _accountService.GetAll().Where(x => x.AccountId == item.PurchaseAccountId).DefaultIfEmpty()

									  from client in _clientService.GetAll().Where(x => x.ClientId == generalInventory.ClientId).DefaultIfEmpty()
									  from seller in _sellerService.GetAll().Where(x => x.SellerId == generalInventory.SellerId).DefaultIfEmpty()
									  from costCenter in _costCenterService.GetAll().Where(x => x.CostCenterId == generalInventory.CostCenterId).DefaultIfEmpty()
									  where item.ItemTypeId != ItemTypeData.Service
									  orderby generalInventory.DocumentDate, generalInventory.EntryDate, generalInventory.CreatedAt, generalInventory.HeaderId, generalInventory.MenuCode
									  select new ItemDetailMovementReportDto
									  {
										  HeaderId = generalInventory.HeaderId,
										  MenuCode = generalInventory.MenuCode,
										  MenuName = language == LanguageCode.Arabic ? menu.MenuNameAr : menu.MenuNameEn,
										  DocumentFullCode = generalInventory.DocumentFullCode,
										  ClientId = client.ClientId,
										  ClientCode = client.ClientCode,
										  ClientName = language == LanguageCode.Arabic ? client.ClientNameAr : client.ClientNameEn,
										  SellerId = seller.SellerId,
										  SellerCode = seller.SellerCode,
										  SellerName = language == LanguageCode.Arabic ? seller.SellerNameAr : seller.SellerNameEn,
										  CostCenterId = costCenter.CostCenterId,
										  CostCenterCode = costCenter.CostCenterCode,
										  CostCenterName = language == LanguageCode.Arabic ? costCenter.CostCenterNameAr : costCenter.CostCenterNameEn,
										  StoreId = generalInventory.StoreId,
										  StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
										  ItemId = generalInventory.ItemId,
										  ItemCode = item.ItemCode,
										  ItemName = item.ItemTypeId == ItemTypeData.Note ? generalInventory.ItemNote : (language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn),
										  ItemTypeId = item.ItemTypeId,
										  ItemTypeName = language == LanguageCode.Arabic ? itemType.ItemTypeNameAr : itemType.ItemTypeNameEn,
										  ItemPackageId = generalInventory.ItemPackageId,
										  ItemPackageCode = itemPackage.ItemPackageCode,
										  ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
										  ExpireDate = generalInventory.ExpireDate,
										  BatchNumber = generalInventory.BatchNumber,
										  DocumentDate = generalInventory.DocumentDate,
										  EntryDate = generalInventory.EntryDate,
										  BarCode = generalInventory.BarCode,
										  CostValue = generalInventory.CostPackage * generalInventory.StoredQuantity,
										  Price = generalInventory.Price,
										  Reference = generalInventory.Reference,
										  RemarksAr = generalInventory.RemarksAr,
										  RemarksEn = generalInventory.RemarksEn,
										  Notes = generalInventory.Notes,
										  InQuantity = generalInventory.OpenQuantity + generalInventory.InQuantity + generalInventory.PendingInQuantity,
										  OutQuantity = generalInventory.OutQuantity,
										  PendingOutQuantity = generalInventory.PendingOutQuantity,
										  ReservedQuantity = generalInventory.ReservedQuantity,
										  CurrentBalance = 0,
										  AvailableBalance = 0,

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
  
										  OtherTaxes = otherTaxes,
										  ItemAttributes = attributes,
										  ItemMenuNotes = menuNotes,
  
  										  CreatedAt = generalInventory.CreatedAt,
										  UserNameCreated = generalInventory.UserNameCreated,
										  ModifiedAt = generalInventory.ModifiedAt,
										  UserNameModified = generalInventory.UserNameModified,
									  }).ToListAsync();

			var reportMiddleGroups = reportMiddle.GroupBy(x => new { x.StoreId, x.ItemId, x.ItemName, x.ItemPackageId, x.ExpireDate, x.BatchNumber }).ToDictionary(x => x.Key);

			var closingBalances = await (from item in _itemService.GetAll().Where(x => x.ItemId == itemId && x.ItemTypeId != ItemTypeData.Service)
										 from itemType in _itemTypeService.GetAll().Where(x => x.ItemTypeId == item.ItemTypeId)
							             from itemCategory in _itemCategoryService.GetAll().Where(x => x.ItemCategoryId == item.ItemCategoryId).DefaultIfEmpty()
							             from itemSubCategory in _itemSubCategoryService.GetAll().Where(x => x.ItemSubCategoryId == item.ItemSubCategoryId).DefaultIfEmpty()
							             from itemSection in _itemSectionService.GetAll().Where(x => x.ItemSectionId == item.ItemSectionId).DefaultIfEmpty()
							             from itemSubSection in _itemSubSectionService.GetAll().Where(x => x.ItemSubSectionId == item.ItemSubSectionId).DefaultIfEmpty()
							             from mainItem in _mainItemService.GetAll().Where(x => x.MainItemId == item.MainItemId).DefaultIfEmpty()

						                 from vendor in _vendorService.GetAll().Where(x => x.VendorId == item.VendorId).DefaultIfEmpty()

						                 from taxType in _taxTypeService.GetAll().Where(x => x.TaxTypeId == item.TaxTypeId)
						                 from vatPercent in _taxPercentService.GetAllCompanyVatPercents(today).Where(x => x.CompanyId == item.CompanyId).DefaultIfEmpty()

						                 from salesAccount in _accountService.GetAll().Where(x => x.AccountId == item.SalesAccountId).DefaultIfEmpty()
						                 from purchaseAccount in _accountService.GetAll().Where(x => x.AccountId == item.PurchaseAccountId).DefaultIfEmpty()

				                         from store in _storeService.GetAll().Where(x => storeIds.Contains(x.StoreId))
				                         from branch in _branchService.GetAll().Where(x => x.BranchId == store.BranchId && x.CompanyId == item.CompanyId) //store and item must be from same company

				                         from itemBarCode in _itemBarCodeService.GetAll().Where(x => x.ItemId == item.ItemId)
				                         from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == itemBarCode.FromPackageId)

										 from generalInventory in generalInventoriesWithoutGroup.Where(x => x.ItemId == item.ItemId && x.StoreId == store.StoreId && x.ItemPackageId == itemPackage.ItemPackageId).DefaultIfEmpty()
										 group new { generalInventory } by new { store.StoreId, store.StoreNameAr, store.StoreNameEn, generalInventory.ItemNote, item.ItemId, item.ItemCode, item.ItemNameAr, item.ItemNameEn, itemType.ItemTypeId, itemType.ItemTypeNameAr, itemType.ItemTypeNameEn, itemPackage.ItemPackageId, itemPackage.ItemPackageCode, itemPackage.PackageNameAr, itemPackage.PackageNameEn, generalInventory.ExpireDate, generalInventory.BatchNumber,
							                 ReorderPointQuantity = item.ReorderPointQuantity,

 							                 TaxTypeName = language == LanguageCode.Arabic ? taxType.TaxTypeNameAr : taxType.TaxTypeNameEn,
							                 TaxValue = item.TaxTypeId == TaxTypeData.Taxable ? (decimal?)vatPercent.VatPercent ?? 0 : 0,

							                 VendorId = vendor.VendorId,
							                 VendorCode = vendor.VendorCode,
							                 VendorName = language == LanguageCode.Arabic ? vendor.VendorNameAr : vendor.VendorNameEn,

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
										 } into g
										 select new ItemDetailMovementReportDto
										 {
										 	HeaderId = null,
										 	MenuCode = null,
										 	MenuName = _localizer["ClosingBalance"],
										 	DocumentFullCode = null,
										 	StoreId = g.Key.StoreId,
										 	StoreName = language == LanguageCode.Arabic ? g.Key.StoreNameAr : g.Key.StoreNameEn,
										 	ItemId = g.Key.ItemId,
											ItemCode = g.Key.ItemCode,
										 	ItemName = g.Key.ItemTypeId == ItemTypeData.Note ? g.Key.ItemNote : (language == LanguageCode.Arabic ? g.Key.ItemNameAr : g.Key.ItemNameEn),
											ItemTypeId = g.Key.ItemTypeId,
											ItemTypeName = language == LanguageCode.Arabic ? g.Key.ItemTypeNameAr : g.Key.ItemTypeNameEn,
										 	ItemPackageId = g.Key.ItemPackageId,
											ItemPackageCode = g.Key.ItemPackageCode,
										 	ItemPackageName = language == LanguageCode.Arabic ? g.Key.PackageNameAr : g.Key.PackageNameEn,
										 	ExpireDate = g.Key.ExpireDate,
										 	BatchNumber = g.Key.BatchNumber,
										 	DocumentDate = null,
										 	EntryDate = null,
										 	BarCode = null,
										 	CostValue = null,
										 	Price = null,
										 	InQuantity = null,
										 	OutQuantity = null,
										 	PendingOutQuantity = null,
										 	ReservedQuantity = null,
										 	CurrentBalance = g.Sum(x => toDate == null || x.generalInventory.DocumentDate <= toDate ? 
												x.generalInventory.InQuantity + x.generalInventory.OpenQuantity - x.generalInventory.OutQuantity :
												0),
										 	AvailableBalance = g.Sum(x => toDate == null || x.generalInventory.DocumentDate <= toDate ? 
												x.generalInventory.InQuantity + x.generalInventory.OpenQuantity - x.generalInventory.OutQuantity - x.generalInventory.PendingOutQuantity + x.generalInventory.PendingInQuantity :
												0),

											ReorderPointQuantity = g.Key.ReorderPointQuantity,

 							                TaxTypeName = g.Key.TaxTypeName,
							                TaxValue = g.Key.TaxValue,

							                VendorId = g.Key.VendorId,
							                VendorCode = g.Key.VendorCode,
							                VendorName = g.Key.VendorName,

							                PurchasingPrice = g.Key.PurchasingPrice,
							                ConsumerPrice = g.Key.ConsumerPrice,
							                InternalPrice = g.Key.InternalPrice,
							                MaxDiscountPercent = g.Key.MaxDiscountPercent,
							                SalesAccountId = g.Key.SalesAccountId,
  							                SalesAccountName = g.Key.SalesAccountName,
							                PurchaseAccountId = g.Key.PurchaseAccountId,
							                PurchaseAccountName = g.Key.PurchaseAccountName,
							                MinBuyQuantity = g.Key.MinBuyQuantity,
							                MinSellQuantity = g.Key.MinSellQuantity,
							                MaxBuyQuantity = g.Key.MaxBuyQuantity,
							                MaxSellQuantity = g.Key.MaxSellQuantity,
							                CoverageQuantity = g.Key.CoverageQuantity,
							                IsActive = g.Key.IsActive,
							                InActiveReasons = g.Key.InActiveReasons,
							                NoReplenishment = g.Key.NoReplenishment,
							                IsUnderSelling = g.Key.IsUnderSelling,
							                IsNoStock = g.Key.IsNoStock,
							                IsUntradeable = g.Key.IsUntradeable,
							                IsDeficit = g.Key.IsDeficit,
							                IsPos = g.Key.IsPos,
							                IsOnline = g.Key.IsOnline,
							                IsPoints = g.Key.IsPoints,
							                IsPromoted = g.Key.IsPromoted,
							                IsExpired = g.Key.IsExpired,
							                IsBatched = g.Key.IsBatched,
							                ItemLocation = g.Key.ItemLocation,

							                ItemCategoryId = g.Key.ItemCategoryId,
							                ItemCategoryName = g.Key.ItemCategoryName,
							                ItemSubCategoryId = g.Key.ItemSubCategoryId,
							                ItemSubCategoryName = g.Key.ItemSubCategoryName,
							                ItemSectionId = g.Key.ItemSectionId,
							                ItemSectionName = g.Key.ItemSectionName,
							                ItemSubSectionId = g.Key.ItemSubSectionId,
							                ItemSubSectionName = g.Key.ItemSubSectionName,
							                MainItemId = g.Key.MainItemId,
							                MainItemName = g.Key.MainItemName,

											OtherTaxes = otherTaxes,
											ItemAttributes = attributes,
											ItemMenuNotes = menuNotes,
										 }).ToDictionaryAsync(x => new { x.StoreId, x.ItemId, x.ItemName, x.ItemPackageId, x.ExpireDate, x.BatchNumber });

			var resultList = new List<ItemDetailMovementReportDto>();

			foreach (var openingBalance in openingBalances)
			{
				resultList.Add(openingBalance.Value);

				var reportMiddleGroup = reportMiddleGroups!.GetValueOrDefault(openingBalance.Key, null);
				if (reportMiddleGroup != null)
				{
					var runningCurrentBalance = openingBalance.Value.CurrentBalance;
					var runningAvailableBalance = openingBalance.Value.AvailableBalance;
					foreach (var reportMiddleElement in reportMiddleGroup)
					{
						runningCurrentBalance += (decimal)reportMiddleElement.InQuantity! - (decimal)reportMiddleElement.OutQuantity!;
						runningAvailableBalance += (decimal)reportMiddleElement.InQuantity! - (decimal)reportMiddleElement.OutQuantity! - (decimal)reportMiddleElement.PendingOutQuantity!;
						reportMiddleElement.CurrentBalance = runningCurrentBalance;
						reportMiddleElement.AvailableBalance = runningAvailableBalance;
					}
					resultList.AddRange(reportMiddleGroup);
				}

				var closingBalance = closingBalances[openingBalance.Key];
				resultList.Add(closingBalance);
			}

			var serial = 0;
			foreach (var row in resultList) { 
				row.Serial = serial++;
			}

			return resultList;
		}

		private IQueryable<GeneralInventoryDocumentWithMenuCodeDto> GetGeneralInventoryDocumentsWithMenuDataGrouped()
		{
			return from generalInventory in _generalInventoryDocumentService.GetGeneralInventoryDocumentsWithMenuData()
				   group generalInventory by new
				   {
					   //Header Data
					   generalInventory.HeaderId,
					   generalInventory.MenuCode,
					   generalInventory.DocumentFullCode,
					   generalInventory.ClientId,
					   generalInventory.SellerId,
					   generalInventory.StoreId,
					   generalInventory.DocumentDate,
					   generalInventory.EntryDate,
					   generalInventory.Reference,
					   generalInventory.RemarksAr,
					   generalInventory.RemarksEn,
					   generalInventory.CreatedAt,
					   generalInventory.UserNameCreated,
					   generalInventory.ModifiedAt,
					   generalInventory.UserNameModified,
					   //Detail Data
					   generalInventory.ItemId,
					   generalInventory.ItemNote,
					   generalInventory.ItemPackageId,
					   generalInventory.ExpireDate,
					   generalInventory.BatchNumber,
					   generalInventory.BarCode,
					   generalInventory.Notes,
					   generalInventory.Price,
					   generalInventory.CostPackage,
					   generalInventory.CostCenterId,
				   } into g
				   select new GeneralInventoryDocumentWithMenuCodeDto
				   {
					   HeaderId = g.Key.HeaderId,
					   MenuCode = g.Key.MenuCode,
					   DocumentFullCode = g.Key.DocumentFullCode,
					   ClientId = g.Key.ClientId,
					   SellerId = g.Key.SellerId,
					   CostCenterId = g.Key.CostCenterId,
					   StoreId = g.Key.StoreId,
					   ItemId = g.Key.ItemId,
					   ItemNote = g.Key.ItemNote,
					   ItemPackageId = g.Key.ItemPackageId,
					   ExpireDate = g.Key.ExpireDate,
					   BatchNumber = g.Key.BatchNumber,
					   DocumentDate = g.Key.DocumentDate,
					   EntryDate = g.Key.EntryDate,
					   BarCode = g.Key.BarCode,
					   Reference = g.Key.Reference,
					   RemarksAr = g.Key.RemarksAr,
					   RemarksEn = g.Key.RemarksEn,
					   Notes = g.Key.Notes,
					   StoredQuantity = g.Sum(x => x.StoredQuantity),
					   OpenQuantity = g.Sum(x => x.OpenQuantity),
					   InQuantity = g.Sum(x => x.InQuantity),
					   OutQuantity = g.Sum(x => x.OutQuantity),
					   PendingInQuantity = g.Sum(x => x.PendingInQuantity),
					   PendingOutQuantity = g.Sum(x => x.PendingOutQuantity),
					   ReservedQuantity = g.Sum(x => x.ReservedQuantity),
					   CostPackage = g.Key.CostPackage,
					   Price = g.Key.Price,
					   CreatedAt = g.Key.CreatedAt,
					   UserNameCreated = g.Key.UserNameCreated,
					   ModifiedAt = g.Key.ModifiedAt,
					   UserNameModified = g.Key.UserNameModified,
				   };
		}

		private IQueryable<GeneralInventoryDocumentWithMenuCodeDto> GetGeneralInventoryDocumentsWithMenuDataGroupedOnlyItemAndPackage()
		{
			return from generalInventory in _generalInventoryDocumentService.GetGeneralInventoryDocumentsWithMenuData()
				   group generalInventory by new
				   {
					   //Header Data
					   generalInventory.HeaderId,
					   generalInventory.MenuCode,
					   generalInventory.DocumentFullCode,
					   generalInventory.ClientId,
					   generalInventory.SellerId,
					   generalInventory.StoreId,
					   generalInventory.DocumentDate,
					   generalInventory.EntryDate,
					   generalInventory.Reference,
					   generalInventory.RemarksAr,
					   generalInventory.RemarksEn,
					   generalInventory.CreatedAt,
					   generalInventory.UserNameCreated,
					   generalInventory.ModifiedAt,
					   generalInventory.UserNameModified,
					   //Detail Data
					   generalInventory.ItemId,
					   generalInventory.ItemNote,
					   generalInventory.ItemPackageId,
				   } into g
				   select new GeneralInventoryDocumentWithMenuCodeDto
				   {
					   HeaderId = g.Key.HeaderId,
					   MenuCode = g.Key.MenuCode,
					   DocumentFullCode = g.Key.DocumentFullCode,
					   ClientId = g.Key.ClientId,
					   SellerId = g.Key.SellerId,
					   CostCenterId = null,
					   StoreId = g.Key.StoreId,
					   ItemId = g.Key.ItemId,
					   ItemNote = g.Key.ItemNote,
					   ItemPackageId = g.Key.ItemPackageId,
					   ExpireDate = null,
					   BatchNumber = null,
					   DocumentDate = g.Key.DocumentDate,
					   EntryDate = g.Key.EntryDate,
					   Reference = g.Key.Reference,
					   RemarksAr = g.Key.RemarksAr,
					   RemarksEn = g.Key.RemarksEn,
					   Notes = null,
					   StoredQuantity = g.Sum(x => x.StoredQuantity),
					   OpenQuantity = g.Sum(x => x.OpenQuantity),
					   InQuantity = g.Sum(x => x.InQuantity),
					   OutQuantity = g.Sum(x => x.OutQuantity),
					   PendingInQuantity = g.Sum(x => x.PendingInQuantity),
					   PendingOutQuantity = g.Sum(x => x.PendingOutQuantity),
					   ReservedQuantity = g.Sum(x => x.ReservedQuantity),
					   CostPackage = null,
					   Price = null,
					   CreatedAt = g.Key.CreatedAt,
					   UserNameCreated = g.Key.UserNameCreated,
					   ModifiedAt = g.Key.ModifiedAt,
					   UserNameModified = g.Key.UserNameModified,
				   };
		}

		private IQueryable<GeneralInventoryDocumentDto> GetGeneralInventoryDocuments(bool isGrouped)
		{
			return from generalInventory in _generalInventoryDocumentService.GetGeneralInventoryDocuments()
				   select new GeneralInventoryDocumentDto
				   {
					   StoreId = generalInventory.StoreId,
					   ItemId = generalInventory.ItemId,
					   ItemNote = generalInventory.ItemNote,
					   ItemPackageId = generalInventory.ItemPackageId,
					   ExpireDate = isGrouped ? null : generalInventory.ExpireDate,
					   BatchNumber = isGrouped ? null : generalInventory.BatchNumber,
					   DocumentDate = generalInventory.DocumentDate,
					   OpenQuantity = generalInventory.OpenQuantity,
					   InQuantity = generalInventory.InQuantity,
					   OutQuantity = generalInventory.OutQuantity,
					   PendingInQuantity = generalInventory.PendingInQuantity,
					   PendingOutQuantity = generalInventory.PendingOutQuantity,
					   ReservedQuantity = generalInventory.ReservedQuantity,
				   };
		}
	}
}
