namespace Outermind;

public class MGlobals : MPart
{
    public static readonly MGlobals Default = new();

    internal MGlobals(
        string id = "",
        string title = "",
        string language = "",
        string direction = "",
        bool hidden = false,
        MClasses? classes = null,
        MData? data = null,
        MContent? content = null)
    {
        Id = id ?? "";
        Title = title ?? "";
        Language = language ?? "";
        Direction = direction ?? "";
        Hidden = hidden;
        Classes = classes ?? MClasses.None;
        Data = data ?? MData.None;
        Content = content ?? MContent.None;
    }

    public override MPartType PartType => MPartType.Globals;
    public string Id { get; }
    public string Title { get; }
    public string Language { get; }
    public string Direction { get; }
    public bool Hidden { get; }
    public MClasses Classes { get; }
    public MData Data { get; }
    public MContent Content { get; }

    protected internal override MPart Accept(MPartVisitor visitor) =>
        visitor.VisitGlobals(this);

    public MGlobals Rewrite(MClasses classes, MData data, MContent content) =>
        classes == Classes && data == Data && content == Content
            ? this
            : new(Id, Title, Language, Direction, Hidden, classes, data, content);
}
