using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shared.Helper.Extensions
{
	public class DateTimeConverter : JsonConverter<DateTime>
	{
		public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			Debug.Assert(typeToConvert == typeof(DateTime));
			var date = DateTime.Parse(reader.GetString() ?? string.Empty);
			if (IsDatetimeIsDateOnly(date))
			{
				return date; //Date Only is matter
			}
			return date.ToUniversalTime(); // no timezone => utc
		}

		public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
		{
			writer.WriteStringValue(value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ssZ"));
		}

		public static bool IsDatetimeIsDateOnly(DateTime value)
		{
			return value.TimeOfDay == TimeSpan.Zero;
		}
	}
}