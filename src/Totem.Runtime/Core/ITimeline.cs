namespace Totem.Core;

public interface ITimeline
{
    Id? Id { get; set; }
    bool HasErrors { get; }
    IEnumerable<ErrorInfo> Errors { get; }
}
