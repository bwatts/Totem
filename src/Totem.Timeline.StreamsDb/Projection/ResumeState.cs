using StreamsDB.Driver;
using System.Collections.Generic;

namespace Totem.Timeline.StreamsDb
{
    public class ResumeState
    {
        public long Checkpoint { get; set; }
        public List<string> Routes { get; set; }
        public List<long> Schedule { get; set; }
    }

  public class HeaderWithOriginalStream<T>: T
  {
    public MessageInputWithOriginalStream(MessageInput messageInput)
    {
      ID = messageInput.ID;
      messageInput.
    }
  }
}