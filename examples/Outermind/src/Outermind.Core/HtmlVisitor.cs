namespace Outermind;

internal class HtmlVisitor : MPartVisitor
{
    readonly Stack<XElement> _elements = new();

    internal XNode ToHtml(MPart part)
    {
        Visit(part);

        return _elements.Pop();
    }

    XElement Element => _elements.Peek();

    protected internal override MPart VisitClasses(MClasses part)
    {
        SetAttribute("class", part.ToString());

        return part;
    }

    protected internal override MPart VisitContent(MContent part)
    {
        Element.Add(part.Value is MPart partContent ? ToHtml(partContent) : part.Value);

        return part;
    }

    protected internal override MPart VisitData(MData part)
    {
        foreach(var pair in part)
        {
            SetAttribute(pair.Key, pair.Value);
        }

        return part;
    }

    protected internal override MPart VisitDocument(MDocument part)
    {
        _elements.Push(new XElement("html", ToHtml(part.Head), ToHtml(part.Body)));

        return part;
    }

    protected internal override MPart VisitDocumentHead(MDocumentHead part)
    {
        PushElement("head", part);

        Element.Add(new XElement("title", part.Title));

        if(!string.IsNullOrWhiteSpace(part.BaseHref))
        {
            Element.Add(new XElement("base", new XAttribute("href", part.BaseHref)));
        }

        foreach(var link in part.Links)
        {
            Visit(link);

            Element.Add(_elements.Pop());
        }

        return part;
    }

    protected internal override MPart VisitDocumentLink(MDocumentLink part)
    {
        PushElement("link", part);

        SetAttribute("href", part.Href);
        SetAttribute("hrefLanguage", part.HrefLanguage);
        SetAttribute("rel", part.Relationship);
        SetAttribute("type", part.ContentType);

        return part;
    }

    protected internal override MPart VisitFooter(MElement part) =>
        PushElement("footer", part);

    protected internal override MPart VisitForm(MForm part)
    {
        PushElement("form", part);

        SetAttribute("name", part.Name);
        SetAttribute("method", part.Method);
        SetAttribute("action", part.Action);
        SetAttribute("enctype", part.EncodingType);
        SetAttribute("rel", part.Relationship);

        return part;
    }

    protected internal override MPart VisitGlobals(MGlobals part)
    {
        Visit(part.Classes);
        Visit(part.Data);

        if(part.Content is not null)
        {
            Visit(part.Content);
        }

        return part;
    }

    protected internal override MPart VisitHeader(MElement part) =>
        PushElement("header", part);

    protected internal override MPart VisitHeading1(MElement part) =>
        PushElement("h1", part);

    protected internal override MPart VisitHeading2(MElement part) =>
        PushElement("h2", part);

    protected internal override MPart VisitHeading3(MElement part) =>
        PushElement("h3", part);

    protected internal override MPart VisitHeading4(MElement part) =>
        PushElement("h4", part);

    protected internal override MPart VisitHeading5(MElement part) =>
        PushElement("h5", part);

    protected internal override MPart VisitHeading6(MElement part) =>
        PushElement("h6", part);

    protected internal override MPart VisitLink(MLink part)
    {
        PushElement("a", part);

        SetAttribute("href", part.Href);
        SetAttribute("hrefLanguage", part.HrefLanguage);
        SetAttribute("contentType", part.ContentType);
        SetAttribute("relationship", part.Relationship);
        SetAttribute("download", part.Download);

        return part;
    }

    protected internal override MPart VisitList(MElement part) =>
        PushElement("header", part);

    protected internal override MPart VisitListItem(MElement part) =>
        PushElement("header", part);

    protected internal override MPart VisitLiteral(MElement part) =>
        PushElement("pre", part);

    protected internal override MPart VisitMain(MElement part) =>
        PushElement("main", part);

    protected internal override MPart VisitNavigation(MElement part) =>
        PushElement("nav", part);

    protected internal override MPart VisitParagraph(MElement part) =>
        PushElement("p", part);

    protected internal override MPart VisitQuotation(MQuotation part)
    {
        PushElement("blockquote", part);

        SetAttribute("cite", part.Cite);

        return part;
    }

    protected internal override MPart VisitSection(MElement part) =>
        PushElement("section", part);

    protected internal override MPart VisitTime(MTime part)
    {
        PushElement("blockquote", part);

        SetAttribute("datetime", part.DateTime);

        return part;
    }

    MPart PushElement(string name, MElement part)
    {
        _elements.Push(new XElement(name));

        Visit(part.Globals);

        return part;
    }

    void SetAttribute(string name, string value)
    {
        if(!string.IsNullOrEmpty(value))
        {
            Element.SetAttributeValue(name, value);
        }
    }
}
