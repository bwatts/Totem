using System.Collections.Concurrent;

namespace Totem.Timeline.Client
{
  /// <summary>
  /// A persistent connection notified of changed queries
  /// </summary>
  internal sealed class QueryConnection
  {
    readonly ConcurrentDictionary<FlowKey, QueryInstance> _instancesByKey = new ConcurrentDictionary<FlowKey, QueryInstance>();
    readonly QueryHost _db;

    internal QueryConnection(Id id, QueryHost db)
    {
      Id = id;
      _db = db;
    }

    internal readonly Id Id;

    internal void Subscribe(QueryInstance instance)
    {
      _instancesByKey.TryAdd(instance.Key, instance);

      instance.Subscribe(this);
    }

    internal void Unsubscribe(FlowKey key)
    {
      if(_instancesByKey.TryGetValue(key, out var instance))
      {
        instance.Unsubscribe(this);

        _instancesByKey.TryRemove(key, out _);

        if(_instancesByKey.Count == 0)
        {
          _db.RemoveConnection(this);
        }
      }
    }

    internal void Unsubscribe()
    {
      foreach(var instance in _instancesByKey.Values)
      {
        instance.Unsubscribe(this);
      }

      _db.RemoveConnection(this);
    }
  }
}