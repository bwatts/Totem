using System.Collections.Concurrent;
using System.Threading.Tasks;
using Totem.Timeline;
using Totem.Timeline.Area;

namespace Totem.App.Tests.Hosting
{
  /// <summary>
  /// An application applying a test to instances of a query type
  /// </summary>
  internal sealed class QueryApp
  {
    readonly ConcurrentDictionary<Id, QueryInstance> _instancesById = new ConcurrentDictionary<Id, QueryInstance>();
    readonly QueryAppTimelineDb _timelineDb;
    readonly QueryType _queryType;

    public QueryApp(QueryAppTimelineDb timelineDb, QueryType queryType)
    {
      _timelineDb = timelineDb;
      _queryType = queryType;

      timelineDb.SubscribeApp(this);
    }

    internal async Task Append(Event e)
    {
      var point = await _timelineDb.WriteFromApp(e);

      foreach(var route in point.Routes)
      {
        if(route.Type == _queryType)
        {
          GetInstance(route.Id).OnAppended(point);
        }
      }
    }

    internal Task<TQuery> GetQuery<TQuery>(Id instanceId, ExpectTimeout timeout) where TQuery : Query =>
      GetInstance(instanceId).Get<TQuery>(timeout);

    internal Task ExpectDone(Id instanceId, ExpectTimeout timeout) =>
      GetInstance(instanceId).ExpectDone(timeout);

    internal void OnCheckpoint(Query query) =>
      GetInstance(query.Id).OnCheckpoint(query);

    QueryInstance GetInstance(Id id)
    {
      _queryType.ExpectIdMatchesCardinality(id);

      return _instancesById.GetOrAdd(id, _ => new QueryInstance(_queryType.CreateKey(id)));
    }
  }
}