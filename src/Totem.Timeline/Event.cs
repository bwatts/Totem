using System;
using Totem.Runtime;

namespace Totem.Timeline
{
  /// <summary>
  /// A signal on the timeline of a distributed environment
  /// </summary>
  [Durable]
  public abstract class Event : Notion
  {
    protected Event()
    {
      When = Clock.Now;
    }

    [Transient]
    public DateTimeOffset When
    {
      get { return Traits.When.Get(this); }
      private set { Traits.When.Set(this, value); }
    }

    public static DateTimeOffset? GetWhenOccurs(Event e) =>
      Traits.WhenOccurs.Get(e);

    public static bool IsScheduled(Event e) =>
      GetWhenOccurs(e) != null;

    public new static class Traits
    {
      public static readonly Field<DateTimeOffset> When = Field.Declare(() => When);
      public static readonly Field<DateTimeOffset?> WhenOccurs = Field.Declare(() => WhenOccurs);
      public static readonly Field<Id> EventId = Field.Declare(() => EventId);
      public static readonly Field<Id> CommandId = Field.Declare(() => CommandId);
      public static readonly Field<Id> UserId = Field.Declare(() => UserId);
    }
  }
}