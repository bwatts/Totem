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
    [Transient] Client _client;

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
        _client = null;
      }
		}

		protected void ThenDone()
		{
			Context.SetDone();
		}

    protected async Task<Client> ReadClient()
    {
      var whenCall = Call as FlowCall.When;

      Expect(whenCall).IsNotNull("Flow is not making a When call");

      if(_client == null)
      {
        IClientAuthority authority;

        _client = whenCall.Dependencies.TryResolve(out authority)
          ? await authority.Authenticate(Call.Point.ClientId)
          : new Client();
      }

      return _client;
    }

    protected async Task<TResult> ReadClient<TResult>(Func<Client, TResult> selectResult)
    {
      return selectResult(await ReadClient());
    }

    protected Task<Id> ReadClientId()
    {
      return ReadClient(c => c.Id);
    }

    protected Task<string> ReadClientName()
    {
      return ReadClient(c => c.Name);
    }

		public new static class Traits
		{
      public static readonly Tag<Id> RequestId = Tag.Declare(() => RequestId);
      public static readonly Tag<Id> ClientId = Tag.Declare(() => ClientId);

      public static void ForwardRequestId(Event source, Event target)
			{
        RequestId.Copy(source, target);
			}

      public static void ForwardClientId(Event source, Event target)
      {
        ClientId.Copy(source, target);
      }
    }
	}
}