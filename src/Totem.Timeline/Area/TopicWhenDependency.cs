using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Totem.Timeline.Area
{
  /// <summary>
  /// A parameter to a .When method representing a service resolved at call time
  /// </summary>
  public sealed class TopicWhenDependency
  {
    internal TopicWhenDependency(ParameterInfo parameter)
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