using System;

namespace Totem.EventBus
{
  public class SubscriptionInfo
  {
    public string EventName { get; }
    public Type EventType { get; }
    public Type HandlerType { get; }

    public SubscriptionInfo(string eventName, Type eventType, Type handlerType)
    {
      EventName = eventName;
      EventType = eventType;
      HandlerType = handlerType;
    }
  }
}
