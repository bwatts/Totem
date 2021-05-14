using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Totem.Core;

namespace Totem.Features.Default
{
    public class EventHandlerFeatureProvider : IFeatureProvider<EventHandlerFeature>
    {
        public void PopulateFeature(IEnumerable<FeaturePart> parts, EventHandlerFeature feature)
        {
            foreach(var handler in
                from part in parts.OfType<IFeatureTypeProvider>()
                from type in part.Types
                where type.ImplementsGenericInterface(typeof(IEventHandler<>))
                select type.GetTypeInfo())
            {
                feature.EventHandlers.Add(handler);
            }
        }
    }
}