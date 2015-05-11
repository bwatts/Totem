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
		private sealed class MethodLookup<TMethod> : Dictionary<EventType, List<TMethod>> {}

		private readonly HashSet<EventType> _eventTypes = new HashSet<EventType>();
		private readonly MethodLookup<FlowEventBefore> _beforeLookup = new MethodLookup<FlowEventBefore>();
		private readonly MethodLookup<FlowEventWhen> _whenLookup = new MethodLookup<FlowEventWhen>();
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
			if(method.Name == "Before" || method.Name == "When")
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
			if(Text.EditDistance(method.Name, "Before") <= 3)
			{
				Log.Warning("[runtime] Flow method 'Before' possibly misspelled: {0}.{1}", method.DeclaringType.FullName, method.Name);
			}
			else
			{
				if(Text.EditDistance(method.Name, "When") <= 2)
				{
					Log.Warning("[runtime] Flow method 'When' possibly misspelled: {0}.{1}", method.DeclaringType.FullName, method.Name);
				}
			}
		}

		private void TryReadEventMethod(MethodInfo method)
		{
			EventType eventType;

			if(TryReadEventType(method, out eventType))
			{
				if(method.Name == "Before")
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
			ReadEventMethod(eventType, _beforeLookup, new FlowEventBefore(method, eventType));
		}

		private void ReadWhen(MethodInfo method, EventType eventType)
		{
			ReadEventMethod(eventType, _whenLookup, new FlowEventWhen(method, eventType, ReadWhenDependencies(method)));
		}

		private void ReadEventMethod<T>(EventType eventType, MethodLookup<T> lookup, T method)
		{
			List<T> methods;

			if(!lookup.TryGetValue(eventType, out methods))
			{
				methods = new List<T>();

				lookup.Add(eventType, methods);
			}

			methods.Add(method);
		}

		private IReadOnlyList<WhenDependency> ReadWhenDependencies(MethodInfo method)
		{
			return method.GetParameters().Skip(1).Select(ReadWhenDependency).ToList();
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
				List<FlowEventBefore> beforeMethods;
				List<FlowEventWhen> whenMethods;

				if(!_beforeLookup.TryGetValue(eventType, out beforeMethods))
				{
					beforeMethods = new List<FlowEventBefore>();
				}

				if(!_whenLookup.TryGetValue(eventType, out whenMethods))
				{
					whenMethods = new List<FlowEventWhen>();
				}

				_flow.Events.Register(new FlowEvent(_flow, eventType, beforeMethods, whenMethods));
			}
		}
	}
}