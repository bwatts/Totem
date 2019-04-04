using System.IO;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Totem.Runtime.Json;
using Totem.Timeline.Client;
using Totem.Timeline.Runtime;

namespace Totem.Timeline.EventStore.DbOperations
{
  /// <summary>
  /// Reads the <see cref="FlowResumeInfo"/> for a particular flow with a checkpoint
  /// </summary>
  internal class ReadQueryContentCommand
  {
    readonly EventStoreContext _context;
    readonly QueryETag _etag;
    readonly string _stream;

    internal ReadQueryContentCommand(EventStoreContext context, QueryETag etag)
    {
      _context = context;
      _etag = etag;

      _stream = etag.Key.GetCheckpointStream();
    }

    internal async Task<Stream> Execute()
    {
      long nextReadStart = StreamPosition.End;

      while(true)
      {
        var slice = await ReadCheckpointEventsBackward(nextReadStart);

        foreach(var e in slice.Events)
        {
          if(HasETagCheckpoint(e.Event))
          {
            return new MemoryStream(e.Event.Data);
          }
        }

        if(slice.IsEndOfStream)
        {
          throw new ExpectException($"Expected checkpoint stream {_stream} to contain position {_etag.Checkpoint}");
        }

        nextReadStart = slice.NextEventNumber;
      }
    }

    async Task<StreamEventsSlice> ReadCheckpointEventsBackward(long start)
    {
      var slice = await _context.Connection.ReadStreamEventsBackwardAsync(_stream, start, 3, resolveLinkTos: false);

      if(slice.Status != SliceReadStatus.Success)
      {
        throw new ExpectException($"Unexpected result when reading {_stream}: {slice.Status}");
      }

      return slice;
    }

    bool HasETagCheckpoint(RecordedEvent e) =>
      _context.Json.FromJsonUtf8<CheckpointMetadata>(e.Metadata).Position == _etag.Checkpoint;
  }
}