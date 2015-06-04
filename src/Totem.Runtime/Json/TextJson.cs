using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Totem.IO;

namespace Totem.Runtime.Json
{
	/// <summary>
	/// The text format of JSON in the Totem runtime
	/// </summary>
	public class TextJson : JsonFormat<Text>
	{
		//
		// Serialize
		//

		public override Text Serialize(object value, JsonSerializerSettings settings = null)
		{
			return JsonConvert.SerializeObject(value, settings ?? new TotemSerializerSettings());
		}

		public override Text Serialize(object value, Type type, JsonSerializerSettings settings = null)
		{
			settings = settings ?? new TotemSerializerSettings();

			return JsonConvert.SerializeObject(value, type, settings.Formatting, settings);
		}

		public override JObject SerializeJson(object value, JsonSerializerSettings settings = null)
		{
			return DeserializeJson(Serialize(value, settings));
		}

		public override JObject SerializeJson(object value, Type type, JsonSerializerSettings settings = null)
		{
			return DeserializeJson(Serialize(value, type, settings));
		}

		//
		// Deserialize
		//

		public override object Deserialize(Text value, JsonSerializerSettings settings = null)
		{
			return JsonConvert.DeserializeObject(value, settings ?? new TotemSerializerSettings());
		}

		public override object Deserialize(Text json, Type type, JsonSerializerSettings settings = null)
		{
			return JsonConvert.DeserializeObject(json, type, settings ?? new TotemSerializerSettings());
		}

		public override TEntity Deserialize<TEntity>(Text json, JsonSerializerSettings settings = null)
		{
			return JsonConvert.DeserializeObject<TEntity>(json, settings ?? new TotemSerializerSettings());
		}

		public override JObject DeserializeJson(Text value)
		{
			return JObject.Parse(value);
		}
	}
}