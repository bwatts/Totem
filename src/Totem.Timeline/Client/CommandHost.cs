using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Totem.Threading;

namespace Totem.Timeline.Client
{
  /// <summary>
  /// Hosts commands executing on the timeline as a service in the .NET runtime
  /// </summary>
  internal sealed class CommandHost : ICommandHost
  {
    readonly ConcurrentDictionary<Id, WaitingCommand> _commandsById = new ConcurrentDictionary<Id, WaitingCommand>();
    readonly IClientDb _clientDb;

    public CommandHost(IClientDb clientDb)
    {
      _clientDb = clientDb;
    }

    internal async Task OnNext(TimelinePoint point)
    {
      if(_commandsById.TryGetValue(point.CommandId, out var command))
      {
        await command.OnNext(point);
      }
    }

    internal Task OnFailed(Id commandId, string error)
    {
      if(_commandsById.TryGetValue(commandId, out var command))
      {
        command.OnFailed(error);
      }

      return Task.CompletedTask;
    }

    public async Task<TResponse> Execute<TResponse>(Command command, IEnumerable<ICommandWhen<TResponse>> whens)
    {
      var id = Event.Traits.CommandId.Get(command);

      if(id.IsUnassigned)
      {
        id = Id.FromGuid();

        Event.Traits.CommandId.Set(command, id);
      }

      var waitForResponseTask = WaitForResponse(id, whens);

      await _clientDb.WriteEvent(command);

      return await waitForResponseTask;
    }

    async Task<TResponse> WaitForResponse<TResponse>(Id id, IEnumerable<ICommandWhen<TResponse>> whens)
    {
      var command = new WaitingCommand<TResponse>(whens);

      AddCommand(id, command);

      try
      {
        return await command.Task;
      }
      finally
      {
        RemoveCommand(id);
      }
    }

    void AddCommand(Id id, WaitingCommand command)
    {
      if(!_commandsById.TryAdd(id, command))
      {
        throw new Exception($"Cannot reuse command identifier {id}");
      }
    }

    void RemoveCommand(Id id) =>
      _commandsById.TryRemove(id, out var _);

    abstract class WaitingCommand
    {
      internal abstract Task OnNext(TimelinePoint point);

      internal abstract void OnFailed(string error);
    }

    class WaitingCommand<TResponse> : WaitingCommand
    {
      readonly TaskSource<TResponse> _taskSource = new TaskSource<TResponse>();
      readonly IEnumerable<ICommandWhen<TResponse>> _whens;

      internal WaitingCommand(IEnumerable<ICommandWhen<TResponse>> whens)
      {
        _whens = whens;
      }

      internal Task<TResponse> Task => _taskSource.Task;

      internal override async Task OnNext(TimelinePoint point)
      {
        var eventWhen = _whens.FirstOrDefault(when => when.EventType == point.Type.DeclaredType);

        if(eventWhen != null)
        {
          _taskSource.SetResult(await eventWhen.Respond(point.Event));
        }
      }

      internal override void OnFailed(string error) =>
        _taskSource.SetException(new Exception("Command failed with the following error: " + error));
    }
  }
}