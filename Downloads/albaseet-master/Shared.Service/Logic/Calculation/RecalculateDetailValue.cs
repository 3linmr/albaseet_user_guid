using Sales.CoreOne.Models.Dtos.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Service.Logic.Calculation
{
	public class RecalculateDetailValue
	{
		//this function is used to recalculate all values affected by quantity
		public static void RecalculateDetailValues<DetailType>(List<DetailType> details, Func<DetailType, decimal> quantitySelector, Func<DetailType, decimal> bonusQuantitySelector, Func<DetailType, decimal> priceSelector, Func<DetailType, bool> isItemVatInclusiveSelector, Func<DetailType, decimal> vatPercentSelector, Func<DetailType, decimal> itemDiscountPercentSelector, Func<DetailType, decimal> costPackageSelector, Action<DetailType, decimal> totalValueAssigner, Action<DetailType, decimal> itemDiscountValueAssigner, Action<DetailType, decimal> totalValueAfterDiscountAssigner, Action<DetailType, decimal> headerDiscountValueAssigner, Action<DetailType, decimal> grossValueAssigner, Action<DetailType, decimal> vatValueAssigner, Action<DetailType, decimal> subNetValueAssigner, Action<DetailType, decimal> costValueAssigner, decimal headerDiscountPercent)
		{
			foreach (var detail in details)
			{
				totalValueAssigner(detail, CalculateDetailValue.TotalValue(quantitySelector(detail), priceSelector(detail), isItemVatInclusiveSelector(detail), vatPercentSelector(detail)));
				itemDiscountValueAssigner(detail, CalculateDetailValue.DiscountValue(quantitySelector(detail), priceSelector(detail), itemDiscountPercentSelector(detail), isItemVatInclusiveSelector(detail), vatPercentSelector(detail)));
				totalValueAfterDiscountAssigner(detail, CalculateDetailValue.TotalValueAfterDiscount(quantitySelector(detail), priceSelector(detail), itemDiscountPercentSelector(detail), isItemVatInclusiveSelector(detail), vatPercentSelector(detail)));
				headerDiscountValueAssigner(detail, CalculateDetailValue.HeaderDiscountValue(quantitySelector(detail), priceSelector(detail), itemDiscountPercentSelector(detail), headerDiscountPercent, isItemVatInclusiveSelector(detail), vatPercentSelector(detail)));
				grossValueAssigner(detail, CalculateDetailValue.GrossValue(quantitySelector(detail), priceSelector(detail), itemDiscountPercentSelector(detail), headerDiscountPercent, isItemVatInclusiveSelector(detail), vatPercentSelector(detail)));
				vatValueAssigner(detail, CalculateDetailValue.VatValue(quantitySelector(detail), priceSelector(detail), itemDiscountPercentSelector(detail), vatPercentSelector(detail), headerDiscountPercent, isItemVatInclusiveSelector(detail)));
				subNetValueAssigner(detail, CalculateDetailValue.SubNetValue(quantitySelector(detail), priceSelector(detail), itemDiscountPercentSelector(detail), vatPercentSelector(detail), headerDiscountPercent, isItemVatInclusiveSelector(detail)));

				costValueAssigner(detail, costPackageSelector(detail) * (quantitySelector(detail) + bonusQuantitySelector(detail)));
			}
		}
	}
}
