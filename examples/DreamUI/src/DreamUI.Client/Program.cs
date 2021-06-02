using System.Threading.Tasks;
using DreamUI.Hosting;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace DreamUI
{
    public static class Program
    {
        public static Task Main(string[] args) =>
            WebAssemblyHostBuilder.CreateDefault(args)
                .ConfigureSerilog()
                .ConfigureTotem()
                .ConfigureDreamUI()
                .Build()
                .RunAsync();
    }
}