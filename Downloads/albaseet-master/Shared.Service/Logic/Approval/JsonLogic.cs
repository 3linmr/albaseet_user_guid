using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonDiffPatchDotNet;
using Newtonsoft.Json.Linq;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;

namespace Shared.Service.Logic.Approval
{
	public static class JsonLogic
	{
		public static string CompareJson(string oldValue, string newValue)
		{
			var jdp = new JsonDiffPatch();
			var left = JToken.Parse(oldValue);
			var right = JToken.Parse(newValue);
			JToken patch = jdp.Diff(left, right);
			return patch.ToString();
		}

		/// <summary>
		/// Deep compare two NewtonSoft JObjects. If they don't match, returns text diffs
		/// </summary>
		/// <param name="source">The expected results</param>
		/// <param name="target">The actual results</param>
		/// <returns>Text string</returns>
		public static StringBuilder CompareObjects(JObject source, JObject target)
		{
			StringBuilder returnString = new StringBuilder();
			foreach (KeyValuePair<string, JToken> sourcePair in source)
			{
				if (sourcePair.Value.Type == JTokenType.Object)
				{
					if (target.GetValue(sourcePair.Key) == null)
					{
						returnString.Append("Key " + sourcePair.Key
											+ " not found" + Environment.NewLine);
					}
					else if (target.GetValue(sourcePair.Key).Type != JTokenType.Object)
					{
						returnString.Append("Key " + sourcePair.Key
											+ " is not an object in target" + Environment.NewLine);
					}
					else
					{
						returnString.Append(CompareObjects(sourcePair.Value.ToObject<JObject>(),
							target.GetValue(sourcePair.Key).ToObject<JObject>()));
					}
				}
				else if (sourcePair.Value.Type == JTokenType.Array)
				{
					if (target.GetValue(sourcePair.Key) == null)
					{
						returnString.Append("Key " + sourcePair.Key
											+ " not found" + Environment.NewLine);
					}
					else
					{
						returnString.Append(CompareArrays(sourcePair.Value.ToObject<JArray>(),
							target.GetValue(sourcePair.Key).ToObject<JArray>(), sourcePair.Key));
					}
				}
				else
				{
					JToken expected = sourcePair.Value;
					var actual = target.SelectToken(sourcePair.Key);
					if (actual == null)
					{
						returnString.Append("Key " + sourcePair.Key
											+ " not found" + Environment.NewLine);
					}
					else
					{
						if (!JToken.DeepEquals(expected, actual))
						{
							returnString.Append("Key " + sourcePair.Key + ": "
												+ sourcePair.Value + " !=  "
												+ target.Property(sourcePair.Key).Value
												+ Environment.NewLine);
						}
					}
				}
			}
			return returnString;
		}

		/// <summary>
		/// Deep compare two NewtonSoft JArrays. If they don't match, returns text diffs
		/// </summary>
		/// <param name="source">The expected results</param>
		/// <param name="target">The actual results</param>
		/// <param name="arrayName">The name of the array to use in the text diff</param>
		/// <returns>Text string</returns>
		public static StringBuilder CompareArrays(JArray source, JArray target, string arrayName = "")
		{
			var returnString = new StringBuilder();
			for (var index = 0; index < source.Count; index++)
			{

				var expected = source[index];
				if (expected.Type == JTokenType.Object)
				{
					var actual = (index >= target.Count) ? new JObject() : target[index];
					returnString.Append(CompareObjects(expected.ToObject<JObject>(),
						actual.ToObject<JObject>()));
				}
				else
				{

					var actual = (index >= target.Count) ? "" : target[index];
					if (!JToken.DeepEquals(expected, actual))
					{
						if (String.IsNullOrEmpty(arrayName))
						{
							returnString.Append("Index " + index + ": " + expected
												+ " != " + actual + Environment.NewLine);
						}
						else
						{
							returnString.Append("Key " + arrayName
												+ "[" + index + "]: " + expected
												+ " != " + actual + Environment.NewLine);
						}
					}
				}
			}
			return returnString;
		}

		public static Tuple<JObject, JObject> GetDeltaState<TRead>(TRead before, TRead after)
		{
			if (before == null && after == null)
				return new Tuple<JObject, JObject>(null, null);

			JObject beforeResult;
			JObject afterResult;

			// If one record is null then we don't need to scan for changes
			if (before == null ^ after == null)
			{
				beforeResult = before == null ? null : JObject.FromObject(before);
				afterResult = after == null ? null : JObject.FromObject(after);

				return new Tuple<JObject, JObject>(beforeResult, afterResult);
			}

			beforeResult = new JObject();
			afterResult = new JObject();

			JObject beforeState = JObject.FromObject(before);
			JObject afterState = JObject.FromObject(after);

			// Get unique properties from each object
			IEnumerable<JProperty> properties = beforeState.Properties().Concat(afterState.Properties()).DistinctBy(x => x.Name);

			foreach (JProperty prop in properties)
			{
				JToken beforeValue = beforeState[prop.Name];
				JToken afterValue = afterState[prop.Name];

				if (JToken.DeepEquals(beforeValue, afterValue))
					continue;

				beforeResult.Add(prop.Name, beforeValue);
				afterResult.Add(prop.Name, afterValue);
			}

			return new Tuple<JObject, JObject>(beforeResult, afterResult);
		}
	}


}

public static class DynamicSerializer
{
	public static dynamic Deserialize(string payload)
	{
		if (string.IsNullOrEmpty(payload))
		{
			return new ExpandoObject();
		}

		var parsedPayload = JObject.Parse(payload);
		var result = new ExpandoObject();
		var expando = result as IDictionary<string, object>;
		foreach (var (key, value) in parsedPayload)
		{
			TransformJtoken(value, key, expando);
		}

		return result;
	}

	private static dynamic TransformJtoken(JToken value, string key, IDictionary<string, object> expando)
	{
		if (value is JValue jvalue)
		{
			expando[key] = jvalue.Value;
			return expando[key];
		}

		expando[key] = new ExpandoObject();

		// Nested object
		if (value is JObject nestedObject)
		{
			foreach (var (nestedKey, nestedValue) in nestedObject)
			{
				TransformJtoken(nestedValue, nestedKey, expando[key] as IDictionary<string, object>);
			}

			return expando[key];
		}

		// Array
		if (value is JArray array)
		{
			expando[key] = array
				.Select(x => TransformJtoken(x, key, expando[key] as IDictionary<string, object>))
				.ToList();

			return expando[key];
		}

		return null;
	}
}

