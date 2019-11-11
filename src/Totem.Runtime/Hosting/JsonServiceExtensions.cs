using System;
using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Totem.Runtime.Json;

namespace Totem.Runtime.Hosting
{
  /// <summary>
  /// Extends <see cref="IServiceCollection"/> to declare the Totem JSON format
  /// </summary>
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class JsonServiceExtensions
  {
    public static IServiceCollection AddJsonFormat(this IServiceCollection services) =>
      services
      .AddOptionsSetup<JsonFormatOptionsSetup>()
      .AddSingleton<IJsonFormat>(provider =>
        new JsonFormat(provider.GetOptions<JsonFormatOptions>().SerializerSettings));

    public static IServiceCollection AddJsonFormat(this IServiceCollection services, Action<JsonFormatOptions> configure) =>
      services.AddJsonFormat().Configure(configure);

    public static IServiceCollection ConfigureJsonFormat(this IServiceCollection services, Action<JsonFormatOptions> configure) =>
      services.Configure(configure);

    //
    // Durable types
    //

    public static JsonFormatOptions TryAddDurableType(this JsonFormatOptions options, DurablePrefix prefix, Type declaredType)
    {
      if(DurableType.TryFrom(prefix, declaredType, out var type))
      {
        options.DurableTypes.Add(type);
      }

      return options;
    }

    public static JsonFormatOptions TryAddDurableType(this JsonFormatOptions options, string prefix, Type declaredType)
    {
      if(DurableType.TryFrom(prefix, declaredType, out var type))
      {
        options.DurableTypes.Add(type);
      }

      return options;
    }

    public static JsonFormatOptions TryAddDurableType(this JsonFormatOptions options, Type declaredType)
    {
      if(DurableType.TryFrom(declaredType, out var type))
      {
        options.DurableTypes.Add(type);
      }

      return options;
    }

    public static JsonFormatOptions TryAddDurableType<T>(this JsonFormatOptions options) =>
      options.TryAddDurableType(typeof(T));

    public static JsonFormatOptions AddDurableType(this JsonFormatOptions options, DurablePrefix prefix, Type declaredType)
    {
      options.DurableTypes.Add(DurableType.From(prefix, declaredType));

      return options;
    }

    public static JsonFormatOptions AddDurableType(this JsonFormatOptions options, string prefix, Type declaredType)
    {
      options.DurableTypes.Add(DurableType.From(prefix, declaredType));

      return options;
    }

    public static JsonFormatOptions AddDurableType(this JsonFormatOptions options, Type declaredType)
    {
      options.DurableTypes.Add(DurableType.From(declaredType));

      return options;
    }

    public static JsonFormatOptions AddDurableType<T>(this JsonFormatOptions options) =>
      options.AddDurableType(typeof(T));
  }
}