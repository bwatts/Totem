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
  }
}