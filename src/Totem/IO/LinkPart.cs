namespace Totem.IO
{
  /// <summary>
  /// A composable element of a link. A part may represent a template - a pattern for producing links.
  /// </summary>
  public abstract class LinkPart
  {
    public abstract bool IsTemplate { get; }

    public abstract override string ToString();
  }
}