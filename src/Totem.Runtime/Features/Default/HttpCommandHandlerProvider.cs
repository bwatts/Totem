using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Totem.Core;

namespace Totem.Features.Default
{
    public class HttpCommandHandlerProvider : IFeatureProvider<HttpCommandHandlerFeature>
    {
        public void PopulateFeature(IEnumerable<FeaturePart> parts, HttpCommandHandlerFeature feature)
        {
            if(parts == null)
                throw new ArgumentNullException(nameof(parts));

            if(feature == null)
                throw new ArgumentNullException(nameof(feature));

            feature.Handlers.AddRange(
                from part in parts.OfType<IFeatureTypeProvider>()
                from type in part.Types
                where type.ImplementsGenericInterface(typeof(IHttpCommandHandler<>))
                select type.GetTypeInfo());
        }
    }
}