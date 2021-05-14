using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace Totem
{
    public class TotemModelBinderProvider : IModelBinderProvider
    {
        readonly BodyModelBinderProvider _bodyProvider;
        readonly ComplexObjectModelBinderProvider _complexObjectProvider;

        public TotemModelBinderProvider(BodyModelBinderProvider bodyProvider, ComplexObjectModelBinderProvider complexObjectProvider)
        {
            _bodyProvider = bodyProvider ?? throw new ArgumentNullException(nameof(bodyProvider));
            _complexObjectProvider = complexObjectProvider ?? throw new ArgumentNullException(nameof(complexObjectProvider));
        }

        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if(context == null)
                throw new ArgumentNullException(nameof(context));

            var bodyBinder = _bodyProvider.GetBinder(context);
            var complexObjectBinder = _complexObjectProvider.GetBinder(context);

            var source = context.BindingInfo.BindingSource;

            return source != null && source.CanAcceptDataFrom(TotemBindingSource.Totem)
                ? new TotemModelBinder(bodyBinder, complexObjectBinder)
                : null!;
        }
    }
}