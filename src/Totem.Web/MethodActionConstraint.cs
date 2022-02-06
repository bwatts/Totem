using System;
using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace Totem;

public class MethodActionConstraint : IActionConstraint, IActionConstraintMetadata
{
    readonly string _method;

    public MethodActionConstraint(string method) =>
        _method = method ?? throw new ArgumentNullException(nameof(method));

    public int Order => 100;

    public virtual bool Accept(ActionConstraintContext context)
    {
        if(context is null)
            throw new ArgumentNullException(nameof(context));

        if(string.IsNullOrWhiteSpace(_method))
        {
            return true;
        }

        return context.RouteContext.HttpContext.Request.Method.Equals(_method, StringComparison.OrdinalIgnoreCase);
    }
}
