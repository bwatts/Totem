using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dream.Areas.Home;
using Spectre.Console;
using Totem;

namespace Dream
{
    public class ConsoleRouter : IConsoleRouter
    {
        readonly Dictionary<string, IConsoleArea> _areasByRoute = new();


        // TODO: Resolve the area each time it's needed


        public ConsoleRouter(IAnsiConsole console, ITotemClient totemClient) =>
            _areasByRoute["/"] = new HomeArea(console, totemClient);

        public Task NavigateAsync(string route, CancellationToken cancellationToken)
        {
            if(string.IsNullOrWhiteSpace(route))
                throw new ArgumentOutOfRangeException(nameof(route));

            if(!_areasByRoute.TryGetValue(route, out var area))
                throw new ArgumentOutOfRangeException(nameof(route), $"Unsupported route: {route}");

            return Task.Run(() => area.NavigateAsync(cancellationToken), cancellationToken);
        }
    }
}