namespace Outermind;

public abstract class MPart
{
    public abstract MPartType PartType { get; }

    protected internal virtual MPart Accept(MPartVisitor visitor) =>
        visitor.VisitExtension(this);

    protected internal virtual MPart VisitChildren(MPartVisitor visitor) =>
        throw new InvalidOperationException($"Expected extension to override {nameof(VisitChildren)}");
}
