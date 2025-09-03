using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventory.CoreOne.Contracts;
using Inventory.CoreOne.Models.Domain;
using Inventory.CoreOne.Models.Dtos.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Inventory;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Helper.Models.Dtos;
using Shared.Service;

namespace Inventory.Service.Services
{
	public class StockTakingCarryOverEffectDetailService : BaseService<StockTakingCarryOverEffectDetail>, IStockTakingCarryOverEffectDetailService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IItemCurrentBalanceService _itemCurrentBalanceService;

		public StockTakingCarryOverEffectDetailService(IRepository<StockTakingCarryOverEffectDetail> repository,IHttpContextAccessor httpContextAccessor,IItemCurrentBalanceService itemCurrentBalanceService) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
			_itemCurrentBalanceService = itemCurrentBalanceService;
		}

		public async Task<List<StockTakingCarryOverEffectDetail>> InsertCarryOverEffect(int stockTakingCarryOverHeaderId, int storeId,bool isOpenBalance,bool isAllItemsAffected, List<StockTakingCarryOverDetailDto> carryOverDetails)
		{
			var modelList = new List<StockTakingCarryOverEffectDetail>();
			var nextId = await GetNextId();
			var userName = await _httpContextAccessor.GetUserName();
			if (carryOverDetails.Any())
			{
				foreach (var detail in carryOverDetails)
				{
					var model = new StockTakingCarryOverEffectDetail()
					{
						StockTakingCarryOverHeaderId = stockTakingCarryOverHeaderId,
						StockTakingCarryOverEffectDetailId = nextId,
						ItemId = detail.ItemId,
						BatchNumber = detail.BatchNumber,
						ExpireDate = detail.ExpireDate,
						OpenQuantity = isOpenBalance ? detail.OpenQuantity - detail.OldOpenQuantity : 0,
						InQuantity = detail.InQuantity,
						OutQuantity = detail.OutQuantity,
						ItemPackageId = detail.ItemPackageId,
						OldOpenQuantity = detail.OldOpenQuantity,
						CreatedAt = DateHelper.GetDateTimeNow(),
						UserNameCreated = userName,
						IpAddressCreated = _httpContextAccessor.GetIpAddress(),
						Hide = false
					};
					modelList.Add(model);
					nextId++;
				}

				if (isAllItemsAffected)
				{
					var itemCurrentBalances = await _itemCurrentBalanceService.GetAll().Where(x => x.InQuantity + x.OutQuantity + x.OpenQuantity > 0 && x.StoreId == storeId).AsNoTracking().ToListAsync();

					var modelToBeEmpty =
						(from itemCurrentBalance in itemCurrentBalances
						from carryOver in modelList.Where(x => x.ItemId == itemCurrentBalance.ItemId && x.ItemPackageId == itemCurrentBalance.ItemPackageId && x.ExpireDate == itemCurrentBalance.ExpireDate && x.BatchNumber == itemCurrentBalance.BatchNumber).DefaultIfEmpty()
						where carryOver == null
						select new StockTakingCarryOverEffectDetail()
						{
							StockTakingCarryOverHeaderId = stockTakingCarryOverHeaderId,
							StockTakingCarryOverEffectDetailId = nextId,
							ItemId = itemCurrentBalance.ItemId,
							BatchNumber = itemCurrentBalance.BatchNumber,
							ExpireDate = itemCurrentBalance.ExpireDate,
							OpenQuantity = isOpenBalance ? itemCurrentBalance.OpenQuantity * -1 : 0,
							InQuantity = itemCurrentBalance.InQuantity * -1,
							OutQuantity = itemCurrentBalance.OutQuantity * -1,
							ItemPackageId = itemCurrentBalance.ItemPackageId,
							OldOpenQuantity = itemCurrentBalance.OpenQuantity,
							CreatedAt = DateHelper.GetDateTimeNow(),
							UserNameCreated = userName,
							IpAddressCreated = _httpContextAccessor.GetIpAddress(),
							Hide = false
						}).ToList();

					modelToBeEmpty.ForEach(x=>x.StockTakingCarryOverEffectDetailId = nextId++);
					modelList.AddRange(modelToBeEmpty);
				}

				await _repository.InsertRange(modelList);
				await _repository.SaveChanges();
				return modelList;
			}
			return new List<StockTakingCarryOverEffectDetail>();
		}

		public async Task<ResponseDto> DeleteCarryOverEffect(int headerId)
		{
			var data = await _repository.GetAll().Where(x => x.StockTakingCarryOverHeaderId == headerId).AsNoTracking().ToListAsync();
			if (data.Any())
			{
				_repository.RemoveRange(data);
				await _repository.SaveChanges();
				return new ResponseDto() { Success = true, Id = headerId };
			}
			return new ResponseDto();
		}

		public async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.StockTakingCarryOverEffectDetailId) + 1; } catch { id = 1; }
			return id;
		}
	}
}
