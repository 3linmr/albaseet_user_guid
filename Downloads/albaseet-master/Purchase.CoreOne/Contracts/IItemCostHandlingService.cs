using Shared.CoreOne.Models.Dtos.ViewModels.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Purchases.CoreOne.Contracts
{
	public interface IItemCostHandlingService
	{
		public Task<List<ItemCostDto>> CalculateItemCost(CalculateItemCost model);
		public Task<bool> UpdateItemCosts(CalculateItemCost model);
	}
}
