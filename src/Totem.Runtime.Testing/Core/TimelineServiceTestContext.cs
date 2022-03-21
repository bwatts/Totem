using System.Text;
using Totem.Map;

namespace Totem.Core;

public class TimelineServiceTestContext<TTimeline> : ITimelineTestContext<TTimeline>
    where TTimeline : ITimeline
{
    TTimeline _timeline = default!;

    internal TimelineServiceTestContext()
    {
        RuntimeMap = new RuntimeMap(typeof(TTimeline));

        var failedChecks = RuntimeMap.Checks.Where(x => !x.HasOutput).ToList();

        if(failedChecks.Any())
        {
            var error = new StringBuilder($@"Expected timeline {typeof(TTimeline)} to pass all checks:").AppendLine();

            foreach(var failedCheck in failedChecks)
            {
                error.AppendLine();
                error.Append("Input:    ").Append(failedCheck.Input).AppendLine();
                error.Append("Expected: ").AppendLine(failedCheck.Expected);
            }

            throw new ExpectException(error.ToString());
        }

        TimelineType = RuntimeMap.OfType<TimelineType>().Single();
    }

    public RuntimeMap RuntimeMap { get; }
    public TimelineType TimelineType { get; }
    public TTimeline Timeline
    {
        get
        {
            if(_timeline is null)
                throw new InvalidOperationException("Timeline must be set before use");

            return _timeline;
        }
    }

    internal void SetTimeline(TTimeline timeline)
    {
        if(timeline is null)
            throw new ArgumentNullException(nameof(timeline));

        if(_timeline is not null)
            throw new InvalidOperationException("Timeline is already set");

        _timeline = timeline;
    }
}
