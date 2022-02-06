using System.Reflection;

namespace Totem.Map;

public class GivenMethodParameter : TimelineMethodParameter
{
    internal GivenMethodParameter(ParameterInfo info, EventType message) : base(info, message)
    { }

    public new EventType Message => (EventType) base.Message;
}
