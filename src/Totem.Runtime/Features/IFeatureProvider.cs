using System.Collections.Generic;

namespace Totem.Features
{
    public interface IFeatureProvider
    {

    }

    public interface IFeatureProvider<TFeature> : IFeatureProvider
    {
        void PopulateFeature(IEnumerable<FeaturePart> parts, TFeature feature);
    }
}