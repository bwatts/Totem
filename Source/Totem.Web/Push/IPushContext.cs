using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Totem.Web.Push
{
	/// <summary>
	/// Describes the context of the push hub in the current runtime
	/// </summary>
	public interface IPushContext
	{
		Task AddToGroup(Id connectionId, Id groupId);
		Task RemoveFromGroup(Id connectionId, Id groupId);

		Task ToAll(Event e);
		Task ToAllExcept(Event e, Many<Id> excludeConnectionIds);

		Task ToClient(Event e, Id connectionId);
		Task ToClients(Event e, Many<Id> connectionIds);

		Task ToGroup(Event e, Id groupId, Many<Id> excludeConnectionIds);
		Task ToGroups(Event e, Many<Id> groupIds, Many<Id> excludeConnectionIds);

		Task ToUser(Event e, Id userId);
		Task ToUsers(Event e, Many<Id> userIds);
	}
}