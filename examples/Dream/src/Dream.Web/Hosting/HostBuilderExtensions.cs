using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SpectreConsole;
using Totem.Hosting;

namespace Dream.Hosting
{
    internal static class HostBuilderExtensions
    {
        internal static IHostBuilder ConfigureSerilog(this IHostBuilder builder) =>
            builder.UseSerilog((context, logger) =>
            {
                logger
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithShortTotemTypes()
                .Destructure.ShortIds()
                .UseEnvironment(context);

                if(Environment.UserInteractive)
                {
                    logger.WriteTo.SpectreConsole("{Timestamp:HH:mm:ss} [{Level:u4}] {Message:lj}{NewLine}{Exception}", minLevel: LogEventLevel.Verbose).WriteTo.Debug();
                }
            });

        static void UseEnvironment(this LoggerConfiguration logger, HostBuilderContext context)
        {
            if(!context.HostingEnvironment.IsDevelopment())
            {
                logger.MinimumLevel.Warning();
                return;
            }

            var path = context.Configuration["DREAM_LOG_PATH"] ?? @"C:\Totem\Dream\Logs";
            var file = $"{context.HostingEnvironment.ApplicationName}-{context.HostingEnvironment.EnvironmentName}.log";

            logger.WriteTo.File(Path.Combine(path, file));
        }
    }
}