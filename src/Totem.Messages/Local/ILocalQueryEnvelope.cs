namespace Totem.Local
{
    public interface ILocalQueryEnvelope : ILocalMessageEnvelope
    {
        new ILocalQuery Message { get; }
        new LocalQueryInfo Info { get; }
    }
}