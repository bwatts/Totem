using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Totem.Runtime.Json
{
	/// <summary>
	/// The serialized format of JSON in the Totem runtime
	/// </summary>
	/// <typeparam name="T">The type to which objects are serialized and deserialized</typeparam>
	public abstract class TotemJsonFormat<T>
	{
		public abstract T Serialize(object value, JsonSerializerSettings settings = null);

		public abstract T Serialize(object value, Type type, JsonSerializerSettings settings = null);

		public abstract JObject SerializeJson(object value, JsonSerializerSettings settings = null);

		public abstract JObject SerializeJson(object value, Type type, JsonSerializerSettings settings = null);

		public abstract object Deserialize(T value, JsonSerializerSettings settings = null);

		public abstract object Deserialize(T value, Type type, JsonSerializerSettings settings = null);

		public virtual TValue Deserialize<TValue>(T value, JsonSerializerSettings settings = null) where TValue : class
		{
			return (TValue) Deserialize(value, typeof(TValue), settings);
		}

		public abstract JObject DeserializeJson(T value);
	}
}