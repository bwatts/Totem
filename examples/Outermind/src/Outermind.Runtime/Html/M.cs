namespace Outermind.Html;

public static class M
{
    public static MElement Article(MGlobals? globals = null) =>
        new(MNodeType.Article, globals);

    public static MElement Aside(MGlobals? globals = null) =>
        new(MNodeType.Aside, globals);

    public static MElement Citation(MGlobals? globals = null) =>
        new(MNodeType.Citation, globals);

    public static MElement Code(MGlobals? globals = null) =>
        new(MNodeType.Code, globals);

    public static MContent Content(object? value) =>
        value as MContent ?? new(value);

    public static MContent Content(IEnumerable<object?> values) =>
        Content(values as IReadOnlyList<object?> ?? values?.ToArray() ?? Array.Empty<object?>());

    public static MContent Content(params object?[]? values) =>
        new(values);

    public static MData Data(IEnumerable<KeyValuePair<string, string>> pairs)
    {
        if(pairs is null)
            throw new ArgumentNullException(nameof(pairs));

        var data = new Dictionary<string, string>();

        foreach(var pair in pairs)
        {
            data.Add(MData.GetPrefixedKey(pair.Key), pair.Value);
        }

        return Data(data);
    }

    public static MData Data(IEnumerable<(string key, string value)> pairs)
    {
        if(pairs is null)
            throw new ArgumentNullException(nameof(pairs));

        var data = new Dictionary<string, string>();

        foreach(var (key, value) in pairs)
        {
            data.Add(MData.GetPrefixedKey(key), value);
        }

        return Data(data);
    }

    public static MData Data(params KeyValuePair<string, string>[] pairs) =>
        Data(pairs.AsEnumerable());

    public static MData Data(params (string Key, string Value)[] pairs) =>
        Data(pairs.AsEnumerable());

    public static MData Data(string key, string value) =>
        Data(new Dictionary<string, string>
        {
            { MData.GetPrefixedKey(key), value }
        });

    public static MElement Description(MGlobals? globals = null) =>
        new(MNodeType.Description, globals);

    public static MElement DescriptionList(MGlobals? globals = null) =>
        new(MNodeType.DescriptionList, globals);

    public static MElement DescriptionTerm(MGlobals? globals = null) =>
        new(MNodeType.DescriptionTerm, globals);

    public static MElement Division(MGlobals? globals = null) =>
        new(MNodeType.Division, globals);

    public static MDocument Document(MGlobals globals, MDocumentHead head, MElement body)
    {
        if(globals is null)
            throw new ArgumentNullException(nameof(globals));

        if(head is null)
            throw new ArgumentNullException(nameof(head));

        if(body is null)
            throw new ArgumentNullException(nameof(body));

        return new MDocument(globals, head, body);
    }

    public static MDocument Document(MDocumentHead head, MElement body) =>
        Document(MGlobals.Default, head, body);

    public static MElement DocumentBody(MGlobals? globals = null) =>
        new(MNodeType.DocumentBody, globals);

    public static MDocumentHead DocumentHead(MGlobals? globals = null, string? documentTitle = null, string? baseHref = null, IReadOnlyList<MDocumentLink>? links = null) =>
        new(globals, documentTitle, baseHref, links ?? Array.Empty<MDocumentLink>());

    public static MDocumentHead DocumentHead(MGlobals? globals = null, string? title = null, string? baseHref = null, params MDocumentLink[] links) =>
        new(globals, title, baseHref, links);

    public static MDocumentLink DocumentLink(MGlobals? globals = null, string? href = null, string? hrefLanguage = null, string? relationship = null, string? contentType = null) =>
        new(globals, href, hrefLanguage, relationship, contentType);

    public static MElement Footer(MGlobals? globals = null) =>
        new(MNodeType.Footer, globals);

    public static MForm Form(MGlobals? globals = null, string? name = null, string? method = null, string? action = null, string? encodingType = null, string? relationship = null) =>
        new(globals, name, method, action, encodingType, relationship);

    public static MGlobals Globals(
        string? id = null,
        string? title = null,
        string? classes = null,
        string? language = null,
        string? direction = null,
        MData? data = null,
        MContent? content = null) =>
        new(id, title, classes, language, direction, data, content);

    public static MElement Header(MGlobals? globals = null) =>
        new(MNodeType.Header, globals);

    public static MElement Heading1(MGlobals? globals = null) =>
        new(MNodeType.Heading1, globals);

    public static MElement Heading2(MGlobals? globals = null) =>
        new(MNodeType.Heading2, globals);

    public static MElement Heading3(MGlobals? globals = null) =>
        new(MNodeType.Heading3, globals);

    public static MElement Heading4(MGlobals? globals = null) =>
        new(MNodeType.Heading4, globals);

    public static MElement Heading5(MGlobals? globals = null) =>
        new(MNodeType.Heading5, globals);

    public static MElement Heading6(MGlobals? globals = null) =>
        new(MNodeType.Heading6, globals);

    public static MLink Link(MGlobals? globals = null, string? href = null, string? hrefLanguage = null, string? contentType = null, string? relationship = null, string? download = null) =>
        new(globals, href, hrefLanguage, relationship, contentType, download);

    public static MElement List(MGlobals? globals = null) =>
        new(MNodeType.List, globals);

    public static MElement ListItem(MGlobals? globals = null) =>
        new(MNodeType.ListItem, globals);

    public static MElement Literal(MGlobals? globals = null) =>
        new(MNodeType.Literal, globals);

#pragma warning disable 0028 // Wrong signature to be an entry point
    public static MElement Main(MGlobals? globals = null) =>
        new(MNodeType.Main, globals);
#pragma warning restore 0028

    public static MElement Navigation(MGlobals? globals = null) =>
        new(MNodeType.Navigation, globals);

    public static MElement Paragraph(MGlobals? globals = null) =>
        new(MNodeType.Paragraph, globals);

    public static MQuotation Quotation(MGlobals? globals = null, string? cite = null) =>
        new(globals, cite);

    public static MElement Section(MGlobals? globals = null) =>
        new(MNodeType.Section, globals);

    public static MElement Span(MGlobals? globals = null) =>
        new(MNodeType.Span, globals);

    public static MTime Time(MGlobals? globals = null, string? dateTime = null) =>
        new(globals, dateTime);
}
