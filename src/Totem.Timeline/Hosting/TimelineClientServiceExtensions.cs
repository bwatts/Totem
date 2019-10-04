using System;
using Microsoft.Extensions.DependencyInjection;
using Totem.Runtime.Hosting;
using Totem.Timeline.Client;

namespace Totem.Timeline.Hosting
{
  /// <summary>
  /// Extends <see cref="IServiceCollection"/> to declare the timeline client
  /// </summary>
  public static class TimelineClientServiceExtensions
  {
    public static IServiceCollection AddTimelineClient(this IServiceCollection services) =>
      services
      .AddAreaMap()
      .AddOptionsSetup<TimelineJsonFormatOptionsSetup>()
      .AddSingleton<CommandHost>()
      .AddSingleton<QueryHost>()
      .AddSingleton<ICommandHost>(p => p.GetService<CommandHost>())
      .AddSingleton<IQueryHost>(p => p.GetService<QueryHost>())
      .AddHostedService<ClientHost>();

    public static IServiceCollection AddTimelineClient(this IServiceCollection services, Action<ITimelineClientBuilder> configure)
    {
      services.AddTimelineClient();

      configure(new TimelineClientBuilder(services));

      return services;
    }

    public static IServiceCollection AddTimelineClient<TArea>(this IServiceCollection services) where TArea : TimelineArea, new() =>
      services.AddTimelineClient().ConfigureArea<TArea>();

    public static IServiceCollection AddTimelineClient<TArea>(this IServiceCollection services, Action<ITimelineClientBuilder> configure) where TArea : TimelineArea, new() =>
      services.AddTimelineClient(configure).ConfigureArea<TArea>();

    class TimelineClientBuilder : ITimelineClientBuilder
    {
      readonly IServiceCollection _services;

      internal TimelineClientBuilder(IServiceCollection services)
      {
        _services = services;
      }

      public ITimelineClientBuilder ConfigureServices(Action<IServiceCollection> configure)
      {
        configure(_services);

        return this;
      }
    }
  }
}