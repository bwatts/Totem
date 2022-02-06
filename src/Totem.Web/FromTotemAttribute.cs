using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Totem;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
public class FromTotemAttribute : Attribute, IBindingSourceMetadata
{
    public BindingSource BindingSource => TotemBindingSource.Totem;
}
