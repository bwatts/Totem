using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Totem.InternalApi;

namespace Totem.Hosting;

internal class InternalControllerProvider : IApplicationFeatureProvider<ControllerFeature>
{
    public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
    {
        if(parts is null)
            throw new ArgumentNullException(nameof(parts));

        if(feature is null)
            throw new ArgumentNullException(nameof(feature));

        foreach(var internalController in new[] { typeof(MapController) })
        {
            feature.Controllers.Add(internalController.GetTypeInfo());
        }
    }
}
