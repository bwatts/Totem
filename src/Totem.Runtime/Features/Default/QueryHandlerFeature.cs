using System.Collections.Generic;
using System.Reflection;

namespace Totem.Features.Default
{
    public class QueryHandlerFeature
    {
        public IList<TypeInfo> QueryHandlers { get; } = new List<TypeInfo>();
    }
}