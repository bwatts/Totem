using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Totem.Timeline.Area
{
  /// <summary>
  /// Makes decisions in reaction to an event within a <see cref="Topic"/>
  /// </summary>
  public sealed class TopicWhen : FlowMethod
  {
    readonly Lazy<Action<Topic, Event, IServiceProvider>> _call;
    readonly Lazy<Func<Topic, Event, IServiceProvider, Task>> _callAsync;

    internal TopicWhen(MethodInfo info, EventType eventType, Many<TopicWhenDependency> dependencies)
      : base(info, eventType)
    {
      Dependencies = dependencies;

      if(typeof(Task).IsAssignableFrom(info.ReturnType))
      {
        IsAsync = true;

        _callAsync = CreateCall<Func<Topic, Event, IServiceProvider, Task>>();
      }
      else
      {
        _call = CreateCall<Action<Topic, Event, IServiceProvider>>();
      }
    }

    public readonly Many<TopicWhenDependency> Dependencies;
    public readonly bool IsAsync;

    public async Task Call(Topic topic, Event e, IServiceProvider services)
    {
      if(IsAsync)
      {
        await _callAsync.Value(topic, e, services);
      }
      else
      {
        _call.Value(topic, e, services);
      }
    }

    Lazy<TLambda> CreateCall<TLambda>() =>
      new Lazy<TLambda>(CompileCall<TLambda>);

    TLambda CompileCall<TLambda>()
    {
      // We are building an expression tree representing a call to this method:
      //
      // (topic, e, services) => ((TTopic) topic).Info((TEvent) e, [dependencies])
      //
      // Let's break each part down, starting with the parameters:

      var topic = Expression.Parameter(typeof(Topic), "topic");
      var e = Expression.Parameter(typeof(Event), "e");
      var services = Expression.Parameter(typeof(IServiceProvider), "services");

      // Cast the topic and event to their specific types:
      //
      // (TTopic) topic
      // (TEvent) e

      var castTopic = Expression.Convert(topic, Info.DeclaringType);
      var castEvent = Expression.Convert(e, EventType.DeclaredType);

      // Resolve dependencies via Microsoft.Extensions.DependencyInjection

      var dependencies = Dependencies.Select(dependency => dependency.ResolveIn(services));

      // Call the method on the topic, passing the event and dependencies:
      //
      // ((TTopic) topic).Info((TEvent) e, [dependencies])

      var call = Expression.Call(castTopic, Info, Many.Of(castEvent, dependencies));

      // Compile a lambda expression into a delegate we can invoke:
      //
      // (topic, e, services) => [call]

      return Expression.Lambda<TLambda>(call, topic, e, services).Compile();
    }
  }
}