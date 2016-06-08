using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Totem.Web.Push
{
	/// <summary>
	/// Describes a channel pushing events to remote collaborators
	/// </summary>
	public interface IPushChannel
	{
		Task AddToGroup(Id connectionId, Id groupId);
		Task RemoveFromGroup(Id connectionId, Id groupId);

		Task PushToAll(Event e);
		Task PushToAllExcept(Event e, Many<Id> excludeConnectionIds);

		Task PushToClient(Event e, Id connectionId);
		Task PushToClients(Event e, Many<Id> connectionIds);

		Task PushToGroup(Event e, Id groupId, Many<Id> excludeConnectionIds);
		Task PushToGroups(Event e, Many<Id> groupIds, Many<Id> excludeConnectionIds);

		Task PushToUser(Event e, Id userId);
		Task PushToUsers(Event e, Many<Id> userIds);
	}
}