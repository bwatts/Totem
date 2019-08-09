using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Totem.App.Tests;
using Totem.App.Tests.Hosting;

namespace Totem.Timeline.IntegrationTests.Hosting
{
  /// <summary>
  /// A test applied to the timeline via an app declared by the fixture
  /// </summary>
  public abstract class IntegrationTest : AppTests<IntegrationAppHost>
  {
    ServiceCollection _services = new ServiceCollection();

    protected IServiceCollection Services
    {
      get
      {
        Expect(_services).IsNull("Services are read-only once the host has started");

        return _services;
      }
    }

    internal override IntegrationAppHost CreateHost()
    {
      try
      {
        return new IntegrationAppHost(GetType(), _services);
      }
      finally
      {
        _services = null;
      }
    }

    protected async Task<TimelinePosition> Append(Event e)
    {
      var host = await GetOrStartHost();

      return await host.Append(e);
    }

    protected Task<TEvent> Expect<TEvent>(ExpectTimeout timeout = null) where TEvent : Event =>
      GetOrStartAndExpect<TEvent>(timeout, false);

    protected Task<TEvent> ExpectScheduled<TEvent>(ExpectTimeout timeout = null) where TEvent : Event =>
      GetOrStartAndExpect<TEvent>(timeout, true);

    async Task<TEvent> GetOrStartAndExpect<TEvent>(ExpectTimeout timeout, bool scheduled) where TEvent : Event
    {
      var host = await GetOrStartHost();

      return await host.Expect<TEvent>(timeout ?? ExpectTimeout.Default, scheduled);
    }

    protected Task<TQuery> AppendAndGet<TQuery>(Event e, ExpectTimeout changedTimeout = null) where TQuery : Query =>
      AppendAndGet<TQuery>(Id.Unassigned, e, changedTimeout ?? ExpectTimeout.Default);

    protected async Task<TQuery> AppendAndGet<TQuery>(Id queryId, Event e, ExpectTimeout changedTimeout = null) where TQuery : Query
    {
      var host = await GetOrStartHost();

      return await host.AppendAndGet<TQuery>(queryId, e, changedTimeout ?? ExpectTimeout.Default);
    }

    protected Task<TQuery> Get<TQuery>() where TQuery : Query =>
      Get<TQuery>(Id.Unassigned);

    protected async Task<TQuery> Get<TQuery>(Id queryId) where TQuery : Query
    {
      var host = await GetOrStartHost();

      return await host.Get<TQuery>(queryId);
    }
  }
}