using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Approval;
using Shared.CoreOne.Contracts.Items;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Contracts.Settings;
using Shared.CoreOne.Contracts.Taxes;
using Shared.CoreOne.Models.Domain.Items;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Domain.Taxes;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.CoreOne.Models.Dtos.ViewModels.Items;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Extensions;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Helper.Models.Dtos;
using Shared.Service.Logic.Approval;
using Shared.Service.Services.Taxes;
using Shared.Service.Validators;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using static Shared.Helper.Models.StaticData.LanguageData;


namespace Shared.Service.Services.Items
{
	public class ItemService : BaseService<Item>, IItemService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IItemCategoryService _itemCategoryService;
		private readonly IItemSubCategoryService _itemSubCategoryService;
		private readonly IItemSectionService _itemSectionService;
		private readonly IItemSubSectionService _itemSubSectionService;
		private readonly IItemAttributeService _itemAttributeService;
		private readonly IMainItemService _mainItemService;
		private readonly IItemBarCodeService _itemBarCodeService;
		private readonly ICompanyService _companyService;
		private readonly IItemTypeService _itemTypeService;
		private readonly IVendorService _vendorService;
		private readonly IStringLocalizer<ItemService> _localizer;
		private readonly IMenuNoteService _menuNoteService;
		private readonly IItemPackageService _itemPackageService;
		private readonly IItemBarCodeDetailService _itemBarCodeDetailService;
		private readonly ITaxTypeService _taxTypeService;
		private readonly IItemPackingService _itemPackingService;
		private readonly IApplicationSettingService _applicationSettingService;
		private readonly IItemTaxService _itemTaxService;
		private readonly ITaxService _taxService;
		private readonly IStoreService _storeService;
		private const int MenuCode = MenuCodeData.Item;

