using System;
using System.Collections.Concurrent;
using System.Linq;
using Totem.Runtime;
using Totem.Timeline.Client;

namespace Totem.Timeline.Client
{
  /// <summary>
  /// A 5-second window of changed query notifications
  /// </summary>
  /// <remarks>
  /// A client gets ahold of an ETag initially by sending a GET to an API. It then turns around
  /// and subscribes to changes via SignalR; there is a window of time in which the version
  /// they just received is stale, but they haven't yet subscribed, missing any intermediate changes.
  ///
  /// This window ensures a connection subscribing with a stale ETag will receive a change notification.
  /// </remarks>
  internal sealed class QueryChangedWindow : Notion
  {
    static readonly TimeSpan _trimDelay = TimeSpan.FromSeconds(5);
    readonly ConcurrentDictionary<FlowKey, QueryWindow> _queriesByKey = new ConcurrentDictionary<FlowKey, QueryWindow>();
    DateTimeOffset _whenTrimmed;

    internal QueryChangedWindow()
    {
      _whenTrimmed = Clock.Now;
    }

    internal QueryETag OnSubscribed(QueryETag etag) =>
      _queriesByKey
      .GetOrAdd(etag.Key, _ => new QueryWindow(etag, Clock.Now))
      .GetLatest(etag);

    internal void OnChanged(QueryETag etag)
    {
      var now = Clock.Now;

      _queriesByKey[etag.Key] = new QueryWindow(etag, now);

      if(now - _whenTrimmed > _trimDelay)
      {
        TrimQueries(now);
      }
    }

    void TrimQueries(DateTimeOffset now)
    {
      _whenTrimmed = now;

      var trimmedKeys =
        from pair in _queriesByKey
        where pair.Value.CanTrim(now)
        select pair.Key;

      foreach(var trimmedKey in trimmedKeys.ToList())
      {
        _queriesByKey.TryRemove(trimmedKey, out var _);
      }
    }

    class QueryWindow
    {
      readonly QueryETag _etag;
      readonly DateTimeOffset _whenChanged;

      internal QueryWindow(QueryETag etag, DateTimeOffset whenChanged)
      {
        _etag = etag;
        _whenChanged = whenChanged;
      }

      internal QueryETag GetLatest(QueryETag etag) =>
        _etag.GetLatest(etag);

      internal bool CanTrim(DateTimeOffset now) =>
        now - _whenChanged > _trimDelay;
    }
  }
}