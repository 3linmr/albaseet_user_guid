using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json.Linq;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Items;
using Shared.CoreOne.Models.Domain.Items;
using Shared.CoreOne.Models.Dtos.ViewModels.Items;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Identity;
using Shared.Helper.Models.Dtos;
using Shared.Service.Logic.Tree;

namespace Shared.Service.Services.Items
{
	public class ItemPackingService : BaseService<ItemPacking>, IItemPackingService
	{
		private readonly IStringLocalizer<ItemPackingService> _localizer;
		private readonly IItemPackageService _itemPackageService;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public ItemPackingService(IRepository<ItemPacking> repository, IStringLocalizer<ItemPackingService> localizer, IItemPackageService itemPackageService, IHttpContextAccessor httpContextAccessor) : base(repository)
		{
			_localizer = localizer;
			_itemPackageService = itemPackageService;
			_httpContextAccessor = httpContextAccessor;
		}


		public IQueryable<ItemPackingVm> GetItemPacking(int itemId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var data = 
				from itemPacking in _repository.GetAll().Where(x=>x.ItemId == itemId)
				from fromPackage in _itemPackageService.GetAll().Where(x=>x.ItemPackageId  == itemPacking.FromPackageId)
				from toPackage in _itemPackageService.GetAll().Where(x=>x.ItemPackageId  == itemPacking.ToPackageId)
				select new ItemPackingVm
				{
					ItemPackingId = itemPacking.ItemPackingId,
					FromPackageId = itemPacking.FromPackageId,
					ToPackageId = itemPacking.ToPackageId,
					Packing = itemPacking.Packing,
					FromPackageName = language == LanguageData.LanguageCode.Arabic ? fromPackage.PackageNameAr : fromPackage.PackageNameEn,
					ToPackageName = language == LanguageData.LanguageCode.Arabic ? toPackage.PackageNameAr : toPackage.PackageNameEn,
				};
			return data;
		}

		public async Task<ResponseDto> UpdateItemPackings(int itemId, List<ItemBarCodeDto> itemBarCodes)
		{
			await DeleteItemPackings(itemId);

			List<ItemPacking>? newItemPackings = PackageConversion.GenerateTransitivePackings(itemId, itemBarCodes);

			if (newItemPackings == null)
			{
				return new ResponseDto { Success = false, Message = _localizer["CircularDependency"] };
			}

			var nextId = await GetNextId();
			newItemPackings.ForEach(x => x.ItemPackingId = nextId++);

			if (newItemPackings.Any())
			{
				await _repository.InsertRange(newItemPackings);
				await _repository.SaveChanges();
			}

			return new ResponseDto { Success = true };
		}

		public async Task<decimal> GetItemPacking(int itemId, int fromPackageId, int toPackageId)
		{
			return await _repository.GetAll().Where(x => x.ItemId == itemId && x.FromPackageId == fromPackageId && x.ToPackageId == toPackageId).Select(x => x.Packing).FirstOrDefaultAsync();
		}

		public async Task<List<ItemPackageVm>> GetItemSiblingPackages(int itemId, int packageId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var data = await 
				(from itemPacking in _repository.GetAll().Where(x => x.ItemId == itemId && x.FromPackageId == packageId && x.ToPackageId != packageId)
				 from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == itemPacking.ToPackageId)
				 select new ItemPackageVm()
				 {
					 ItemPackageId = itemPacking.ToPackageId,
					 ItemPackageName = language == LanguageData.LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
					 Packing = itemPacking.Packing
				 }).ToListAsync();
			return data;
		}

		public async Task<List<ItemPackageDropDownDto>> GetItemSiblingPackagesDropDown(int itemId, int packageId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var data = await
			(from itemPacking in _repository.GetAll().Where(x => x.ItemId == itemId && x.FromPackageId == packageId && x.ToPackageId != packageId)
				from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == itemPacking.ToPackageId)
				select new ItemPackageDropDownDto()
				{
					ItemPackageId = itemPacking.ToPackageId,
					ItemPackageName = language == LanguageData.LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
				}).ToListAsync();
			return data;
		}

		public async Task<bool> DeleteItemPackings(int itemId)
		{
			var packings = await _repository.GetAll().Where(x => x.ItemId == itemId).ToListAsync();

			if (packings.Any())
			{
				_repository.RemoveRange(packings);
				await _repository.SaveChanges();
				return true;
			}

			return false;
		}

		public async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.ItemPackingId) + 1; } catch { id = 1; }
			return id;
		}
	}
}
