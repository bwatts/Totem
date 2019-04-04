using System;
using System.Threading.Tasks;
using Totem.Timeline.IntegrationTests.Hosting;
using Xunit;

[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly)]

namespace Totem.Timeline.IntegrationTests
{
  /// <summary>
  /// A test fixture declaring timeline area types
  /// </summary>
  public abstract class TestArea : Tests, IDisposable
  {
    readonly TestApp _app;

    protected TestArea()
    {
      _app = new TestApp(this);
    }

    void IDisposable.Dispose() =>
      _app.Dispose();

    TestTimeline Timeline => _app.Timeline;

    protected Task<TimelinePosition> Append(Event e) =>
      Timeline.Append(e);

    protected Task<TEvent> Expect<TEvent>(ExpectTimeout timeout = null) where TEvent : Event =>
      Timeline.Expect<TEvent>(timeout ?? ExpectTimeout.Default, scheduled: false);

    protected Task<TEvent> ExpectScheduled<TEvent>(ExpectTimeout timeout = null) where TEvent : Event =>
      Timeline.Expect<TEvent>(timeout ?? ExpectTimeout.Default, scheduled: true);

    protected Task<TQuery> Get<TQuery>() where TQuery : Query =>
      Timeline.Get<TQuery>(Id.Unassigned);

    protected Task<TQuery> Get<TQuery>(Id id) where TQuery : Query =>
      Timeline.Get<TQuery>(id);

    protected Task<TQuery> GetAfter<TQuery>(Event e) where TQuery : Query =>
      Timeline.GetAfter<TQuery>(Id.Unassigned, e);

    protected Task<TQuery> GetAfter<TQuery>(Id id, Event e) where TQuery : Query =>
      Timeline.GetAfter<TQuery>(id, e);
  }
}