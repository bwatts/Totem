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
	/// The binary format of JSON in the Totem runtime
	/// </summary>
	public class BinaryJsonFormat : TotemJsonFormat<Binary>
	{
		public BinaryJsonFormat(Encoding encoding)
		{
			Encoding = encoding;
		}

		public readonly Encoding Encoding;

		//
		// Serialize
		//

		// These are shortcuts to and from text - it is possible to skip string representations and use binary natively. For more information, see:
		//
		// http://james.newtonking.com/json/help/index.html?topic=html/T_Newtonsoft_Json_Bson_BsonReader.htm

		public override Binary Serialize(object value, JsonSerializerSettings settings = null)
		{
			return GetBinary(TotemJson.Text.Serialize(value, settings));
		}

		public override Binary Serialize(object value, Type type, JsonSerializerSettings settings = null)
		{
			return GetBinary(TotemJson.Text.Serialize(value, type, settings));
		}

		public override JObject SerializeJson(object value, JsonSerializerSettings settings = null)
		{
			return TotemJson.Text.SerializeJson(value, settings);
		}

		public override JObject SerializeJson(object value, Type type, JsonSerializerSettings settings = null)
		{
			return TotemJson.Text.SerializeJson(value, type, settings);
		}

		private Binary GetBinary(Text json)
		{
			return Binary.From(Encoding.GetBytes(json));
		}

		//
		// Deserialize
		//

		public override object Deserialize(Binary value, JsonSerializerSettings settings = null)
		{
			return TotemJson.Text.Deserialize(GetText(value));
		}

		public override object Deserialize(Binary value, Type type, JsonSerializerSettings settings = null)
		{
			return TotemJson.Text.Deserialize(GetText(value), type, settings);
		}

		public override TValue Deserialize<TValue>(Binary value, JsonSerializerSettings settings = null)
		{
			return TotemJson.Text.Deserialize<TValue>(GetText(value), settings);
		}

		public override JObject DeserializeJson(Binary value)
		{
			return TotemJson.Text.DeserializeJson(GetText(value));
		}

		private string GetText(Binary value)
		{
			return Encoding.GetString(value.ToBytes());
		}
	}
}