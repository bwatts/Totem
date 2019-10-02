using System;
using System.Threading;
using System.Threading.Tasks;

namespace Totem.Threading
{
  /// <summary>
  /// Waits a certain amount of time for an expected occurrence
  /// </summary>
  public class TimedWait
  {
    readonly TaskSource _taskSource = new TaskSource();
    CancellationTokenSource _timeoutSource;

    public TimedWait(TimeSpan timeout, Exception timeoutError)
    {
      _timeoutSource = new CancellationTokenSource(timeout);

      _timeoutSource.Token.Register(() => OnError(timeoutError));
    }

    public TimedWait(TimeSpan timeout)
      : this(timeout, new TimeoutException($"Timed out after {timeout}"))
    {}

    public Task Task => _taskSource.Task;

    public void OnOccurred()
    {
      _taskSource.TrySetResult();

      Cancel();
    }

    public void OnError(Exception error)
    {
      _taskSource.TrySetException(error);

      Cancel();
    }

    public void Cancel()
    {
      _timeoutSource?.Dispose();
      _timeoutSource = null;
    }
  }
}