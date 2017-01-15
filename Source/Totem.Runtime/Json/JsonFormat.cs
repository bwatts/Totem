using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Totem.Runtime.Json
{
	/// <summary>
	/// The set of inherent JSON formats
	/// </summary>
	public static class JsonFormat
	{
		public static readonly TextJson Text = new TextJson();

		public static readonly BinaryJson BinaryAscii = new BinaryJson(Encoding.ASCII);
		public static readonly BinaryJson BinaryBigEndianUnicode = new BinaryJson(Encoding.BigEndianUnicode);
		public static readonly BinaryJson BinaryDefault = new BinaryJson(Encoding.Default);
		public static readonly BinaryJson BinaryUnicode = new BinaryJson(Encoding.Unicode);
		public static readonly BinaryJson BinaryUtf32 = new BinaryJson(Encoding.UTF32);
		public static readonly BinaryJson BinaryUtf7 = new BinaryJson(Encoding.UTF7);
		public static readonly BinaryJson BinaryUtf8 = new BinaryJson(Encoding.UTF8);
	}

	/// <summary>
	/// The serialized format of JSON in the Totem runtime
	/// </summary>
	/// <typeparam name="T">The type to which objects are serialized and deserialized</typeparam>
	public abstract class JsonFormat<T>
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

		public abstract JObject DeserializeJson(T value, JsonLoadSettings settings = null);

    public abstract void DeserializeInto(T value, object instance, JsonSerializerSettings settings = null);
	}
}