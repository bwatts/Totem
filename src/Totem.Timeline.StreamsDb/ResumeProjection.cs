using StreamsDB.Driver;
using System.Threading.Tasks;
using Totem.Runtime;
using Totem.Timeline.Area;

namespace Totem.Timeline.EventStore
{
  /// <summary>
  /// The projection installed to track the set of flows to resume
  /// </summary>
  public sealed class ResumeProjection : Notion, IResumeProjection
  {
    private readonly StreamsDBClient _streamsDbClient;
    private readonly AreaKey _area;

    public ResumeProjection(StreamsDBClient streamsDbClient, AreaKey area)
    {
      _streamsDbClient = streamsDbClient;
      _area = area;
    }

    public async Task Synchronize()
    {
      var projection = new ResumeProjectionSubscriber(_streamsDbClient);
      await projection.Start(_area.ToString());

      Log.Debug("[timeline] Created projection {Name}", TimelineStreams.Resume);
    }   
  }
}