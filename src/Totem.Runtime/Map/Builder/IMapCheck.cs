namespace Totem.Map.Builder;

public interface IMapCheck
{
    object Input { get; }
    Type OutputType { get; }
    [MemberNotNullWhen(true, nameof(Output))]
    bool HasOutput { get; }
    object? Output { get; }
    string Expected { get; }
    IReadOnlyList<IMapCheck> Details { get; }
}
