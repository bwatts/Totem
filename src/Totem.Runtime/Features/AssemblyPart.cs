using System;
using System.Collections.Generic;
using System.Reflection;

namespace Totem.Features
{
    public class AssemblyPart : FeaturePart, IFeatureTypeProvider
    {
        public AssemblyPart(Assembly assembly) =>
            Assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));

        public Assembly Assembly { get; }
        public IEnumerable<TypeInfo> Types => Assembly.DefinedTypes;

        public override string Name =>
            Assembly.GetName().Name ?? Assembly.ToString();
    }
}