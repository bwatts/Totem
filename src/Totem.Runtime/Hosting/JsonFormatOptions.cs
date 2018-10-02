using Newtonsoft.Json;
using Totem.Runtime.Json;

namespace Totem.Runtime.Hosting
{
  /// <summary>
  /// Configuration for the Totem JSON format
  /// </summary>
  public class JsonFormatOptions
  {
    public readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings();
    public readonly Many<IDurableType> DurableTypes = new Many<IDurableType>();
  }
}