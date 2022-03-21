using System.Linq.Expressions;
using System.Reflection;
using Totem.Core;

namespace Totem.Map;

public class ObserverConstructor
{
    internal ObserverConstructor(Type observerType)
    {
        // () => new TObserver()

        var lambda = Expression.Lambda<Func<IObserverTimeline>>(Expression.New(observerType));

        Call = lambda.Compile();
    }

    internal ObserverConstructor(ConstructorInfo info)
    {
        Info = info;

        // () => info()

        var lambda = Expression.Lambda<Func<IObserverTimeline>>(Expression.New(info));

        Call = lambda.Compile();
    }

    public ConstructorInfo? Info { get; }
    internal Func<IObserverTimeline> Call { get; }
}
