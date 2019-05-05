using System;
using Totem.Runtime;

namespace Totem.Timeline
{
  /// <summary>
  /// A timeline presence that makes decisions
  /// </summary>
  public abstract class Topic : Flow
  {
    ITopicScheduler _thenSchedule;

    [Transient]
    protected ITopicScheduler ThenSchedule =>
      _thenSchedule ?? (_thenSchedule = new Scheduler(this));

    protected void Then(Event e)
    {
      var whenCall = Context.Call as FlowCall.When;

      Expect.That(whenCall).IsNotNull("Topic is not making a When call. This may indicate an asynchronous operation was not awaited in a prior When call.");

      whenCall.Append(e);
    }

    protected void ThenDone(Event e)
    {
      Then(e);

      ThenDone();
    }

    class Scheduler : ITopicScheduler
    {
      readonly Topic _topic;

      internal Scheduler(Topic topic)
      {
        _topic = topic;
      }

      public DateTimeOffset Now => _topic.Clock.Now;

      public void At(Event e, DateTimeOffset whenOccurs)
      {
        Event.Traits.WhenOccurs.Set(e, whenOccurs);

        _topic.Then(e);
      }
    }
  }
}