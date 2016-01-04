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
	/// Registers <see cref="FlowEvent"/> instances for a specific flow type
	/// </summary>
	internal sealed class RuntimeReaderFlow : Notion
	{
		private sealed class MethodLookup<TMethod> : Dictionary<EventType, FlowMethodSet<TMethod>> where TMethod : FlowMethod
		{}

		private readonly HashSet<EventType> _events = new HashSet<EventType>();
		private readonly MethodLookup<FlowGiven> _givenLookup = new MethodLookup<FlowGiven>();
		private readonly MethodLookup<FlowWhen> _whenLookup = new MethodLookup<FlowWhen>();
    private readonly Dictionary<EventType, FlowRoute> _routesByEvent = new Dictionary<EventType, FlowRoute>();
    private readonly RuntimeMap _map;
    private readonly FlowType _flow;

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

    private void RegisterMethods()
    {
      foreach(var method in
        from sourceType in _flow.DeclaredType.GetInheritanceChainTo<Flow>(includeType: true, includeTargetType: false)
        from method in sourceType.GetMethods(
          BindingFlags.Public
          | BindingFlags.NonPublic
          | BindingFlags.Static
          | BindingFlags.Instance
          | BindingFlags.DeclaredOnly)
        select method)
      {
        switch(method.Name)
        {
          case "Route":
            TryRegisterMethod(method, e => RegisterRoute(method, e), expectStatic: true);
            break;
          case "Given":
            TryRegisterMethod(method, e => RegisterGiven(method, e));
            break;
          case "GivenScheduled":
            TryRegisterMethod(method, e => RegisterGiven(method, e, scheduled: true));
            break;
          case "When":
            TryRegisterMethod(method, e => RegisterWhen(method, e));
            break;
          case "WhenScheduled":
            TryRegisterMethod(method, e => RegisterWhen(method, e, scheduled: true));
            break;
          default:
            WarnIfPossiblyMisspelled(method);
            break;
        }
      }
    }

    private void TryRegisterMethod(MethodInfo method, Action<EventType> register, bool expectStatic = false)
    {
      if(expectStatic && !method.IsStatic)
      {
        Log.Warning("[runtime] Flow method is not static: {Type}.{Name}", method.DeclaringType.FullName, method.Name);
      }
      else if(!expectStatic && method.IsStatic)
      {
        Log.Warning("[runtime] Flow method is static: {Type}.{Name}", method.DeclaringType.FullName, method.Name);
      }
      else if(!method.IsPrivate)
      {
        Log.Warning("[runtime] Flow method is not private: {Type}.{Name}", method.DeclaringType.FullName, method.Name);
      }
      else
      {
        EventType e;

        if(TryReadEvent(method, out e))
        {
          register(e);
        }
      }
    }

    private bool TryReadEvent(MethodInfo method, out EventType e)
    {
      e = null;

      var firstParameter = method.GetParameters().FirstOrDefault();
      var firstParameterType = firstParameter?.ParameterType;

      if(firstParameterType == null || !typeof(Event).IsAssignableFrom(firstParameterType))
      {
        Log.Warning("[runtime] Flow method {Type}.{Name} does not have an event as the first parameter", method.DeclaringType.FullName, method.Name);
      }
      else
      {
        e = _map.GetEvent(firstParameterType, strict: false);

        if(e != null)
        {
          _events.Add(e);
        }
        else
        {
          Log.Warning(
            "[runtime] Flow method {Type}.{Name} first parameter {Parameter} has event type {Event} that is not in the map",
            method.DeclaringType.FullName,
            method.Name,
            firstParameter.Name,
            firstParameterType);
        }
      }

      return e != null;
    }

    private void RegisterRoute(MethodInfo method, EventType e)
    {
      // Take only the first route method we see for a type - RegisterMethods calls in order of most-to-least derived

      if(!_routesByEvent.ContainsKey(e))
      {
        TryRegisterRoute(method, e);
      }
    }

    private void TryRegisterRoute(MethodInfo method, EventType e)
    {
      if(_flow.IsRequest)
      {
        Log.Warning("[timeline] {Type} is a request andcannot have Route methods", method.DeclaringType.FullName);
      }
      else if(method.ReturnType == typeof(Id) || typeof(IEnumerable<Id>).IsAssignableFrom(method.ReturnType))
      {
        _routesByEvent.Add(e, new FlowRoute(method, e));
      }
      else
      {
        Log.Warning(
          "[timeline] Route method {Type}.{Name}({Event}) does not return one or many Id values",
          method.DeclaringType.FullName,
          method.Name,
          e);
      }
    }

    private void RegisterGiven(MethodInfo method, EventType e, bool scheduled = false)
    {
      RegisterMethod(e, _givenLookup, new FlowGiven(method, e), scheduled);
    }

    private void RegisterWhen(MethodInfo method, EventType e, bool scheduled = false)
    {
      RegisterMethod(e, _whenLookup, new FlowWhen(method, e, ReadWhenDependencies(method)), scheduled);
    }

    private static void RegisterMethod<T>(EventType e, MethodLookup<T> lookup, T method, bool scheduled) where T : FlowMethod
    {
      FlowMethodSet<T> methods;

      if(!lookup.TryGetValue(e, out methods))
      {
        methods = new FlowMethodSet<T>();

        lookup.Add(e, methods);
      }

      if(scheduled)
      {
        methods.ScheduledMethods.Write.Add(method);
      }
      else
      {
        methods.Methods.Write.Add(method);
      }
    }

    private static Many<WhenDependency> ReadWhenDependencies(MethodInfo method)
    {
      return method.GetParameters().Skip(1).Select(ReadWhenDependency).ToMany();
    }

    private static WhenDependency ReadWhenDependency(ParameterInfo parameter)
    {
      var attribute = parameter.GetCustomAttribute<WhenDependencyAttribute>(inherit: true);

      return attribute != null
        ? attribute.GetDependency(parameter)
        : WhenDependency.Typed(parameter);
    }

    private void WarnIfPossiblyMisspelled(MethodInfo method)
		{
      if(method.DeclaringType == typeof(Topic)
        || method.DeclaringType == typeof(Query)
        || method.DeclaringType == typeof(View))
      {
        return;
      }

      foreach(var knownName in Many.Of("Route", "Given", "GivenScheduled", "When", "WhenScheduled"))
      {
        if(Text.EditDistance(method.Name, knownName) <= 2)
        {
          Log.Warning("[runtime] Flow method '{Method}' possibly misspelled: {Type}.{Name}", knownName, method.DeclaringType.FullName, method.Name);

          break;
        }
      }
		}

		private void RegisterEvents()
		{
      var unroutedEvents = new List<EventType>();

			foreach(var e in _events)
			{
				FlowMethodSet<FlowGiven> given;
				FlowMethodSet<FlowWhen> when;
        FlowRoute route;

				if(!_givenLookup.TryGetValue(e, out given))
				{
					given = new FlowMethodSet<FlowGiven>();
				}

				if(!_whenLookup.TryGetValue(e, out when))
				{
					when = new FlowMethodSet<FlowWhen>();
				}

        if(!_routesByEvent.TryGetValue(e, out route))
        {
          unroutedEvents.Add(e);
        }

        _flow.Events.Register(new FlowEvent(_flow, e, given, when, route));
			}

      if(unroutedEvents.Count > 0 && unroutedEvents.Count < _events.Count)
      {
        throw new Exception($"Flow {_flow} specifies routes but is missing some: {unroutedEvents.ToTextSeparatedBy(", ")}");
      }

      if(_flow.IsRequest || (_events.Count > 0 && unroutedEvents.Count == 0))
      {
        _flow.SetRouted();
      }
      else
      {
        _flow.SetSingleInstance();
      }
    }
	}
}