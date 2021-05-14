using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Totem.Core
{
    public abstract class MessageContext : IMessageContext
    {
        protected MessageContext(Id pipelineId, IMessageEnvelope envelope)
        {
            PipelineId = pipelineId ?? throw new ArgumentNullException(nameof(pipelineId));
            Envelope = envelope ?? throw new ArgumentNullException(nameof(envelope));
            CorrelationId = envelope.CorrelationId;
            Principal = envelope.Principal;
        }

        public Id PipelineId { get; }
        public IMessageEnvelope Envelope { get; }
        public Id CorrelationId { get; set; }
        public ClaimsPrincipal Principal { get; set; }
        public ErrorBag Errors { get; } = new();
        public bool HasErrors => Errors.Any;

        public void AddError(ErrorInfo error) =>
            Errors.Add(error ?? throw new ArgumentNullException(nameof(error)));

        public void AddErrors(IEnumerable<ErrorInfo> errors) =>
            AddErrors(errors?.ToArray() ?? Array.Empty<ErrorInfo>());

        public void AddErrors(params ErrorInfo[] errors)
        {
            if(errors == null || errors.Length == 0)
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
}