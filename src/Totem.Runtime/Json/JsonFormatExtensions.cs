using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Totem.IO;

namespace Totem.Runtime.Json
{
  /// <summary>
  /// Formats JSON in a Totem runtime using JSON.NET
  /// </summary>
  public static class JsonFormatExtensions
  {
    //
    // To
    //

    public static string ToJson(this IJsonFormat format, object value) =>
      format.Apply(settings => JsonConvert.SerializeObject(value, settings));

    public static string ToJson(this IJsonFormat format, object value, Type type) =>
      format.Apply(settings => JsonConvert.SerializeObject(value, type, settings));

    public static JObject ToJObject(this IJsonFormat format, object value) =>
      format.Apply(settings => JObject.Parse(JsonConvert.SerializeObject(value, settings)));

    public static JObject ToJObject(this IJsonFormat format, object value, Type type) =>
      format.Apply(settings => JObject.Parse(JsonConvert.SerializeObject(value, type, settings)));

    public static JObject ToJObject(this IJsonFormat format, string json) =>
      JObject.Parse(json);

    //
    // From
    //

    public static T FromJson<T>(this IJsonFormat format, string json) =>
      format.Apply(settings => JsonConvert.DeserializeObject<T>(json, settings));

    public static object FromJson(this IJsonFormat format, string json) =>
      format.Apply(settings => JsonConvert.DeserializeObject(json, settings));

    public static object FromJson(this IJsonFormat format, string json, Type type) =>
      format.Apply(settings => JsonConvert.DeserializeObject(json, type, settings));

    public static void FromJson(this IJsonFormat format, string json, object target) =>
      format.Apply(settings => JsonConvert.PopulateObject(json, target, settings));

    //
    // To (binary)
    //

    public static Binary ToJsonUtf8(this IJsonFormat format, object value) =>
      Binary.FromUtf8(format.ToJson(value));

    public static Binary ToJsonUtf8(this IJsonFormat format, object value, Type type) =>
      Binary.FromUtf8(format.ToJson(value, type));

    public static JObject ToJObjectUtf8(this IJsonFormat format, Binary json) =>
      format.ToJObject(json.ToStringUtf8());

    public static JObject ToJObjectUtf8(this IJsonFormat format, byte[] json) =>
      format.ToJObjectUtf8(Binary.From(json));

    //
    // From (binary)
    //

    public static T FromJsonUtf8<T>(this IJsonFormat format, Binary json) =>
      format.FromJson<T>(json.ToStringUtf8());

    public static object FromJsonUtf8(this IJsonFormat format, Binary json) =>
      format.FromJson(json.ToStringUtf8());

    public static object FromJsonUtf8(this IJsonFormat format, Binary json, Type type) =>
      format.FromJson(json.ToStringUtf8(), type);

    public static void FromJsonUtf8(this IJsonFormat format, Binary json, object target) =>
      format.FromJson(json.ToStringUtf8(), target);

    public static T FromJsonUtf8<T>(this IJsonFormat format, byte[] json) =>
      format.FromJsonUtf8<T>(Binary.From(json));

    public static object FromJsonUtf8(this IJsonFormat format, byte[] json) =>
      format.FromJsonUtf8(Binary.From(json));

    public static object FromJsonUtf8(this IJsonFormat format, byte[] json, Type type) =>
      format.FromJsonUtf8(Binary.From(json), type);

    public static void FromJsonUtf8(this IJsonFormat format, byte[] json, object target) =>
      format.FromJsonUtf8(Binary.From(json), target);

    public static T FromJsonUtf8<T>(this IJsonFormat format, Stream json) =>
      format.FromJsonUtf8<T>(Binary.From(json));

    public static object FromJsonUtf8(this IJsonFormat format, Stream json) =>
      format.FromJsonUtf8(Binary.From(json));

    public static object FromJsonUtf8(this IJsonFormat format, Stream json, Type type) =>
      format.FromJsonUtf8(Binary.From(json), type);

    public static void FromJsonUtf8(this IJsonFormat format, Stream json, object target) =>
      format.FromJsonUtf8(Binary.From(json), target);
  }
}