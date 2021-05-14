using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Totem.Features.Default
{
    public class QueryFeatureProvider : IFeatureProvider<QueryFeature>
    {
        public void PopulateFeature(IEnumerable<FeaturePart> parts, QueryFeature feature)
        {
            foreach(var query in
                from part in parts.OfType<IFeatureTypeProvider>()
                from type in part.Types
                where QueryInfo.IsQuery(type)
                select type.GetTypeInfo())
            {
                feature.Queries.Add(query);
            }
        }
    }
}