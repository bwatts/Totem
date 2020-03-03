namespace Totem.Timeline.StreamsDb
{
    public class ResumeProjectInstance
    {
        public long? Latest { get; set; }
        public long? Checkpoint { get; set; }
        public bool IsStopped { get; set; }
    }
}