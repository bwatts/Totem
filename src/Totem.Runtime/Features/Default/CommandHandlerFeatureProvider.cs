using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Totem.Core;

namespace Totem.Features.Default
{
    public class CommandHandlerFeatureProvider : IFeatureProvider<CommandHandlerFeature>
    {
        public void PopulateFeature(IEnumerable<FeaturePart> parts, CommandHandlerFeature feature)
        {
            foreach(var handler in
                from part in parts.OfType<IFeatureTypeProvider>()
                from type in part.Types
                where type.ImplementsGenericInterface(typeof(ICommandHandler<>))
                select type.GetTypeInfo())
            {
                feature.CommandHandlers.Add(handler);
            }
        }
    }
}