using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Totem.Runtime.Hosting;

namespace Totem.Timeline.Mvc.Hosting
{
  /// <summary>
  /// Configures JSON serialization via <see cref="MvcJsonOptions"/> 
  /// </summary>
  public class WebRuntimeOptionsSetup : IPostConfigureOptions<MvcJsonOptions>
  {
    readonly IOptions<JsonFormatOptions> _jsonFormatOptions;

    public WebRuntimeOptionsSetup(IOptions<JsonFormatOptions> jsonFormatOptions)
    {
      _jsonFormatOptions = jsonFormatOptions;
    }

    public void PostConfigure(string name, MvcJsonOptions options)
    {
      var source = _jsonFormatOptions.Value.SerializerSettings;
      var target = options.SerializerSettings;

      target.Context = source.Context;
      target.Culture = source.Culture;
      target.ContractResolver = source.ContractResolver;
      target.ConstructorHandling = source.ConstructorHandling;
      target.Converters = source.Converters;
      target.CheckAdditionalContent = source.CheckAdditionalContent;
      target.DateFormatHandling = source.DateFormatHandling;
      target.DateFormatString = source.DateFormatString;
      target.DateParseHandling = source.DateParseHandling;
      target.DateTimeZoneHandling = source.DateTimeZoneHandling;
      target.DefaultValueHandling = source.DefaultValueHandling;
      target.EqualityComparer = source.EqualityComparer;
      target.FloatFormatHandling = source.FloatFormatHandling;
      target.Formatting = source.Formatting;
      target.FloatParseHandling = source.FloatParseHandling;
      target.MaxDepth = source.MaxDepth;
      target.MetadataPropertyHandling = source.MetadataPropertyHandling;
      target.MissingMemberHandling = source.MissingMemberHandling;
      target.NullValueHandling = source.NullValueHandling;
      target.ObjectCreationHandling = source.ObjectCreationHandling;
      target.PreserveReferencesHandling = source.PreserveReferencesHandling;
      target.ReferenceLoopHandling = source.ReferenceLoopHandling;
      target.SerializationBinder = source.SerializationBinder;
      target.StringEscapeHandling = source.StringEscapeHandling;
      target.TraceWriter = source.TraceWriter;
      target.TypeNameHandling = source.TypeNameHandling;
      target.TypeNameAssemblyFormatHandling = source.TypeNameAssemblyFormatHandling;
    }
  }
}