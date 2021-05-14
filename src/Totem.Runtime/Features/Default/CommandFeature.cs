using System.Collections.Generic;
using System.Reflection;

namespace Totem.Features.Default
{
    public class CommandFeature
    {
        public IList<TypeInfo> Commands { get; } = new List<TypeInfo>();
    }
}