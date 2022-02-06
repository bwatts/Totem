using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Totem.Http;
using Totem.Map;

namespace Totem.Hosting;

public class CommandControllerProvider : IApplicationFeatureProvider<ControllerFeature>
{
    readonly RuntimeMap _map;

    public CommandControllerProvider(RuntimeMap map) =>
        _map = map ?? throw new ArgumentNullException(nameof(map));

    public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
    {
        if(parts is null)
            throw new ArgumentNullException(nameof(parts));

        if(feature is null)
            throw new ArgumentNullException(nameof(feature));

        foreach(var controllerType in
            from command in _map.Commands
            from context in command.Contexts
            where context.Info is HttpCommandInfo
            select typeof(HttpCommandController<>).MakeGenericType(command.DeclaredType))
        {
            feature.Controllers.Add(controllerType.GetTypeInfo());
        }
    }
}
