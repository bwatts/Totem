using OutermindUI.Hosting;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace OutermindUI;

public static class Program
{
    public static Task Main(string[] args) =>
        WebAssemblyHostBuilder.CreateDefault(args)
            .ConfigureSerilog()
            .ConfigureTotem()
            .ConfigureOutermindUI()
            .Build()
            .RunAsync();
}
