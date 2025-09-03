using Shared.CoreOne.Contracts.Items;
using Compound.CoreOne.Contracts.Items;
using Compound.CoreOne.Models.Dtos.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne.Contracts.Inventory;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Identity;
using Purchases.CoreOne.Contracts;
using Shared.CoreOne.Contracts.Taxes;
using Sales.CoreOne.Contracts;
using Shared.CoreOne.Models.Dtos.ViewModels.Inventory;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using System.Text.RegularExpressions;
using Sales.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne.Models.Domain.Items;
using Sales.Service.Services;
using Shared.CoreOne.Models.Dtos.ViewModels.Items;
using Shared.CoreOne.Models.Domain.Inventory;
using Shared.CoreOne.Models.Domain.Modules;

namespace Compound.Service.Services.Items
{
	public class ItemHandlingService : IItemHandlingService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IItemService _itemService;
		private readonly IItemBarCodeService _itemBarCodeService;
		private readonly IItemCostService _itemCostService;
		private readonly IItemPackageService _itemPackageService;
		private readonly IPurchaseInvoiceService _purchaseInvoiceService;
		private readonly IItemBarCodeDetailService _itemBarCodeDetailService;
		private readonly ICompanyService _companyService;
		private readonly IBranchService _branchService;
		private readonly IStoreService _storeService;
		private readonly ITaxService _taxService;
		private readonly IItemTaxService _itemTaxService;
		private readonly ITaxPercentService _taxPercentService;
		private readonly ISalesInvoiceService _salesInvoiceService;
		private readonly IItemCurrentBalanceService _itemCurrentBalanceService;
		private readonly IItemPackingService _itemPackingService;
		private readonly ISalesInvoiceHeaderService _salesInvoiceHeaderService;
		private readonly ISalesInvoiceDetailService _salesInvoiceDetailService;

