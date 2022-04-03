namespace Totem.Map.Builder;

internal class MapCheck<TOutput> : IMapCheck
{
    internal MapCheck(object input, TOutput output, IReadOnlyList<IMapCheck> details)
    {
        HasOutput = true;
        Expected = "";
        Input = input;
        Output = output;
        Details = details;
    }

    internal MapCheck(object input, string expected, IReadOnlyList<IMapCheck> details)
    {
        HasOutput = false;
        Expected = expected;
        Input = input;
        Output = default;
        Details = details;
    }

    internal MapCheck(object input, TOutput output, params IMapCheck[] details)
        : this(input, output, details as IReadOnlyList<IMapCheck>)
    { }

    internal MapCheck(object input, string expected, params IMapCheck[] details)
        : this(input, expected, details as IReadOnlyList<IMapCheck>)
    { }

    public object Input { get; }
    public Type OutputType => typeof(TOutput);
    [MemberNotNullWhen(true, nameof(Output))]
    public bool HasOutput { get; }
    public TOutput? Output { get; }
    public string Expected { get; }
    public IReadOnlyList<IMapCheck> Details { get; }

    object? IMapCheck.Output => Output!;

    public static implicit operator bool(MapCheck<TOutput> check) => check.HasOutput;
    public static implicit operator TOutput(MapCheck<TOutput> check) => check.Output!;
}
