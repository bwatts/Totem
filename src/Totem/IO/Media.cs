namespace Totem.IO
{
  /// <summary>
  /// Content of type <see cref="TContent"/> identified by media type
  /// </summary>
  /// <typeparam name="TContent">The identified type of content</typeparam>
  public class Media<TContent>
  {
    public Media(MediaType type, TContent content)
    {
      Type = type;
      Content = content;
    }

    public readonly MediaType Type;
    public readonly TContent Content;

    public override string ToString() =>
      Text.Of(Content);

    public bool Is(MediaType type) =>
      type == Type;

    public Media<T> Cast<T>() where T : TContent =>
      new Media<T>(Type, (T) Content);
  }
}