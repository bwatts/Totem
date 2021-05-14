using System.Collections.Generic;
using System.Reflection;

namespace Totem.Features.Default
{
    public class EventHandlerFeature
    {
        public IList<TypeInfo> EventHandlers { get; } = new List<TypeInfo>();
    }
}