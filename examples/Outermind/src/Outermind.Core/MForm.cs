namespace Outermind;

public class MForm : MElement
{
    internal MForm(MGlobals? globals, string name, string method, string action, string encodingType, string relationship)
         : base(MPartType.Form, globals)
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

    protected internal override MPart Accept(MPartVisitor visitor) =>
        visitor.VisitForm(this);

    public override MElement Rewrite(MGlobals globals) =>
        globals == Globals ? this : new(globals, Name, Method, Action, EncodingType, Relationship);
}
