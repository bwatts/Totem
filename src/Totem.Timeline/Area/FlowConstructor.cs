using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Totem.Timeline.Area
{
  /// <summary>
  /// Initiates the lifecycle of a <see cref="Flow"/>
  /// </summary>
  public sealed class FlowConstructor
  {
    readonly Lazy<Func<Flow>> _call;

    internal FlowConstructor(ConstructorInfo info)
    {
      Info = info;

      _call = new Lazy<Func<Flow>>(CompileCall);
    }

    public readonly ConstructorInfo Info;

    public override string ToString() =>
      Info.ToString();

    public Flow Call() =>
      _call.Value();

    Func<Flow> CompileCall()
    {
      // () => Info()

      var call = Expression.New(Info);

      var lambda = Expression.Lambda<Func<Flow>>(call);

      return lambda.Compile();
    }
  }
}