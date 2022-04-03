using OutermindUI.Hosting;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

await WebAssemblyHostBuilder.CreateDefault(args)
    .ConfigureSerilog()
    .ConfigureTotem()
    .ConfigureOutermindUI()
    .Build()
    .RunAsync();
