using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Totem.Timeline.Area
{
  /// <summary>
  /// Sets state in reaction to an event within a <see cref="Flow"/>
  /// </summary>
  public sealed class FlowGiven : FlowMethod
  {
    readonly Lazy<Action<Flow, Event>> _call;

    internal FlowGiven(MethodInfo info, EventType eventType) : base(info, eventType)
    {
      _call = new Lazy<Action<Flow, Event>>(CompileCall);
    }

    public Action<Flow, Event> Call => _call.Value;

    Action<Flow, Event> CompileCall()
    {
      // We are building an expression tree representing a call to this method:
      //
      // (flow, e) => ((TFlow) flow).Info((TEvent) e)
      //
      // Let's break each part down, starting with the parameter:

      var flow = Expression.Parameter(typeof(Flow), "flow");
      var e = Expression.Parameter(typeof(Event), "e");

      // Cast the flow and event to their specific types:
      //
      // (TFlow) flow
      // (TEvent) e

      var castFlow = Expression.Convert(flow, Info.DeclaringType);
      var castEvent = Expression.Convert(e, EventType.DeclaredType);

      // Call the method on the flow, passing the event and dependencies:
      //
      // ((TFlow) flow).Info((TEvent) e)

      var call = Expression.Call(castFlow, Info, castEvent);

      // Compile a lambda expression into a delegate we can invoke:
      //
      // (flow, e) => [call]

      return Expression.Lambda<Action<Flow, Event>>(call, flow, e).Compile();
    }
  }
}