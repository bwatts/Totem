using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Totem;

public class TotemModelBinder : IModelBinder
{
    readonly IModelBinder _bodyBinder;
    readonly IModelBinder _complexObjectBinder;

    public TotemModelBinder(IModelBinder bodyBinder, IModelBinder complexObjectBinder)
    {
        _bodyBinder = bodyBinder ?? throw new ArgumentNullException(nameof(bodyBinder));
        _complexObjectBinder = complexObjectBinder ?? throw new ArgumentNullException(nameof(complexObjectBinder));
    }

    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if(bindingContext is null)
            throw new ArgumentNullException(nameof(bindingContext));

        await TryBindBody(bindingContext);
        await _complexObjectBinder.BindModelAsync(bindingContext);
    }

    async Task TryBindBody(ModelBindingContext bindingContext)
    {
        if(HasAnyMethod(HttpMethod.Post, HttpMethod.Put, HttpMethod.Delete))
        {
            await _bodyBinder.BindModelAsync(bindingContext);

            if(bindingContext.Result.IsModelSet && bindingContext.Result.Model is not null)
            {
                bindingContext.Model = bindingContext.Result.Model;
            }
        }

        bool HasAnyMethod(params HttpMethod[] methods) =>
            methods.Any(method =>
                bindingContext.HttpContext.Request.Method.Equals(method.Method, StringComparison.OrdinalIgnoreCase));
    }
}
