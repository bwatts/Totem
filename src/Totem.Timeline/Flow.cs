using System;
using System.Threading.Tasks;
using Totem.Runtime;

namespace Totem.Timeline
{
  /// <summary>
  /// A timeline presence observing and reacting to events
  /// </summary>
  [Durable]
  public abstract class Flow : Notion
  {
    [Transient] public FlowContext Context { get; internal set; }
    [Transient] public Id Id { get; internal set; }

    public override string ToString() =>
      Context.ToString();

    protected void ThenDone() =>
      Context.Done = true;

    protected Task<User> ReadUser() =>
      Context.ReadUser();

    protected async Task<TResult> ReadUser<TResult>(Func<User, TResult> selectResult) =>
      selectResult(await ReadUser());

    protected Task<Id> ReadUserId() =>
      ReadUser(c => c.Id);

    protected Task<string> ReadUserName() =>
      ReadUser(c => c.Name);
  }
}