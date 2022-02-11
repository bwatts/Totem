namespace Outermind;

public static class M
{
    public static MElement Article(MGlobals? globals = null) =>
        new(MPartType.Article, globals);

    public static MElement Aside(MGlobals? globals = null) =>
        new(MPartType.Aside, globals);

    public static MElement Citation(MGlobals? globals = null) =>
        new(MPartType.Citation, globals);

    public static MClasses Classes(IReadOnlyList<string> names)
    {
        if(names is null)
            throw new ArgumentNullException(nameof(names));

        return new MClasses(names);
    }

    public static MClasses Classes(IEnumerable<string> names)
    {
        if(names is null)
            throw new ArgumentNullException(nameof(names));

        return new MClasses(names.ToArray());
    }

    public static MClasses Classes(params string[] names)
    {
        if(names is null)
            throw new ArgumentNullException(nameof(names));

        return new MClasses(names);
    }

    public static MElement Code(MGlobals? globals = null) =>
        new(MPartType.Code, globals);

    public static MContent Content(object? value) =>
        value as MContent ?? new MContent(value);

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
        new(MPartType.Description, globals);

    public static MElement DescriptionList(MGlobals? globals = null) =>
        new(MPartType.DescriptionList, globals);

    public static MElement DescriptionTerm(MGlobals? globals = null) =>
        new(MPartType.DescriptionTerm, globals);

    public static MDocument Document(MDocumentHead head, MElement body, MGlobals? globals = null)
    {
        if(head is null)
            throw new ArgumentNullException(nameof(head));

        if(body is null)
            throw new ArgumentNullException(nameof(body));

        return new MDocument(globals, head, body);
    }

    public static MElement DocumentBody(MGlobals? globals = null) =>
        new(MPartType.DocumentBody, globals);

    public static MDocumentHead DocumentHead(string title = "", string baseHref = "", MGlobals? globals = null, IReadOnlyList<MDocumentLink>? links = null) =>
        new(globals, title, baseHref, links);

    public static MDocumentHead DocumentHead(string title = "", string baseHref = "", MGlobals? globals = null, params MDocumentLink[] links) =>
        new(globals, title, baseHref, links);

    public static MDocumentLink DocumentLink(MGlobals? globals = null, string href = "", string hrefLanguage = "", string relationship = "", string contentType = "") =>
        new(globals, href, hrefLanguage, relationship, contentType);

    public static MElement Footer(MGlobals? globals = null) =>
        new(MPartType.Footer, globals);

    public static MForm Form(MGlobals? globals = null, string name = "", string method = "", string action = "", string encodingType = "", string relationship = "") =>
        new(globals, name, method, action, encodingType, relationship);

    public static MGlobals Globals(
        string id = "",
        string title = "",
        string direction = "",
        string language = "",
        bool hidden = false,
        MClasses? classes = null,
        MData? data = null,
        MContent? content = null)
    {
        return new MGlobals(id, title, direction, language, hidden, classes, data, content);
    }

    public static MElement Header(MGlobals? globals = null) =>
        new(MPartType.Header, globals);

    public static MElement Heading1(MGlobals? globals = null) =>
        new(MPartType.Heading1, globals);

    public static MElement Heading2(MGlobals? globals = null) =>
        new(MPartType.Heading2, globals);

    public static MElement Heading3(MGlobals? globals = null) =>
        new(MPartType.Heading3, globals);

    public static MElement Heading4(MGlobals? globals = null) =>
        new(MPartType.Heading4, globals);

    public static MElement Heading5(MGlobals? globals = null) =>
        new(MPartType.Heading5, globals);

    public static MElement Heading6(MGlobals? globals = null) =>
        new(MPartType.Heading6, globals);

    public static MLink Link(MGlobals? globals = null, string href = "", string hrefLanguage = "", string contentType = "", string relationship = "", string download = "") =>
        new(globals, href, hrefLanguage, relationship, contentType, download);

    public static MElement List(MGlobals? globals = null) =>
        new(MPartType.List, globals);

    public static MElement ListItem(MGlobals? globals = null) =>
        new(MPartType.ListItem, globals);

    public static MElement Literal(MGlobals? globals = null) =>
        new(MPartType.Literal, globals);

#pragma warning disable 0028 // Wrong signature to be an entry point
    public static MElement Main(MGlobals? globals = null) =>
        new(MPartType.Main, globals);
#pragma warning restore 0028

    public static MElement Navigation(MGlobals? globals = null) =>
        new(MPartType.Navigation, globals);

    public static MElement Paragraph(MGlobals? globals = null) =>
        new(MPartType.Paragraph, globals);

    public static MQuotation Quotation(MGlobals? globals = null, string cite = "") =>
        new(globals, cite);

    public static MElement Section(MGlobals? globals = null) =>
        new(MPartType.Section, globals);

    public static MTime Time(MGlobals? globals = null, string cite = "") =>
        new(globals, cite);
}
