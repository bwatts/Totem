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
    public class QueryControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        readonly FeatureRegistry _features;

        public QueryControllerFeatureProvider(FeatureRegistry features) =>
            _features = features ?? throw new ArgumentNullException(nameof(features));

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            if(feature == null)
                throw new ArgumentNullException(nameof(feature));

            foreach(var controllerType in
                from query in _features.Populate<QueryFeature>().Queries
                select typeof(QueryController<>).MakeGenericType(query))
            {
                feature.Controllers.Add(controllerType.GetTypeInfo());
            }
        }
    }
}