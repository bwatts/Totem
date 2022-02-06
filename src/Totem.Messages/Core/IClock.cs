using System;

namespace Totem.Core;

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}
