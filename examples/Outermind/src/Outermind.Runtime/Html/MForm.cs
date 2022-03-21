namespace Outermind.Html;

public class MForm : MElement
{
    internal MForm(MGlobals? globals, string? name = null, string? method = null, string? action = null, string? encodingType = null, string? relationship = null)
         : base(MNodeType.Form, globals)
    {
        Name = name ?? "";
        Method = method ?? "";
        Action = action ?? "";
        EncodingType = encodingType ?? "";
        Relationship = relationship ?? "";
    }

    public string Name { get; }
    public string Method { get; }
    public string Action { get; }
    public string EncodingType { get; }
    public string Relationship { get; }

    protected internal override MNode Accept(MNodeVisitor visitor) =>
        visitor.VisitForm(this);

    public override MElement Rewrite(MGlobals globals) =>
        globals == Globals ? this : new(globals, Name, Method, Action, EncodingType, Relationship);
}
