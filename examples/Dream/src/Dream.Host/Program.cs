using Dream;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

await Host.CreateDefaultBuilder(args)
    .ConfigureSerilog()
    .ConfigureWebHostDefaults(webHost => webHost.UseStartup<Startup>())
    .Build()
    .RunAsync();