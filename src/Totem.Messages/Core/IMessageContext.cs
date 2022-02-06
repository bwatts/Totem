using System.Collections.Generic;
using System.Security.Claims;

namespace Totem.Core;

public interface IMessageContext
{
    Id PipelineId { get; }
    IMessageEnvelope Envelope { get; }
    MessageInfo Info { get; }
    Id CorrelationId { get; }
    ClaimsPrincipal Principal { get; }
    ErrorBag Errors { get; }
    bool HasErrors { get; }

    void AddError(ErrorInfo error);
    void AddErrors(IEnumerable<ErrorInfo> errors);
    void AddErrors(params ErrorInfo[] errors);
}
