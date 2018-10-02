using System.Linq;

namespace Totem.Timeline.Area
{
  /// <summary>
  /// The set of .When or .Given methods declared by a flow
  /// </summary>
  /// <typeparam name="T">The type of declared methods</typeparam>
  public class FlowMethodSet<T> where T : FlowMethod
  {
    public FlowMethodSet(Many<T> methods, Many<T> scheduledMethods)
    {
      Methods = methods;
      ScheduledMethods = scheduledMethods;
    }
      
    public FlowMethodSet() : this(new Many<T>(), new Many<T>())
    {}

    public readonly Many<T> Methods;
    public readonly Many<T> ScheduledMethods;

    public bool HasMethod(bool scheduled) =>
      GetMethods(scheduled).Any();

    public Many<T> GetMethods(bool scheduled) =>
      scheduled ? ScheduledMethods : Methods;
  }
}