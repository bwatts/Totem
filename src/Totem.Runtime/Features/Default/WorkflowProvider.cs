using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Totem.Workflows;

namespace Totem.Features.Default
{
    public class WorkflowProvider : IFeatureProvider<WorkflowFeature>
    {
        public void PopulateFeature(IEnumerable<FeaturePart> parts, WorkflowFeature feature)
        {
            if(parts == null)
                throw new ArgumentNullException(nameof(parts));

            if(feature == null)
                throw new ArgumentNullException(nameof(feature));

            feature.Workflows.AddRange(
                from part in parts.OfType<IFeatureTypeProvider>()
                from type in part.Types
                where typeof(IWorkflow).IsAssignableFrom(type)
                select type.GetTypeInfo());
        }
    }
}