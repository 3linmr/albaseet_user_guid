using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shared.Helper.Extensions
{
	public static class JsonExtensions
	{
		public static string ToJson<T>(this T obj)
		{
			var settings = new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
				Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
			};
			return JsonSerializer.Serialize(obj, settings);
		}
	}
}
