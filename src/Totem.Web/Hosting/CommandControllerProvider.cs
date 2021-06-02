using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Totem.Features;
using Totem.Features.Default;

namespace Totem.Hosting
{
    public class CommandControllerProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        readonly FeatureRegistry _features;

        public CommandControllerProvider(FeatureRegistry features) =>
            _features = features ?? throw new ArgumentNullException(nameof(features));

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            if(parts == null)
                throw new ArgumentNullException(nameof(parts));

            if(feature == null)
                throw new ArgumentNullException(nameof(feature));

            foreach(var controllerType in
                from command in _features.Populate<HttpCommandFeature>().Commands
                select typeof(CommandController<>).MakeGenericType(command))
            {
                feature.Controllers.Add(controllerType.GetTypeInfo());
            }
        }
    }
}