namespace Totem.Timeline.Runtime
{
  /// <summary>
  /// A set of .Given calls to be applied to a topic immediately after writing new events
  /// </summary>
  public class ImmediateGivens
  {
    public ImmediateGivens(Many<FlowCall.Given> calls)
    {
      Calls = calls;
    }

    public readonly Many<FlowCall.Given> Calls;
  }
}