using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Totem;

public class TotemBindingSource : BindingSource
{
    public TotemBindingSource(string id, string displayName, bool isGreedy, bool isFromRequest)
        : base(id, displayName, isGreedy, isFromRequest)
    { }

    public override bool CanAcceptDataFrom(BindingSource bindingSource) =>
        bindingSource == Body || bindingSource == Query || bindingSource == this;

    public static readonly BindingSource Totem = new TotemBindingSource("Totem", "Totem", isGreedy: true, isFromRequest: true);
}
