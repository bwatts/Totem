using System;
using System.Linq;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.DependencyInjection;
using Totem.Core;

namespace Totem.Hosting
{
    public static class TotemBuilderExtensions
    {
        public static ITotemBuilder AddTotemAspNetCore(this ITotemBuilder builder)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSingleton<ICorrelationIdAccessor, CorrelationIdAccessor>();
            builder.Services.Configure<MvcOptions>(mvc =>
                mvc.AddTotemRequestConventions().AddTotemModelBinderProvider());

            return builder;
        }

        public static MvcOptions AddTotemRequestConventions(this MvcOptions mvc)
        {
            if(mvc == null)
                throw new ArgumentNullException(nameof(mvc));

            mvc.Conventions.Add(new RequestControllerModelConvention());
            mvc.AllowEmptyInputInBodyModelBinding = true;

            return mvc;
        }

        public static MvcOptions AddTotemModelBinderProvider(this MvcOptions mvc)
        {
            if(mvc == null)
                throw new ArgumentNullException(nameof(mvc));

            var bodyProvider = mvc.ModelBinderProviders.OfType<BodyModelBinderProvider>().FirstOrDefault();
            var complexObjectProvider = mvc.ModelBinderProviders.OfType<ComplexObjectModelBinderProvider>().FirstOrDefault();

            if(bodyProvider == null)
                throw new ArgumentException($"Expected {typeof(IModelBinderProvider)} of type {typeof(BodyModelBinderProvider)}", nameof(mvc));

            if(complexObjectProvider == null)
                throw new ArgumentException($"Expected {typeof(IModelBinderProvider)} of type {typeof(ComplexObjectModelBinderProvider)}", nameof(mvc));

            mvc.ModelBinderProviders.Insert(0, new TotemModelBinderProvider(bodyProvider, complexObjectProvider));

            return mvc;
        }

        public static IMvcBuilder AddCommandControllers(this IMvcBuilder builder)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.Services.AddSingleton<CommandUserMiddleware>();

            builder.ConfigureApplicationPartManager(applicationPartManager =>
            {
                var features = builder.Services.GetFeatures();

                applicationPartManager.FeatureProviders.Add(new CommandControllerFeatureProvider(features));
            });

            return builder;
        }

        public static IMvcBuilder AddQueryControllers(this IMvcBuilder builder)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.Services.AddSingleton<QueryUserMiddleware>();

            builder.ConfigureApplicationPartManager(applicationPartManager =>
            {
                var features = builder.Services.GetFeatures();

                applicationPartManager.FeatureProviders.Add(new QueryControllerFeatureProvider(features));
            });

            return builder;
        }

        public static IMvcBuilder AddJsonStringEnumConverter(this IMvcBuilder builder)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        }
    }
}