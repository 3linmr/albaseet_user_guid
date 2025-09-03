using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Diagnostics;

namespace Shared.Helper.Extensions
{
	public class TrimConverter: JsonConverter<string>
	{
		public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			Debug.Assert(typeToConvert == typeof(string));
			var value = reader.GetString();
			return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
		}

		public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
		{
			writer.WriteStringValue(value);
		}
	}

	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class TrimAttribute: JsonConverterAttribute
	{
		public override JsonConverter? CreateConverter(Type typeToConvert)
		{
			return new TrimConverter();
		}
	}
}
