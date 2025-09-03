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
using Shared.CoreOne.Contracts.Inventory;
using Shared.CoreOne.Contracts.Items;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne.Contracts.Accounts;

namespace Compound.Service.Services.Reports.Inventory
{
	public class ItemsMovementReportService : IItemsMovementReportService
	{
		private readonly IGeneralInventoryDocumentService _generalInventoryDocumentService;
		private readonly IMenuService _menuService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IStoreService _storeService;
		private readonly IItemPackageService _itemPackageService;
		private readonly IItemService _itemService;
		private readonly IItemTypeService _itemTypeService;
		private readonly IItemPackingService _itemPackingService;
		private readonly IItemCostService _itemCostService;
		private readonly IClientService _clientService;
		private readonly IItemCategoryService _itemCategoryService;
		private readonly IItemSubCategoryService _itemSubCategoryService;
		private readonly IItemSectionService _itemSectionService;
		private readonly IItemSubSectionService _itemSubSectionService;
		private readonly IMainItemService _mainItemService;
		private readonly IItemAttributeService _itemAttributeService;
		private readonly IAccountService _accountService;
		private readonly IItemReportDataService _itemReportDataService;

		public ItemsMovementReportService(IGeneralInventoryDocumentService generalInventoryDocumentService, IMenuService menuService, IHttpContextAccessor httpContextAccessor, IStoreService storeService, IItemPackageService itemPackageService, IItemService itemService, IItemTypeService itemTypeService, IItemPackingService itemPackingService, IItemCostService itemCostService, IClientService clientService, IItemCategoryService itemCategoryService, IItemSubCategoryService itemSubCategoryService, IItemSectionService itemSectionService, IItemSubSectionService itemSubSectionService, IMainItemService mainItemService, IItemAttributeService itemAttributeService, IAccountService accountService, IItemReportDataService itemReportDataService)
		{
			_generalInventoryDocumentService = generalInventoryDocumentService;
			_menuService = menuService;
			_httpContextAccessor = httpContextAccessor;
			_storeService = storeService;
			_itemPackageService = itemPackageService;
			_itemService = itemService;
			_itemTypeService = itemTypeService;
			_itemPackingService = itemPackingService;
			_itemCostService = itemCostService;
			_clientService = clientService;
			_itemCategoryService = itemCategoryService;
			_itemSubCategoryService = itemSubCategoryService;
			_itemSectionService = itemSectionService;
			_itemSubSectionService = itemSubSectionService;
			_mainItemService = mainItemService;
			_itemAttributeService = itemAttributeService;
			_accountService = accountService;
			_itemReportDataService = itemReportDataService;
		}

		public async Task<IQueryable<ItemsMovementReportDto>> GetItemsMovementReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate, List<int> menuCodes)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var companyIds = await _storeService.GetAllStores().Where(x => storeIds.Contains(x.StoreId)).Select(x => x.CompanyId).ToListAsync();
			var itemIds = await _itemService.GetAll().Where(x => companyIds.Contains(x.CompanyId)).Select(x => x.ItemId).ToListAsync();

			var perItemOtherTaxes = await _itemReportDataService.GetItemOtherTaxes(companyIds);
			var perItemAttributes = await _itemReportDataService.GetItemAttributes(companyIds);
			var perItemMenuNotes = await _itemReportDataService.GetItemMenuNotes(companyIds);

