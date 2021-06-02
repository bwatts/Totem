namespace Totem.Local
{
    public interface ILocalCommandContextFactory
    {
        ILocalCommandContext<ILocalCommand> Create(Id pipelineId, ILocalCommandEnvelope envelope);
    }
}