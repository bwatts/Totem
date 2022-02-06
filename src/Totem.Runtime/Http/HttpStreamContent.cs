namespace Totem.Http;

public class HttpStreamContent
{
    public const string DefaultContentType = "application/json";
    public const string DefaultContentName = ".content";

    public HttpStreamContent(
        Stream stream,
        string contentType = DefaultContentType,
        string contentName = DefaultContentName)
    {
        Stream = stream ?? throw new ArgumentNullException(nameof(stream));
        ContentType = contentType ?? throw new ArgumentNullException(nameof(contentType));
        ContentName = contentName ?? throw new ArgumentNullException(nameof(contentName));
    }

    public Stream Stream { get; }
    public string ContentType { get; }
    public string ContentName { get; }
}
