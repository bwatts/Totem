using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Totem.Runtime.Timeline;

namespace Totem.Runtime.Map.Timeline
{
  /// <summary>
	/// A .Route method specifying the identifiers of a <see cref="Flow"/>
	/// </summary>
  public sealed class FlowRoute : FlowMethod
  {
    private readonly Lazy<Func<Event, Many<Id>>> _call;

    public FlowRoute(MethodInfo info, EventType eventType, FlowType flowType, bool isFirst) : base(info, eventType)
    {
			FlowType = flowType;
			IsFirst = isFirst;

      _call = new Lazy<Func<Event, Many<Id>>>(CompileCall);
    }

		public readonly FlowType FlowType;
		public readonly bool IsFirst;

		public IEnumerable<TimelineRoute> Call(Event e)
		{
			return
				from id in _call.Value(e)
				select new TimelineRoute(FlowKey.From(FlowType, id), IsFirst);
		}

    private Func<Event, Many<Id>> CompileCall()
    {
      // We are building an expression tree representing a call to this method:
      //
      // e => .Info((TEvent) e)

      var e = Expression.Parameter(typeof(Event), "e");

      // Call the static method, casting the event to its specific type:
      //
      // .Info((TEvent) e)

      var call = Expression.Call(Info, Expression.Convert(e, EventType.DeclaredType));

      // Make a Many<Id> out of the return value

      if(Info.ReturnType == typeof(Id))
      {
        // Many.Of(call)

        call = Expression.Call(typeof(Many), "Of", new[] { typeof(Id) }, call);
      }
      else
      {
        if(Info.ReturnType != typeof(Many<Id>))
        {
          // call.ToMany()

          call = Expression.Call(typeof(Sequences), "ToMany", new[] { typeof(Id) }, call);
        }
      }

      // Create a lambda expression that makes the call:
      //
      // e => [call]

      var lambda = Expression.Lambda<Func<Event, Many<Id>>>(call, e);

      // Compile the lambda into a delegate we can use to make the call:

      return lambda.Compile();
    }
  }
}