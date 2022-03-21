using System.Xml.Linq;

namespace Outermind.Html;

internal static class XmlReader
{
    internal static MDocument Read(XDocument html)
    {
        var root = html.Root ?? throw new ArgumentException("Expected document to have a root", nameof(html));

        if(root.Name != "html")
        {
            throw new ArgumentException($"Expected root to have name 'html': {root.Name}", nameof(html));
        }

        return M.Document(root.ReadGlobals(), root.ReadDocumentHead(), root.ReadDocumentBody());
    }

    static MGlobals ReadGlobals(this XElement element) =>
        M.Globals(
            element.AttributeValue("id"),
            element.AttributeValue("title"),
            element.AttributeValue("class"),
            element.AttributeValue("lang"),
            element.AttributeValue("dir"),
            element.ReadData(),
            element.ReadContent());

    static MData ReadData(this XElement element) =>
        M.Data(
            from attribute in element.Attributes()
            let name = attribute.Name.ToString()
            where name.Length > 5 && name.StartsWith("data-")
            select (attribute.Name.ToString(), (string) attribute));

    static MContent ReadContent(this XElement element)
    {
        var items = new List<object?>();

        foreach(var node in element.Nodes())
        {
            if(node is XElement child)
            {
                items.Add(child.ReadElement());
            }
            else
            {
                if(node is XText text)
                {
                    items.Add(text.ToString());
                }
            }
        }

        return items.Count switch
        {
            0 => MContent.None,
            1 => M.Content(items[0]),
            _ => M.Content(items)
        };
    }

    static MDocumentHead ReadDocumentHead(this XElement root)
    {
        var head = root.Element("head");

        if(head is null)
            throw new ArgumentException("Expected root to have child element 'head'", nameof(root));

        var links =
            from link in root.Elements("link")
            select M.DocumentLink(
                link.ReadGlobals(),
                link.AttributeValue("href"),
                link.AttributeValue("hrefLanguage"),
                link.AttributeValue("relationship"),
                link.AttributeValue("type"));

        return M.DocumentHead(
            head.ReadGlobals(),
            head.Element("title")?.Value() ?? "",
            head.Element("base")?.Value() ?? "/",
            links.ToList());
    }

    static MElement ReadDocumentBody(this XElement root)
    {
        var body = root.Element("body");

        if(body is null)
            throw new ArgumentException("Expected root to have child element 'body'", nameof(root));

        return body.ReadElement(MNodeType.DocumentBody);
    }

    static MElement ReadElement(this XElement element, MNodeType nodeType) =>
        new(nodeType, element.ReadGlobals());

    static MElement ReadElement(this XElement element) =>
        element.Name.ToString() switch
        {
            "article" => element.ReadElement(MNodeType.Article),
            "aside" => element.ReadElement(MNodeType.Aside),
            "cite" => element.ReadElement(MNodeType.Citation),
            "code" => element.ReadElement(MNodeType.Code),
            "dd" => element.ReadElement(MNodeType.Description),
            "div" => element.ReadElement(MNodeType.Division),
            "dl" => element.ReadElement(MNodeType.DescriptionList),
            "dt" => element.ReadElement(MNodeType.DescriptionTerm),
            "footer" => element.ReadElement(MNodeType.Footer),
            "form" => element.ReadForm(),
            "header" => element.ReadElement(MNodeType.Header),
            "h1" => element.ReadElement(MNodeType.Heading1),
            "h2" => element.ReadElement(MNodeType.Heading2),
            "h3" => element.ReadElement(MNodeType.Heading3),
            "h4" => element.ReadElement(MNodeType.Heading4),
            "h5" => element.ReadElement(MNodeType.Heading5),
            "h6" => element.ReadElement(MNodeType.Heading6),
            "a" => element.ReadLink(),
            "ul" or "ol" => element.ReadElement(MNodeType.List),
            "li" => element.ReadElement(MNodeType.ListItem),
            "pre" => element.ReadElement(MNodeType.Literal),
            "main" => element.ReadElement(MNodeType.Main),
            "nav" => element.ReadElement(MNodeType.Navigation),
            "p" => element.ReadElement(MNodeType.Paragraph),
            "blockquote" => element.ReadQuotation(),
            "section" => element.ReadElement(MNodeType.Section),
            "span" => element.ReadElement(MNodeType.Span),
            "time" => element.ReadTime(),
            _ => throw new NotSupportedException($"Unsupported element name: {element.Name}")
        };

    static MForm ReadForm(this XElement element) =>
        M.Form(
            element.ReadGlobals(),
            element.AttributeValue("name"),
            element.AttributeValue("action"),
            element.AttributeValue("enctype"));

    static MLink ReadLink(this XElement element) =>
        M.Link(
            element.ReadGlobals(),
            element.AttributeValue("href"),
            element.AttributeValue("hrefLanguage"),
            element.AttributeValue("rel"),
            element.AttributeValue("type"));

    static MQuotation ReadQuotation(this XElement element) =>
        M.Quotation(element.ReadGlobals(), element.AttributeValue("cite"));

    static MTime ReadTime(this XElement element) =>
        M.Time(element.ReadGlobals(), element.AttributeValue("datetime"));

    static string Value(this XElement? element) =>
        (string?) element ?? "";

    static string AttributeValue(this XElement? element, XName attribute) =>
        (string?) element?.Attribute(attribute) ?? "";
}
