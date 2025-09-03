namespace Shared.Service.Logic.Calculation
{
	public static class CalculateDetailValue
	{
		public static decimal PriceBeforeVat(decimal price, bool isVatInclusive, decimal vatPercent)
		{
			return isVatInclusive ? (vatPercent > 0 ? (price * 100 / (100 + vatPercent)) : price) : price;
		}

		public static decimal TotalValue(decimal quantity, decimal price, bool isVatInclusive, decimal vatPercent)
		{
			var currentPrice = PriceBeforeVat(price, isVatInclusive, vatPercent);
			return quantity * currentPrice;
		}

		public static decimal DiscountPercent(decimal quantity, decimal price, decimal discountValue, bool isVatInclusive, decimal vatPercent)
		{
			var totalValue = TotalValue(quantity, price, isVatInclusive, vatPercent);
			return (discountValue / totalValue) * 100;
		}

		public static decimal DiscountValue(decimal quantity, decimal price, decimal discountPercent, bool isVatInclusive, decimal vatPercent)
		{
			var totalValue = TotalValue(quantity, price, isVatInclusive, vatPercent);
			return totalValue * (discountPercent / 100);
		}

		public static decimal TotalValueAfterDiscount(decimal quantity, decimal price, decimal itemDiscountPercent, bool isVatInclusive, decimal vatPercent)
		{
			var totalValue = TotalValue(quantity, price, isVatInclusive, vatPercent);
			var discountValue = DiscountValue(quantity, price, itemDiscountPercent, isVatInclusive, vatPercent);
			return totalValue - discountValue;
		}

		public static decimal HeaderDiscountValue(decimal quantity, decimal price, decimal itemDiscountPercent, decimal headerDiscountPercent, bool isVatInclusive, decimal vatPercent)
		{
			var totalValueAfterDiscount = TotalValueAfterDiscount(quantity, price, itemDiscountPercent, isVatInclusive, vatPercent);
			return totalValueAfterDiscount * (headerDiscountPercent/100);
		}

		public static decimal GrossValue(decimal quantity, decimal price, decimal itemDiscountPercent, decimal headerDiscountPercent, bool isVatInclusive, decimal vatPercent)
		{
			var totalValueAfterDiscount = TotalValueAfterDiscount(quantity, price, itemDiscountPercent, isVatInclusive, vatPercent);
			var headerDiscountValue = HeaderDiscountValue(quantity, price, itemDiscountPercent, headerDiscountPercent, isVatInclusive, vatPercent);
			return totalValueAfterDiscount - headerDiscountValue;
		}

		public static decimal VatValue(decimal quantity, decimal price, decimal itemDiscountPercent, decimal vatPercent, decimal headerDiscountPercent, bool isVatInclusive)
		{
			var grossValue = GrossValue(quantity, price, itemDiscountPercent, headerDiscountPercent, isVatInclusive, vatPercent);
			return grossValue * (vatPercent / 100);
		}

		public static decimal SubNetValue(decimal quantity, decimal price, decimal itemDiscountPercent, decimal vatPercent, decimal headerDiscountPercent, bool isVatInclusive)
		{
			var grossValue = GrossValue(quantity, price, itemDiscountPercent, headerDiscountPercent, isVatInclusive, vatPercent);
			var vatValue = VatValue(quantity, price, itemDiscountPercent, vatPercent, headerDiscountPercent, isVatInclusive);
			return grossValue + vatValue;
		}

		public static decimal TaxValue(decimal quantity, decimal price, decimal itemDiscountPercent, decimal vatPercent, decimal taxPercent, bool afterVat, decimal headerDiscountPercent, bool isVatInclusive)
		{
			if (afterVat)
			{
				var subNetValue = SubNetValue(quantity, price, itemDiscountPercent, vatPercent, headerDiscountPercent, isVatInclusive);
				return subNetValue * (taxPercent / 100);
			}
			else
			{
				var grossValue = GrossValue(quantity, price, itemDiscountPercent, headerDiscountPercent, isVatInclusive, vatPercent);
				return grossValue * (taxPercent / 100);
			}
		}

		public static decimal NetValue(decimal quantity, decimal price, decimal itemDiscountPercent, decimal vatPercent, decimal otherTaxValue, decimal headerDiscountPercent, bool isVatInclusive)
		{
			var subNetValue = SubNetValue(quantity, price, itemDiscountPercent, vatPercent, headerDiscountPercent, isVatInclusive);
			return subNetValue + otherTaxValue;
		}
	}

	public static class CalculateHeaderValue
	{
		public static decimal DiscountPercent(decimal totalValueAfterDiscountFromDetail, decimal discountValue)
		{
			return (discountValue / totalValueAfterDiscountFromDetail) * 100;
		}

		public static decimal DiscountValue(decimal totalValueAfterDiscountFromDetail, decimal discountPercent)
		{
			return totalValueAfterDiscountFromDetail * (discountPercent / 100);
		}

		public static decimal NetValue(decimal netValueFromDetail, decimal additionalDiscountValue)
		{
			return netValueFromDetail - additionalDiscountValue;
		}
	}
}
