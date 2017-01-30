using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// A timeline presence observing and reacting to events
	/// </summary>
	[Durable]
	public abstract class Flow : Notion
	{
    [Transient] User _user;

    [Transient] public FlowContext Context { get; internal set; }
		[Transient] public FlowCall Call { get; internal set; }
    [Transient] public Id Id { get; internal set; }

		public override Text ToText() => Context.ToText();

		protected internal virtual Task CallWhen(FlowCall.When call)
		{
      try
      {
        return call.MakeInternal(this);
      }
      finally
      {
        _user = null;
      }
		}

		protected void ThenDone()
		{
			Context.SetDone();
		}

    protected async Task<User> ReadUser()
    {
      var whenCall = Call as FlowCall.When;

      Expect(whenCall).IsNotNull("Flow is not making a When call");

      if(_user == null)
      {
        IUserDb userDb;

        _user = whenCall.Dependencies.TryResolve(out userDb)
          ? await userDb.Authenticate(Call.Point.UserId)
          : new User();
      }

      return _user;
    }

    protected async Task<TResult> ReadUser<TResult>(Func<User, TResult> selectResult)
    {
      return selectResult(await ReadUser());
    }

    protected Task<Id> ReadUserId()
    {
      return ReadUser(c => c.Id);
    }

    protected Task<string> ReadUserName()
    {
      return ReadUser(c => c.Name);
    }

		public new static class Traits
		{
      public static readonly Tag<Id> RequestId = Tag.Declare(() => RequestId);
      public static readonly Tag<Id> UserId = Tag.Declare(() => UserId);

      public static void ForwardRequestId(Event source, Event target)
			{
        RequestId.Copy(source, target);
			}

      public static void ForwardUserId(Event source, Event target)
      {
        UserId.Copy(source, target);
      }
    }
	}
}