using System;
using Microsoft.Extensions.Logging;

namespace Totem.Routes
{
    public class RouteSubscriber : IRouteSubscriber
    {
        readonly ILoggerFactory _loggerFactory;
        readonly IRouteSettings _settings;
        readonly IRoutePipeline _pipeline;

        public RouteSubscriber(ILoggerFactory loggerFactory, IRouteSettings settings, IRoutePipeline pipeline, IRouteAddress address)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
            Address = address ?? throw new ArgumentNullException(nameof(address));
        }

        public IRouteAddress Address { get; }

        public IRouteSubscription Subscribe() =>
            new RouteSubscription(_loggerFactory.CreateLogger<RouteSubscription>(), _settings, _pipeline, Address);
    }
}