		public ItemHandlingService(IHttpContextAccessor httpContextAccessor, IItemService itemService, IItemBarCodeService itemBarCodeService, IItemCostService itemCostService, IItemPackageService itemPackageService, IPurchaseInvoiceService purchaseInvoiceService, IItemBarCodeDetailService itemBarCodeDetailService, ICompanyService companyService, IBranchService branchService, IStoreService storeService, ITaxService taxService, ITaxPercentService taxPercentService, IItemTaxService itemTaxService, ISalesInvoiceService salesInvoiceService, IItemCurrentBalanceService itemCurrentBalanceService,IItemPackingService itemPackingService,ISalesInvoiceHeaderService salesInvoiceHeaderService, ISalesInvoiceDetailService salesInvoiceDetailService)
		{
			_httpContextAccessor = httpContextAccessor;
			_itemService = itemService;
			_itemBarCodeService = itemBarCodeService;
			_itemCostService = itemCostService;
			_itemPackageService = itemPackageService;
			_purchaseInvoiceService = purchaseInvoiceService;
			_itemBarCodeDetailService = itemBarCodeDetailService;
			_companyService = companyService;
			_branchService = branchService;
			_storeService = storeService;
			_taxService = taxService;
			_taxPercentService = taxPercentService;
			_itemTaxService = itemTaxService;
			_salesInvoiceService = salesInvoiceService;
			_itemCurrentBalanceService = itemCurrentBalanceService;
			_itemPackingService = itemPackingService;
			_salesInvoiceHeaderService = salesInvoiceHeaderService;
			_salesInvoiceDetailService = salesInvoiceDetailService;
		}
		public async Task<IQueryable<ItemBalanceDto>> GetItemBalances(int storeId,bool isGrouped)
		{
			var invoices = (
				from salesInvoiceHeader in _salesInvoiceHeaderService.GetAll().Where(x => x.StoreId == storeId)
					.DefaultIfEmpty()
				from salesDetail in _salesInvoiceDetailService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeader.SalesInvoiceHeaderId)
					.OrderByDescending(x => x.SalesInvoiceDetailId)
				select salesDetail);
			var vatTaxId = (await _taxPercentService.GetVatByStoreId(storeId, DateTime.Now)).TaxId; //The date doesn't matter, I only want the taxId


			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var data =
			from itemCurrentBalance in _itemCurrentBalanceService.GetAll().Where(x=>x.StoreId == storeId)
			from item in _itemService.GetAll().Where(x => x.ItemId == itemCurrentBalance.ItemId)
			from store in _storeService.GetAll().Where(x => x.StoreId == itemCurrentBalance.StoreId)
			from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == itemCurrentBalance.ItemPackageId)
			from itemBarCode in _itemBarCodeService.GetAll().Where(x => x.ItemId == item.ItemId && x.FromPackageId == itemPackage.ItemPackageId)
			from itemBarCodeDetail in _itemBarCodeDetailService.GetAll().Where(x => x.ItemBarCodeId == itemBarCode.ItemBarCodeId).Take(1).DefaultIfEmpty()
			from itemPacking in _itemPackingService.GetAll().Where(x=>x.ItemId == item.ItemId && x.FromPackageId == itemPackage.ItemPackageId && x.ToPackageId == item.SingularPackageId).DefaultIfEmpty() 
			from itemCost in _itemCostService.GetAll().Where(x => x.ItemId == item.ItemId && x.StoreId == store.StoreId && x.ItemPackageId == itemPackage.ItemPackageId).DefaultIfEmpty()
			from salesDetail in invoices.Where(x => x.ItemId == itemBarCode.ItemId && x.ItemPackageId == itemBarCode.FromPackageId && x.ExpireDate == itemCurrentBalance.ExpireDate && x.BatchNumber == itemCurrentBalance.BatchNumber).OrderByDescending(x => x.SalesInvoiceDetailId).Take(1).DefaultIfEmpty()
			group new { item,store,itemPackage,itemCurrentBalance, itemBarCodeDetail, itemCost, itemPacking,salesDetail } by new { itemCurrentBalance.ItemId, item.ItemCode, item.ItemNameAr, item.ItemNameEn, store.StoreNameAr,store.StoreNameEn,itemCurrentBalance.ItemPackageId , itemPackage.PackageNameAr ,itemBarCode.IsSingularPackage, itemPackage.PackageNameEn, BatchNumber = isGrouped ? null : itemCurrentBalance.BatchNumber, ExpireDate = isGrouped ? null : itemCurrentBalance.ExpireDate,item.TaxTypeId ,item.ItemTypeId } into g
				orderby g.Key.ItemId
				select new ItemBalanceDto
				{
					ItemId = g.Key.ItemId,
					ItemCode = g.Key.ItemCode,
					TaxId = vatTaxId,
					TaxTypeId = g.Key.TaxTypeId,
					ItemTypeId = g.Key.ItemTypeId,
					ItemName = language == LanguageCode.Arabic ? g.Key.ItemNameAr : g.Key.ItemNameEn,
					StoreId = storeId,
					StoreName = language == LanguageCode.Arabic ? g.Key.StoreNameAr : g.Key.StoreNameEn,
					ItemPackageId = g.Key.ItemPackageId,
					ItemPackageName = language == LanguageCode.Arabic ? g.Key.PackageNameAr : g.Key.PackageNameEn,
					IsSingularPackage = g.Key.IsSingularPackage,
					BatchNumber = isGrouped ? null : string.IsNullOrWhiteSpace(g.Key.BatchNumber) ? null : g.Key.BatchNumber.Trim(),
					ExpireDate = isGrouped ? null : g.Key.ExpireDate,
					OpenQuantity = g.Sum(x => x.itemCurrentBalance.OpenQuantity),
					InQuantity = g.Sum(x=>x.itemCurrentBalance.InQuantity),
					OutQuantity = g.Sum(x => x.itemCurrentBalance.OutQuantity),
					PendingInQuantity = g.Sum(x => x.itemCurrentBalance.PendingInQuantity),
					PendingOutQuantity = g.Sum(x => x.itemCurrentBalance.PendingOutQuantity),
					CurrentBalance = g.Sum(x => x.itemCurrentBalance.OpenQuantity) + g.Sum(x => x.itemCurrentBalance.InQuantity) - g.Sum(x => x.itemCurrentBalance.OutQuantity),
					AvailableBalance = g.Sum(x => x.itemCurrentBalance.OpenQuantity) + g.Sum(x => x.itemCurrentBalance.InQuantity) - g.Sum(x => x.itemCurrentBalance.OutQuantity) - g.Sum(x => x.itemCurrentBalance.PendingOutQuantity),
					BarCode = g.Select(x => x.itemBarCodeDetail.BarCode).FirstOrDefault(),
					ConsumerPrice = g.Select(x => x.itemBarCodeDetail.ConsumerPrice).FirstOrDefault(),
					CostPrice = g.Select(x => x.itemCost.CostPrice).FirstOrDefault(),
					CostPackage = g.Select(x => x.itemCost.CostPrice).FirstOrDefault() * g.Select(x => x.itemPacking.Packing).FirstOrDefault(),
					LastCostPrice = g.Select(x => x.itemCost.LastCostPrice).FirstOrDefault(),
					LastPurchasePrice = g.Select(x => x.itemCost.LastPurchasePrice).FirstOrDefault(),
					Packing = g.Select(x => x.itemPacking.Packing).FirstOrDefault(),
					LastSalesPrice = g.Select(x=>x.salesDetail.SellingPrice).FirstOrDefault()
				};
			return data;
		}

		public async Task<IQueryable<ItemBalanceDto>> GetItemCard(int storeId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var invoices = (
				from salesInvoiceHeader in _salesInvoiceHeaderService.GetAll().Where(x => x.StoreId == storeId)
					.DefaultIfEmpty()
				from salesDetail in _salesInvoiceDetailService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeader.SalesInvoiceHeaderId)
					.OrderByDescending(x => x.SalesInvoiceDetailId)
				select salesDetail);
			var vatTaxId = (await _taxPercentService.GetVatByStoreId(storeId, DateTime.Now)).TaxId; //The date doesn't matter, I only want the taxId


			var companyId = await _storeService.GetCompanyIdByStoreId(storeId);
			var data =
				from item in _itemService.GetAll().Where(x => x.CompanyId == companyId)
				from store in _storeService.GetAll().Where(x => x.StoreId == storeId)
				from itemBarCode in _itemBarCodeService.GetAll().Where(x => x.ItemId == item.ItemId)
				from itemBarCodeDetail in _itemBarCodeDetailService.GetAll().Where(x => x.ItemBarCodeId == itemBarCode.ItemBarCodeId).Take(1).DefaultIfEmpty()
				from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == itemBarCode.FromPackageId)
				from itemPacking in _itemPackingService.GetAll().Where(x => x.ItemId == item.ItemId && x.FromPackageId == itemPackage.ItemPackageId && x.ToPackageId == item.SingularPackageId).DefaultIfEmpty()
				from itemCost in _itemCostService.GetAll().Where(x => x.ItemId == item.ItemId && x.StoreId == store.StoreId && x.ItemPackageId == itemPackage.ItemPackageId).DefaultIfEmpty()
				from salesDetail in invoices.Where(x => x.ItemId == itemBarCode.ItemId && x.ItemPackageId == itemBarCode.FromPackageId).OrderByDescending(x => x.SalesInvoiceDetailId).Take(1).DefaultIfEmpty()
				group new { item, store, itemPackage, itemBarCodeDetail, itemCost, itemPacking, salesDetail } by new { item.ItemId, item.ItemCode, item.ItemNameAr, item.ItemNameEn, store.StoreNameAr, store.StoreNameEn, itemPackage.ItemPackageId, itemPackage.PackageNameAr, itemPackage.PackageNameEn, item.TaxTypeId,item.ItemTypeId } into g
				orderby g.Key.ItemId
				select new ItemBalanceDto
				{
					ItemId = g.Key.ItemId,
					ItemCode = g.Key.ItemCode,
					TaxTypeId = g.Key.TaxTypeId,
					TaxId = vatTaxId,
					ItemTypeId = g.Key.ItemTypeId,
					ItemName = language == LanguageCode.Arabic ? g.Key.ItemNameAr : g.Key.ItemNameEn,
					StoreId = storeId,
					StoreName = language == LanguageCode.Arabic ? g.Key.StoreNameAr : g.Key.StoreNameEn,
					ItemPackageId = g.Key.ItemPackageId,
					ItemPackageName = language == LanguageCode.Arabic ? g.Key.PackageNameAr : g.Key.PackageNameEn,
					BarCode = g.Select(x => x.itemBarCodeDetail.BarCode).FirstOrDefault(),
					ConsumerPrice = g.Select(x => x.itemBarCodeDetail.ConsumerPrice).FirstOrDefault(),
					CostPrice = g.Select(x => x.itemCost.CostPrice).FirstOrDefault(),
					CostPackage = g.Select(x => x.itemCost.CostPrice).FirstOrDefault() * g.Select(x => x.itemPacking.Packing).FirstOrDefault(),
					LastCostPrice = g.Select(x => x.itemCost.LastCostPrice).FirstOrDefault(),
					LastPurchasePrice = g.Select(x => x.itemCost.LastPurchasePrice).FirstOrDefault(),
					Packing = g.Select(x => x.itemPacking.Packing).FirstOrDefault(),
					LastSalesPrice = g.Select(x => x.salesDetail.SellingPrice).FirstOrDefault()
				};
			return data;
		}

		public async Task<IQueryable<ItemAutoCompleteDto>> GetItemsByStoreId(int storeId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var companyId = _httpContextAccessor.GetCurrentUserCompany();
			var vatTaxId = (await _taxPercentService.GetVatByStoreId(storeId, DateTime.Now)).TaxId; //The date doesn't matter, I only want the taxId

			var data =
				(from item in _itemService.GetAll().Where(x => x.CompanyId == companyId)
				 from itemCost in _itemCostService.GetAll().Where(x => x.ItemId == item.ItemId).DefaultIfEmpty()
				 from company in _companyService.GetAll().Where(x => x.CompanyId == item.CompanyId)
				 from branch in _branchService.GetAll().Where(x => x.CompanyId == company.CompanyId)
				 from store in _storeService.GetAll().Where(x => x.BranchId == branch.BranchId && x.StoreId == storeId)
				 from itemBarCode in _itemBarCodeService.GetAll().Where(x => x.ItemId == item.ItemId && x.IsSingularPackage).DefaultIfEmpty()
				 from itemBarCodeDetail in _itemBarCodeDetailService.GetAll().Where(x => x.ItemBarCodeId == itemBarCode.ItemBarCodeId).Take(1).DefaultIfEmpty()
				 from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == itemBarCode.FromPackageId)
				 select new ItemAutoCompleteDto
				 {
					 ItemId = item.ItemId,
					 ItemCode = item.ItemCode,
					 ItemName = language == LanguageData.LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn,
					 ItemNameAr = item.ItemNameAr,
					 ItemNameEn = item.ItemNameEn,
					 ConsumerPrice = item.ConsumerPrice,
					 CostPrice = itemCost != null ? itemCost.CostPrice : 0,
					 TaxId = vatTaxId,
					 TaxTypeId = item.TaxTypeId,
					 ItemTypeId = item.ItemTypeId,
					 ItemPackageId = item.SingularPackageId,
					 ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
					 BarCode = itemBarCodeDetail.BarCode,
					 FromWhere = StaticData.ItemSearchFromWhere.ItemName
				 });
			return data;
		}

		public async Task<List<ItemPricesDto>> GetItemPrices(int storeId, int itemId, int packageId)
		{
			var companyId = _httpContextAccessor.GetCurrentUserCompany();
			return await (
					from item in _itemService.GetAll().Where(x => x.ItemId == itemId && x.CompanyId == companyId)
					from itemBarCode in _itemBarCodeService.GetAll().Where(x => x.ItemId == item.ItemId && x.FromPackageId == packageId)
					from itemBarCodeDetail in _itemBarCodeDetailService.GetAll().Where(x => x.ItemBarCodeId == itemBarCode.ItemBarCodeId)
					select new ItemPricesDto
					{
						BarCode = itemBarCodeDetail.BarCode,
						ConsumerPrice = itemBarCodeDetail.ConsumerPrice,
					}).ToListAsync();
		}

		public async Task<ItemAutoCompleteDto?> GetItemByBarCode(string? barCode, int storeId, DateTime? currentDate = null)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var companyId = _httpContextAccessor.GetCurrentUserCompany();
			var isItemsVatInclusive = await _itemService.IsItemsVatInclusive(storeId);
			var vatTaxId = (await _taxPercentService.GetVatByStoreId(storeId, DateTime.Now)).TaxId; //The date doesn't matter, I only want the taxId

			var data =
				await (
					from itemBarCodeDetail in _itemBarCodeDetailService.GetAll().Where(x => x.BarCode == barCode)
					from itemBarCode in _itemBarCodeService.GetAll().Where(x => x.ItemBarCodeId == itemBarCodeDetail.ItemBarCodeId)
					from item in _itemService.GetAll().Where(x => x.ItemId == itemBarCode.ItemId && x.CompanyId == companyId)
					from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == itemBarCode.FromPackageId)
					select new ItemAutoCompleteDto()
					{
						ItemId = item.ItemId,
						ItemName = language == LanguageData.LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn,
						ItemCode = item.ItemCode,
						BarCode = itemBarCodeDetail.BarCode,
						ItemPackageId = itemBarCode.FromPackageId,
						ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
						ConsumerPrice = itemBarCodeDetail.ConsumerPrice != 0 ? itemBarCodeDetail.ConsumerPrice : item.ConsumerPrice,
						TaxId = vatTaxId,
						TaxTypeId = item.TaxTypeId,
						ItemTypeId = item.ItemTypeId,
						IsItemVatInclusive = isItemsVatInclusive,
					}).FirstOrDefaultAsync();

			if (data != null)
			{
				var itemPrices = new List<ItemPricesDto>();
				var itemPrice = new ItemPricesDto() { BarCode = barCode, ConsumerPrice = data.ConsumerPrice };
				itemPrices.Add(itemPrice);

				var packing = await _itemBarCodeService.GetSingularItemPacking(data.ItemId.GetValueOrDefault(), data.ItemPackageId.GetValueOrDefault());
				var itemCurrentBalance = await _itemCurrentBalanceService.GetItemCurrentBalanceInfo(storeId, (int)data.ItemId!, (int)data.ItemPackageId!, null,null);

				data.Packages = await _itemBarCodeService.GetItemPackages(data.ItemId.GetValueOrDefault());
				data.CostPrice = await _itemCostService.GetAll().Where(x => x.ItemId == data.ItemId && x.StoreId == storeId).Select(x => x.CostPrice).FirstOrDefaultAsync();
				data.CostPackage = data.CostPrice * packing;
				data.LastPurchasePrice = await _purchaseInvoiceService.GetLastPurchasePrice((int)data.ItemId!, (int)data.ItemPackageId!);
				data.LastSalesPrice = await _salesInvoiceService.GetLastSalesPrice((int)data.ItemId!, (int)data.ItemPackageId!);
				data.Packing = packing;
				data.ItemTaxData = await _itemTaxService.GetItemTaxDataByItemId((int)data.ItemId, currentDate);
				data.CurrentBalance = itemCurrentBalance.CurrentBalance;
				data.AvailableBalance = itemCurrentBalance.AvailableBalance;
				data.ItemPrices = itemPrices;
				data.FromWhere = StaticData.ItemSearchFromWhere.BarCode;
				return data;
			}
			return new ItemAutoCompleteDto();
		}

		public async Task<ItemAutoCompleteDto?> GetItemSearchByItemId(int itemId, int storeId, DateTime? currentDate = null)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var companyId = _httpContextAccessor.GetCurrentUserCompany();
            var isItemsVatInclusive = await _itemService.IsItemsVatInclusive(storeId);
            var vatTaxId = (await _taxPercentService.GetVatByStoreId(storeId, DateTime.Now)).TaxId; //The date doesn't matter, I only want the taxId

			var data =
				await (from item in _itemService.GetAll().Where(x => x.ItemId == itemId && x.CompanyId == companyId)
					   from itemCost in _itemCostService.GetAll().Where(x => x.ItemId == item.ItemId && x.StoreId == storeId).DefaultIfEmpty()
					   from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == item.SingularPackageId)
					   select new ItemAutoCompleteDto()
					   {
						   ItemId = item.ItemId,
						   ItemName = language == LanguageData.LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn,
						   ItemCode = item.ItemCode,
						   ConsumerPrice = item.ConsumerPrice,
						   CostPrice = itemCost != null ? itemCost.CostPrice : 0,
						   CostPackage = itemCost != null ? itemCost.CostPrice : 0,
						   TaxId = vatTaxId,
						   TaxTypeId = item.TaxTypeId,
						   ItemTypeId = item.ItemTypeId,
						   ItemPackageId = item.SingularPackageId,
						   ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
						   IsItemVatInclusive = isItemsVatInclusive,
					   }).FirstOrDefaultAsync();

			if (data != null)
			{
				var itemCurrentBalance = await _itemCurrentBalanceService.GetItemCurrentBalanceInfoByItemAndPackageId(storeId, (int)data.ItemId!, (int)data.ItemPackageId!, true);
				data.Packages = await _itemBarCodeService.GetItemPackages(data.ItemId.GetValueOrDefault());
				data.Packing = 1;
				data.LastPurchasePrice = await _purchaseInvoiceService.GetLastPurchasePrice((int)data.ItemId!, (int)data.ItemPackageId!);
				data.LastSalesPrice = await _salesInvoiceService.GetLastSalesPrice((int)data.ItemId!, (int)data.ItemPackageId!);
				data.ItemTaxData = await _itemTaxService.GetItemTaxDataByItemId((int)data.ItemId!, currentDate);
				data.BarCode = await _itemBarCodeService.GetDefaultItemBarCode(itemId);
				data.ItemPrices = await GetItemPrices(storeId, itemId, data.ItemPackageId.GetValueOrDefault());
				data.CurrentBalance = itemCurrentBalance.CurrentBalance;
				data.AvailableBalance = itemCurrentBalance.AvailableBalance;
				data.FromWhere = StaticData.ItemSearchFromWhere.ItemId;
				return data;
			}
			return new ItemAutoCompleteDto();
		}


		public async Task<ItemAutoCompleteDto?> GetItemSearchByItemCode(string? itemCode, int storeId, DateTime? currentDate = null)
		{
			if (itemCode != null)
			{
				var language = _httpContextAccessor.GetProgramCurrentLanguage();
				var companyId = _httpContextAccessor.GetCurrentUserCompany();
                var isItemsVatInclusive = await _itemService.IsItemsVatInclusive(storeId);
                var vatTaxId = (await _taxPercentService.GetVatByStoreId(storeId, DateTime.Now)).TaxId; //The date doesn't matter, I only want the taxId

				var data =
					await (from item in _itemService.GetAll().Where(x => x.ItemCode!.Trim() == itemCode.Trim() && x.CompanyId == companyId)
						   from itemCost in _itemCostService.GetAll().Where(x => x.ItemId == item.ItemId && x.StoreId == storeId).DefaultIfEmpty()
						   from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == item.SingularPackageId)
						   select new ItemAutoCompleteDto()
						   {
							   ItemId = item.ItemId,
							   ItemName = language == LanguageData.LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn,
							   ItemCode = item.ItemCode,
							   ConsumerPrice = item.ConsumerPrice,
							   CostPrice = itemCost != null ? itemCost.CostPrice : 0,
							   CostPackage = itemCost != null ? itemCost.CostPrice : 0,
							   TaxId = vatTaxId,
							   TaxTypeId = item.TaxTypeId,
							   ItemTypeId = item.ItemTypeId,
							   ItemPackageId = item.SingularPackageId,
							   ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
							   IsItemVatInclusive = isItemsVatInclusive,
						   }).FirstOrDefaultAsync();
				if (data != null)
				{
					var itemCurrentBalance = await _itemCurrentBalanceService.GetItemCurrentBalanceInfoByItemAndPackageId(storeId, (int)data.ItemId!, (int)data.ItemPackageId!, true);

					data.Packing = 1;
					data.Packages = await _itemBarCodeService.GetItemPackages(data.ItemId.GetValueOrDefault());
					data.LastPurchasePrice = await _purchaseInvoiceService.GetLastPurchasePrice((int)data.ItemId!, (int)data.ItemPackageId!);
					data.LastSalesPrice = await _salesInvoiceService.GetLastSalesPrice((int)data.ItemId!, (int)data.ItemPackageId!);
					data.ItemTaxData = await _itemTaxService.GetItemTaxDataByItemId((int)data.ItemId!, currentDate);
					data.ItemPrices = await GetItemPrices(storeId, data.ItemId.GetValueOrDefault(), data.ItemPackageId.GetValueOrDefault());
					data.BarCode = await _itemBarCodeService.GetDefaultItemBarCode((int)data.ItemId!);
					data.CurrentBalance = itemCurrentBalance.CurrentBalance;
					data.AvailableBalance = itemCurrentBalance.AvailableBalance;
					data.FromWhere = StaticData.ItemSearchFromWhere.ItemCode;
					return data;
				}
			}
			return new ItemAutoCompleteDto();
		}

		public async Task<ItemAutoCompleteDto?> GetItemDataOnPackageChange(int itemId, int fromPackageId, int storeId, bool isGrouped, DateTime? currentDate = null, DateTime? expireDate = null, string? batchNumber = null)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var companyId = _httpContextAccessor.GetCurrentUserCompany();
            var isItemsVatInclusive = await _itemService.IsItemsVatInclusive(storeId);
			var itemCurrentBalance = isGrouped ? await _itemCurrentBalanceService.GetItemCurrentBalanceInfoByItemAndPackageId(storeId,itemId,fromPackageId,true) : await _itemCurrentBalanceService.GetItemCurrentBalanceInfo(storeId, itemId, fromPackageId, expireDate,batchNumber);
			var vatTaxId = (await _taxPercentService.GetVatByStoreId(storeId, DateTime.Now)).TaxId; //The date doesn't matter, I only want the taxId

			var packing = await _itemBarCodeService.GetSingularItemPacking(itemId, fromPackageId);
			var barCodeData =
				await (
				from item in _itemService.GetAll().Where(x => x.ItemId == itemId && x.CompanyId == companyId)
				from itemBarCode in _itemBarCodeService.GetAll().Where(x => x.ItemId == item.ItemId && x.FromPackageId == fromPackageId)
				from itemBarCodeDetail in _itemBarCodeDetailService.GetAll().Where(x => x.ItemBarCodeId == itemBarCode.ItemBarCodeId)
				from itemCost in _itemCostService.GetAll().Where(x => x.ItemId == itemId && x.ItemPackageId == item.SingularPackageId && x.StoreId == storeId).DefaultIfEmpty()
				from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == itemBarCode.FromPackageId)
				select new ItemAutoCompleteDto
				{
					ItemId = item.ItemId,
					ItemName = language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn,
					ItemCode = item.ItemCode,
					BarCode = itemBarCodeDetail.BarCode,
					ConsumerPrice = itemBarCodeDetail.ConsumerPrice,
					CostPrice = itemCost != null ? itemCost.CostPrice : 0,
					CostPackage = itemCost != null ? itemCost.CostPrice * packing : 0,
					ItemPackageId = fromPackageId,
					ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
					TaxId = vatTaxId,
					TaxTypeId = item.TaxTypeId,
					ItemTypeId = item.ItemTypeId,
					IsItemVatInclusive = isItemsVatInclusive

				}).FirstOrDefaultAsync();
			if (barCodeData != null)
			{
				return new ItemAutoCompleteDto()
				{
					Packages = await _itemBarCodeService.GetItemPackages(itemId),
					ConsumerPrice = barCodeData.ConsumerPrice,
					BarCode = barCodeData.BarCode,
					Packing = packing,
					ItemId = barCodeData.ItemId,
					ItemName = barCodeData.ItemName,
					ItemCode = barCodeData.ItemCode,
					CostPrice = barCodeData.CostPrice,
					CostPackage = barCodeData.CostPackage,
					ItemPackageId = barCodeData.ItemPackageId,
					ItemPackageName = barCodeData.ItemPackageName,
					TaxId = barCodeData.TaxId,
					TaxTypeId = barCodeData.TaxTypeId,
					ItemTypeId = barCodeData.ItemTypeId,
					LastPurchasePrice = await _purchaseInvoiceService.GetLastPurchasePrice((int)barCodeData.ItemId!, (int)barCodeData.ItemPackageId!),
					LastSalesPrice = await _salesInvoiceService.GetLastSalesPrice((int)barCodeData.ItemId!, (int)barCodeData.ItemPackageId!),
					ItemTaxData = await _itemTaxService.GetItemTaxDataByItemId(itemId, currentDate),
					ItemPrices = await GetItemPrices(storeId, itemId, fromPackageId),
					CurrentBalance= itemCurrentBalance.CurrentBalance,
					AvailableBalance = itemCurrentBalance.AvailableBalance,
					FromWhere = StaticData.ItemSearchFromWhere.PackageChange,
					IsItemVatInclusive = barCodeData.IsItemVatInclusive,
				};
			}
			return new ItemAutoCompleteDto() { Packing = packing };
		}

		public async Task<ItemPricesAutoCompleteDto?> GetItemPricesOnPackageChange(int itemId, int fromPackageId, int storeId)
		{
			var packing = await _itemBarCodeService.GetSingularItemPacking(itemId, fromPackageId);
			var costPrice = await _itemCostService.GetItemCostPriceByItemId(itemId, storeId);

			var prices = await GetItemPrices(storeId, itemId, fromPackageId);
			return new ItemPricesAutoCompleteDto()
			{
				Packing = packing,
				CostPrice = costPrice,
				CostPackage = costPrice * packing,
				ItemPrices = prices
			};
		}

		public async Task<ItemCostPriceValueDto?> GetItemCostPriceValue(int itemId, int itemPackageId, int storeId)
		{
			var itemCost = await _itemCostService.GetItemCostByItemId(itemId, storeId);

			return new ItemCostPriceValueDto()
			{
				ItemId = itemId,
				ItemPackageId = itemPackageId,
				CostPrice = itemCost != null ? itemCost.CostPrice : 0
			};
		}
	}
}
