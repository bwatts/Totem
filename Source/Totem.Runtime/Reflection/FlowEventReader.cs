using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Totem.Reflection;
using Totem.Runtime.Map;
using Totem.Runtime.Map.Timeline;
using Totem.Runtime.Timeline;

namespace Totem.Runtime.Reflection
{
	/// <summary>
	/// Reads <see cref="FlowEvent"/> instances for a specific flow type
	/// </summary>
	internal sealed class FlowEventReader : Notion
	{
		private sealed class MethodLookup<TMethod> : Dictionary<EventType, FlowMethodSet<TMethod>> where TMethod : FlowMethod
		{}

		private readonly HashSet<EventType> _eventTypes = new HashSet<EventType>();
		private readonly MethodLookup<FlowBefore> _beforeLookup = new MethodLookup<FlowBefore>();
		private readonly MethodLookup<FlowWhen> _whenLookup = new MethodLookup<FlowWhen>();
		private readonly RuntimeMap _map;
		private readonly FlowType _flow;

		internal FlowEventReader(RuntimeMap map, FlowType flow)
		{
			_map = map;
			_flow = flow;
		}

		internal void ReadEvents()
		{
			ReadMethods();

			ReadEventsFromMethods();
		}

		private void ReadMethods()
		{
			foreach(var method in
				from sourceType in _flow.DeclaredType.GetInheritanceChainTo<Flow>(includeType: true, includeTargetType: false)
				from method in sourceType.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
				select method)
			{
				TryReadMethod(method);
			}
		}

		private void TryReadMethod(MethodInfo method)
		{
			if(method.Name == "Before"
				|| method.Name == "BeforeScheduled"
				|| method.Name == "When"
				|| method.Name == "WhenScheduled")
			{
				WarnIfNotPrivate(method);

				TryReadEventMethod(method);
			}
			else
			{
				WarnIfPossiblyMisspelled(method);
			}
		}

		private void WarnIfNotPrivate(MethodInfo method)
		{
			if(!method.IsPrivate)
			{
				Log.Warning("[runtime] Flow method is not private: {0}.{1}", method.DeclaringType.FullName, method.Name);
			}
		}

		private void WarnIfPossiblyMisspelled(MethodInfo method)
		{
			WarnIfPossiblyMisspelled(method, "Before");
			WarnIfPossiblyMisspelled(method, "BeforeScheduled");
			WarnIfPossiblyMisspelled(method, "When");
			WarnIfPossiblyMisspelled(method, "WhenScheduled");
		}

		private void WarnIfPossiblyMisspelled(MethodInfo method, string name)
		{
			if(Text.EditDistance(method.Name, name) <= 2)
			{
				Log.Warning("[runtime] Flow method '{0}' possibly misspelled: {1}.{2}", name, method.DeclaringType.FullName, method.Name);
			}
		}

		private void TryReadEventMethod(MethodInfo method)
		{
			EventType eventType;

			if(TryReadEventType(method, out eventType))
			{
				if(method.Name.StartsWith("Before"))
				{
					ReadBefore(method, eventType);
				}
				else
				{
					ReadWhen(method, eventType);
				}
			}
		}

		private bool TryReadEventType(MethodInfo method, out EventType eventType)
		{
			var parameters = method.GetParameters();
			var firstParameter = parameters.FirstOrDefault();

			if(firstParameter != null && typeof(Event).IsAssignableFrom(firstParameter.ParameterType))
			{
				eventType = _map.GetEvent(firstParameter.ParameterType);

				_eventTypes.Add(eventType);
			}
			else
			{
				Log.Warning("[runtime] Flow method {0}.{1} does not have an event as the first parameter", method.DeclaringType.FullName, method.Name);

				eventType = null;
			}

			return eventType != null;
		}

		private void ReadBefore(MethodInfo method, EventType eventType)
		{
			ReadMethod(eventType, _beforeLookup, new FlowBefore(method, eventType));
		}

		private void ReadWhen(MethodInfo method, EventType eventType)
		{
			ReadMethod(eventType, _whenLookup, new FlowWhen(method, eventType, ReadWhenDependencies(method)));
		}

		private static void ReadMethod<T>(EventType eventType, MethodLookup<T> lookup, T method) where T : FlowMethod
		{
			FlowMethodSet<T> methods;

			if(!lookup.TryGetValue(eventType, out methods))
			{
				methods = new FlowMethodSet<T>(new Many<T>(), new Many<T>());

				lookup.Add(eventType, methods);
			}

			if(method.Info.Name.EndsWith("Scheduled"))
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

		private void ReadEventsFromMethods()
		{
			foreach(var eventType in _eventTypes)
			{
				FlowMethodSet<FlowBefore> before;
				FlowMethodSet<FlowWhen> when;

				if(!_beforeLookup.TryGetValue(eventType, out before))
				{
					before = new FlowMethodSet<FlowBefore>(new Many<FlowBefore>(), new Many<FlowBefore>());
				}

				if(!_whenLookup.TryGetValue(eventType, out when))
				{
					when = new FlowMethodSet<FlowWhen>(new Many<FlowWhen>(), new Many<FlowWhen>());
				}

				_flow.Events.Register(new FlowEvent(_flow, eventType, before, when));
			}
		}
	}
}