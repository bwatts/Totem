using System;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Totem.Runtime;
using Totem.Timeline.Client;
using Totem.Timeline.Runtime;

namespace Totem.Timeline.EventStore.Client
{
  /// <summary>
  /// The EventStore database through which a timeline client executes commands
  /// </summary>
  public sealed class CommandDb : Connection, ICommandDb
  {
    readonly EventStoreContext _context;

    public CommandDb(EventStoreContext context)
    {
      _context = context;
    }

    protected override Task Open()
    {
      Track(_context);

      return base.Open();
    }

    public async Task<IDisposable> Subscribe(ITimelineObserver observer) =>
      await _context.Connection.SubscribeToStreamAsync(
        _context.Area.ToString(),
        resolveLinkTos: false,
        eventAppeared: (_, e) =>
          observer.OnNext(_context.ReadAreaPoint(e)),
        subscriptionDropped: (_, reason, error) =>
          observer.OnDropped(reason.ToString(), error));

    public Task WriteEvent(Event e)
    {
      var stream = _context.Area.ToString();

      var type = _context.GetEventType(e);

      var data = _context.GetAreaEventData(
        e,
        TimelinePosition.None,
        e.When,
        Event.Traits.WhenOccurs.Get(e),
        Event.Traits.CommandId.Get(e),
        Event.Traits.UserId.Get(e),
        null,
        type.GetRoutes(e).ToMany());

      return _context.Connection.AppendToStreamAsync(stream, ExpectedVersion.Any, data);
    }
  }
}