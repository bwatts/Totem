using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Totem.Http;

namespace Totem
{
    public class RequestControllerModelConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            if(controller == null)
                throw new ArgumentNullException(nameof(controller));

            if(TryGetInfo(controller.ControllerType, out var method, out var route))
            {
                var selector = controller.Selectors.Single();
                selector.ActionConstraints.Add(new MethodActionConstraint(method));
                selector.AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(route) { Order = 1 });
            }
        }

        static bool TryGetInfo(Type controllerType, out string method, out string route)
        {
            if(controllerType.IsGenericType)
            {
                var definition = controllerType.GetGenericTypeDefinition();

                if(definition == typeof(CommandController<>))
                {
                    var info = HttpCommandInfo.From(controllerType.GetGenericArguments().First());

                    method = info.Request.Method;
                    route = info.Request.Route;
                    return true;
                }

                if(definition == typeof(QueryController<>))
                {
                    var info = HttpQueryInfo.From(controllerType.GetGenericArguments().First());

                    method = info.Request.Method;
                    route = info.Request.Route;
                    return true;
                }
            }

            method = null!;
            route = null!;
            return false;
        }
    }
}