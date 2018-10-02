using System;
using System.ComponentModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Totem.Reflection;

namespace Totem.Runtime.Hosting
{
  /// <summary>
  /// Extends <see cref="IServiceCollection"/> for common declarations
  /// </summary>
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class OptionsExtensions
  {
    public static TOptions GetOptions<TOptions>(this IServiceProvider provider) where TOptions : class, new() =>
      provider.GetRequiredService<IOptions<TOptions>>().Value;

    //
    // AddOptionsSetup
    //

    public static IServiceCollection AddOptionsSetup<TOptions, TSetup>(this IServiceCollection services)
      where TOptions : class
      where TSetup : class
    {
      if(typeof(IConfigureOptions<TOptions>).IsAssignableFrom(typeof(TSetup)))
      {
        services.AddTransient(typeof(IConfigureOptions<TOptions>), typeof(TSetup));
      }

      if(typeof(IPostConfigureOptions<TOptions>).IsAssignableFrom(typeof(TSetup)))
      {
        services.AddTransient(typeof(IPostConfigureOptions<TOptions>), typeof(TSetup));
      }

      return services;
    }

    public static IServiceCollection AddOptionsSetup(this IServiceCollection services, Type optionsType, Type setupType)
    {
      var configureType = typeof(IConfigureOptions<>).MakeGenericType(optionsType);
      var postConfigureType = typeof(IPostConfigureOptions<>).MakeGenericType(optionsType);

      foreach(var type in new[] { configureType, postConfigureType })
      {
        if(type.IsAssignableFrom(setupType))
        {
          services.AddTransient(type, setupType);
        }
      }

      return services;
    }

    public static IServiceCollection AddOptionsSetup(this IServiceCollection services, Type setupType)
    {
      if(typeof(IConfigureOptions<>).TryGetAssignableGenericType(setupType, out var configureOptionsType))
      {
        services.AddTransient(configureOptionsType, setupType);
      }

      if(typeof(IPostConfigureOptions<>).TryGetAssignableGenericType(setupType, out var postConfigureOptionsType))
      {
        services.AddTransient(postConfigureOptionsType, setupType);
      }

      return services;
    }

    public static IServiceCollection AddOptionsSetup<TSetup>(this IServiceCollection services) =>
      services.AddOptionsSetup(typeof(TSetup));

    //
    // BindOptions
    //

    public static IServiceCollection BindOptionsToConfiguration<TOptions>(this IServiceCollection services, string key) where TOptions : class
    {
      if(Check.That(key).IsNotNullOrWhiteSpace())
      {
        services.AddTransient<IConfigureOptions<TOptions>>(provider =>
          new BindOptionsSetup<TOptions>(provider.GetRequiredService<IConfiguration>(), key));
      }

      return services;
    }

    class BindOptionsSetup<TOptions> : IConfigureOptions<TOptions> where TOptions : class
    {
      readonly IConfiguration _config;
      readonly string _key;

      public BindOptionsSetup(IConfiguration config, string key)
      {
        _config = config;
        _key = key;
      }

      public void Configure(TOptions options) =>
        _config.Bind(_key, options);
    }
  }
}