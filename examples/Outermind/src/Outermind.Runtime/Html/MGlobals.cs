namespace Outermind.Html;

public class MGlobals : MNode
{
    public static readonly MGlobals Default = new();

    internal MGlobals(
        string? id = null,
        string? title = null,
        string? classes = null,
        string? language = null,
        string? direction = null,
        MData? data = null,
        MContent? content = null)
    {
        Id = id ?? "";
        Title = title ?? "";
        Classes = classes ?? "";
        Language = language ?? "";
        Direction = direction ?? "";
        Data = data ?? MData.None;
        Content = content ?? MContent.None;
    }

    public override MNodeType NodeType => MNodeType.Globals;
    public string Id { get; }
    public string Title { get; }
    public string Classes { get; }
    public string Language { get; }
    public string Direction { get; }
    public MData Data { get; }
    public MContent Content { get; }

    protected internal override MNode Accept(MNodeVisitor visitor) =>
        visitor.VisitGlobals(this);

    public MGlobals Rewrite(MContent content) =>
        content == Content ? this : new(Id, Title, Classes, Language, Direction, Data, content);
}
