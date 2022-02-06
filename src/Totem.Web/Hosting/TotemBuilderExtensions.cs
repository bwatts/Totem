using System;
using System.Linq;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.DependencyInjection;

namespace Totem.Hosting;

public static class TotemBuilderExtensions
{
    public static ITotemBuilder AddTotemWeb(this ITotemBuilder builder)
    {
        if(builder is null)
            throw new ArgumentNullException(nameof(builder));

        builder.Services
            .AddHttpContextAccessor()
            .Configure<MvcOptions>(mvc => mvc.AddTotemRequestConventions().AddTotemModelBinderProvider());

        return builder;
    }

    public static MvcOptions AddTotemRequestConventions(this MvcOptions mvc)
    {
        if(mvc is null)
            throw new ArgumentNullException(nameof(mvc));

        mvc.Conventions.Add(new RequestControllerModelConvention());
        mvc.AllowEmptyInputInBodyModelBinding = true;

        return mvc;
    }

    public static MvcOptions AddTotemModelBinderProvider(this MvcOptions mvc)
    {
        if(mvc is null)
            throw new ArgumentNullException(nameof(mvc));

        var bodyProvider = mvc.ModelBinderProviders.OfType<BodyModelBinderProvider>().FirstOrDefault();
        var complexObjectProvider = mvc.ModelBinderProviders.OfType<ComplexObjectModelBinderProvider>().FirstOrDefault();

        if(bodyProvider is null)
            throw new ArgumentException($"Expected {typeof(IModelBinderProvider)} of type {typeof(BodyModelBinderProvider)}", nameof(mvc));

        if(complexObjectProvider is null)
            throw new ArgumentException($"Expected {typeof(IModelBinderProvider)} of type {typeof(ComplexObjectModelBinderProvider)}", nameof(mvc));

        mvc.ModelBinderProviders.Insert(0, new TotemModelBinderProvider(bodyProvider, complexObjectProvider));

        return mvc;
    }

    public static IMvcBuilder AddCommandControllers(this IMvcBuilder builder)
    {
        if(builder is null)
            throw new ArgumentNullException(nameof(builder));

        builder.ConfigureApplicationPartManager(applicationPartManager =>
        {
            var map = builder.Services.GetRuntimeMap();

            applicationPartManager.FeatureProviders.Add(new CommandControllerProvider(map));
        });

        return builder;
    }

    public static IMvcBuilder AddQueryControllers(this IMvcBuilder builder)
    {
        if(builder is null)
            throw new ArgumentNullException(nameof(builder));

        builder.ConfigureApplicationPartManager(applicationPartManager =>
        {
            var map = builder.Services.GetRuntimeMap();

            applicationPartManager.FeatureProviders.Add(new QueryControllerProvider(map));
        });

        return builder;
    }

    public static IMvcBuilder AddJsonStringEnumConverter(this IMvcBuilder builder)
    {
        if(builder is null)
            throw new ArgumentNullException(nameof(builder));

        return builder.AddJsonOptions(options =>
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
    }
}
