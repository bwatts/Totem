using System;

namespace Totem.Http;

public static class ContentTypes
{
    public const string Json = "application/json";
    public const string PlainText = "text/plain";

    public static bool IsJson(string contentType) =>
        Json.Equals(contentType, StringComparison.OrdinalIgnoreCase);

    public static bool IsPlainText(string contentType) =>
        PlainText.Equals(contentType, StringComparison.OrdinalIgnoreCase);
}
