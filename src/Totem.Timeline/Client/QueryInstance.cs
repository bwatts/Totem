using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Totem.Timeline.Client
{
  /// <summary>
  /// A query notifying one or more connections when it changes
  /// </summary>
  internal sealed class QueryInstance
  {
    readonly ConcurrentDictionary<Id, bool> _connectionIds = new ConcurrentDictionary<Id, bool>();
    readonly QueryHost _host;

    internal QueryInstance(FlowKey key, QueryHost host)
    {
      Key = key;
      _host = host;
    }

    internal FlowKey Key { get; set; }
    internal IEnumerable<Id> ConnectionIds => _connectionIds.Keys;

    internal void Subscribe(QueryConnection connection) =>
      _connectionIds.TryAdd(connection.Id, false);

    internal void Unsubscribe(QueryConnection connection)
    {
      _connectionIds.TryRemove(connection.Id, out _);

      if(_connectionIds.Count == 0)
      {
        _host.RemoveInstance(this);
      }
    }
  }
}