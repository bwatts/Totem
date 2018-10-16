using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Totem.Runtime.Json;

namespace Totem.Runtime.Hosting
{
  /// <summary>
  /// Configures default values for instances of <see cref="JsonFormatOptions"/> 
  /// </summary>
  public class JsonFormatOptionsSetup : IConfigureOptions<JsonFormatOptions>, IPostConfigureOptions<JsonFormatOptions>
  {
    public void Configure(JsonFormatOptions options)
    {
      var settings = options.SerializerSettings;

      settings.Formatting = Formatting.Indented;
      settings.TypeNameHandling = TypeNameHandling.Auto;

      settings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
      settings.DateParseHandling = DateParseHandling.None;

      settings.Converters.AddRange(
        new StringEnumConverter(),
        new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fffK" });
    }

    public void PostConfigure(string name, JsonFormatOptions options)
    {
      var durableTypes = new DurableTypeSet(options.DurableTypes);

      options.SerializerSettings.ContractResolver = new JsonFormatContractResolver(durableTypes);
      options.SerializerSettings.SerializationBinder = new JsonFormatSerializationBinder(durableTypes);
    }
  }
}