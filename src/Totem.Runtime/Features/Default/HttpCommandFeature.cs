using System.Collections.Generic;
using System.Reflection;

namespace Totem.Features.Default
{
    public class HttpCommandFeature
    {
        public List<TypeInfo> Commands { get; } = new List<TypeInfo>();
    }
}