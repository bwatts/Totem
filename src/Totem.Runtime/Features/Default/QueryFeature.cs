using System.Collections.Generic;
using System.Reflection;

namespace Totem.Features.Default
{
    public class QueryFeature
    {
        public IList<TypeInfo> Queries { get; } = new List<TypeInfo>();
    }
}