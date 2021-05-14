using System.Collections.Generic;
using System.Reflection;

namespace Totem.Features.Default
{
    public class WorkflowFeature
    {
        public IList<TypeInfo> Workflows { get; } = new List<TypeInfo>();
    }
}