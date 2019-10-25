using Newtonsoft.Json;
using StreamsDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Totem.EventBus.StreamsDb
{
  public class StreamsDbEventBus : IEventBus, IDisposable
  {
    const string STREAM_NAME = "eventbus";

    private readonly StreamsDBClient _client;
    private readonly Func<Type, IIntegrationEventHandler> _eventHandlerResolver;
    private readonly List<SubscriptionInfo> _subscriptions;

    public StreamsDbEventBus(StreamsDBClient client, Func<Type, IIntegrationEventHandler> eventHandlerResolver)
    {
      _client = client;
      _eventHandlerResolver = eventHandlerResolver;
      _subscriptions = new List<SubscriptionInfo>();
    }

    public void Start()
    {
      var subscription = _client.DB().SubscribeStream(STREAM_NAME, 0);

      Task.Run(async () =>
      {
        do
        {
          await subscription.MoveNext(CancellationToken.None);
          await ProcessEvent(subscription.Current);
        }
        while (true);
      });
    }

    public async Task Publish(IntegrationEvent @event)
    {
      var message = JsonConvert.SerializeObject(@event);
      var body = Encoding.UTF8.GetBytes(message);

      var eventName = @event.GetType().Name;

      var messageInput = new MessageInput
      {
        ID = Guid.NewGuid().ToString(),
        Type = eventName,
        Value = body
      };

      await _client.DB().AppendStream(STREAM_NAME, messageInput);
    }

    public void Subscribe<T, TH>(string eventName)
      where T : IntegrationEvent
      where TH : IIntegrationEventHandler<T>
    {
      _subscriptions.Add(new SubscriptionInfo(eventName, typeof(T), typeof(TH)));
    }

    public void Unsubscribe<T, TH>(string eventName)
      where T : IntegrationEvent
      where TH : IIntegrationEventHandler<T>
    {
      _subscriptions.RemoveAll(x => x.EventName == eventName && x.EventType == typeof(T) && x.HandlerType == typeof(TH));
    }

    public void Dispose()
    {
      _client.Close();
    }

    private async Task ProcessEvent(Message message)
    {
      var eventName = message.Type;

      var body = Encoding.UTF8.GetString(message.Value);

      var subscriptions = _subscriptions.Where(x => x.EventName == eventName);

      foreach (var subscription in subscriptions)
      {
        var handler = _eventHandlerResolver(subscription.HandlerType);

        if (handler == null) continue;

        var integrationEvent = JsonConvert.DeserializeObject(body, subscription.EventType);
        var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(subscription.EventType);
        await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });
      }
    }
  }
}
