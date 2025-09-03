using Shared.CoreOne.Contracts.Items;
using Shared.CoreOne.Contracts.Inventory;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Contracts.Taxes;
using Compound.CoreOne.Contracts.Reports.Inventory;
using Shared.Helper.Identity;
using Microsoft.AspNetCore.Http;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Shared.CoreOne.Models.StaticData;
using Compound.CoreOne.Contracts.Reports.Shared;
using Microsoft.EntityFrameworkCore;
using Compound.CoreOne.Models.Dtos.Reports.Inventory;
using Purchases.CoreOne.Contracts;
using Sales.CoreOne.Contracts;
using Shared.CoreOne.Contracts.Basics;
using Shared.CoreOne.Contracts.Menus;
using Shared.CoreOne.Contracts.CostCenters;
using Shared.CoreOne.Contracts.Accounts;
using Shared.CoreOne.Contracts.Journal;

namespace Compound.Service.Services.Reports.Inventory
{
	public class ItemsTradingMovementReportService: IItemsTradingMovementReportService
	{
		private readonly IItemService _itemService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IPurchaseInvoiceHeaderService _purchaseInvoiceHeaderService;
		private readonly IPurchaseInvoiceDetailService _purchaseInvoiceDetailService;
		private readonly IPurchaseInvoiceReturnHeaderService _purchaseInvoiceReturnHeaderService;
		private readonly IPurchaseInvoiceReturnDetailService _purchaseInvoiceReturnDetailService;
		private readonly ISalesInvoiceHeaderService _salesInvoiceHeaderService;
		private readonly ISalesInvoiceDetailService _salesInvoiceDetailService;
		private readonly ISalesInvoiceReturnHeaderService _salesInvoiceReturnHeaderService;
		private readonly ISalesInvoiceReturnDetailService _salesInvoiceReturnDetailService;
		private readonly IItemPackageService _itemPackageService;
		private readonly IStoreService _storeService;
		private readonly IItemTypeService _itemTypeService;
		private readonly IInvoiceTypeService _invoiceTypeService;
		private readonly IMenuService _menuService;
		private readonly IClientService _clientService;
		private readonly ISellerService _sellerService;
		private readonly IPurchaseInvoiceDetailTaxService _purchaseInvoiceDetailTaxService;
		private readonly IPurchaseInvoiceReturnDetailTaxService _purchaseInvoiceReturnDetailTaxService;
		private readonly ISalesInvoiceDetailTaxService _salesInvoiceDetailTaxService;
		private readonly ISalesInvoiceReturnDetailTaxService _salesInvoiceReturnDetailTaxService;
		private readonly ITaxService _taxService;
		private readonly ICostCenterService _costCenterService;
		private readonly IAccountService _accountService;
		private readonly IVendorService _vendorService;
		private readonly ITaxTypeService _taxTypeService;
		private readonly IItemCategoryService _itemCategoryService;
		private readonly IItemSubCategoryService _itemSubCategoryService;
		private readonly IItemSectionService _itemSectionService;
		private readonly IItemSubSectionService _itemSubSectionService;
		private readonly IMainItemService _mainItemService;
		private readonly IBranchService _branchService;
		private readonly IItemAttributeService _itemAttributeService;
		private readonly IItemTaxService _itemTaxService;
		private readonly IItemReportDataService _itemReportDataService;
		private readonly IClientCreditMemoService _clientCreditMemoService;
		private readonly IClientDebitMemoService _clientDebitMemoService;
		private readonly ISupplierCreditMemoService _supplierCreditMemoService;
		private readonly ISupplierDebitMemoService _supplierDebitMemoService;
		private readonly IJournalDetailService _journalDetailService;

		public ItemsTradingMovementReportService(IItemService itemService, IHttpContextAccessor httpContextAccessor, IPurchaseInvoiceHeaderService purchaseInvoiceHeaderService, IPurchaseInvoiceDetailService purchaseInvoiceDetailService, IPurchaseInvoiceReturnHeaderService purchaseInvoiceReturnHeaderService, IPurchaseInvoiceReturnDetailService purchaseInvoiceReturnDetailService, ISalesInvoiceHeaderService salesInvoiceHeaderService, ISalesInvoiceDetailService salesInvoiceDetailService, ISalesInvoiceReturnHeaderService salesInvoiceReturnHeaderService, ISalesInvoiceReturnDetailService salesInvoiceReturnDetailService, IItemPackageService itemPackageService, IStoreService storeService, IItemTypeService itemTypeService, IInvoiceTypeService invoiceTypeService, IMenuService menuService, IClientService clientService, ISellerService sellerService, IPurchaseInvoiceDetailTaxService purchaseInvoiceDetailTaxService, IPurchaseInvoiceReturnDetailTaxService purchaseInvoiceReturnDetailTaxService, ISalesInvoiceDetailTaxService salesInvoiceDetailTaxService, ISalesInvoiceReturnDetailTaxService salesInvoiceReturnDetailTaxService, ITaxService taxService, ICostCenterService costCenterService, IAccountService accountService, IVendorService vendorService, ITaxTypeService taxTypeService, IItemCategoryService itemCategoryService, IItemSubCategoryService itemSubCategoryService, IItemSectionService itemSectionService, IItemSubSectionService itemSubSectionService, IMainItemService mainItemService, IBranchService branchService, IItemAttributeService itemAttributeService, IItemTaxService itemTaxService, IItemReportDataService itemReportDataService, IClientCreditMemoService clientCreditMemoService, IClientDebitMemoService clientDebitMemoService, ISupplierCreditMemoService supplierCreditMemoService, ISupplierDebitMemoService supplierDebitMemoService, IJournalDetailService journalDetailService)
		{
			_itemService = itemService;
			_httpContextAccessor = httpContextAccessor;
			_purchaseInvoiceHeaderService = purchaseInvoiceHeaderService;
			_purchaseInvoiceDetailService = purchaseInvoiceDetailService;
			_purchaseInvoiceReturnHeaderService = purchaseInvoiceReturnHeaderService;
			_purchaseInvoiceReturnDetailService = purchaseInvoiceReturnDetailService;
			_salesInvoiceHeaderService = salesInvoiceHeaderService;
			_salesInvoiceDetailService = salesInvoiceDetailService;
			_salesInvoiceReturnHeaderService = salesInvoiceReturnHeaderService;
			_salesInvoiceReturnDetailService = salesInvoiceReturnDetailService;
			_itemPackageService = itemPackageService;
			_storeService = storeService;
			_itemTypeService = itemTypeService;
			_invoiceTypeService = invoiceTypeService;
			_menuService = menuService;
			_clientService = clientService;
			_sellerService = sellerService;
			_purchaseInvoiceDetailTaxService = purchaseInvoiceDetailTaxService;
			_purchaseInvoiceReturnDetailTaxService = purchaseInvoiceReturnDetailTaxService;
			_salesInvoiceDetailTaxService = salesInvoiceDetailTaxService;
			_salesInvoiceReturnDetailTaxService = salesInvoiceReturnDetailTaxService;
			_taxService = taxService;
			_costCenterService = costCenterService;
			_accountService = accountService;
			_vendorService = vendorService;
			_taxTypeService = taxTypeService;
			_itemCategoryService = itemCategoryService;
			_itemSubCategoryService = itemSubCategoryService;
			_itemSectionService = itemSectionService;
			_itemSubSectionService = itemSubSectionService;
			_mainItemService = mainItemService;
			_branchService = branchService;
			_itemAttributeService = itemAttributeService;
			_itemTaxService = itemTaxService;
			_itemReportDataService = itemReportDataService;
			_clientCreditMemoService = clientCreditMemoService;
			_clientDebitMemoService = clientDebitMemoService;
			_supplierCreditMemoService = supplierCreditMemoService;
			_supplierDebitMemoService = supplierDebitMemoService;
			_journalDetailService = journalDetailService;
		}

		private enum TableIds {
			PurchaseInvoice,
			PurchaseInvoiceReturn,
			SalesInvoice,
			SalesInvoiceReturn,
		}

		public async Task<IQueryable<ItemsTradingMovementReportDto>> GetItemsTradingMovementReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate, bool isGrouped, List<int> menuCodes)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var perItemAttributes = await _itemReportDataService.GetItemAttributesByStoreIds(storeIds);
			var perItemMenuNotes = await _itemReportDataService.GetItemMenuNotesByStoreIds(storeIds);

			var detailTaxes = await GetDetailTaxes(storeIds, fromDate, toDate);


