using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Totem.Timeline.EventStore.Client
{
  /// <summary>
  /// A query notifying one or more connections when it changes
  /// </summary>
  internal class QueryInstance
  {
    readonly ConcurrentDictionary<Id, bool> _connectionIds = new ConcurrentDictionary<Id, bool>();
    readonly QueryDb _db;

    internal QueryInstance(FlowKey key, QueryDb db)
    {
      Key = key;
      _db = db;
    }

    internal FlowKey Key { get; set; }
    internal IEnumerable<Id> ConnectionIds => _connectionIds.Keys;

    internal void Subscribe(QueryConnection connection) =>
      _connectionIds.TryAdd(connection.Id, false);

    internal void Unsubscribe(QueryConnection connection)
    {
      _connectionIds.TryRemove(connection.Id, out var _);

      if(_connectionIds.Count == 0)
      {
        _db.RemoveInstance(this);
      }
    }
  }
}