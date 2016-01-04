using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Totem.Runtime.Map.Timeline
{
  /// <summary>
	/// A .Route method specifying the identifiers of a <see cref="Flow"/>
	/// </summary>
  public sealed class FlowRoute : FlowMethod
  {
    private readonly Lazy<Func<Event, Many<Id>>> _call;

    public FlowRoute(MethodInfo info, EventType eventType) : base(info, eventType)
    {
      _call = new Lazy<Func<Event, Many<Id>>>(CompileCall);
    }

    public Func<Event, Many<Id>> Call => _call.Value;

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