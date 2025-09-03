using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Items;
using Shared.CoreOne.Models.Domain.Items;
using Shared.CoreOne.Models.Dtos.ViewModels.Items;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Helper.Models.Dtos;
using Shared.Service.Logic.Tree;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Shared.Service.Services.Items
{
	public class ItemBarCodeService : BaseService<ItemBarCode>, IItemBarCodeService
	{
		private readonly IItemPackingService _itemPackingService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IItemPackageService _itemPackageService;
		private readonly IItemBarCodeDetailService _itemBarCodeDetailService;
		public decimal CurrentPack { get; set; }

		public ItemBarCodeService(IRepository<ItemBarCode> repository, IItemPackingService itemPackingService, IHttpContextAccessor httpContextAccessor, IItemPackageService itemPackageService,IItemBarCodeDetailService itemBarCodeDetailService) : base(repository)
		{
			_itemPackingService = itemPackingService;
			_httpContextAccessor = httpContextAccessor;
			_itemPackageService = itemPackageService;
			_itemBarCodeDetailService = itemBarCodeDetailService;
		}

		public IQueryable<ItemBarCodeDto> GetAllItemBarCodes()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var data =
				from itemBarCode in _repository.GetAll()
				from fromPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == itemBarCode.FromPackageId)
				from toPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == itemBarCode.ToPackageId)
				from itemBarCodeDetail in _itemBarCodeDetailService.GetAll().Where(x=>x.ItemBarCodeId == itemBarCode.ItemBarCodeId).Take(1).DefaultIfEmpty()
				select new ItemBarCodeDto
				{
					ItemBarCodeId = itemBarCode.ItemBarCodeId,
					FromPackageId = itemBarCode.FromPackageId,
					ToPackageId = itemBarCode.ToPackageId,
					ItemId = itemBarCode.ItemId,
					Packing = itemBarCode.Packing,
					IsSingularPackage = itemBarCode.IsSingularPackage,
					FromPackageName = language == LanguageData.LanguageCode.Arabic ? fromPackage.PackageNameAr : fromPackage.PackageNameEn,
					ToPackageName = language == LanguageData.LanguageCode.Arabic ? toPackage.PackageNameAr : toPackage.PackageNameEn,
					ConsumerPrice = itemBarCodeDetail != null ? itemBarCodeDetail.ConsumerPrice : 0,
					BarCode = itemBarCodeDetail != null ? itemBarCodeDetail.BarCode : null,
				};
			return data;
		}

		public async Task<List<ItemBarCodeDto>> GetItemBarCodesByItemId(int itemId)
		{
			return await GetAllItemBarCodes().Where(x => x.ItemId == itemId).AsNoTracking().ToListAsync();
		}

		public async Task<string?> GetDefaultItemBarCode(int itemId)
		{
			 return 
			 await (from itemBarCode in _repository.GetAll().Where(x=>x.ItemId == itemId)
				from itemBarCodeDetail in _itemBarCodeDetailService.GetAll().Where(x => x.ItemBarCodeId == itemBarCode.ItemBarCodeId).Take(1).DefaultIfEmpty()
				select itemBarCodeDetail.BarCode).FirstOrDefaultAsync();
		}

		public Task<ItemBarCodeDto?> GetItemBarCodeById(int id)
		{
			return GetAllItemBarCodes().FirstOrDefaultAsync(x => x.ItemBarCodeId == id);
		}

		public async Task<List<PackageTreeDto>> ItemPackagesTree(int itemId)
		{
			var data = await _repository.GetAll().Where(x => x.ItemId == itemId).ToListAsync();
			var filteredData = data.Where(x => !x.IsSingularPackage).Select(x => new PackageTreeDto
			{
				PackageId = x.FromPackageId,
				MainPackageId = data.Where(z=>z.ToPackageId == x.FromPackageId).Select(x=>x.FromPackageId).FirstOrDefault(),
				Packing = x.Packing
			}).ToList();
			var filteredDataIds = filteredData.Select(x => x.PackageId).ToList();

			var theSingularId = data.Where(x => !filteredDataIds.Contains(x.FromPackageId)).Select(x => x.FromPackageId).FirstOrDefault();
			var singularRow = new PackageTreeDto()
			{
				PackageId = theSingularId,
				MainPackageId = data.Where(z => z.ToPackageId == theSingularId && !z.IsSingularPackage).Select(x => x.FromPackageId).FirstOrDefault(),
				Packing = 1
			};
			filteredData.Add(singularRow);

			var newData = PackageConversion.BuildPackages(filteredData);
			return newData;
		}

        public List<PackageTreeDto> ItemPackagesTreeFromModel(List<ItemBarCodeDto> barCodeDtos)
		{
            var data = barCodeDtos;
            var filteredData = data.Where(x => !x.IsSingularPackage).Select(x => new PackageTreeDto
            {
                PackageId = x.FromPackageId,
                MainPackageId = data.Where(z => z.ToPackageId == x.FromPackageId).Select(x => x.FromPackageId).FirstOrDefault(),
                Packing = x.Packing
            }).ToList();
            var filteredDataIds = filteredData.Select(x => x.PackageId).ToList();

            var theSingularId = data.Where(x => !filteredDataIds.Contains(x.FromPackageId)).Select(x => x.FromPackageId).FirstOrDefault();
            var singularRow = new PackageTreeDto()
            {
                PackageId = theSingularId,
                MainPackageId = data.Where(z => z.ToPackageId == theSingularId && !z.IsSingularPackage).Select(x => x.FromPackageId).FirstOrDefault(),
                Packing = 1
            };
            filteredData.Add(singularRow);

            var newData = PackageConversion.BuildPackages(filteredData);
            return newData;
        }

        public async Task<decimal> GetPacking(int itemId, int fromPackageId, int toPackageId)
		{
			return await _itemPackingService.GetItemPacking(itemId, fromPackageId, toPackageId);
		}

		public async Task<decimal> GetSingularItemPacking(int itemId, int fromPackageId)
		{
			var singularPackage = await _repository.GetAll().Where(x=>x.IsSingularPackage && x.ItemId == itemId).Select(x=>x.FromPackageId).FirstOrDefaultAsync();
			return await GetPacking(itemId, fromPackageId, singularPackage);
		}

		public async Task<List<int>> GetItemPackages(int itemId)
		{
			var data = await _repository.GetAll().Where(x => x.ItemId == itemId).Select(x => x.FromPackageId).ToListAsync();
			return data.Count > 0 ? data : new List<int>() { ItemPackageData.Each };
		}

		private async Task<List<int>> GetItemPackagesWithoutSingular(int itemId)
		{
			var data = await _repository.GetAll().Where(x => x.ItemId == itemId && !x.IsSingularPackage).Select(x => x.FromPackageId).ToListAsync();
			return data.Count > 0 ? data : new List<int>() { ItemPackageData.Each };
		}

		public async Task<List<ItemPackageLevelDto>> GetItemPackagesLevel(int itemId, int fromPackage, int toPackage)
		{
			var itemPackages = await GetPackagesInBetween(itemId,fromPackage, toPackage);
			//var itemPackages = packages != null
			//	? itemPackagesAll.Where(x => packages.Select(s => s.ItemPackageId).Contains(x.FromPackageId)).ToList()
			//	: itemPackagesAll;

			var packageLevels = new Dictionary<int, int>();

			var rootPackages = itemPackages
				.Select(ip => ip.FromPackageId)
				.Except(itemPackages.Select(ip => ip.ToPackageId))
				.Distinct();

			foreach (var rootPackage in rootPackages)
			{
				CalculateLevel(rootPackage, 1);
			}

			var newPackageLevels = itemPackages.Select(ip => new ItemPackageLevelDto
			{
				ItemPackageId = ip.FromPackageId,
				ItemPackageName = ip.FromPackageName,
				Level = packageLevels[ip.FromPackageId]
			}).ToList().OrderBy(x=>x.Level).ToList();

			var returnPackages = new List<ItemPackageLevelDto>();
			for (int i = 0; i < newPackageLevels.Count; i++)
			{
				if (i == 0 )
				{
					newPackageLevels[i].IsFirstLevel = true;
					returnPackages.Add(newPackageLevels[i]);
				}
				else if (i == 1 && newPackageLevels.Count >= 3)
				{
					newPackageLevels[i].IsSecondLevel = true;
					newPackageLevels[i].MainItemPackageId = itemPackages.Where(x => x.ToPackageId == newPackageLevels[i].ItemPackageId).Select(s => s.FromPackageId).FirstOrDefault();
					returnPackages.Add(newPackageLevels[i]);
					
				}
				else if (i == newPackageLevels.Count - 1)
				{
					newPackageLevels[i].IsLastLevel = true;
					newPackageLevels[i].MainItemPackageId = itemPackages.Where(z => z.ToPackageId == newPackageLevels[i].ItemPackageId && !z.IsSingularPackage).Select(x => x.FromPackageId).FirstOrDefault()
						;
					returnPackages.Add(newPackageLevels[i]);
				}
				else
				{
					newPackageLevels[i].MainItemPackageId = itemPackages.Where(x => x.ToPackageId == newPackageLevels[i].ItemPackageId).Select(s => s.FromPackageId).FirstOrDefault();
					returnPackages.Add(newPackageLevels[i]);
				}
			}


			return returnPackages;

			void CalculateLevel(int package, int level)
			{
				if (packageLevels.TryAdd(package, level))
				{
					var children = itemPackages.Where(ip => ip.FromPackageId == package).ToList();

					foreach (var child in children)
					{
						CalculateLevel(child.ToPackageId, level + 1);
					}
				}
			}
		}

		public async Task<List<ItemBarCodeDto>> GetPackagesInBetween(int itemId,int fromPackageId, int toPackageId)
		{
			var packages = await GetItemBarCodesByItemId(itemId);
			var result = new List<ItemBarCodeDto>();
			var currentPackage = packages.FirstOrDefault(p => p.FromPackageId == fromPackageId);

			while (currentPackage != null && currentPackage.ToPackageId >= toPackageId)
			{
				result.Add(currentPackage);

				if (currentPackage.ToPackageId == toPackageId)
				{
					var toPackage = packages.FirstOrDefault(p => p.FromPackageId == toPackageId);
					if (toPackage != null) result.Add(toPackage);
					break;
				}
				

				currentPackage = packages.FirstOrDefault(p => p.FromPackageId == currentPackage.ToPackageId);
			}

			return result;
		}

		public async Task<List<ItemPackageDropDownDto>> GetItemPackagesDropDown(int itemId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var itemPackages = await GetItemPackages(itemId);
			var packages =await _itemPackageService.GetAll().Where(x=> itemPackages.Contains(x.ItemPackageId)).Select(s=> new ItemPackageDropDownDto
			{
				ItemPackageId = s.ItemPackageId,
				ItemPackageName = language == LanguageData.LanguageCode.Arabic ? s.PackageNameAr : s.PackageNameEn
			}).ToListAsync();
			return packages ?? [];
		}

		public async Task<List<ItemPackageDropDownDto>> GetItemPackagesWithoutSingularDropDown(int itemId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var itemPackages = await GetItemPackagesWithoutSingular(itemId);
			var packages = await _itemPackageService.GetAll().Where(x => itemPackages.Contains(x.ItemPackageId)).Select(s => new ItemPackageDropDownDto
			{
				ItemPackageId = s.ItemPackageId,
				ItemPackageName = language == LanguageData.LanguageCode.Arabic ? s.PackageNameAr : s.PackageNameEn
			}).ToListAsync();
			return packages ?? [];
		}

		public async Task<List<ItemPackagesDto>> GetItemsPackages(List<int> itemId)
		{
			var modelList = new List<ItemPackagesDto>();
			var data = await _repository.GetAll().Where(x => itemId.Contains(x.ItemId)).Select(x => new
			{
				ItemId = x.ItemId,
				PackageId = x.FromPackageId
			}).ToListAsync();
			var dataGrouped = data.GroupBy(x => x.ItemId).ToList();
			foreach (var item in dataGrouped)
			{
				var model = new ItemPackagesDto
				{
					ItemId = item.Key,
					Packages = item.Select(x => x.PackageId).ToList()
				};
				modelList.Add(model);
			}
			return modelList;
		}

		public async Task<List<ItemBarCodeDetailDto>> SaveItemBarCodes(List<ItemBarCodeDto> barCodes, int itemId)
		{
			if (barCodes.Any())
			{
				if (itemId == 0)
				{
					return await CreateItemBarCode(barCodes, itemId);
				}
				else
				{
					await DeleteItemBarCode(barCodes, itemId);
					var createResult = await CreateItemBarCode(barCodes, itemId);
					var updateResult = await UpdateItemBarCode(barCodes, itemId);
					return createResult.Union(updateResult).ToList();
				}
			}
			return new List<ItemBarCodeDetailDto>();
		}

		public async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.ItemBarCodeId) + 1; } catch { id = 1; }
			return id;
		}
		public async Task<List<ItemBarCodeDetailDto>> CreateItemBarCode(List<ItemBarCodeDto> barCodes, int itemId)
		{
			var barCodeList = new List<ItemBarCode>();
			var barCodeDetailList = new List<ItemBarCodeDetailDto>();
			var newId = await GetNextId();
			foreach (var barCode in barCodes)
			{
				if (barCode.ItemBarCodeId <= 0)
				{
					var newPercent = new ItemBarCode()
					{
						ItemBarCodeId = newId,
						ItemId = itemId,
						FromPackageId = barCode.FromPackageId,
						ToPackageId = barCode.ToPackageId,
						Packing = barCode.Packing,
						IsSingularPackage = barCode.IsSingularPackage,
						CreatedAt = DateHelper.GetDateTimeNow(),
						UserNameCreated = await _httpContextAccessor!.GetUserName(),
						IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
						Hide = false,
					};
					barCodeList.Add(newPercent);
					if (barCode.ItemBarCodeDetails != null)
					{
						var barCodeDetails = barCode.ItemBarCodeDetails.Select(c =>
						{
							c.ItemBarCodeId = newId;
							c.IsSingularPackage = barCode.IsSingularPackage;
							return c;
						}).ToList();
						barCodeDetailList.AddRange(barCodeDetails);
					}
					newId++;
				}
				
			}

			if (barCodeList.Any())
			{
				await _repository.InsertRange(barCodeList);
				await _repository.SaveChanges();
				return barCodeDetailList;
			}
			return new List<ItemBarCodeDetailDto>();
		}
		public async Task<List<ItemBarCodeDetailDto>> UpdateItemBarCode(List<ItemBarCodeDto> barCodes, int itemId)
		{
			var currentPercents = barCodes.Where(x => x.ItemBarCodeId > 0 && x.ItemId > 0).ToList();
			var barCodeList = new List<ItemBarCode>();
			var barCodeDetailList = new List<ItemBarCodeDetailDto>();
			foreach (var barCode in currentPercents)
			{
				if (barCode.ItemBarCodeId > 0)
				{
					var newNote = new ItemBarCode()
					{
						ItemBarCodeId = barCode.ItemBarCodeId,
						ItemId = itemId,
						FromPackageId = barCode.FromPackageId,
						ToPackageId = barCode.ToPackageId,
						Packing = barCode.Packing,
						IsSingularPackage = barCode.IsSingularPackage,
						ModifiedAt = DateHelper.GetDateTimeNow(),
						UserNameModified = await _httpContextAccessor!.GetUserName(),
						IpAddressModified = _httpContextAccessor?.GetIpAddress(),
						Hide = false
					};
					barCodeList.Add(newNote);
					if (barCode.ItemBarCodeDetails != null)
					{
						var barCodeDetails = barCode.ItemBarCodeDetails.Select(c =>
						{
							c.ItemBarCodeId = barCode.ItemBarCodeId;
							c.IsSingularPackage = barCode.IsSingularPackage;
							return c;
						}).ToList();
						barCodeDetailList.AddRange(barCodeDetails);
					}
				}
			}

			if (barCodeList.Any())
			{
				_repository.UpdateRange(barCodeList);
				await _repository.SaveChanges();
				return barCodeDetailList;
			}
			return new List<ItemBarCodeDetailDto>();
		}
		public async Task<bool> DeleteItemBarCode(List<ItemBarCodeDto> barCodes, int itemId)
		{
			if (barCodes.Any())
			{
				var currentPercents = await _repository.GetAll().Where(x => x.ItemId == itemId).AsNoTracking().ToListAsync();
				var toBeDeleted = currentPercents.Where(p => barCodes.All(p2 => p2.ItemBarCodeId != p.ItemBarCodeId)).ToList();
				if (toBeDeleted.Any())
				{
					await DeleteItemBarCodeDetails(toBeDeleted.Select(x => x.ItemBarCodeId).ToList());
					_repository.RemoveRange(toBeDeleted);
					await _repository.SaveChanges();
					return true;
				}
			}
			return false;
		}

		public async Task<bool> DeleteItemBarCodeDetails(List<int> barCodes)
		{
			if (barCodes.Any())
			{
				var barCodeDetails = await _itemBarCodeDetailService.GetAll().Where(x => barCodes.Contains(x.ItemBarCodeId)).ToListAsync();
				await _itemBarCodeDetailService.RemoveRange(barCodeDetails);
				return true;
			}
			return false;
		}

		public async Task<ResponseDto> DeleteItemBarCodesByItemId(int itemId)
		{
			var data = await _repository.GetAll().Where(x => x.ItemId == itemId).ToListAsync();
			if (data.Any())
			{
				_repository.RemoveRange(data);
				await _repository.SaveChanges();
				return new ResponseDto() { Success = true };
			}
			return new ResponseDto() { Success = false };
		}
	}
}
