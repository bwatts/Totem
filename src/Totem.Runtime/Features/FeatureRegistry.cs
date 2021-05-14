using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Features
{
    public class FeatureRegistry
    {
        public IList<IFeatureProvider> Providers { get; } = new List<IFeatureProvider>();
        public IList<FeaturePart> Parts { get; } = new List<FeaturePart>();

        public void Populate<TFeature>(TFeature feature)
        {
            if(feature == null)
                throw new ArgumentNullException(nameof(feature));

            foreach(var provider in Providers.OfType<IFeatureProvider<TFeature>>())
            {
                provider.PopulateFeature(Parts, feature);
            }
        }

        public TFeature Populate<TFeature>() where TFeature : new()
        {
            var feature = new TFeature();

            Populate(feature);

            return feature;
        }
    }
}