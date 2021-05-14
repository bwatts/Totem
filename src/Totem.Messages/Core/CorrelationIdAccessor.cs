using System.Threading;

namespace Totem.Core
{
    public class CorrelationIdAccessor : ICorrelationIdAccessor
    {
        // Adapted from https://github.com/aspnet/HttpAbstractions/blob/master/src/Microsoft.AspNetCore.Http/HttpContextAccessor.cs

        static readonly AsyncLocal<CorrelationIdHolder> _holder = new();

        public Id? CorrelationId
        {
            get => _holder.Value?.CorrelationId;
            set
            {
                var holder = _holder.Value;

                if(holder != null)
                {
                    holder.CorrelationId = null;
                }

                if(value != null)
                {
                    _holder.Value = new CorrelationIdHolder { CorrelationId = value };
                }
            }
        }

        class CorrelationIdHolder
        {
            public Id? CorrelationId;
        }
    }
}