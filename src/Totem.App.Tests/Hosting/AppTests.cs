using System;
using System.Threading.Tasks;
using Totem.Runtime;

namespace Totem.App.Tests.Hosting
{
  /// <summary>
  /// A set of application tests hosted by the specified type
  /// </summary>
  /// <typeparam name="THost">The type of host that runs each application test</typeparam>
  public abstract class AppTests<THost> : Expectations, IDisposable, IBindable where THost : AppHost
  {
    readonly Lazy<Task<THost>> _host;
    Fields _fields;

    internal AppTests()
    {
      _host = new Lazy<Task<THost>>(() => Task.Run(StartHost));
    }

    async Task<THost> StartHost()
    {
      var host = CreateHost();

      await host.Connect();

      return host;
    }

    internal abstract THost CreateHost();

    internal Task<THost> GetOrStartHost() =>
      _host.Value;

    void IDisposable.Dispose() =>
      Task.Run(StopHost).GetAwaiter().GetResult();

    async Task StopHost()
    {
      if(_host.IsValueCreated)
      {
        var host = await _host.Value;

        await host.Disconnect();
      }
    }

    Fields IBindable.Fields => _fields ?? (_fields = new Fields(this));

    protected IClock Clock => Notion.Traits.Clock.Get(this);
  }
}