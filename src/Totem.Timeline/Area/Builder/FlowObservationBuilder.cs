using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Totem.Reflection;
using Totem.Runtime;

namespace Totem.Timeline.Area.Builder
{
  /// <summary>
  /// Builds the observations of a flow in a timeline area
  /// </summary>
  /// <remarks>
  /// This stateful builder finds the Route, When, and Given methods defined by tye flow type.
  /// 
  /// During iteration, each method/event pair are stored in the <see cref="_method"/> and
  /// <see cref="_event"/> fields, respectively, allowing universal access without passing
  /// them throughout the call chain.
  /// 
  /// After finding those methods, it associates each of the <see cref="FlowType"/> and
  /// <see cref="EventType"/> pairs.
  /// </remarks>
  internal sealed class FlowObservationBuilder : Notion
  {
    readonly HashSet<EventType> _events = new HashSet<EventType>();
    readonly Dictionary<EventType, FlowRoute> _routesByEvent = new Dictionary<EventType, FlowRoute>();
    readonly Dictionary<EventType, FlowMethodSet<FlowGiven>> _givensByEvent = new Dictionary<EventType, FlowMethodSet<FlowGiven>>();
    readonly Dictionary<EventType, FlowMethodSet<TopicWhen>> _whensByEvent = new Dictionary<EventType, FlowMethodSet<TopicWhen>>();
    readonly AreaMapBuilder _map;
    readonly FlowType _flow;
    MethodInfo _method;
    string _methodPath;
    EventType _event;

    internal FlowObservationBuilder(AreaMapBuilder map, FlowType flow)
    {
      _map = map;
      _flow = flow;
    }

    internal void Build()
    {
      DeclareMethods();
      DeclareObservations();
    }

    //
    // Methods
    //

    void DeclareMethods()
    {
      foreach(var method in
        from sourceType in _flow.DeclaredType.GetInheritanceChainTo<Flow>(includeType: true, includeTargetType: false)
        from method in sourceType.GetMethods(
          BindingFlags.Public
          | BindingFlags.NonPublic
          | BindingFlags.Static
          | BindingFlags.Instance
          | BindingFlags.DeclaredOnly)
        orderby method.Name.StartsWith("Route")
        select method)
      {
        _methodPath = $"{method.DeclaringType.FullName}.{method.Name}";

        _method = method;

        DeclareMethod();
      }
    }

    void DeclareMethod()
    {
      switch(_method.Name)
      {
        case "Route":
          TryDeclareMethod(() => DeclareRoute(), expectStatic: true);
          break;
        case "RouteFirst":
          TryDeclareMethod(() => DeclareRoute(isFirst: true), expectStatic: true);
          break;
        case "Given":
          TryDeclareMethod(() => DeclareGiven());
          break;
        case "GivenScheduled":
          TryDeclareMethod(() => DeclareGiven(scheduled: true));
          break;
        case "When":
          TryDeclareMethod(() => DeclareWhen());
          break;
        case "WhenScheduled":
          TryDeclareMethod(() => DeclareWhen(scheduled: true));
          break;
        default:
          WarnIfMethodPossiblyMisspelled();
          break;
      }
    }

    void TryDeclareMethod(Action declare, bool expectStatic = false)
    {
      if(expectStatic && !_method.IsStatic)
      {
        Log.Warning($"[runtime] Flow method {_methodPath} is not static; ignoring");
      }
      else if(!expectStatic && _method.IsStatic)
      {
        Log.Warning($"[runtime] Flow method {_methodPath} is static; ignoring");
      }
      else if(!_method.IsPrivate)
      {
        Log.Warning($"[runtime] Flow method {_methodPath} is not private; ignoring");
      }
      else
      {
        if(TryReadEvent())
        {
          declare();
        }
      }
    }

    bool TryReadEvent()
    {
      var firstParameter = _method.GetParameters().FirstOrDefault();
      var firstParameterType = firstParameter?.ParameterType;

      if(firstParameterType == null || !typeof(Event).IsAssignableFrom(firstParameterType))
      {
        Log.Warning($"[runtime] Flow method {_methodPath} does not have an event as the first parameter; ignoring");

        return false;
      }
      else if(!_map.TryGetEvent(firstParameterType, out _event))
      {
        Log.Warning($"[runtime] Flow method {_methodPath} refers to an event that is not in the map; ignoring");

        return false;
      }
      else
      {
        _events.Add(_event);

        return true;
      }
    }

