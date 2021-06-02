using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Totem.Http;

namespace Totem.Features.Default
{
    public class HttpCommandProvider : IFeatureProvider<HttpCommandFeature>
    {
        public void PopulateFeature(IEnumerable<FeaturePart> parts, HttpCommandFeature feature)
        {
            if(parts == null)
                throw new ArgumentNullException(nameof(parts));

            if(feature == null)
                throw new ArgumentNullException(nameof(feature));

            feature.Commands.AddRange(
                from part in parts.OfType<IFeatureTypeProvider>()
                from type in part.Types
                where HttpCommandInfo.IsHttpCommand(type)
                select type.GetTypeInfo());
        }
    }
}