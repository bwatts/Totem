using System;
using System.Threading.Tasks;
using StreamsDB.Driver;
using Totem.Runtime.Json;
using Totem.Timeline.Runtime;

namespace Totem.Timeline.StreamsDb.DbOperations
{
  /// <summary>
  /// Reads the <see cref="FlowInfo"/> for a particular flow
  /// </summary>
  internal sealed class ReadFlowCommand
  {
    readonly StreamsDbContext _context;
    readonly FlowKey _key;
    Message _checkpoint;
    CheckpointMetadata _metadata;

    internal ReadFlowCommand(StreamsDbContext context, FlowKey key)
    {
      _context = context;
      _key = key;
    }

    internal async Task<FlowInfo> Execute()
    {
      var stream = _key.GetCheckpointStream(_context.AreaName);

      var (message, found) = await _context.Client.DB().ReadLastMessageFromStream(stream);

      if (!found)
      {
        return new FlowInfo.NotFound();
      }

      _checkpoint = message;
      _metadata = _context.ReadCheckpointMetadata(_checkpoint);

      return ReadFlow();
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
      (Flow) _context.Json.FromJsonUtf8(_checkpoint.Value, _key.Type.DeclaredType);
  }
}