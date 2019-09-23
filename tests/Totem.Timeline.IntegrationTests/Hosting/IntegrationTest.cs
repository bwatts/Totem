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

    protected async Task<TEvent> Expect<TEvent>(ExpectTimeout timeout = null) where TEvent : Event
    {
      var host = await GetOrStartHost();

      return await host.Expect<TEvent>(timeout ?? ExpectTimeout.Default, scheduled: false);
    }

    protected async Task<TEvent> ExpectScheduled<TEvent>(ExpectTimeout timeout = null) where TEvent : Event
    {
      var host = await GetOrStartHost();

      return await host.Expect<TEvent>(timeout ?? ExpectTimeout.Default, scheduled: true);
    }

    protected Task<TQuery> GetQuery<TQuery>(ExpectTimeout timeout = null) where TQuery : Query =>
      GetQuery<TQuery>(Id.Unassigned, timeout);

    protected async Task<TQuery> GetQuery<TQuery>(Id instanceId, ExpectTimeout timeout = null) where TQuery : Query
    {
      var host = await GetOrStartHost();

      return await host.GetQuery<TQuery>(instanceId, timeout ?? ExpectTimeout.Default);
    }
  }
}