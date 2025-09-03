using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Helper.Logic
{
	public static class NumberHelper
	{
		/// <summary>
		/// Return if a number exist in range of two other numbers
		/// </summary>
		/// <param name="currentValue"></param>
		/// <param name="fromValue"></param>
		/// <param name="toValue"></param>
		/// <returns></returns>
		public static bool InRange(this int currentValue, int fromValue, int toValue)
		{
			return currentValue >= fromValue && currentValue <= toValue;
		}
		public static int GetTrailingZerosFromInteger(int no)
		{
			if (no == 0)
				return 1;

			int count = 0;
			while (no % 10 == 0)
			{
				no /= 10;
				count++;
			}
			return count;
		}

		public static decimal RoundNumber(decimal number,int rounding)
		{
			return Math.Round(number, rounding, MidpointRounding.AwayFromZero);
		}
	}
}
