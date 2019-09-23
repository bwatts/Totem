using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Totem.App.Tests.Hosting
{
  /// <summary>
  /// Connects the host to objects created within the container
  /// </summary>
  internal sealed class TopicAppLifetime : IHostLifetime
  {
    public TopicAppLifetime(TopicAppHost host, TopicApp app, IApplicationLifetime lifetimeService)
    {
      host.SetApp(app);
      host.SetLifetimeService(lifetimeService);
    }

    public Task WaitForStartAsync(CancellationToken cancellationToken) =>
      Task.CompletedTask;

    public Task StopAsync(CancellationToken cancellationToken) =>
      Task.CompletedTask;
  }
}