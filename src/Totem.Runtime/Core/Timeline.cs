using System.Collections.Concurrent;

namespace Totem.Core;

public abstract class Timeline : ITimeline
{
    readonly ConcurrentQueue<ErrorInfo> _errors = new();
    Id? _id;
    long? _version;

    public bool HasErrors => !_errors.IsEmpty;
    public IEnumerable<ErrorInfo> Errors => _errors;

    public Id Id
    {
        get => _id ?? throw new InvalidOperationException($"{nameof(Id)} property has not been initialized by timeline {GetType()}");
        private set => _id = value;
    }

    public long? Version
    {
        get => _version;
        private set => _version = value;
    }

    Id ITimeline.Id
    {
        get => Id;
        set => Id = value;
    }

    long? ITimeline.Version
    {
        get => Version;
        set => Version = value;
    }

    protected void ThenError(ErrorInfo error)
    {
        if(error is null)
            throw new ArgumentNullException(nameof(error));

        _errors.Enqueue(error);
    }

    protected void ThenErrors(IEnumerable<ErrorInfo> errors)
    {
        if(errors is null)
            throw new ArgumentNullException(nameof(errors));

        foreach(var error in errors)
        {
            _errors.Enqueue(error);
        }
    }

    protected void ThenErrors(params ErrorInfo[] errors) =>
        ThenErrors(errors.AsEnumerable());
}
