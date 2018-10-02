using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Runtime;
using Totem.Timeline.Area;

namespace Totem.Timeline.EventStore
{
  /// <summary>
  /// Data describing an event appended to an area timeline
  /// </summary>
  [Durable]
  public class AreaEventMetadata
  {
    public TimelinePosition Cause;
    public DateTimeOffset When;
    public DateTimeOffset? WhenOccurs;
    public Id CommandId;
    public Id UserId;
    public FlowKey Topic;
    public List<MapTypeKey> RouteTypes;
    public List<RouteTypeIds> RouteIds;

    public class RouteTypeIds
    {
      public int Type;
      public List<Id> Ids;
    }

    public void ApplyRoutes(EventType eventType, IEnumerable<FlowKey> routes)
    {
      var flows =
        from route in routes
        group route.Id by route.Type into idsByType
        let type = idsByType.Key
        select new
        {
          Type = type.Key,
          Ids = type.IsSingleInstance ? null : idsByType.ToList()
        };

      RouteTypes = new List<MapTypeKey>();
      RouteIds = new List<RouteTypeIds>();

      var typeIndex = 0;

      foreach(var flow in flows)
      {
        RouteTypes.Add(flow.Type);

        if(flow.Ids != null)
        {
          RouteIds.Add(new RouteTypeIds { Type = typeIndex, Ids = flow.Ids });
        }

        typeIndex++;
      }
    }

    public IEnumerable<FlowKey> GetRoutes(AreaMap area)
    {
      var multiInstanceTypes = RouteTypes.ToHashSet();

      foreach(var typeIds in RouteIds)
      {
        var type = RouteTypes[typeIds.Type];

        multiInstanceTypes.Remove(type);

        var areaType = area.Flows.Get(type);

        foreach(var id in typeIds.Ids)
        {
          yield return FlowKey.From(areaType, id);
        }
      }

      foreach(var type in multiInstanceTypes)
      {
        yield return FlowKey.From(area.Flows.Get(type));
      }
    }
  }
}