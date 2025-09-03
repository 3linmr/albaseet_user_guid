using Shared.CoreOne.Contracts.Inventory;
using Shared.CoreOne.Models.Domain.Inventory;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Items;
using Shared.CoreOne.Models.Dtos.ViewModels.Inventory;
using Shared.Helper.Identity;
using Shared.Helper.Models.Dtos;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Shared.Helper.Logic;

namespace Shared.Service.Services.Inventory
{
	public class ItemDisassembleSerialService :BaseService<ItemDisassembleSerial>,IItemDisassembleSerialService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IItemService _itemService;
		private readonly IItemPackageService _itemPackageService;

		public ItemDisassembleSerialService(IRepository<ItemDisassembleSerial> repository,IHttpContextAccessor httpContextAccessor,IItemService itemService,IItemPackageService itemPackageService) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
			_itemService = itemService;
			_itemPackageService = itemPackageService;
		}

		public IQueryable<ItemDisassembleSerialDto> GetItemDisassembleSerial()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var data =
				from itemDisassembleDetail in _repository.GetAll()
				from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == itemDisassembleDetail.ItemPackageId)
				from mainItemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == itemDisassembleDetail.MainItemPackageId).DefaultIfEmpty()
				from item in _itemService.GetAll().Where(x => x.ItemId == itemDisassembleDetail.ItemId)
				select new ItemDisassembleSerialDto
				{
					ItemName = language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn,
					ItemPackageId = itemDisassembleDetail.ItemPackageId,
					ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
					ItemId = itemDisassembleDetail.ItemId,
					MainItemPackageId = itemDisassembleDetail.MainItemPackageId,
					ItemDisassembleHeaderId = itemDisassembleDetail.ItemDisassembleHeaderId,
					ItemPackageBalanceAfter = itemDisassembleDetail.ItemPackageBalanceAfter,
					ItemPackageBalanceBefore = itemDisassembleDetail.ItemPackageBalanceBefore,
					ItemDisassembleSerialId = itemDisassembleDetail.ItemDisassembleSerialId,
					MainItemPackageName = itemDisassembleDetail.MainItemPackage != null ? language == LanguageCode.Arabic ? mainItemPackage.PackageNameAr : mainItemPackage.PackageNameEn : null
				};
			return data;
		}

		public async Task<List<ItemDisassembleSerialDto>> GetItemDisassembleSerial(int itemDisassembleHeaderId)
		{
			return await GetItemDisassembleSerial().Where(x=>x.ItemDisassembleHeaderId == itemDisassembleHeaderId).ToListAsync();
		}

		public async Task<ResponseDto> SaveItemDisassembleSerial(int itemDisassembleHeaderId, List<ItemDisassembleSerialDto> details)
		{
			var nextId = await GetNextId();
			var modelList = new List<ItemDisassembleSerial>();
			foreach (var item in details)
			{
				var model = new ItemDisassembleSerial
				{
					ItemPackageId = item.ItemPackageId,
					ItemId = item.ItemId,
					MainItemPackageId = item.MainItemPackageId,
					ItemDisassembleHeaderId = itemDisassembleHeaderId,
					ItemPackageBalanceAfter = item.ItemPackageBalanceAfter,
					ItemPackageBalanceBefore = item.ItemPackageBalanceBefore,
					ItemDisassembleSerialId = nextId,
					CreatedAt = DateHelper.GetDateTimeNow(),
					UserNameCreated = await _httpContextAccessor!.GetUserName(),
					IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
					Hide = false
				};
				modelList.Add(model);
				nextId++;
			}

			if (modelList.Any())
			{
				await DeleteItemDisassembleSerial(itemDisassembleHeaderId);
				await _repository.InsertRange(modelList);
				await _repository.SaveChanges();
				return new ResponseDto { Success = true, Message = "Success" };
			}
			return new ResponseDto { Success = false, Message = "Save Error" };
		}

		public async Task<ResponseDto> DeleteItemDisassembleSerial(int itemDisassembleHeaderId)
		{
			var modelDb = await _repository.GetAll().Where(x => x.ItemDisassembleHeaderId == itemDisassembleHeaderId).ToListAsync();
			_repository.RemoveRange(modelDb);
			await _repository.SaveChanges();
			return new ResponseDto { Success = true, Message = "Success" };
		}

		public async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.ItemDisassembleSerialId) + 1; } catch { id = 1; }
			return id;
		}
	}
}
