using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Dtos.ViewModels.Shared;

namespace Shared.Service.Logic.Calculation
{
	public static class ItemCostCalculation
	{
		public static decimal GetItemExpense(decimal itemTotalValue, decimal invoiceTotalValue, decimal invoiceExpense, decimal itemQuantity)
		{
			return (((itemTotalValue / invoiceTotalValue) * invoiceExpense)) / itemQuantity;
		}

		public static decimal GetAverageItemCost(List<QuantityPriceDto> model, decimal currentInvoicePrice)
		{
			return model.Any() ? model.Sum(x => x.Quantity * x.Price) / model.Sum(x => x.Quantity) : currentInvoicePrice;
		}

		public static decimal GetActualAverageCost(decimal stockQuantity, decimal invoiceQuantity,decimal stockPrice,decimal purchasePrice,decimal itemExpense)
		{
			return ((stockQuantity * stockPrice) + (invoiceQuantity * (purchasePrice + itemExpense)))  / (stockQuantity + invoiceQuantity);
		}
	}
}
