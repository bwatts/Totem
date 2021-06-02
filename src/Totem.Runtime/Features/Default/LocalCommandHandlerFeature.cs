using System.Collections.Generic;
using System.Reflection;

namespace Totem.Features.Default
{
    public class LocalCommandHandlerFeature
    {
        public List<TypeInfo> Handlers { get; } = new List<TypeInfo>();
    }
}