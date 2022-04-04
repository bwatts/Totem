using Microsoft.Extensions.Hosting;

namespace Totem.Map.Builder;

public class FailedCheckLoggingService : IHostedService
{
    readonly ILogger<FailedCheckLoggingService> _logger;
    readonly RuntimeMap _map;

    public FailedCheckLoggingService(ILogger<FailedCheckLoggingService> logger, RuntimeMap map)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _map = map ?? throw new ArgumentNullException(nameof(map));
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        foreach(var failedCheck in _map.FailedChecks)
        {
            _logger.LogError(@"[map] Map type did not meet expectations
Input:    {Input}
Expected: {Expected}", failedCheck.Input, failedCheck.Expected);
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) =>
        Task.CompletedTask;
}
