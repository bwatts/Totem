using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.DependencyInjection;
using Totem.InternalApi;
using Totem.Reports.Subscriptions;
using Totem.Subscriptions;

namespace Totem.Hosting;

public static class TotemBuilderWebExtensions
{
    public static ITotemBuilder AddTotemWeb(this ITotemBuilder builder)
    {
        if(builder is null)
            throw new ArgumentNullException(nameof(builder));

        builder.Services
        .AddHttpContextAccessor()
        .AddScoped<SubscriptionHub>()
        .AddSingleton<IReportBroker, ReportBroker>()
        .AddSingleton<IReportChannel, SubscriptionHubReportChannel>()
        .Configure<MvcOptions>(mvc =>
        {
            mvc.Conventions.Add(new RequestControllerModelConvention());
            mvc.Conventions.Add(new InternalApiControllerModelConvention());

            mvc.AllowEmptyInputInBodyModelBinding = true;

            var bodyProvider = mvc.ModelBinderProviders.OfType<BodyModelBinderProvider>().FirstOrDefault();
            var complexObjectProvider = mvc.ModelBinderProviders.OfType<ComplexObjectModelBinderProvider>().FirstOrDefault();

            if(bodyProvider is null)
                throw new ArgumentException($"Expected {typeof(IModelBinderProvider)} of type {typeof(BodyModelBinderProvider)}", nameof(mvc));

            if(complexObjectProvider is null)
                throw new ArgumentException($"Expected {typeof(IModelBinderProvider)} of type {typeof(ComplexObjectModelBinderProvider)}", nameof(mvc));

            mvc.ModelBinderProviders.Insert(0, new TotemModelBinderProvider(bodyProvider, complexObjectProvider));
        });

        return builder;
    }
}
