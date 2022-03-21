namespace Outermind.Html;

public abstract class MNode
{
    public abstract MNodeType NodeType { get; }

    protected internal virtual MNode Accept(MNodeVisitor visitor) =>
        visitor.VisitExtension(this);

    protected internal virtual MNode VisitChildren(MNodeVisitor visitor) =>
        throw new InvalidOperationException($"Expected extension to override {nameof(VisitChildren)}");
}
