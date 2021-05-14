using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Totem.Core;

namespace Totem.Features.Default
{
    public class QueueHandlerFeatureProvider : IFeatureProvider<QueueHandlerFeature>
    {
        public void PopulateFeature(IEnumerable<FeaturePart> parts, QueueHandlerFeature feature)
        {
            foreach(var handler in
                from part in parts.OfType<IFeatureTypeProvider>()
                from type in part.Types
                where type.ImplementsGenericInterface(typeof(IQueueHandler<>))
                select type.GetTypeInfo())
            {
                feature.QueueHandlers.Add(handler);
            }
        }
    }
}