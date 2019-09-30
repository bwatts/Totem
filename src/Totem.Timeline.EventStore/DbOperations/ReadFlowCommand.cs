using System;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Totem.Runtime.Json;
using Totem.Timeline.Runtime;

namespace Totem.Timeline.EventStore.DbOperations
{
  /// <summary>
  /// Reads the <see cref="FlowInfo"/> for a particular flow
  /// </summary>
  internal sealed class ReadFlowCommand
  {
    readonly EventStoreContext _context;
    readonly FlowKey _key;
    ResolvedEvent _checkpoint;
    CheckpointMetadata _metadata;

    internal ReadFlowCommand(EventStoreContext context, FlowKey key)
    {
      _context = context;
      _key = key;
    }

    internal async Task<FlowInfo> Execute()
    {
      var stream = _key.GetCheckpointStream();

      var result = await _context.Connection.ReadEventAsync(stream, StreamPosition.End, resolveLinkTos: false);

      switch(result.Status)
      {
        case EventReadStatus.NoStream:
        case EventReadStatus.NotFound:
          return new FlowInfo.NotFound();
        case EventReadStatus.Success:
          _checkpoint = result.Event.Value;
          _metadata = _context.ReadCheckpointMetadata(_checkpoint);

          return ReadFlow();
        default:
          throw new Exception($"Unexpected result when reading {stream}: {result.Status}");
      }
    }

    FlowInfo ReadFlow()
    {
      if(_metadata.IsDone)
      {
        return new FlowInfo.NotFound();
      }
      else if(_metadata.ErrorPosition.IsSome)
      {
        return new FlowInfo.Stopped(_metadata.ErrorPosition, _metadata.ErrorMessage);
      }
      else
      {
        return new FlowInfo.Loaded(LoadFlow());
      }
    }

    Flow LoadFlow()
    {
      var flow = ReadInstance();

      FlowContext.Bind(flow, _key, _metadata.Position, TimelinePosition.None);

      return flow;
    }

    Flow ReadInstance() =>
      (Flow) _context.Json.FromJsonUtf8(_checkpoint.Event.Data, _key.Type.DeclaredType);
  }
}