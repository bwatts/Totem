using System.Collections.Generic;
using System.Reflection;

namespace Totem.Features.Default
{
    public class WorkflowFeature
    {
        public List<TypeInfo> Workflows { get; } = new List<TypeInfo>();
    }
}