			var result = from invoice in GetSalesAndPurchaseInvoices(fromDate, toDate, isGrouped)
						 from store in _storeService.GetAll().Where(x => x.StoreId == invoice.StoreId)
						 from item in _itemService.GetAll().Where(x => x.ItemId == invoice.ItemId)
						 from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == invoice.ItemPackageId)
						 from itemType in _itemTypeService.GetAll().Where(x => x.ItemTypeId == item.ItemTypeId)
						 from itemCategory in _itemCategoryService.GetAll().Where(x => x.ItemCategoryId == item.ItemCategoryId).DefaultIfEmpty()
						 from itemSubCategory in _itemSubCategoryService.GetAll().Where(x => x.ItemSubCategoryId == item.ItemSubCategoryId).DefaultIfEmpty()
						 from itemSection in _itemSectionService.GetAll().Where(x => x.ItemSectionId == item.ItemSectionId).DefaultIfEmpty()
						 from itemSubSection in _itemSubSectionService.GetAll().Where(x => x.ItemSubSectionId == item.ItemSubSectionId).DefaultIfEmpty()
						 from mainItem in _mainItemService.GetAll().Where(x => x.MainItemId == item.MainItemId).DefaultIfEmpty()
						 from vendor in _vendorService.GetAll().Where(x => x.VendorId == item.VendorId).DefaultIfEmpty()
						 from salesAccount in _accountService.GetAll().Where(x => x.AccountId == item.SalesAccountId).DefaultIfEmpty()
						 from purchaseAccount in _accountService.GetAll().Where(x => x.AccountId == item.PurchaseAccountId).DefaultIfEmpty()
						 from invoiceType in _invoiceTypeService.GetAll().Where(x => x.InvoiceTypeId == invoice.InvoiceTypeId)
						 from menu in _menuService.GetAll().Where(x => x.MenuCode == invoice.MenuCode).DefaultIfEmpty()
						 from client in _clientService.GetAll().Where(x => x.ClientId == invoice.ClientId).DefaultIfEmpty()
						 from seller in _sellerService.GetAll().Where(x => x.SellerId == invoice.SellerId).DefaultIfEmpty()
						 from costCenter in _costCenterService.GetAll().Where(x => x.CostCenterId == invoice.CostCenterId).DefaultIfEmpty()
						 from taxType in _taxTypeService.GetAll().Where(x => x.TaxTypeId == invoice.VatTaxId).DefaultIfEmpty()
						 where item.ItemTypeId != ItemTypeData.Service &&
							   storeIds.Contains(invoice.StoreId) &&
							   (menuCodes.Count == 0 || menuCodes.Contains(invoice.MenuCode ?? 0))
						 group invoice by new
						 {
							 invoice.HeaderId,
							 invoice.DocumentFullCode,
							 invoice.MenuCode,
							 invoice.TableId,
							 invoice.DocumentDate,
							 invoice.EntryDate,
							 invoice.InvoiceTypeId,
							 client.ClientId,
							 client.ClientCode,
							 client.ClientNameAr,
							 client.ClientNameEn,
							 seller.SellerId,
							 seller.SellerCode,
							 seller.SellerNameAr,
							 seller.SellerNameEn,
							 invoice.Reference,
							 invoice.RemarksAr,
							 invoice.RemarksEn,
							 costCenter.CostCenterId,
							 costCenter.CostCenterCode,
							 costCenter.CostCenterNameAr,
							 costCenter.CostCenterNameEn,
							 invoice.StoreId,
							 invoice.VatTaxId,
							 invoice.VatTaxCode,
							 invoice.VatTaxName,
							 invoice.VatTaxAccountId,
							 invoice.VatTaxAccountCode,
							 invoice.VatTaxAccountName,
							 invoice.ItemId,
							 item.ItemCode,
							 invoice.ItemPackageId,
							 invoice.ExpireDate,
							 invoice.BatchNumber,
							 invoice.Price,
							 invoice.ItemDiscountPercent,
							 invoice.HeaderDiscountPercent,
							 invoice.VatPercent,
							 invoice.ItemExpensePercent,
							 invoice.Notes,
							 invoice.ItemNote,
							 invoice.Packing,
							 invoice.ConsumerPrice,
							 invoice.CostPrice,
							 invoice.CostPackage,
							 invoiceType.InvoiceTypeNameAr,
							 invoiceType.InvoiceTypeNameEn,
							 store.StoreNameAr,
							 store.StoreNameEn,
							 item.ItemNameAr,
							 item.ItemNameEn,
							 item.ItemTypeId,
							 itemType.ItemTypeNameEn,
							 itemType.ItemTypeNameAr,
							 itemPackage.ItemPackageCode,
							 itemPackage.PackageNameAr,
							 itemPackage.PackageNameEn,
							 menu.MenuNameAr,
							 menu.MenuNameEn,

							 item.ReorderPointQuantity,

							 vendor.VendorId,
							 vendor.VendorCode,
							 vendor.VendorNameAr,
							 vendor.VendorNameEn,

							 taxType.TaxTypeNameAr,
							 taxType.TaxTypeNameEn,

							 item.PurchasingPrice,
							 item.InternalPrice,
							 item.MaxDiscountPercent,
							 item.SalesAccountId,
							 SalesAccountName = language == LanguageCode.Arabic ? salesAccount.AccountNameAr : salesAccount.AccountNameEn,
							 item.PurchaseAccountId,
							 PurchaseAccountName = language == LanguageCode.Arabic ? purchaseAccount.AccountNameAr : purchaseAccount.AccountNameEn,
							 item.MinBuyQuantity,
							 item.MinSellQuantity,
							 item.MaxBuyQuantity,
							 item.MaxSellQuantity,
							 item.CoverageQuantity,
							 item.IsActive,
							 item.InActiveReasons,
							 item.NoReplenishment,
							 item.IsUnderSelling,
							 item.IsNoStock,
							 item.IsUntradeable,
							 item.IsDeficit,
							 item.IsPos,
							 item.IsOnline,
							 item.IsPoints,
							 item.IsPromoted,
							 item.IsExpired,
							 item.IsBatched,
							 item.ItemLocation,

							 item.ItemCategoryId,
							 ItemCategoryName = itemCategory != null ? language == LanguageCode.Arabic ? itemCategory.CategoryNameAr : itemCategory.CategoryNameEn : null,
							 item.ItemSubCategoryId,
							 ItemSubCategoryName = itemSubCategory != null ? language == LanguageCode.Arabic ? itemSubCategory.SubCategoryNameAr : itemSubCategory.SubCategoryNameEn : null,
							 item.ItemSectionId,
							 ItemSectionName = itemSection != null ? language == LanguageCode.Arabic ? itemSection.SectionNameAr : itemSection.SectionNameEn : null,
							 item.ItemSubSectionId,
							 ItemSubSectionName = itemSubSection != null ? language == LanguageCode.Arabic ? itemSubSection.SubSectionNameAr : itemSubSection.SubSectionNameEn : null,
							 item.MainItemId,
							 MainItemName = mainItem != null ? language == LanguageCode.Arabic ? mainItem.MainItemNameAr : mainItem.MainItemNameEn : null,

							 invoice.CreatedAt,
							 invoice.UserNameCreated,
							 invoice.ModifiedAt,
							 invoice.UserNameModified
						 } into g
						 orderby g.Key.DocumentDate, g.Key.EntryDate, g.Key.MenuCode, g.Key.HeaderId
						 select new ItemsTradingMovementReportDto
						 {
							 HeaderId = g.Key.HeaderId,
							 DocumentFullCode = g.Key.DocumentFullCode,
							 MenuCode = g.Key.MenuCode,
							 MenuName = language == LanguageCode.Arabic ? g.Key.MenuNameAr : g.Key.MenuNameEn,
							 DocumentDate = g.Key.DocumentDate,
							 EntryDate = g.Key.EntryDate,
							 InvoiceTypeId = g.Key.InvoiceTypeId,
							 ClientId = g.Key.ClientId,
							 ClientCode = g.Key.ClientCode,
							 ClientName = language == LanguageCode.Arabic ? g.Key.ClientNameAr : g.Key.ClientNameEn,
							 SellerId = g.Key.SellerId,
							 SellerCode = g.Key.SellerCode,
							 SellerName = language == LanguageCode.Arabic ? g.Key.SellerNameAr : g.Key.SellerNameEn,
							 Reference = g.Key.Reference,
							 RemarksAr = g.Key.RemarksAr,
							 RemarksEn = g.Key.RemarksEn,
							 CostCenterId = g.Key.CostCenterId,
							 CostCenterCode = g.Key.CostCenterCode,
							 CostCenterName = language == LanguageCode.Arabic ? g.Key.CostCenterNameAr : g.Key.CostCenterNameEn,
							 VatTaxId = g.Key.VatTaxId,
							 VatTaxCode = g.Key.VatTaxCode,
							 VatTaxName = g.Key.VatTaxName,
							 VatTaxAccountId = g.Key.VatTaxAccountId,
							 VatTaxAccountCode = g.Key.VatTaxAccountCode,
							 VatTaxAccountName = g.Key.VatTaxAccountName,
							 OtherTaxAccounts = detailTaxes.GetValueOrDefault( new DetailTaxKey { HeaderId = g.Key.HeaderId, TableId = g.Key.TableId, ItemId = g.Key.ItemId, ItemPackageId = g.Key.ItemPackageId, ExpireDate = g.Key.ExpireDate, BatchNumber = g.Key.BatchNumber, CostCenterId = g.Key.CostCenterId, ItemDiscountPercent = g.Key.ItemDiscountPercent, Price = g.Key.Price, ItemNote = g.Key.ItemNote } , ""),
							 InvoiceTypeName = language == LanguageCode.Arabic ? g.Key.InvoiceTypeNameAr : g.Key.InvoiceTypeNameEn,
							 StoreId = g.Key.StoreId,
							 StoreName = language == LanguageCode.Arabic ? g.Key.StoreNameAr : g.Key.StoreNameEn,
							 ItemId = g.Key.ItemId,
							 ItemCode = g.Key.ItemCode,
							 ItemName = g.Key.ItemTypeId == ItemTypeData.Note ? g.Key.ItemNote : (language == LanguageCode.Arabic ? g.Key.ItemNameAr : g.Key.ItemNameEn),
							 ItemNameAr = g.Key.ItemNameAr,
							 ItemNameEn = g.Key.ItemNameEn,
							 ItemTypeId = g.Key.ItemTypeId,
							 ItemTypeName = language == LanguageCode.Arabic ? g.Key.ItemTypeNameAr : g.Key.ItemTypeNameEn,
							 ItemPackageId = g.Key.ItemPackageId,
							 ItemPackageCode = g.Key.ItemPackageCode,
							 ItemPackageName = language == LanguageCode.Arabic ? g.Key.PackageNameAr : g.Key.PackageNameEn,
							 ExpireDate = g.Key.ExpireDate,
							 BatchNumber = g.Key.BatchNumber,
							 Price = g.Key.Price,
							 Quantity = g.Sum(x => x.Quantity),
							 BonusQuantity = g.Sum(x => x.BonusQuantity),
							 PurchaseQuantity = g.Sum(x => x.PurchaseQuantity),
							 SellingQuantity = g.Sum(x => x.SellingQuantity),
							 TotalValue = g.Sum(x => x.TotalValue),
							 ItemDiscountPercent = g.Key.ItemDiscountPercent,
							 ItemDiscountValue = g.Sum(x => x.ItemDiscountValue),
							 TotalValueAfterDiscount = g.Sum(x => x.TotalValueAfterDiscount),
							 HeaderDiscountPercent = g.Key.HeaderDiscountPercent,
							 HeaderDiscountValue = g.Sum(x => x.HeaderDiscountValue),
							 GrossValue = g.Sum(x => x.GrossValue),
							 PurchaseValue = g.Sum(x => x.PurchaseValue),
							 SalesValue = g.Sum(x => x.SalesValue),
							 VatPercent = g.Key.VatPercent,
							 VatValue = g.Sum(x => x.VatValue),
							 SubNetValue = g.Sum(x => x.SubNetValue),
							 OtherTaxValue = g.Sum(x => x.OtherTaxValue),
							 NetValue = g.Sum(x => x.NetValue),
							 ItemExpensePercent = g.Key.ItemExpensePercent,
							 ItemExpenseValue = g.Sum(x => x.ItemExpenseValue),
							 Notes = g.Key.Notes,
							 ConsumerPrice = g.Key.ConsumerPrice,
							 CostPrice = g.Key.CostPrice,
							 CostPackage = g.Key.CostPackage,
							 Packing = g.Key.Packing,
							 CostValue = g.Sum(x => x.CostValue),
							 SalesProfit = g.Sum(x => x.SalesProfit),

							 ReorderPointQuantity = g.Key.ReorderPointQuantity,

							 VendorId = g.Key.VendorId,
							 VendorCode = g.Key.VendorCode,
							 VendorName = language == LanguageCode.Arabic ? g.Key.VendorNameAr : g.Key.VendorNameEn,

							 TaxTypeName = language == LanguageCode.Arabic ? g.Key.TaxTypeNameAr : g.Key.TaxTypeNameEn,

							 PurchasingPrice = g.Key.PurchasingPrice,
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

					         ItemAttributes = perItemAttributes.GetValueOrDefault(g.Key.ItemId, string.Empty),
					         ItemMenuNotes = perItemMenuNotes.GetValueOrDefault(g.Key.ItemId, string.Empty),

							 CreatedAt = g.Key.CreatedAt,
							 UserNameCreated = g.Key.UserNameCreated,
							 ModifiedAt = g.Key.ModifiedAt,
							 UserNameModified = g.Key.UserNameModified,
						 };

			return result;
		}

		private async Task<Dictionary<DetailTaxKey, string>> GetDetailTaxes(List<int> storeIds, DateTime? fromDate, DateTime? toDate)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var purchaseInvoiceDetailTaxes = from purchaseInvoiceDetailTax in _purchaseInvoiceDetailTaxService.GetAll()
											 from purchaseInvoiceDetail in _purchaseInvoiceDetailService.GetAll().Where(x => x.PurchaseInvoiceDetailId == purchaseInvoiceDetailTax.PurchaseInvoiceDetailId)
											 from purchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceDetail.PurchaseInvoiceHeaderId)
											 from tax in _taxService.GetAll().Where(x => x.TaxId == purchaseInvoiceDetailTax.TaxId)
											 from taxAccount in _accountService.GetAll().Where(x => x.AccountId == purchaseInvoiceDetailTax.DebitAccountId)
											 where storeIds.Contains(purchaseInvoiceHeader.StoreId) &&
												   (fromDate == null || purchaseInvoiceHeader.DocumentDate >= fromDate) &&
												   (toDate == null || purchaseInvoiceHeader.DocumentDate <= toDate)
											 select new
											 {
												 //Keys
												 HeaderId = purchaseInvoiceDetail.PurchaseInvoiceHeaderId,
												 TableId = (int)TableIds.PurchaseInvoice,
												 ItemId = purchaseInvoiceDetail.ItemId,
												 ItemPackageId = purchaseInvoiceDetail.ItemPackageId,
												 ExpireDate = purchaseInvoiceDetail.ExpireDate,
												 BatchNumber = purchaseInvoiceDetail.BatchNumber,
												 CostCenterId = purchaseInvoiceDetail.CostCenterId,
												 ItemDiscountPercent = purchaseInvoiceDetail.ItemDiscountPercent,
												 Price = purchaseInvoiceDetail.PurchasePrice,
												 ItemNote = purchaseInvoiceDetail.ItemNote,
												 //Value
												 OtherTaxAccount = (language == LanguageCode.Arabic ? tax.TaxNameAr : tax.TaxNameEn) + " (" + (language == LanguageCode.Arabic ? taxAccount.AccountNameAr : taxAccount.AccountNameEn) + ")",
											 };

			var purchaseInvoiceReturnDetailTaxes = from purchaseInvoiceReturnDetailTax in _purchaseInvoiceReturnDetailTaxService.GetAll()
												   from purchaseInvoiceReturnDetail in _purchaseInvoiceReturnDetailService.GetAll().Where(x => x.PurchaseInvoiceReturnDetailId == purchaseInvoiceReturnDetailTax.PurchaseInvoiceReturnDetailId)
												   from purchaseInvoiceReturnHeader in _purchaseInvoiceReturnHeaderService.GetAll().Where(x => x.PurchaseInvoiceReturnHeaderId == purchaseInvoiceReturnDetail.PurchaseInvoiceReturnHeaderId)
												   from tax in _taxService.GetAll().Where(x => x.TaxId == purchaseInvoiceReturnDetailTax.TaxId)
												   from taxAccount in _accountService.GetAll().Where(x => x.AccountId == purchaseInvoiceReturnDetailTax.CreditAccountId)
												   where storeIds.Contains(purchaseInvoiceReturnHeader.StoreId) &&
														 (fromDate == null || purchaseInvoiceReturnHeader.DocumentDate >= fromDate) &&
														 (toDate == null || purchaseInvoiceReturnHeader.DocumentDate <= toDate)
												   select new
												   {
													   //Keys
													   HeaderId = purchaseInvoiceReturnDetail.PurchaseInvoiceReturnHeaderId,
													   TableId = (int)TableIds.PurchaseInvoiceReturn,
													   ItemId = purchaseInvoiceReturnDetail.ItemId,
													   ItemPackageId = purchaseInvoiceReturnDetail.ItemPackageId,
													   ExpireDate = purchaseInvoiceReturnDetail.ExpireDate,
													   BatchNumber = purchaseInvoiceReturnDetail.BatchNumber,
													   CostCenterId = purchaseInvoiceReturnDetail.CostCenterId,
													   ItemDiscountPercent = purchaseInvoiceReturnDetail.ItemDiscountPercent,
													   Price = purchaseInvoiceReturnDetail.PurchasePrice,
													   ItemNote = purchaseInvoiceReturnDetail.ItemNote,
													   //Value
													   OtherTaxAccount = (language == LanguageCode.Arabic ? tax.TaxNameAr : tax.TaxNameEn) + " (" + (language == LanguageCode.Arabic ? taxAccount.AccountNameAr : taxAccount.AccountNameEn) + ")",
					 						       };

			var salesInvoiceDetailTaxes = from salesInvoiceDetailTax in _salesInvoiceDetailTaxService.GetAll()
										  from salesInvoiceDetail in _salesInvoiceDetailService.GetAll().Where(x => x.SalesInvoiceDetailId == salesInvoiceDetailTax.SalesInvoiceDetailId)
										  from salesInvoiceHeader in _salesInvoiceHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceDetail.SalesInvoiceHeaderId)
										  from tax in _taxService.GetAll().Where(x => x.TaxId == salesInvoiceDetailTax.TaxId)
										  from taxAccount in _accountService.GetAll().Where(x => x.AccountId == salesInvoiceDetailTax.CreditAccountId)
										  where storeIds.Contains(salesInvoiceHeader.StoreId) &&
												(fromDate == null || salesInvoiceHeader.DocumentDate >= fromDate) &&
												(toDate == null || salesInvoiceHeader.DocumentDate <= toDate)
										  select new
										  {
											  // Keys
											  HeaderId = salesInvoiceDetail.SalesInvoiceHeaderId,
											  TableId = (int)TableIds.SalesInvoice,
											  ItemId = salesInvoiceDetail.ItemId,
											  ItemPackageId = salesInvoiceDetail.ItemPackageId,
											  ExpireDate = salesInvoiceDetail.ExpireDate,
											  BatchNumber = salesInvoiceDetail.BatchNumber,
											  CostCenterId = salesInvoiceDetail.CostCenterId,
											  ItemDiscountPercent = salesInvoiceDetail.ItemDiscountPercent,
											  Price = salesInvoiceDetail.SellingPrice,
											  ItemNote = salesInvoiceDetail.ItemNote,
											  // Value
											  OtherTaxAccount = (language == LanguageCode.Arabic ? tax.TaxNameAr : tax.TaxNameEn) + " (" + (language == LanguageCode.Arabic ? taxAccount.AccountNameAr : taxAccount.AccountNameEn) + ")",
										  };

			var salesInvoiceReturnDetailTaxes = from salesInvoiceReturnDetailTax in _salesInvoiceReturnDetailTaxService.GetAll()
												from salesInvoiceReturnDetail in _salesInvoiceReturnDetailService.GetAll().Where(x => x.SalesInvoiceReturnDetailId == salesInvoiceReturnDetailTax.SalesInvoiceReturnDetailId)
												from salesInvoiceReturnHeader in _salesInvoiceReturnHeaderService.GetAll().Where(x => x.SalesInvoiceReturnHeaderId == salesInvoiceReturnDetail.SalesInvoiceReturnHeaderId)
												from tax in _taxService.GetAll().Where(x => x.TaxId == salesInvoiceReturnDetailTax.TaxId)
												from taxAccount in _accountService.GetAll().Where(x => x.AccountId == salesInvoiceReturnDetailTax.DebitAccountId)
												where storeIds.Contains(salesInvoiceReturnHeader.StoreId) &&
													  (fromDate == null || salesInvoiceReturnHeader.DocumentDate >= fromDate) &&
													  (toDate == null || salesInvoiceReturnHeader.DocumentDate <= toDate)
												select new
												{
													// Keys
													HeaderId = salesInvoiceReturnDetail.SalesInvoiceReturnHeaderId,
													TableId = (int)TableIds.SalesInvoiceReturn,
													ItemId = salesInvoiceReturnDetail.ItemId,
													ItemPackageId = salesInvoiceReturnDetail.ItemPackageId,
													ExpireDate = salesInvoiceReturnDetail.ExpireDate,
													BatchNumber = salesInvoiceReturnDetail.BatchNumber,
													CostCenterId = salesInvoiceReturnDetail.CostCenterId,
													ItemDiscountPercent = salesInvoiceReturnDetail.ItemDiscountPercent,
													Price = salesInvoiceReturnDetail.SellingPrice,
													ItemNote = salesInvoiceReturnDetail.ItemNote,
													// Value
													OtherTaxAccount = (language == LanguageCode.Arabic ? tax.TaxNameAr : tax.TaxNameEn) + " (" + (language == LanguageCode.Arabic ? taxAccount.AccountNameAr : taxAccount.AccountNameEn) + ")",
												};
            var combinedDetailTaxQuery = purchaseInvoiceDetailTaxes
                .Concat(purchaseInvoiceReturnDetailTaxes)
                .Concat(salesInvoiceDetailTaxes)
                .Concat(salesInvoiceReturnDetailTaxes);
            
            var result = (await combinedDetailTaxQuery.ToListAsync())
                .GroupBy(x => new DetailTaxKey
                {
                    HeaderId = x.HeaderId,
                    TableId = x.TableId,
                    ItemId = x.ItemId,
                    ItemPackageId = x.ItemPackageId,
                    ExpireDate = x.ExpireDate,
                    BatchNumber = x.BatchNumber,
                    CostCenterId = x.CostCenterId,
                    ItemDiscountPercent = x.ItemDiscountPercent,
                    Price = x.Price,
					ItemNote = x.ItemNote
                })
                .ToDictionary(
                    g => g.Key,
                    g => string.Join(", ", g.Select(x => x.OtherTaxAccount).Distinct())
                );

			return result;
		}

		private IQueryable<ItemsTradingMovementDataDto> GetSalesAndPurchaseInvoices(DateTime? fromDate, DateTime? toDate, bool isGrouped)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var purchaseInvoices = from purchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll()
								   from purchaseInvoiceDetail in _purchaseInvoiceDetailService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeader.PurchaseInvoiceHeaderId)
								   from vatTax in _taxService.GetAllStoreVatTaxes().Where(x => x.StoreId == purchaseInvoiceHeader.StoreId).DefaultIfEmpty()
								   where (fromDate == null || purchaseInvoiceHeader.DocumentDate >= fromDate) &&
										 (toDate == null || purchaseInvoiceHeader.DocumentDate <= toDate)
								   select new ItemsTradingMovementDataDto
								   {
									   HeaderId = purchaseInvoiceHeader.PurchaseInvoiceHeaderId,
									   DocumentFullCode = purchaseInvoiceHeader.Prefix + purchaseInvoiceHeader.DocumentCode + purchaseInvoiceHeader.Suffix,
									   MenuCode = purchaseInvoiceHeader.MenuCode,
							           TableId = (int)TableIds.PurchaseInvoice,
									   DocumentDate = purchaseInvoiceHeader.DocumentDate,
									   EntryDate = purchaseInvoiceHeader.EntryDate,
									   InvoiceTypeId = purchaseInvoiceHeader.InvoiceTypeId,
									   ClientId = null,
									   SellerId = null,
									   Reference = purchaseInvoiceHeader.Reference,
									   RemarksAr = purchaseInvoiceHeader.RemarksAr,
									   RemarksEn = purchaseInvoiceHeader.RemarksEn,
									   CostCenterId = purchaseInvoiceDetail.CostCenterId,
									   VatTaxId = vatTax.TaxId,
									   VatTaxCode = vatTax.TaxCode,
									   VatTaxName = language == LanguageCode.Arabic ? vatTax.TaxNameAr : vatTax.TaxNameEn,
									   VatTaxAccountId = vatTax.DrAccount,
									   VatTaxAccountCode = vatTax.DrAccountCode,
									   VatTaxAccountName = vatTax.DrAccountName,
									   StoreId = purchaseInvoiceHeader.StoreId,
									   ItemId = purchaseInvoiceDetail.ItemId,
									   ItemPackageId = purchaseInvoiceDetail.ItemPackageId,
									   ExpireDate = isGrouped ? null : purchaseInvoiceDetail.ExpireDate,
									   BatchNumber = isGrouped ? null : purchaseInvoiceDetail.BatchNumber,
									   Price = purchaseInvoiceDetail.PurchasePrice,
									   Quantity = purchaseInvoiceDetail.Quantity,
									   BonusQuantity = purchaseInvoiceDetail.BonusQuantity,
									   PurchaseQuantity = (purchaseInvoiceDetail.Quantity + purchaseInvoiceDetail.BonusQuantity),
									   SellingQuantity = 0.0M,
									   TotalValue = purchaseInvoiceDetail.TotalValue,
									   ItemDiscountPercent = purchaseInvoiceDetail.ItemDiscountPercent,
									   ItemDiscountValue = purchaseInvoiceDetail.ItemDiscountValue,
									   TotalValueAfterDiscount = purchaseInvoiceDetail.TotalValueAfterDiscount,
									   HeaderDiscountPercent = purchaseInvoiceHeader.DiscountPercent,
									   HeaderDiscountValue = purchaseInvoiceDetail.HeaderDiscountValue,
									   GrossValue = purchaseInvoiceDetail.GrossValue,
									   PurchaseValue = purchaseInvoiceDetail.GrossValue,
									   SalesValue = 0.0M,
									   VatPercent = purchaseInvoiceDetail.VatPercent,
									   VatValue = purchaseInvoiceDetail.VatValue,
									   SubNetValue = purchaseInvoiceDetail.SubNetValue,
									   OtherTaxValue = purchaseInvoiceDetail.OtherTaxValue,
									   NetValue = purchaseInvoiceDetail.NetValue,
									   ItemExpensePercent = purchaseInvoiceDetail.ItemExpensePercent,
									   ItemExpenseValue = purchaseInvoiceDetail.ItemExpenseValue,
									   Notes = isGrouped ? null : purchaseInvoiceDetail.Notes,
									   ItemNote = purchaseInvoiceDetail.ItemNote,
									   ConsumerPrice = purchaseInvoiceDetail.ConsumerPrice,
									   Packing = purchaseInvoiceDetail.Packing,
									   CostPrice = purchaseInvoiceDetail.CostPrice,
									   CostPackage = purchaseInvoiceDetail.CostPackage,
									   CostValue = purchaseInvoiceDetail.CostValue,
									   SalesProfit = 0.0M,
									   CreatedAt = purchaseInvoiceHeader.CreatedAt,
									   UserNameCreated = purchaseInvoiceHeader.UserNameCreated,
									   ModifiedAt = purchaseInvoiceHeader.ModifiedAt,
									   UserNameModified = purchaseInvoiceHeader.UserNameModified,
								   };

			var purchaseInvoiceReturns = from purchaseInvoiceReturnHeader in _purchaseInvoiceReturnHeaderService.GetAll()
										 from purchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceReturnHeader.PurchaseInvoiceHeaderId)
										 from purchaseInvoiceReturnDetail in _purchaseInvoiceReturnDetailService.GetAll().Where(x => x.PurchaseInvoiceReturnHeaderId == purchaseInvoiceReturnHeader.PurchaseInvoiceReturnHeaderId)
								         from vatTax in _taxService.GetAllStoreVatTaxes().Where(x => x.StoreId == purchaseInvoiceReturnHeader.StoreId).DefaultIfEmpty()
										 where (fromDate == null || purchaseInvoiceReturnHeader.DocumentDate >= fromDate) &&
											   (toDate == null || purchaseInvoiceReturnHeader.DocumentDate <= toDate)
										 select new ItemsTradingMovementDataDto
										 {
											 HeaderId = purchaseInvoiceReturnHeader.PurchaseInvoiceReturnHeaderId,
											 DocumentFullCode = purchaseInvoiceReturnHeader.Prefix + purchaseInvoiceReturnHeader.DocumentCode + purchaseInvoiceReturnHeader.Suffix,
											 MenuCode = purchaseInvoiceReturnHeader.MenuCode,
							                 TableId = (int)TableIds.PurchaseInvoiceReturn,
											 DocumentDate = purchaseInvoiceReturnHeader.DocumentDate,
											 EntryDate = purchaseInvoiceReturnHeader.EntryDate,
											 InvoiceTypeId = purchaseInvoiceHeader.InvoiceTypeId,
									         ClientId = null,
									         SellerId = null,
									         Reference = purchaseInvoiceReturnHeader.Reference,
									         RemarksAr = purchaseInvoiceReturnHeader.RemarksAr,
									         RemarksEn = purchaseInvoiceReturnHeader.RemarksEn,
									         CostCenterId = purchaseInvoiceReturnDetail.CostCenterId,
									         VatTaxId = vatTax.TaxId,
									         VatTaxCode = vatTax.TaxCode,
									         VatTaxName = language == LanguageCode.Arabic ? vatTax.TaxNameAr : vatTax.TaxNameEn,
									         VatTaxAccountId = vatTax.CrAccount,
									         VatTaxAccountCode = vatTax.CrAccountCode,
									         VatTaxAccountName = vatTax.CrAccountName,
											 StoreId = purchaseInvoiceReturnHeader.StoreId,
											 ItemId = purchaseInvoiceReturnDetail.ItemId,
											 ItemPackageId = purchaseInvoiceReturnDetail.ItemPackageId,
											 ExpireDate = isGrouped ? null : purchaseInvoiceReturnDetail.ExpireDate,
											 BatchNumber = isGrouped ? null : purchaseInvoiceReturnDetail.BatchNumber,
											 Price = purchaseInvoiceReturnDetail.PurchasePrice,
											 Quantity = purchaseInvoiceReturnDetail.Quantity,
											 BonusQuantity = purchaseInvoiceReturnDetail.BonusQuantity,
											 PurchaseQuantity = -(purchaseInvoiceReturnDetail.Quantity + purchaseInvoiceReturnDetail.BonusQuantity),
											 SellingQuantity = 0.0M,
											 TotalValue = purchaseInvoiceReturnDetail.TotalValue,
											 ItemDiscountPercent = purchaseInvoiceReturnDetail.ItemDiscountPercent,
											 ItemDiscountValue = purchaseInvoiceReturnDetail.ItemDiscountValue,
											 TotalValueAfterDiscount = purchaseInvoiceReturnDetail.TotalValueAfterDiscount,
											 HeaderDiscountPercent = purchaseInvoiceReturnHeader.DiscountPercent,
											 HeaderDiscountValue = purchaseInvoiceReturnDetail.HeaderDiscountValue,
											 GrossValue = purchaseInvoiceReturnDetail.GrossValue,
											 PurchaseValue = -purchaseInvoiceReturnDetail.GrossValue,
											 SalesValue = 0.0M,
											 VatPercent = purchaseInvoiceReturnDetail.VatPercent,
											 VatValue = purchaseInvoiceReturnDetail.VatValue,
											 SubNetValue = purchaseInvoiceReturnDetail.SubNetValue,
											 OtherTaxValue = purchaseInvoiceReturnDetail.OtherTaxValue,
											 NetValue = purchaseInvoiceReturnDetail.NetValue,
											 ItemExpensePercent = 0.0M,
											 ItemExpenseValue = 0.0M,
											 Notes = isGrouped ? null : purchaseInvoiceReturnDetail.Notes,
											 ItemNote = purchaseInvoiceReturnDetail.ItemNote,
											 ConsumerPrice = purchaseInvoiceReturnDetail.ConsumerPrice,
									         Packing = purchaseInvoiceReturnDetail.Packing,
											 CostPrice = purchaseInvoiceReturnDetail.CostPrice,
											 CostPackage = purchaseInvoiceReturnDetail.CostPackage,
											 CostValue = purchaseInvoiceReturnDetail.CostValue,
											 SalesProfit = 0.0M,
											 CreatedAt = purchaseInvoiceReturnHeader.CreatedAt,
											 UserNameCreated = purchaseInvoiceReturnHeader.UserNameCreated,
											 ModifiedAt = purchaseInvoiceReturnHeader.ModifiedAt,
											 UserNameModified = purchaseInvoiceReturnHeader.UserNameModified,
										 };

			var salesInvoices = from salesInvoiceHeader in _salesInvoiceHeaderService.GetAll()
								from salesInvoiceDetail in _salesInvoiceDetailService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeader.SalesInvoiceHeaderId)
								from vatTax in _taxService.GetAllStoreVatTaxes().Where(x => x.StoreId == salesInvoiceHeader.StoreId).DefaultIfEmpty()
								where (fromDate == null || salesInvoiceHeader.DocumentDate >= fromDate) &&
									  (toDate == null || salesInvoiceHeader.DocumentDate <= toDate)
								select new ItemsTradingMovementDataDto
								{
									HeaderId = salesInvoiceHeader.SalesInvoiceHeaderId,
									DocumentFullCode = salesInvoiceHeader.Prefix + salesInvoiceHeader.DocumentCode + salesInvoiceHeader.Suffix,
									MenuCode = salesInvoiceHeader.MenuCode,
							        TableId = (int)TableIds.SalesInvoice,
									DocumentDate = salesInvoiceHeader.DocumentDate,
									EntryDate = salesInvoiceHeader.EntryDate,
									InvoiceTypeId = salesInvoiceHeader.InvoiceTypeId,
									ClientId = salesInvoiceHeader.ClientId,
									SellerId = salesInvoiceHeader.SellerId,
									Reference = salesInvoiceHeader.Reference,
									RemarksAr = salesInvoiceHeader.RemarksAr,
									RemarksEn = salesInvoiceHeader.RemarksEn,
									CostCenterId = salesInvoiceDetail.CostCenterId,
									VatTaxId = vatTax.TaxId,
									VatTaxCode = vatTax.TaxCode,
									VatTaxName = language == LanguageCode.Arabic ? vatTax.TaxNameAr : vatTax.TaxNameEn,
									VatTaxAccountId = vatTax.CrAccount,
									VatTaxAccountCode = vatTax.CrAccountCode,
									VatTaxAccountName = vatTax.CrAccountName,
									StoreId = salesInvoiceHeader.StoreId,
									ItemId = salesInvoiceDetail.ItemId,
									ItemPackageId = salesInvoiceDetail.ItemPackageId,
									ExpireDate = isGrouped ? null : salesInvoiceDetail.ExpireDate,
									BatchNumber = isGrouped ? null : salesInvoiceDetail.BatchNumber,
									Price = salesInvoiceDetail.SellingPrice,
									Quantity = salesInvoiceDetail.Quantity,
									BonusQuantity = salesInvoiceDetail.BonusQuantity,
									PurchaseQuantity = 0.0M,
									SellingQuantity = (salesInvoiceDetail.Quantity + salesInvoiceDetail.BonusQuantity),
									TotalValue = salesInvoiceDetail.TotalValue,
									ItemDiscountPercent = salesInvoiceDetail.ItemDiscountPercent,
									ItemDiscountValue = salesInvoiceDetail.ItemDiscountValue,
									TotalValueAfterDiscount = salesInvoiceDetail.TotalValueAfterDiscount,
									HeaderDiscountPercent = salesInvoiceHeader.DiscountPercent,
									HeaderDiscountValue = salesInvoiceDetail.HeaderDiscountValue,
									GrossValue = salesInvoiceDetail.GrossValue,
									PurchaseValue = 0.0M,
									SalesValue = salesInvoiceDetail.GrossValue,
									VatPercent = salesInvoiceDetail.VatPercent,
									VatValue = salesInvoiceDetail.VatValue,
									SubNetValue = salesInvoiceDetail.SubNetValue,
									OtherTaxValue = salesInvoiceDetail.OtherTaxValue,
									NetValue = salesInvoiceDetail.NetValue,
									ItemExpensePercent = 0.0M,
									ItemExpenseValue = 0.0M,
									Notes = isGrouped ? null : salesInvoiceDetail.Notes,
									ItemNote = salesInvoiceDetail.ItemNote,
									Packing = salesInvoiceDetail.Packing,
									ConsumerPrice = salesInvoiceDetail.ConsumerPrice,
									CostPrice = salesInvoiceDetail.CostPrice,
									CostPackage = salesInvoiceDetail.CostPackage,
									CostValue = salesInvoiceDetail.CostValue,
									SalesProfit = salesInvoiceDetail.GrossValue - salesInvoiceDetail.CostValue,
									CreatedAt = salesInvoiceHeader.CreatedAt,
									UserNameCreated = salesInvoiceHeader.UserNameCreated,
									ModifiedAt = salesInvoiceHeader.ModifiedAt,
									UserNameModified = salesInvoiceHeader.UserNameModified,
								};

			var salesInvoiceReturns = from salesInvoiceReturnHeader in _salesInvoiceReturnHeaderService.GetAll()
									  from salesInvoiceHeader in _salesInvoiceHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceReturnHeader.SalesInvoiceHeaderId)
									  from salesInvoiceReturnDetail in _salesInvoiceReturnDetailService.GetAll().Where(x => x.SalesInvoiceReturnHeaderId == salesInvoiceReturnHeader.SalesInvoiceReturnHeaderId)
									  from vatTax in _taxService.GetAllStoreVatTaxes().Where(x => x.StoreId == salesInvoiceReturnHeader.StoreId).DefaultIfEmpty()
									  where (fromDate == null || salesInvoiceReturnHeader.DocumentDate >= fromDate) &&
											(toDate == null || salesInvoiceReturnHeader.DocumentDate <= toDate)
									  select new ItemsTradingMovementDataDto
									  {
										  HeaderId = salesInvoiceReturnHeader.SalesInvoiceReturnHeaderId,
										  DocumentFullCode = salesInvoiceReturnHeader.Prefix + salesInvoiceReturnHeader.DocumentCode + salesInvoiceReturnHeader.Suffix,
										  MenuCode = salesInvoiceReturnHeader.MenuCode,
							              TableId = (int)TableIds.SalesInvoiceReturn,
										  DocumentDate = salesInvoiceReturnHeader.DocumentDate,
										  EntryDate = salesInvoiceReturnHeader.EntryDate,
										  InvoiceTypeId = salesInvoiceHeader.InvoiceTypeId,
									      ClientId = salesInvoiceReturnHeader.ClientId,
									      SellerId = salesInvoiceReturnHeader.SellerId,
									      Reference = salesInvoiceReturnHeader.Reference,
									      RemarksAr = salesInvoiceReturnHeader.RemarksAr,
									      RemarksEn = salesInvoiceReturnHeader.RemarksEn,
									      CostCenterId = salesInvoiceReturnDetail.CostCenterId,
									      VatTaxId = vatTax.TaxId,
									      VatTaxCode = vatTax.TaxCode,
									      VatTaxName = language == LanguageCode.Arabic ? vatTax.TaxNameAr : vatTax.TaxNameEn,
									      VatTaxAccountId = vatTax.DrAccount,
									      VatTaxAccountCode = vatTax.DrAccountCode,
									      VatTaxAccountName = vatTax.DrAccountName,
										  StoreId = salesInvoiceReturnHeader.StoreId,
										  ItemId = salesInvoiceReturnDetail.ItemId,
										  ItemPackageId = salesInvoiceReturnDetail.ItemPackageId,
										  ExpireDate = isGrouped ? null : salesInvoiceReturnDetail.ExpireDate,
										  BatchNumber = isGrouped ? null : salesInvoiceReturnDetail.BatchNumber,
										  Price = salesInvoiceReturnDetail.SellingPrice,
										  Quantity = salesInvoiceReturnDetail.Quantity,
										  BonusQuantity = salesInvoiceReturnDetail.BonusQuantity,
										  PurchaseQuantity = 0.0M,
										  SellingQuantity = -(salesInvoiceReturnDetail.Quantity + salesInvoiceReturnDetail.BonusQuantity),
										  TotalValue = salesInvoiceReturnDetail.TotalValue,
										  ItemDiscountPercent = salesInvoiceReturnDetail.ItemDiscountPercent,
										  ItemDiscountValue = salesInvoiceReturnDetail.ItemDiscountValue,
										  TotalValueAfterDiscount = salesInvoiceReturnDetail.TotalValueAfterDiscount,
										  HeaderDiscountPercent = salesInvoiceReturnHeader.DiscountPercent,
										  HeaderDiscountValue = salesInvoiceReturnDetail.HeaderDiscountValue,
										  GrossValue = salesInvoiceReturnDetail.GrossValue,
										  PurchaseValue = 0.0M,
										  SalesValue = -salesInvoiceReturnDetail.GrossValue,
										  VatPercent = salesInvoiceReturnDetail.VatPercent,
										  VatValue = salesInvoiceReturnDetail.VatValue,
										  SubNetValue = salesInvoiceReturnDetail.SubNetValue,
										  OtherTaxValue = salesInvoiceReturnDetail.OtherTaxValue,
										  NetValue = salesInvoiceReturnDetail.NetValue,
										  ItemExpensePercent = 0.0M,
										  ItemExpenseValue = 0.0M,
										  Notes = isGrouped ? null : salesInvoiceReturnDetail.Notes,
										  ItemNote = salesInvoiceReturnDetail.ItemNote,
									      Packing = salesInvoiceReturnDetail.Packing,
										  ConsumerPrice = salesInvoiceReturnDetail.ConsumerPrice,
										  CostPrice = salesInvoiceReturnDetail.CostPrice,
										  CostPackage = salesInvoiceReturnDetail.CostPackage,
										  CostValue = salesInvoiceReturnDetail.CostValue,
										  SalesProfit = -(salesInvoiceReturnDetail.GrossValue - salesInvoiceReturnDetail.CostValue),
										  CreatedAt = salesInvoiceReturnHeader.CreatedAt,
										  UserNameCreated = salesInvoiceReturnHeader.UserNameCreated,
										  ModifiedAt = salesInvoiceReturnHeader.ModifiedAt,
										  UserNameModified = salesInvoiceReturnHeader.UserNameModified,
									  };


			var salesInvoiceHeaderTotals = _salesInvoiceHeaderService.GetAll().Select(x => new { x.SalesInvoiceHeaderId, x.GrossValue })
				.Concat(_salesInvoiceReturnHeaderService.GetAll().Select(x => new { x.SalesInvoiceHeaderId, GrossValue = -x.GrossValue }))
				.GroupBy(x => x.SalesInvoiceHeaderId).Select(g => new { SalesInvoiceHeaderId = g.Key, FinalGrossValue = g.Sum(x => x.GrossValue) });

			var salesInvoiceDetailTotals = _salesInvoiceDetailService.GetAll().Select(x => new { x.SalesInvoiceHeaderId, x.ItemId, x.ItemPackageId, ExpireDate = isGrouped ? null : x.ExpireDate, BatchNumber = isGrouped ? null : x.BatchNumber, x.ItemNote, x.GrossValue })
				.Concat(_salesInvoiceReturnDetailService.GetAll().Join(_salesInvoiceReturnHeaderService.GetAll(), x => x.SalesInvoiceReturnHeaderId, x => x.SalesInvoiceReturnHeaderId, (detail, header) => new { header.SalesInvoiceHeaderId, detail.ItemId, detail.ItemPackageId, detail.ExpireDate, detail.BatchNumber, detail.ItemNote, GrossValue = -detail.GrossValue }))
				.GroupBy(x => new { x.SalesInvoiceHeaderId, x.ItemId, x.ItemPackageId, x.ExpireDate, x.BatchNumber, x.ItemNote }).Select(g => new { g.Key.SalesInvoiceHeaderId, g.Key.ItemId, g.Key.ItemPackageId, g.Key.ExpireDate, g.Key.BatchNumber, g.Key.ItemNote, FinalGrossValue = g.Sum(x => x.GrossValue) });

			var clientCreditMemos = from clientCreditMemo in _clientCreditMemoService.GetAll()
									from taxJournal in _journalDetailService.GetAll().Where(x => x.JournalHeaderId == clientCreditMemo.JournalHeaderId && x.IsTax)
									from memoValueBeforeTaxJournal in _journalDetailService.GetAll().Where(x => x.JournalDetailId == taxJournal.TaxParentId)
									from salesInvoiceHeader in _salesInvoiceHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == clientCreditMemo.SalesInvoiceHeaderId)
									from salesInvoiceHeaderTotal in salesInvoiceHeaderTotals.Where(x => x.SalesInvoiceHeaderId == clientCreditMemo.SalesInvoiceHeaderId)
									from salesInvoiceDetailTotal in salesInvoiceDetailTotals.Where(x => x.SalesInvoiceHeaderId == clientCreditMemo.SalesInvoiceHeaderId)
									from vatTax in _taxService.GetAllStoreVatTaxes().Where(x => x.StoreId == clientCreditMemo.StoreId).DefaultIfEmpty()
									where (fromDate == null || clientCreditMemo.DocumentDate >= fromDate) &&
										  (toDate == null || clientCreditMemo.DocumentDate <= toDate)
									select new ItemsTradingMovementDataDto
									{
										HeaderId = clientCreditMemo.SalesInvoiceHeaderId,
										DocumentFullCode = clientCreditMemo.Prefix + clientCreditMemo.DocumentCode + clientCreditMemo.Suffix,
										MenuCode = MenuCodeData.ClientCreditMemo,
										TableId = 0,
										DocumentDate = clientCreditMemo.DocumentDate,
										EntryDate = clientCreditMemo.EntryDate,
										InvoiceTypeId = salesInvoiceHeader.InvoiceTypeId,
										ClientId = clientCreditMemo.ClientId,
										SellerId = clientCreditMemo.SellerId,
										Reference = clientCreditMemo.Reference,
										RemarksAr = clientCreditMemo.RemarksAr,
										RemarksEn = clientCreditMemo.RemarksEn,
										CostCenterId = null,
										VatTaxId = vatTax.TaxId,
										VatTaxCode = vatTax.TaxCode,
										VatTaxName = language == LanguageCode.Arabic ? vatTax.TaxNameAr : vatTax.TaxNameEn,
										VatTaxAccountId = vatTax.DrAccount,
										VatTaxAccountCode = vatTax.DrAccountCode,
										VatTaxAccountName = vatTax.DrAccountName,
										StoreId = clientCreditMemo.StoreId,
										ItemId = salesInvoiceDetailTotal.ItemId,
										ItemPackageId = salesInvoiceDetailTotal.ItemPackageId,
										ExpireDate = salesInvoiceDetailTotal.ExpireDate,
										BatchNumber = salesInvoiceDetailTotal.BatchNumber,
										Price = 0,
										Quantity = 0,
										BonusQuantity = 0,
										PurchaseQuantity = 0.0M,
										SellingQuantity = 0.0M,
										TotalValue = salesInvoiceDetailTotal.FinalGrossValue * (memoValueBeforeTaxJournal.DebitValue / salesInvoiceHeaderTotal.FinalGrossValue),
										ItemDiscountPercent = 0,
										ItemDiscountValue = 0,
										TotalValueAfterDiscount = 0,
										HeaderDiscountPercent = 0,
										HeaderDiscountValue = 0,
										GrossValue = salesInvoiceDetailTotal.FinalGrossValue * (memoValueBeforeTaxJournal.DebitValue / salesInvoiceHeaderTotal.FinalGrossValue),
										PurchaseValue = 0.0M,
										SalesValue = -salesInvoiceDetailTotal.FinalGrossValue * (memoValueBeforeTaxJournal.DebitValue / salesInvoiceHeaderTotal.FinalGrossValue),
										VatPercent = taxJournal.TaxPercent,
										VatValue = salesInvoiceDetailTotal.FinalGrossValue * (taxJournal.DebitValue / salesInvoiceHeaderTotal.FinalGrossValue),
										SubNetValue = salesInvoiceDetailTotal.FinalGrossValue * (clientCreditMemo.MemoValue / salesInvoiceHeaderTotal.FinalGrossValue),
										OtherTaxValue = 0,
										NetValue = salesInvoiceDetailTotal.FinalGrossValue * (clientCreditMemo.MemoValue / salesInvoiceHeaderTotal.FinalGrossValue),
										ItemExpensePercent = 0.0M,
										ItemExpenseValue = 0.0M,
										Notes = null,
										ItemNote = salesInvoiceDetailTotal.ItemNote,
										Packing = 0,
										ConsumerPrice = 0.0M,
										CostPrice = 0.0M,
										CostPackage = 0.0M,
										CostValue = 0.0M,
										SalesProfit = -salesInvoiceDetailTotal.FinalGrossValue * (memoValueBeforeTaxJournal.DebitValue / salesInvoiceHeaderTotal.FinalGrossValue),
										CreatedAt = clientCreditMemo.CreatedAt,
										UserNameCreated = clientCreditMemo.UserNameCreated,
										ModifiedAt = clientCreditMemo.ModifiedAt,
										UserNameModified = clientCreditMemo.UserNameModified,
									};

			var clientDebitMemos = from clientDebitMemo in _clientDebitMemoService.GetAll()
								   from taxJournal in _journalDetailService.GetAll().Where(x => x.JournalHeaderId == clientDebitMemo.JournalHeaderId && x.IsTax)
								   from memoValueBeforeTaxJournal in _journalDetailService.GetAll().Where(x => x.JournalDetailId == taxJournal.TaxParentId)
								   from salesInvoiceHeader in _salesInvoiceHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == clientDebitMemo.SalesInvoiceHeaderId)
								   from salesInvoiceHeaderTotal in salesInvoiceHeaderTotals.Where(x => x.SalesInvoiceHeaderId == clientDebitMemo.SalesInvoiceHeaderId)
								   from salesInvoiceDetailTotal in salesInvoiceDetailTotals.Where(x => x.SalesInvoiceHeaderId == clientDebitMemo.SalesInvoiceHeaderId)
								   from vatTax in _taxService.GetAllStoreVatTaxes().Where(x => x.StoreId == clientDebitMemo.StoreId).DefaultIfEmpty()
								   where (fromDate == null || clientDebitMemo.DocumentDate >= fromDate) &&
										 (toDate == null || clientDebitMemo.DocumentDate <= toDate)
								   select new ItemsTradingMovementDataDto
								   {
									   HeaderId = clientDebitMemo.SalesInvoiceHeaderId,
									   DocumentFullCode = clientDebitMemo.Prefix + clientDebitMemo.DocumentCode + clientDebitMemo.Suffix,
									   MenuCode = MenuCodeData.ClientDebitMemo,
									   TableId = 0,
									   DocumentDate = clientDebitMemo.DocumentDate,
									   EntryDate = clientDebitMemo.EntryDate,
									   InvoiceTypeId = salesInvoiceHeader.InvoiceTypeId,
									   ClientId = clientDebitMemo.ClientId,
									   SellerId = clientDebitMemo.SellerId,
									   Reference = clientDebitMemo.Reference,
									   RemarksAr = clientDebitMemo.RemarksAr,
									   RemarksEn = clientDebitMemo.RemarksEn,
									   CostCenterId = null,
									   VatTaxId = vatTax.TaxId,
									   VatTaxCode = vatTax.TaxCode,
									   VatTaxName = language == LanguageCode.Arabic ? vatTax.TaxNameAr : vatTax.TaxNameEn,
									   VatTaxAccountId = vatTax.DrAccount,
									   VatTaxAccountCode = vatTax.DrAccountCode,
									   VatTaxAccountName = vatTax.DrAccountName,
									   StoreId = clientDebitMemo.StoreId,
									   ItemId = salesInvoiceDetailTotal.ItemId,
									   ItemPackageId = salesInvoiceDetailTotal.ItemPackageId,
									   ExpireDate = salesInvoiceDetailTotal.ExpireDate,
									   BatchNumber = salesInvoiceDetailTotal.BatchNumber,
									   Price = 0,
									   Quantity = 0,
									   BonusQuantity = 0,
									   PurchaseQuantity = 0.0M,
									   SellingQuantity = 0.0M,
									   TotalValue = salesInvoiceDetailTotal.FinalGrossValue * (memoValueBeforeTaxJournal.CreditValue / salesInvoiceHeaderTotal.FinalGrossValue),
									   ItemDiscountPercent = 0,
									   ItemDiscountValue = 0,
									   TotalValueAfterDiscount = 0,
									   HeaderDiscountPercent = 0,
									   HeaderDiscountValue = 0,
									   GrossValue = salesInvoiceDetailTotal.FinalGrossValue * (memoValueBeforeTaxJournal.CreditValue / salesInvoiceHeaderTotal.FinalGrossValue),
									   PurchaseValue = 0.0M,
									   SalesValue = salesInvoiceDetailTotal.FinalGrossValue * (memoValueBeforeTaxJournal.CreditValue / salesInvoiceHeaderTotal.FinalGrossValue),
									   VatPercent = taxJournal.TaxPercent,
									   VatValue = salesInvoiceDetailTotal.FinalGrossValue * (taxJournal.CreditValue / salesInvoiceHeaderTotal.FinalGrossValue),
									   SubNetValue = salesInvoiceDetailTotal.FinalGrossValue * (clientDebitMemo.MemoValue / salesInvoiceHeaderTotal.FinalGrossValue),
									   OtherTaxValue = 0,
									   NetValue = salesInvoiceDetailTotal.FinalGrossValue * (clientDebitMemo.MemoValue / salesInvoiceHeaderTotal.FinalGrossValue),
									   ItemExpensePercent = 0.0M,
									   ItemExpenseValue = 0.0M,
									   Notes = null,
									   ItemNote = salesInvoiceDetailTotal.ItemNote,
									   Packing = 0,
									   ConsumerPrice = 0.0M,
									   CostPrice = 0.0M,
									   CostPackage = 0.0M,
									   CostValue = 0.0M,
									   SalesProfit = salesInvoiceDetailTotal.FinalGrossValue * (memoValueBeforeTaxJournal.CreditValue / salesInvoiceHeaderTotal.FinalGrossValue),
									   CreatedAt = clientDebitMemo.CreatedAt,
									   UserNameCreated = clientDebitMemo.UserNameCreated,
									   ModifiedAt = clientDebitMemo.ModifiedAt,
									   UserNameModified = clientDebitMemo.UserNameModified,
								   };

			var purchaseInvoiceHeaderTotals = _purchaseInvoiceHeaderService.GetAll().Select(x => new { x.PurchaseInvoiceHeaderId, x.GrossValue })
				.Concat(_purchaseInvoiceReturnHeaderService.GetAll().Select(x => new { x.PurchaseInvoiceHeaderId, GrossValue = -x.GrossValue }))
				.GroupBy(x => x.PurchaseInvoiceHeaderId).Select(g => new { PurchaseInvoiceHeaderId = g.Key, FinalGrossValue = g.Sum(x => x.GrossValue) });

			var purchaseInvoiceDetailTotals = _purchaseInvoiceDetailService.GetAll().Select(x => new { x.PurchaseInvoiceHeaderId, x.ItemId, x.ItemPackageId, ExpireDate = isGrouped ? null : x.ExpireDate, BatchNumber = isGrouped ? null : x.BatchNumber, x.ItemNote, x.GrossValue })
				.Concat(_purchaseInvoiceReturnDetailService.GetAll().Join(_purchaseInvoiceReturnHeaderService.GetAll(), x => x.PurchaseInvoiceReturnHeaderId, x => x.PurchaseInvoiceReturnHeaderId, (detail, header) => new { header.PurchaseInvoiceHeaderId, detail.ItemId, detail.ItemPackageId, detail.ExpireDate, detail.BatchNumber, detail.ItemNote, GrossValue = -detail.GrossValue }))
				.GroupBy(x => new { x.PurchaseInvoiceHeaderId, x.ItemId, x.ItemPackageId, x.ExpireDate, x.BatchNumber, x.ItemNote }).Select(g => new { g.Key.PurchaseInvoiceHeaderId, g.Key.ItemId, g.Key.ItemPackageId, g.Key.ExpireDate, g.Key.BatchNumber, g.Key.ItemNote, FinalGrossValue = g.Sum(x => x.GrossValue) });

			var supplierCreditMemos = from supplierCreditMemo in _supplierCreditMemoService.GetAll()
								      from taxJournal in _journalDetailService.GetAll().Where(x => x.JournalHeaderId == supplierCreditMemo.JournalHeaderId && x.IsTax)
								      from memoValueBeforeTaxJournal in _journalDetailService.GetAll().Where(x => x.JournalDetailId == taxJournal.TaxParentId)
									  from purchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == supplierCreditMemo.PurchaseInvoiceHeaderId)
									  from purchaseInvoiceHeaderTotal in purchaseInvoiceHeaderTotals.Where(x => x.PurchaseInvoiceHeaderId == supplierCreditMemo.PurchaseInvoiceHeaderId)
									  from purchaseInvoiceDetailTotal in purchaseInvoiceDetailTotals.Where(x => x.PurchaseInvoiceHeaderId == supplierCreditMemo.PurchaseInvoiceHeaderId)
									  from vatTax in _taxService.GetAllStoreVatTaxes().Where(x => x.StoreId == supplierCreditMemo.StoreId).DefaultIfEmpty()
									  where (fromDate == null || supplierCreditMemo.DocumentDate >= fromDate) &&
											(toDate == null || supplierCreditMemo.DocumentDate <= toDate)
									  select new ItemsTradingMovementDataDto
									  {
										  HeaderId = supplierCreditMemo.PurchaseInvoiceHeaderId,
										  DocumentFullCode = supplierCreditMemo.Prefix + supplierCreditMemo.DocumentCode + supplierCreditMemo.Suffix,
										  MenuCode = MenuCodeData.SupplierCreditMemo,
										  TableId = 0,
										  DocumentDate = supplierCreditMemo.DocumentDate,
										  EntryDate = supplierCreditMemo.EntryDate,
										  InvoiceTypeId = purchaseInvoiceHeader.InvoiceTypeId,
										  ClientId = null,
										  SellerId = null,
										  Reference = supplierCreditMemo.Reference,
										  RemarksAr = supplierCreditMemo.RemarksAr,
										  RemarksEn = supplierCreditMemo.RemarksEn,
										  CostCenterId = null,
										  VatTaxId = vatTax.TaxId,
										  VatTaxCode = vatTax.TaxCode,
										  VatTaxName = language == LanguageCode.Arabic ? vatTax.TaxNameAr : vatTax.TaxNameEn,
										  VatTaxAccountId = vatTax.DrAccount,
										  VatTaxAccountCode = vatTax.DrAccountCode,
										  VatTaxAccountName = vatTax.DrAccountName,
										  StoreId = supplierCreditMemo.StoreId,
										  ItemId = purchaseInvoiceDetailTotal.ItemId,
										  ItemPackageId = purchaseInvoiceDetailTotal.ItemPackageId,
										  ExpireDate = purchaseInvoiceDetailTotal.ExpireDate,
										  BatchNumber = purchaseInvoiceDetailTotal.BatchNumber,
										  Price = 0,
										  Quantity = 0,
										  BonusQuantity = 0,
										  PurchaseQuantity = 0.0M,
										  SellingQuantity = 0.0M,
										  TotalValue = purchaseInvoiceDetailTotal.FinalGrossValue * (memoValueBeforeTaxJournal.DebitValue / purchaseInvoiceHeaderTotal.FinalGrossValue),
										  ItemDiscountPercent = 0,
										  ItemDiscountValue = 0,
										  TotalValueAfterDiscount = 0,
										  HeaderDiscountPercent = 0,
										  HeaderDiscountValue = 0,
										  GrossValue = purchaseInvoiceDetailTotal.FinalGrossValue * (memoValueBeforeTaxJournal.DebitValue / purchaseInvoiceHeaderTotal.FinalGrossValue),
										  PurchaseValue = purchaseInvoiceDetailTotal.FinalGrossValue * (memoValueBeforeTaxJournal.DebitValue / purchaseInvoiceHeaderTotal.FinalGrossValue),
										  SalesValue = 0,
										  VatPercent = taxJournal.TaxPercent,
										  VatValue = purchaseInvoiceDetailTotal.FinalGrossValue * (taxJournal.DebitValue / purchaseInvoiceHeaderTotal.FinalGrossValue),
										  SubNetValue = purchaseInvoiceDetailTotal.FinalGrossValue * (supplierCreditMemo.MemoValue / purchaseInvoiceHeaderTotal.FinalGrossValue),
										  OtherTaxValue = 0,
										  NetValue = purchaseInvoiceDetailTotal.FinalGrossValue * (supplierCreditMemo.MemoValue / purchaseInvoiceHeaderTotal.FinalGrossValue),
										  ItemExpensePercent = 0.0M,
										  ItemExpenseValue = 0.0M,
										  Notes = null,
										  ItemNote = purchaseInvoiceDetailTotal.ItemNote,
										  Packing = 0,
										  ConsumerPrice = 0.0M,
										  CostPrice = 0.0M,
										  CostPackage = 0.0M,
										  CostValue = 0.0M,
										  SalesProfit = 0.0M,
										  CreatedAt = supplierCreditMemo.CreatedAt,
										  UserNameCreated = supplierCreditMemo.UserNameCreated,
										  ModifiedAt = supplierCreditMemo.ModifiedAt,
										  UserNameModified = supplierCreditMemo.UserNameModified,
									  };

			var supplierDebitMemos = from supplierDebitMemo in _supplierDebitMemoService.GetAll()
								     from taxJournal in _journalDetailService.GetAll().Where(x => x.JournalHeaderId == supplierDebitMemo.JournalHeaderId && x.IsTax)
								     from memoValueBeforeTaxJournal in _journalDetailService.GetAll().Where(x => x.JournalDetailId == taxJournal.TaxParentId)
									 from purchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == supplierDebitMemo.PurchaseInvoiceHeaderId)
									 from purchaseInvoiceHeaderTotal in purchaseInvoiceHeaderTotals.Where(x => x.PurchaseInvoiceHeaderId == supplierDebitMemo.PurchaseInvoiceHeaderId)
									 from purchaseInvoiceDetailTotal in purchaseInvoiceDetailTotals.Where(x => x.PurchaseInvoiceHeaderId == supplierDebitMemo.PurchaseInvoiceHeaderId)
									 from vatTax in _taxService.GetAllStoreVatTaxes().Where(x => x.StoreId == supplierDebitMemo.StoreId).DefaultIfEmpty()
									 where (fromDate == null || supplierDebitMemo.DocumentDate >= fromDate) &&
										   (toDate == null || supplierDebitMemo.DocumentDate <= toDate)
									 select new ItemsTradingMovementDataDto
									 {
										 HeaderId = supplierDebitMemo.PurchaseInvoiceHeaderId,
										 DocumentFullCode = supplierDebitMemo.Prefix + supplierDebitMemo.DocumentCode + supplierDebitMemo.Suffix,
										 MenuCode = MenuCodeData.SupplierDebitMemo,
										 TableId = 0,
										 DocumentDate = supplierDebitMemo.DocumentDate,
										 EntryDate = supplierDebitMemo.EntryDate,
										 InvoiceTypeId = purchaseInvoiceHeader.InvoiceTypeId,
										 ClientId = null,
										 SellerId = null,
										 Reference = supplierDebitMemo.Reference,
										 RemarksAr = supplierDebitMemo.RemarksAr,
										 RemarksEn = supplierDebitMemo.RemarksEn,
										 CostCenterId = null,
										 VatTaxId = vatTax.TaxId,
										 VatTaxCode = vatTax.TaxCode,
										 VatTaxName = language == LanguageCode.Arabic ? vatTax.TaxNameAr : vatTax.TaxNameEn,
										 VatTaxAccountId = vatTax.DrAccount,
										 VatTaxAccountCode = vatTax.DrAccountCode,
										 VatTaxAccountName = vatTax.DrAccountName,
										 StoreId = supplierDebitMemo.StoreId,
										 ItemId = purchaseInvoiceDetailTotal.ItemId,
										 ItemPackageId = purchaseInvoiceDetailTotal.ItemPackageId,
										 ExpireDate = purchaseInvoiceDetailTotal.ExpireDate,
										 BatchNumber = purchaseInvoiceDetailTotal.BatchNumber,
										 Price = 0,
										 Quantity = 0,
										 BonusQuantity = 0,
										 PurchaseQuantity = 0.0M,
										 SellingQuantity = 0.0M,
										 TotalValue = purchaseInvoiceDetailTotal.FinalGrossValue * (memoValueBeforeTaxJournal.CreditValue / purchaseInvoiceHeaderTotal.FinalGrossValue),
										 ItemDiscountPercent = 0,
										 ItemDiscountValue = 0,
										 TotalValueAfterDiscount = 0,
										 HeaderDiscountPercent = 0,
										 HeaderDiscountValue = 0,
										 GrossValue = purchaseInvoiceDetailTotal.FinalGrossValue * (memoValueBeforeTaxJournal.CreditValue / purchaseInvoiceHeaderTotal.FinalGrossValue),
										 PurchaseValue = -purchaseInvoiceDetailTotal.FinalGrossValue * (memoValueBeforeTaxJournal.CreditValue / purchaseInvoiceHeaderTotal.FinalGrossValue),
										 SalesValue = 0,
										 VatPercent = taxJournal.TaxPercent,
										 VatValue = purchaseInvoiceDetailTotal.FinalGrossValue * (taxJournal.CreditValue / purchaseInvoiceHeaderTotal.FinalGrossValue),
										 SubNetValue = purchaseInvoiceDetailTotal.FinalGrossValue * (supplierDebitMemo.MemoValue / purchaseInvoiceHeaderTotal.FinalGrossValue),
										 OtherTaxValue = 0,
										 NetValue = purchaseInvoiceDetailTotal.FinalGrossValue * (supplierDebitMemo.MemoValue / purchaseInvoiceHeaderTotal.FinalGrossValue),
										 ItemExpensePercent = 0.0M,
										 ItemExpenseValue = 0.0M,
										 Notes = null,
										 ItemNote = purchaseInvoiceDetailTotal.ItemNote,
										 Packing = 0,
										 ConsumerPrice = 0.0M,
										 CostPrice = 0.0M,
										 CostPackage = 0.0M,
										 CostValue = 0.0M,
										 SalesProfit = 0.0M,
										 CreatedAt = supplierDebitMemo.CreatedAt,
										 UserNameCreated = supplierDebitMemo.UserNameCreated,
										 ModifiedAt = supplierDebitMemo.ModifiedAt,
										 UserNameModified = supplierDebitMemo.UserNameModified,
									 };

			return purchaseInvoices.Concat(purchaseInvoiceReturns).Concat(salesInvoices).Concat(salesInvoiceReturns).Concat(clientCreditMemos).Concat(clientDebitMemos).Concat(supplierCreditMemos).Concat(supplierDebitMemos);
		}
	}
}
