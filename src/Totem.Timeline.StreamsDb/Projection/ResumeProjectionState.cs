using System.Collections.Generic;
using Totem.Timeline.Area;

namespace Totem.Timeline.StreamsDb
{
  public class ResumeProjectionState
  {
    public Dictionary<string, long> Checkpoints { get; }
    public Dictionary<long, ResumeProjectionSchedule> Schedule { get; }
    public Dictionary<AreaTypeName, ResumeProjectInstance> SingleInstances { get; }
    public Dictionary<AreaTypeName, Dictionary<Id, ResumeProjectInstance>> MultiInstances { get; }

    public ResumeProjectionState(Dictionary<string, long> checkpoints)
    {
      Checkpoints = checkpoints;
      Schedule = new Dictionary<long, ResumeProjectionSchedule>();
      SingleInstances = new Dictionary<AreaTypeName, ResumeProjectInstance>();
      MultiInstances = new Dictionary<AreaTypeName, Dictionary<Id, ResumeProjectInstance>>();
    }
  }
}