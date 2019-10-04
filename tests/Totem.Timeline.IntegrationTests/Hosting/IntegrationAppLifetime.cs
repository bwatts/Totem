using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Totem.Timeline.IntegrationTests.Hosting
{
  /// <summary>
  /// Connects the host to objects created within the container
  /// </summary>
  internal sealed class IntegrationAppLifetime : IHostLifetime
  {
    readonly IntegrationApp _app;

    internal IntegrationAppLifetime(IntegrationAppHost host, IntegrationApp app, IApplicationLifetime lifetimeService)
    {
      _app = app;

      host.SetApp(app);
      host.SetLifetimeService(lifetimeService);
    }

    public Task WaitForStartAsync(CancellationToken cancellationToken) =>
      _app.Connect();

    public Task StopAsync(CancellationToken cancellationToken) =>
      _app.Disconnect();
  }
}