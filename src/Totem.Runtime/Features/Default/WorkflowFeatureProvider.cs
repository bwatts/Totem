using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Totem.Workflows;

namespace Totem.Features.Default
{
    public class WorkflowFeatureProvider : IFeatureProvider<WorkflowFeature>
    {
        public void PopulateFeature(IEnumerable<FeaturePart> parts, WorkflowFeature feature)
        {
            foreach(var workflow in
                from part in parts.OfType<IFeatureTypeProvider>()
                from type in part.Types
                where typeof(IWorkflow).IsAssignableFrom(type)
                select type.GetTypeInfo())
            {
                feature.Workflows.Add(workflow);
            }
        }
    }
}