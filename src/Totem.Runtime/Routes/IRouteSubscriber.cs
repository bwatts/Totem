namespace Totem.Routes
{
    public interface IRouteSubscriber
    {
        IRouteAddress Address { get; }

        IRouteSubscription Subscribe();
    }
}