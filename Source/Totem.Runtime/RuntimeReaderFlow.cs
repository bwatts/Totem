using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Totem.Reflection;
using Totem.Runtime.Map;
using Totem.Runtime.Map.Timeline;
using Totem.Runtime.Timeline;

namespace Totem.Runtime
{
	/// <summary>
	/// Registers methods and events for a specific flow type
	/// </summary>
	internal sealed class RuntimeReaderFlow : Notion
	{
		readonly HashSet<EventType> _events = new HashSet<EventType>();
    readonly Dictionary<EventType, RouteMethod> _routesByEvent = new Dictionary<EventType, RouteMethod>();
		readonly Dictionary<EventType, FlowMethodSet<WhenMethod>> _whensByEvent = new Dictionary<EventType, FlowMethodSet<WhenMethod>>();
		readonly Dictionary<EventType, FlowMethodSet<GivenMethod>> _givensByEvent = new Dictionary<EventType, FlowMethodSet<GivenMethod>>();
    readonly RuntimeMap _map;
    readonly FlowType _flow;
		MethodInfo _method;
		EventType _event;

		internal RuntimeReaderFlow(RuntimeMap map, FlowType flow)
		{
			_map = map;
			_flow = flow;
		}

		internal void Register()
		{
      RegisterMethods();

      RegisterEvents();
		}

    void RegisterMethods()
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
				_method = method;