			var report = from generalInventory in GetGeneralInventoryDocumentsWithMenuDataGrouped().Where(x => 
				      			storeIds.Contains(x.StoreId) &&
				      			(fromDate == null || x.DocumentDate >= fromDate) &&
				      			(toDate == null || x.DocumentDate <= toDate) &&
				      			(menuCodes.Count == 0 || menuCodes.Contains(x.MenuCode))
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
				         from salesAccount in _accountService.GetAll().Where(x => x.AccountId == item.SalesAccountId).DefaultIfEmpty()
				         from purchaseAccount in _accountService.GetAll().Where(x => x.AccountId == item.PurchaseAccountId).DefaultIfEmpty()
				         from itemPacking in _itemPackingService.GetAll().Where(x => x.ItemId == item.ItemId && x.FromPackageId == itemPackage.ItemPackageId && x.ToPackageId == item.SingularPackageId).DefaultIfEmpty()
				         from itemCost in _itemCostService.GetAll().Where(x => x.ItemId == item.ItemId && x.StoreId == store.StoreId).DefaultIfEmpty()
				         from client in _clientService.GetAll().Where(x => x.ClientId == generalInventory.ClientId).DefaultIfEmpty()
				         where item.ItemTypeId != ItemTypeData.Service
				         orderby generalInventory.DocumentDate, generalInventory.EntryDate, generalInventory.MenuCode, generalInventory.HeaderId
				         select new ItemsMovementReportDto
				         {
				      	      HeaderId = generalInventory.HeaderId,
				      	      MenuCode = generalInventory.MenuCode,
				      	      MenuName = language == LanguageCode.Arabic ? menu.MenuNameAr : menu.MenuNameEn,
				      	      DocumentFullCode = generalInventory.DocumentFullCode,
				      	      StoreId = generalInventory.StoreId,
				      	      StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
				      	      ItemId = generalInventory.ItemId,
				      	      ItemCode = item.ItemCode,
				      	      ItemName = item.ItemTypeId == ItemTypeData.Note ? generalInventory.ItemNote : (language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn),
				      	      ItemNameAr = item.ItemNameAr,
				      	      ItemNameEn = item.ItemNameEn,
				      	      ItemTypeId = itemType.ItemTypeId,
				      	      ItemTypeName = language == LanguageCode.Arabic ? itemType.ItemTypeNameAr : itemType.ItemTypeNameEn,
				      	      ItemPackageId = generalInventory.ItemPackageId,
				      	      ItemPackageCode = itemPackage.ItemPackageCode,
				      	      ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
				      	      CostPrice = itemCost != null ? itemCost.CostPrice : 0,
				      	      Packing = itemPacking.Packing,
				      	      CostPackage = itemCost != null ? itemCost.CostPrice * (itemPacking != null ? itemPacking.Packing : 1) : 0,
				      	      CostValue = itemCost != null && generalInventory.StoredQuantity != null ? itemCost.CostPrice * (itemPacking != null ? itemPacking.Packing : 1) * generalInventory.StoredQuantity : 0,
				      	      SellingCostValue = generalInventory.CostPrice,
				      	      ExpireDate = generalInventory.ExpireDate,
				      	      BatchNumber = generalInventory.BatchNumber,
				      	      DocumentDate = generalInventory.DocumentDate,
				      	      EntryDate = generalInventory.EntryDate,
				      	      BarCode = generalInventory.BarCode,
				      	      SellingCostPrice = generalInventory.CostPrice,
				      	      Price = generalInventory.Price,

				      	      PurchaseInvoicesQuantity = generalInventory.PurchaseInvoicesQuantity,
				      	      SalesInvoiceReturnQuantity = generalInventory.SalesInvoiceReturnQuantity,
				      	      StockInQuantity = generalInventory.StockInQuantity,
				      	      StockOutReturnQuantity = generalInventory.StockOutReturnQuantity,
				      	      InternalTransferInQuantity = generalInventory.InternalTransferInQuantity,
				      	      InventoryInQuantity = generalInventory.InventoryInQuantity,
				      	      CarryOverInQuantity = generalInventory.CarryOverInQuantity,
				      	      DisassembleInQuantity = generalInventory.DisassembleInQuantity,

				      	      InQuantity = generalInventory.OpenQuantity + generalInventory.InQuantity + generalInventory.PendingInQuantity,
				      	      OutQuantity = generalInventory.OutQuantity,
				      	      PendingOutQuantity = generalInventory.PendingOutQuantity,

				      	      SalesInvoicesQuantity = generalInventory.SalesInvoicesQuantity,
				      	      PurchaseInvoiceReturnQuantity = generalInventory.PurchaseInvoiceReturnQuantity,
				      	      StockOutQuantity = generalInventory.StockOutQuantity,
				      	      StockInReturnQuantity = generalInventory.StockInReturnQuantity,
				      	      InternalTransferOutQuantity = generalInventory.InternalTransferOutQuantity,
				      	      InventoryOutQuantity = generalInventory.InventoryOutQuantity,
				      	      CarryOverOutQuantity = generalInventory.CarryOverOutQuantity,
				      	      DisassembleOutQuantity = generalInventory.DisassembleOutQuantity,

				      	      ClientId = client.ClientId,
				      	      ClientCode = client.ClientCode,
				      	      ClientName = language == LanguageCode.Arabic ? client.ClientNameAr : client.ClientNameEn,
				      	      Reference = generalInventory.Reference,
				      	      RemarksAr = generalInventory.RemarksAr,
				      	      RemarksEn = generalInventory.RemarksEn,
				      	      Notes = generalInventory.Notes,

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

				      	      CreatedAt = generalInventory.CreatedAt,
				      	      UserNameCreated = generalInventory.UserNameCreated,
				      	      ModifiedAt = generalInventory.ModifiedAt,
				      	      UserNameModified = generalInventory.UserNameModified,
				         };

			return report.AsNoTracking();
		}

