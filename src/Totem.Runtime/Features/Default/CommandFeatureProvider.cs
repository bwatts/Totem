using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Totem.Features.Default
{
    public class CommandFeatureProvider : IFeatureProvider<CommandFeature>
    {
        public void PopulateFeature(IEnumerable<FeaturePart> parts, CommandFeature feature)
        {
            foreach(var command in
                from part in parts.OfType<IFeatureTypeProvider>()
                from type in part.Types
                where CommandInfo.IsCommand(type)
                select type.GetTypeInfo())
            {
                feature.Commands.Add(command);
            }
        }
    }
}