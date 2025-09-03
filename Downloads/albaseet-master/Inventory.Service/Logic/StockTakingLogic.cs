using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Service.Logic
{
	public static class StockTakingLogic
	{
		public static decimal CalculateBalanceQuantity(bool isOpenBalance, decimal openQuantity, decimal inQuantity, decimal outQuantity, decimal pendingOut)
		{
			if (isOpenBalance)
			{
				return openQuantity;
			}
			else
			{
				return openQuantity + inQuantity - outQuantity;
				//return openQuantity + inQuantity - outQuantity - pendingOut;
			}
		}
		public static decimal CalculateCurrentConsumerValue(bool isOpenBalance, decimal consumerPrice, decimal openQuantity, decimal inQuantity, decimal outQuantity, decimal pendingOut)
		{
			var quantity = CalculateBalanceQuantity(isOpenBalance, openQuantity, inQuantity, outQuantity, pendingOut);
			return quantity * consumerPrice;
		}

		public static decimal CalculateDifferenceBetweenStockTakingConsumerAndCurrent(bool isOpenBalance, decimal consumerPrice, decimal openQuantity, decimal inQuantity, decimal outQuantity, decimal pendingOut,decimal stockTakingConsumerValue)
		{
			var currentConsumerValue = CalculateCurrentConsumerValue(isOpenBalance,consumerPrice,openQuantity,inQuantity,outQuantity,pendingOut);
			return stockTakingConsumerValue - currentConsumerValue;
		}


		public static decimal CalculateBalanceCostValue(bool isOpenBalance, decimal costPackage, decimal openQuantity, decimal inQuantity, decimal outQuantity, decimal pendingOut)
		{
			var quantity = CalculateBalanceQuantity(isOpenBalance, openQuantity, inQuantity, outQuantity, pendingOut);
			return quantity * costPackage;
		}

		public static decimal CalculateDifferenceBetweenStockTakingCostAndCurrent(bool isOpenBalance, decimal consumerPrice, decimal openQuantity, decimal inQuantity, decimal outQuantity, decimal pendingOut, decimal stockTakingCostValue)
		{
			var currentCostValue = CalculateBalanceCostValue(isOpenBalance, consumerPrice, openQuantity, inQuantity, outQuantity, pendingOut);
			return stockTakingCostValue - currentCostValue;
		}

		public static decimal CalculateOpenQuantity(bool isOpenBalance, decimal stockTakingQuantity, decimal openQuantity)
		{
			if (isOpenBalance)
			{
				return stockTakingQuantity;
			}
			else
			{
				return openQuantity;
			}
		}

		public static decimal CalculateDifferenceBetweenStockTakingAndCurrent(bool isOpenBalance, decimal oldOpenQuantity, decimal stockTakingQuantity, decimal openQuantity, decimal inQuantity, decimal outQuantity, decimal pendingOut)
		{
			if (isOpenBalance)
			{
				return CalculateOpenQuantity(isOpenBalance, stockTakingQuantity, openQuantity) - oldOpenQuantity;

			}
			else
			{
				var quantity = CalculateBalanceQuantity(isOpenBalance, openQuantity, inQuantity, outQuantity, pendingOut);
				return stockTakingQuantity - quantity;
			}
		}
		public static decimal CalculateInQuantity(bool isOpenBalance, decimal stockTakingQuantity, decimal openQuantity, decimal inQuantity, decimal outQuantity, decimal pendingOut)
		{
			if (isOpenBalance)
			{
				return 0;
			}
			else
			{
				var balanceQuantity = CalculateBalanceQuantity(isOpenBalance, openQuantity, inQuantity, outQuantity, pendingOut);
				if (stockTakingQuantity >= balanceQuantity)
				{
					return stockTakingQuantity - balanceQuantity;
				}
				else
				{
					return 0;
				}
			}
		}
		public static decimal CalculateOutQuantity(bool isOpenBalance, decimal stockTakingQuantity, decimal openQuantity, decimal inQuantity, decimal outQuantity, decimal pendingOut)
		{
			if (isOpenBalance)
			{
				return 0;
			}
			else
			{
				var balanceQuantity = CalculateBalanceQuantity(isOpenBalance, openQuantity, inQuantity, outQuantity, pendingOut);
				if (stockTakingQuantity <= balanceQuantity)
				{
					return balanceQuantity - stockTakingQuantity;
				}
				else
				{
					return 0;
				}
			}
		}
	}
}
