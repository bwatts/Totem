using System;
using Dream.Hosting;
using Microsoft.Extensions.Hosting;
using Spectre.Console;

try
{
    await Host.CreateDefaultBuilder(args)
        .ConfigureSerilog()
        .ConfigureServices(services => services.AddDreamConsole())
        .Build()
        .RunAsync();

    return 0;
}
catch(Exception exception)
{
    var rule = new Rule().RuleStyle("orange1");

    AnsiConsole.WriteLine();
    AnsiConsole.MarkupLine("[orange1]Dream encountered an unrecoverable error[/]");
    AnsiConsole.Render(rule);
    AnsiConsole.WriteException(exception, ExceptionFormats.ShortenEverything);
    AnsiConsole.Render(rule);

    return 1;
}