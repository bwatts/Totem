using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Totem.App.Tests;
using Totem.App.Tests.Hosting;

namespace Totem.Timeline.IntegrationTests.Hosting
{
  /// <summary>
  /// Hosts a timeline application for the duration of an integration test
  /// </summary>
  public sealed class IntegrationAppHost : AppHost
  {
    IntegrationApp _app;

    public IntegrationAppHost(Type testType, IServiceCollection appServices)
    {
      TestType = testType;
      AppServices = appServices;
    }

    internal readonly Type TestType;
    internal readonly IServiceCollection AppServices;

    protected override IHostBuilder CreateBuilder() =>
      new HostBuilder().ConfigureIntegrationApp(this);

    internal void SetApp(IntegrationApp app) =>
      _app = app;

    internal Task<TimelinePosition> Append(Event e) =>
      _app.Append(e);

    internal Task<TEvent> Expect<TEvent>(ExpectTimeout timeout, bool scheduled) where TEvent : Event =>
      _app.Expect<TEvent>(timeout, scheduled);

    internal Task<TQuery> AppendAndGet<TQuery>(Id queryId, Event e, ExpectTimeout timeout) where TQuery : Query =>
      _app.AppendAndGet<TQuery>(queryId, e, timeout);

    internal Task<TQuery> Get<TQuery>(Id queryId) where TQuery : Query =>
      _app.Get<TQuery>(queryId);
  }
}