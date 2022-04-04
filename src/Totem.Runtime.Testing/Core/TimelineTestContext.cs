using System.Text;
using Totem.Map;

namespace Totem.Core;

public class TimelineTestContext<TTimeline> : ITimelineTestContext<TTimeline>
    where TTimeline : ITimeline, new()
{
    internal TimelineTestContext()
    {
        RuntimeMap = new RuntimeMap(typeof(TTimeline));

        if(RuntimeMap.FailedChecks.Any())
        {
            var error = new StringBuilder($@"Expected timeline {typeof(TTimeline)} to pass all checks:").AppendLine();

            foreach(var failedCheck in RuntimeMap.FailedChecks)
            {
                error.AppendLine();
                error.Append("Input:    ").Append(failedCheck.Input).AppendLine();
                error.Append("Expected: ").AppendLine(failedCheck.Expected);
            }

            throw new ExpectException(error.ToString());
        }

        TimelineType = RuntimeMap.OfType<TimelineType>().Single();
        Timeline = new();
    }

    public RuntimeMap RuntimeMap { get; }
    public TimelineType TimelineType { get; }
    public TTimeline Timeline { get; }
}
