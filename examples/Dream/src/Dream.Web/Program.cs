using Dream;
using Dream.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

await Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(host => host.UseStartup<Startup>())
    .ConfigureSerilog()
    .Build()
    .RunAsync();