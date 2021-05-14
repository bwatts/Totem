using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using Totem.Hosting;

namespace Dream.Hosting
{
    internal static class ServiceCollectionExtensions
    {
        internal static IServiceCollection AddDreamConsole(this IServiceCollection services)
        {
            services
            .AddTotemClient()
            .AddCommands(pipeline => pipeline.UseHttpRequest())
            .AddQueries(pipeline => pipeline.UseHttpRequest());

            services.AddHostedService<ConsoleService>();
            services.AddSingleton<IConsoleRouter, ConsoleRouter>();
            services.AddSingleton(AnsiConsole.Console);

            return services;
        }
    }
}