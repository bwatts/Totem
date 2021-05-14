using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Totem.Core;

namespace Totem.Features.Default
{
    public class QueryHandlerFeatureProvider : IFeatureProvider<QueryHandlerFeature>
    {
        public void PopulateFeature(IEnumerable<FeaturePart> parts, QueryHandlerFeature feature)
        {
            foreach(var handler in
                from part in parts.OfType<IFeatureTypeProvider>()
                from type in part.Types
                where type.ImplementsGenericInterface(typeof(IQueryHandler<>))
                select type.GetTypeInfo())
            {
                feature.QueryHandlers.Add(handler);
            }
        }
    }
}