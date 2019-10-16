using System.Threading.Tasks;
using Totem.Runtime;

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
      var resumeAreaProjection = new ResumeAreaSubscriber(_context);
      resumeAreaProjection.Start();

      foreach(var flowType in _context.Area.FlowTypes)
      {
        var resumeProgressProjection = new ResumeProgressSubscriber(_context, flowType);
        resumeProgressProjection.Start();
      }

      Log.Debug("[timeline] Created projection {Name}", TimelineStreams.Resume);

      return Task.CompletedTask;
    }   
  }
}