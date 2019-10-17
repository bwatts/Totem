using System.Collections.Generic;
using Totem.Timeline.Area;

namespace Totem.Timeline.StreamsDb
{
  public class ResumeProjectionState
  {
    public long Checkpoint { get; set; }
    public Dictionary<long, ResumeProjectionSchedule> Schedule { get; set; }
    public Dictionary<AreaTypeName, ResumeProjectInstance> SingleInstances { get; set; }
    public Dictionary<AreaTypeName, Dictionary<Id, ResumeProjectInstance>> MultiInstances { get; set; }

    public ResumeProjectionState()
    {
      Schedule = new Dictionary<long, ResumeProjectionSchedule>();
      SingleInstances = new Dictionary<AreaTypeName, ResumeProjectInstance>();
      MultiInstances = new Dictionary<AreaTypeName, Dictionary<Id, ResumeProjectInstance>>();
    }
  }
}