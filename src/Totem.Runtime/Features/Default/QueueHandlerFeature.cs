using System.Collections.Generic;
using System.Reflection;

namespace Totem.Features.Default
{
    public class QueueHandlerFeature
    {
        public IList<TypeInfo> QueueHandlers { get; } = new List<TypeInfo>();
    }
}