    void DeclareRoute(bool isFirst = false)
    {
      // Take only the first route method we see for a type - DeclareMethods calls in order of most-to-least derived

      if(!_routesByEvent.ContainsKey(_event))
      {
        if(_method.ReturnType == typeof(Id) || typeof(IEnumerable<Id>).IsAssignableFrom(_method.ReturnType))
        {
          _routesByEvent.Add(_event, new FlowRoute(_method, _event, _flow, isFirst));
        }
        else
        {
          Log.Warning($"[runtime] {_methodPath}({_event}) does not return one or many Id values; ignoring");
        }
      }
    }

    void DeclareGiven(bool scheduled = false)
    {
      if(_method.GetParameters().Length == 1)
      {
        DeclareMethods(_givensByEvent, new FlowGiven(_method, _event), scheduled);
      }
      else
      {
        Log.Warning($"[runtime] {_methodPath}({_event}, ...) is solely for state and cannot have dependencies; ignoring");
      }
    }

    void DeclareWhen(bool scheduled = false)
    {
      if(_flow.IsTopic)
      {
        DeclareMethods(_whensByEvent, new TopicWhen(_method, _event, ReadWhenDependencies()), scheduled);
      }
      else
      {
        Log.Warning($"[runtime] {_method.DeclaringType.FullName} is not a topic and cannot have {_method.Name} methods; ignoring");
      }
    }

    void DeclareMethods<T>(Dictionary<EventType, FlowMethodSet<T>> methodsByEvent, T method, bool scheduled) where T : FlowMethod
    {
      if(!methodsByEvent.TryGetValue(_event, out var methods))
      {
        methods = new FlowMethodSet<T>();

        methodsByEvent.Add(_event, methods);
      }

      methods.GetMethods(scheduled).Write.Add(method);
    }

    Many<TopicWhenDependency> ReadWhenDependencies() =>
      _method.GetParameters().Skip(1).ToMany(parameter => new TopicWhenDependency(parameter));

    void WarnIfMethodPossiblyMisspelled()
    {
      if(_method.DeclaringType != typeof(Topic) && _method.DeclaringType != typeof(Query))
      {
        foreach(var knownMethodName in GetKnownMethodNames())
        {
          if(Text.EditDistance(_method.Name, knownMethodName) <= 2)
          {
            Log.Warning($"[runtime] Flow method \"{knownMethodName}\" possibly misspelled: {_methodPath}");

            break;
          }
        }
      }
    }

    IEnumerable<string> GetKnownMethodNames()
    {
      yield return "Route";
      yield return "RouteFirst";

      yield return "When";
      yield return "WhenScheduled";

      if(_flow.IsTopic)
      {
        yield return "Given";
        yield return "GivenScheduled";
      }
    }

    //
    // Observations
    //

    void DeclareObservations()
    {
      var unobserved = ObserveEvents().ToMany();

      ExpectNoneOrAllUnobserved(unobserved);

      SetCardinality(unobserved);
    }

    IEnumerable<EventType> ObserveEvents()
    {
      foreach(var e in _events)
      {
        _event = e;

        if(!TryRouteEvent())
        {
          yield return e;
        }
      }
    }

    bool TryRouteEvent()
    {
      var hasRoute = _routesByEvent.TryGetValue(_event, out var route);

      if(!_givensByEvent.TryGetValue(_event, out var given))
      {
        given = new FlowMethodSet<FlowGiven>();
      }

      FlowObservation observation;

      if(!_flow.IsTopic)
      {
        observation = new FlowObservation(_flow, _event, route, given);
      }
      else
      {
        if(!_whensByEvent.TryGetValue(_event, out var when))
        {
          when = new FlowMethodSet<TopicWhen>();
        }

        observation = new TopicObservation(_flow, _event, route, given, when);
      }

      _flow.Observations.Declare(observation);
      _event.Observations.Write.Add(observation);

      return hasRoute;
    }

    void SetCardinality(Many<EventType> unobserved)
    {
      if(_events.Count > 0 && unobserved.Count == 0)
      {
        ExpectRouteFirst();

        _flow.IsMultiInstance = true;
      }
      else
      {
        _flow.IsSingleInstance = true;
      }
    }

    // These are a bit of a cop-out, as most other issues result in warnings.
    //
    // By this point, we've already poked at the flow and event types enough that
    // we can't cleanly hide our tracks. Mutable state ftw.

    void ExpectNoneOrAllUnobserved(Many<EventType> unobserved) =>
      Expect.True(
        unobserved.Count == 0 || unobserved.Count == _events.Count,
        $"Flow {_flow} specifies routes but is missing some: {unobserved.ToTextSeparatedBy(", ")}");

    void ExpectRouteFirst() =>
      Expect.True(
        _flow.Observations.Any(e => e.Route.First),
        $"Flow {_flow} specifies routes but no RouteFirst methods");
  }
}