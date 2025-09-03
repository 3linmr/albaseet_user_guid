using Shared.CoreOne.Models.Domain.Taxes;
using Shared.Helper.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Service.Logic.Calculation
{
	public static class PaymentMethodCalculation
	{
		public static decimal GetCommissionValue(decimal commissionPercent, decimal vatPercent, decimal minCommissionValue, decimal maxCommissionValue, decimal entryValue, bool hasVat, bool vatInclusive, int rounding)
		{
			var commissionValue = (entryValue * (commissionPercent / 100));

			if (commissionValue <= minCommissionValue && minCommissionValue != 0)
			{
				if (hasVat)
				{
					if (vatInclusive)
					{
						return NumberHelper.RoundNumber((minCommissionValue / (100 + vatPercent)) * 100, rounding);
					}
					else
					{
						return NumberHelper.RoundNumber(minCommissionValue, rounding);
					}
				}
				else
				{
					return NumberHelper.RoundNumber(minCommissionValue, rounding);
				}

			}
			else if (commissionValue >= maxCommissionValue && maxCommissionValue != 0)
			{
				if (hasVat)
				{
					if (vatInclusive)
					{
						return NumberHelper.RoundNumber((maxCommissionValue / (100 + vatPercent)) * 100, rounding);
					}
					else
					{
						return NumberHelper.RoundNumber(maxCommissionValue, rounding);
					}
				}
				else
				{
					return NumberHelper.RoundNumber(maxCommissionValue, rounding);
				}
			}
			else
			{
				if (hasVat)
				{
					if (vatInclusive)
					{
						return ((entryValue * (commissionPercent / 100)) / (100 + vatPercent)) * 100;
					}
					else
					{
						return (entryValue * (commissionPercent / 100));
					}
				}
				else
				{
					return (entryValue * (commissionPercent / 100));
				}
			}
		}

		public static decimal GetCommissionTaxValue(decimal commissionPercent, decimal vatPercent, decimal minCommissionValue, decimal maxCommissionValue, decimal entryValue, bool hasVat, bool vatInclusive, int rounding)
		{
			if (hasVat)
			{
				var commissionValue = (entryValue * (commissionPercent / 100));

				if (commissionValue < minCommissionValue && minCommissionValue > 0)
				{
					if (vatInclusive)
					{
						return NumberHelper.RoundNumber((minCommissionValue / (100 + vatPercent)) * vatPercent, rounding);
					}
					else
					{
						return NumberHelper.RoundNumber((minCommissionValue) * (vatPercent / 100), rounding);
					}
				}
				else if (commissionValue > minCommissionValue && commissionValue > maxCommissionValue && maxCommissionValue > 0)
				{
					if (vatInclusive)
					{
						return NumberHelper.RoundNumber((maxCommissionValue / (100 + vatPercent)) * vatPercent, rounding);
					}
					else
					{
						return NumberHelper.RoundNumber((maxCommissionValue) * (vatPercent / 100), rounding);
					}
				}
				else
				{
					if (vatInclusive)
					{
						return NumberHelper.RoundNumber(((entryValue * (commissionPercent / 100)) / (100 + vatPercent)) * vatPercent, rounding);
					}
					else
					{
						return NumberHelper.RoundNumber(((entryValue * (commissionPercent / 100))) * (vatPercent / 100), rounding);
					}
				}
			}
			else
			{
				return 0;
			}
		}

		public static decimal GetFixedCommissionValue(decimal fixedCommissionValue, decimal vatPercent, bool hasVat, bool vatInclusive, int rounding)
		{
			if (hasVat)
			{
				if (vatInclusive)
				{
					return NumberHelper.RoundNumber((fixedCommissionValue / (100 + vatPercent)) * 100, rounding);
				}
				else
				{
					return NumberHelper.RoundNumber(fixedCommissionValue, rounding);
				}
			}
			else
			{
				return NumberHelper.RoundNumber(fixedCommissionValue, rounding);
			}
		}

		public static decimal GetFixedCommissionTaxValue(decimal fixedCommissionValue, decimal vatPercent, bool hasVat, bool vatInclusive, int rounding)
		{
			if (hasVat)
			{
				if (vatInclusive)
				{
					return NumberHelper.RoundNumber((fixedCommissionValue / (100 + vatPercent)) * vatPercent, rounding);
				}
				else
				{
					return NumberHelper.RoundNumber(fixedCommissionValue * (vatPercent / 100), rounding);
				}
			}
			else
			{
				return 0;
			}
		}

		public static decimal GetValueAfterCommission(decimal fixedCommissionValue, decimal commissionPercent, decimal vatPercent, decimal minCommissionValue, decimal maxCommissionValue, decimal entryValue, bool fixedHasVat, bool fixedVatInclusive, bool commissionHasVat, bool commissionVatInclusive, int rounding)
		{
			var commission = GetCommissionValue(commissionPercent, vatPercent, minCommissionValue, maxCommissionValue, entryValue, commissionHasVat, commissionVatInclusive, rounding);
			var commissionTax = GetCommissionTaxValue(commissionPercent, vatPercent, minCommissionValue, maxCommissionValue, entryValue, commissionHasVat, commissionVatInclusive, rounding);
			var fixedCommission = GetFixedCommissionValue(fixedCommissionValue, vatPercent, fixedHasVat, fixedVatInclusive, rounding);
			var fixedCommissionTax = GetFixedCommissionTaxValue(fixedCommissionValue, vatPercent, fixedHasVat, fixedVatInclusive, rounding);
			return NumberHelper.RoundNumber(entryValue - commission - commissionTax - fixedCommission - fixedCommissionTax, rounding);
		}
	}
}
