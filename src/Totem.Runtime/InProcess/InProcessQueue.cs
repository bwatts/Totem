using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Totem.Core;
using Totem.Queues;

namespace Totem.InProcess
{
    public sealed class InProcessQueue : IDisposable
    {
        readonly Channel<IQueueEnvelope> _channel = Channel.CreateUnbounded<IQueueEnvelope>(new UnboundedChannelOptions
        {
            SingleReader = true
        });
        readonly CancellationTokenSource _cancellation = new();
        readonly ILogger _logger;
        readonly IQueuePipeline _pipeline;
        readonly string _name;

        public InProcessQueue(ILogger<InProcessQueue> logger, IQueuePipeline pipeline, string name)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
            _name = name ?? throw new ArgumentNullException(nameof(name));

            Task.Run(ObserveAsync);
        }

        public async Task EnqueueAsync(IQueueEnvelope point, CancellationToken cancellationToken)
        {
            if(point == null)
                throw new ArgumentNullException(nameof(point));

            _logger.LogTrace("[queue {QueueName}] Enqueue {CommandType}", _name, point.MessageType);

            await _channel.Writer.WriteAsync(point, cancellationToken);
        }

        public async Task EnqueueAsync(IEnumerable<IQueueEnvelope> points, CancellationToken cancellationToken)
        {
            if(points == null)
                throw new ArgumentNullException(nameof(points));

            foreach(var point in points)
            {
                await EnqueueAsync(point, cancellationToken);
            }
        }

        public void Dispose()
        {
            _logger.LogTrace("[queue {QueueName}] Close subscription", _name);

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

                    _logger.LogTrace("[queue {QueueName}] Wait for next command...", _name);
                }
            }
            catch(OperationCanceledException)
            { }
            catch(ChannelClosedException)
            { }
            catch(Exception exception)
            {
                _logger.LogError(exception, "[queue {QueueName}] Unhandled exception; subscription closed", _name);
            }
        }

        async Task RunPipelineAsync(IQueueEnvelope envelope)
        {
            var context = await _pipeline.RunAsync(envelope, _cancellation.Token);

            if(context.HasErrors)
            {
                _logger.LogError("[queue {QueueName}] Command {CommandType}.{CommandId} has errors: {Errors}", _name, envelope.MessageType, envelope.MessageId, context.Errors);
            }
        }
    }
}