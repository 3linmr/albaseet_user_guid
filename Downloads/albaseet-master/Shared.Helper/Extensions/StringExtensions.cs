using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Shared.Helper.Extensions
{
	public static class StringExtensions
	{
		public static bool IsDigit(this string value)
		{
			return value.All(char.IsDigit);
		}
		public static bool IsDigitRegex(this string value)
		{
			return Regex.IsMatch(value, @"^\d+(\.\d+)?$");
		}

		public static bool HasNonNumericChar(this string value)
		{
			return Regex.IsMatch(value, @"[^\d\-+\.,eE]\d+");
		}
		public static string RemoveTrailingZeros(this string value)
		{
			return Regex.Replace(value, "\\.0+$", "");
		}
		public static string? ToNullIfEmptyOrWhiteSpaces(this string value)
		{
			return string.IsNullOrWhiteSpace(value) ? null : value;
		}
	}
}
