using System.Collections.Generic;
using System.Reflection;

namespace Totem.Features.Default
{
    public class CommandHandlerFeature
    {
        public IList<TypeInfo> CommandHandlers { get; } = new List<TypeInfo>();
    }
}