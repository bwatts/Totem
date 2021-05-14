using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Totem.Reports;

namespace Totem.Features.Default
{
    public class ReportFeatureProvider : IFeatureProvider<ReportFeature>
    {
        public void PopulateFeature(IEnumerable<FeaturePart> parts, ReportFeature feature)
        {
            foreach(var report in
                from part in parts.OfType<IFeatureTypeProvider>()
                from type in part.Types
                where typeof(IReport).IsAssignableFrom(type)
                select type.GetTypeInfo())
            {
                feature.Reports.Add(report);
            }
        }
    }
}