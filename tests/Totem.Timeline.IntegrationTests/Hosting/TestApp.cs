using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace Totem.Timeline.IntegrationTests.Hosting
{
  /// <summary>
  /// A composed instance of the timeline hosted for a test
  /// </summary>
  /// <remarks>
  /// xUnit runs the body of tests after the <see cref="TestArea"/> constructor completes.
  /// It also tears down the app domain, possibly leaving external EventStore processes active.
  ///
  /// Block threads in both cases until the app finishes starting/stopping.
  /// </remarks>
  internal sealed class TestApp : IDisposable
  {
    readonly ManualResetEvent _startBlock = new ManualResetEvent(false);
    readonly ManualResetEvent _stopBlock = new ManualResetEvent(false);
    readonly TestArea _area;
    Exception _error;

    internal TestApp(TestArea area)
    {
      _area = area;

      TestHost.Start(this);

      _startBlock.WaitOne();

      if(_error != null)
      {
        throw new Exception("Failed to start test timeline", _error);
      }
    }

    internal TestLifetime Lifetime { get; private set; }
    internal TestTimeline Timeline { get; private set; }

    internal void OnStarted(TestLifetime lifetime, TestTimeline timeline)
    {
      Lifetime = lifetime;
      Timeline = timeline;

      _startBlock.Set();
    }

    internal void OnStartFailed(Exception error)
    {
      _error = error;

      _startBlock.Set();
      _stopBlock.Set();
    }

    internal Assembly GetAreaAssembly() =>
      _area.GetType().Assembly;

    internal IEnumerable<Type> GetAreaTypes() =>
      _area.GetType().GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic);

    public void Dispose()
    {
      Lifetime.StopApp();

      _stopBlock.WaitOne();

      if(_error != null)
      {
        throw new Exception("Failed to stop test timeline", _error);
      }
    }

    internal void OnStopped() =>
      _stopBlock.Set();
  }
}