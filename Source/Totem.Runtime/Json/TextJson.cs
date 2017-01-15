using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

		public override object Deserialize(Text value, Type type, JsonSerializerSettings settings = null)
		{
			return JsonConvert.DeserializeObject(value, type, settings ?? new TotemSerializerSettings());
		}

		public override TEntity Deserialize<TEntity>(Text value, JsonSerializerSettings settings = null)
		{
			return JsonConvert.DeserializeObject<TEntity>(value, settings ?? new TotemSerializerSettings());
		}

		public override JObject DeserializeJson(Text value, JsonLoadSettings settings = null)
		{
      return JObject.Parse(value, settings);
    }

    public override void DeserializeInto(Text value, object instance, JsonSerializerSettings settings = null)
    {
      JsonConvert.PopulateObject(value, instance, settings);
    }
  }
}