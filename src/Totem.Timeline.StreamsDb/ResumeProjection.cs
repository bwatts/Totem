using StreamsDB.Driver;
using System.Threading.Tasks;
using Totem.Runtime;
using Totem.Timeline.Area;

namespace Totem.Timeline.StreamsDb
{
  /// <summary>
  /// The projection installed to track the set of flows to resume
  /// </summary>
  public sealed class ResumeProjection : Notion, IResumeProjection
  {
    private readonly StreamsDbContext _context;

    public ResumeProjection(StreamsDbContext context)
    {
      _context = context;
    }

    public Task Synchronize()
    {
      var projection = new ResumeProjectionSubscriber(_context);
      projection.Start("default");

      Log.Debug("[timeline] Created projection {Name}", TimelineStreams.Resume);

      return Task.CompletedTask;
    }   
  }
}