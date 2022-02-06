using System.Security.Claims;

namespace Totem.Core;

public abstract class MessageContext : IMessageContext
{
    protected MessageContext(Id pipelineId, IMessageEnvelope envelope)
    {
        PipelineId = pipelineId ?? throw new ArgumentNullException(nameof(pipelineId));
        Envelope = envelope ?? throw new ArgumentNullException(nameof(envelope));
    }

    public Id PipelineId { get; }
    public IMessageEnvelope Envelope { get; }
    public MessageInfo Info => Envelope.Info;
    public Id CorrelationId => Envelope.CorrelationId;
    public ClaimsPrincipal Principal => Envelope.Principal;
    public ErrorBag Errors { get; } = new();
    public bool HasErrors => Errors.Any;

    public void AddError(ErrorInfo error) =>
        Errors.Add(error ?? throw new ArgumentNullException(nameof(error)));

    public void AddErrors(IEnumerable<ErrorInfo> errors) =>
        AddErrors(errors?.ToArray() ?? Array.Empty<ErrorInfo>());

    public void AddErrors(params ErrorInfo[] errors)
    {
        if(errors is null || errors.Length == 0)
        {
            Errors.Add(ErrorInfo.General);
            return;
        }

        foreach(var error in errors)
        {
            Errors.Add(error);
        }
    }
}
