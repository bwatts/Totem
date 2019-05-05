using System;
using System.Threading.Tasks;
using Totem.Runtime;

namespace Totem.Timeline.IntegrationTests.Hosting
{
  /// <summary>
  /// A timeline area declared by a test fixture
  /// </summary>
  internal sealed class TestTimeline : Connection, IDisposable
  {
    readonly EventStoreProcess _eventStoreProcess;
    readonly TestCommandContext _commands;
    readonly TestQueryContext _queries;

    public TestTimeline(EventStoreProcess eventStoreProcess, TestCommandContext commands, TestQueryContext queries)
    {
      _eventStoreProcess = eventStoreProcess;
      _commands = commands;
      _queries = queries;
    }

    protected override Task Open()
    {
      Track(_eventStoreProcess);

      return base.Open();
    }

    void IDisposable.Dispose()
    {
      // If the host fails to start, TestLifetime will not get an opportunity to
      // shut down gracefully. This ensures the process isn't orphaned.

      if(_eventStoreProcess.State.IsConnected)
      {
        _eventStoreProcess.Disconnect().Wait();
      }
    }

    internal Task<TimelinePosition> Append(Event e) =>
      _commands.Append(e);

    internal Task<TEvent> Expect<TEvent>(ExpectTimeout timeout, bool scheduled) where TEvent : Event =>
      _commands.Expect<TEvent>(timeout, scheduled);

    internal Task<TQuery> Get<TQuery>(Id id) where TQuery : Query =>
      _queries.Get<TQuery>(id);

    internal Task<TQuery> GetAfter<TQuery>(Id id, Event e) where TQuery : Query =>
      _queries.GetAfter<TQuery>(id, e, this);
  }
}