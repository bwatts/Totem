using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Totem.Core;

namespace Totem.Routes
{
    public sealed class RouteSubscription : IRouteSubscription
    {
        readonly ILogger _logger;
        readonly IRoutePipeline _pipeline;
        readonly IRouteAddress _address;
        readonly Channel<IEventEnvelope> _channel;
        readonly CancellationTokenSource _cancellation = new();

        public RouteSubscription(ILogger<RouteSubscription> logger, IRouteSettings settings, IRoutePipeline pipeline, IRouteAddress address)
        {
            if(settings == null)
                throw new ArgumentNullException(nameof(settings));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
            _address = address ?? throw new ArgumentNullException(nameof(address));
            _channel = Channel.CreateBounded<IEventEnvelope>(new BoundedChannelOptions(settings.SubscriptionCapacity)
            {
                SingleReader = true
            });

            Task.Run(ObserveAsync);
        }

        public async Task EnqueueAsync(IEventEnvelope envelope, CancellationToken cancellationToken)
        {
            if(envelope == null)
                throw new ArgumentNullException(nameof(envelope));

            await _channel.Writer.WriteAsync(envelope, cancellationToken);
        }

        public void Dispose()
        {
            _logger.LogTrace("[route] Close subscription {RouteType}.{RouteId}", _address.RouteType, _address.RouteId);

            _channel.Writer.Complete();
            _cancellation.Cancel();
        }

        async Task ObserveAsync()
        {
            try
            {
                while(await _channel.Reader.WaitToReadAsync(_cancellation.Token))
                {
                    if(_channel.Reader.TryRead(out var envelope))
                    {
                        await RunPipelineAsync(envelope);
                    }

                    _logger.LogTrace("[route] Wait for next event to {RouteType}.{RouteId}...", _address.RouteType, _address.RouteId);
                }
            }
            catch(OperationCanceledException)
            { }
            catch(ChannelClosedException)
            { }
            catch(Exception exception)
            {
                _logger.LogError(exception, "[route] Unhandled exception; subscription closed for {RouteType}.{RouteId}", _address.RouteType, _address.RouteId);
            }
        }

        async Task RunPipelineAsync(IEventEnvelope envelope)
        {
            var context = await _pipeline.RunAsync(envelope, _address, _cancellation.Token);

            if(context.HasErrors)
            {
                _logger.LogError("[route] Pipeline {PipelineId} failed for {RouteType}.{RouteId} and event {EventType}.{EventId} from {TimelineType}.{TimelineId}@{TimelineVersion}", _pipeline.Id, _address.RouteType, _address.RouteId, envelope.MessageType, envelope.MessageId, envelope.TimelineType, envelope.TimelineId, envelope.TimelineVersion);
            }
        }
    }
}