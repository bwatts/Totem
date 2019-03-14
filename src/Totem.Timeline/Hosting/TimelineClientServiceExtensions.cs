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
      .AddHostedServiceAs<ICommandHost, CommandHost>();

    public static IServiceCollection AddTimelineClient<TArea>(this IServiceCollection services) where TArea : TimelineArea, new() =>
      services.AddTimelineClient().ConfigureArea<TArea>();

    public static IServiceCollection AddTimelineClient<TArea>(this IServiceCollection services, Action<ITimelineClientBuilder> configure) where TArea : TimelineArea, new()
    {
      services.AddTimelineClient<TArea>();

      configure(new TimelineClientBuilder(services));

      return services;
    }

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