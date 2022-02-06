using System.Reflection;

namespace Totem.Map;

public class ObserverMethod : TimelineMethod
{
    internal ObserverMethod(MethodInfo info, ObserverMethodParameter parameter) : base(info, parameter)
    { }

    public new ObserverMethodParameter Parameter => (ObserverMethodParameter) base.Parameter;
}
