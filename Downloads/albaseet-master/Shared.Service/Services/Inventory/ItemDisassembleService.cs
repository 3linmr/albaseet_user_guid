using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Inventory;
using Shared.CoreOne.Contracts.Items;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Domain.Inventory;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Inventory;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Helper.Models.Dtos;
using Shared.Service.Validators;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Shared.Service.Services.Inventory
{
	public class ItemDisassembleService : BaseService<ItemDisassemble>, IItemDisassembleService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IStringLocalizer<ItemDisassembleService> _localizer;
		private readonly IItemService _itemService;
		private readonly IItemPackageService _itemPackageService;
		private readonly IStoreService _storeService;

		public ItemDisassembleService(IRepository<ItemDisassemble> repository, IHttpContextAccessor httpContextAccessor, IStringLocalizer<ItemDisassembleService> localizer,IItemService itemService,IItemPackageService itemPackageService,IStoreService storeService) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
			_localizer = localizer;
			_itemService = itemService;
			_itemPackageService = itemPackageService;
			_storeService = storeService;
		}

		public IQueryable<ItemDisassembleDto> GetItemDisassembles()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var data =
				from itemDisassemble in _repository.GetAll()
				from item in _itemService.GetAll().Where(x => x.ItemId == itemDisassemble.ItemId)
				from store in _storeService.GetAll().Where(x => x.StoreId == itemDisassemble.StoreId)
				from package in _itemPackageService.GetAll().Where(x => x.ItemPackageId == itemDisassemble.ItemPackageId)
				orderby itemDisassemble.ItemDisassembleId descending
				select new ItemDisassembleDto()
				{
					ItemDisassembleId = itemDisassemble.ItemDisassembleId,
					ItemDisassembleHeaderId = itemDisassemble.ItemDisassembleHeaderId,
					StoreId = itemDisassemble.StoreId,
					ItemId = itemDisassemble.ItemId,
					BatchNumber = itemDisassemble.BatchNumber,
					ExpireDate = itemDisassemble.ExpireDate,
					EntryDate = itemDisassemble.EntryDate,
					ItemName = language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn,
					StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
					ItemCode = item.ItemCode,
					UserNameCreated = itemDisassemble.UserNameCreated,
					InQuantity = itemDisassemble.InQuantity,
					OutQuantity = itemDisassemble.OutQuantity,
					ItemPackageId = itemDisassemble.ItemPackageId,
					ItemPackageName = language == LanguageCode.Arabic ? package.PackageNameAr : package.PackageNameEn
				};
			return data;
		}

		public IQueryable<ItemDisassembleDto> GetItemDisassembles(int storeId)
		{
			return GetItemDisassembles().Where(x => x.StoreId == storeId);
		}

		public async Task<List<ItemDisassembleDto>> GetItemDisassembleByHeaderId(int itemDisassembleHeaderId)
		{
			return await GetItemDisassembles().Where(x => x.ItemDisassembleHeaderId == itemDisassembleHeaderId).ToListAsync();
		}

		public async Task<ResponseDto> SaveItemDisassemble(int storeId,int itemDisassembleHeaderId, List<ItemDisassembleDto> data)
		{
			var newItemList = new List<ItemDisassemble>();

			var nextItemDisassembleId = await GetNextId();

			foreach (var itemDisassemble in data)
			{
				var newItem = new ItemDisassemble()
				{
					ItemDisassembleId = nextItemDisassembleId,
					StoreId = storeId,
					ItemDisassembleHeaderId = itemDisassembleHeaderId,
					ItemId = itemDisassemble.ItemId,
					InQuantity = itemDisassemble.InQuantity,
					OutQuantity = itemDisassemble.OutQuantity,
					ItemPackageId = itemDisassemble.ItemPackageId,
					ExpireDate = itemDisassemble.ExpireDate,
					BatchNumber = itemDisassemble.BatchNumber,
					CreatedAt = DateHelper.GetDateTimeNow(),
					UserNameCreated = await _httpContextAccessor!.GetUserName(),
					IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
					Hide = false,
					EntryDate = DateHelper.GetDateTimeNow(),
				};

				var validator = await new ItemDisassembleValidator(_localizer).ValidateAsync(newItem);
				var validationResult = validator.IsValid;
				if (validationResult)
				{
					nextItemDisassembleId++;
					newItemList.Add(newItem);
				}
				else
				{
					return new ResponseDto() { Id = newItem.ItemDisassembleId, Success = false, Message = validator.ToString("~") };
				}
			}

			if (newItemList.Any())
			{
				await _repository.InsertRange(newItemList);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = 0, Success = true, Message = _localizer["NewItemDisassembleSuccessMessage"] };
			}
			else
			{
				return new ResponseDto() { Id = 0, Success = true, Message = _localizer["NoItemsFound"] };
			}
		}

		public async Task<ResponseDto> DeleteItemDisassemble(int itemDisassembleHeaderId)
		{
			var data = await _repository.GetAll().Where(x=>x.ItemDisassembleHeaderId == itemDisassembleHeaderId).ToListAsync();
			_repository.RemoveRange(data);
			await _repository.SaveChanges();
			return new ResponseDto() { Success = true, Message = "Delete Succeeded" };
		}

		public async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.ItemDisassembleId) + 1; } catch { id = 1; }
			return id;
		}
	}
}
