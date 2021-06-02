using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Totem.Queues;

namespace Totem.InMemory
{
    public sealed class InMemoryQueueCommandClient : IQueueCommandClient, IDisposable
    {
        readonly ConcurrentDictionary<string, InMemoryQueue> _queuesByName = new();
        readonly ILoggerFactory _loggerFactory;
        readonly IQueueCommandPipeline _pipeline;

        public InMemoryQueueCommandClient(ILoggerFactory loggerFactory, IQueueCommandPipeline pipeline)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
        }

        public Task EnqueueAsync(IQueueCommandEnvelope envelope, CancellationToken cancellationToken)
        {
            if(envelope == null)
                throw new ArgumentNullException(nameof(envelope));

            return GetOrAddQueue(envelope).EnqueueAsync(envelope, cancellationToken);
        }

        public async Task EnqueueAsync(IEnumerable<IQueueCommandEnvelope> envelopes, CancellationToken cancellationToken)
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

        InMemoryQueue GetOrAddQueue(IQueueCommandEnvelope envelope) =>
            _queuesByName.GetOrAdd(envelope.Info.QueueName, name => new(_loggerFactory.CreateLogger<InMemoryQueue>(), _pipeline, name));
    }
}