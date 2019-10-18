using StreamsDB.Driver;
using System.Collections.Generic;

namespace Totem.Timeline.StreamsDb
{
    public class ResumeState
    {
        public Dictionary<string, long> Checkpoints { get; set; }
        public List<string> Routes { get; set; }
        public List<long> Schedule { get; set; }
    } 
}