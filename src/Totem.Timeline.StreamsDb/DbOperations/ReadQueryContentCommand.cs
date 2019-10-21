using System.IO;
using System.Threading.Tasks;
using StreamsDB.Driver;
using Totem.Runtime.Json;
using Totem.Timeline.Client;
using Totem.Timeline.Runtime;

namespace Totem.Timeline.StreamsDb.DbOperations
{
  /// <summary>
  /// Reads the <see cref="FlowResumeInfo"/> for a particular flow with a checkpoint
  /// </summary>
  internal class ReadQueryContentCommand
  {
    readonly StreamsDbContext _context;
    readonly QueryETag _etag;
    readonly string _stream;

    internal ReadQueryContentCommand(StreamsDbContext context, QueryETag etag)
    {
      _context = context;
      _etag = etag;

      _stream = etag.Key.GetCheckpointStream(context.AreaName);
    }

    internal async Task<Stream> Execute()
    {
      long nextReadStart = -1;

      while(true)
      {
        var slice = await ReadCheckpointEventsBackward(nextReadStart);

        foreach(var e in slice.Messages)
        {
          if(HasETagCheckpoint(e))
          {
            return new MemoryStream(e.Value);
          }
        }

        if(slice.HasNext)
        {
          throw new ExpectException($"Expected checkpoint stream {_stream} to contain position {_etag.Checkpoint}");
        }

        nextReadStart = slice.Next;
      }
    }

    async Task<IStreamSlice> ReadCheckpointEventsBackward(long start)
    {
      var slice = await _context.Client.DB().ReadStreamBackward($"{_context.AreaName}-{_stream}", start, 3);
      return slice;
    }

    bool HasETagCheckpoint(Message e) =>
      _context.Json.FromJsonUtf8<CheckpointMetadata>(e.Header).Position == _etag.Checkpoint;
  }
}