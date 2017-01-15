using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Totem.Runtime.Timeline;

namespace Totem.Runtime.Map.Timeline
{
  /// <summary>
  /// Creates an event that initiates a request
  /// </summary>
  public class RequestStartMethod
  {
    readonly Lazy<Func<Request, IDependencySource, Event>> _call;
    readonly Lazy<Func<Request, IDependencySource, Task<Event>>> _callAsync;

    internal RequestStartMethod(MethodInfo info, Many<EventType> eventTypes, Many<Dependency> dependencies)
    {
      Info = info;
      EventTypes = eventTypes;
      Dependencies = dependencies;

      if(typeof(Task).IsAssignableFrom(info.ReturnType))
      {
        IsAsync = true;

        _callAsync = CreateCall<Func<Request, IDependencySource, Task<Event>>>();
      }
      else
      {
        _call = CreateCall<Func<Request, IDependencySource, Event>>();
      }
    }

    public readonly MethodInfo Info;
    public readonly Many<EventType> EventTypes;
    public readonly Many<Dependency> Dependencies;
    public readonly bool IsAsync;

    public async Task<Event> Call(Request request, IDependencySource dependencies)
    {
      if(IsAsync)
      {
        return await _callAsync.Value(request, dependencies);
      }
      else
      {
        return _call.Value(request, dependencies);
      }
    }

    Lazy<TLambda> CreateCall<TLambda>() => new Lazy<TLambda>(CompileCall<TLambda>);

    TLambda CompileCall<TLambda>()
    {
      // We are building an expression tree representing a call to this method:
      //
      // (request, dependencies) => ((TRequest) request).Info([resolved dependencies])
      //
      // Let's break each part down, starting with the parameters:

      var request = Expression.Parameter(typeof(Request), "request");
      var dependencies = Expression.Parameter(typeof(IDependencySource), "dependencies");

      // Cast the request to its specific type:
      //
      // (TRequest) request

      var castRequest = Expression.Convert(request, Info.DeclaringType);

      // Resolve dependencies:
      //
      // dependencies.Resolve<T>()
      // dependencies.ResolveNamed<T>("name")
      // dependencies.ResolveKeyed<T>(key)

      var resolvedDependencies = Dependencies.Select(dependency => dependency.ResolveIn(dependencies));

      // Call the method on the request, passing the dependencies:
      //
      // ((TRequest) request).Info([resolved dependencies])

      var call = Expression.Call(castRequest, Info, resolvedDependencies);

      // Compile a lambda expression into a delegate we can invoke:
      //
      // (request, dependencies) => [call]

      return Expression.Lambda<TLambda>(call, request, dependencies).Compile();
    }
  }
}