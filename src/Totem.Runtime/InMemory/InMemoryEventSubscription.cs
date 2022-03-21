using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using Totem.Core;
using Totem.Events;

namespace Totem.InMemory;

public sealed class InMemoryEventSubscription : IInMemoryEventSubscription, IDisposable
{
    readonly Channel<IEventEnvelope> _channel = Channel.CreateUnbounded<IEventEnvelope>(new UnboundedChannelOptions
    {
        SingleReader = true
    });
    readonly CancellationTokenSource _cancellation = new();
    readonly ILogger _logger;
    readonly IEventPipeline _pipeline;

    public InMemoryEventSubscription(ILogger<InMemoryEventSubscription> logger, IEventPipeline pipeline)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));

        Task.Run(ObserveAsync);
    }

    public void Publish(IEventEnvelope envelope)
    {
        if(envelope is null)
            throw new ArgumentNullException(nameof(envelope));

        _logger.LogTrace("[event] Publish {@EventType}.{@EventId}", envelope.MessageKey.DeclaredType, envelope.MessageKey.Id);

        _channel.Writer.TryWrite(envelope);
    }

    public void Dispose()
    {
        _logger.LogTrace("[events] Close subscription");

        _channel.Writer.Complete();
        _cancellation.Cancel();
    }

    async Task ObserveAsync()
    {
        try
        {
            while(await _channel.Reader.WaitToReadAsync(_cancellation.Token))
            {
                if(_channel.Reader.TryRead(out var point))
                {
                    await RunPipelineAsync(point);
                }

                _logger.LogTrace("[events] Wait for next event...");
            }
        }
        catch(OperationCanceledException)
        { }
        catch(ChannelClosedException)
        { }
        catch(Exception exception)
        {
            _logger.LogError(exception, "[events] Unhandled exception; subscription closed");
        }
    }

    async Task RunPipelineAsync(IEventEnvelope envelope)
    {
        var context = await _pipeline.RunAsync(envelope, _cancellation.Token);

        if(context.HasErrors)
        {
            _logger.LogError("[event] Pipeline {@PipelineId} failed for {@EventType}.{@EventId}", _pipeline.Id, envelope.MessageKey.DeclaredType, envelope.MessageKey.Id);
        }
    }
}
