using System.Collections.Generic;

namespace Totem.Core;

public interface ITimeline
{
    Id Id { get; set; }
    long? Version { get; set; }
    bool HasErrors { get; }
    IEnumerable<ErrorInfo> Errors { get; }
}
