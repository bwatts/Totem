using System;
using Dream;
using Dream.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

try
{
    await Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webHost => webHost.UseStartup<Startup>())
        .ConfigureSerilog()
        .Build()
        .RunAsync();

    return 0;
}
catch(Exception exception)
{
    var foreground = Console.ForegroundColor;

    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(exception);
    Console.ForegroundColor = foreground;

    return 1;
}