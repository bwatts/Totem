using System.Collections.Generic;
using System.Reflection;

namespace Totem.Features.Default
{
    public class HttpQueryFeature
    {
        public List<TypeInfo> Queries { get; } = new List<TypeInfo>();
    }
}