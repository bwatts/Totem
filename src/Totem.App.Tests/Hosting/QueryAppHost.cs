using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Totem.Runtime.Hosting;
using Totem.Timeline;
using Totem.Timeline.Area;
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
      .AddSingleton(p => p.GetService<AreaMap>().Queries[FlowType])
      .AddSingleton(this)
      .AddSingleton<QueryApp>()
      .AddSingleton<QueryAppTimelineDb>()
      .AddSingleton<ITimelineDb>(p => p.GetService<QueryAppTimelineDb>())
      .AddSingleton<IHostLifetime, QueryAppLifetime>();

    internal void SetApp(QueryApp app) =>
      _app = app;

    internal Task Append(Event e) =>
      _app.Append(e);

    internal Task<TQuery> GetQuery<TQuery>(Id instanceId, ExpectTimeout timeout) where TQuery : Query =>
      _app.GetQuery<TQuery>(instanceId, timeout);

    internal Task ExpectDone(Id instanceId, ExpectTimeout timeout) =>
      _app.ExpectDone(instanceId, timeout);
  }
}