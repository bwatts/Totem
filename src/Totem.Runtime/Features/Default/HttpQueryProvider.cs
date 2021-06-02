using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Totem.Http;

namespace Totem.Features.Default
{
    public class HttpQueryProvider : IFeatureProvider<HttpQueryFeature>
    {
        public void PopulateFeature(IEnumerable<FeaturePart> parts, HttpQueryFeature feature)
        {
            if(parts == null)
                throw new ArgumentNullException(nameof(parts));

            if(feature == null)
                throw new ArgumentNullException(nameof(feature));

            feature.Queries.AddRange(
                from part in parts.OfType<IFeatureTypeProvider>()
                from type in part.Types
                where HttpQueryInfo.IsHttpQuery(type)
                select type.GetTypeInfo());
        }
    }
}