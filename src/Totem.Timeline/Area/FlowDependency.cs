using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Totem.Timeline.Area
{
  /// <summary>
  /// A method parameter representing a service resolved at call time
  /// </summary>
  public class FlowDependency
  {
    public FlowDependency(ParameterInfo parameter)
    {
      Parameter = parameter;
    }

    public readonly ParameterInfo Parameter;

    public Expression ResolveIn(Expression services) =>
      Expression.Call(
        typeof(ServiceProviderServiceExtensions),
        "GetRequiredService",
        new[] { Parameter.ParameterType },
        services);
  }
}