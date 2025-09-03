using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Helper.Extensions;
using System.Text.RegularExpressions;
using Shared.Helper.Logic;

namespace Shared.Service.Logic.Approval
{
	public static class CompareLogic
	{
		public static List<RequestChangesDto> GetDifferences<T>(T oldModel, T newModel)
		{
			var result = new List<RequestChangesDto>();
			var index = 1;
			var key = 0;
			foreach (var property in typeof(T).GetProperties())
			{
				if (property.GetValue(oldModel) != null && property.GetValue(newModel) != null)
				{
					if (index == 1)
					{
						key = Convert.ToInt32(property?.GetValue(newModel)?.ToString());
					}
					if (!property!.GetValue(oldModel)!.Equals(property.GetValue(newModel)))
					{
						if (!string.IsNullOrEmpty(property?.GetValue(oldModel)?.ToString()) || !string.IsNullOrEmpty(property?.GetValue(newModel)?.ToString()))
						{
							var tableName = typeof(T).Name;
							if (tableName.EndsWith("Dto"))
							{
								tableName = tableName.Remove(tableName.Length - 3);
							}
							var model = new RequestChangesDto()
							{
								TableName = tableName,
								ColumnName = property?.Name,
								Key = key,
								OldValue = GetValueString(property?.GetValue(oldModel)?.ToString()),
								NewValue = GetValueString(property?.GetValue(newModel)?.ToString())
							};
							result.Add(model);
						}
					}
					index++;
				}
			}

			return result;
		}

		public static string? GetValueString(string? value)
		{
			if (!string.IsNullOrEmpty(value) || !string.IsNullOrWhiteSpace(value))
			{
				if (value.IsDigitRegex())
				{
					var newValue =  NumberHelper.RoundNumber(Convert.ToDecimal(value), 3).ToString(CultureInfo.InvariantCulture);
					return newValue.RemoveTrailingZeros();
				}
				else if (value.HasNonNumericChar())
				{
					var newValue =  value.Replace("٫", ".");
					if (newValue.IsDigitRegex())
					{
						newValue = NumberHelper.RoundNumber(Convert.ToDecimal(value), 3).ToString(CultureInfo.InvariantCulture);
						return newValue.RemoveTrailingZeros();
					}
					else
					{
						return value;
					}
				}
				else
				{
					return value;
				}
			}
			return "";
		}
	}
}
