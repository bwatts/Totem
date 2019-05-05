using System.IO;
using System.Threading.Tasks;
using Totem.Runtime.Json;
using Totem.Timeline.Area;
using Totem.Timeline.Client;

namespace Totem.Timeline.IntegrationTests.Hosting
{
  /// <summary>
  /// Gets instances of queries, either the latest version or after a specific event
  /// </summary>
  internal sealed class TestQueryContext
  {
    readonly AreaMap _area;
    readonly IJsonFormat _jsonFormat;
    readonly IQueryDb _queryDb;
    readonly TestQueryNotifier _notifier;

    public TestQueryContext(AreaMap area, IJsonFormat jsonFormat, IQueryDb queryDb, TestQueryNotifier notifier)
    {
      _area = area;
      _jsonFormat = jsonFormat;
      _queryDb = queryDb;
      _notifier = notifier;
    }

    internal async Task<TQuery> Get<TQuery>(Id id) where TQuery : Query
    {
      var key = GetKey<TQuery>(id);

      var state = await _queryDb.ReadState(QueryETag.From(key));

      return (TQuery) Deserialize(key.Type, state.ReadContent());
    }

    internal async Task<TQuery> GetAfter<TQuery>(Id id, Event e, TestTimeline timeline) where TQuery : Query
    {
      var key = GetKey<TQuery>(id);

      ExpectObserves(key.Type, e);

      var content = await AppendAndWaitUntilChanged(key, e, timeline);

      return (TQuery) Deserialize(key.Type, content);
    }

    FlowKey GetKey<TQuery>(Id id) =>
      _area.Queries.Get<TQuery>().CreateKey(id);

    Query Deserialize(FlowType type, Stream content) =>
      (Query) _jsonFormat.FromJsonUtf8(content, type.DeclaredType);

    void ExpectObserves(FlowType queryType, Event e)
    {
      var eventType = _area.Events[e.GetType()];

      if(!queryType.Observations.Contains(eventType))
      {
        throw new ExpectException($"Query {queryType} does not observe event {eventType}");
      }
    }

    async Task<Stream> AppendAndWaitUntilChanged(FlowKey key, Event e, TestTimeline timeline)
    {
      var connectionId = Id.FromGuid();

      await _queryDb.SubscribeToChanged(connectionId, QueryETag.From(key));

      var position = await timeline.Append(e);

      var newETag = QueryETag.From(key, position);

      await _notifier.WaitUntilChanged(connectionId, newETag);

      _queryDb.UnsubscribeFromChanged(connectionId);

      return await _queryDb.ReadContent(newETag);
    }
  }
}