using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Schema;

namespace Shared.Helper.Extensions
{
	public static class ObjectExtensions
	{
		public static bool CanBeConverted<T>(this object value) where T : class
		{
			var jsonData = JsonConvert.SerializeObject(value);
			var schema = NJsonSchema.JsonSchema.FromType<T>();
			return !(schema.Validate(jsonData).Any());
		}

		public static T? ConvertToType<T>(this object value) where T : class
		{
			var jsonData = JsonConvert.SerializeObject(value);
			return JsonConvert.DeserializeObject<T>(jsonData);
		}
	}
}