		public ItemService(IRepository<Item> repository, IHttpContextAccessor httpContextAccessor, IItemCategoryService itemCategoryService, IItemSubCategoryService itemSubCategoryService, IItemSectionService itemSectionService, IItemSubSectionService itemSubSectionService, IItemAttributeService itemAttributeService, IMainItemService mainItemService, IItemBarCodeService itemBarCodeService, ICompanyService companyService, IItemTypeService itemTypeService, IVendorService vendorService, IStringLocalizer<ItemService> localizer, IMenuNoteService menuNoteService, IItemPackageService itemPackageService, IItemBarCodeDetailService itemBarCodeDetailService, ITaxTypeService taxTypeService, IItemPackingService itemPackingService, IApplicationSettingService applicationSettingService, IItemTaxService itemTaxService, ITaxService taxService , IStoreService storeService) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
			_itemCategoryService = itemCategoryService;
			_itemSubCategoryService = itemSubCategoryService;
			_itemSectionService = itemSectionService;
			_itemSubSectionService = itemSubSectionService;
			_itemAttributeService = itemAttributeService;
			_mainItemService = mainItemService;
			_itemBarCodeService = itemBarCodeService;
			_companyService = companyService;
			_itemTypeService = itemTypeService;
			_vendorService = vendorService;
			_localizer = localizer;
			_menuNoteService = menuNoteService;
			_itemPackageService = itemPackageService;
			_itemBarCodeDetailService = itemBarCodeDetailService;
			_taxTypeService = taxTypeService;
			_itemPackingService = itemPackingService;
			_applicationSettingService = applicationSettingService;
			_itemTaxService = itemTaxService;
			_taxService = taxService;
			_storeService = storeService;
		}

		public IQueryable<ItemDto> GetAllItems()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var data =
				from item in _repository.GetAll()
				from company in _companyService.GetAll().Where(x => x.CompanyId == item.CompanyId)
				from itemType in _itemTypeService.GetAll().Where(x => x.ItemTypeId == item.ItemTypeId).DefaultIfEmpty()
				from itemCategory in _itemCategoryService.GetAll().Where(x => x.ItemCategoryId == item.ItemCategoryId).DefaultIfEmpty()
				from itemSubCategory in _itemSubCategoryService.GetAll().Where(x => x.ItemSubCategoryId == item.ItemSubCategoryId).DefaultIfEmpty()
				from itemSection in _itemSectionService.GetAll().Where(x => x.ItemSectionId == item.ItemSectionId).DefaultIfEmpty()
				from itemSubSection in _itemSubSectionService.GetAll().Where(x => x.ItemSubSectionId == item.ItemSubSectionId).DefaultIfEmpty()
				from mainItem in _mainItemService.GetAll().Where(x => x.MainItemId == item.MainItemId).DefaultIfEmpty()
				from vendor in _vendorService.GetAll().Where(x => x.VendorId == item.VendorId).DefaultIfEmpty()
				from taxType in _taxTypeService.GetAll().Where(x => x.TaxTypeId == item.TaxTypeId).DefaultIfEmpty()
				from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == item.SingularPackageId)
				select new ItemDto()
				{
					ItemId = item.ItemId,
					ItemName = language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn,
					ItemNameAr = item.ItemNameAr,
					ItemNameEn = item.ItemNameEn,
					CompanyId = item.CompanyId,
					ConsumerPrice = item.ConsumerPrice,
					CoverageQuantity = item.CoverageQuantity,
					InternalPrice = item.InternalPrice,
					MaxBuyQuantity = item.MaxBuyQuantity,
					ReorderPointQuantity = item.ReorderPointQuantity,
					PurchasingPrice = item.PurchasingPrice,
					SalesAccountId = item.SalesAccountId,
					PurchaseAccountId = item.PurchaseAccountId,
					MinSellQuantity = item.MinSellQuantity,
					MinBuyQuantity = item.MinBuyQuantity,
					MaxSellQuantity = item.MaxSellQuantity,
					MaxDiscountPercent = item.MaxDiscountPercent,
					ItemCode = item.ItemCode,
					ItemLocation = item.ItemLocation,
					ItemCategoryId = item.ItemCategoryId,
					ItemSubCategoryId = item.ItemSubCategoryId,
					ItemSectionId = item.ItemSectionId,
					ItemSubSectionId = item.ItemSubSectionId,
					MainItemId = item.MainItemId,
					ItemTypeId = item.ItemTypeId,
					TaxTypeId = item.TaxTypeId,
					SingularPackageId = item.SingularPackageId,
					VendorId = item.VendorId,
					IsActive = item.IsActive,
					IsBatched = item.IsBatched,
					IsDeficit = item.IsDeficit,
					IsExpired = item.IsExpired,
					IsGifts = item.IsGifts,
					IsNoStock = item.IsNoStock,
					IsOnline = item.IsOnline,
					IsPoints = item.IsPoints,
					IsPos = item.IsPos,
					IsPromoted = item.IsPromoted,
					IsUnderSelling = item.IsUnderSelling,
					IsUntradeable = item.IsUntradeable,
					InActiveReasons = item.InActiveReasons,
					NoReplenishment = item.NoReplenishment,
					ArchiveHeaderId = item.ArchiveHeaderId,

					ItemCategoryName = itemCategory != null ? language == LanguageCode.Arabic ? itemCategory.CategoryNameAr : itemCategory.CategoryNameEn : null,
					ItemSubCategoryName = itemSubCategory != null ? language == LanguageCode.Arabic ? itemSubCategory.SubCategoryNameAr : itemSubCategory.SubCategoryNameEn : null,
					ItemSectionName = itemSection != null ? language == LanguageCode.Arabic ? itemSection.SectionNameAr : itemSection.SectionNameEn : null,
					ItemSubSectionName = itemSubSection != null ? language == LanguageCode.Arabic ? itemSubSection.SubSectionNameAr : itemSubSection.SubSectionNameEn : null,
					MainItemName = mainItem != null ? language == LanguageCode.Arabic ? mainItem.MainItemNameAr : mainItem.MainItemNameEn : null,
					CompanyName = company != null ? language == LanguageCode.Arabic ? company.CompanyNameAr : company.CompanyNameEn : null,
					VendorName = vendor != null ? language == LanguageCode.Arabic ? vendor.VendorNameAr : vendor.VendorNameEn : null,
					TaxTypeName = vendor != null ? language == LanguageCode.Arabic ? taxType.TaxTypeNameAr : taxType.TaxTypeNameEn : null,
					ItemTypeName = itemType != null ? language == LanguageCode.Arabic ? itemType.ItemTypeNameAr : itemType.ItemTypeNameEn : null,
					SingularPackageName = itemPackage != null ? language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn : null,
					CreatedAt = item.CreatedAt,
					ModifiedAt = item.ModifiedAt,
					UserNameCreated = item.UserNameCreated,
					UserNameModified = item.UserNameModified,
				};
			return data;
		}

        public IQueryable<ItemDto> GetUserItems()
		{
			var companyId = _httpContextAccessor.GetCurrentUserCompany();
			return GetAllItems().Where(x => x.CompanyId == companyId);
		}

		public async Task<List<ItemAutoCompleteVm>> GetItemsAutoComplete(string? term)
		{
			var companyId = _httpContextAccessor.GetCurrentUserCompany();
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			if (!string.IsNullOrWhiteSpace(term))
			{
				term = term.Trim().ToLower();
				var data = await _repository.GetAll()
					.Where(x => x.CompanyId == companyId && x.IsActive)
					.Select(x => new
					{
						x.ItemId,
						x.ItemCode,
						ItemName = language == LanguageCode.Arabic ? x.ItemNameAr! : x.ItemNameEn!
					})
					.Where(x => (x.ItemCode + " - " + x.ItemName).ToLower().Contains(term))
					.Take(10)
					.ToListAsync();

				return data.Select(x => new ItemAutoCompleteVm
				{
					ItemId = x.ItemId,
					ItemCode = x.ItemCode,
					ItemName = $"{x.ItemCode} - {x.ItemName}"
				}).ToList();
			}

			return new List<ItemAutoCompleteVm>();
		}

		public async Task<List<ItemAutoCompleteVm>> GetItemsAutoCompleteByStoreIds(string term, List<int> storeIds)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var companyIds = _storeService.GetAllStores().Where(x => storeIds.Contains(x.StoreId)).Select(x => x.CompanyId);

			if (!string.IsNullOrWhiteSpace(term))
			{
				term = term.Trim().ToLower();
				var data = await _repository.GetAll()
					.Where(x => companyIds.Contains(x.CompanyId) && x.IsActive)
					.Select(x => new
					{
						x.ItemId,
						x.ItemCode,
						ItemName = language == LanguageCode.Arabic ? x.ItemNameAr! : x.ItemNameEn!
					})
					.Where(x => (x.ItemCode + " - " + x.ItemName).ToLower().Contains(term))
					.Take(10)
					.ToListAsync();

				return data.Select(x => new ItemAutoCompleteVm
				{
					ItemId = x.ItemId,
					ItemCode = x.ItemCode,
					ItemName = $"{x.ItemCode} - {x.ItemName}"
				}).ToList();
			}

			return new List<ItemAutoCompleteVm>();
		}

		public Task<ItemDto?> GetItemById(int id)
		{
			return GetAllItems().FirstOrDefaultAsync(x => x.ItemId == id);
		}

		public async Task<List<ItemBarCodeDto>> GetItemBarCodeByItemId(int itemId)
		{
			var modelList = new List<ItemBarCodeDto>();
			var barCodeDetail = await GetItemBarCodeDetailByItemId(itemId);
			var itemBarCodes = await _itemBarCodeService.GetItemBarCodesByItemId(itemId);
			foreach (var itemBarCode in itemBarCodes)
			{
				var model = new ItemBarCodeDto()
				{
					ItemId = itemBarCode.ItemId,
					FromPackageId = itemBarCode.FromPackageId,
					ToPackageId = itemBarCode.ToPackageId,
					ItemBarCodeId = itemBarCode.ItemBarCodeId,
					Packing = itemBarCode.Packing,
					FromPackageName = itemBarCode.FromPackageName,
					ToPackageName = itemBarCode.ToPackageName,
					IsSingularPackage = itemBarCode.IsSingularPackage,
					ConsumerPrice = itemBarCode.ConsumerPrice,
					BarCode = itemBarCode.BarCode,
					ItemBarCodeDetails = barCodeDetail.Where(x => x.ItemBarCodeId == itemBarCode.ItemBarCodeId).ToList()
				};
				modelList.Add(model);
			}
			return modelList;
		}

        public Task<int> GetItemItemIdByBarCode(string barCode)
        {
            return 
                (from itemBarCode in _itemBarCodeService.GetAll()
                from itemBarCodeDetail in _itemBarCodeDetailService.GetAll().Where(x=>x.BarCode.Trim() == barCode.Trim())
                select itemBarCode.ItemId).FirstOrDefaultAsync();
        }

        public async Task<List<ItemBarCodeDetailDto>> GetItemBarCodeDetailByItemId(int itemId)
		{
			var data =
				await (from itemBarCode in _itemBarCodeService.GetAll().Where(x => x.ItemId == itemId)
					   from itemBarCodeDetail in _itemBarCodeDetailService.GetAll().Where(x => x.ItemBarCodeId == itemBarCode.ItemBarCodeId).DefaultIfEmpty()
					   select new ItemBarCodeDetailDto
					   {
						   ItemBarCodeId = itemBarCodeDetail != null ? itemBarCodeDetail.ItemBarCodeId : 0,
						   ItemBarCodeDetailId = itemBarCodeDetail != null ? itemBarCodeDetail.ItemBarCodeDetailId : 0,
						   ConsumerPrice = itemBarCodeDetail != null ? itemBarCodeDetail.ConsumerPrice : 0,
						   BarCode = itemBarCodeDetail != null ? itemBarCodeDetail.BarCode : null
					   }).ToListAsync();
			return data;
		}

		public async Task<bool> DeleteItemBarCodeDetailByItemId(int itemId)
		{
			var data =
				await (from itemBarCode in _itemBarCodeService.GetAll().Where(x => x.ItemId == itemId)
					   from itemBarCodeDetail in _itemBarCodeDetailService.GetAll().Where(x => x.ItemBarCodeId == itemBarCode.ItemBarCodeId)
					   select itemBarCodeDetail).ToListAsync();
			if (data.Any())
			{
				await _itemBarCodeDetailService.RemoveRange(data);
			}
			return true;
		}

        public IQueryable<ItemTaxDto> GetItemTaxesByItemId(int itemId)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var itemTaxes = (
                    from itemTax in _itemTaxService.GetAll().Where(x => x.ItemId == itemId)
					from item in _repository.GetAll().Where(x => x.ItemId == itemTax.ItemId)
                    from tax in _taxService.GetAll().Where(x => x.TaxId == itemTax.TaxId)
                    select new ItemTaxDto
                    {
						ItemTaxId = itemTax.ItemTaxId,
                        ItemId = itemTax.ItemId,
                        ItemName = language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn,
                        TaxId = itemTax.TaxId,
                        TaxName = language == LanguageCode.Arabic ? tax.TaxNameAr : tax.TaxNameEn,
                    }
            );

            return itemTaxes;
        }


        public async Task<ItemVm> GetItem(int id)
		{
			var item = await GetAllItems().AsNoTracking().FirstOrDefaultAsync(x => x.ItemId == id);
			if (item != null)
			{
				var itemBarCode = await GetItemBarCodeByItemId(id);
				var itemAttributes = await _itemAttributeService.GetItemAttributesByItemId(id).AsNoTrackingWithIdentityResolution().ToListAsync();
				var itemNotes = await _menuNoteService.GetMenuNotes(MenuCodeData.Item, id).AsNoTracking().ToListAsync();
				var barCodeDetails = await GetItemBarCodeDetailByItemId(id);
				var itemTaxes = await GetItemTaxesByItemId(id).ToListAsync();
				item.TaxesList = itemTaxes.Select(x => x.TaxId).ToJson();
				return new ItemVm()
				{
					Item = item ?? new ItemDto(),
					BarCodes = itemBarCode,
					BarCodesDetails = barCodeDetails,
					Attributes = itemAttributes,
					MenuNotes = itemNotes,
					ItemTaxes = itemTaxes,
				};
			}
			else
			{
				return new ItemVm();
			}
		}

		public async Task<bool> IsItemsVatInclusive(int storeId)
		{
			var flagValue = await _applicationSettingService.GetApplicationSettingValueByStoreId(storeId, ApplicationSettingDetailData.ItemsVatInclusive);
			return flagValue == "1";
		}

		public async Task<ResponseDto> SaveItem(ItemDto item)
		{
			var isItemExist = await IsItemExist(item.ItemId, item.CompanyId, item.ItemNameAr, item.ItemNameEn);
			if (isItemExist.Success)
			{
				return new ResponseDto() { Id = isItemExist.Id, Success = false, Message = _localizer["ItemAlreadyExist"] };
			}
			else
			{
				if (item!.ItemId == 0)
				{
					return await CreateItem(item);
				}
				else
				{
					return await UpdateItem(item);
				}
			}
		}

		public async Task<ResponseDto> SaveItemInFull(ItemVm item)
		{
			var result = await SaveItem(item.Item);
			if (result.Success)
			{
				await SaveItemTaxes(result.Id,item.Item.TaxesList);
				await _itemAttributeService.SaveItemAttributes(item.Attributes, result.Id);

				var barCodeValidationResult = ValidateItemBarCodes(item.BarCodes!);
				if (barCodeValidationResult.Success == false) return barCodeValidationResult;

				var barCodeDetails = await _itemBarCodeService.SaveItemBarCodes(item.BarCodes, result.Id);
				var success = await _itemBarCodeDetailService.SaveItemBarCodeDetails(barCodeDetails, item.Item.ItemCode, item.Item.CompanyId, (barCodes, itemCompanyId) => IsBarCodeExist(barCodes, itemCompanyId));
				if (success)
				{
					await _menuNoteService.SaveMenuNotes(item.MenuNotes, result.Id);

					var packingResult = await _itemPackingService.UpdateItemPackings(result.Id, item.BarCodes);
					if (packingResult.Success == false) return packingResult;
					return result;
				}
				else
				{
					return new ResponseDto() { Id = item.Item.ItemId, Success = false, Message = _localizer["BarCodeAlreadyExist"] };
				}
			}
			return new ResponseDto() { Id = item.Item.ItemId, Success = false, Message = result.Message ?? _localizer["SomethingWentWrong"] };
		}

		private ResponseDto ValidateItemBarCodes(List<ItemBarCodeDto> itemBarCodes)
		{
			ResponseDto result;

			result = CheckOnlyOneSinglePackageAndSelfReference(itemBarCodes);
			if (result.Success == false) return result;

			result = CheckFromPackageUnique(itemBarCodes);
			if (result.Success == false) return result;

			result = CheckEveryPackageHasChild(itemBarCodes);
			if (result.Success == false) return result;

			return new ResponseDto { Success = true };
		}

		//This function was moved from itemBarCodeDetailService to here to avoid the circular dependency problem
		public async Task<bool> IsBarCodeExist(List<ItemBarCodeDetail> barCodes, int itemCompanyId)
		{
			var itemBarCodes = barCodes.Where(x => x.BarCode?.Trim() != null || !string.IsNullOrWhiteSpace(x.BarCode)).ToList();
			var allBarCodes = barCodes.Select(x => x.BarCode).ToList();
			var detailIds = itemBarCodes.Select(x => x.ItemBarCodeDetailId).ToList();

			var otherBarCodes = from item in _repository.GetAll().AsNoTracking()
								from itemBarCode in _itemBarCodeService.GetAll().AsNoTracking().Where(x => x.ItemId == item.ItemId)
								from itemBarCodeDetail in _itemBarCodeDetailService.GetAll().AsNoTracking().Where(x => x.ItemBarCodeId == itemBarCode.ItemBarCodeId)
								where !detailIds.Contains(itemBarCodeDetail.ItemBarCodeDetailId) && itemBarCodeDetail.BarCode != null && item.CompanyId == itemCompanyId
								select itemBarCodeDetail;

			var barCodeExists = await otherBarCodes.AnyAsync(x => allBarCodes.Contains(x.BarCode!.Trim()));
			return barCodeExists;
		}

		private ResponseDto CheckOnlyOneSinglePackageAndSelfReference(List<ItemBarCodeDto> itemBarCodes)
		{
			//make sure only one package is marked as single and has self reference, and that no other
			//package is either marked as single or has self reference
			var singlePackageCount = itemBarCodes.Count(x => x.IsSingularPackage && x.FromPackageId == x.ToPackageId);
			var partialSinglePackageCount = itemBarCodes.Count(x => x.IsSingularPackage || x.FromPackageId == x.ToPackageId);

			if (singlePackageCount != 1 || partialSinglePackageCount != 1)
			{
				return new ResponseDto { Success = false, Message = _localizer["OnlyOneSinglePackage"] };
			}
			return new ResponseDto() { Success = true };
		}

		private ResponseDto CheckFromPackageUnique(List<ItemBarCodeDto> itemBarCodes)
		{
			var fromPackageCounts = itemBarCodes.GroupBy(x => x.FromPackageId).Select(x => new { x.Key, Count = x.Count() });

			if (fromPackageCounts.Where(x => x.Count > 1).Any())
			{
				return new ResponseDto { Success = false, Message = _localizer["FromPackagesNotUnique"] };
			}
			return new ResponseDto() { Success = true };
		}

		private ResponseDto CheckEveryPackageHasChild(List<ItemBarCodeDto> itemBarCodes)
		{
			var childrenPackageCounts = itemBarCodes.GroupJoin(itemBarCodes, parentPackage => parentPackage.ToPackageId, childPackage => childPackage.FromPackageId, (parentPackage, childrenPackages) => new {parentPackage, Count=childrenPackages.Count()});

			if (childrenPackageCounts.Where(x => x.Count == 0).Any())
			{
				return new ResponseDto { Success = false, Message = _localizer["NoChildPackage"] };
			}
			return new ResponseDto() { Success = true };
		}

		public async Task<bool> SaveItemTaxes(int itemId,string? taxesList)
		{
			if (taxesList != null)
			{
				var itemTaxes = JsonConvert.DeserializeObject<List<int>>(taxesList);
				if (itemTaxes != null)
				{
					var taxList = new List<ItemTaxDto>();
					foreach (var itemTax in itemTaxes)
					{
						var tax = new ItemTaxDto()
						{
							ItemId = itemId,
							TaxId = itemTax,
							ItemTaxId = 0
						};
						taxList.Add(tax);
					}
					await _itemTaxService.SaveItemTaxes(taxList, itemId);
				}
			}
			return true;
		}

		public List<RequestChangesDto> GetItemRequestChanges(ItemVm oldItem, ItemVm newItem)
		{
			var requestChanges = new List<RequestChangesDto>();
			var items = CompareLogic.GetDifferences(oldItem.Item, newItem.Item);
			requestChanges.AddRange(items);

			if (oldItem.BarCodes != null && oldItem.BarCodes.Any() && newItem.BarCodes != null && newItem.BarCodes.Any())
			{
				var oldCount = oldItem.BarCodes.Count;
				var newCount = newItem.BarCodes.Count;
				var oldBarCodes = oldItem.BarCodes.OrderBy(x => x.ItemBarCodeId).ToList();
				var newBarCodes = newItem.BarCodes.OrderBy(x => x.ItemBarCodeId).ToList();
				var index = 0;
				if (oldCount <= newCount)
				{
					for (int i = index; i < oldCount; i++)
					{
						for (int j = index; j < newCount; j++)
						{
							//oldBarCodes[i].ItemBarCodeDetails = null;
							//newBarCodes[i].ItemBarCodeDetails = null;
							var changes = CompareLogic.GetDifferences(oldBarCodes[i], newBarCodes[i]);
							requestChanges.AddRange(changes.Where(x => x.ColumnName != "ItemBarCodeDetails"));
							index++;
							break;
						}
					}
				}
			}

			if (oldItem.BarCodesDetails != null && oldItem.BarCodesDetails.Any() && newItem.BarCodesDetails != null && newItem.BarCodesDetails.Any())
			{
				var oldCount = oldItem.BarCodesDetails.Count;
				var newCount = newItem.BarCodesDetails.Count;
				var oldBarCodes = oldItem.BarCodesDetails.OrderBy(x => x.ItemBarCodeId).ToList();
				var newBarCodes = newItem.BarCodesDetails.OrderBy(x => x.ItemBarCodeId).ToList();
				var index = 0;
				if (oldCount <= newCount)
				{
					for (int i = index; i < oldCount; i++)
					{
						for (int j = index; j < newCount; j++)
						{
							var changes = CompareLogic.GetDifferences(oldBarCodes[i], newBarCodes[i]);
							requestChanges.AddRange(changes);
							index++;
							break;
						}
					}
				}
			}

			if (oldItem.Attributes != null && oldItem.Attributes.Any() && newItem.Attributes != null && newItem.Attributes.Any())
			{
				var oldCount = oldItem.Attributes.Count;
				var newCount = newItem.Attributes.Count;
				var index = 0;
				if (oldCount <= newCount)
				{
					for (int i = index; i < oldCount; i++)
					{
						for (int j = index; j < newCount; j++)
						{
							var changes = CompareLogic.GetDifferences(oldItem.Attributes[i], newItem.Attributes[i]);
							requestChanges.AddRange(changes);
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

            if (oldItem.ItemTaxes != null && oldItem.ItemTaxes.Any() && newItem.ItemTaxes != null && newItem.ItemTaxes.Any())
            {
                var oldCount = oldItem.ItemTaxes.Count;
                var newCount = newItem.ItemTaxes.Count;
                var index = 0;
                if (oldCount <= newCount)
                {
                    for (int i = index; i < oldCount; i++)
                    {
                        for (int j = index; j < newCount; j++)
                        {
                            var changes = CompareLogic.GetDifferences(oldItem.ItemTaxes[i], newItem.ItemTaxes[i]);
                            requestChanges.AddRange(changes);
                            index++;
                            break;
                        }
                    }
                }
            }
            return requestChanges;
		}

		public async Task<ResponseDto> IsItemExist(int id, int companyId, string? nameAr, string? nameEn)
		{
			var item = await _repository.GetAll().FirstOrDefaultAsync(x => (x.ItemNameAr == nameAr || x.ItemNameEn == nameEn || x.ItemNameAr == nameEn || x.ItemNameEn == nameAr) && x.ItemId != id && x.CompanyId == companyId);
			if (item != null)
			{
				return new ResponseDto() { Id = item.ItemId, Success = true };
			}
			return new ResponseDto() { Id = 0, Success = false };
		}

		public async Task<ResponseDto> IsItemCodeExist(int id, int companyId, string? itemCode)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var item = await _repository.GetAll().FirstOrDefaultAsync(x => x.ItemCode!.Trim() == itemCode!.Trim() && x.ItemId != id && x.CompanyId == companyId);
			if (item != null)
			{
				return new ResponseDto() { Id = item.ItemId, Success = true, Message = _localizer["ItemCodeAlreadyExist", ((language == LanguageCode.Arabic ? item?.ItemNameAr : item?.ItemNameEn) ?? "")] };
			}
			return new ResponseDto() { Id = 0, Success = false };
		}

		public async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.ItemId) + 1; } catch { id = 1; }
			return id;
		}

		public async Task<ResponseDto> CreateItem(ItemDto item)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var itemId = await GetNextId();
			var newItem = new Item()
			{
				ItemId = itemId,
				ItemNameAr = item?.ItemNameAr?.Trim(),
				ItemNameEn = item?.ItemNameEn?.Trim(),
				CompanyId = item!.CompanyId,
				ConsumerPrice = item.ConsumerPrice,
				CoverageQuantity = item.CoverageQuantity,
				InternalPrice = item.InternalPrice,
				MaxBuyQuantity = item.MaxBuyQuantity,
				ReorderPointQuantity = item.ReorderPointQuantity,
				PurchasingPrice = item.PurchasingPrice,
				SalesAccountId = item.SalesAccountId,
				PurchaseAccountId = item.PurchaseAccountId,
				MinSellQuantity = item.MinSellQuantity,
				MinBuyQuantity = item.MinBuyQuantity,
				MaxSellQuantity = item.MaxSellQuantity,
				MaxDiscountPercent = item.MaxDiscountPercent,
				ItemCode = string.IsNullOrEmpty(item.ItemCode?.Trim()) ? itemId.ToString() : item.ItemCode?.Trim(),
				ItemLocation = item.ItemLocation,
				ItemCategoryId = item.ItemCategoryId == 0 ? null : item.ItemCategoryId,
				ItemSubCategoryId = item.ItemSubCategoryId == 0 ? null : item.ItemSubCategoryId,
				ItemSectionId = item.ItemSectionId == 0 ? null : item.ItemSectionId,
				ItemSubSectionId = item.ItemSubSectionId == 0 ? null : item.ItemSubSectionId,
				MainItemId = item.MainItemId == 0 ? null : item.MainItemId,
				ItemTypeId = item.ItemTypeId,
				TaxTypeId = item.TaxTypeId,
				SingularPackageId = item.SingularPackageId,
				VendorId = item.VendorId == 0 ? null : item.VendorId,
				IsActive = item.IsActive,
				IsBatched = item.IsBatched,
				IsDeficit = item.IsDeficit,
				IsExpired = item.IsExpired,
				IsGifts = item.IsGifts,
				IsNoStock = item.IsNoStock,
				IsOnline = item.IsOnline,
				IsPoints = item.IsPoints,
				IsPos = item.IsPos,
				IsPromoted = item.IsPromoted,
				IsUnderSelling = item.IsUnderSelling,
				IsUntradeable = item.IsUntradeable,
				InActiveReasons = item.InActiveReasons,
				NoReplenishment = item.NoReplenishment,
				ArchiveHeaderId = item.ArchiveHeaderId,
				CreatedAt = DateHelper.GetDateTimeNow(),
				UserNameCreated = await _httpContextAccessor!.GetUserName(),
				IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
				Hide = false,
			};

			item.ItemCode = newItem.ItemCode;

			var itemValidator = await new ItemValidator(_localizer).ValidateAsync(newItem);
			var validationResult = itemValidator.IsValid;
			if (validationResult)
			{
				var itemCodeExist = await IsItemCodeExist(item.ItemId, item.CompanyId, newItem.ItemCode);
				if (!itemCodeExist.Success)
				{
					await _repository.Insert(newItem);
					await _repository.SaveChanges();
					return new ResponseDto() { Id = newItem.ItemId, Success = true, Message = _localizer["NewItemSuccessMessage", ((language == LanguageCode.Arabic ? newItem.ItemNameAr : newItem.ItemNameEn) ?? ""), newItem.ItemId] };
				}
				else
				{
					return new ResponseDto() { Id = newItem.ItemId, Success = false, Message = itemCodeExist.Message };
				}
			}
			else
			{
				return new ResponseDto() { Id = newItem.ItemId, Success = false, Message = itemValidator.ToString("~") };
			}
		}

		public async Task<ResponseDto> UpdateItem(ItemDto item)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var itemDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.ItemId == item.ItemId);
			if (itemDb != null)
			{
				itemDb.ItemNameAr = item.ItemNameAr?.Trim();
				itemDb.ItemNameEn = item.ItemNameEn?.Trim();
				itemDb.CompanyId = item!.CompanyId;
				itemDb.ConsumerPrice = item.ConsumerPrice;
				itemDb.CoverageQuantity = item.CoverageQuantity;
				itemDb.InternalPrice = item.InternalPrice;
				itemDb.MaxBuyQuantity = item.MaxBuyQuantity;
				itemDb.ReorderPointQuantity = item.ReorderPointQuantity;
				itemDb.PurchasingPrice = item.PurchasingPrice;
				itemDb.SalesAccountId = item.SalesAccountId;
				itemDb.PurchaseAccountId = item.PurchaseAccountId;
				itemDb.MinSellQuantity = item.MinSellQuantity;
				itemDb.MinBuyQuantity = item.MinBuyQuantity;
				itemDb.MaxSellQuantity = item.MaxSellQuantity;
				itemDb.MaxDiscountPercent = item.MaxDiscountPercent;
				itemDb.ItemCode = item.ItemCode;
				itemDb.ItemLocation = item.ItemLocation;
				itemDb.ItemCategoryId = item.ItemCategoryId == 0 ? null : item.ItemCategoryId;
				itemDb.ItemSubCategoryId = item.ItemSubCategoryId == 0 ? null : item.ItemSubCategoryId;
				itemDb.ItemSectionId = item.ItemSectionId == 0 ? null : item.ItemSectionId;
				itemDb.ItemSubSectionId = item.ItemSubSectionId == 0 ? null : item.ItemSubSectionId;
				itemDb.MainItemId = item.MainItemId == 0 ? null : item.MainItemId;
				itemDb.ItemTypeId = item.ItemTypeId;
				itemDb.TaxTypeId = item.TaxTypeId;
				itemDb.SingularPackageId = item.SingularPackageId;
				itemDb.VendorId = item.VendorId == 0 ? null : item.VendorId;
				itemDb.IsActive = item.IsActive;
				itemDb.IsBatched = item.IsBatched;
				itemDb.IsDeficit = item.IsDeficit;
				itemDb.IsExpired = item.IsExpired;
				itemDb.IsGifts = item.IsGifts;
				itemDb.IsNoStock = item.IsNoStock;
				itemDb.IsOnline = item.IsOnline;
				itemDb.IsPoints = item.IsPoints;
				itemDb.IsPos = item.IsPos;
				itemDb.IsPromoted = item.IsPromoted;
				itemDb.IsUnderSelling = item.IsUnderSelling;
				itemDb.IsUntradeable = item.IsUntradeable;
				itemDb.InActiveReasons = item.InActiveReasons;
				itemDb.NoReplenishment = item.NoReplenishment;
				itemDb.ArchiveHeaderId = item.ArchiveHeaderId;
				itemDb.ModifiedAt = DateHelper.GetDateTimeNow();
				itemDb.UserNameModified = await _httpContextAccessor!.GetUserName();
				itemDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();

				var itemValidator = await new ItemValidator(_localizer).ValidateAsync(itemDb);
				var validationResult = itemValidator.IsValid;
				if (validationResult)
				{
					var itemCodeExist = await IsItemCodeExist(item.ItemId, item.CompanyId, itemDb.ItemCode);
					if (!itemCodeExist.Success)
					{
						_repository.Update(itemDb);
						await _repository.SaveChanges();
						return new ResponseDto() { Id = itemDb.ItemId, Success = true, Message = _localizer["UpdateItemSuccessMessage", ((language == LanguageCode.Arabic ? itemDb.ItemNameAr : itemDb.ItemNameEn) ?? "")] };
					}
					else
					{
						return new ResponseDto() { Id = itemDb.ItemId, Success = false, Message = itemCodeExist.Message };
					}
				}
				else
				{
					return new ResponseDto() { Id = 0, Success = false, Message = itemValidator.ToString("~") };
				}
			}
			return new ResponseDto() { Id = 0, Success = false, Message = _localizer["NoItemFound"] };
		}

		public async Task<ResponseDto> DeleteItem(int id)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var itemDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.ItemId == id);
			if (itemDb != null)
			{
				_repository.Delete(itemDb);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = id, Success = true, Message = _localizer["DeleteItemSuccessMessage", ((language == LanguageCode.Arabic ? itemDb.ItemNameAr : itemDb.ItemNameEn) ?? "")] };
			}
			return new ResponseDto() { Id = id, Success = false, Message = _localizer["NoItemFound"] };
		}

		public async Task<ResponseDto> DeleteItemInFull(int id)
		{
			await _itemAttributeService.DeleteItemAttributesByItemId(id);
			await DeleteItemBarCodeDetailByItemId(id);
			await _itemBarCodeService.DeleteItemBarCodesByItemId(id);
			await _itemPackingService.DeleteItemPackings(id);
			await _menuNoteService.DeleteMenuNotes(MenuCodeData.Item, id);
			await _itemTaxService.DeleteItemTaxesByItemId(id);
			return await DeleteItem(id);
		}
	}
}
