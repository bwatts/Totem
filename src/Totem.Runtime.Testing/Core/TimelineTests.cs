using Totem.Map;

namespace Totem.Core;

public abstract class TimelineTests<TTimeline> where TTimeline : ITimeline
{
    bool _expectationsStarted;

    internal abstract ITimelineTestContext<TTimeline> TestContext { get; }

    internal void OnCallingWhen()
    {
        if(_expectationsStarted)
            throw new InvalidOperationException($"Call all {TimelineMethod.When} methods before starting expectations");

        if(TestContext.Timeline.HasErrors)
            throw new ExpectException($"Expected no errors but found [{string.Join("; ", TestContext.Timeline.Errors)}]");
    }

    internal void OnExpectation()
    {
        if(TestContext.Timeline.HasErrors)
            throw new ExpectException($"Expected no errors but found [{string.Join("; ", TestContext.Timeline.Errors)}]");

        _expectationsStarted = true;
    }

    protected void ExpectError(ErrorInfo error)
    {
        if(error is null)
            throw new ArgumentNullException(nameof(error));

        _expectationsStarted = true;

        if(!TestContext.Timeline.Errors.Contains(error))
            throw new ExpectException($"Expected error {error} but found [{string.Join(", ", TestContext.Timeline.Errors)}]");
    }
}
