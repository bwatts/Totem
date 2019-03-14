using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Totem.Timeline.Area
{
  /// <summary>
  /// A .Route method selecting events to observe within a <see cref="Flow"/>
  /// </summary>
  public sealed class FlowRoute : FlowMethod
  {
    readonly Lazy<Func<Event, Id>> _callSingle;
    readonly Lazy<Func<Event, IEnumerable<Id>>> _callMultiple;

    internal FlowRoute(MethodInfo info, EventType eventType, FlowType flowType, bool first) : base(info, eventType)
    {
      FlowType = flowType;
      First = first;

      if(info.ReturnType == typeof(Id))
      {
        _callSingle = new Lazy<Func<Event, Id>>(CompileCallSingle);
      }
      else
      {
        _callMultiple = new Lazy<Func<Event, IEnumerable<Id>>>(CompileCallMultiple);
      }
    }

    public readonly FlowType FlowType;
    public readonly bool First;

    public IEnumerable<Id> Call(Event e)
    {
      if(_callSingle != null)
      {
        yield return _callSingle.Value(e);
      }
      else
      {
        foreach(var id in _callMultiple.Value(e))
        {
          yield return id;
        }
      }
    }

    Func<Event, Id> CompileCallSingle()
    {
      // We are building an expression tree representing a call to this method:
      //
      // e => .Info((TEvent) e)

      var e = Expression.Parameter(typeof(Event), "e");

      // Call the static method, casting the event to its specific type:
      //
      // .Info((TEvent) e)

      var call = Expression.Call(Info, Expression.Convert(e, EventType.DeclaredType));

      // Create a lambda expression that makes the call:
      //
      // e => call(e)

      var lambda = Expression.Lambda<Func<Event, Id>>(call, e);

      // Compile the lambda into a delegate we can invoke:

      return lambda.Compile();
    }

    Func<Event, IEnumerable<Id>> CompileCallMultiple()
    {
      // We are building an expression tree representing a call to this method:
      //
      // e => .Info((TEvent) e)

      var e = Expression.Parameter(typeof(Event), "e");

      // Call the static method, casting the event to its specific type:
      //
      // .Info((TEvent) e)

      var call = Expression.Call(Info, Expression.Convert(e, EventType.DeclaredType));

      // Make an IEnumerable<Id> out of the return value

      if(Info.ReturnType != typeof(IEnumerable<Id>))
      {
        // call.AsEnumerable()

        call = Expression.Call(typeof(Enumerable), "AsEnumerable", new[] { typeof(Id) }, call);
      }

      // Create a lambda expression that makes the call:
      //
      // e => [call]

      var lambda = Expression.Lambda<Func<Event, IEnumerable<Id>>>(call, e);

      // Compile the lambda into a delegate we can invoke:

      return lambda.Compile();
    }
  }
}