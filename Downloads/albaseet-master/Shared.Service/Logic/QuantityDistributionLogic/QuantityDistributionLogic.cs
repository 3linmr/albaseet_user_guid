using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Purchases.Service.Services
{
	public class QuantityDistributionLogic
	{
		public static void DistributeQuantitiesOnDetails<DetailType, KeyType>(List<DetailType> details, Func<DetailType, KeyType> keySelector, Func<DetailType,decimal> availableQuantitySelector, Func<DetailType, decimal> availableBonusQuantitySelector, Func<DetailType, decimal> quantitySelector, Func<DetailType, decimal> bonusQuantitySelector, Action<DetailType,decimal> quantityAssigner, Action<DetailType,decimal> bonusQuantityAssigner) where KeyType : struct where DetailType : class
		{
			var groups = details.GroupBy(keySelector).Select(x => x.ToList());

			foreach (var group in groups)
			{
				var totalAvailableQuantity = availableQuantitySelector(group.First());
				var totalAvailableBonusQuantity = availableBonusQuantitySelector(group.First());

				decimal remainingQuantity = totalAvailableQuantity;
				decimal remainingBonusQuantity = totalAvailableBonusQuantity;

				foreach(var detail in group)
				{
					var quantity = Math.Min(quantitySelector(detail), remainingQuantity);
					var bonusQuantity = Math.Min(bonusQuantitySelector(detail), remainingBonusQuantity);

					quantityAssigner(detail, quantity);
					bonusQuantityAssigner(detail, bonusQuantity);

					remainingQuantity -= quantity;
					remainingBonusQuantity -= bonusQuantity;
				}
			}
		}
	}
}