        switch(method.Name)
        {
          case "Route":
            TryRegisterMethod(() => RegisterRoute(), expectStatic: true);
            break;
					case "RouteFirst":
						TryRegisterMethod(() => RegisterRoute(isFirst: true), expectStatic: true);
						break;
					case "When":
						TryRegisterMethod(() => RegisterWhen());
						break;
					case "WhenScheduled":
						TryRegisterMethod(() => RegisterWhen(scheduled: true));
						break;
					case "Given":
            TryRegisterMethod(() => RegisterGiven());
            break;
          case "GivenScheduled":
            TryRegisterMethod(() => RegisterGiven(scheduled: true));
            break;
          default:
            WarnIfMethodPossiblyMisspelled();
            break;
        }
      }
    }

    void TryRegisterMethod(Action register, bool expectStatic = false)
    {
      if(expectStatic && !_method.IsStatic)
      {
        Log.Warning("[runtime] Flow method is not static: {Type}.{Method}", _method.DeclaringType.FullName, _method.Name);
      }
      else if(!expectStatic && _method.IsStatic)
      {
        Log.Warning("[runtime] Flow method is static: {Type}.{Method}", _method.DeclaringType.FullName, _method.Name);
      }
      else if(!_method.IsPrivate)
      {
        Log.Warning("[runtime] Flow method is not private: {Type}.{Method}", _method.DeclaringType.FullName, _method.Name);
      }
      else
      {
        if(TryReadEvent())
        {
          register();
        }
      }
    }

    bool TryReadEvent()
    {
      var firstParameter = _method.GetParameters().FirstOrDefault();
      var firstParameterType = firstParameter?.ParameterType;

      if(firstParameterType == null || !typeof(Event).IsAssignableFrom(firstParameterType))
      {
        Log.Warning("[runtime] {Type}.{Method} does not have an event as the first parameter", _method.DeclaringType.FullName, _method.Name);
      }
      else
      {
				_event = _map.GetEvent(firstParameterType, strict: false);

        if(_event != null)
        {
          _events.Add(_event);
        }
        else
        {
          Log.Warning(
						"[runtime] Flow method {Type}.{Method} first parameter {Parameter} has event type {Event} that is not in the map",
						_method.DeclaringType.FullName,
						_method.Name,
            firstParameter.Name,
            firstParameterType);
        }
      }

      return _event != null;
    }

    void RegisterRoute(bool isFirst = false)
    {
      // Take only the first route method we see for a type - RegisterMethods calls in order of most-to-least derived

      if(!_routesByEvent.ContainsKey(_event))
      {
        TryRegisterRoute(isFirst);
      }
    }

    void TryRegisterRoute(bool isFirst)
    {
      if(_flow.IsRequest)
      {
        Log.Warning("[timeline] {Type} is a request and cannot have {Method} methods", _method.DeclaringType.FullName, _method.Name);
      }
      else if(_method.ReturnType == typeof(Id) || typeof(IEnumerable<Id>).IsAssignableFrom(_method.ReturnType))
      {
				_routesByEvent.Add(_event, new RouteMethod(_method, _event, _flow, isFirst));
      }
      else
      {
        Log.Warning(
          "[timeline] {Type}.{Method}({Event}) does not return one or many Id values",
					_method.DeclaringType.FullName,
					_method.Name,
          _event);
      }
    }

    void RegisterWhen(bool scheduled = false)
    {
			RegisterMethods(_whensByEvent, new WhenMethod(_method, _event, ReadWhenDependencies()), scheduled);
    }

		void RegisterGiven(bool scheduled = false)
		{
			if(!_flow.IsTopic)
			{
				Log.Warning("[timeline] {Type} is not a topic and cannot have {Method} methods", _method.DeclaringType.FullName, _method.Name);
			}
			else if(_method.GetParameters().Length > 1)
			{
				Log.Warning("[timeline] {Type}.{Method}({Event}, ...) is solely for state and cannot have dependencies", _method.DeclaringType.FullName, _method.Name, _event);
			}
			else
			{
				RegisterMethods(_givensByEvent, new GivenMethod(_method, _event), scheduled);
			}
		}

		void RegisterMethods<T>(Dictionary<EventType, FlowMethodSet<T>> methodsByEvent, T method, bool scheduled) where T : FlowMethod
    {
			FlowMethodSet<T> methods;

      if(!methodsByEvent.TryGetValue(_event, out methods))
      {
				methods = new FlowMethodSet<T>();

        methodsByEvent.Add(_event, methods);
      }

			methods.SelectMethods(scheduled).Write.Add(method);
    }

    Many<Dependency> ReadWhenDependencies()
    {
			return _method.GetParameters().Skip(1).ToMany(parameter =>
			{
				var attribute = parameter.GetCustomAttribute<DependencyAttribute>(inherit: true);

				return attribute != null
					? attribute.GetDependency(parameter)
					: Dependency.Typed(parameter);
			});
    }

		void WarnIfMethodPossiblyMisspelled()
		{
			if(_method.DeclaringType == typeof(Topic) || _method.DeclaringType == typeof(View))
			{
				return;
			}

			foreach(var knownMethodName in GetKnownMethodNames())
			{
				if(Text.EditDistance(_method.Name, knownMethodName) <= 2)
				{
					Log.Warning("[runtime] Flow method \"{Method}\" possibly misspelled: {Type}.{Method}", knownMethodName, _method.DeclaringType.FullName, _method.Name);

					break;
				}
			}
		}

		IEnumerable<string> GetKnownMethodNames()
		{
			yield return "When";
			yield return "WhenScheduled";

			if(!_flow.IsRequest)
			{
				yield return "Route";
				yield return "RouteFirst";
			}

			if(_flow.IsTopic)
			{
				yield return "Given";
				yield return "GivenScheduled";
			}
		}

		//
		// Events
		//

		void RegisterEvents()
		{
			var unrouted = RouteEvents().ToMany();

			ExpectNoneOrAllUnrouted(unrouted);

			SetRoutedOrSingleInstance(unrouted);
    }

		IEnumerable<EventType> RouteEvents()
		{
			foreach(var e in _events)
			{
				_event = e;

				if(!RouteEvent())
				{
					yield return e;
				}
			}
		}

		bool RouteEvent()
		{
			RouteMethod route;
			FlowMethodSet<WhenMethod> when;

			var hasRoute = _routesByEvent.TryGetValue(_event, out route);

			if(!_whensByEvent.TryGetValue(_event, out when))
			{
				when = new FlowMethodSet<WhenMethod>();
			}

			FlowEvent flowEvent;

			if(_flow.IsTopic)
			{
				FlowMethodSet<GivenMethod> given;

				if(!_givensByEvent.TryGetValue(_event, out given))
				{
					given = new FlowMethodSet<GivenMethod>();
				}

				flowEvent = new TopicEvent(_flow, _event, route, when, given);
			}
			else
			{
				flowEvent = new FlowEvent(_flow, _event, route, when);
			}

			_flow.Events.Register(flowEvent);

			_event.RegisterFlow(flowEvent);

			return hasRoute;
		}

		void ExpectNoneOrAllUnrouted(Many<EventType> unrouted)
		{
			if(unrouted.Count > 0 && unrouted.Count < _events.Count)
			{
				throw new Exception($"Flow {_flow} specifies routes but is missing some: {unrouted.ToTextSeparatedBy(", ")}");
			}
		}

    void ExpectRouteFirst()
    {
      if(!_flow.Events.Any(e => e.Route.First))
      {
        throw new Exception($"Flow {_flow} specifies routes but no RouteFirst methods");
      }
    }

    void SetRoutedOrSingleInstance(Many<EventType> unrouted)
		{
			if(_flow.IsRequest)
			{
				_flow.SetRouted();
			}
			else if(_events.Count > 0 && unrouted.Count == 0)
			{
				ExpectRouteFirst();

				_flow.SetRouted();
			}
			else
			{
				_flow.SetSingleInstance();
			}
		}
	}
}