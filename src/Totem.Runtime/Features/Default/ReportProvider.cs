using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Totem.Reports;

namespace Totem.Features.Default
{
    public class ReportProvider : IFeatureProvider<ReportFeature>
    {
        public void PopulateFeature(IEnumerable<FeaturePart> parts, ReportFeature feature)
        {
            if(parts == null)
                throw new ArgumentNullException(nameof(parts));

            if(feature == null)
                throw new ArgumentNullException(nameof(feature));

            feature.Reports.AddRange(
                from part in parts.OfType<IFeatureTypeProvider>()
                from type in part.Types
                where typeof(IReport).IsAssignableFrom(type)
                select type.GetTypeInfo());
        }
    }
}