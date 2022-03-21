using Outermind;
using Outermind.Hosting;

await Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(host => host.UseStartup<Startup>())
    .ConfigureSerilog()
    .Build()
    .RunAsync();
