using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Totem.Queues;
using Totem.Routes;
using Totem.Timelines;
using Totem.Workflows;

namespace Totem.InMemory
{
    public class InMemoryRouteStore : IRouteStore
    {
        readonly ConcurrentDictionary<Id, RouteHistory> _historiesById = new();
        readonly IServiceProvider _services;
        readonly IQueueClient _queueClient;

        public InMemoryRouteStore(IServiceProvider services, IQueueClient queueClient)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _queueClient = queueClient ?? throw new ArgumentNullException(nameof(queueClient));
        }

        public Task<IRouteStoreTransaction> StartTransactionAsync(IRouteContext<IEvent> context, CancellationToken cancellationToken)
        {
            if(context == null)
                throw new ArgumentNullException(nameof(context));

            var route = (IRoute) _services.GetRequiredService(context.RouteType);
            var version = 0L;

            if(_historiesById.TryGetValue(context.RouteId, out var history))
            {
                version = history.Load(route);
            }

            route.Load(context.Event, version);

            return Task.FromResult<IRouteStoreTransaction>(new RouteStoreTransaction(this, route, context, cancellationToken));
        }

        public async Task CommitAsync(IRouteStoreTransaction transaction, CancellationToken cancellationToken)
        {
            if(transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            await EnqueueNewPointsIfWorkflow(transaction, cancellationToken);

            AddOrUpdateHistory(transaction);
        }

        async Task EnqueueNewPointsIfWorkflow(IRouteStoreTransaction transaction, CancellationToken cancellationToken)
        {
            if(transaction.Route is not IWorkflow workflow || !workflow.HasNewCommands)
            {
                return;
            }

            var newPoints =
                from newCommand in workflow.NewCommands
                select newCommand.CorrelationId == Workflow.RouteCorrelationId
                    ? newCommand.Message.InEnvelope(transaction.Context.CorrelationId, newCommand.Principal)
                    : newCommand;

            await _queueClient.EnqueueAsync(newPoints, cancellationToken);
        }

        void AddOrUpdateHistory(IRouteStoreTransaction transaction) =>
            _historiesById.AddOrUpdate(
                transaction.Context.RouteId,
                _ => new RouteHistory(transaction),
                (_, history) => history.Update(transaction));

        class RouteHistory
        {
            readonly List<IEvent> _events = new();
            readonly string _routeType;
            readonly string _routeId;

            internal RouteHistory(IRouteStoreTransaction transaction)
            {
                _routeType = transaction.Context.RouteType.Name;
                _routeId = transaction.Context.RouteId.ToShortString();

                var version = transaction.Route.Version ?? 0;

                if(version != 0)
                    throw new UnexpectedVersionException($"Expected route {_routeType}/{_routeId} to not exist, but found @{version}");
            }

            internal long Load(IRoute route)
            {
                var version = 0L;

                foreach(var e in _events)
                {
                    route.Load(e, version++);
                }

                return version;
            }

            internal RouteHistory Update(IRouteStoreTransaction transaction)
            {
                var expectedVersion = _events.Count;

                if(transaction.Route.Version != expectedVersion)
                    throw new UnexpectedVersionException($"Expected route {_routeType}/{_routeId}@{expectedVersion}, but received @{transaction.Route.Version}");

                _events.Add(transaction.Context.Event);

                return this;
            }
        }
    }
}