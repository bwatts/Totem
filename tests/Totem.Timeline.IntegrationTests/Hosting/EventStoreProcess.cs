using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Totem.Runtime;

namespace Totem.Timeline.IntegrationTests.Hosting
{
  /// <summary>
  /// An external EventStore process managed by a test area
  /// </summary>
  internal sealed class EventStoreProcess : Connection
  {
    readonly EventStoreProcessCommand _command;
    readonly TimeSpan _readyDelay;
    Process _process;

    internal EventStoreProcess(EventStoreProcessCommand command, TimeSpan readyDelay)
    {
      _command = command;
      _readyDelay = readyDelay;
    }

    protected override async Task Open()
    {
      _process = await _command.StartProcess();

      await Task.Delay(_readyDelay);
    }

    protected override Task Close()
    {
      if(!_process.HasExited)
      {
        _process.Kill();
      }

      _process.WaitForExit();
      _process.Dispose();

      return Task.CompletedTask;
    }
  }
}