		public IQueryable<GeneralInventoryDocumentWithMenuCodeDto> GetGeneralInventoryDocumentsWithMenuDataGrouped()
		{
			return from generalInventory in _generalInventoryDocumentService.GetGeneralInventoryDocumentsWithMenuData()
				   group generalInventory by new
				   {
					   //Header Data
					   generalInventory.HeaderId,
					   generalInventory.MenuCode,
					   generalInventory.DocumentFullCode,
					   generalInventory.ClientId,
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
					   generalInventory.Price,
					   generalInventory.Notes,
					   generalInventory.CostPackage,
				   } into g
				   select new GeneralInventoryDocumentWithMenuCodeDto
				   {
					   HeaderId = g.Key.HeaderId,
					   MenuCode = g.Key.MenuCode,
					   DocumentFullCode = g.Key.DocumentFullCode,
					   ClientId = g.Key.ClientId,
					   StoreId = g.Key.StoreId,
					   ItemId = g.Key.ItemId,
					   ItemNote = g.Key.ItemNote,
					   ItemPackageId = g.Key.ItemPackageId,
					   ExpireDate = g.Key.ExpireDate,
					   BatchNumber = g.Key.BatchNumber,
					   DocumentDate = g.Key.DocumentDate,
					   EntryDate = g.Key.EntryDate,
					   BarCode = g.Key.BarCode,
					   StoredQuantity = g.Sum(x => x.StoredQuantity),
					   OpenQuantity = g.Sum(x => x.OpenQuantity),
					   InQuantity = g.Sum(x => x.InQuantity),
					   OutQuantity = g.Sum(x => x.OutQuantity),
					   PendingInQuantity = g.Sum(x => x.PendingInQuantity),
					   PendingOutQuantity = g.Sum(x => x.PendingOutQuantity),
					   CostPackage = g.Key.CostPackage,
					   Price = g.Key.Price,
					   Reference = g.Key.Reference,
					   RemarksAr = g.Key.RemarksAr,
					   RemarksEn = g.Key.RemarksEn,
					   Notes = g.Key.Notes,
					   CreatedAt = g.Key.CreatedAt,
					   UserNameCreated = g.Key.UserNameCreated,
					   ModifiedAt = g.Key.ModifiedAt,
					   UserNameModified = g.Key.UserNameModified,

					   PurchaseInvoicesQuantity = g.Sum(x => x.PurchaseInvoicesQuantity),
					   SalesInvoiceReturnQuantity = g.Sum(x => x.SalesInvoiceReturnQuantity),
					   StockInQuantity = g.Sum(x => x.StockInQuantity),
					   StockOutReturnQuantity = g.Sum(x => x.StockOutReturnQuantity),
					   InternalTransferInQuantity = g.Sum(x => x.InternalTransferInQuantity),
					   InventoryInQuantity = g.Sum(x => x.InventoryInQuantity),
					   CarryOverInQuantity = g.Sum(x => x.CarryOverInQuantity),
					   DisassembleInQuantity = g.Sum(x => x.DisassembleInQuantity),

					   SalesInvoicesQuantity = g.Sum(x => x.SalesInvoicesQuantity),
					   PurchaseInvoiceReturnQuantity = g.Sum(x => x.PurchaseInvoiceReturnQuantity),
					   StockOutQuantity = g.Sum(x => x.StockOutQuantity),
					   StockInReturnQuantity = g.Sum(x => x.StockInReturnQuantity),
					   InternalTransferOutQuantity = g.Sum(x => x.InternalTransferOutQuantity),
					   InventoryOutQuantity = g.Sum(x => x.InventoryOutQuantity),
					   CarryOverOutQuantity = g.Sum(x => x.CarryOverOutQuantity),
					   DisassembleOutQuantity = g.Sum(x => x.DisassembleOutQuantity),
				   };
		}
	}
}
