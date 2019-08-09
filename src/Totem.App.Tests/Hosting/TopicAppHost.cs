using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Totem.Runtime.Hosting;
using Totem.Timeline;
using Totem.Timeline.Hosting;
using Totem.Timeline.Runtime;

namespace Totem.App.Tests.Hosting
{
  /// <summary>
  /// Hosts a timeline application for the duration of a topic test
  /// </summary>
  /// <remarks>
  /// This test pattern assumes single-assembly areas (for now). Otherwise, tests have to enumerate the relevant events.
  /// </remarks>
  public sealed class TopicAppHost : FlowAppHost
  {
    readonly IServiceCollection _appServices;
    TopicApp _app;

    internal TopicAppHost(Type topicType, IServiceCollection appServices) : base(topicType)
    {
      _appServices = appServices;
    }

    protected override void ConfigureServices(IServiceCollection services) =>
      services
      .AddTotemRuntime()
      .AddTimeline()
      .ConfigureArea(GetAreaTypes())
      .AddSingleton<TopicApp>()
      .AddSingleton<TopicAppTimelineDb>()
      .AddSingleton<ITimelineDb>(p => p.GetService<TopicAppTimelineDb>())
      .AddSingleton<IHostLifetime>(p => new TopicAppLifetime(
        this,
        p.GetService<TopicApp>(),
        p.GetService<IApplicationLifetime>()))
      .Add(_appServices);

    internal void SetApp(TopicApp app) =>
      _app = app;

    internal Task Append(Event e) =>
      _app.Append(e);

    internal Task<TEvent> Expect<TEvent>(ExpectTimeout timeout, bool scheduled) where TEvent : Event =>
      _app.Expect<TEvent>(timeout, scheduled);
  }
}