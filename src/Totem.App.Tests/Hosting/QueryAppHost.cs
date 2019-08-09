using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Totem.Runtime.Hosting;
using Totem.Timeline;
using Totem.Timeline.Hosting;
using Totem.Timeline.Runtime;

namespace Totem.App.Tests.Hosting
{
  /// <summary>
  /// Hosts a timeline application for the duration of a query test
  /// </summary>
  public sealed class QueryAppHost : FlowAppHost
  {
    QueryApp _app;

    internal QueryAppHost(Type queryType) : base(queryType)
    {}

    protected override void ConfigureServices(IServiceCollection services) =>
      services
      .AddTotemRuntime()
      .AddTimeline()
      .ConfigureArea(GetAreaTypes())
      .AddSingleton<QueryApp>()
      .AddSingleton<QueryAppTimelineDb>()
      .AddSingleton<ITimelineDb>(p => p.GetService<QueryAppTimelineDb>())
      .AddSingleton<IHostLifetime>(p => new QueryAppLifetime(
        this,
        p.GetService<QueryApp>(),
        p.GetService<IApplicationLifetime>()));

    internal void SetApp(QueryApp app) =>
      _app = app;

    internal Task Append(Event e) =>
      _app.Append(e);

    internal Task<TQuery> AppendAndGet<TQuery>(Id queryId, Event e, ExpectTimeout changeTimeout) where TQuery : Query =>
      _app.AppendAndGet<TQuery>(queryId, e, changeTimeout ?? ExpectTimeout.Default);
  }
}