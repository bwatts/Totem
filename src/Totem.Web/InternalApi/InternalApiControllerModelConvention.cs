using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Totem.InternalApi;

internal class InternalApiControllerModelConvention : IControllerModelConvention
{
    public void Apply(ControllerModel controller)
    {
        if(controller is null)
            throw new ArgumentNullException(nameof(controller));

        if(controller.ControllerType.Assembly == Assembly.GetExecutingAssembly() && !controller.ControllerType.IsVisible)
        {
            var apiRootModel = new AttributeRouteModel(new RouteAttribute(InternalApiRoot.Path));

            foreach(var selector in controller.Selectors)
            {
                selector.AttributeRouteModel = selector.AttributeRouteModel is null
                    ? apiRootModel
                    : AttributeRouteModel.CombineAttributeRouteModel(apiRootModel, selector.AttributeRouteModel);
            }
        }
    }
}
