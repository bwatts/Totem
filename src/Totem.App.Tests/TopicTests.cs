using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Totem.App.Tests.Hosting;
using Totem.Timeline;

namespace Totem.App.Tests
{
  /// <summary>
  /// A set of tests applied to a topic of the specified type
  /// </summary>
  /// <typeparam name="TTopic">The type of topic under test</typeparam>
  public abstract class TopicTests<TTopic> : AppTests<TopicAppHost> where TTopic : Topic
  {
    ServiceCollection _services = new ServiceCollection();

    protected IServiceCollection Services
    {
      get
      {
        Expect(_services).IsNotNull("Services are read-only once the host has started");

        return _services;
      }
    }

    internal override TopicAppHost CreateHost()
    {
      try
      {
        return new TopicAppHost(typeof(TTopic), _services);
      }
      finally
      {
        _services = null;
      }
    }

    protected async Task Append(Event e)
    {
      var host = await GetOrStartHost();

      await host.Append(e);
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

    protected Task ExpectDone(ExpectTimeout timeout = null) =>
      ExpectDone(Id.Unassigned, timeout);

    protected async Task ExpectDone(Id instanceId, ExpectTimeout timeout = null)
    {
      var host = await GetOrStartHost();

      await host.ExpectDone(instanceId, timeout ?? ExpectTimeout.Default);
    }
  }
}