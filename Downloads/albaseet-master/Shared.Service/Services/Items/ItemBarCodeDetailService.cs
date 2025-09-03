using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Items;
using Shared.CoreOne.Models.Domain.Inventory;
using Shared.CoreOne.Models.Domain.Items;
using Shared.CoreOne.Models.Dtos.ViewModels.Items;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Helper.Models.Dtos;

namespace Shared.Service.Services.Items
{
	public class ItemBarCodeDetailService : BaseService<ItemBarCodeDetail>, IItemBarCodeDetailService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		public ItemBarCodeDetailService(IRepository<ItemBarCodeDetail> repository,IHttpContextAccessor httpContextAccessor) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		public async Task<List<ItemBarCodeDetailDto>> GetItemBarCodeDetails(int itemBarCodeId)
		{
			return await _repository.GetAll().Where(x => x.ItemBarCodeId == itemBarCodeId).Select(x=> new ItemBarCodeDetailDto
			{
				ItemBarCodeId = x.ItemBarCodeId,
				ItemBarCodeDetailId = x.ItemBarCodeDetailId,
				ConsumerPrice = x.ConsumerPrice,
				BarCode = x.BarCode
			}).ToListAsync();
		}

		public async Task<bool> SaveItemBarCodeDetails(List<ItemBarCodeDetailDto> barCodes,string? itemCode, int itemCompanyId, Func<List<ItemBarCodeDetail>, int, Task<bool>> isBarCodeExistFunc)
		{
			var isDuplicated = CheckIfBarCodeIsDuplicated(barCodes);
            if (isDuplicated)
            {
                return false;
            }
			await DeleteItemBarCodes(barCodes);
			var newBarCodes = barCodes.Where(x => x.ItemBarCodeDetailId <= 0).ToList();
			var oldBarCodes = barCodes.Where(x => x.ItemBarCodeDetailId > 0).ToList();
			var result = await CreateItemBarCodes(newBarCodes, itemCode, itemCompanyId, isBarCodeExistFunc);
			if (result)
			{
				result = await UpdateItemBarCodes(oldBarCodes, itemCode, itemCompanyId, isBarCodeExistFunc);
				return result;
			}
			return result;
		}

		public async Task<bool> CreateItemBarCodes(List<ItemBarCodeDetailDto> barCodes, string? itemCode, int itemCompanyId, Func<List<ItemBarCodeDetail>,int,Task<bool>> isBarCodeExistFunc)
		{
			if (barCodes.Any())
			{
				var nextId = await GetNextId();
				var modelList = new List<ItemBarCodeDetail>();

				foreach (var barCode in barCodes)
				{
					var newBarCode = new ItemBarCodeDetail
					{
						ItemBarCodeDetailId = nextId,
						ItemBarCodeId = barCode.ItemBarCodeId,
						BarCode = (string.IsNullOrEmpty(barCode.BarCode) && barCode.IsSingularPackage) ? itemCode : (string.IsNullOrWhiteSpace(barCode.BarCode) ? null : barCode.BarCode),
						ConsumerPrice = barCode.ConsumerPrice,
						Hide = false,
						CreatedAt = DateHelper.GetDateTimeNow(),
						UserNameCreated = await _httpContextAccessor!.GetUserName(),
						IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
					};
					modelList.Add(newBarCode);
					nextId++;
				}
				var exist = await isBarCodeExistFunc(modelList.ToList(), itemCompanyId);
				if (!exist)
				{
					await _repository.InsertRange(modelList);
					await _repository.SaveChanges();
					return true;
				}
				else
				{
					return false;
				}
			}
			return true;
		}

		public async Task<bool> UpdateItemBarCodes(List<ItemBarCodeDetailDto> barCodes, string? itemCode, int itemCompanyId, Func<List<ItemBarCodeDetail>, int, Task<bool>> isBarCodeExistFunc)
		{
			if (barCodes.Any())
			{
				var modelList = new List<ItemBarCodeDetail>();
				var currentIds = barCodes.Select(x => x.ItemBarCodeDetailId).ToList();
				var currentBarCodes = await _repository.GetAll().AsNoTracking().Where(x => currentIds.Contains(x.ItemBarCodeDetailId)).ToListAsync();

				foreach (var barCode in barCodes)
				{
					var newBarCode = new ItemBarCodeDetail
					{
						ItemBarCodeDetailId = barCode.ItemBarCodeDetailId,
						ItemBarCodeId = barCode.ItemBarCodeId,
						BarCode = (string.IsNullOrEmpty(barCode.BarCode) && barCode.IsSingularPackage) ? itemCode : (string.IsNullOrWhiteSpace(barCode.BarCode) ? null : barCode.BarCode),
						ConsumerPrice = barCode.ConsumerPrice,
						Hide = false,
						ModifiedAt = DateHelper.GetDateTimeNow(),
						UserNameModified = await _httpContextAccessor!.GetUserName(),
						IpAddressModified = _httpContextAccessor?.GetIpAddress(),
						CreatedAt = currentBarCodes.Where(x => x.ItemBarCodeDetailId == barCode.ItemBarCodeDetailId).Select(x => x.CreatedAt).FirstOrDefault(),
						UserNameCreated = currentBarCodes.Where(x => x.ItemBarCodeDetailId == barCode.ItemBarCodeDetailId).Select(x => x.UserNameCreated).FirstOrDefault(),
						IpAddressCreated = currentBarCodes.Where(x => x.ItemBarCodeDetailId == barCode.ItemBarCodeDetailId).Select(x => x.IpAddressCreated).FirstOrDefault(),
					};
					modelList.Add(newBarCode);
				}
				var exist = await isBarCodeExistFunc(modelList.ToList(), itemCompanyId);
				if (!exist)
				{
					_repository.UpdateRange(modelList);
					await _repository.SaveChanges();
					return true;
				}
				else
				{
					return false;
				}
			}

			return true;
		}

		public async Task<bool> DeleteItemBarCodes(List<ItemBarCodeDetailDto> barCodes)
		{
			if (barCodes.Any())
			{
				var itemBarCodeId = barCodes.Select(x=>x.ItemBarCodeId).ToList();
				var currentBarCodes = await _repository.GetAll().Where(x => itemBarCodeId.Contains(x.ItemBarCodeId)).AsNoTracking().ToListAsync();
				var toBeDeleted = currentBarCodes.Where(p => barCodes.All(p2 => p2.ItemBarCodeDetailId != p.ItemBarCodeDetailId)).ToList();
				if (toBeDeleted.Any())
				{
					_repository.RemoveRange(toBeDeleted);
					await _repository.SaveChanges();
					return true;
				}
			}
			return true;
		}

		public async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.ItemBarCodeDetailId) + 1; } catch { id = 1; }
			return id;
		}

        private static bool CheckIfBarCodeIsDuplicated(List<ItemBarCodeDetailDto> barCodes)
        {
            return barCodes.Where(x=>x.BarCode?.Trim()?.Length > 0).GroupBy(x => x.BarCode).Any(g => g.Count() > 1);
        }

    }
}
