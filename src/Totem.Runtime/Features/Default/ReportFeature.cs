using System.Collections.Generic;
using System.Reflection;

namespace Totem.Features.Default
{
    public class ReportFeature
    {
        public IList<TypeInfo> Reports { get; } = new List<TypeInfo>();
    }
}