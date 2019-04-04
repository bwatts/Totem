using System;
using System.Threading;
using System.Threading.Tasks;
using Totem.Timeline.Area;

namespace Totem.Timeline.IntegrationTests.Hosting
{
  /// <summary>
  /// An expectation in a <see cref="TestArea"/> of receiving an event in a specified time frame
  /// </summary>
  internal sealed class PendingExpect
  {
    readonly TaskSource _taskSource = new TaskSource();
    readonly EventType _eventType;
    readonly ExpectTimeout _timeout;
    readonly CancellationTokenSource _tokenSource;

    internal PendingExpect(EventType eventType, ExpectTimeout timeout)
    {
      _eventType = eventType;
      _timeout = timeout;

      _tokenSource = new CancellationTokenSource(timeout.TimeSpan);

      _tokenSource.Token.Register(SetTimedOut);
    }

    internal Task WaitForNextPoint() =>
      _taskSource.Task;

    internal void Continue() =>
      _taskSource.TrySetResult();

    void SetTimedOut() =>
      _taskSource.TrySetException(new TimeoutException($"Timed out after expecting event of type {_eventType} for {_timeout.TimeSpan}"));
  }
}