using System.Collections.Generic;
using System.Security.Claims;

namespace Totem.Core
{
    public interface IMessageContext
    {
        Id PipelineId { get; }
        IMessageEnvelope Envelope { get; }
        Id CorrelationId { get; set; }
        ClaimsPrincipal Principal { get; set; }
        ErrorBag Errors { get; }
        bool HasErrors { get; }

        void AddError(ErrorInfo error);
        void AddErrors(IEnumerable<ErrorInfo> errors);
        void AddErrors(params ErrorInfo[] errors);
    }
}