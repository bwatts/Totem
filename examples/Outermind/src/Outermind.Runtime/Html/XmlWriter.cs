using System.Xml.Linq;

namespace Outermind.Html;

internal class XmlWriter : MNodeVisitor
{
    readonly Stack<XElement> _elements = new();
    XElement _element = null!;

    internal XElement ToXml(MElement element)
    {
        Visit(element);

        PopElement();

        return _element;
    }

    protected internal override MNode VisitArticle(MElement node) =>
        PushElement("article", node);

    protected internal override MNode VisitAside(MElement node) =>
        PushElement("aside", node);

    protected internal override MNode VisitCitation(MElement node) =>
        PushElement("cite", node);

    protected internal override MNode VisitCode(MElement node) =>
        PushElement("code", node);

    protected internal override object? VisitContentValue(object? value)
    {
        if(value is MElement element)
        {
            AddContent(element.ToXml());
        }
        else if(value is IReadOnlyList<object?> list)
        {
            foreach(var item in list)
            {
                VisitContentValue(item);
            }
        }
        else
        {
            AddContent(value);
        }

        return value;
    }

    protected internal override MNode VisitData(MData node)
    {
        foreach(var item in node)
        {
            SetAttribute(item.Key, item.Value);
        }

        return node;
    }

    protected internal override MNode VisitDescription(MElement node) =>
        PushElement("dd", node);

    protected internal override MNode VisitDescriptionList(MElement node) =>
        PushElement("dl", node);

    protected internal override MNode VisitDescriptionTerm(MElement node) =>
        PushElement("dt", node);

    protected internal override MNode VisitDivision(MElement node) =>
        PushElement("div", node);

    protected internal override MNode VisitDocument(MDocument node)
    {
        PushElement(new XElement("html", node.Head.ToXml(), node.Body.ToXml()));

        return node;
    }

    protected internal override MNode VisitDocumentBody(MElement node) =>
        PushElement("body", node);

    protected internal override MNode VisitDocumentHead(MDocumentHead node)
    {
        PushElement("head", node);

        AddContent(new XElement("title", node.Title));

        if(!string.IsNullOrWhiteSpace(node.BaseHref))
        {
            AddContent(new XElement("base", new XAttribute("href", node.BaseHref)));
        }

        foreach(var link in node.Links)
        {
            Visit(link);

            PopElement();

            AddContent(_element);
        }

        return node;
    }

    protected internal override MNode VisitDocumentLink(MDocumentLink node)
    {
        PushElement("link", node);

        SetAttribute("href", node.Href);
        SetAttribute("hrefLanguage", node.HrefLanguage);
        SetAttribute("rel", node.Relationship);
        SetAttribute("type", node.ContentType);

        return node;
    }

    protected internal override MNode VisitFooter(MElement node) =>
        PushElement("footer", node);

    protected internal override MNode VisitForm(MForm node)
    {
        PushElement("form", node);

        SetAttribute("name", node.Name);
        SetAttribute("method", node.Method);
        SetAttribute("action", node.Action);
        SetAttribute("enctype", node.EncodingType);
        SetAttribute("rel", node.Relationship);

        return node;
    }

    protected internal override MNode VisitGlobals(MGlobals node)
    {
        SetAttribute("class", node.Classes);

        Visit(node.Data);
        VisitContent(node.Content);

        return node;
    }

    protected internal override MNode VisitHeader(MElement node) =>
        PushElement("header", node);

    protected internal override MNode VisitHeading1(MElement node) =>
        PushElement("h1", node);

    protected internal override MNode VisitHeading2(MElement node) =>
        PushElement("h2", node);

    protected internal override MNode VisitHeading3(MElement node) =>
        PushElement("h3", node);

    protected internal override MNode VisitHeading4(MElement node) =>
        PushElement("h4", node);

    protected internal override MNode VisitHeading5(MElement node) =>
        PushElement("h5", node);

    protected internal override MNode VisitHeading6(MElement node) =>
        PushElement("h6", node);

    protected internal override MNode VisitLink(MLink node)
    {
        PushElement("a", node);

        SetAttribute("href", node.Href);
        SetAttribute("hrefLanguage", node.HrefLanguage);
        SetAttribute("contentType", node.ContentType);
        SetAttribute("relationship", node.Relationship);
        SetAttribute("download", node.Download);

        return node;
    }

    protected internal override MNode VisitList(MElement node) =>
        PushElement("header", node);

    protected internal override MNode VisitListItem(MElement node) =>
        PushElement("header", node);

    protected internal override MNode VisitLiteral(MElement node) =>
        PushElement("pre", node);

    protected internal override MNode VisitMain(MElement node) =>
        PushElement("main", node);

    protected internal override MNode VisitNavigation(MElement node) =>
        PushElement("nav", node);

    protected internal override MNode VisitParagraph(MElement node) =>
        PushElement("p", node);

    protected internal override MNode VisitQuotation(MQuotation node)
    {
        PushElement("blockquote", node);

        SetAttribute("cite", node.Cite);

        return node;
    }

    protected internal override MNode VisitSection(MElement node) =>
        PushElement("section", node);

    protected internal override MNode VisitSpan(MElement node) =>
        PushElement("span", node);

    protected internal override MNode VisitTime(MTime node)
    {
        PushElement("time", node);

        SetAttribute("datetime", node.DateTime);

        return node;
    }

    void PushElement(XElement element)
    {
        _elements.Push(element);
        _element = element;
    }

    MElement PushElement(string tagName, MElement element)
    {
        PushElement(new XElement(tagName));

        Visit(element.Globals);

        return element;
    }

    void AddContent(object? content) =>
        _element.Add(content);

    void SetAttribute(string name, string value)
    {
        if(!string.IsNullOrEmpty(value))
        {
            _element.SetAttributeValue(name, value);
        }
    }

    void PopElement() =>
        _element = _elements.Pop();
}
