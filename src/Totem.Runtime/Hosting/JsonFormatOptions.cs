using Newtonsoft.Json;
using Totem.Runtime.Json;

namespace Totem.Runtime.Hosting
{
  /// <summary>
  /// Configuration for the Totem JSON format
  /// </summary>
  public class JsonFormatOptions
  {
    public JsonSerializerSettings SerializerSettings { get; } = new JsonSerializerSettings();
    public Many<IDurableType> DurableTypes { get; } = new Many<IDurableType>();
  }
}