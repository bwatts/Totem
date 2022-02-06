using System.Reflection;

namespace Totem.Map;

public abstract class TopicMethod : TimelineMethod
{
    internal TopicMethod(MethodInfo info, TopicMethodParameter parameter) : base(info, parameter)
    { }

    public new TopicMethodParameter Parameter => (TopicMethodParameter) base.Parameter;
}
