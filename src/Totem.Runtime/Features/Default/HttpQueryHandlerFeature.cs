using System.Collections.Generic;
using System.Reflection;

namespace Totem.Features.Default
{
    public class HttpQueryHandlerFeature
    {
        public List<TypeInfo> Handlers { get; } = new List<TypeInfo>();
    }
}