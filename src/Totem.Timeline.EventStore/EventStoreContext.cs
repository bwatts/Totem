using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Totem.Runtime;
using Totem.Runtime.Json;
using Totem.Threading;
using Totem.Timeline.Area;

namespace Totem.Timeline.EventStore
{
  /// <summary>
  /// The connection, JSON format, and area map in effect for the EventStore timeline
  /// </summary>
  public class EventStoreContext : Connection
  {
    TaskSource _connectInitially;

    public EventStoreContext(IEventStoreConnection connection, IJsonFormat json, AreaMap area)
    {
      Connection = connection;
      Json = json;
      Area = area;
    }

    public readonly IEventStoreConnection Connection;
    public readonly IJsonFormat Json;
    public readonly AreaMap Area;

    protected override Task Open()
    {
      ObserveConnected();
      ObserveDisconnected();
      ObserveReconnecting();
      ObserveClosed();
      ObserveErrorOccurred();
      ObserveAuthenticationFailed();

      return ConnectInitially();
    }

    protected override Task Close()
    {
      Connection.Close();

      return base.Close();
    }

    void ObserveConnected() =>
      Observe<ClientConnectionEventArgs>(
        e => Connection.Connected += e,
        e => Connection.Connected -= e,
        args =>
        {
          Log.Info("Connected to EventStore at {EndPoint}", args.RemoteEndPoint);

          _connectInitially?.SetResult();
        });

    void ObserveDisconnected() =>
      Observe<ClientConnectionEventArgs>(
        e => Connection.Disconnected += e,
        e => Connection.Disconnected -= e,
        args => Log.Info("Disconnected from EventStore"));

    void ObserveReconnecting() =>
      Observe<ClientReconnectingEventArgs>(
        e => Connection.Reconnecting += e,
        e => Connection.Reconnecting -= e,
        args => Log.Info("Reconnecting to EventStore..."));

    void ObserveClosed() =>
      Observe<ClientClosedEventArgs>(
        e => Connection.Closed += e,
        e => Connection.Closed -= e,
        args =>
        {
          Log.Info("EventStore connection closed ({Reason})", args.Reason);

          _connectInitially?.SetException(new Exception("EventStore connection closed while connecting initially"));
        });

    void ObserveErrorOccurred() =>
      Observe<ClientErrorEventArgs>(
        e => Connection.ErrorOccurred += e,
        e => Connection.ErrorOccurred -= e,
        args =>
        {
          Log.Error(args.Exception, "EventStore connection error occurred");

          _connectInitially?.SetException(args.Exception);
        });

    void ObserveAuthenticationFailed() =>
      Observe<ClientAuthenticationFailedEventArgs>(
        e => Connection.AuthenticationFailed += e,
        e => Connection.AuthenticationFailed -= e,
        args =>
        {
          Log.Error("EventStore connection failed to authenticate ({Reason})", args.Reason);

          _connectInitially?.SetException(new Exception($"EventStore connection failed to authenticate when connecting initially ({args.Reason})"));
        });

    void Observe<TArgs>(
      Action<EventHandler<TArgs>> add,
      Action<EventHandler<TArgs>> remove,
      Action<TArgs> onNext)
    {
      Track(Observable
        .FromEventPattern(add, remove)
        .Select(e => e.EventArgs)
        .Subscribe(onNext));
    }

    async Task ConnectInitially()
    {
      _connectInitially = new TaskSource();

      try
      {
        await Connection.ConnectAsync();

        await _connectInitially.Task;
      }
      finally
      {
        _connectInitially = null;
      }
    }
  }
}