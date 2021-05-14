namespace Totem.Core
{
    public interface ICorrelationIdAccessor
    {
        Id? CorrelationId { get; }
    }
}