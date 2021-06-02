namespace Totem.Local
{
    public interface ILocalCommandEnvelope : ILocalMessageEnvelope
    {
        new ILocalCommand Message { get; }
        new LocalCommandInfo Info { get; }
    }
}