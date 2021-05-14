using System.Collections.Generic;
using System.Reflection;

namespace Totem.Features
{
    public interface IFeatureTypeProvider
    {
        IEnumerable<TypeInfo> Types { get; }
    }
}