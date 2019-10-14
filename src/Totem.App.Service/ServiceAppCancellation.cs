using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Totem.App.Service
{
  /// <summary>
  /// Connects the host to objects created within the container
  /// </summary>
  public sealed class ServiceAppCancellation : IHostedService
  {
    public ServiceAppCancellation(IApplicationLifetime lifetimeService, CancellationToken stopToken)
    {
      stopToken.Register(lifetimeService.StopApplication);
    }

    public Task StartAsync(CancellationToken cancellationToken) =>
      Task.CompletedTask;

    public Task StopAsync(CancellationToken cancellationToken) =>
      Task.CompletedTask;
  }
}