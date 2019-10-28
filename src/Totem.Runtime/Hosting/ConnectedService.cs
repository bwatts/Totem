using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Totem.Runtime.Hosting
{
  /// <summary>
  /// An implementation of <see cref="IHostedService"/> using <see cref="Connection"/>
  /// </summary>
  public abstract class ConnectedService : Connection, IHostedService
  {
    readonly CancellationTokenSource _stopToken = new CancellationTokenSource();

    public virtual async Task StartAsync(CancellationToken cancellationToken)
    {
      try
      {
        await Connect(_stopToken.Token);
      }
      catch (System.Exception ex)
      {

        throw;
      }
    }
    

    public virtual async Task StopAsync(CancellationToken cancellationToken)
    {
      try
      {
        _stopToken.Cancel();
      }
      finally
      {
        await Task.WhenAny(Disconnect(), Task.Delay(Timeout.Infinite, cancellationToken));
      }
    }
  }
}