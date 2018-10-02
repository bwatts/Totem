using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Totem.Timeline.Area
{
  /// <summary>
  /// Makes decisions in reaction to an event within a <see cref="Flow"/>
  /// </summary>
  public sealed class FlowWhen : FlowMethod
  {
    readonly Lazy<Action<Flow, Event, IServiceProvider>> _call;
    readonly Lazy<Func<Flow, Event, IServiceProvider, Task>> _callAsync;

    public FlowWhen(MethodInfo info, EventType eventType, Many<FlowDependency> dependencies) : base(info, eventType)
    {
      Dependencies = dependencies;

      if(typeof(Task).IsAssignableFrom(info.ReturnType))
      {
        IsAsync = true;

        _callAsync = CreateCall<Func<Flow, Event, IServiceProvider, Task>>();
      }
      else
      {
        _call = CreateCall<Action<Flow, Event, IServiceProvider>>();
      }
    }

    public readonly Many<FlowDependency> Dependencies;
    public readonly bool IsAsync;

    public async Task Call(Flow flow, Event e, IServiceProvider services)
    {
      if(IsAsync)
      {
        await _callAsync.Value(flow, e, services);
      }
      else
      {
        _call.Value(flow, e, services);
      }
    }

    Lazy<TLambda> CreateCall<TLambda>() =>
      new Lazy<TLambda>(CompileCall<TLambda>);

    TLambda CompileCall<TLambda>()
    {
      // We are building an expression tree representing a call to this method:
      //
      // (flow, e, services) => ((TFlow) flow).Info((TEvent) e, [dependencies])
      //
      // Let's break each part down, starting with the parameters:

      var flow = Expression.Parameter(typeof(Flow), "flow");
      var e = Expression.Parameter(typeof(Event), "e");
      var services = Expression.Parameter(typeof(IServiceProvider), "services");

      // Cast the flow and event to their specific types:
      //
      // (TFlow) flow
      // (TEvent) e

      var castFlow = Expression.Convert(flow, Info.DeclaringType);
      var castEvent = Expression.Convert(e, EventType.DeclaredType);

      // Resolve dependencies via Microsoft.Extensions.DependencyInjection

      var dependencies = Dependencies.Select(dependency => dependency.ResolveIn(services));

      // Call the method on the flow, passing the event and dependencies:
      //
      // ((TFlow) flow).Info((TEvent) e, [dependencies])

      var call = Expression.Call(castFlow, Info, Many.Of(castEvent, dependencies));

      // Compile a lambda expression into a delegate we can invoke:
      //
      // (flow, e, services) => [call]

      return Expression.Lambda<TLambda>(call, flow, e, services).Compile();
    }
  }
}