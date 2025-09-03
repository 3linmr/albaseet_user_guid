using Shared.CoreOne.Contracts.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Items;
using Shared.CoreOne.Models.Domain.Inventory;
using Shared.CoreOne.Models.Dtos.ViewModels.Inventory;
using Shared.Helper.Models.Dtos;
using Shared.CoreOne.Models.Domain.Modules;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
using Shared.Helper.Identity;
using Shared.Helper.Logic;

namespace Shared.Service.Services.Inventory
{
	public class ItemDisassembleDetailService :BaseService<ItemDisassembleDetail>, IItemDisassembleDetailService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IItemPackageService _itemPackageService;
		private readonly IItemService _itemService;

		public ItemDisassembleDetailService(IRepository<ItemDisassembleDetail> repository,IHttpContextAccessor httpContextAccessor,IItemPackageService itemPackageService,IItemService itemService) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
			_itemPackageService = itemPackageService;
			_itemService = itemService;
		}

		public IQueryable<ItemDisassembleDetailDto> GetItemDisassembleDetails()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var data =
				from itemDisassembleDetail in _repository.GetAll()
				from formPackage in _itemPackageService.GetAll().Where(x=>x.ItemPackageId == itemDisassembleDetail.FromPackageId)
				from toPackage in _itemPackageService.GetAll().Where(x=>x.ItemPackageId == itemDisassembleDetail.ToPackageId)
				from item in _itemService.GetAll().Where(x=>x.ItemId == itemDisassembleDetail.ItemId)
				select new ItemDisassembleDetailDto
				{
					FromPackageId = itemDisassembleDetail.FromPackageId,
					ItemId = itemDisassembleDetail.ItemId,
					ToPackageId = itemDisassembleDetail.ToPackageId,
					Packing = itemDisassembleDetail.Packing,
					Quantity = itemDisassembleDetail.Quantity,
					BatchNumber = itemDisassembleDetail.BatchNumber,
					ExpireDate = itemDisassembleDetail.ExpireDate,
					FromPackageName = language == LanguageCode.Arabic ? formPackage.PackageNameAr : formPackage.PackageNameEn,
					ToPackageName = language == LanguageCode.Arabic ? toPackage.PackageNameAr : toPackage.PackageNameEn,
					ItemName = language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn,
					ItemDisassembleHeaderId = itemDisassembleDetail.ItemDisassembleHeaderId,
					FromPackageQuantityBefore = itemDisassembleDetail.FromPackageQuantityBefore,
					FromPackageQuantityAfter = itemDisassembleDetail.FromPackageQuantityAfter,
					ToPackageQuantityBefore = itemDisassembleDetail.ToPackageQuantityBefore,
					ToPackageQuantityAfter = itemDisassembleDetail.ToPackageQuantityAfter,
					IsSerialConversion = itemDisassembleDetail.IsSerialConversion,
					ItemDisassembleDetailId = itemDisassembleDetail.ItemDisassembleDetailId
				};
			return data;
		}

		public async Task<List<ItemDisassembleDetailDto>> GetItemDisassembleDetails(int itemDisassembleHeaderId)
		{
			return await GetItemDisassembleDetails().Where(x => x.ItemDisassembleHeaderId == itemDisassembleHeaderId).ToListAsync() ?? [];
		}

		public  async Task<ResponseDto> SaveItemDisassembleDetails(int itemDisassembleHeaderId, List<ItemDisassembleDetailDto> details)
		{
			var nextId = await GetNextId();
			var modelList = new List<ItemDisassembleDetail>();
			foreach (var item in details)
			{
				var model = new ItemDisassembleDetail
				{
					FromPackageId = item.FromPackageId,
					ItemId = item.ItemId,
					Packing = item.Packing,
					ToPackageId = item.ToPackageId,
					Quantity = item.Quantity,
					BatchNumber = item.BatchNumber,
					ExpireDate = item.ExpireDate,
					ItemDisassembleHeaderId = itemDisassembleHeaderId,
					FromPackageQuantityBefore = item.FromPackageQuantityBefore,
					FromPackageQuantityAfter = item.FromPackageQuantityAfter,
					ItemDisassembleDetailId = nextId,
					IsSerialConversion = item.IsSerialConversion,
					ToPackageQuantityAfter = item.ToPackageQuantityAfter,
					ToPackageQuantityBefore = item.ToPackageQuantityBefore,
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
				await DeleteItemDisassembleDetails(itemDisassembleHeaderId);
				await _repository.InsertRange(modelList);
				await _repository.SaveChanges();
				return new ResponseDto { Success = true, Message = "Success" };
			}
			return new ResponseDto { Success = false, Message = "Save Error" };
		}

		public async Task<ResponseDto> DeleteItemDisassembleDetails(int itemDisassembleHeaderId)
		{
			var modelDb = await _repository.GetAll().Where(x=>x.ItemDisassembleHeaderId == itemDisassembleHeaderId).ToListAsync();
			_repository.RemoveRange(modelDb);
			await _repository.SaveChanges();
			return new ResponseDto { Success = true, Message = "Success" };
		}

		public async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.ItemDisassembleDetailId) + 1; } catch { id = 1; }
			return id;
		}
	}
}
