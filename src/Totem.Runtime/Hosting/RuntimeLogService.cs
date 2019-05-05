using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Totem.Runtime.Hosting
{
  /// <summary>
  /// Hosts the <see cref="ILoggerFactory"/> as the source of runtime loggers
  /// </summary>
  public class RuntimeLogService : IHostedService
  {
    readonly ILoggerFactory _factory;

    public RuntimeLogService(ILoggerFactory factory)
    {
      _factory = factory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
      Notion.Traits.Log.SetDefault(binding => _factory.CreateLogger(binding.GetType()));

      return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
      Notion.Traits.ResetLog();

      return Task.CompletedTask;
    }
  }
}