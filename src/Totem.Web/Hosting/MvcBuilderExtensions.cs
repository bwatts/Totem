using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace Totem.Hosting;

public static class MvcBuilderExtensions
{
    public static IMvcBuilder AddTotemMvc(this IMvcBuilder builder)
    {
        if(builder is null)
            throw new ArgumentNullException(nameof(builder));

        builder.AddJsonOptions(options =>
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        builder.ConfigureApplicationPartManager(applicationPartManager =>
        {
            var map = builder.Services.GetRuntimeMap();

            applicationPartManager.FeatureProviders.Add(new CommandControllerProvider(map));
            applicationPartManager.FeatureProviders.Add(new QueryControllerProvider(map));
            applicationPartManager.FeatureProviders.Add(new InternalControllerProvider());
        });

        return builder;
    }
}
