using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Dream.Hosting
{
    public class ConsoleService : IHostedService
    {
        readonly IConsoleRouter _router;

        public ConsoleService(IConsoleRouter router) =>
            _router = router ?? throw new ArgumentNullException(nameof(router));

        public Task StartAsync(CancellationToken cancellationToken) =>
            _router.NavigateAsync("/", cancellationToken);

        public Task StopAsync(CancellationToken cancellationToken) =>
            Task.CompletedTask;
    }
}