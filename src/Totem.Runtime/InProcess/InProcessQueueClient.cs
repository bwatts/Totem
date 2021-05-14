using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Totem.Core;
using Totem.Queues;

namespace Totem.InProcess
{
    public sealed class InProcessQueueClient : IQueueClient, IDisposable
    {
        readonly ConcurrentDictionary<string, InProcessQueue> _queuesByName = new();
        readonly ILoggerFactory _loggerFactory;
        readonly IQueuePipeline _pipeline;

        public InProcessQueueClient(ILoggerFactory loggerFactory, IQueuePipeline pipeline)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
        }

        public Task EnqueueAsync(IQueueEnvelope envelope, CancellationToken cancellationToken)
        {
            if(envelope == null)
                throw new ArgumentNullException(nameof(envelope));

            return GetOrAddQueue(envelope).EnqueueAsync(envelope, cancellationToken);
        }

        public async Task EnqueueAsync(IEnumerable<IQueueEnvelope> envelopes, CancellationToken cancellationToken)
        {
            if(envelopes == null)
                throw new ArgumentNullException(nameof(envelopes));

            foreach(var envelope in envelopes)
            {
                await GetOrAddQueue(envelope).EnqueueAsync(envelope, cancellationToken);
            }
        }

        public void Dispose()
        {
            foreach(var queue in _queuesByName.Values)
            {
                queue.Dispose();
            }
        }

        InProcessQueue GetOrAddQueue(IQueueEnvelope envelope) =>
            _queuesByName.GetOrAdd(envelope.QueueName, name => new(_loggerFactory.CreateLogger<InProcessQueue>(), _pipeline, name));
    }
}