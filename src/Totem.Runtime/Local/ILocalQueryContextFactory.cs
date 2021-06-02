namespace Totem.Local
{
    public interface ILocalQueryContextFactory
    {
        ILocalQueryContext<ILocalQuery> Create(Id pipelineId, ILocalQueryEnvelope envelope);
    }
}