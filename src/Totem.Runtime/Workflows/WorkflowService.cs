using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Totem.Features;
using Totem.Features.Default;
using Totem.Routes;
using Totem.Routes.Dispatch;

namespace Totem.Workflows
{
    public class WorkflowService : IWorkflowService
    {
        readonly IRouteStore _store;
        readonly IRouteDispatcher _dispatcher;
        readonly Lazy<Type[]> _workflowTypes;

        public WorkflowService(FeatureRegistry features, IRouteStore store, IRouteDispatcher dispatcher)
        {
            if(features == null)
                throw new ArgumentNullException(nameof(features));

            _store = store ?? throw new ArgumentNullException(nameof(store));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _workflowTypes = new(() => features.Populate<WorkflowFeature>().Workflows.ToArray());
        }

        public IEnumerable<IRouteSubscriber> Route(IEvent e) =>
            _workflowTypes.Value.SelectMany(type => _dispatcher.CallRoute(type, e));

        public async Task HandleAsync(IRouteContext<IEvent> context, CancellationToken cancellationToken)
        {
            using var transaction = await _store.StartTransactionAsync(context, cancellationToken);

            await _dispatcher.CallWhenAsync(transaction.Route, context, cancellationToken);

            await transaction.CommitAsync();
        }
    }
}