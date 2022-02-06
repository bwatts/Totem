using Dream;
using Dream.Hosting;

await Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(host => host.UseStartup<Startup>())
    .ConfigureSerilog()
    .Build()
    .RunAsync();