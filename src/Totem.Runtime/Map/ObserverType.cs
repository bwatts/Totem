using Totem.Core;

namespace Totem.Map;

public abstract class ObserverType : TimelineType
{
    internal ObserverType(Type declaredType, ObserverConstructor constructor) : base(declaredType) =>
        Constructor = constructor;

    public ObserverConstructor Constructor { get; }
    public TypeKeyedCollection<Observation> Observations { get; } = new();

    internal IObserverTimeline Create(Id id)
    {
        var observer = Constructor.Call();

        observer.Id = id;

        return observer;
    }

    internal IEnumerable<ItemKey> CallRoute(IEventContext<IEvent> context)
    {
        if(Observations.TryGet(context.EventType.DeclaredType, out var observation))
        {
            foreach(var reportKey in observation.CallRoute(context))
            {
                yield return reportKey;
            }
        }
    }

    internal void CallGivenIfDefined(IObserverTimeline observer, IEvent e)
    {
        if(Observations.TryGet(e.GetType(), out var observation) && observation.Given is not null)
        {
            observation.Given.Call(observer, e);
        }
    }

    internal void CallWhenIfDefined(IObserverTimeline observer, IEventContext<IEvent> context)
    {
        if(Observations.TryGet(context.EventType.DeclaredType, out var observation) && observation.When is not null)
        {
            observation.When.Call(observer, context);
        }
    }
}
